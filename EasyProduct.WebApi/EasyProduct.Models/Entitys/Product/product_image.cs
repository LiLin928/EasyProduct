using SqlSugar;
using EasyProduct.Models.Entitys.Base;

namespace EasyProduct.Models.Entitys.Product;

/// <summary>
/// 商品图集实体
/// </summary>
/// <remarks>
/// 对应数据库表 product_image
/// 继承 BaseEntity 基类，自动包含 Id、IsDeleted、CreatedAt、UpdatedAt、CreatedBy 字段
/// 用于存储商品图片信息，支持多图片管理、主图设置、图片排序等功能
/// </remarks>
[SugarTable("product_image", "商品图集表")]
public class product_image : BaseEntity
{
    /// <summary>
    /// 商品ID
    /// </summary>
    /// <remarks>
    /// 关联商品主档表（product_spu）的主键ID
    /// </remarks>
    [SugarColumn(ColumnDataType = "char(36)", ColumnDescription = "商品ID")]
    public string SpuId { get; set; } = string.Empty;

    /// <summary>
    /// 图片URL
    /// </summary>
    /// <remarks>
    /// 图片的完整访问地址，支持相对路径和绝对路径
    /// </remarks>
    [SugarColumn(Length = 500, ColumnDescription = "图片URL")]
    public string ImageUrl { get; set; } = string.Empty;

    /// <summary>
    /// 缩略图URL
    /// </summary>
    /// <remarks>
    /// 图片的缩略版本，用于列表展示等场景
    /// </remarks>
    [SugarColumn(Length = 500, IsNullable = true, ColumnDescription = "缩略图URL")]
    public string? ThumbnailUrl { get; set; }

    /// <summary>
    /// 图片名称
    /// </summary>
    /// <remarks>
    /// 图片的原始文件名或自定义名称
    /// </remarks>
    [SugarColumn(Length = 200, IsNullable = true, ColumnDescription = "图片名称")]
    public string? ImageName { get; set; }

    /// <summary>
    /// 图片大小（字节）
    /// </summary>
    /// <remarks>
    /// 图片文件的大小，单位为字节
    /// </remarks>
    [SugarColumn(IsNullable = true, ColumnDescription = "图片大小")]
    public int? ImageSize { get; set; }

    /// <summary>
    /// 图片类型（jpg/png/webp等）
    /// </summary>
    /// <remarks>
    /// 图片的格式类型，如 jpg、png、webp 等
    /// </remarks>
    [SugarColumn(Length = 50, IsNullable = true, ColumnDescription = "图片类型")]
    public string? ImageType { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    /// <remarks>
    /// 图片在同商品下的显示顺序，数值越小越靠前
    /// </remarks>
    [SugarColumn(ColumnDescription = "排序")]
    public int Sort { get; set; } = 0;

    /// <summary>
    /// 是否主图
    /// </summary>
    /// <remarks>
    /// 标识该图片是否为商品的主图
    /// true = 是主图，false = 不是主图
    /// 同一商品只能有一张主图
    /// </remarks>
    [SugarColumn(ColumnDescription = "是否主图")]
    public bool IsMain { get; set; } = false;
}