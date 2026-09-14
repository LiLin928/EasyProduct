using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using EasyProduct.Business.Workflow.Helpers;
using EasyProduct.Common.Base;
using EasyProduct.Common.Error;
using EasyProduct.Models.Constants;
using EasyProduct.Models.Dto.Workflow.Instance;
using EasyProduct.Models.Entitys.Workflow;
using Mapster;
using Microsoft.Extensions.Logging;
using SqlSugar;

namespace EasyProduct.Business.Workflow
{
    /// <summary>
    /// 流程实例服务实现
    /// </summary>
    public class WorkflowInstanceService : BaseService, IWorkflowInstanceService
    {
        private readonly ILogger<WorkflowInstanceService> _logger;
        private readonly IWorkflowTaskService _taskService;

        public WorkflowInstanceService(
            ILogger<WorkflowInstanceService> logger,
            IWorkflowTaskService taskService)
        {
            _logger = logger;
            _taskService = taskService;
        }

        /// <summary>
        /// 获取流程实例列表（分页）
        /// </summary>
        /// <param name="query">查询参数</param>
        /// <returns>流程实例分页结果</returns>
        public async Task<PageResponse<InstanceDto>> GetListAsync(InstanceQuery query)
        {
            var queryable = _db.Queryable<WfInstance>()
                .WhereIF(!string.IsNullOrEmpty(query.Keyword), x => x.Title.Contains(query.Keyword))
                .WhereIF(!string.IsNullOrEmpty(query.BusinessType), x => x.BusinessType == query.BusinessType)
                .WhereIF(query.Status.HasValue, x => x.Status == query.Status)
                .WhereIF(query.ApplicantId.HasValue, x => x.ApplicantId == query.ApplicantId)
                .OrderBy(x => x.StartTime, OrderByType.Desc);

            var totalCount = 0;
            var list = await queryable.ToPageListAsync(query.PageIndex, query.PageSize, totalCount);
            totalCount = queryable.Count();

            return new PageResponse<InstanceDto>
            {
                List = list.Adapt<List<InstanceDto>>(),
                Total = totalCount
            };
        }

        /// <summary>
        /// 获取流程实例详情
        /// </summary>
        /// <param name="id">流程实例ID</param>
        /// <returns>流程实例详情</returns>
        public async Task<InstanceDto> GetByIdAsync(Guid id)
        {
            var entity = await _db.Queryable<WfInstance>()
                .FirstAsync(x => x.Id == id);

            if (entity == null)
            {
                throw BusinessException.NotFound("流程实例不存在");
            }

            return entity.Adapt<InstanceDto>();
        }

        /// <summary>
        /// 发起流程
        /// </summary>
        /// <param name="dto">发起流程参数</param>
        /// <param name="applicantId">申请人ID</param>
        /// <param name="applicantName">申请人姓名</param>
        /// <returns>流程实例ID</returns>
        public async Task<string> StartAsync(StartWorkflowDto dto, Guid applicantId, string applicantName)
        {
            // 获取流程定义
            var definition = await _db.Queryable<WfDefinition>()
                .FirstAsync(x => x.Id == Guid.Parse(dto.DefinitionId) && x.Status == WorkflowConstants.DefinitionStatus.PUBLISHED);

            if (definition == null)
            {
                throw BusinessException.NotFound("流程定义不存在或未发布");
            }

            // 检查业务单据是否已发起流程
            var exists = await _db.Queryable<WfInstance>()
                .AnyAsync(x => x.BusinessKey == dto.BusinessKey && x.Status == WorkflowConstants.InstanceStatus.RUNNING);

            if (exists)
            {
                throw BusinessException.BadRequest($"业务单据 {dto.BusinessKey} 已存在正在运行的流程");
            }

            // 获取流程节点
            var nodes = JsonSerializer.Deserialize<List<WfNode>>(definition.Nodes) ?? new List<WfNode>();
            var startNode = nodes.FirstOrDefault(x => x.Type == WorkflowConstants.NodeType.START);
            var firstTaskNode = nodes.FirstOrDefault(x => x.Type == WorkflowConstants.NodeType.TASK);

            if (startNode == null || firstTaskNode == null)
            {
                throw BusinessException.BadRequest("流程定义异常，缺少开始节点或任务节点");
            }

            // 创建流程实例
            var instance = new WfInstance
            {
                DefinitionId = definition.Id,
                DefinitionName = definition.Name,
                BusinessKey = dto.BusinessKey,
                BusinessType = dto.BusinessType,
                Title = dto.Title,
                ApplicantId = applicantId,
                ApplicantName = applicantName,
                CurrentNodeId = firstTaskNode.Id,
                CurrentNodeName = firstTaskNode.Name,
                Status = WorkflowConstants.InstanceStatus.RUNNING,
                StartTime = DateTime.Now
            };

            // 创建第一个任务
            var task = new WfTask
            {
                InstanceId = instance.Id,
                NodeId = firstTaskNode.Id,
                NodeName = firstTaskNode.Name,
                AssigneeId = await GetAssigneeId(firstTaskNode),
                AssigneeName = await GetAssigneeName(firstTaskNode),
                Status = WorkflowConstants.TaskStatus.PENDING
            };

            _db.Ado.BeginTran();
            try
            {
                await _db.Insertable(instance).ExecuteCommandAsync();
                await _db.Insertable(task).ExecuteCommandAsync();
                _db.Ado.CommitTran();

                _logger.LogInformation("发起流程成功，实例ID: {InstanceId}, 流程: {DefinitionName}", instance.Id, definition.Name);

                return instance.Id.ToString();
            }
            catch (Exception ex)
            {
                _db.Ado.RollbackTran();
                _logger.LogError(ex, "发起流程失败，流程定义ID: {DefinitionId}", definition.Id);
                throw;
            }
        }

