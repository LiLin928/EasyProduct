using SqlSugar;

namespace EasyProduct.Models.Entitys.Mall;

/// <summary>
/// 积分兑换记录实体
/// </summary>
/// <remarks>
/// 记录会员使用积分兑换优惠券的明细
/// </remarks>
[SugarTable("mall_point_exchange")]
public class PointExchange : BaseEntity
{
    /// <summary>
    /// 会员ID
    /// </summary>
    [SugarColumn(Length = 36)]
    public string MemberId { get; set; } = string.Empty;

    /// <summary>
    /// 兑换类型
    /// </summary>
    /// <remarks>
    /// 1=优惠券，后续可扩展其他类型
    /// </remarks>
    public int ExchangeType { get; set; }

    /// <summary>
    /// 优惠券ID
    /// </summary>
    /// <remarks>
    /// 当ExchangeType=1时有效
    /// </remarks>
    [SugarColumn(Length = 36, IsNullable = true)]
    public string? CouponId { get; set; }

    /// <summary>
    /// 消耗积分
    /// </summary>
    public int Points { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    /// <remarks>
    /// true=成功，false=失败
    /// </remarks>
    public bool Status { get; set; }

    /// <summary>
    /// 失败原因
    /// </summary>
    [SugarColumn(Length = 500, IsNullable = true)]
    public string? FailReason { get; set; }
}