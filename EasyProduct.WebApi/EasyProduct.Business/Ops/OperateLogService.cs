using EasyProduct.Common.Base;
using EasyProduct.Common.Error;
using EasyProduct.Models.Dto.Ops.OperateLog;
using EasyProduct.Models.Entitys.Ops;
using Mapster;
using Microsoft.Extensions.Logging;
using SqlSugar;

namespace EasyProduct.Business.Ops;

/// <summary>
/// 操作日志服务实现
/// </summary>
/// <remarks>
/// 提供操作日志的查询和删除功能
/// </remarks>
public class OperateLogService : BaseService, IOperateLogService
{
    private readonly ILogger<OperateLogService> _logger;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="logger">日志服务</param>
    public OperateLogService(ILogger<OperateLogService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// 获取操作日志分页列表
    /// </summary>
    /// <param name="query">查询参数，包含分页、筛选条件等</param>
    /// <returns>操作日志分页列表</returns>
    public async Task<PageResponse<OperateLogDto>> GetPageListAsync(OperateLogQueryDto query)
    {
        var queryable = _db.Queryable<ops_operate_log>()
            .Where(x => x.IsDeleted == 0);

        // 按模块筛选
        if (!string.IsNullOrEmpty(query.Module))
        {
            queryable = queryable.Where(x => x.Module.Contains(query.Module));
        }

        // 按操作类型筛选
        if (!string.IsNullOrEmpty(query.Action))
        {
            queryable = queryable.Where(x => x.Action.Contains(query.Action));
        }

        // 按用户名筛选
        if (!string.IsNullOrEmpty(query.UserName))
        {
            queryable = queryable.Where(x => x.UserName.Contains(query.UserName));
        }

        // 按操作状态筛选
        if (query.OperateStatus.HasValue)
        {
            queryable = queryable.Where(x => x.OperateStatus == query.OperateStatus.Value);
        }

        // 按时间范围筛选
        if (query.StartTime.HasValue)
        {
            queryable = queryable.Where(x => x.CreatedAt >= query.StartTime.Value);
        }

        if (query.EndTime.HasValue)
        {
            queryable = queryable.Where(x => x.CreatedAt <= query.EndTime.Value);
        }

        // 分页查询
        RefAsync<int> total = 0;
        var list = await queryable
            .OrderBy(x => x.CreatedAt, OrderByType.Desc)
            .ToPageListAsync(query.PageIndex, query.PageSize, total);

        return PageResponse<OperateLogDto>.Create(
            list.Adapt<List<OperateLogDto>>(),
            total.Value,
            query.PageIndex,
            query.PageSize
        );
    }

    /// <summary>
    /// 根据ID获取操作日志详情
    /// </summary>
    /// <param name="id">日志ID</param>
    /// <returns>操作日志详情</returns>
    public async Task<OperateLogDto> GetByIdAsync(string id)
    {
        var entity = await _db.Queryable<ops_operate_log>()
            .Where(x => x.Id.ToString() == id && x.IsDeleted == 0)
            .FirstAsync();

        if (entity == null)
        {
            throw new BusinessException("操作日志不存在", 404);
        }

        return entity.Adapt<OperateLogDto>();
    }

    /// <summary>
    /// 删除操作日志
    /// </summary>
    /// <param name="id">日志ID</param>
    /// <returns>删除成功返回 true</returns>
    public async Task<bool> DeleteAsync(string id)
    {
        var entity = await _db.Queryable<ops_operate_log>()
            .Where(x => x.Id.ToString() == id && x.IsDeleted == 0)
            .FirstAsync();

        if (entity == null)
        {
            throw new BusinessException("操作日志不存在", 404);
        }

        // 软删除
        entity.IsDeleted = 1;
        entity.UpdatedAt = DateTime.UtcNow;

        await _db.Updateable(entity).ExecuteCommandAsync();

        _logger.LogInformation("删除操作日志成功，ID: {Id}", entity.Id);

        return true;
    }

    /// <summary>
    /// 批量删除操作日志
    /// </summary>
    /// <param name="ids">日志ID列表</param>
    /// <returns>删除成功返回 true</returns>
    public async Task<bool> DeleteBatchAsync(List<string> ids)
    {
        if (ids == null || ids.Count == 0)
        {
            throw new BusinessException("请选择要删除的日志");
        }

        // 批量软删除
        var result = await _db.Updateable<ops_operate_log>()
            .Where(x => ids.Contains(x.Id.ToString()) && x.IsDeleted == 0)
            .SetColumns(x => x.IsDeleted == 1)
            .SetColumns(x => x.UpdatedAt == DateTime.UtcNow)
            .ExecuteCommandAsync();

        _logger.LogInformation("批量删除操作日志成功，数量: {Count}", result);

        return result > 0;
    }
}