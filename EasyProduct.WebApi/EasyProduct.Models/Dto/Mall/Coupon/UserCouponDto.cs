namespace EasyProduct.Models.Dto.Mall.Coupon;

/// <summary>
/// 用户优惠券 DTO
/// </summary>
public class UserCouponDto
{
    /// <summary>
    /// 用户优惠券ID
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// 优惠券ID
    /// </summary>
    public string CouponId { get; set; } = string.Empty;

    /// <summary>
    /// 优惠券名称
    /// </summary>
    public string CouponName { get; set; } = string.Empty;

    /// <summary>
    /// 优惠券类型
    /// </summary>
    public string CouponType { get; set; } = string.Empty;

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
    /// 优惠券状态
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// 领取时间
    /// </summary>
    public DateTime ClaimTime { get; set; }

    /// <summary>
    /// 使用时间
    /// </summary>
    public DateTime? UsedTime { get; set; }

    /// <summary>
    /// 关联订单ID
    /// </summary>
    public string? OrderId { get; set; }
}