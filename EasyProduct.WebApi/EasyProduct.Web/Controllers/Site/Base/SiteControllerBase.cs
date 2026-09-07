using EasyProduct.Common.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyProduct.Web.Controllers.Site.Base;

/// <summary>
/// 官网端控制器基类
/// </summary>
/// <remarks>
/// 所有官网端 API 控制器应继承此基类，自动添加路由前缀。
/// 路由前缀：/api/site
/// 允许匿名访问。
/// </remarks>
[ApiController]
[Route("api/site/[controller]/[action]")]
[AllowAnonymous]
public abstract class SiteControllerBase : BaseController
{
}
