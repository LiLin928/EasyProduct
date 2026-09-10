using EasyProduct.Common.Helper;
using Microsoft.Extensions.Configuration;

namespace EasyProduct.Business.Basic;

/// <summary>
/// JWT 服务实现
/// </summary>
/// <remarks>
/// 提供会员 JWT 生成功能。
/// 会员 JWT 使用独立的认证方案（MemberJwt）。
/// </remarks>
public class JwtService : IJwtService
{
    private readonly JwtHelper _jwtHelper;
    private readonly IConfiguration _configuration;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="jwtHelper">JWT 帮助类</param>
    /// <param name="configuration">配置</param>
    public JwtService(JwtHelper jwtHelper, IConfiguration configuration)
    {
        _jwtHelper = jwtHelper;
        _configuration = configuration;
    }

    /// <summary>
    /// 生成会员 Token
    /// </summary>
    /// <param name="memberId">会员 ID</param>
    /// <param name="openid">微信 openid</param>
    /// <returns>JWT Token</returns>
    /// <remarks>
    /// 会员 JWT 包含以下 Claims：
    /// - UserId：会员 ID
    /// - OpenId：微信 openid
    /// - identity_type：固定为 "member"，用于区分管理端用户
    /// - jti：JWT ID，用于唯一标识
    /// - iat：签发时间
    /// </remarks>
    public string GenerateMemberToken(string memberId, string openid)
    {
        var jwtOptions = _configuration.GetSection("JWTTokenOptions");
        var secretKey = jwtOptions.GetValue<string>("SecretKey") ?? throw new InvalidOperationException("JWT SecretKey 未配置");
        var issuer = jwtOptions.GetValue<string>("Issuer") ?? "EasyProduct";
        var audience = jwtOptions.GetValue<string>("Audience") ?? "EasyProduct";
        var expiresInMinutes = jwtOptions.GetValue<int>("MemberExpiresInMinutes", 1440); // 会员 Token 默认 24 小时

        var claims = new List<System.Security.Claims.Claim>
        {
            new System.Security.Claims.Claim("UserId", memberId),
            new System.Security.Claims.Claim("OpenId", openid),
            new System.Security.Claims.Claim("identity_type", "member"),
            new System.Security.Claims.Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new System.Security.Claims.Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), System.Security.Claims.ClaimValueTypes.Integer64)
        };

        var key = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(secretKey));
        var credentials = new Microsoft.IdentityModel.Tokens.SigningCredentials(key, Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256);

        var token = new System.IdentityModel.Tokens.Jwt.JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiresInMinutes),
            signingCredentials: credentials
        );

        return new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler().WriteToken(token);
    }
}