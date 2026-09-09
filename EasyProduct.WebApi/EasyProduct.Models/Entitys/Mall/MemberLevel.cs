using SqlSugar;
using EasyProduct.Models.Enums.Mall;
using EasyProduct.Models.Entitys.Base;

namespace EasyProduct.Models.Entitys.Mall;

/// <summary>
/// 会员等级实体
/// </summary>
[SugarTable("mall_member_level", "会员等级表")]
public class MemberLevel : BaseEntity
{
    /// <summary>
    /// 等级名称
    /// </summary>
    [SugarColumn(Length = 50)]
    public string LevelName { get; set; } = null!;

    /// <summary>
    /// 等级编码
    /// </summary>
    [SugarColumn(Length = 50)]
    public string LevelCode { get; set; } = null!;

    /// <summary>
    /// 等级数值
    /// </summary>
    public int Level { get; set; }

    /// <summary>
    /// 最低积分要求
    /// </summary>
    public int MinPoints { get; set; }

    /// <summary>
    /// 最高积分上限
    /// </summary>
    public int MaxPoints { get; set; }

    /// <summary>
    /// 折扣率(0.00-1.00)
    /// </summary>
    [SugarColumn(Length = 3, DecimalDigits = 2)]
    public decimal DiscountRate { get; set; } = 1.00m;

    /// <summary>
    /// 等级图标
    /// </summary>
    [SugarColumn(Length = 255, IsNullable = true)]
    public string? Icon { get; set; }

    /// <summary>
    /// 状态：0=禁用，1=启用
    /// </summary>
    public MemberStatus Status { get; set; } = MemberStatus.Enabled;
}