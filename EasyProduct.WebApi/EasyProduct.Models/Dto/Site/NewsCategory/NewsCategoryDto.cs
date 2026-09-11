using EasyProduct.Models.Enums;

namespace EasyProduct.Models.Dto.Site.NewsCategory;

/// <summary>
/// 新闻分类 DTO
/// </summary>
/// <remarks>
/// 用于返回新闻分类信息，包含所有分类字段
/// 对应实体类 site_news_category
/// </remarks>
public class NewsCategoryDto
{
    /// <summary>
    /// 分类ID
    /// </summary>
    /// <remarks>
    /// GUID 主键，唯一标识
    /// </remarks>
    public Guid Id { get; set; }

    /// <summary>
    /// 分类名称
    /// </summary>
    /// <remarks>
    /// 新闻分类的显示名称，如"公司新闻"、"行业动态"等
    /// </remarks>
    public string CategoryName { get; set; } = string.Empty;

    /// <summary>
    /// 分类编码
    /// </summary>
    /// <remarks>
    /// 分类的唯一编码，用于程序中标识分类
    /// 示例：company_news, industry_news
    /// </remarks>
    public string? CategoryCode { get; set; }

    /// <summary>
    /// 排序号
    /// </summary>
    /// <remarks>
    /// 用于控制分类在列表中的显示顺序
    /// 数值越小越靠前
    /// </remarks>
    public int Sort { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    /// <remarks>
    /// 使用 Status 枚举：Disabled（0）= 禁用，Enabled（1）= 启用
    /// </remarks>
    public Status Status { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// 创建人ID
    /// </summary>
    public string? CreatedBy { get; set; }
}