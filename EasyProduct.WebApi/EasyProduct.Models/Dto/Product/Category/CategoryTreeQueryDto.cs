namespace EasyProduct.Models.Dto.Product.Category;

/// <summary>
/// 商品分类树形查询 DTO
/// </summary>
/// <remarks>
/// 用于查询分类树形结构数据
/// 支持按父级分类筛选、仅查询启用状态等条件
/// </remarks>
public class CategoryTreeQueryDto
{
    /// <summary>
    /// 父分类ID
    /// </summary>
    /// <remarks>
    /// 指定父分类ID时，只返回该分类下的子分类
    /// 为空时返回所有分类
    /// </remarks>
    public string? ParentId { get; set; }

    /// <summary>
    /// 是否仅查询启用的分类
    /// </summary>
    /// <remarks>
    /// true: 仅返回 Status=Enabled 的分类
    /// false: 返回所有状态的分类
    /// </remarks>
    public bool? OnlyEnabled { get; set; }
}