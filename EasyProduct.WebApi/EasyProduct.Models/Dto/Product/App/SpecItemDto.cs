namespace EasyProduct.Models.Dto.Product.App;

/// <summary>
/// 规格项 DTO
/// </summary>
/// <remarks>
/// 用于表示 SKU 的单个规格，如"颜色：红色"
/// </remarks>
public class SpecItemDto
{
    /// <summary>
    /// 规格名称
    /// </summary>
    /// <example>
    /// "颜色"、"尺寸"、"材质"
    /// </example>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 规格值
    /// </summary>
    /// <example>
    /// "红色"、"L"、"棉"
    /// </example>
    public string Value { get; set; } = string.Empty;
}