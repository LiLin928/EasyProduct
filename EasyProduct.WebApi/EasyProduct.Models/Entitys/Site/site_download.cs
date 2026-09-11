using SqlSugar;
using EasyProduct.Models.Entitys.Base;

namespace EasyProduct.Models.Entitys.Site;

/// <summary>
/// 下载管理实体
/// </summary>
/// <remarks>
/// 对应数据库表 site_download
/// 继承 BaseEntity 基类，自动包含 Id、IsDeleted、Status、CreatedAt、UpdatedAt、CreatedBy、UpdatedBy 字段
/// 用于管理网站下载文件列表
/// 支持中英文双语，记录下载次数
/// </remarks>
[SugarTable("site_download", "下载管理表")]
public class site_download : BaseEntity
{
    /// <summary>
    /// 下载标题
    /// </summary>
    /// <remarks>
    /// 下载文件的标题，必填字段
    /// 最大长度200字符
    /// </remarks>
    [SugarColumn(Length = 200, ColumnDescription = "下载标题")]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 下载标题（英文）
    /// </summary>
    /// <remarks>
    /// 下载文件的英文标题
    /// 可选字段，最大长度200字符
    /// </remarks>
    [SugarColumn(Length = 200, IsNullable = true, ColumnDescription = "下载标题（英文）")]
    public string? TitleEn { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    /// <remarks>
    /// 下载文件的详细描述
    /// 可选字段，最大长度500字符
    /// </remarks>
    [SugarColumn(Length = 500, IsNullable = true, ColumnDescription = "描述")]
    public string? Description { get; set; }

    /// <summary>
    /// 描述（英文）
    /// </summary>
    /// <remarks>
    /// 下载文件的英文详细描述
    /// 可选字段，最大长度500字符
    /// </remarks>
    [SugarColumn(Length = 500, IsNullable = true, ColumnDescription = "描述（英文）")]
    public string? DescriptionEn { get; set; }

    /// <summary>
    /// 文件URL
    /// </summary>
    /// <remarks>
    /// 下载文件的URL地址，必填字段
    /// 最大长度500字符
    /// </remarks>
    [SugarColumn(Length = 500, ColumnDescription = "文件URL")]
    public string FileUrl { get; set; } = string.Empty;

    /// <summary>
    /// 文件名称
    /// </summary>
    /// <remarks>
    /// 下载文件的原文件名
    /// 可选字段，最大长度200字符
    /// </remarks>
    [SugarColumn(Length = 200, IsNullable = true, ColumnDescription = "文件名称")]
    public string? FileName { get; set; }

    /// <summary>
    /// 文件大小（字节）
    /// </summary>
    /// <remarks>
    /// 下载文件的大小，以字节为单位
    /// 可选字段，默认值为0
    /// </remarks>
    [SugarColumn(ColumnDescription = "文件大小（字节）")]
    public long FileSize { get; set; } = 0;

    /// <summary>
    /// 文件类型（扩展名）
    /// </summary>
    /// <remarks>
    /// 下载文件的类型/扩展名，如：pdf、doc、zip等
    /// 可选字段，最大长度50字符
    /// </remarks>
    [SugarColumn(Length = 50, IsNullable = true, ColumnDescription = "文件类型（扩展名）")]
    public string? FileType { get; set; }

    /// <summary>
    /// 下载次数
    /// </summary>
    /// <remarks>
    /// 记录文件被下载的次数
    /// 默认值为0，官网公开接口会自动增加此字段
    /// </remarks>
    [SugarColumn(ColumnDescription = "下载次数")]
    public int DownloadCount { get; set; } = 0;

    /// <summary>
    /// 分类
    /// </summary>
    /// <remarks>
    /// 下载文件的分类标识
    /// 可选字段，最大长度50字符
    /// </remarks>
    [SugarColumn(Length = 50, IsNullable = true, ColumnDescription = "分类")]
    public string? Category { get; set; }

    /// <summary>
    /// 排序号
    /// </summary>
    /// <remarks>
    /// 用于控制下载在列表中的显示顺序
    /// 数值越小越靠前，默认值为0
    /// </remarks>
    [SugarColumn(ColumnDescription = "排序号")]
    public int Sort { get; set; } = 0;
}