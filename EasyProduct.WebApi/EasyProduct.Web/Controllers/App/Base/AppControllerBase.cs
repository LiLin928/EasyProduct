using EasyProduct.Common.Base;
using Microsoft.AspNetCore.Authorization;

namespace EasyProduct.Web.Controllers.App.Base;

/// <summary>
/// 小程序端控制器基类
/// </summary>
/// <remarks>
/// 所有小程序端 API 控制器应继承此基类，自动添加 [Authorize] 特性和路由前缀。
/// 路由前缀：/api/app
/// </remarks>
[ApiController]
[Route("api/app/[controller]/[action]")]
[Authorize(Roles = "Member")]
public abstract class AppControllerBase : BaseController
{
}
