using System.ComponentModel.DataAnnotations;
using EasyProduct.Models.Enums;

namespace EasyProduct.Models.Dto.Product.Category;

/// <summary>
/// 商品分类查询 DTO
/// </summary>
/// <remarks>
/// 用于分类列表查询，支持按分类名称、分类编码、状态筛选
/// 包含分页参数
/// </remarks>
public class CategoryQueryDto
{
    /// <summary>
    /// 页码，从 1 开始
    /// </summary>
    public int PageIndex { get; set; } = 1;

    /// <summary>
    /// 每页数量
    /// </summary>
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// 分类名称关键词
    /// </summary>
    /// <remarks>
    /// 支持模糊搜索，最大长度 100 个字符
    /// </remarks>
    [StringLength(100, ErrorMessage = "分类名称长度不能超过100个字符")]
    public string? CategoryName { get; set; }

    /// <summary>
    /// 分类编码
    /// </summary>
    /// <remarks>
    /// 精确匹配，最大长度 50 个字符
    /// </remarks>
    [StringLength(50, ErrorMessage = "分类编码长度不能超过50个字符")]
    public string? CategoryCode { get; set; }

    /// <summary>
    /// 状态筛选
    /// </summary>
    /// <remarks>
    /// 使用 Status 枚举：Disabled（0）= 禁用，Enabled（1）= 启用
    /// </remarks>
    public Status? Status { get; set; }
}