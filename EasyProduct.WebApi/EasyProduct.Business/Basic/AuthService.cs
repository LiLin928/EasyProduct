using EasyProduct.Common.Error;
using EasyProduct.Common.Helper;
using EasyProduct.Models.Dto.Basic.Auth;
using EasyProduct.Models.Entitys.Basic;
using EasyProduct.Models.Entitys.Ops;
using EasyProduct.Models.Enums;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using SqlSugar;
using System.Net;
using System.Text.Json;

namespace EasyProduct.Business.Basic;

/// <summary>
/// 认证服务实现
/// </summary>
/// <remarks>
/// 提供用户登录、Token 刷新、用户信息查询等认证相关功能
/// 使用构造器注入依赖服务
/// </remarks>
public class AuthService : IAuthService
{
    private readonly ISqlSugarClient _db;
    private readonly JwtHelper _jwtHelper;
    private readonly IConfiguration _configuration;
    private readonly IHttpContextAccessor _httpContextAccessor;

    /// <summary>
    /// 构造函数，通过依赖注入获取所需服务
    /// </summary>
    /// <param name="db">数据库上下文</param>
    /// <param name="jwtHelper">JWT 帮助类</param>
    /// <param name="configuration">应用程序配置</param>
    /// <param name="httpContextAccessor">HTTP 上下文访问器</param>
    public AuthService(
        ISqlSugarClient db,
        JwtHelper jwtHelper,
        IConfiguration configuration,
        IHttpContextAccessor httpContextAccessor)
    {
        _db = db;
        _jwtHelper = jwtHelper;
        _configuration = configuration;
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// 用户登录
    /// </summary>
    /// <param name="loginDto">登录请求参数，包含用户名和密码</param>
    /// <returns>登录响应，包含 Token 和用户信息</returns>
    /// <exception cref="BusinessException">用户不存在、密码错误或用户已禁用时抛出</exception>
    public async Task<LoginResponseDto> LoginAsync(LoginDto loginDto)
    {
        // 1. 查询用户信息
        var user = await _db.Queryable<basic_user>()
            .Where(u => u.UserName == loginDto.UserName && u.IsDeleted == 0)
            .FirstAsync();

        if (user == null)
        {
            await RecordLoginLogAsync(null, loginDto.UserName, 0, "用户不存在");
            throw new BusinessException("用户名或密码错误", 401);
        }

        // 2. 验证密码
        if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.Password))
        {
            await RecordLoginLogAsync(user.Id.ToString(), user.UserName, 0, "密码错误");
            throw new BusinessException("用户名或密码错误", 401);
        }

        // 3. 检查用户状态
        if (user.Status == Status.Disabled)
        {
            await RecordLoginLogAsync(user.Id.ToString(), user.UserName, 0, "用户已禁用");
            throw new BusinessException("用户已禁用，请联系管理员", 403);
        }

        // 4. 查询用户角色
        var roles = await _db.Queryable<basic_user_role, basic_role>((ur, r) => new JoinQueryInfos(
                JoinType.Left, ur.RoleId == r.Id.ToString()
            ))
            .Where((ur, r) => ur.UserId == user.Id.ToString() && ur.IsDeleted == 0 && r.IsDeleted == 0 && r.Status == Status.Enabled)
            .Select((ur, r) => r.RoleCode)
            .ToListAsync();

        // 5. 生成 Token
        var accessToken = _jwtHelper.GenerateAccessToken(user.Id.ToString(), user.UserName, user.RealName, roles);
        var refreshToken = _jwtHelper.GenerateRefreshToken();

        // 6. 保存刷新令牌
        var refreshTokenEntity = new basic_refresh_token
        {
            Id = Guid.NewGuid(),
            UserId = user.Id.ToString(),
            Token = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(7), // 刷新令牌有效期 7 天
            IsRevoked = 0,
            DeviceInfo = GetUserAgent(),
            IpAddress = GetClientIpAddress(),
            IsDeleted = 0,
            Status = Status.Enabled,
            CreatedAt = DateTime.UtcNow
        };

        await _db.Insertable(refreshTokenEntity).ExecuteCommandAsync();

        // 7. 记录登录日志
        await RecordLoginLogAsync(user.Id.ToString(), user.UserName, 1, "登录成功");

        // 8. 查询用户权限
        var permissions = await GetUserPermissionsAsync(user.Id.ToString());

