using EasyProduct.Common.Base;
using EasyProduct.Common.Error;
using EasyProduct.Models.Dto.Basic.Auth;
using EasyProduct.Business.Basic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyProduct.Web.Controllers.Admin.Basic;

/// <summary>
/// 认证控制器
/// </summary>
/// <remarks>
/// 提供用户登录、Token 刷新、用户信息查询等认证相关接口
/// </remarks>
[Route("api/admin/auth")]
public class AuthController : BaseController
{
    private readonly IAuthService _authService;

    /// <summary>
    /// 构造函数，通过依赖注入获取认证服务
    /// </summary>
    /// <param name="authService">认证服务接口</param>
    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// 用户登录
    /// </summary>
    /// <param name="loginDto">登录请求参数，包含用户名和密码</param>
    /// <returns>登录响应，包含 Token 和用户信息</returns>
    /// <remarks>
    /// 用户登录成功后返回 JWT Token 和用户基本信息，Token 用于后续 API 请求认证
    /// </remarks>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ApiResponse<LoginResponseDto>> Login([FromBody] LoginDto loginDto)
    {
        try
        {
            var result = await _authService.LoginAsync(loginDto);
            return Success(result, "登录成功");
        }
        catch (BusinessException ex)
        {
            return Error<LoginResponseDto>(ex.Message, ex.Code);
        }
    }

    /// <summary>
    /// 刷新访问令牌
    /// </summary>
    /// <param name="refreshToken">刷新令牌</param>
    /// <returns>新的 Token 信息</returns>
    /// <remarks>
    /// 使用刷新令牌获取新的访问令牌，旧的刷新令牌将失效
    /// </remarks>
    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<ApiResponse<TokenDto>> RefreshToken([FromBody] string refreshToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                return Error<TokenDto>("刷新令牌不能为空", 400);
            }

            var result = await _authService.RefreshTokenAsync(refreshToken);
            return Success(result, "刷新成功");
        }
        catch (BusinessException ex)
        {
            return Error<TokenDto>(ex.Message, ex.Code);
        }
    }

    /// <summary>
    /// 获取当前用户信息
    /// </summary>
    /// <returns>用户信息，包含角色和权限</returns>
    /// <remarks>
    /// 根据当前登录用户的 Token 获取用户详细信息，包括角色和权限列表
    /// </remarks>
    [HttpGet("user-info")]
    [Authorize]
    public async Task<ApiResponse<UserInfoDto>> GetUserInfo()
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
            {
                return Error<UserInfoDto>("未登录", 401);
            }

            var result = await _authService.GetUserInfoAsync(userId.ToString());
            return Success(result);
        }
        catch (BusinessException ex)
        {
            return Error<UserInfoDto>(ex.Message, ex.Code);
        }
    }
}