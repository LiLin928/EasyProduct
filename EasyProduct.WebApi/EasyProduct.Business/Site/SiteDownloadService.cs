using EasyProduct.Common.Base;
using EasyProduct.Models.Dto.Site.Download;
using EasyProduct.Models.Entitys.Site;
using Mapster;
using Microsoft.Extensions.Logging;
using SqlSugar;

namespace EasyProduct.Business.Site;

/// <summary>
/// 下载管理服务实现（官网公开）
/// </summary>
/// <remarks>
/// 提供下载管理的公开查询功能，供官网使用
/// 只返回启用状态的下载内容
/// 访问时会自动增加下载次数
/// </remarks>
public class SiteDownloadService : BaseService, ISiteDownloadService
{
    private readonly ILogger<SiteDownloadService> _logger;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="logger">日志服务</param>
    public SiteDownloadService(ILogger<SiteDownloadService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// 获取下载列表（官网公开，只返回启用状态的下载）
    /// </summary>
    /// <param name="category">分类，可选筛选</param>
    /// <returns>下载列表</returns>
    public async Task<List<DownloadDto>> GetListAsync(string? category = null)
    {
        var queryable = _db.Queryable<site_download>()
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

        return list.Adapt<List<DownloadDto>>();
    }

    /// <summary>
    /// 获取下载详情并增加下载次数
    /// </summary>
    /// <param name="id">下载ID</param>
    /// <returns>下载详情</returns>
    public async Task<DownloadDto> GetDownloadAsync(string id)
    {
        var entity = await _db.Queryable<site_download>()
            .Where(x => x.Id.ToString() == id && x.IsDeleted == 0 && x.Status == Models.Enums.Status.Enabled)
            .FirstAsync();

        if (entity == null)
        {
            _logger.LogWarning("下载不存在或已禁用：{Id}", id);
            return new DownloadDto();
        }

        // 增加下载次数
        entity.DownloadCount++;
        await _db.Updateable(entity).UpdateColumns(x => x.DownloadCount).ExecuteCommandAsync();

        _logger.LogInformation("下载文件：{Title}, DownloadCount: {Count}, ID: {Id}", entity.Title, entity.DownloadCount, entity.Id);

        return entity.Adapt<DownloadDto>();
    }
}