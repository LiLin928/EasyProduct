using EasyProduct.Common.Base;
using EasyProduct.Models.Dto.Ops.OperateLog;

namespace EasyProduct.Business.Ops;

/// <summary>
/// 操作日志服务接口
/// </summary>
/// <remarks>
/// 提供操作日志的查询和删除功能
/// </remarks>
public interface IOperateLogService
{
    /// <summary>
    /// 获取操作日志分页列表
    /// </summary>
    /// <param name="query">查询参数，包含分页、筛选条件等</param>
    /// <returns>操作日志分页列表</returns>
    Task<PageResponse<OperateLogDto>> GetPageListAsync(OperateLogQueryDto query);

    /// <summary>
    /// 根据ID获取操作日志详情
    /// </summary>
    /// <param name="id">日志ID</param>
    /// <returns>操作日志详情</returns>
    Task<OperateLogDto> GetByIdAsync(string id);

    /// <summary>
    /// 删除操作日志
    /// </summary>
    /// <param name="id">日志ID</param>
    /// <returns>删除成功返回 true</returns>
    Task<bool> DeleteAsync(string id);

    /// <summary>
    /// 批量删除操作日志
    /// </summary>
    /// <param name="ids">日志ID列表</param>
    /// <returns>删除成功返回 true</returns>
    Task<bool> DeleteBatchAsync(List<string> ids);
}