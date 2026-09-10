using SqlSugar;
using EasyProduct.Models.Enums.Mall;
using EasyProduct.Models.Entitys.Base;

namespace EasyProduct.Models.Entitys.Mall;

/// <summary>
/// 用户优惠券实体类
/// </summary>
/// <remarks>
/// 用户优惠券表，记录用户领取的优惠券信息。
/// 用户可以查看自己领取的优惠券。
/// 用户在下单时可以使用优惠券。
/// 优惠券使用后会关联订单ID。
/// </remarks>
[SugarTable("mall_user_coupon", "用户优惠券表")]
public class UserCoupon : BaseEntity
{
    /// <summary>
    /// 优惠券ID
    /// </summary>
    /// <remarks>
    /// 关联优惠券表 mall_coupon 的 Id 字段。
    /// </remarks>
    [SugarColumn(Length = 36, IsNullable = false, ColumnDescription = "优惠券ID")]
    public string CouponId { get; set; } = string.Empty;

    /// <summary>
    /// 会员ID
    /// </summary>
    /// <remarks>
    /// 关联会员表 mall_member 的 Id 字段。
    /// </remarks>
    [SugarColumn(Length = 36, IsNullable = false, ColumnDescription = "会员ID")]
    public string MemberId { get; set; } = string.Empty;

    /// <summary>
    /// 优惠券状态
    /// </summary>
    /// <remarks>
    /// 使用 UserCouponStatus 枚举：
    /// - Unused: 未使用
    /// - Used: 已使用
    /// - Expired: 已过期
    /// </remarks>
    public UserCouponStatus Status { get; set; } = UserCouponStatus.Unused;

    /// <summary>
    /// 使用时间
    /// </summary>
    /// <remarks>
    /// 优惠券被使用的时间。
    /// 未使用时为 null。
    /// </remarks>
    [SugarColumn(IsNullable = true, ColumnDescription = "使用时间")]
    public DateTime? UsedTime { get; set; }

    /// <summary>
    /// 关联订单ID
    /// </summary>
    /// <remarks>
    /// 使用该优惠券的订单ID。
    /// 未使用时为 null。
    /// </remarks>
    [SugarColumn(Length = 36, IsNullable = true, ColumnDescription = "关联订单ID")]
    public string? OrderId { get; set; }

    /// <summary>
    /// 领取时间
    /// </summary>
    /// <remarks>
    /// 用户领取优惠券的时间。
    /// </remarks>
    [SugarColumn(IsNullable = false, ColumnDescription = "领取时间")]
    public DateTime ClaimTime { get; set; } = DateTime.Now;
}