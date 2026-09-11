using EasyProduct.Business.Site;
using EasyProduct.Common.Base;
using EasyProduct.Models.Dto.Site.Banner;
using EasyProduct.Web.Controllers.Site.Base;
using Microsoft.AspNetCore.Mvc;

namespace EasyProduct.Web.Controllers.Site;

/// <summary>
/// Banner控制器（官网公开）
/// </summary>
/// <remarks>
/// 提供Banner的公开查询功能
/// 官网接口，允许匿名访问
/// 只返回启用状态且在有效期内的Banner
/// </remarks>
public class BannerController : SiteControllerBase
{
    private readonly ISiteBannerService _siteBannerService;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="siteBannerService">官网Banner服务</param>
    public BannerController(ISiteBannerService siteBannerService)
    {
        _siteBannerService = siteBannerService;
    }

    /// <summary>
    /// 根据位置获取Banner列表
    /// </summary>
    /// <param name="position">显示位置，如：home、product_list、detail</param>
    /// <returns>Banner列表</returns>
    /// <remarks>
    /// 根据位置获取Banner列表，只返回启用状态且在有效期内的Banner
    /// 如果不传position参数，返回所有位置的Banner
    /// </remarks>
    [HttpGet("{position?}")]
    public async Task<ApiResponse<List<BannerDto>>> GetByPosition(string? position = null)
    {
        var result = await _siteBannerService.GetListAsync(position);
        return Success(result);
    }
}