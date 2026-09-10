namespace EasyProduct.Models.Dto.Mall.Payment;

/// <summary>
/// 支付单 DTO
/// </summary>
public class PaymentDto
{
    /// <summary>
    /// 支付单ID
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// 支付单号
    /// </summary>
    public string PaymentNo { get; set; } = string.Empty;

    /// <summary>
    /// 订单ID
    /// </summary>
    public string OrderId { get; set; } = string.Empty;

    /// <summary>
    /// 会员ID
    /// </summary>
    public string MemberId { get; set; } = string.Empty;

    /// <summary>
    /// 支付金额
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// 支付方式
    /// </summary>
    public string PaymentMethod { get; set; } = string.Empty;

    /// <summary>
    /// 支付渠道
    /// </summary>
    public string PaymentChannel { get; set; } = string.Empty;

    /// <summary>
    /// 支付状态
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// 第三方交易号
    /// </summary>
    public string? ThirdPartyNo { get; set; }

    /// <summary>
    /// 支付时间
    /// </summary>
    public DateTime? PaymentTime { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; }
}