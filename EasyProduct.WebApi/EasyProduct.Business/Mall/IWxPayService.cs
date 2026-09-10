namespace EasyProduct.Business.Mall;

/// <summary>
/// 微信支付服务接口
/// </summary>
/// <remarks>
/// 提供微信小程序 JSAPI 支付功能。
/// </remarks>
public interface IWxPayService
{
    /// <summary>
    /// 创建 JSAPI 支付订单
    /// </summary>
    /// <param name="orderId">订单 ID</param>
    /// <param name="memberId">会员 ID</param>
    /// <param name="openid">微信 openid</param>
    /// <returns>支付参数</returns>
    /// <remarks>
    /// 1. 创建支付单（状态：pending）。
    /// 2. 调用微信 JSAPI 下单接口。
    /// 3. 返回前端支付所需的参数（timeStamp、nonceStr、package、signType、paySign）。
    /// </remarks>
    Task<JsapiPayParams> CreateJsapiOrderAsync(string orderId, string memberId, string openid);

    /// <summary>
    /// 处理支付回调
    /// </summary>
    /// <param name="callbackData">回调数据</param>
    /// <returns>处理结果</returns>
    /// <remarks>
    /// 1. 验证签名。
    /// 2. 更新支付单状态（success）。
    /// 3. 更新订单状态（paid）。
    /// 4. 记录支付流水。
    /// 5. 返回成功响应给微信。
    /// </remarks>
    Task<bool> HandlePayCallbackAsync(string callbackData);

    /// <summary>
    /// 查询支付状态
    /// </summary>
    /// <param name="paymentId">支付单 ID</param>
    /// <returns>支付状态</returns>
    Task<PaymentStatus> QueryPayStatusAsync(string paymentId);
}

/// <summary>
/// JSAPI 支付参数
/// </summary>
public class JsapiPayParams
{
    /// <summary>
    /// 时间戳
    /// </summary>
    public string TimeStamp { get; set; } = string.Empty;

    /// <summary>
    /// 随机字符串
    /// </summary>
    public string NonceStr { get; set; } = string.Empty;

    /// <summary>
    /// 订单详情扩展字符串
    /// </summary>
    public string Package { get; set; } = string.Empty;

    /// <summary>
    /// 签名方式
    /// </summary>
    public string SignType { get; set; } = "RSA";

    /// <summary>
    /// 签名
    /// </summary>
    public string PaySign { get; set; } = string.Empty;
}

/// <summary>
/// 支付状态
/// </summary>
public class PaymentStatus
{
    /// <summary>
    /// 支付单 ID
    /// </summary>
    public string PaymentId { get; set; } = string.Empty;

    /// <summary>
    /// 订单 ID
    /// </summary>
    public string OrderId { get; set; } = string.Empty;

    /// <summary>
    /// 状态（pending、success、failed）
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// 支付时间
    /// </summary>
    public DateTime? PayTime { get; set; }
}