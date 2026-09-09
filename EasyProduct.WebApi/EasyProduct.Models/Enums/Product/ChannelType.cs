namespace EasyProduct.Models.Enums.Product;

/// <summary>
/// 渠道类型枚举
/// </summary>
/// <remarks>
/// 定义商品可发布的渠道类型
/// </remarks>
public enum ChannelType
{
    /// <summary>
    /// 官网
    /// </summary>
    /// <remarks>
    /// 官网站点渠道
    /// </remarks>
    Site = 1,

    /// <summary>
    /// 小程序
    /// </summary>
    /// <remarks>
    /// 微信小程序渠道
    /// </remarks>
    MiniApp = 2,

    /// <summary>
    /// B2B
    /// </summary>
    /// <remarks>
    /// B2B批发渠道
    /// </remarks>
    B2B = 3
}

/// <summary>
/// 渠道编码常量
/// </summary>
/// <remarks>
/// 提供渠道编码的标准字符串常量
/// </remarks>
public static class ChannelCode
{
    /// <summary>
    /// 官网渠道编码
    /// </summary>
    public const string Site = "site";

    /// <summary>
    /// 小程序渠道编码
    /// </summary>
    public const string MiniApp = "miniapp";

    /// <summary>
    /// B2B渠道编码
    /// </summary>
    public const string B2B = "b2b";

    /// <summary>
    /// 所有渠道编码列表
    /// </summary>
    public static readonly string[] All = { Site, MiniApp, B2B };
}