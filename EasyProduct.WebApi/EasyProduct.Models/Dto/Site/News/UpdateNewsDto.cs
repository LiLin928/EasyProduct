using System.ComponentModel.DataAnnotations;

namespace EasyProduct.Models.Dto.Site.News;

/// <summary>
/// 更新新闻请求 DTO
/// </summary>
/// <remarks>
/// 用于更新已存在的新闻
/// 必填字段：Id
/// 可选字段：CategoryId、Title、Summary、Content、CoverImage、Author、Source、PublishTime、IsTop、Sort、Status
/// </remarks>
public class UpdateNewsDto
{
    /// <summary>
    /// 新闻ID
    /// </summary>
    /// <remarks>
    /// 必填，GUID 格式
    /// </remarks>
    [Required(ErrorMessage = "新闻ID不能为空")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// 分类ID
    /// </summary>
    /// <remarks>
    /// 可选，关联 site_news_category 表
    /// </remarks>
    public string? CategoryId { get; set; }

    /// <summary>
    /// 新闻标题
    /// </summary>
    /// <remarks>
    /// 可选，最大长度 200 个字符
    /// </remarks>
    [StringLength(200, MinimumLength = 1, ErrorMessage = "新闻标题长度必须在1-200个字符之间")]
    public string? Title { get; set; }

    /// <summary>
    /// 新闻摘要
    /// </summary>
    /// <remarks>
    /// 可选，最大长度 500 个字符
    /// </remarks>
    [StringLength(500, ErrorMessage = "新闻摘要长度不能超过500个字符")]
    public string? Summary { get; set; }

    /// <summary>
    /// 新闻内容
    /// </summary>
    /// <remarks>
    /// 可选，支持富文本格式
    /// </remarks>
    public string? Content { get; set; }

    /// <summary>
    /// 封面图片
    /// </summary>
    /// <remarks>
    /// 可选，最大长度 500 个字符
    /// </remarks>
    [StringLength(500, ErrorMessage = "封面图片URL长度不能超过500个字符")]
    public string? CoverImage { get; set; }

    /// <summary>
    /// 作者
    /// </summary>
    /// <remarks>
    /// 可选，最大长度 50 个字符
    /// </remarks>
    [StringLength(50, ErrorMessage = "作者名称长度不能超过50个字符")]
    public string? Author { get; set; }

    /// <summary>
    /// 来源
    /// </summary>
    /// <remarks>
    /// 可选，最大长度 100 个字符
    /// </remarks>
    [StringLength(100, ErrorMessage = "来源长度不能超过100个字符")]
    public string? Source { get; set; }

    /// <summary>
    /// 发布时间
    /// </summary>
    /// <remarks>
    /// 可选，为空时表示立即发布
    /// </remarks>
    public DateTime? PublishTime { get; set; }

    /// <summary>
    /// 是否置顶
    /// </summary>
    /// <remarks>
    /// 0=不置顶，1=置顶
    /// </remarks>
    [Range(0, 1, ErrorMessage = "是否置顶只能是0或1")]
    public int? IsTop { get; set; }

    /// <summary>
    /// 排序号
    /// </summary>
    /// <remarks>
    /// 数值越小越靠前
    /// </remarks>
    [Range(0, int.MaxValue, ErrorMessage = "排序值必须大于或等于0")]
    public int? Sort { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    /// <remarks>
    /// 0=禁用，1=启用
    /// </remarks>
    [Range(0, 1, ErrorMessage = "状态只能是0或1")]
    public int? Status { get; set; }

    /// <summary>
    /// 英文标题
    /// </summary>
    /// <remarks>
    /// 可选，用于多语言支持
    /// </remarks>
    [StringLength(200, ErrorMessage = "英文标题长度不能超过200个字符")]
    public string? TitleEn { get; set; }

    /// <summary>
    /// 英文摘要
    /// </summary>
    /// <remarks>
    /// 可选，用于多语言支持
    /// </remarks>
    [StringLength(500, ErrorMessage = "英文摘要长度不能超过500个字符")]
    public string? SummaryEn { get; set; }

    /// <summary>
    /// 英文内容
    /// </summary>
    /// <remarks>
    /// 可选，用于多语言支持
    /// </remarks>
    public string? ContentEn { get; set; }
}