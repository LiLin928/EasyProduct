namespace EasyProduct.Models.Dto.Mall.Member;

/// <summary>
/// 会员DTO
/// </summary>
/// <remarks>
/// 用于返回会员信息，包含所有会员字段
/// 包含会员等级名称用于显示
/// </remarks>
public class MemberDto
{
    /// <summary>
    /// 主键ID
    /// </summary>
    /// <remarks>
    /// GUID 字符串格式
    /// </remarks>
    public string Id { get; set; } = null!;

    /// <summary>
    /// 微信OpenID
    /// </summary>
    public string? OpenId { get; set; }

    /// <summary>
    /// 微信UnionID
    /// </summary>
    public string? UnionId { get; set; }

    /// <summary>
    /// 昵称
    /// </summary>
    public string? Nickname { get; set; }

    /// <summary>
    /// 头像URL
    /// </summary>
    public string? Avatar { get; set; }

    /// <summary>
    /// 性别：0=未知，1=男，2=女
    /// </summary>
    public int Gender { get; set; }

    /// <summary>
    /// 手机号
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// 真实姓名
    /// </summary>
    public string? RealName { get; set; }

    /// <summary>
    /// 生日
    /// </summary>
    public DateTime? Birthday { get; set; }

    /// <summary>
    /// 邮箱
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// 会员等级ID
    /// </summary>
    /// <remarks>
    /// GUID 字符串格式
    /// </remarks>
    public string? LevelId { get; set; }

    /// <summary>
    /// 会员等级名称
    /// </summary>
    /// <remarks>
    /// 用于显示的会员等级名称，方便前端展示
    /// </remarks>
    public string? LevelName { get; set; }

    /// <summary>
    /// 当前积分
    /// </summary>
    public int Points { get; set; }

    /// <summary>
    /// 累计积分
    /// </summary>
    public int TotalPoints { get; set; }

    /// <summary>
    /// 账户余额
    /// </summary>
    public decimal Balance { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    /// <remarks>
    /// 使用 MemberStatus 枚举：Disabled（0）= 禁用，Enabled（1）= 启用
    /// </remarks>
    public int Status { get; set; }

    /// <summary>
    /// 最后登录时间
    /// </summary>
    public DateTime? LastLoginTime { get; set; }

    /// <summary>
    /// 最后登录IP
    /// </summary>
    public string? LastLoginIp { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreateTime { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime? UpdateTime { get; set; }
}