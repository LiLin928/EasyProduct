using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EasyProduct.Common.Base;
using EasyProduct.Common.Error;
using EasyProduct.Models.Constants;
using EasyProduct.Models.Dto.Workflow.Instance;
using EasyProduct.Models.Dto.Workflow.Task;
using EasyProduct.Models.Entitys.Workflow;
using Mapster;
using Microsoft.Extensions.Logging;
using SqlSugar;

namespace EasyProduct.Business.Workflow
{
    /// <summary>
    /// 流程任务服务实现
    /// </summary>
    public class WorkflowTaskService : BaseService, IWorkflowTaskService
    {
        private readonly ILogger<WorkflowTaskService> _logger;
        private readonly IWorkflowInstanceService _instanceService;

        public WorkflowTaskService(
            ILogger<WorkflowTaskService> logger,
            IWorkflowInstanceService instanceService)
        {
            _logger = logger;
            _instanceService = instanceService;
        }

        /// <summary>
        /// 获取待办任务列表（分页）
        /// </summary>
        /// <param name="query">查询参数</param>
        /// <param name="assigneeId">受托人ID</param>
        /// <returns>任务分页结果</returns>
        public async Task<PageResponse<TaskDto>> GetTodoListAsync(TaskQuery query, Guid assigneeId)
        {
            var queryable = _db.Queryable<WfTask>()
                .Where(x => x.AssigneeId == assigneeId && x.Status == WorkflowConstants.TaskStatus.PENDING)
                .WhereIF(query.Status.HasValue, x => x.Status == query.Status)
                .OrderBy(x => x.CreatedAt, OrderByType.Desc);

            var totalCount = 0;
            var list = await queryable.ToPageListAsync(query.PageIndex, query.PageSize, totalCount);
            totalCount = queryable.Count();

            var dtoList = list.Adapt<List<TaskDto>>();

            return new PageResponse<TaskDto>
            {
                List = dtoList,
                Total = totalCount
            };
        }

        /// <summary>
        /// 获取已办任务列表（分页）
        /// </summary>
        /// <param name="query">查询参数</param>
        /// <param name="assigneeId">受托人ID</param>
        /// <returns>任务分页结果</returns>
        public async Task<PageResponse<TaskDto>> GetDoneListAsync(TaskQuery query, Guid assigneeId)
        {
            var queryable = _db.Queryable<WfTask>()
                .Where(x => x.AssigneeId == assigneeId && x.Status != WorkflowConstants.TaskStatus.PENDING)
                .WhereIF(query.Status.HasValue, x => x.Status == query.Status)
                .OrderBy(x => x.CompletedAt, OrderByType.Desc);

            var totalCount = 0;
            var list = await queryable.ToPageListAsync(query.PageIndex, query.PageSize, totalCount);
            totalCount = queryable.Count();

            var dtoList = list.Adapt<List<TaskDto>>();

            return new PageResponse<TaskDto>
            {
                List = dtoList,
                Total = totalCount
            };
        }

        /// <summary>
        /// 获取任务详情
        /// </summary>
        /// <param name="id">任务ID</param>
        /// <returns>任务详情</returns>
        public async Task<TaskDto> GetByIdAsync(Guid id)
        {
            var entity = await _db.Queryable<WfTask>()
                .FirstAsync(x => x.Id == id);

            if (entity == null)
            {
                throw BusinessException.NotFound("任务不存在");
            }

            var dto = entity.Adapt<TaskDto>();

            // 获取流程实例信息
            var instance = await _db.Queryable<WfInstance>()
                .FirstAsync(x => x.Id == entity.InstanceId);

            if (instance != null)
            {
                dto.Instance = instance.Adapt<InstanceDto>();
            }

            return dto;
        }

        /// <summary>
        /// 审批通过
        /// </summary>
        /// <param name="id">任务ID</param>
        /// <param name="dto">审批参数</param>
        /// <param name="assigneeId">受托人ID</param>
        /// <returns>是否成功</returns>
        public async Task<bool> ApproveAsync(Guid id, ApproveDto dto, Guid assigneeId)
        {
            var task = await _db.Queryable<WfTask>()
                .FirstAsync(x => x.Id == id);

            if (task == null)
            {
                throw BusinessException.NotFound("任务不存在");
            }

            // 验证权限
            if (task.AssigneeId != assigneeId)
            {
                throw BusinessException.Forbidden("无权处理此任务");
            }

            // 验证状态
            if (task.Status != WorkflowConstants.TaskStatus.PENDING)
            {
                throw BusinessException.BadRequest("任务已处理");
            }

            // 更新任务状态
            task.Status = WorkflowConstants.TaskStatus.APPROVED;
            task.Comment = dto.Comment;
            task.CompletedAt = DateTime.Now;

            await _db.Updateable(task).ExecuteCommandAsync();

            _logger.LogInformation("审批通过，任务ID: {TaskId}", id);

            return true;
        }

