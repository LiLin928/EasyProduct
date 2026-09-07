using EasyProduct.Common.Base;
using Microsoft.AspNetCore.Authorization;

namespace EasyProduct.Web.Controllers.Admin.Base;

/// <summary>
/// 管理端控制器基类
/// </summary>
/// <remarks>
/// 所有管理端 API 控制器应继承此基类，自动添加 [Authorize] 特性和路由前缀。
/// 路由前缀：/api/admin
/// </remarks>
[ApiController]
[Route("api/admin/[controller]/[action]")]
[Authorize(Roles = "Admin")]
public abstract class AdminControllerBase : BaseController
{
}
