using EasyProduct.Models.Enums;

namespace EasyProduct.Models.Dto.Site.News;

/// <summary>
/// 新闻 DTO
/// </summary>
/// <remarks>
/// 用于返回新闻信息，包含所有新闻字段和分类名称
/// 对应实体类 site_news
/// </remarks>
public class NewsDto
{
    /// <summary>
    /// 新闻ID
    /// </summary>
    /// <remarks>
    /// GUID 主键，唯一标识
    /// </remarks>
    public Guid Id { get; set; }

    /// <summary>
    /// 分类ID
    /// </summary>
    /// <remarks>
    /// 关联 site_news_category 表的分类ID
    /// </remarks>
    public string CategoryId { get; set; } = string.Empty;

    /// <summary>
    /// 分类名称
    /// </summary>
    /// <remarks>
    /// 新闻所属分类的名称，通过关联查询获取
    /// </remarks>
    public string? CategoryName { get; set; }

    /// <summary>
    /// 新闻标题
    /// </summary>
    /// <remarks>
    /// 新闻的标题，用于列表展示和详情页标题
    /// </remarks>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 新闻摘要
    /// </summary>
    /// <remarks>
    /// 新闻的简短描述，用于列表页展示
    /// </remarks>
    public string? Summary { get; set; }

    /// <summary>
    /// 新闻内容
    /// </summary>
    /// <remarks>
    /// 新闻的详细内容，支持富文本格式
    /// </remarks>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// 封面图片
    /// </summary>
    /// <remarks>
    /// 新闻封面图片URL，用于列表页展示
    /// </remarks>
    public string? CoverImage { get; set; }

    /// <summary>
    /// 作者
    /// </summary>
    /// <remarks>
    /// 新闻作者名称
    /// </remarks>
    public string? Author { get; set; }

    /// <summary>
    /// 来源
    /// </summary>
    /// <remarks>
    /// 新闻来源，如"原创"、"转载"等
    /// </remarks>
    public string? Source { get; set; }

    /// <summary>
    /// 浏览量
    /// </summary>
    /// <remarks>
    /// 新闻被浏览的次数统计
    /// </remarks>
    public int ViewCount { get; set; }

    /// <summary>
    /// 发布时间
    /// </summary>
    /// <remarks>
    /// 新闻正式发布的时间
    /// </remarks>
    public DateTime? PublishTime { get; set; }

    /// <summary>
    /// 是否置顶
    /// </summary>
    /// <remarks>
    /// 是否在列表中置顶显示
    /// 0=不置顶，1=置顶
    /// </remarks>
    public int IsTop { get; set; }

    /// <summary>
    /// 排序号
    /// </summary>
    /// <remarks>
    /// 用于控制新闻在列表中的显示顺序
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