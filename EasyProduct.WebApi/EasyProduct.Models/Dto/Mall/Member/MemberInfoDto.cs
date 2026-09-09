namespace EasyProduct.Models.Dto.Mall.Member;

/// <summary>
/// 会员信息DTO(小程序端使用)
/// </summary>
/// <remarks>
/// 用于小程序端显示会员基本信息
/// 包含会员等级信息和折扣率
/// </remarks>
public class MemberInfoDto
{
    /// <summary>
    /// 主键ID
    /// </summary>
    /// <remarks>
    /// GUID 字符串格式
    /// </remarks>
    public string Id { get; set; } = null!;

    /// <summary>
    /// 昵称
    /// </summary>
    public string? Nickname { get; set; }

    /// <summary>
    /// 头像URL
    /// </summary>
    public string? Avatar { get; set; }

    /// <summary>
    /// 手机号
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// 会员等级名称
    /// </summary>
    /// <remarks>
    /// 用于显示的会员等级名称
    /// </remarks>
    public string? LevelName { get; set; }

    /// <summary>
    /// 会员等级图标
    /// </summary>
    /// <remarks>
    /// 等级图标URL，用于展示等级标识
    /// </remarks>
    public string? LevelIcon { get; set; }

    /// <summary>
    /// 当前积分
    /// </summary>
    public int Points { get; set; }

    /// <summary>
    /// 账户余额
    /// </summary>
    public decimal Balance { get; set; }

    /// <summary>
    /// 折扣率
    /// </summary>
    /// <remarks>
    /// 会员等级对应的折扣率，范围 0.00-1.00
    /// 1.00 表示无折扣，0.90 表示 9 折
    /// </remarks>
    public decimal DiscountRate { get; set; } = 1.00m;
}