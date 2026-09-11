using EasyProduct.Common.Base;
using EasyProduct.Models.Dto.Site.Video;
using EasyProduct.Models.Entitys.Site;
using Mapster;
using Microsoft.Extensions.Logging;
using SqlSugar;

namespace EasyProduct.Business.Site;

/// <summary>
/// 视频管理服务实现（官网公开）
/// </summary>
/// <remarks>
/// 提供视频管理的公开查询功能，供官网使用
/// 只返回启用状态的视频内容
/// 访问时会自动增加播放次数
/// </remarks>
public class SiteVideoService : BaseService, ISiteVideoService
{
    private readonly ILogger<SiteVideoService> _logger;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="logger">日志服务</param>
    public SiteVideoService(ILogger<SiteVideoService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// 获取视频列表（官网公开，只返回启用状态的视频）
    /// </summary>
    /// <param name="category">分类，可选筛选</param>
    /// <returns>视频列表</returns>
    public async Task<List<VideoDto>> GetListAsync(string? category = null)
    {
        var queryable = _db.Queryable<site_video>()
            .Where(x => x.IsDeleted == 0 && x.Status == Models.Enums.Status.Enabled);

        // 按分类筛选
        if (!string.IsNullOrEmpty(category))
        {
            queryable = queryable.Where(x => x.Category == category);
        }

        // 排序：先按排序号，再按创建时间倒序
        var list = await queryable
            .OrderBy(x => x.Sort)
            .OrderBy(x => x.CreatedAt, OrderByType.Desc)
            .ToListAsync();

        return list.Adapt<List<VideoDto>>();
    }

    /// <summary>
    /// 获取视频详情并增加播放次数
    /// </summary>
    /// <param name="id">视频ID</param>
    /// <returns>视频详情</returns>
    public async Task<VideoDto> GetVideoAsync(string id)
    {
        var entity = await _db.Queryable<site_video>()
            .Where(x => x.Id.ToString() == id && x.IsDeleted == 0 && x.Status == Models.Enums.Status.Enabled)
            .FirstAsync();

        if (entity == null)
        {
            _logger.LogWarning("视频不存在或已禁用：{Id}", id);
            return new VideoDto();
        }

        // 增加播放次数
        entity.PlayCount++;
        await _db.Updateable(entity).UpdateColumns(x => x.PlayCount).ExecuteCommandAsync();

        _logger.LogInformation("播放视频：{Title}, PlayCount: {Count}, ID: {Id}", entity.Title, entity.PlayCount, entity.Id);

        return entity.Adapt<VideoDto>();
    }
}