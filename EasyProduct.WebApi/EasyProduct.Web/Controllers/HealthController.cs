using EasyProduct.Common.Base;
using Microsoft.AspNetCore.Mvc;

namespace EasyProduct.Web.Controllers;

/// <summary>
/// 健康检查控制器
/// </summary>
/// <remarks>
/// 提供系统健康状态检查接口，用于运维监控
/// </remarks>
[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class HealthController : ControllerBase
{
    /// <summary>
    /// 健康检查
    /// </summary>
    /// <returns>健康状态</returns>
    [HttpGet]
    public ApiResponse<object> Get()
    {
        return ApiResponse<object>.Success(new
        {
            Status = "healthy",
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        }, "服务运行正常");
    }
}
