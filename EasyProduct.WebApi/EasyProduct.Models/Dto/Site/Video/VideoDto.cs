using EasyProduct.Models.Enums;

namespace EasyProduct.Models.Dto.Site.Video;

/// <summary>
/// 视频管理 DTO
/// </summary>
/// <remarks>
/// 用于返回视频信息，包含所有字段
/// 对应实体类 site_video
/// </remarks>
public class VideoDto
{
    /// <summary>
    /// 视频ID
    /// </summary>
    /// <remarks>
    /// GUID 主键，唯一标识
    /// </remarks>
    public Guid Id { get; set; }

    /// <summary>
    /// 视频标题
    /// </summary>
    /// <remarks>
    /// 视频的标题
    /// </remarks>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 视频标题（英文）
    /// </summary>
    /// <remarks>
    /// 视频的英文标题
    /// </remarks>
    public string? TitleEn { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    /// <remarks>
    /// 视频的详细描述
    /// </remarks>
    public string? Description { get; set; }

    /// <summary>
    /// 描述（英文）
    /// </summary>
    /// <remarks>
    /// 视频的英文详细描述
    /// </remarks>
    public string? DescriptionEn { get; set; }

    /// <summary>
    /// 封面图片URL
    /// </summary>
    /// <remarks>
    /// 视频的封面图片URL地址
    /// </remarks>
    public string? CoverImage { get; set; }

    /// <summary>
    /// 视频URL
    /// </summary>
    /// <remarks>
    /// 视频文件的URL地址
    /// </remarks>
    public string VideoUrl { get; set; } = string.Empty;

    /// <summary>
    /// 视频类型
    /// </summary>
    /// <remarks>
    /// 视频文件的类型
    /// 枚举值：mp4、webm、external
    /// </remarks>
    public string? VideoType { get; set; }

    /// <summary>
    /// 视频时长（秒）
    /// </summary>
    /// <remarks>
    /// 视频的总时长，以秒为单位
    /// </remarks>
    public int Duration { get; set; }

    /// <summary>
    /// 播放次数
    /// </summary>
    /// <remarks>
    /// 记录视频被播放的次数
    /// </remarks>
    public int PlayCount { get; set; }

    /// <summary>
    /// 分类
    /// </summary>
    /// <remarks>
    /// 视频的分类标识
    /// </remarks>
    public string? Category { get; set; }

    /// <summary>
    /// 排序号
    /// </summary>
    /// <remarks>
    /// 用于控制视频在列表中的显示顺序
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