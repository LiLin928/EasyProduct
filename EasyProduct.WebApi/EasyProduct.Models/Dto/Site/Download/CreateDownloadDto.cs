using System.ComponentModel.DataAnnotations;

namespace EasyProduct.Models.Dto.Site.Download;

/// <summary>
/// 创建下载请求 DTO
/// </summary>
/// <remarks>
/// 用于创建新的下载记录
/// 必填字段：Title、FileUrl
/// 可选字段：TitleEn、Description、DescriptionEn、FileName、FileSize、FileType、Category、Sort
/// </remarks>
public class CreateDownloadDto
{
    /// <summary>
    /// 下载标题
    /// </summary>
    /// <remarks>
    /// 必填，最大长度 200 个字符
    /// </remarks>
    [Required(ErrorMessage = "下载标题不能为空")]
    [StringLength(200, MinimumLength = 1, ErrorMessage = "下载标题长度必须在1-200个字符之间")]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 下载标题（英文）
    /// </summary>
    /// <remarks>
    /// 可选，最大长度 200 个字符
    /// </remarks>
    [StringLength(200, ErrorMessage = "下载标题（英文）长度不能超过200个字符")]
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
    /// 文件URL
    /// </summary>
    /// <remarks>
    /// 必填，最大长度 500 个字符
    /// </remarks>
    [Required(ErrorMessage = "文件URL不能为空")]
    [StringLength(500, ErrorMessage = "文件URL长度不能超过500个字符")]
    public string FileUrl { get; set; } = string.Empty;

    /// <summary>
    /// 文件名称
    /// </summary>
    /// <remarks>
    /// 可选，最大长度 200 个字符
    /// </remarks>
    [StringLength(200, ErrorMessage = "文件名称长度不能超过200个字符")]
    public string? FileName { get; set; }

    /// <summary>
    /// 文件大小（字节）
    /// </summary>
    /// <remarks>
    /// 可选，默认为 0
    /// </remarks>
    [Range(0, long.MaxValue, ErrorMessage = "文件大小必须大于或等于0")]
    public long FileSize { get; set; } = 0;

    /// <summary>
    /// 文件类型（扩展名）
    /// </summary>
    /// <remarks>
    /// 可选，最大长度 50 个字符
    /// </remarks>
    [StringLength(50, ErrorMessage = "文件类型长度不能超过50个字符")]
    public string? FileType { get; set; }

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