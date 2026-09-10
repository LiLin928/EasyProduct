namespace EasyProduct.Models.Dto.Mall.Coupon;

/// <summary>
/// 优惠券统计 DTO
/// </summary>
public class CouponStatisticsDto
{
    /// <summary>
    /// 优惠券ID
    /// </summary>
    public string CouponId { get; set; } = string.Empty;

    /// <summary>
    /// 发放总数
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// 已领取数量
    /// </summary>
    public int ClaimedCount { get; set; }

    /// <summary>
    /// 已使用数量
    /// </summary>
    public int UsedCount { get; set; }

    /// <summary>
    /// 未使用数量
    /// </summary>
    public int UnusedCount => ClaimedCount - UsedCount;

    /// <summary>
    /// 剩余数量
    /// </summary>
    public int RemainCount => TotalCount - ClaimedCount;

    /// <summary>
    /// 使用率
    /// </summary>
    public decimal UsageRate => ClaimedCount > 0 ? (decimal)UsedCount / ClaimedCount * 100 : 0;

    /// <summary>
    /// 领取率
    /// </summary>
    public decimal ClaimRate => TotalCount > 0 ? (decimal)ClaimedCount / TotalCount * 100 : 0;
}