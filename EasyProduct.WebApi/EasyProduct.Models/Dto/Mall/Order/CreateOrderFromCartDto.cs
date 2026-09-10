using System.ComponentModel.DataAnnotations;

namespace EasyProduct.Models.Dto.Mall.Order;

/// <summary>
/// 从购物车创建订单 DTO
/// </summary>
public class CreateOrderFromCartDto
{
    /// <summary>
    /// 购物车项ID列表
    /// </summary>
    [Required(ErrorMessage = "购物车项ID不能为空")]
    [MinLength(1, ErrorMessage = "至少选择一个商品")]
    public List<string> CartIds { get; set; } = new();

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