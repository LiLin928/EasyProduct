using EasyProduct.Models.Dto.Site.Download;

namespace EasyProduct.Business.Site;

/// <summary>
/// 下载管理服务接口（管理端）
/// </summary>
/// <remarks>
/// 提供下载管理的增删改查功能，供管理端使用
/// </remarks>
public interface IDownloadService
{
    /// <summary>
    /// 获取下载列表（支持分页、筛选）
    /// </summary>
    /// <param name="query">查询参数，包含分页、标题、分类、状态、文件类型筛选</param>
    /// <returns>下载列表</returns>
    Task<List<DownloadDto>> GetListAsync(DownloadQueryDto query);

    /// <summary>
    /// 获取下载详情
    /// </summary>
    /// <param name="id">下载ID</param>
    /// <returns>下载详情</returns>
    Task<DownloadDto> GetByIdAsync(string id);

    /// <summary>
    /// 创建下载
    /// </summary>
    /// <param name="dto">创建参数</param>
    /// <returns>新下载ID</returns>
    Task<string> CreateAsync(CreateDownloadDto dto);

    /// <summary>
    /// 更新下载
    /// </summary>
    /// <param name="dto">更新参数</param>
    /// <returns>是否成功</returns>
    Task<bool> UpdateAsync(UpdateDownloadDto dto);

    /// <summary>
    /// 删除下载
    /// </summary>
    /// <param name="id">下载ID</param>
    /// <returns>是否成功</returns>
    Task<bool> DeleteAsync(string id);

    /// <summary>
    /// 更新下载状态
    /// </summary>
    /// <param name="id">下载ID</param>
    /// <param name="status">状态：0=禁用，1=启用</param>
    /// <returns>是否成功</returns>
    Task<bool> UpdateStatusAsync(string id, int status);
}