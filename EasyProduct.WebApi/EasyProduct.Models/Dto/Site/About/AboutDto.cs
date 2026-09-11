using EasyProduct.Models.Enums;

namespace EasyProduct.Models.Dto.Site.About;

/// <summary>
/// 关于我们 DTO
/// </summary>
/// <remarks>
/// 用于返回关于我们信息，包含所有字段
/// 对应实体类 site_about
/// </remarks>
public class AboutDto
{
    /// <summary>
    /// 关于我们ID
    /// </summary>
    /// <remarks>
    /// GUID 主键，唯一标识
    /// </remarks>
    public Guid Id { get; set; }

    /// <summary>
    /// 标题
    /// </summary>
    /// <remarks>
    /// 关于我们页面的标题
    /// </remarks>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 标题（英文）
    /// </summary>
    /// <remarks>
    /// 关于我们页面的英文标题
    /// </remarks>
    public string? TitleEn { get; set; }

    /// <summary>
    /// 副标题
    /// </summary>
    /// <remarks>
    /// 关于我们页面的副标题
    /// </remarks>
    public string? Subtitle { get; set; }

    /// <summary>
    /// 副标题（英文）
    /// </summary>
    /// <remarks>
    /// 关于我们页面的英文副标题
    /// </remarks>
    public string? SubtitleEn { get; set; }

    /// <summary>
    /// 内容
    /// </summary>
    /// <remarks>
    /// 关于我们页面的详细内容，支持富文本
    /// </remarks>
    public string? Content { get; set; }

    /// <summary>
    /// 内容（英文）
    /// </summary>
    /// <remarks>
    /// 关于我们页面的英文详细内容，支持富文本
    /// </remarks>
    public string? ContentEn { get; set; }

    /// <summary>
    /// 封面图片URL
    /// </summary>
    /// <remarks>
    /// 关于我们页面的封面图片URL地址
    /// </remarks>
    public string? CoverImage { get; set; }

    /// <summary>
    /// SEO关键词
    /// </summary>
    /// <remarks>
    /// 用于SEO优化的关键词
    /// </remarks>
    public string? Keywords { get; set; }

    /// <summary>
    /// SEO描述
    /// </summary>
    /// <remarks>
    /// 用于SEO优化的页面描述
    /// </remarks>
    public string? Description { get; set; }

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