using EasyProduct.Models.Enums;

namespace EasyProduct.Models.Dto.Site.Download;

/// <summary>
/// 下载管理 DTO
/// </summary>
/// <remarks>
/// 用于返回下载信息，包含所有字段
/// 对应实体类 site_download
/// </remarks>
public class DownloadDto
{
    /// <summary>
    /// 下载ID
    /// </summary>
    /// <remarks>
    /// GUID 主键，唯一标识
    /// </remarks>
    public Guid Id { get; set; }

    /// <summary>
    /// 下载标题
    /// </summary>
    /// <remarks>
    /// 下载文件的标题
    /// </remarks>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 下载标题（英文）
    /// </summary>
    /// <remarks>
    /// 下载文件的英文标题
    /// </remarks>
    public string? TitleEn { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    /// <remarks>
    /// 下载文件的详细描述
    /// </remarks>
    public string? Description { get; set; }

    /// <summary>
    /// 描述（英文）
    /// </summary>
    /// <remarks>
    /// 下载文件的英文详细描述
    /// </remarks>
    public string? DescriptionEn { get; set; }

    /// <summary>
    /// 文件URL
    /// </summary>
    /// <remarks>
    /// 下载文件的URL地址
    /// </remarks>
    public string FileUrl { get; set; } = string.Empty;

    /// <summary>
    /// 文件名称
    /// </summary>
    /// <remarks>
    /// 下载文件的原文件名
    /// </remarks>
    public string? FileName { get; set; }

    /// <summary>
    /// 文件大小（字节）
    /// </summary>
    /// <remarks>
    /// 下载文件的大小，以字节为单位
    /// </remarks>
    public long FileSize { get; set; }

    /// <summary>
    /// 文件类型（扩展名）
    /// </summary>
    /// <remarks>
    /// 下载文件的类型/扩展名，如：pdf、doc、zip等
    /// </remarks>
    public string? FileType { get; set; }

    /// <summary>
    /// 下载次数
    /// </summary>
    /// <remarks>
    /// 记录文件被下载的次数
    /// </remarks>
    public int DownloadCount { get; set; }

    /// <summary>
    /// 分类
    /// </summary>
    /// <remarks>
    /// 下载文件的分类标识
    /// </remarks>
    public string? Category { get; set; }

    /// <summary>
    /// 排序号
    /// </summary>
    /// <remarks>
    /// 用于控制下载在列表中的显示顺序
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