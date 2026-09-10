using System.ComponentModel.DataAnnotations;

namespace EasyProduct.Models.Dto.Mall.Payment;

/// <summary>
/// 创建支付单 DTO
/// </summary>
public class CreatePaymentDto
{
    /// <summary>
    /// 订单ID
    /// </summary>
    [Required(ErrorMessage = "订单ID不能为空")]
    public string OrderId { get; set; } = string.Empty;

    /// <summary>
    /// 支付方式
    /// </summary>
    [Required(ErrorMessage = "支付方式不能为空")]
    public string PaymentMethod { get; set; } = string.Empty;

    /// <summary>
    /// 支付渠道
    /// </summary>
    /// <remarks>
    /// jsapi: 小程序支付
    /// h5: H5支付
    /// native: 扫码支付
    /// app: APP支付
    /// </remarks>
    public string? PaymentChannel { get; set; }
}