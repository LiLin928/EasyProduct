using System.ComponentModel.DataAnnotations;

namespace EasyProduct.Models.Dto.Site.Video;

/// <summary>
/// 更新视频请求 DTO
/// </summary>
/// <remarks>
/// 用于更新视频信息
/// 所有字段均为可选，只更新传入的非空字段
/// </remarks>
public class UpdateVideoDto
{
    /// <summary>
    /// 视频ID
    /// </summary>
    /// <remarks>
    /// 必填，GUID 字符串
    /// </remarks>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// 视频标题
    /// </summary>
    /// <remarks>
    /// 可选，最大长度 200 个字符
    /// </remarks>
    [StringLength(200, ErrorMessage = "视频标题长度不能超过200个字符")]
    public string? Title { get; set; }

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
    /// 可选，最大长度 500 个字符
    /// </remarks>
    [StringLength(500, ErrorMessage = "视频URL长度不能超过500个字符")]
    public string? VideoUrl { get; set; }

    /// <summary>
    /// 视频类型
    /// </summary>
    /// <remarks>
    /// 可选，最大长度 20 个字符
    /// </remarks>
    [StringLength(20, ErrorMessage = "视频类型长度不能超过20个字符")]
    public string? VideoType { get; set; }

    /// <summary>
    /// 视频时长（秒）
    /// </summary>
    /// <remarks>
    /// 可选
    /// </remarks>
    public int? Duration { get; set; }

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
    /// 可选，数值越小越靠前
    /// </remarks>
    public int? Sort { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    /// <remarks>
    /// 可选，使用 Status 枚举值：0=禁用，1=启用
    /// </remarks>
    public int? Status { get; set; }
}