        // 9. 构建响应
        var response = new LoginResponseDto
        {
            Token = new TokenDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresIn = _jwtHelper.GetAccessTokenExpiration(),
                TokenType = "Bearer"
            },
            UserInfo = new UserInfoDto
            {
                Id = user.Id.ToString(),
                UserName = user.UserName,
                RealName = user.RealName,
                Phone = user.Phone,
                Email = user.Email,
                Avatar = user.Avatar,
                DeptId = user.DeptId,
                DeptName = user.DeptName,
                Introduction = user.Introduction,
                Roles = roles,
                Permissions = permissions
            }
        };

        return response;
    }

    /// <summary>
    /// 刷新访问令牌
    /// </summary>
    /// <param name="refreshToken">刷新令牌</param>
    /// <returns>新的 Token 信息</returns>
    /// <exception cref="BusinessException">刷新令牌无效、过期或已撤销时抛出</exception>
    public async Task<TokenDto> RefreshTokenAsync(string refreshToken)
    {
        // 1. 查询刷新令牌
        var tokenEntity = await _db.Queryable<basic_refresh_token>()
            .Where(t => t.Token == refreshToken && t.IsDeleted == 0)
            .FirstAsync();

        if (tokenEntity == null)
        {
            throw new BusinessException("刷新令牌无效", 401);
        }

        // 2. 检查令牌是否已过期
        if (tokenEntity.ExpiresAt < DateTime.UtcNow)
        {
            throw new BusinessException("刷新令牌已过期", 401);
        }

        // 3. 检查令牌是否已撤销
        if (tokenEntity.IsRevoked == 1)
        {
            throw new BusinessException("刷新令牌已撤销", 401);
        }

        // 4. 查询用户信息
        var user = await _db.Queryable<basic_user>()
            .Where(u => u.Id.ToString() == tokenEntity.UserId && u.IsDeleted == 0)
            .FirstAsync();

        if (user == null)
        {
            throw new BusinessException("用户不存在", 401);
        }

        // 5. 检查用户状态
        if (user.Status == Status.Disabled)
        {
            throw new BusinessException("用户已禁用", 403);
        }

        // 6. 查询用户角色
        var roles = await _db.Queryable<basic_user_role, basic_role>((ur, r) => new JoinQueryInfos(
                JoinType.Left, ur.RoleId == r.Id.ToString()
            ))
            .Where((ur, r) => ur.UserId == user.Id.ToString() && ur.IsDeleted == 0 && r.IsDeleted == 0 && r.Status == Status.Enabled)
            .Select((ur, r) => r.RoleCode)
            .ToListAsync();

        // 7. 撤销旧的刷新令牌
        tokenEntity.IsRevoked = 1;
        tokenEntity.UpdatedAt = DateTime.UtcNow;
        await _db.Updateable(tokenEntity).ExecuteCommandAsync();

        // 8. 生成新的 Token
        var newAccessToken = _jwtHelper.GenerateAccessToken(user.Id.ToString(), user.UserName, user.RealName, roles);
        var newRefreshToken = _jwtHelper.GenerateRefreshToken();

        // 9. 保存新的刷新令牌
        var newTokenEntity = new basic_refresh_token
        {
            Id = Guid.NewGuid(),
            UserId = user.Id.ToString(),
            Token = newRefreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            IsRevoked = 0,
            DeviceInfo = tokenEntity.DeviceInfo,
            IpAddress = GetClientIpAddress(),
            IsDeleted = 0,
            Status = Status.Enabled,
            CreatedAt = DateTime.UtcNow
        };

        await _db.Insertable(newTokenEntity).ExecuteCommandAsync();

        // 10. 返回新的 Token
        return new TokenDto
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken,
            ExpiresIn = _jwtHelper.GetAccessTokenExpiration(),
            TokenType = "Bearer"
        };
    }

    /// <summary>
    /// 获取用户信息
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <returns>用户信息，包含角色和权限</returns>
    /// <exception cref="BusinessException">用户不存在时抛出</exception>
    public async Task<UserInfoDto> GetUserInfoAsync(string userId)
    {
        // 1. 查询用户信息
        var user = await _db.Queryable<basic_user>()
            .Where(u => u.Id.ToString() == userId && u.IsDeleted == 0)
            .FirstAsync();

        if (user == null)
        {
            throw new BusinessException("用户不存在", 404);
        }

        // 2. 查询用户角色
        var roles = await _db.Queryable<basic_user_role, basic_role>((ur, r) => new JoinQueryInfos(
                JoinType.Left, ur.RoleId == r.Id.ToString()
            ))
            .Where((ur, r) => ur.UserId == userId && ur.IsDeleted == 0 && r.IsDeleted == 0 && r.Status == Status.Enabled)
            .Select((ur, r) => r.RoleCode)
            .ToListAsync();

        // 3. 查询用户权限
        var permissions = await GetUserPermissionsAsync(userId);

        // 4. 返回用户信息
        return new UserInfoDto
        {
            Id = user.Id.ToString(),
            UserName = user.UserName,
            RealName = user.RealName,
            Phone = user.Phone,
            Email = user.Email,
            Avatar = user.Avatar,
            DeptId = user.DeptId,
            DeptName = user.DeptName,
            Introduction = user.Introduction,
            Roles = roles,
            Permissions = permissions
        };
    }

    /// <summary>
    /// 获取用户权限列表
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <returns>权限标识列表</returns>
    private async Task<List<string>> GetUserPermissionsAsync(string userId)
    {
        // 查询用户的角色ID列表
        var roleIds = await _db.Queryable<basic_user_role>()
            .Where(ur => ur.UserId == userId && ur.IsDeleted == 0)
            .Select(ur => ur.RoleId)
            .ToListAsync();

        if (!roleIds.Any())
        {
            return new List<string>();
        }

        // 查询角色关联的菜单权限
        var permissions = await _db.Queryable<basic_role_menu, basic_menu>((rm, m) => new JoinQueryInfos(
                JoinType.Left, rm.MenuId == m.Id.ToString()
            ))
            .Where((rm, m) => roleIds.Contains(rm.RoleId) && rm.IsDeleted == 0 && m.IsDeleted == 0 && m.Status == Status.Enabled)
            .Where((rm, m) => m.Type == 2 && !string.IsNullOrEmpty(m.Permission)) // 仅查询按钮权限
            .Select((rm, m) => m.Permission!)
            .Distinct()
            .ToListAsync();

        return permissions;
    }

    /// <summary>
    /// 记录登录日志
    /// </summary>
    /// <param name="userId">用户ID（登录失败时可能为空）</param>
    /// <param name="userName">用户名</param>
    /// <param name="loginStatus">登录状态：0=失败，1=成功</param>
    /// <param name="loginMessage">登录消息</param>
    private async Task RecordLoginLogAsync(string? userId, string userName, int loginStatus, string loginMessage)
    {
        var log = new ops_login_log
        {
            Id = Guid.NewGuid(),
            UserId = userId ?? string.Empty,
            UserName = userName,
            LoginIp = GetClientIpAddress(),
            LoginLocation = await GetIpLocationAsync(GetClientIpAddress()),
            Browser = ParseBrowser(GetUserAgent()),
            Os = ParseOs(GetUserAgent()),
            LoginStatus = loginStatus,
            LoginMessage = loginMessage,
            LoginTime = DateTime.UtcNow,
            IsDeleted = 0,
            Status = Status.Enabled,
            CreatedAt = DateTime.UtcNow
        };

        await _db.Insertable(log).ExecuteCommandAsync();
    }

    /// <summary>
    /// 获取客户端 IP 地址
    /// </summary>
    /// <returns>IP 地址字符串</returns>
    private string GetClientIpAddress()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null)
        {
            return "unknown";
        }

        // 尝试从 X-Forwarded-For 头获取真实 IP
        var forwardedFor = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(forwardedFor))
        {
            // 取第一个 IP（客户端原始 IP）
            var ips = forwardedFor.Split(',');
            return ips.Length > 0 ? ips[0].Trim() : "unknown";
        }

        // 从连接获取 IP
        return httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    }

    /// <summary>
    /// 获取 User-Agent
    /// </summary>
    /// <returns>User-Agent 字符串</returns>
    private string GetUserAgent()
    {
        return _httpContextAccessor.HttpContext?.Request.Headers["User-Agent"].FirstOrDefault() ?? "unknown";
    }

    /// <summary>
    /// 根据 IP 获取地理位置（简单实现，可后续对接 IP 地址库）
    /// </summary>
    /// <param name="ip">IP 地址</param>
    /// <returns>地理位置字符串</returns>
    private Task<string?> GetIpLocationAsync(string ip)
    {
        // TODO: 对接 IP 地址库服务，如高德、百度地图 API
        // 目前返回 null，表示未知
        if (ip == "unknown" || ip == "127.0.0.1" || ip == "::1")
        {
            return Task.FromResult<string?>("本地开发环境");
        }
        return Task.FromResult<string?>(null);
    }

    /// <summary>
    /// 解析浏览器类型（简单实现）
    /// </summary>
    /// <param name="userAgent">User-Agent 字符串</param>
    /// <returns>浏览器类型</returns>
    private string ParseBrowser(string userAgent)
    {
        if (string.IsNullOrEmpty(userAgent) || userAgent == "unknown")
        {
            return "unknown";
        }

        // 简单的浏览器识别
        if (userAgent.Contains("Edg"))
            return "Microsoft Edge";
        if (userAgent.Contains("Chrome"))
            return "Google Chrome";
        if (userAgent.Contains("Firefox"))
            return "Mozilla Firefox";
        if (userAgent.Contains("Safari"))
            return "Apple Safari";
        if (userAgent.Contains("Opera") || userAgent.Contains("OPR"))
            return "Opera";

        return "unknown";
    }

    /// <summary>
    /// 解析操作系统（简单实现）
    /// </summary>
    /// <param name="userAgent">User-Agent 字符串</param>
    /// <returns>操作系统</returns>
    private string ParseOs(string userAgent)
    {
        if (string.IsNullOrEmpty(userAgent) || userAgent == "unknown")
        {
            return "unknown";
        }

        // 简单的操作系统识别
        if (userAgent.Contains("Windows NT 10"))
            return "Windows 10/11";
        if (userAgent.Contains("Windows NT 6.3"))
            return "Windows 8.1";
        if (userAgent.Contains("Windows NT 6.2"))
            return "Windows 8";
        if (userAgent.Contains("Windows NT 6.1"))
            return "Windows 7";
        if (userAgent.Contains("Mac OS X"))
            return "macOS";
        if (userAgent.Contains("Linux"))
            return "Linux";
        if (userAgent.Contains("Android"))
            return "Android";
        if (userAgent.Contains("iPhone") || userAgent.Contains("iPad"))
            return "iOS";

        return "unknown";
    }
}