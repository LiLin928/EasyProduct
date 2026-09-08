using EasyProduct.Models.Enums;

namespace EasyProduct.Models.Dto.Product.Category;

/// <summary>
/// 商品分类树形 DTO
/// </summary>
/// <remarks>
/// 用于返回树形结构的分类数据，包含所有分类字段和子分类列表
/// 主要用于前端树形选择器、导航菜单等场景
/// </remarks>
public class CategoryTreeDto
{
    /// <summary>
    /// 分类ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 父分类ID
    /// </summary>
    /// <remarks>
    /// 上级分类ID，根分类为空字符串
    /// </remarks>
    public string? ParentId { get; set; }

    /// <summary>
    /// 分类名称
    /// </summary>
    public string CategoryName { get; set; } = string.Empty;

    /// <summary>
    /// 分类编码
    /// </summary>
    /// <remarks>
    /// 分类的唯一编码，用于系统对接
    /// </remarks>
    public string? CategoryCode { get; set; }

    /// <summary>
    /// 分类图标
    /// </summary>
    /// <remarks>
    /// 图标 URL 或图标类名
    /// </remarks>
    public string? Icon { get; set; }

    /// <summary>
    /// 分类图片
    /// </summary>
    /// <remarks>
    /// 分类展示图片 URL
    /// </remarks>
    public string? Image { get; set; }

    /// <summary>
    /// 排序序号
    /// </summary>
    /// <remarks>
    /// 数字越小越靠前
    /// </remarks>
    public int Sort { get; set; }

    /// <summary>
    /// 层级
    /// </summary>
    /// <remarks>
    /// 分类层级，从 1 开始
    /// </remarks>
    public int Level { get; set; }

    /// <summary>
    /// 完整路径
    /// </summary>
    /// <remarks>
    /// 如：电子产品/手机/智能手机，用于快速定位和显示
    /// </remarks>
    public string? FullPath { get; set; }

    /// <summary>
    /// 是否显示在导航
    /// </summary>
    /// <remarks>
    /// 0=否，1=是
    /// </remarks>
    public int ShowInNav { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    /// <remarks>
    /// 使用 Status 枚举：Disabled（0）= 禁用，Enabled（1）= 启用
    /// </remarks>
    public Status Status { get; set; }

    /// <summary>
    /// 子分类列表
    /// </summary>
    /// <remarks>
    /// 递归包含该分类的所有子分类，形成树形结构
    /// </remarks>
    public List<CategoryTreeDto>? Children { get; set; }
}