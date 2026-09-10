namespace EasyProduct.Models.Dto.Mall.Payment;

/// <summary>
/// 支付回调 DTO
/// </summary>
/// <remarks>
/// 用于接收第三方支付平台的回调通知
/// </remarks>
public class PaymentCallbackDto
{
    /// <summary>
    /// 支付单号
    /// </summary>
    public string PaymentNo { get; set; } = string.Empty;

    /// <summary>
    /// 第三方交易号
    /// </summary>
    public string ThirdPartyNo { get; set; } = string.Empty;

    /// <summary>
    /// 支付状态
    /// </summary>
    /// <remarks>
    /// success: 支付成功
    /// failed: 支付失败
    /// </remarks>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// 支付时间
    /// </summary>
    public DateTime? PaymentTime { get; set; }

    /// <summary>
    /// 原始回调数据
    /// </summary>
    public string? RawData { get; set; }
}