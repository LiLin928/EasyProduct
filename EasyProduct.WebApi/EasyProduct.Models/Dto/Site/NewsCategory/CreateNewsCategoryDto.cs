using System.ComponentModel.DataAnnotations;

namespace EasyProduct.Models.Dto.Site.NewsCategory;

/// <summary>
/// 创建新闻分类请求 DTO
/// </summary>
/// <remarks>
/// 用于创建新的新闻分类
/// 必填字段：CategoryName
/// 可选字段：CategoryCode、Sort
/// </remarks>
public class CreateNewsCategoryDto
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
    /// 分类编码
    /// </summary>
    /// <remarks>
    /// 分类的唯一编码，用于程序中标识分类
    /// 可选字段，最大长度 50 个字符
    /// 示例：company_news, industry_news
    /// </remarks>
    [StringLength(50, ErrorMessage = "分类编码长度不能超过50个字符")]
    public string? CategoryCode { get; set; }

    /// <summary>
    /// 排序号
    /// </summary>
    /// <remarks>
    /// 用于控制分类在列表中的显示顺序
    /// 数值越小越靠前，默认为 0
    /// </remarks>
    [Range(0, int.MaxValue, ErrorMessage = "排序值必须大于或等于0")]
    public int Sort { get; set; } = 0;
}