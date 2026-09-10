using System.ComponentModel.DataAnnotations;

namespace EasyProduct.Models.Dto.Mall.Point;

/// <summary>
/// 积分兑换请求DTO
/// </summary>
public class ExchangePointDto
{
    /// <summary>
    /// 兑换类型
    /// </summary>
    /// <remarks>
    /// 1=优惠券
    /// </remarks>
    [Required(ErrorMessage = "兑换类型不能为空")]
    public int ExchangeType { get; set; }

    /// <summary>
    /// 优惠券ID
    /// </summary>
    /// <remarks>
    /// 当ExchangeType=1时必填
    /// </remarks>
    [StringLength(36, ErrorMessage = "优惠券ID长度不能超过36个字符")]
    public string? CouponId { get; set; }
}