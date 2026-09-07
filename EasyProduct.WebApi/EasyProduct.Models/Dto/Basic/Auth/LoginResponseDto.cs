namespace EasyProduct.Models.Dto.Basic.Auth;

/// <summary>
/// 登录响应 DTO
/// </summary>
/// <remarks>
/// 包含登录成功后的 Token 信息和用户基本信息
/// </remarks>
public class LoginResponseDto
{
    /// <summary>
    /// Token 信息
    /// </summary>
    /// <remarks>
    /// 包含 AccessToken、RefreshToken 和过期时间
    /// </remarks>
    public TokenDto Token { get; set; } = new();

    /// <summary>
    /// 用户信息
    /// </summary>
    /// <remarks>
    /// 包含用户基本信息、角色和权限
    /// </remarks>
    public UserInfoDto UserInfo { get; set; } = new();
}