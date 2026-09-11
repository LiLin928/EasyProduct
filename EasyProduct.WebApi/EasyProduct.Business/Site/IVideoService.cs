using EasyProduct.Models.Dto.Site.Video;

namespace EasyProduct.Business.Site;

/// <summary>
/// 视频管理服务接口（管理端）
/// </summary>
/// <remarks>
/// 提供视频管理的增删改查功能，供管理端使用
/// </remarks>
public interface IVideoService
{
    /// <summary>
    /// 获取视频列表（支持分页、筛选）
    /// </summary>
    /// <param name="query">查询参数，包含分页、标题、分类、状态、视频类型筛选</param>
    /// <returns>视频列表</returns>
    Task<List<VideoDto>> GetListAsync(VideoQueryDto query);

    /// <summary>
    /// 获取视频详情
    /// </summary>
    /// <param name="id">视频ID</param>
    /// <returns>视频详情</returns>
    Task<VideoDto> GetByIdAsync(string id);

    /// <summary>
    /// 创建视频
    /// </summary>
    /// <param name="dto">创建参数</param>
    /// <returns>新视频ID</returns>
    Task<string> CreateAsync(CreateVideoDto dto);

    /// <summary>
    /// 更新视频
    /// </summary>
    /// <param name="dto">更新参数</param>
    /// <returns>是否成功</returns>
    Task<bool> UpdateAsync(UpdateVideoDto dto);

    /// <summary>
    /// 删除视频
    /// </summary>
    /// <param name="id">视频ID</param>
    /// <returns>是否成功</returns>
    Task<bool> DeleteAsync(string id);

    /// <summary>
    /// 更新视频状态
    /// </summary>
    /// <param name="id">视频ID</param>
    /// <param name="status">状态：0=禁用，1=启用</param>
    /// <returns>是否成功</returns>
    Task<bool> UpdateStatusAsync(string id, int status);
}