namespace EasyProduct.Models.Dto.Product.App;

/// <summary>
/// 分类树 DTO
/// </summary>
public class CategoryTreeDto
{
    /// <summary>
    /// 分类 ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 分类名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 图标
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// 图片
    /// </summary>
    public string? Image { get; set; }

    /// <summary>
    /// 子分类
    /// </summary>
    public List<CategoryTreeDto>? Children { get; set; }
}