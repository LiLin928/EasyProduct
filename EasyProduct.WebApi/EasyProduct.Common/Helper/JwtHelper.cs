using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace EasyProduct.Common.Helper;

/// <summary>
/// JWT Token 生成帮助类
/// </summary>
/// <remarks>
/// 用于生成 JWT Access Token 和 Refresh Token
/// </remarks>
public class JwtHelper
{
    private readonly IConfiguration _configuration;

    /// <summary>
    /// 构造函数，通过依赖注入获取配置
    /// </summary>
    /// <param name="configuration">应用程序配置</param>
    public JwtHelper(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// 生成访问令牌（Access Token）
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="userName">用户名</param>
    /// <param name="realName">真实姓名</param>
    /// <param name="roles">角色列表</param>
    /// <returns>JWT Token 字符串</returns>
    public string GenerateAccessToken(string userId, string userName, string? realName, List<string> roles)
    {
        var jwtOptions = _configuration.GetSection("JWTTokenOptions");
        var secretKey = jwtOptions.GetValue<string>("SecretKey") ?? throw new InvalidOperationException("JWT SecretKey 未配置");
        var issuer = jwtOptions.GetValue<string>("Issuer") ?? "EasyProduct";
        var audience = jwtOptions.GetValue<string>("Audience") ?? "EasyProduct";
        var expiresInMinutes = jwtOptions.GetValue<int>("ExpiresInMinutes", 1440); // 默认 24 小时

        var claims = new List<Claim>
        {
            new Claim("UserId", userId),
            new Claim("UserName", userName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
        };

        // 添加真实姓名（如果有）
        if (!string.IsNullOrEmpty(realName))
        {
            claims.Add(new Claim("RealName", realName));
        }

        // 添加角色声明
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiresInMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>
    /// 生成刷新令牌（Refresh Token）
    /// </summary>
    /// <returns>随机生成的刷新令牌</returns>
    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    /// <summary>
    /// 获取访问令牌过期时间（秒）
    /// </summary>
    /// <returns>过期时间（秒）</returns>
    public int GetAccessTokenExpiration()
    {
        var jwtOptions = _configuration.GetSection("JWTTokenOptions");
        return jwtOptions.GetValue<int>("ExpiresInMinutes", 1440) * 60; // 转换为秒
    }
}