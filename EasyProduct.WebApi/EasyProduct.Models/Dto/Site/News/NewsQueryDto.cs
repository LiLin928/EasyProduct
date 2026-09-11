using System.ComponentModel.DataAnnotations;
using EasyProduct.Models.Enums;

namespace EasyProduct.Models.Dto.Site.News;

/// <summary>
/// 新闻查询 DTO
/// </summary>
/// <remarks>
/// 用于新闻列表查询，支持按关键词、分类、状态、置顶、发布时间筛选
/// 包含分页参数
/// </remarks>
public class NewsQueryDto
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
    /// 关键词搜索
    /// </summary>
    /// <remarks>
    /// 支持模糊搜索新闻标题和摘要，最大长度 200 个字符
    /// </remarks>
    [StringLength(200, ErrorMessage = "关键词长度不能超过200个字符")]
    public string? Keyword { get; set; }

    /// <summary>
    /// 分类ID
    /// </summary>
    /// <remarks>
    /// 精确匹配分类ID
    /// </remarks>
    public string? CategoryId { get; set; }

    /// <summary>
    /// 状态筛选
    /// </summary>
    /// <remarks>
    /// 使用 Status 枚举：Disabled（0）= 禁用，Enabled（1）= 启用
    /// </remarks>
    public Status? Status { get; set; }

    /// <summary>
    /// 是否置顶
    /// </summary>
    /// <remarks>
    /// 筛选置顶新闻：null=全部，0=不置顶，1=置顶
    /// </remarks>
    public int? IsTop { get; set; }

    /// <summary>
    /// 发布时间开始
    /// </summary>
    /// <remarks>
    /// 筛选发布时间大于等于此时间的新闻
    /// </remarks>
    public DateTime? PublishTimeStart { get; set; }

    /// <summary>
    /// 发布时间结束
    /// </summary>
    /// <remarks>
    /// 筛选发布时间小于等于此时间的新闻
    /// </remarks>
    public DateTime? PublishTimeEnd { get; set; }
}