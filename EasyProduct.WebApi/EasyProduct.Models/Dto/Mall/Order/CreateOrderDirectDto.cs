using System.ComponentModel.DataAnnotations;

namespace EasyProduct.Models.Dto.Mall.Order;

/// <summary>
/// 直接购买创建订单 DTO
/// </summary>
public class CreateOrderDirectDto
{
    /// <summary>
    /// SKU ID
    /// </summary>
    [Required(ErrorMessage = "SKU ID不能为空")]
    public string SkuId { get; set; } = string.Empty;

    /// <summary>
    /// 数量
    /// </summary>
    [Required(ErrorMessage = "数量不能为空")]
    [Range(1, int.MaxValue, ErrorMessage = "数量必须大于0")]
    public int Quantity { get; set; }

    /// <summary>
    /// 收货人姓名
    /// </summary>
    [Required(ErrorMessage = "收货人姓名不能为空")]
    [StringLength(50, ErrorMessage = "收货人姓名长度不能超过50")]
    public string ReceiverName { get; set; } = string.Empty;

    /// <summary>
    /// 收货人电话
    /// </summary>
    [Required(ErrorMessage = "收货人电话不能为空")]
    [StringLength(20, ErrorMessage = "收货人电话长度不能超过20")]
    public string ReceiverPhone { get; set; } = string.Empty;

    /// <summary>
    /// 收货地址
    /// </summary>
    [Required(ErrorMessage = "收货地址不能为空")]
    [StringLength(200, ErrorMessage = "收货地址长度不能超过200")]
    public string ReceiverAddress { get; set; } = string.Empty;

    /// <summary>
    /// 优惠券ID
    /// </summary>
    public string? CouponId { get; set; }

    /// <summary>
    /// 订单备注
    /// </summary>
    [StringLength(500, ErrorMessage = "备注长度不能超过500")]
    public string? Remark { get; set; }
}