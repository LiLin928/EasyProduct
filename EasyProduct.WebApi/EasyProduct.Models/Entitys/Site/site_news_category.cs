using SqlSugar;
using EasyProduct.Models.Entitys.Base;

namespace EasyProduct.Models.Entitys.Site;

/// <summary>
/// 新闻分类实体
/// </summary>
/// <remarks>
/// 对应数据库表 site_news_category
/// 继承 BaseEntity 基类，自动包含 Id、IsDeleted、Status、CreatedAt、UpdatedAt、CreatedBy、UpdatedBy 字段
/// 用于管理新闻的分类信息，支持分类层级和排序
/// </remarks>
[SugarTable("site_news_category", "新闻分类表")]
public class site_news_category : BaseEntity
{
    /// <summary>
    /// 分类名称
    /// </summary>
    /// <remarks>
    /// 新闻分类的显示名称，如"公司新闻"、"行业动态"等
    /// 必填字段，最大长度100字符
    /// </remarks>
    [SugarColumn(Length = 100, ColumnDescription = "分类名称")]
    public string CategoryName { get; set; } = string.Empty;

    /// <summary>
    /// 分类编码
    /// </summary>
    /// <remarks>
    /// 分类的唯一编码，用于程序中标识分类
    /// 可选字段，最大长度50字符
    /// 示例：company_news, industry_news
    /// </remarks>
    [SugarColumn(Length = 50, IsNullable = true, ColumnDescription = "分类编码")]
    public string? CategoryCode { get; set; }

    /// <summary>
    /// 排序号
    /// </summary>
    /// <remarks>
    /// 用于控制分类在列表中的显示顺序
    /// 数值越小越靠前，默认值为0
    /// </remarks>
    [SugarColumn(ColumnDescription = "排序号")]
    public int Sort { get; set; } = 0;
}