        /// <summary>
        /// 审批拒绝
        /// </summary>
        /// <param name="id">任务ID</param>
        /// <param name="dto">拒绝参数</param>
        /// <param name="assigneeId">受托人ID</param>
        /// <returns>是否成功</returns>
        public async Task<bool> RejectAsync(Guid id, RejectDto dto, Guid assigneeId)
        {
            var task = await _db.Queryable<WfTask>()
                .FirstAsync(x => x.Id == id);

            if (task == null)
            {
                throw BusinessException.NotFound("任务不存在");
            }

            // 验证权限
            if (task.AssigneeId != assigneeId)
            {
                throw BusinessException.Forbidden("无权处理此任务");
            }

            // 验证状态
            if (task.Status != WorkflowConstants.TaskStatus.PENDING)
            {
                throw BusinessException.BadRequest("任务已处理");
            }

            // 更新任务状态
            task.Status = WorkflowConstants.TaskStatus.REJECTED;
            task.Comment = dto.Comment;
            task.CompletedAt = DateTime.Now;

            // 更新流程实例状态
            var instance = await _db.Queryable<WfInstance>()
                .FirstAsync(x => x.Id == task.InstanceId);

            if (instance != null)
            {
                instance.Status = WorkflowConstants.InstanceStatus.REJECTED;
                instance.EndTime = DateTime.Now;
            }

            _db.Ado.BeginTran();
            try
            {
                await _db.Updateable(task).ExecuteCommandAsync();
                if (instance != null)
                {
                    await _db.Updateable(instance).ExecuteCommandAsync();
                }
                _db.Ado.CommitTran();

                _logger.LogInformation("审批拒绝，任务ID: {TaskId}", id);

                return true;
            }
            catch (Exception ex)
            {
                _db.Ado.RollbackTran();
                _logger.LogError(ex, "审批拒绝失败，任务ID: {TaskId}", id);
                throw;
            }
        }

        /// <summary>
        /// 委托任务
        /// </summary>
        /// <param name="id">任务ID</param>
        /// <param name="dto">委托参数</param>
        /// <param name="delegatorId">委托人ID</param>
        /// <returns>是否成功</returns>
        public async Task<bool> DelegateAsync(Guid id, DelegateDto dto, Guid delegatorId)
        {
            var task = await _db.Queryable<WfTask>()
                .FirstAsync(x => x.Id == id);

            if (task == null)
            {
                throw BusinessException.NotFound("任务不存在");
            }

            // 验证权限
            if (task.AssigneeId != delegatorId)
            {
                throw BusinessException.Forbidden("无权委托此任务");
            }

            // 验证状态
            if (task.Status != WorkflowConstants.TaskStatus.PENDING)
            {
                throw BusinessException.BadRequest("任务已处理");
            }

            // 创建新任务
            var newTask = new WfTask
            {
                InstanceId = task.InstanceId,
                NodeId = task.NodeId,
                NodeName = task.NodeName,
                AssigneeId = Guid.Parse(dto.AssigneeId),
                AssigneeName = "", // TODO: 查询用户姓名
                Status = WorkflowConstants.TaskStatus.PENDING,
                DelegatorId = delegatorId,
                DelegatorName = task.AssigneeName
            };

            // 更新原任务状态
            task.Status = WorkflowConstants.TaskStatus.DELEGATED;
            task.Comment = dto.Comment;
            task.CompletedAt = DateTime.Now;

            _db.Ado.BeginTran();
            try
            {
                await _db.Insertable(newTask).ExecuteCommandAsync();
                await _db.Updateable(task).ExecuteCommandAsync();
                _db.Ado.CommitTran();

                _logger.LogInformation("委托任务成功，原任务ID: {TaskId}, 新任务ID: {NewTaskId}", id, newTask.Id);

                return true;
            }
            catch (Exception ex)
            {
                _db.Ado.RollbackTran();
                _logger.LogError(ex, "委托任务失败，任务ID: {TaskId}", id);
                throw;
            }
        }
    }
}