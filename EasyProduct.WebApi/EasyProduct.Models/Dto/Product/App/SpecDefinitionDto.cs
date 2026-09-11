namespace EasyProduct.Models.Dto.Product.App;

/// <summary>
/// 规格定义 DTO
/// </summary>
/// <remarks>
/// 用于表示单个规格的所有可选值，如"颜色：[红色, 蓝色, 绿色]"
/// </remarks>
public class SpecDefinitionDto
{
    /// <summary>
    /// 规格名称
    /// </summary>
    /// <example>
    /// "颜色"、"尺寸"、"材质"
    /// </example>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 可选值
    /// </summary>
    /// <remarks>
    /// 该规格所有可选的值列表
    /// </remarks>
    /// <example>
    /// ["红色", "蓝色", "绿色"]
    /// </example>
    public List<string> Values { get; set; } = new();
}