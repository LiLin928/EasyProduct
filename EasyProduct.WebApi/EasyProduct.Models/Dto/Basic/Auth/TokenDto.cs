namespace EasyProduct.Models.Dto.Basic.Auth;

/// <summary>
/// Token 信息 DTO
/// </summary>
/// <remarks>
/// 包含访问令牌和刷新令牌，用于前端存储和使用
/// </remarks>
public class TokenDto
{
    /// <summary>
    /// 访问令牌（Access Token）
    /// </summary>
    /// <remarks>
    /// 用于 API 访问的 JWT 令牌，有效期较短（默认 24 小时）
    /// </remarks>
    public string AccessToken { get; set; } = string.Empty;

    /// <summary>
    /// 刷新令牌（Refresh Token）
    /// </summary>
    /// <remarks>
    /// 用于刷新 Access Token，有效期较长（默认 7 天）
    /// </remarks>
    public string RefreshToken { get; set; } = string.Empty;

    /// <summary>
    /// 访问令牌过期时间（秒）
    /// </summary>
    /// <remarks>
    /// 从当前时间到令牌过期的秒数，前端可用于计算过期时间
    /// </remarks>
    public int ExpiresIn { get; set; }

    /// <summary>
    /// 令牌类型
    /// </summary>
    /// <remarks>
    /// 固定为 "Bearer"，用于 Authorization 请求头
    /// </remarks>
    public string TokenType { get; set; } = "Bearer";
}