using EasyProduct.Common.Base;
using EasyProduct.Models.Dto.Site.Banner;
using EasyProduct.Models.Entitys.Site;
using Mapster;
using Microsoft.Extensions.Logging;
using SqlSugar;

namespace EasyProduct.Business.Site;

/// <summary>
/// Banner服务实现（官网公开）
/// </summary>
/// <remarks>
/// 提供Banner的公开查询功能，供官网使用
/// 只返回启用状态且在有效期内的Banner
/// </remarks>
public class SiteBannerService : BaseService, ISiteBannerService
{
    private readonly ILogger<SiteBannerService> _logger;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="logger">日志服务</param>
    public SiteBannerService(ILogger<SiteBannerService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// 获取Banner列表（官网公开，只返回启用且在有效期内的Banner）
    /// </summary>
    /// <param name="position">显示位置，如：home、product_list、detail</param>
    /// <returns>Banner列表</returns>
    public async Task<List<BannerDto>> GetListAsync(string? position = null)
    {
        var now = DateTime.UtcNow;

        var queryable = _db.Queryable<site_banner>()
            .Where(x => x.IsDeleted == 0 && x.Status == Models.Enums.Status.Enabled);

        // 按显示位置筛选
        if (!string.IsNullOrEmpty(position))
        {
            queryable = queryable.Where(x => x.Position == position);
        }

        // 筛选在有效期内的Banner
        // 开始时间为空或小于当前时间，结束时间为空或大于当前时间
        queryable = queryable.Where(x =>
            (x.StartTime == null || x.StartTime <= now) &&
            (x.EndTime == null || x.EndTime >= now));

        // 排序：先按排序号，再按创建时间倒序
        var list = await queryable
            .OrderBy(x => x.Sort)
            .OrderBy(x => x.CreatedAt, OrderByType.Desc)
            .ToListAsync();

        return list.Adapt<List<BannerDto>>();
    }

    /// <summary>
    /// 获取首页Banner列表（官网公开，只返回启用且在有效期内的Banner）
    /// </summary>
    /// <returns>Banner列表</returns>
    public async Task<List<BannerDto>> GetHomeListAsync()
    {
        return await GetListAsync("home");
    }
}