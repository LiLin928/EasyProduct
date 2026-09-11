using System.ComponentModel.DataAnnotations;

namespace EasyProduct.Models.Dto.Site.Video;

/// <summary>
/// 创建视频请求 DTO
/// </summary>
/// <remarks>
/// 用于创建新的视频记录
/// 必填字段：Title、VideoUrl
/// 可选字段：TitleEn、Description、DescriptionEn、CoverImage、VideoType、Duration、Category、Sort
/// </remarks>
public class CreateVideoDto
{
    /// <summary>
    /// 视频标题
    /// </summary>
    /// <remarks>
    /// 必填，最大长度 200 个字符
    /// </remarks>
    [Required(ErrorMessage = "视频标题不能为空")]
    [StringLength(200, MinimumLength = 1, ErrorMessage = "视频标题长度必须在1-200个字符之间")]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 视频标题（英文）
    /// </summary>
    /// <remarks>
    /// 可选，最大长度 200 个字符
    /// </remarks>
    [StringLength(200, ErrorMessage = "视频标题（英文）长度不能超过200个字符")]
    public string? TitleEn { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    /// <remarks>
    /// 可选，最大长度 500 个字符
    /// </remarks>
    [StringLength(500, ErrorMessage = "描述长度不能超过500个字符")]
    public string? Description { get; set; }

    /// <summary>
    /// 描述（英文）
    /// </summary>
    /// <remarks>
    /// 可选，最大长度 500 个字符
    /// </remarks>
    [StringLength(500, ErrorMessage = "描述（英文）长度不能超过500个字符")]
    public string? DescriptionEn { get; set; }

    /// <summary>
    /// 封面图片URL
    /// </summary>
    /// <remarks>
    /// 可选，最大长度 500 个字符
    /// </remarks>
    [StringLength(500, ErrorMessage = "封面图片URL长度不能超过500个字符")]
    public string? CoverImage { get; set; }

    /// <summary>
    /// 视频URL
    /// </summary>
    /// <remarks>
    /// 必填，最大长度 500 个字符
    /// </remarks>
    [Required(ErrorMessage = "视频URL不能为空")]
    [StringLength(500, ErrorMessage = "视频URL长度不能超过500个字符")]
    public string VideoUrl { get; set; } = string.Empty;

    /// <summary>
    /// 视频类型
    /// </summary>
    /// <remarks>
    /// 可选，最大长度 20 个字符
    /// 枚举值：mp4、webm、external
    /// 默认为 mp4
    /// </remarks>
    [StringLength(20, ErrorMessage = "视频类型长度不能超过20个字符")]
    public string? VideoType { get; set; } = "mp4";

    /// <summary>
    /// 视频时长（秒）
    /// </summary>
    /// <remarks>
    /// 可选，默认为 0
    /// </remarks>
    [Range(0, int.MaxValue, ErrorMessage = "视频时长必须大于或等于0")]
    public int Duration { get; set; } = 0;

    /// <summary>
    /// 分类
    /// </summary>
    /// <remarks>
    /// 可选，最大长度 50 个字符
    /// </remarks>
    [StringLength(50, ErrorMessage = "分类长度不能超过50个字符")]
    public string? Category { get; set; }

    /// <summary>
    /// 排序号
    /// </summary>
    /// <remarks>
    /// 数值越小越靠前，默认为 0
    /// </remarks>
    [Range(0, int.MaxValue, ErrorMessage = "排序值必须大于或等于0")]
    public int Sort { get; set; } = 0;
}