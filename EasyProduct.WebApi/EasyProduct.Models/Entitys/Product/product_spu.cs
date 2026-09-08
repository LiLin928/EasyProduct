using SqlSugar;
using EasyProduct.Models.Entitys.Base;
using EasyProduct.Models.Enums.Product;

namespace EasyProduct.Models.Entitys.Product;

/// <summary>
/// 商品主档实体（SPU）
/// </summary>
/// <remarks>
/// 对应数据库表 product_spu
/// 继承 BaseEntity 基类，自动包含 Id、IsDeleted、Status、CreatedAt、UpdatedAt、CreatedBy、UpdatedBy 字段
/// 用于存储商品主档信息（Standard Product Unit，标准化产品单元）
/// </remarks>
[SugarTable("product_spu", "商品主档表")]
public class product_spu : BaseEntity
{
    /// <summary>
    /// 商品名称
    /// </summary>
    [SugarColumn(Length = 200, ColumnDescription = "商品名称")]
    public string SpuName { get; set; } = string.Empty;

    /// <summary>
    /// 商品编码
    /// </summary>
    /// <remarks>
    /// 商品的唯一编码，用于业务系统内部标识
    /// </remarks>
    [SugarColumn(Length = 50, IsNullable = true, ColumnDescription = "商品编码")]
    public string? SpuCode { get; set; }

    /// <summary>
    /// 分类ID
    /// </summary>
    /// <remarks>
    /// 关联 product_category 表的 Id 字段
    /// </remarks>
    [SugarColumn(ColumnDataType = "varchar(36)", IsNullable = true, ColumnDescription = "分类ID")]
    public string? CategoryId { get; set; }

    /// <summary>
    /// 主图
    /// </summary>
    /// <remarks>
    /// 商品展示的主图片URL
    /// </remarks>
    [SugarColumn(Length = 500, IsNullable = true, ColumnDescription = "主图")]
    public string? MainImage { get; set; }

    /// <summary>
    /// 商品图片列表
    /// </summary>
    /// <remarks>
    /// JSON 数组格式存储多张图片URL
    /// 示例：["url1", "url2", "url3"]
    /// </remarks>
    [SugarColumn(ColumnDataType = "text", IsNullable = true, ColumnDescription = "商品图片列表")]
    public string? Images { get; set; }

    /// <summary>
    /// 商品描述
    /// </summary>
    /// <remarks>
    /// 商品的详细描述，支持富文本
    /// </remarks>
    [SugarColumn(ColumnDataType = "text", IsNullable = true, ColumnDescription = "商品描述")]
    public string? Description { get; set; }

    /// <summary>
    /// 计量单位
    /// </summary>
    /// <remarks>
    /// 商品的计量单位，如：件、个、台、套等
    /// </remarks>
    [SugarColumn(Length = 20, IsNullable = true, ColumnDescription = "计量单位")]
    public string? Unit { get; set; }

    /// <summary>
    /// 商品类型
    /// </summary>
    /// <remarks>
    /// 枚举值：1=实物商品，2=虚拟商品，3=票品
    /// </remarks>
    [SugarColumn(ColumnDescription = "商品类型")]
    public SpuType SpuType { get; set; } = SpuType.Physical;

    /// <summary>
    /// 品牌
    /// </summary>
    [SugarColumn(Length = 100, IsNullable = true, ColumnDescription = "品牌")]
    public string? Brand { get; set; }

    /// <summary>
    /// 规格模板
    /// </summary>
    /// <remarks>
    /// JSON 格式存储规格模板配置
    /// 示例：[{"name":"颜色","values":["红色","蓝色"]},{"name":"尺寸","values":["S","M","L"]}]
    /// </remarks>
    [SugarColumn(ColumnDataType = "text", IsNullable = true, ColumnDescription = "规格模板")]
    public string? SpecTemplate { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    /// <remarks>
    /// 数值越小越靠前，用于商品列表排序
    /// </remarks>
    [SugarColumn(ColumnDescription = "排序")]
    public int Sort { get; set; } = 0;
}