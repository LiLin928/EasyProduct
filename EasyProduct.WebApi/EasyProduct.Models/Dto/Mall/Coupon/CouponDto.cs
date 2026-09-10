namespace EasyProduct.Models.Dto.Mall.Coupon;

/// <summary>
/// 优惠券 DTO
/// </summary>
public class CouponDto
{
    /// <summary>
    /// 优惠券ID
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// 优惠券名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 优惠券类型
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// 优惠值
    /// </summary>
    public decimal Value { get; set; }

    /// <summary>
    /// 最低消费金额
    /// </summary>
    public decimal MinAmount { get; set; }

    /// <summary>
    /// 开始时间
    /// </summary>
    public DateTime StartTime { get; set; }

    /// <summary>
    /// 结束时间
    /// </summary>
    public DateTime EndTime { get; set; }

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
    /// 剩余数量
    /// </summary>
    public int RemainCount => TotalCount - ClaimedCount;

    /// <summary>
    /// 优惠券说明
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; }
}