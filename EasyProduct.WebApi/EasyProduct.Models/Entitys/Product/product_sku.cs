using SqlSugar;
using EasyProduct.Models.Entitys.Base;

namespace EasyProduct.Models.Entitys.Product;

/// <summary>
/// 商品SKU实体
/// </summary>
/// <remarks>
/// 对应数据库表 product_sku
/// 继承 BaseEntity 基类，自动包含 Id、IsDeleted、Status、CreatedAt、UpdatedAt、CreatedBy、UpdatedBy 字段
/// 用于存储商品SKU信息（Stock Keeping Unit，库存量单位）
/// </remarks>
[SugarTable("product_sku", "商品SKU表")]
public class product_sku : BaseEntity
{
    /// <summary>
    /// 商品SPU ID
    /// </summary>
    /// <remarks>
    /// 关联 product_spu 表的 Id 字段
    /// </remarks>
    [SugarColumn(ColumnDataType = "varchar(36)", ColumnDescription = "商品SPU ID")]
    public string SpuId { get; set; } = string.Empty;

    /// <summary>
    /// SKU名称
    /// </summary>
    /// <remarks>
    /// SKU的名称，通常由商品名称+规格组合而成
    /// 示例：iPhone 15 Pro 256GB 深空黑色
    /// </remarks>
    [SugarColumn(Length = 200, ColumnDescription = "SKU名称")]
    public string SkuName { get; set; } = string.Empty;

    /// <summary>
    /// SKU编码
    /// </summary>
    /// <remarks>
    /// SKU的唯一编码，用于业务系统内部标识
    /// </remarks>
    [SugarColumn(Length = 50, IsNullable = true, ColumnDescription = "SKU编码")]
    public string? SkuCode { get; set; }

    /// <summary>
    /// 条码
    /// </summary>
    /// <remarks>
    /// 商品条形码，如 EAN-13、UPC 等
    /// </remarks>
    [SugarColumn(Length = 50, IsNullable = true, ColumnDescription = "条码")]
    public string? Barcode { get; set; }

    /// <summary>
    /// 规格组合JSON
    /// </summary>
    /// <remarks>
    /// JSON格式存储规格组合
    /// 示例：[{"name":"颜色","value":"深空黑色"},{"name":"存储容量","value":"256GB"}]
    /// </remarks>
    [SugarColumn(ColumnDataType = "text", IsNullable = true, ColumnDescription = "规格组合")]
    public string? SpecJson { get; set; }

    /// <summary>
    /// 零售价
    /// </summary>
    /// <remarks>
    /// 商品的销售价格，精确到分（保留2位小数）
    /// </remarks>
    [SugarColumn(Length = 18, DecimalDigits = 2, ColumnDescription = "零售价")]
    public decimal Price { get; set; }

    /// <summary>
    /// 会员价
    /// </summary>
    /// <remarks>
    /// 会员专享价格，可为空表示不设置会员价
    /// 精确到分（保留2位小数）
    /// </remarks>
    [SugarColumn(Length = 18, DecimalDigits = 2, IsNullable = true, ColumnDescription = "会员价")]
    public decimal? MemberPrice { get; set; }

    /// <summary>
    /// 批发价
    /// </summary>
    /// <remarks>
    /// 批发价格，可为空表示不设置批发价
    /// 精确到分（保留2位小数）
    /// </remarks>
    [SugarColumn(Length = 18, DecimalDigits = 2, IsNullable = true, ColumnDescription = "批发价")]
    public decimal? WholesalePrice { get; set; }

    /// <summary>
    /// 成本价
    /// </summary>
    /// <remarks>
    /// 商品的进货成本，可为空表示不记录成本
    /// 精确到分（保留2位小数）
    /// </remarks>
    [SugarColumn(Length = 18, DecimalDigits = 2, IsNullable = true, ColumnDescription = "成本价")]
    public decimal? CostPrice { get; set; }

    /// <summary>
    /// 库存数量
    /// </summary>
    /// <remarks>
    /// 当前SKU的实际库存数量
    /// 默认为0，表示无库存
    /// </remarks>
    [SugarColumn(ColumnDescription = "库存数量")]
    public int Stock { get; set; } = 0;
}