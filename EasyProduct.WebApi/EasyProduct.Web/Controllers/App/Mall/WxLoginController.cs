using EasyProduct.Common.Base;
using EasyProduct.Business.Mall;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace EasyProduct.Web.Controllers.App.Mall;

/// <summary>
/// 微信登录控制器（会员端）
/// </summary>
/// <remarks>
/// 提供微信小程序登录功能。
/// 无需认证，公开接口。
/// </remarks>
[ApiController]
[Route("api/app/auth")]
public class WxLoginController : BaseController
{
    private readonly IWxLoginService _wxLoginService;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="wxLoginService">微信登录服务</param>
    public WxLoginController(IWxLoginService wxLoginService)
    {
        _wxLoginService = wxLoginService;
    }

    /// <summary>
    /// 微信小程序登录
    /// </summary>
    /// <param name="request">登录请求，包含微信 code</param>
    /// <returns>会员 Token 和会员信息</returns>
    /// <remarks>
    /// 1. 前端调用 wx.login() 获取 code。
    /// 2. 将 code 发送到本接口。
    /// 3. 后端调用微信 code2session 接口获取 openid。
    /// 4. 根据 openid 查询或创建会员记录。
    /// 5. 生成会员 JWT。
    /// 6. 返回 Token 和会员信息。
    /// </remarks>
    [HttpPost("wx-login")]
    [AllowAnonymous]
    public async Task<ApiResponse<WxLoginResult>> WxLogin([FromBody] WxLoginRequest request)
    {
        var result = await _wxLoginService.WxLoginAsync(request.Code);
        return ApiResponse<WxLoginResult>.Success(result, "登录成功");
    }

    /// <summary>
    /// 获取微信用户信息
    /// </summary>
    /// <returns>用户信息</returns>
    /// <remarks>
    /// 需要会员登录。
    /// 返回会员的微信信息。
    /// </remarks>
    [HttpGet("wx-user-info")]
    [Authorize(AuthenticationSchemes = "MemberJwt")]
    public async Task<ApiResponse<WxUserInfo>> GetWxUserInfo()
    {
        var openid = GetCurrentUserId().ToString();
        var result = await _wxLoginService.GetWxUserInfoAsync(openid);

        if (result == null)
        {
            throw Common.Error.BusinessException.NotFound("用户信息不存在");
        }

        return Success(result);
    }
}

/// <summary>
/// 微信登录请求
/// </summary>
public class WxLoginRequest
{
    /// <summary>
    /// 微信登录 code
    /// </summary>
    public string Code { get; set; } = string.Empty;
}