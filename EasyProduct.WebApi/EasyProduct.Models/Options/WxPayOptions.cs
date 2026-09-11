namespace EasyProduct.Models.Options;

/// <summary>
/// 微信支付配置
/// </summary>
public class WxPayOptions
{
    /// <summary>
    /// 是否启用微信支付
    /// </summary>
    /// <remarks>
    /// 开发环境可设置为 false，使用模拟支付
    /// </remarks>
    public bool Enabled { get; set; } = false;

    /// <summary>
    /// 小程序 AppId
    /// </summary>
    public string AppId { get; set; } = string.Empty;

    /// <summary>
    /// 商户号（从环境变量读取）
    /// </summary>
    /// <remarks>
    /// 环境变量：WX_PAY_MCH_ID
    /// </remarks>
    public string MchId { get; set; } = string.Empty;

    /// <summary>
    /// 商户密钥（从环境变量读取）
    /// </summary>
    /// <remarks>
    /// 环境变量：WX_PAY_API_KEY
    /// </remarks>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// 证书序列号（从环境变量读取）
    /// </summary>
    /// <remarks>
    /// 环境变量：WX_PAY_CERT_SERIAL
    /// </remarks>
    public string CertSerialNo { get; set; } = string.Empty;

    /// <summary>
    /// 证书路径
    /// </summary>
    /// <remarks>
    /// 相对于项目根目录，默认：certs/apiclient_key.pem
    /// </remarks>
    public string CertPath { get; set; } = "certs/apiclient_key.pem";

    /// <summary>
    /// 回调通知 URL
    /// </summary>
    /// <remarks>
    /// 必须是外网可访问的 HTTPS URL
    /// </remarks>
    public string NotifyUrl { get; set; } = string.Empty;

    /// <summary>
    /// 支付超时时间（分钟）
    /// </summary>
    /// <remarks>
    /// 超过此时间未支付的订单将自动关闭
    /// </remarks>
    public int ExpireMinutes { get; set; } = 30;
}