using System.ComponentModel.DataAnnotations;

namespace EasyProduct.Models.Dto.Site.Download;

/// <summary>
/// 更新下载请求 DTO
/// </summary>
/// <remarks>
/// 用于更新下载信息
/// 所有字段均为可选，只更新传入的非空字段
/// </remarks>
public class UpdateDownloadDto
{
    /// <summary>
    /// 下载ID
    /// </summary>
    /// <remarks>
    /// 必填，GUID 字符串
    /// </remarks>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// 下载标题
    /// </summary>
    /// <remarks>
    /// 可选，最大长度 200 个字符
    /// </remarks>
    [StringLength(200, ErrorMessage = "下载标题长度不能超过200个字符")]
    public string? Title { get; set; }

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
    /// 可选，最大长度 500 个字符
    /// </remarks>
    [StringLength(500, ErrorMessage = "文件URL长度不能超过500个字符")]
    public string? FileUrl { get; set; }

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
    /// 可选
    /// </remarks>
    public long? FileSize { get; set; }

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