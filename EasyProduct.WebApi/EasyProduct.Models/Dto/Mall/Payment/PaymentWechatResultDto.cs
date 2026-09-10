namespace EasyProduct.Models.Dto.Mall.Payment;

/// <summary>
/// 微信支付结果 DTO
/// </summary>
/// <remarks>
/// 用于返回微信支付所需的参数，供小程序调用支付接口
/// </remarks>
public class PaymentWechatResultDto
{
    /// <summary>
    /// 时间戳
    /// </summary>
    /// <remarks>
    /// 当前时间的时间戳，用于微信支付签名
    /// </remarks>
    public string TimeStamp { get; set; } = string.Empty;

    /// <summary>
    /// 随机字符串
    /// </summary>
    /// <remarks>
    /// 随机生成的字符串，用于微信支付签名
    /// </remarks>
    public string NonceStr { get; set; } = string.Empty;

    /// <summary>
    /// 订单详情扩展字符串
    /// </summary>
    /// <remarks>
    /// 格式为 prepay_id=xxx，由微信支付返回的预支付交易会话标识
    /// </remarks>
    public string Package { get; set; } = string.Empty;

    /// <summary>
    /// 签名方式
    /// </summary>
    /// <remarks>
    /// 签名类型，默认为 RSA
    /// </remarks>
    public string SignType { get; set; } = "RSA";

    /// <summary>
    /// 签名
    /// </summary>
    /// <remarks>
    /// 使用商户私钥对以上参数进行签名后的字符串
    /// </remarks>
    public string PaySign { get; set; } = string.Empty;
}