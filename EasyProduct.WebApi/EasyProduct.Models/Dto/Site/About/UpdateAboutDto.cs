using System.ComponentModel.DataAnnotations;

namespace EasyProduct.Models.Dto.Site.About;

/// <summary>
/// 更新关于我们请求 DTO
/// </summary>
/// <remarks>
/// 用于更新关于我们信息
/// 所有字段均为可选，只更新传入的非空字段
/// </remarks>
public class UpdateAboutDto
{
    /// <summary>
    /// 标题
    /// </summary>
    /// <remarks>
    /// 可选，最大长度 200 个字符
    /// </remarks>
    [StringLength(200, ErrorMessage = "标题长度不能超过200个字符")]
    public string? Title { get; set; }

    /// <summary>
    /// 标题（英文）
    /// </summary>
    /// <remarks>
    /// 可选，最大长度 200 个字符
    /// </remarks>
    [StringLength(200, ErrorMessage = "标题（英文）长度不能超过200个字符")]
    public string? TitleEn { get; set; }

    /// <summary>
    /// 副标题
    /// </summary>
    /// <remarks>
    /// 可选，最大长度 500 个字符
    /// </remarks>
    [StringLength(500, ErrorMessage = "副标题长度不能超过500个字符")]
    public string? Subtitle { get; set; }

    /// <summary>
    /// 副标题（英文）
    /// </summary>
    /// <remarks>
    /// 可选，最大长度 500 个字符
    /// </remarks>
    [StringLength(500, ErrorMessage = "副标题（英文）长度不能超过500个字符")]
    public string? SubtitleEn { get; set; }

    /// <summary>
    /// 内容
    /// </summary>
    /// <remarks>
    /// 可选，支持富文本
    /// </remarks>
    public string? Content { get; set; }

    /// <summary>
    /// 内容（英文）
    /// </summary>
    /// <remarks>
    /// 可选，支持富文本
    /// </remarks>
    public string? ContentEn { get; set; }

    /// <summary>
    /// 封面图片URL
    /// </summary>
    /// <remarks>
    /// 可选，最大长度 500 个字符
    /// </remarks>
    [StringLength(500, ErrorMessage = "封面图片URL长度不能超过500个字符")]
    public string? CoverImage { get; set; }

    /// <summary>
    /// SEO关键词
    /// </summary>
    /// <remarks>
    /// 可选，最大长度 200 个字符
    /// </remarks>
    [StringLength(200, ErrorMessage = "SEO关键词长度不能超过200个字符")]
    public string? Keywords { get; set; }

    /// <summary>
    /// SEO描述
    /// </summary>
    /// <remarks>
    /// 可选，最大长度 500 个字符
    /// </remarks>
    [StringLength(500, ErrorMessage = "SEO描述长度不能超过500个字符")]
    public string? Description { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    /// <remarks>
    /// 可选，使用 Status 枚举值：0=禁用，1=启用
    /// </remarks>
    public int? Status { get; set; }
}