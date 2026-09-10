using System.ComponentModel.DataAnnotations;
using EasyProduct.Models.Enums.Mall;

namespace EasyProduct.Models.Dto.Mall.Coupon;

/// <summary>
/// 创建优惠券 DTO
/// </summary>
public class CreateCouponDto
{
    /// <summary>
    /// 优惠券名称
    /// </summary>
    [Required(ErrorMessage = "优惠券名称不能为空")]
    [StringLength(100, ErrorMessage = "优惠券名称长度不能超过100")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 优惠券类型
    /// </summary>
    [Required(ErrorMessage = "优惠券类型不能为空")]
    public CouponType Type { get; set; }

    /// <summary>
    /// 优惠值
    /// </summary>
    [Required(ErrorMessage = "优惠值不能为空")]
    [Range(0.01, double.MaxValue, ErrorMessage = "优惠值必须大于0")]
    public decimal Value { get; set; }

    /// <summary>
    /// 最低消费金额
    /// </summary>
    [Required(ErrorMessage = "最低消费金额不能为空")]
    [Range(0, double.MaxValue, ErrorMessage = "最低消费金额不能为负数")]
    public decimal MinAmount { get; set; }

    /// <summary>
    /// 开始时间
    /// </summary>
    [Required(ErrorMessage = "开始时间不能为空")]
    public DateTime StartTime { get; set; }

    /// <summary>
    /// 结束时间
    /// </summary>
    [Required(ErrorMessage = "结束时间不能为空")]
    public DateTime EndTime { get; set; }

    /// <summary>
    /// 发放总数
    /// </summary>
    [Required(ErrorMessage = "发放总数不能为空")]
    [Range(1, int.MaxValue, ErrorMessage = "发放总数必须大于0")]
    public int TotalCount { get; set; }

    /// <summary>
    /// 适用商品ID列表
    /// </summary>
    public List<string>? ProductIds { get; set; }

    /// <summary>
    /// 优惠券说明
    /// </summary>
    [StringLength(500, ErrorMessage = "优惠券说明长度不能超过500")]
    public string? Description { get; set; }
}