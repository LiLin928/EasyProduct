using EasyProduct.Models.Dto.Site.Video;

namespace EasyProduct.Business.Site;

/// <summary>
/// 视频管理服务接口（官网公开）
/// </summary>
/// <remarks>
/// 提供视频管理的公开查询功能，供官网使用
/// 只返回启用状态的视频内容
/// 访问时会自动增加播放次数
/// </remarks>
public interface ISiteVideoService
{
    /// <summary>
    /// 获取视频列表（官网公开，只返回启用状态的视频）
    /// </summary>
    /// <param name="category">分类，可选筛选</param>
    /// <returns>视频列表</returns>
    Task<List<VideoDto>> GetListAsync(string? category = null);

    /// <summary>
    /// 获取视频详情并增加播放次数
    /// </summary>
    /// <param name="id">视频ID</param>
    /// <returns>视频详情</returns>
    Task<VideoDto> GetVideoAsync(string id);
}