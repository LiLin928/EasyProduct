using SqlSugar;
using EasyProduct.Models.Entitys.Base;

namespace EasyProduct.Models.Entitys.Site;

/// <summary>
/// 关于我们实体
/// </summary>
/// <remarks>
/// 对应数据库表 site_about
/// 继承 BaseEntity 基类，自动包含 Id、IsDeleted、Status、CreatedAt、UpdatedAt、CreatedBy、UpdatedBy 字段
/// 用于管理网站"关于我们"页面内容，支持中英文双语
/// 单页内容，一个语言版本一条记录
/// </remarks>
[SugarTable("site_about", "关于我们表")]
public class site_about : BaseEntity
{
    /// <summary>
    /// 标题
    /// </summary>
    /// <remarks>
    /// 关于我们页面的标题，必填字段
    /// 最大长度200字符
    /// </remarks>
    [SugarColumn(Length = 200, ColumnDescription = "标题")]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 标题（英文）
    /// </summary>
    /// <remarks>
    /// 关于我们页面的英文标题
    /// 可选字段，最大长度200字符
    /// </remarks>
    [SugarColumn(Length = 200, IsNullable = true, ColumnDescription = "标题（英文）")]
    public string? TitleEn { get; set; }

    /// <summary>
    /// 副标题
    /// </summary>
    /// <remarks>
    /// 关于我们页面的副标题
    /// 可选字段，最大长度500字符
    /// </remarks>
    [SugarColumn(Length = 500, IsNullable = true, ColumnDescription = "副标题")]
    public string? Subtitle { get; set; }

    /// <summary>
    /// 副标题（英文）
    /// </summary>
    /// <remarks>
    /// 关于我们页面的英文副标题
    /// 可选字段，最大长度500字符
    /// </remarks>
    [SugarColumn(Length = 500, IsNullable = true, ColumnDescription = "副标题（英文）")]
    public string? SubtitleEn { get; set; }

    /// <summary>
    /// 内容
    /// </summary>
    /// <remarks>
    /// 关于我们页面的详细内容，支持富文本
    /// 可选字段，使用 LONGTEXT 类型存储大量文本
    /// </remarks>
    [SugarColumn(ColumnDataType = "LONGTEXT", IsNullable = true, ColumnDescription = "内容（富文本）")]
    public string? Content { get; set; }

    /// <summary>
    /// 内容（英文）
    /// </summary>
    /// <remarks>
    /// 关于我们页面的英文详细内容，支持富文本
    /// 可选字段，使用 LONGTEXT 类型存储大量文本
    /// </remarks>
    [SugarColumn(ColumnDataType = "LONGTEXT", IsNullable = true, ColumnDescription = "内容（英文，富文本）")]
    public string? ContentEn { get; set; }

    /// <summary>
    /// 封面图片URL
    /// </summary>
    /// <remarks>
    /// 关于我们页面的封面图片URL地址
    /// 可选字段，最大长度500字符
    /// </remarks>
    [SugarColumn(Length = 500, IsNullable = true, ColumnDescription = "封面图片URL")]
    public string? CoverImage { get; set; }

    /// <summary>
    /// SEO关键词
    /// </summary>
    /// <remarks>
    /// 用于SEO优化的关键词
    /// 可选字段，最大长度200字符
    /// </remarks>
    [SugarColumn(Length = 200, IsNullable = true, ColumnDescription = "SEO关键词")]
    public string? Keywords { get; set; }

    /// <summary>
    /// SEO描述
    /// </summary>
    /// <remarks>
    /// 用于SEO优化的页面描述
    /// 可选字段，最大长度500字符
    /// </remarks>
    [SugarColumn(Length = 500, IsNullable = true, ColumnDescription = "SEO描述")]
    public string? Description { get; set; }
}