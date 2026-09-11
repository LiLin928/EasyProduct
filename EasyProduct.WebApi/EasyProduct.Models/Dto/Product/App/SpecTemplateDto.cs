namespace EasyProduct.Models.Dto.Product.App;

/// <summary>
/// 规格模板 DTO
/// </summary>
/// <remarks>
/// 用于展示商品的所有规格组合，方便前端渲染规格选择器
/// </remarks>
public class SpecTemplateDto
{
    /// <summary>
    /// 规格列表
    /// </summary>
    /// <remarks>
    /// 包含所有规格定义，如颜色、尺寸等
    /// </remarks>
    public List<SpecDefinitionDto> Specs { get; set; } = new();
}