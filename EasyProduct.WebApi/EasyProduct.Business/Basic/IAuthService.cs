using EasyProduct.Models.Dto.Basic.Auth;

namespace EasyProduct.Business.Basic;

/// <summary>
/// 认证服务接口
/// </summary>
/// <remarks>
/// 提供用户登录、Token 刷新、用户信息查询等认证相关功能
/// </remarks>
public interface IAuthService
{
    /// <summary>
    /// 用户登录
    /// </summary>
    /// <param name="loginDto">登录请求参数，包含用户名和密码</param>
    /// <returns>登录响应，包含 Token 和用户信息</returns>
    Task<LoginResponseDto> LoginAsync(LoginDto loginDto);

    /// <summary>
    /// 刷新访问令牌
    /// </summary>
    /// <param name="refreshToken">刷新令牌</param>
    /// <returns>新的 Token 信息</returns>
    Task<TokenDto> RefreshTokenAsync(string refreshToken);

    /// <summary>
    /// 获取用户信息
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <returns>用户信息，包含角色和权限</returns>
    Task<UserInfoDto> GetUserInfoAsync(string userId);
}