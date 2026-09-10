using System.Net.Http;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using EasyProduct.Common.Base;
using EasyProduct.Models.Entitys.Mall;
using EasyProduct.Models.Options;
using EasyProduct.Models.Enums.Mall;

namespace EasyProduct.Business.Mall;

/// <summary>
/// 微信登录服务实现
/// </summary>
/// <remarks>
/// 提供微信小程序登录功能，包括 code2session、会员信息同步、JWT 生成等。
/// </remarks>
public class WxLoginService : BaseService, IWxLoginService
{
    private readonly ILogger<WxLoginService> _logger;
    private readonly WxMiniAppOptions _wxOptions;
    private readonly IMemberService _memberService;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IJwtService _jwtService;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="logger">日志服务</param>
    /// <param name="wxOptions">微信小程序配置</param>
    /// <param name="memberService">会员服务</param>
    /// <param name="httpClientFactory">HTTP 客户端工厂</param>
    /// <param name="jwtService">JWT 服务</param>
    public WxLoginService(
        ILogger<WxLoginService> logger,
        IOptions<WxMiniAppOptions> wxOptions,
        IMemberService memberService,
        IHttpClientFactory httpClientFactory,
        IJwtService jwtService)
    {
        _logger = logger;
        _wxOptions = wxOptions.Value;
        _memberService = memberService;
        _httpClientFactory = httpClientFactory;
        _jwtService = jwtService;
    }

    /// <summary>
    /// 微信小程序登录
    /// </summary>
    /// <param name="code">微信登录 code</param>
    /// <returns>会员 Token 和会员信息</returns>
    public async Task<WxLoginResult> WxLoginAsync(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw Common.Error.BusinessException.BadRequest("微信登录 code 不能为空");
        }

        // 调用微信 code2Session 接口
        var sessionResult = await Code2SessionAsync(code);

        // 根据 openid 查询或创建会员
        var member = await GetOrCreateMemberAsync(sessionResult.OpenId, sessionResult.UnionId);

        // 生成会员 JWT
        var token = _jwtService.GenerateMemberToken(member.Id.ToString(), sessionResult.OpenId);

        _logger.LogInformation("微信登录成功，会员ID：{MemberId}，OpenId：{OpenId}", member.Id, sessionResult.OpenId);

        return new WxLoginResult
        {
            Token = token,
            MemberId = member.Id.ToString(),
            OpenId = sessionResult.OpenId,
            Nickname = member.Nickname,
            Avatar = member.Avatar
        };
    }

    /// <summary>
    /// 获取微信用户信息
    /// </summary>
    /// <param name="openid">微信 openid</param>
    /// <returns>用户信息</returns>
    public async Task<WxUserInfo?> GetWxUserInfoAsync(string openid)
    {
        var member = await _db.Queryable<Member>()
            .Where(m => m.OpenId == openid && !m.IsDeleted)
            .FirstAsync();

        if (member == null)
        {
            return null;
        }

        return new WxUserInfo
        {
            OpenId = member.OpenId ?? string.Empty,
            UnionId = member.UnionId,
            Nickname = member.Nickname,
            Avatar = member.Avatar,
            Gender = (int?)member.Gender
        };
    }

    /// <summary>
    /// 调用微信 code2Session 接口
    /// </summary>
    /// <param name="code">微信登录 code</param>
    /// <returns>会话信息</returns>
    private async Task<Code2SessionResult> Code2SessionAsync(string code)
    {
        // 检查是否启用微信登录
        if (!_wxOptions.Enabled || string.IsNullOrEmpty(_wxOptions.AppId) || string.IsNullOrEmpty(_wxOptions.Secret))
        {
            _logger.LogWarning("微信小程序登录未启用或配置不完整，使用模拟登录");

            // 开发环境：模拟返回
            return new Code2SessionResult
            {
                OpenId = $"mock_openid_{Guid.NewGuid():N}",
                UnionId = null,
                SessionKey = $"mock_session_key_{Guid.NewGuid():N}"
            };
        }

        try
        {
            // 调用微信 API
            var url = $"https://api.weixin.qq.com/sns/jscode2session?appid={_wxOptions.AppId}&secret={_wxOptions.Secret}&js_code={code}&grant_type=authorization_code";

            var httpClient = _httpClientFactory.CreateClient();
            var response = await httpClient.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<Code2SessionResponse>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (result == null || !string.IsNullOrEmpty(result.ErrCode))
            {
                _logger.LogError("微信 code2session 失败：{ErrCode} - {ErrMsg}", result?.ErrCode, result?.ErrMsg);
                throw Common.Error.BusinessException.BadRequest("微信登录失败，请稍后重试");
            }

            return new Code2SessionResult
            {
                OpenId = result.OpenId ?? string.Empty,
                UnionId = result.UnionId,
                SessionKey = result.SessionKey ?? string.Empty
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "调用微信 code2session 接口异常");
            throw Common.Error.BusinessException.BadRequest("微信登录失败，请稍后重试");
        }
    }

    /// <summary>
    /// 根据 openid 查询或创建会员
    /// </summary>
    /// <param name="openid">微信 openid</param>
    /// <param name="unionid">微信 unionid</param>
    /// <returns>会员实体</returns>
    private async Task<Member> GetOrCreateMemberAsync(string openid, string? unionid)
    {
        // 查询会员
        var member = await _db.Queryable<Member>()
            .Where(m => m.OpenId == openid && !m.IsDeleted)
            .FirstAsync();

        if (member == null)
        {
            // 创建新会员
            member = new Member
            {
                Id = Guid.NewGuid(),
                OpenId = openid,
                UnionId = unionid,
                Nickname = $"微信用户{Guid.NewGuid().ToString("N").Substring(0, 8)}",
                Gender = Gender.Unknown,
                Status = MemberStatus.Enabled,
                Points = 0,
                TotalPoints = 0,
                Balance = 0,
                CreateTime = DateTime.Now,
                UpdateTime = DateTime.Now
            };

            await _db.Insertable(member).ExecuteCommandAsync();

            _logger.LogInformation("创建新会员，会员ID：{MemberId}，OpenId：{OpenId}", member.Id, openid);
        }
        else
        {
            // 更新最后登录时间
            member.LastLoginTime = DateTime.Now;
            member.UpdateTime = DateTime.Now;

            await _db.Updateable(member).ExecuteCommandAsync();
        }

        return member;
    }

    /// <summary>
    /// code2session 返回结果
    /// </summary>
    private class Code2SessionResult
    {
        public string OpenId { get; set; } = string.Empty;
        public string? UnionId { get; set; }
        public string SessionKey { get; set; } = string.Empty;
    }

    /// <summary>
    /// 微信 code2session API 响应
    /// </summary>
    private class Code2SessionResponse
    {
        public string? OpenId { get; set; }
        public string? SessionKey { get; set; }
        public string? UnionId { get; set; }
        public string? ErrCode { get; set; }
        public string? ErrMsg { get; set; }
    }
}