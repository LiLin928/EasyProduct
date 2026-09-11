using SqlSugar;
using EasyProduct.Models.Entitys.Base;

namespace EasyProduct.Models.Entitys.Site;

/// <summary>
/// 视频管理实体
/// </summary>
/// <remarks>
/// 对应数据库表 site_video
/// 继承 BaseEntity 基类，自动包含 Id、IsDeleted、Status、CreatedAt、UpdatedAt、CreatedBy、UpdatedBy 字段
/// 用于管理网站视频列表
/// 支持中英文双语，记录播放次数
/// </remarks>
[SugarTable("site_video", "视频管理表")]
public class site_video : BaseEntity
{
    /// <summary>
    /// 视频标题
    /// </summary>
    /// <remarks>
    /// 视频的标题，必填字段
    /// 最大长度200字符
    /// </remarks>
    [SugarColumn(Length = 200, ColumnDescription = "视频标题")]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 视频标题（英文）
    /// </summary>
    /// <remarks>
    /// 视频的英文标题
    /// 可选字段，最大长度200字符
    /// </remarks>
    [SugarColumn(Length = 200, IsNullable = true, ColumnDescription = "视频标题（英文）")]
    public string? TitleEn { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    /// <remarks>
    /// 视频的详细描述
    /// 可选字段，最大长度500字符
    /// </remarks>
    [SugarColumn(Length = 500, IsNullable = true, ColumnDescription = "描述")]
    public string? Description { get; set; }

    /// <summary>
    /// 描述（英文）
    /// </summary>
    /// <remarks>
    /// 视频的英文详细描述
    /// 可选字段，最大长度500字符
    /// </remarks>
    [SugarColumn(Length = 500, IsNullable = true, ColumnDescription = "描述（英文）")]
    public string? DescriptionEn { get; set; }

    /// <summary>
    /// 封面图片URL
    /// </summary>
    /// <remarks>
    /// 视频的封面图片URL地址
    /// 可选字段，最大长度500字符
    /// </remarks>
    [SugarColumn(Length = 500, IsNullable = true, ColumnDescription = "封面图片URL")]
    public string? CoverImage { get; set; }

    /// <summary>
    /// 视频URL
    /// </summary>
    /// <remarks>
    /// 视频文件的URL地址，必填字段
    /// 最大长度500字符
    /// </remarks>
    [SugarColumn(Length = 500, ColumnDescription = "视频URL")]
    public string VideoUrl { get; set; } = string.Empty;

    /// <summary>
    /// 视频类型
    /// </summary>
    /// <remarks>
    /// 视频文件的类型
    /// 可选字段，最大长度20字符
    /// 枚举值：mp4（MP4文件）、webm（WebM文件）、external（外链视频）
    /// 默认为 mp4
    /// </remarks>
    [SugarColumn(Length = 20, IsNullable = true, ColumnDescription = "视频类型")]
    public string? VideoType { get; set; } = "mp4";

    /// <summary>
    /// 视频时长（秒）
    /// </summary>
    /// <remarks>
    /// 视频的总时长，以秒为单位
    /// 可选字段，默认值为0
    /// </remarks>
    [SugarColumn(ColumnDescription = "视频时长（秒）")]
    public int Duration { get; set; } = 0;

    /// <summary>
    /// 播放次数
    /// </summary>
    /// <remarks>
    /// 记录视频被播放的次数
    /// 默认值为0，官网公开接口会自动增加此字段
    /// </remarks>
    [SugarColumn(ColumnDescription = "播放次数")]
    public int PlayCount { get; set; } = 0;

    /// <summary>
    /// 分类
    /// </summary>
    /// <remarks>
    /// 视频的分类标识
    /// 可选字段，最大长度50字符
    /// </remarks>
    [SugarColumn(Length = 50, IsNullable = true, ColumnDescription = "分类")]
    public string? Category { get; set; }

    /// <summary>
    /// 排序号
    /// </summary>
    /// <remarks>
    /// 用于控制视频在列表中的显示顺序
    /// 数值越小越靠前，默认值为0
    /// </remarks>
    [SugarColumn(ColumnDescription = "排序号")]
    public int Sort { get; set; } = 0;
}