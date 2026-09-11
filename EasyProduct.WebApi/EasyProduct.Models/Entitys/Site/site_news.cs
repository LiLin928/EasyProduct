using SqlSugar;
using EasyProduct.Models.Entitys.Base;

namespace EasyProduct.Models.Entitys.Site;

/// <summary>
/// 新闻实体
/// </summary>
/// <remarks>
/// 对应数据库表 site_news
/// 继承 BaseEntity 基类，自动包含 Id、IsDeleted、Status、CreatedAt、UpdatedAt、CreatedBy、UpdatedBy 字段
/// 用于管理新闻文章信息，包括新闻标题、内容、分类等
/// </remarks>
[SugarTable("site_news", "新闻表")]
public class site_news : BaseEntity
{
    /// <summary>
    /// 分类ID
    /// </summary>
    /// <remarks>
    /// 关联 site_news_category 表的分类ID
    /// 必填字段，用于将新闻归类到相应分类
    /// </remarks>
    [SugarColumn(ColumnDataType = "varchar(36)", ColumnDescription = "分类ID")]
    public string CategoryId { get; set; } = string.Empty;

    /// <summary>
    /// 新闻标题
    /// </summary>
    /// <remarks>
    /// 新闻的标题，用于列表展示和详情页标题
    /// 必填字段，最大长度200字符
    /// </remarks>
    [SugarColumn(Length = 200, ColumnDescription = "新闻标题")]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 新闻摘要
    /// </summary>
    /// <remarks>
    /// 新闻的简短描述，用于列表页展示
    /// 可选字段，最大长度500字符
    /// </remarks>
    [SugarColumn(Length = 500, IsNullable = true, ColumnDescription = "新闻摘要")]
    public string? Summary { get; set; }

    /// <summary>
    /// 新闻内容
    /// </summary>
    /// <remarks>
    /// 新闻的详细内容，支持富文本格式
    /// 必填字段，存储为 TEXT 类型
    /// </remarks>
    [SugarColumn(ColumnDataType = "text", ColumnDescription = "新闻内容")]
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// 封面图片
    /// </summary>
    /// <remarks>
    /// 新闻封面图片URL，用于列表页展示
    /// 可选字段，最大长度500字符
    /// </remarks>
    [SugarColumn(Length = 500, IsNullable = true, ColumnDescription = "封面图片")]
    public string? CoverImage { get; set; }

    /// <summary>
    /// 作者
    /// </summary>
    /// <remarks>
    /// 新闻作者名称
    /// 可选字段，最大长度50字符
    /// </remarks>
    [SugarColumn(Length = 50, IsNullable = true, ColumnDescription = "作者")]
    public string? Author { get; set; }

    /// <summary>
    /// 来源
    /// </summary>
    /// <remarks>
    /// 新闻来源，如"原创"、"转载"等
    /// 可选字段，最大长度100字符
    /// </remarks>
    [SugarColumn(Length = 100, IsNullable = true, ColumnDescription = "来源")]
    public string? Source { get; set; }

    /// <summary>
    /// 浏览量
    /// </summary>
    /// <remarks>
    /// 新闻被浏览的次数统计
    /// 默认值为0，每次浏览自动+1
    /// </remarks>
    [SugarColumn(ColumnDescription = "浏览量")]
    public int ViewCount { get; set; } = 0;

    /// <summary>
    /// 发布时间
    /// </summary>
    /// <remarks>
    /// 新闻正式发布的时间，可用于定时发布功能
    /// 可选字段，为空时表示创建时即发布
    /// </remarks>
    [SugarColumn(IsNullable = true, ColumnDescription = "发布时间")]
    public DateTime? PublishTime { get; set; }

    /// <summary>
    /// 是否置顶
    /// </summary>
    /// <remarks>
    /// 是否在列表中置顶显示
    /// 0=不置顶，1=置顶
    /// 默认值为0
    /// </remarks>
    [SugarColumn(ColumnDescription = "是否置顶")]
    public int IsTop { get; set; } = 0;

    /// <summary>
    /// 排序号
    /// </summary>
    /// <remarks>
    /// 用于控制新闻在列表中的显示顺序
    /// 数值越小越靠前，默认值为0
    /// </remarks>
    [SugarColumn(ColumnDescription = "排序号")]
    public int Sort { get; set; } = 0;
}