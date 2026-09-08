using System.ComponentModel.DataAnnotations;

namespace EasyProduct.Models.Dto.Product.Category;

/// <summary>
/// 创建商品分类请求 DTO
/// </summary>
/// <remarks>
/// 用于创建新的商品分类
/// 必填字段：CategoryName
/// 可选字段：ParentId、CategoryCode、Icon、Image、Sort、ShowInNav
/// </remarks>
public class CreateCategoryDto
{
    /// <summary>
    /// 分类名称
    /// </summary>
    /// <remarks>
    /// 必填，最大长度 100 个字符
    /// </remarks>
    [Required(ErrorMessage = "分类名称不能为空")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "分类名称长度必须在1-100个字符之间")]
    public string CategoryName { get; set; } = string.Empty;

    /// <summary>
    /// 父分类ID
    /// </summary>
    /// <remarks>
    /// 上级分类ID，根分类传空字符串或 null
    /// GUID 字符串格式，最大长度 36 个字符
    /// </remarks>
    [StringLength(36, ErrorMessage = "父分类ID长度不能超过36个字符")]
    public string? ParentId { get; set; }

    /// <summary>
    /// 分类编码
    /// </summary>
    /// <remarks>
    /// 分类的唯一编码，用于系统对接
    /// 最大长度 50 个字符
    /// </remarks>
    [StringLength(50, ErrorMessage = "分类编码长度不能超过50个字符")]
    public string? CategoryCode { get; set; }

    /// <summary>
    /// 分类图标
    /// </summary>
    /// <remarks>
    /// 图标 URL 或图标类名
    /// 最大长度 255 个字符
    /// </remarks>
    [StringLength(255, ErrorMessage = "分类图标长度不能超过255个字符")]
    public string? Icon { get; set; }

    /// <summary>
    /// 分类图片
    /// </summary>
    /// <remarks>
    /// 分类展示图片 URL
    /// 最大长度 500 个字符
    /// </remarks>
    [StringLength(500, ErrorMessage = "分类图片长度不能超过500个字符")]
    public string? Image { get; set; }

    /// <summary>
    /// 排序序号
    /// </summary>
    /// <remarks>
    /// 数字越小越靠前，默认为 0
    /// </remarks>
    [Range(0, int.MaxValue, ErrorMessage = "排序值必须大于或等于0")]
    public int Sort { get; set; } = 0;

    /// <summary>
    /// 是否显示在导航
    /// </summary>
    /// <remarks>
    /// 0=否，1=是
    /// 默认为 1（显示）
    /// </remarks>
    [Range(0, 1, ErrorMessage = "是否显示在导航只能是0或1")]
    public int ShowInNav { get; set; } = 1;
}