using Microsoft.AspNetCore.Mvc;

namespace EasyProduct.Common.Base;

/// <summary>
/// 基础控制器，提供通用的辅助方法和响应封装
/// </summary>
/// <remarks>
/// 所有 API 控制器应继承此基类，以获得统一的用户信息获取方法和响应封装。
/// </remarks>
[ApiController]
[Route("api/[controller]/[action]")]
public abstract class BaseController : ControllerBase
{
    /// <summary>
    /// 获取当前登录用户的 ID
    /// </summary>
    /// <returns>用户 ID，未登录时返回 Guid.Empty</returns>
    protected Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst("UserId");
        return userIdClaim != null ? Guid.Parse(userIdClaim.Value) : Guid.Empty;
    }

    /// <summary>
    /// 获取当前登录用户的用户名
    /// </summary>
    /// <returns>用户名，未登录时返回空字符串</returns>
    protected string GetCurrentUserName()
    {
        return User.FindFirst("UserName")?.Value ?? string.Empty;
    }

    /// <summary>
    /// 获取当前登录用户的真实姓名
    /// </summary>
    /// <returns>真实姓名，未登录时返回空字符串</returns>
    protected string GetCurrentRealName()
    {
        return User.FindFirst("RealName")?.Value ?? string.Empty;
    }

    /// <summary>
    /// 判断当前用户是否为管理员
    /// </summary>
    /// <returns>是管理员返回 true，否则返回 false</returns>
    protected bool IsAdmin()
    {
        return User.IsInRole("Admin");
    }

    /// <summary>
    /// 返回带数据的成功响应
    /// </summary>
    protected ApiResponse<T> Success<T>(T? data, string message = "操作成功")
    {
        return ApiResponse<T>.Success(data, message);
    }

    /// <summary>
    /// 返回无数据的成功响应
    /// </summary>
    protected ApiResponse<object> Success(string message = "操作成功")
    {
        return ApiResponse<object>.Success(null, message);
    }

    /// <summary>
    /// 返回失败响应
    /// </summary>
    protected ApiResponse<T> Error<T>(string message, int code = 500)
    {
        return ApiResponse<T>.Error(message, code);
    }
}
