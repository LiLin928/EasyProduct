using EasyProduct.Business.Site;
using EasyProduct.Common.Base;
using EasyProduct.Models.Dto.Site.About;
using EasyProduct.Web.Controllers.Site.Base;
using Microsoft.AspNetCore.Mvc;

namespace EasyProduct.Web.Controllers.Site;

/// <summary>
/// 关于我们控制器（官网公开）
/// </summary>
/// <remarks>
/// 提供关于我们的公开查询功能
/// 官网接口，允许匿名访问
/// 只返回启用状态的关于我们内容
/// </remarks>
public class AboutController : SiteControllerBase
{
    private readonly ISiteAboutService _siteAboutService;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="siteAboutService">官网关于我们服务</param>
    public AboutController(ISiteAboutService siteAboutService)
    {
        _siteAboutService = siteAboutService;
    }

    /// <summary>
    /// 获取关于我们详情
    /// </summary>
    /// <returns>关于我们详情</returns>
    /// <remarks>
    /// 获取启用状态的关于我们内容
    /// </remarks>
    [HttpGet]
    public async Task<ApiResponse<AboutDto>> Get()
    {
        var result = await _siteAboutService.GetAboutAsync();
        return Success(result);
    }
}