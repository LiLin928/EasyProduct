using EasyProduct.Common.Base;
using EasyProduct.Models.Dto.Site.About;
using EasyProduct.Models.Entitys.Site;
using Mapster;
using Microsoft.Extensions.Logging;
using SqlSugar;

namespace EasyProduct.Business.Site;

/// <summary>
/// 关于我们服务实现（官网公开）
/// </summary>
/// <remarks>
/// 提供关于我们的公开查询功能，供官网使用
/// 只返回启用状态的关于我们内容
/// </remarks>
public class SiteAboutService : BaseService, ISiteAboutService
{
    private readonly ILogger<SiteAboutService> _logger;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="logger">日志服务</param>
    public SiteAboutService(ILogger<SiteAboutService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// 获取关于我们详情（官网公开，只返回启用状态的内容）
    /// </summary>
    /// <returns>关于我们详情</returns>
    public async Task<AboutDto> GetAboutAsync()
    {
        var entity = await _db.Queryable<site_about>()
            .Where(x => x.IsDeleted == 0 && x.Status == Models.Enums.Status.Enabled)
            .FirstAsync();

        if (entity == null)
        {
            _logger.LogWarning("未找到启用的关于我们内容");
            return new AboutDto();
        }

        return entity.Adapt<AboutDto>();
    }
}