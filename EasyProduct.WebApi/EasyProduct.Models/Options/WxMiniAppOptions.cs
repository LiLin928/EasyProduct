namespace EasyProduct.Models.Options;

/// <summary>
/// 微信小程序配置
/// </summary>
/// <remarks>
/// 配置微信小程序的 AppId、Secret 等信息。
/// 在 appsettings.json 中配置。
/// </remarks>
public class WxMiniAppOptions
{
    /// <summary>
    /// 配置节名称
    /// </summary>
    public const string SectionName = "WxMiniApp";

    /// <summary>
    /// 小程序 AppId
    /// </summary>
    public string AppId { get; set; } = string.Empty;

    /// <summary>
    /// 小程序 Secret
    /// </summary>
    public string Secret { get; set; } = string.Empty;

    /// <summary>
    /// 是否启用（默认启用）
    /// </summary>
    public bool Enabled { get; set; } = true;
}