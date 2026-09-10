namespace EasyProduct.Business.Mall;

/// <summary>
/// 微信登录服务接口
/// </summary>
/// <remarks>
/// 提供微信小程序登录功能，包括 code2session、会员信息同步、JWT 生成等。
/// </remarks>
public interface IWxLoginService
{
    /// <summary>
    /// 微信小程序登录
    /// </summary>
    /// <param name="code">微信登录 code</param>
    /// <returns>会员 Token 和会员信息</returns>
    /// <remarks>
    /// 1. 调用微信 code2Session 接口获取 openid 和 session_key。
    /// 2. 根据 openid 查询或创建会员记录。
    /// 3. 生成会员 JWT。
    /// 4. 返回 Token 和会员信息。
    /// </remarks>
    Task<WxLoginResult> WxLoginAsync(string code);

    /// <summary>
    /// 获取微信用户信息
    /// </summary>
    /// <param name="openid">微信 openid</param>
    /// <returns>用户信息</returns>
    Task<WxUserInfo?> GetWxUserInfoAsync(string openid);
}

/// <summary>
/// 微信登录结果
/// </summary>
public class WxLoginResult
{
    /// <summary>
    /// 会员 Token
    /// </summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// 会员 ID
    /// </summary>
    public string MemberId { get; set; } = string.Empty;

    /// <summary>
    /// 微信 openid
    /// </summary>
    public string OpenId { get; set; } = string.Empty;

    /// <summary>
    /// 会员昵称
    /// </summary>
    public string? Nickname { get; set; }

    /// <summary>
    /// 会员头像
    /// </summary>
    public string? Avatar { get; set; }
}

/// <summary>
/// 微信用户信息
/// </summary>
public class WxUserInfo
{
    /// <summary>
    /// 微信 openid
    /// </summary>
    public string OpenId { get; set; } = string.Empty;

    /// <summary>
    /// 微信 unionid
    /// </summary>
    public string? UnionId { get; set; }

    /// <summary>
    /// 昵称
    /// </summary>
    public string? Nickname { get; set; }

    /// <summary>
    /// 头像
    /// </summary>
    public string? Avatar { get; set; }

    /// <summary>
    /// 性别
    /// </summary>
    public int? Gender { get; set; }

    /// <summary>
    /// 城市
    /// </summary>
    public string? City { get; set; }

    /// <summary>
    /// 省份
    /// </summary>
    public string? Province { get; set; }

    /// <summary>
    /// 国家
    /// </summary>
    public string? Country { get; set; }
}