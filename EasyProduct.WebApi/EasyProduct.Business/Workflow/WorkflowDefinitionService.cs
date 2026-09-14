using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using EasyProduct.Business.Workflow.Helpers;
using EasyProduct.Common.Base;
using EasyProduct.Common.Error;
using EasyProduct.Models.Constants;
using EasyProduct.Models.Dto.Workflow.Definition;
using EasyProduct.Models.Entitys.Workflow;
using Mapster;
using Microsoft.Extensions.Logging;
using SqlSugar;

namespace EasyProduct.Business.Workflow
{
    /// <summary>
    /// 流程定义服务实现
    /// </summary>
    public class WorkflowDefinitionService : BaseService, IWorkflowDefinitionService
    {
        private readonly ILogger<WorkflowDefinitionService> _logger;

        public WorkflowDefinitionService(ILogger<WorkflowDefinitionService> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 获取流程定义列表（分页）
        /// </summary>
        /// <param name="query">查询参数</param>
        /// <returns>流程定义分页结果</returns>
        public async Task<PageResponse<DefinitionDto>> GetListAsync(DefinitionQuery query)
        {
            var queryable = _db.Queryable<WfDefinition>()
                .WhereIF(!string.IsNullOrEmpty(query.Keyword), x => x.Code.Contains(query.Keyword) || x.Name.Contains(query.Keyword))
                .WhereIF(!string.IsNullOrEmpty(query.Category), x => x.Category == query.Category)
                .WhereIF(query.Status.HasValue, x => x.Status == query.Status)
                .OrderBy(x => x.CreatedAt, OrderByType.Desc);

            var totalCount = 0;
            var list = await queryable.ToPageListAsync(query.PageIndex, query.PageSize, totalCount);
            totalCount = queryable.Count();

            var dtoList = list.Select(x =>
            {
                var dto = x.Adapt<DefinitionDto>();
                dto.Nodes = JsonSerializer.Deserialize<List<WfNode>>(x.Nodes) ?? new List<WfNode>();
                return dto;
            }).ToList();

            return new PageResponse<DefinitionDto>
            {
                List = dtoList,
                Total = totalCount
            };
        }

        /// <summary>
        /// 获取流程定义详情
        /// </summary>
        /// <param name="id">流程定义ID</param>
        /// <returns>流程定义详情</returns>
        public async Task<DefinitionDto> GetByIdAsync(Guid id)
        {
            var entity = await _db.Queryable<WfDefinition>()
                .FirstAsync(x => x.Id == id);

            if (entity == null)
            {
                throw BusinessException.NotFound("流程定义不存在");
            }

            var dto = entity.Adapt<DefinitionDto>();
            dto.Nodes = JsonSerializer.Deserialize<List<WfNode>>(entity.Nodes) ?? new List<WfNode>();
            return dto;
        }

        /// <summary>
        /// 创建流程定义
        /// </summary>
        /// <param name="dto">创建参数</param>
        /// <returns>新流程定义ID</returns>
        public async Task<string> CreateAsync(DefinitionCreateDto dto)
        {
            // 检查编码是否重复
            var exists = await _db.Queryable<WfDefinition>()
                .AnyAsync(x => x.Code == dto.Code && x.IsDeleted == 0);

            if (exists)
            {
                throw BusinessException.BadRequest($"流程编码 {dto.Code} 已存在");
            }

            // 验证流程节点
            ValidateNodes(dto.Nodes);

            var entity = dto.Adapt<WfDefinition>();
            entity.Nodes = JsonSerializer.Serialize(dto.Nodes);
            entity.Status = WorkflowConstants.DefinitionStatus.DRAFT;

            await _db.Insertable(entity).ExecuteCommandAsync();

            _logger.LogInformation("创建流程定义成功，ID: {Id}, Code: {Code}", entity.Id, entity.Code);

            return entity.Id.ToString();
        }

        /// <summary>
        /// 更新流程定义
        /// </summary>
        /// <param name="id">流程定义ID</param>
        /// <param name="dto">更新参数</param>
        /// <returns>是否成功</returns>
        public async Task<bool> UpdateAsync(Guid id, DefinitionUpdateDto dto)
        {
            var entity = await _db.Queryable<WfDefinition>()
                .FirstAsync(x => x.Id == id);

            if (entity == null)
            {
                throw BusinessException.NotFound("流程定义不存在");
            }

            // 已发布的流程不允许修改
            if (entity.Status == WorkflowConstants.DefinitionStatus.PUBLISHED)
            {
                throw BusinessException.BadRequest("已发布的流程不允许修改，请新建版本");
            }

            // 验证流程节点
            ValidateNodes(dto.Nodes);

            entity.Name = dto.Name;
            entity.Category = dto.Category;
            entity.Description = dto.Description;
            entity.Nodes = JsonSerializer.Serialize(dto.Nodes);

            await _db.Updateable(entity).ExecuteCommandAsync();

            _logger.LogInformation("更新流程定义成功，ID: {Id}", id);

            return true;
        }

        /// <summary>
        /// 删除流程定义
        /// </summary>
        /// <param name="id">流程定义ID</param>
        /// <returns>是否成功</returns>
        public async Task<bool> DeleteAsync(Guid id)
        {
            var entity = await _db.Queryable<WfDefinition>()
                .FirstAsync(x => x.Id == id);

            if (entity == null)
            {
                throw BusinessException.NotFound("流程定义不存在");
            }

            // 检查是否有正在运行的流程实例
            var hasRunningInstance = await _db.Queryable<WfInstance>()
                .AnyAsync(x => x.DefinitionId == id && x.Status == WorkflowConstants.InstanceStatus.RUNNING);

            if (hasRunningInstance)
            {
                throw BusinessException.BadRequest("该流程定义下存在正在运行的流程实例，无法删除");
            }

            entity.IsDeleted = 1;
            await _db.Updateable(entity).ExecuteCommandAsync();

            _logger.LogInformation("删除流程定义成功，ID: {Id}", id);

            return true;
        }

        /// <summary>
        /// 发布流程定义
        /// </summary>
        /// <param name="id">流程定义ID</param>
        /// <returns>是否成功</returns>
        public async Task<bool> PublishAsync(Guid id)
        {
            var entity = await _db.Queryable<WfDefinition>()
                .FirstAsync(x => x.Id == id);

            if (entity == null)
            {
                throw BusinessException.NotFound("流程定义不存在");
            }

            // 验证流程节点
            var nodes = JsonSerializer.Deserialize<List<WfNode>>(entity.Nodes) ?? new List<WfNode>();
            ValidateNodes(nodes);

            entity.Status = WorkflowConstants.DefinitionStatus.PUBLISHED;
            await _db.Updateable(entity).ExecuteCommandAsync();

            _logger.LogInformation("发布流程定义成功，ID: {Id}", id);

            return true;
        }

        /// <summary>
        /// 验证流程节点
        /// </summary>
        /// <param name="nodes">节点列表</param>
        private void ValidateNodes(List<WfNode> nodes)
        {
            if (nodes == null || nodes.Count == 0)
            {
                throw BusinessException.BadRequest("流程节点不能为空");
            }

            // 检查是否有开始节点
            var hasStart = nodes.Any(x => x.Type == WorkflowConstants.NodeType.START);
            if (!hasStart)
            {
                throw BusinessException.BadRequest("流程必须包含开始节点");
            }

            // 检查是否有结束节点
            var hasEnd = nodes.Any(x => x.Type == WorkflowConstants.NodeType.END);
            if (!hasEnd)
            {
                throw BusinessException.BadRequest("流程必须包含结束节点");
            }

            // 检查任务节点是否有审批人
            foreach (var node in nodes.Where(x => x.Type == WorkflowConstants.NodeType.TASK))
            {
                if (string.IsNullOrEmpty(node.AssigneeType) || string.IsNullOrEmpty(node.AssigneeId))
                {
                    throw BusinessException.BadRequest($"任务节点【{node.Name}】未设置审批人");
                }
            }
        }
    }
}