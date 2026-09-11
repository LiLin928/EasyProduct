using EasyProduct.Models.Dto.Site.Download;

namespace EasyProduct.Business.Site;

/// <summary>
/// 下载管理服务接口（官网公开）
/// </summary>
/// <remarks>
/// 提供下载管理的公开查询功能，供官网使用
/// 只返回启用状态的下载内容
/// 访问时会自动增加下载次数
/// </remarks>
public interface ISiteDownloadService
{
    /// <summary>
    /// 获取下载列表（官网公开，只返回启用状态的下载）
    /// </summary>
    /// <param name="category">分类，可选筛选</param>
    /// <returns>下载列表</returns>
    Task<List<DownloadDto>> GetListAsync(string? category = null);

    /// <summary>
    /// 获取下载详情并增加下载次数
    /// </summary>
    /// <param name="id">下载ID</param>
    /// <returns>下载详情</returns>
    Task<DownloadDto> GetDownloadAsync(string id);
}