        /// <summary>
        /// 撤销流程
        /// </summary>
        /// <param name="id">流程实例ID</param>
        /// <param name="applicantId">申请人ID</param>
        /// <returns>是否成功</returns>
        public async Task<bool> CancelAsync(Guid id, Guid applicantId)
        {
            var instance = await _db.Queryable<WfInstance>()
                .FirstAsync(x => x.Id == id);

            if (instance == null)
            {
                throw BusinessException.NotFound("流程实例不存在");
            }

            // 只有申请人可以撤销
            if (instance.ApplicantId != applicantId)
            {
                throw BusinessException.Forbidden("只有申请人可以撤销流程");
            }

            // 只有运行中的流程可以撤销
            if (instance.Status != WorkflowConstants.InstanceStatus.RUNNING)
            {
                throw BusinessException.BadRequest("只有运行中的流程可以撤销");
            }

            instance.Status = WorkflowConstants.InstanceStatus.CANCELLED;
            instance.EndTime = DateTime.Now;

            // 取消所有待办任务
            var tasks = await _db.Queryable<WfTask>()
                .Where(x => x.InstanceId == id && x.Status == WorkflowConstants.TaskStatus.PENDING)
                .ToListAsync();

            foreach (var task in tasks)
            {
                task.Status = WorkflowConstants.TaskStatus.CANCELLED;
                task.CompletedAt = DateTime.Now;
            }

            _db.Ado.BeginTran();
            try
            {
                await _db.Updateable(instance).ExecuteCommandAsync();
                await _db.Updateable(tasks).ExecuteCommandAsync();
                _db.Ado.CommitTran();

                _logger.LogInformation("撤销流程成功，实例ID: {InstanceId}", id);

                return true;
            }
            catch (Exception ex)
            {
                _db.Ado.RollbackTran();
                _logger.LogError(ex, "撤销流程失败，实例ID: {InstanceId}", id);
                throw;
            }
        }

        /// <summary>
        /// 获取审批人ID
        /// </summary>
        /// <param name="node">流程节点</param>
        /// <returns>审批人ID</returns>
        private async Task<Guid> GetAssigneeId(WfNode node)
        {
            // TODO: 根据分配类型获取审批人
            // user: 直接返回用户ID
            // role: 根据角色ID查询用户
            // dept: 根据部门ID查询部门负责人
            // expression: 解析表达式获取用户
            return Guid.Parse(node.AssigneeId);
        }

        /// <summary>
        /// 获取审批人姓名
        /// </summary>
        /// <param name="node">流程节点</param>
        /// <returns>审批人姓名</returns>
        private async Task<string> GetAssigneeName(WfNode node)
        {
            // TODO: 根据审批人ID查询用户姓名
            return node.Name;
        }
    }
}