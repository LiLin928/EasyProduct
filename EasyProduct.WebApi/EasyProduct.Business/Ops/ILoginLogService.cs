using EasyProduct.Common.Base;
using EasyProduct.Models.Dto.Ops.LoginLog;

namespace EasyProduct.Business.Ops;

/// <summary>
/// 登录日志服务接口
/// </summary>
/// <remarks>
/// 提供登录日志的查询和删除功能
/// </remarks>
public interface ILoginLogService
{
    /// <summary>
    /// 获取登录日志分页列表
    /// </summary>
    /// <param name="query">查询参数，包含分页、筛选条件等</param>
    /// <returns>登录日志分页列表</returns>
    Task<PageResponse<LoginLogDto>> GetPageListAsync(LoginLogQueryDto query);

    /// <summary>
    /// 根据ID获取登录日志详情
    /// </summary>
    /// <param name="id">日志ID</param>
    /// <returns>登录日志详情</returns>
    Task<LoginLogDto> GetByIdAsync(string id);

    /// <summary>
    /// 删除登录日志
    /// </summary>
    /// <param name="id">日志ID</param>
    /// <returns>删除成功返回 true</returns>
    Task<bool> DeleteAsync(string id);

    /// <summary>
    /// 批量删除登录日志
    /// </summary>
    /// <param name="ids">日志ID列表</param>
    /// <returns>删除成功返回 true</returns>
    Task<bool> DeleteBatchAsync(List<string> ids);
}