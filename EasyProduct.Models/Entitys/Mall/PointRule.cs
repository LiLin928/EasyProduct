using SqlSugar;
using EasyProduct.Models.Enums.Mall;

namespace EasyProduct.Models.Entitys.Mall;

/// <summary>
/// 积分规则实体
/// </summary>
/// <remarks>
/// 定义积分发放规则，包括下单赠送、评价赠送、签到赠送、邀请赠送等类型
/// </remarks>
[SugarTable("mall_point_rule")]
public class PointRule : BaseEntity
{
    /// <summary>
    /// 规则名称
    /// </summary>
    [SugarColumn(Length = 100)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 规则类型
    /// </summary>
    /// <remarks>
    /// 1=下单赠送，2=评价赠送，3=签到赠送，4=邀请赠送
    /// </remarks>
    public PointRuleType Type { get; set; }

    /// <summary>
    /// 积分数
    /// </summary>
    /// <remarks>
    /// 固定积分数，如评价赠送10分；下单赠送为金额倍数基数
    /// </remarks>
    public int Points { get; set; }

    /// <summary>
    /// 是否倍数
    /// </summary>
    /// <remarks>
    /// 下单赠送时，是否按订单金额倍数计算（如每10元积1分）
    /// </remarks>
    public bool IsMultiple { get; set; }

    /// <summary>
    /// 倍数基数
    /// </summary>
    /// <remarks>
    /// 当IsMultiple=true时有效，如10表示每10元积Points分
    /// </remarks>
    public decimal MultipleBase { get; set; }

    /// <summary>
    /// 最大积分数（单次）
    /// </summary>
    /// <remarks>
    /// 限制单次最多获得的积分数，0表示不限制
    /// </remarks>
    public int MaxPoints { get; set; }

    /// <summary>
    /// 规则描述
    /// </summary>
    [SugarColumn(Length = 500, IsNullable = true)]
    public string? Description { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    /// <remarks>
    /// true=启用，false=禁用
    /// </remarks>
    public bool Status { get; set; }

    /// <summary>
    /// 生效时间
    /// </summary>
    public DateTime StartTime { get; set; }

    /// <summary>
    /// 失效时间
    /// </summary>
    public DateTime EndTime { get; set; }

    /// <summary>
    /// 排序号
    /// </summary>
    public int Sort { get; set; }
}