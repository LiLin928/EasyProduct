using SqlSugar;
using EasyProduct.Models.Entitys.Base;

namespace EasyProduct.Models.Entitys.Product;

/// <summary>
/// 商品分类实体
/// </summary>
/// <remarks>
/// 对应数据库表 product_category
/// 继承 BaseEntity 基类，自动包含 Id、IsDeleted、Status、CreatedAt、UpdatedAt、CreatedBy、UpdatedBy 字段
/// 用于存储商品分类信息，支持树形结构
/// </remarks>
[SugarTable("product_category", "商品分类表")]
public class product_category : BaseEntity
{
    /// <summary>
    /// 父分类ID
    /// </summary>
    /// <remarks>
    /// 上级分类ID，根分类为空字符串或"0"
    /// </remarks>
    [SugarColumn(ColumnDataType = "varchar(36)", IsNullable = true, ColumnDescription = "父分类ID")]
    public string? ParentId { get; set; }

    /// <summary>
    /// 分类名称
    /// </summary>
    [SugarColumn(Length = 100, ColumnDescription = "分类名称")]
    public string CategoryName { get; set; } = string.Empty;

    /// <summary>
    /// 分类编码
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = true, ColumnDescription = "分类编码")]
    public string? CategoryCode { get; set; }

    /// <summary>
    /// 分类图标
    /// </summary>
    [SugarColumn(Length = 255, IsNullable = true, ColumnDescription = "分类图标")]
    public string? Icon { get; set; }

    /// <summary>
    /// 分类图片
    /// </summary>
    [SugarColumn(Length = 500, IsNullable = true, ColumnDescription = "分类图片")]
    public string? Image { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    [SugarColumn(ColumnDescription = "排序")]
    public int Sort { get; set; } = 0;

    /// <summary>
    /// 层级
    /// </summary>
    [SugarColumn(ColumnDescription = "层级")]
    public int Level { get; set; } = 1;

    /// <summary>
    /// 完整路径
    /// </summary>
    /// <remarks>
    /// 如：电子产品/手机/智能手机，用于快速定位和显示
    /// </remarks>
    [SugarColumn(Length = 500, IsNullable = true, ColumnDescription = "完整路径")]
    public string? FullPath { get; set; }

    /// <summary>
    /// 是否显示在导航
    /// </summary>
    /// <remarks>
    /// 0=否，1=是
    /// </remarks>
    [SugarColumn(ColumnDescription = "是否显示在导航")]
    public int ShowInNav { get; set; } = 1;
}