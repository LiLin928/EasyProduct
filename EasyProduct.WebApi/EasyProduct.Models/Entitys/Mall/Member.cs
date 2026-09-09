using SqlSugar;
using EasyProduct.Models.Enums.Mall;
using EasyProduct.Models.Entitys.Base;

namespace EasyProduct.Models.Entitys.Mall;

/// <summary>
/// 会员主表实体
/// </summary>
[SugarTable("mall_member", "会员主表")]
public class Member : BaseEntity
{
    /// <summary>
    /// 微信OpenID
    /// </summary>
    [SugarColumn(Length = 100, IsNullable = true)]
    public string? OpenId { get; set; }

    /// <summary>
    /// 微信UnionID
    /// </summary>
    [SugarColumn(Length = 100, IsNullable = true)]
    public string? UnionId { get; set; }

    /// <summary>
    /// 昵称
    /// </summary>
    [SugarColumn(Length = 100, IsNullable = true)]
    public string? Nickname { get; set; }

    /// <summary>
    /// 头像URL
    /// </summary>
    [SugarColumn(Length = 500, IsNullable = true)]
    public string? Avatar { get; set; }

    /// <summary>
    /// 性别：0=未知，1=男，2=女
    /// </summary>
    public Gender Gender { get; set; } = Gender.Unknown;

    /// <summary>
    /// 手机号
    /// </summary>
    [SugarColumn(Length = 20, IsNullable = true)]
    public string? Phone { get; set; }

    /// <summary>
    /// 真实姓名
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = true)]
    public string? RealName { get; set; }

    /// <summary>
    /// 生日
    /// </summary>
    [SugarColumn(IsNullable = true)]
    public DateTime? Birthday { get; set; }

    /// <summary>
    /// 邮箱
    /// </summary>
    [SugarColumn(Length = 100, IsNullable = true)]
    public string? Email { get; set; }

    /// <summary>
    /// 会员等级ID
    /// </summary>
    [SugarColumn(IsNullable = true)]
    public Guid? LevelId { get; set; }

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
    [SugarColumn(Length = 18, DecimalDigits = 2)]
    public decimal Balance { get; set; }

    /// <summary>
    /// 状态：0=禁用，1=启用
    /// </summary>
    public MemberStatus Status { get; set; } = MemberStatus.Enabled;

    /// <summary>
    /// 最后登录时间
    /// </summary>
    [SugarColumn(IsNullable = true)]
    public DateTime? LastLoginTime { get; set; }

    /// <summary>
    /// 最后登录IP
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = true)]
    public string? LastLoginIp { get; set; }
}