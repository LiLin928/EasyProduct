using EasyProduct.Models.Enums;

namespace EasyProduct.Models.Dto.Product.Sku;

/// <summary>
/// 商品SKU DTO
/// </summary>
/// <remarks>
/// 用于返回商品SKU（库存量单位）信息，包含所有SKU字段
/// 包含 SPU 名称用于显示
/// </remarks>
public class SkuDto
{
    /// <summary>
    /// SKU ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 商品SPU ID
    /// </summary>
    /// <remarks>
    /// 关联 product_spu 表的 Id 字段
    /// </remarks>
    public string SpuId { get; set; } = string.Empty;

    /// <summary>
    /// 商品SPU名称
    /// </summary>
    /// <remarks>
    /// 用于显示的 SPU 名称，方便前端展示
    /// </remarks>
    public string? SpuName { get; set; }

    /// <summary>
    /// SKU名称
    /// </summary>
    /// <remarks>
    /// SKU的名称，通常由商品名称+规格组合而成
    /// 示例：iPhone 15 Pro 256GB 深空黑色
    /// </remarks>
    public string SkuName { get; set; } = string.Empty;

    /// <summary>
    /// SKU编码
    /// </summary>
    /// <remarks>
    /// SKU的唯一编码，用于业务系统内部标识
    /// </remarks>
    public string? SkuCode { get; set; }

    /// <summary>
    /// 条码
    /// </summary>
    /// <remarks>
    /// 商品条形码，如 EAN-13、UPC 等
    /// </remarks>
    public string? Barcode { get; set; }

    /// <summary>
    /// 规格组合JSON
    /// </summary>
    /// <remarks>
    /// JSON格式存储规格组合
    /// 示例：[{"name":"颜色","value":"深空黑色"},{"name":"存储容量","value":"256GB"}]
    /// </remarks>
    public string? SpecJson { get; set; }

    /// <summary>
    /// 零售价
    /// </summary>
    /// <remarks>
    /// 商品的销售价格，精确到分（保留2位小数）
    /// </remarks>
    public decimal Price { get; set; }

    /// <summary>
    /// 会员价
    /// </summary>
    /// <remarks>
    /// 会员专享价格，可为空表示不设置会员价
    /// 精确到分（保留2位小数）
    /// </remarks>
    public decimal? MemberPrice { get; set; }

    /// <summary>
    /// 批发价
    /// </summary>
    /// <remarks>
    /// 批发价格，可为空表示不设置批发价
    /// 精确到分（保留2位小数）
    /// </remarks>
    public decimal? WholesalePrice { get; set; }

    /// <summary>
    /// 成本价
    /// </summary>
    /// <remarks>
    /// 商品的进货成本，可为空表示不记录成本
    /// 精确到分（保留2位小数）
    /// </remarks>
    public decimal? CostPrice { get; set; }

    /// <summary>
    /// 库存数量
    /// </summary>
    /// <remarks>
    /// 当前SKU的实际库存数量
    /// </remarks>
    public int Stock { get; set; }

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

    /// <summary>
    /// 更新人ID
    /// </summary>
    public string? UpdatedBy { get; set; }
}