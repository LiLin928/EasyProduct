using EasyProduct.Models.Dto.Common;

namespace EasyProduct.Models.Dto.Mall.Coupon;

/// <summary>
/// 优惠券查询参数
/// </summary>
public class CouponQuery : PageQuery
{
    /// <summary>
    /// 优惠券名称
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// 优惠券类型
    /// </summary>
    public int? Type { get; set; }

    /// <summary>
    /// 开始时间
    /// </summary>
    public DateTime? StartTime { get; set; }

    /// <summary>
    /// 结束时间
    /// </summary>
    public DateTime? EndTime { get; set; }
}