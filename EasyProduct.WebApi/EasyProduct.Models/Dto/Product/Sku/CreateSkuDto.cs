using System.ComponentModel.DataAnnotations;

namespace EasyProduct.Models.Dto.Product.Sku;

/// <summary>
/// 创建商品SKU请求 DTO
/// </summary>
/// <remarks>
/// 用于创建新的商品SKU（库存量单位）
/// 必填字段：SpuId、SkuName、Price
/// 可选字段：SkuCode、Barcode、SpecJson、MemberPrice、WholesalePrice、CostPrice、Stock
/// </remarks>
public class CreateSkuDto
{
    /// <summary>
    /// 商品SPU ID
    /// </summary>
    /// <remarks>
    /// 必填，关联 product_spu 表的 Id 字段
    /// GUID 字符串格式，最大长度 36 个字符
    /// </remarks>
    [Required(ErrorMessage = "SPU ID不能为空")]
    [StringLength(36, ErrorMessage = "SPU ID长度不能超过36个字符")]
    public string SpuId { get; set; } = string.Empty;

    /// <summary>
    /// SKU名称
    /// </summary>
    /// <remarks>
    /// 必填，SKU的名称，通常由商品名称+规格组合而成
    /// 最大长度 200 个字符
    /// 示例：iPhone 15 Pro 256GB 深空黑色
    /// </remarks>
    [Required(ErrorMessage = "SKU名称不能为空")]
    [StringLength(200, MinimumLength = 1, ErrorMessage = "SKU名称长度必须在1-200个字符之间")]
    public string SkuName { get; set; } = string.Empty;

    /// <summary>
    /// SKU编码
    /// </summary>
    /// <remarks>
    /// SKU的唯一编码，用于业务系统内部标识
    /// 最大长度 50 个字符
    /// </remarks>
    [StringLength(50, ErrorMessage = "SKU编码长度不能超过50个字符")]
    public string? SkuCode { get; set; }

    /// <summary>
    /// 条码
    /// </summary>
    /// <remarks>
    /// 商品条形码，如 EAN-13、UPC 等
    /// 最大长度 50 个字符
    /// </remarks>
    [StringLength(50, ErrorMessage = "条码长度不能超过50个字符")]
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
    /// 必填，商品的销售价格，精确到分（保留2位小数）
    /// 必须大于或等于 0
    /// </remarks>
    [Required(ErrorMessage = "零售价不能为空")]
    [Range(0, double.MaxValue, ErrorMessage = "零售价必须大于或等于0")]
    public decimal Price { get; set; }

    /// <summary>
    /// 会员价
    /// </summary>
    /// <remarks>
    /// 会员专享价格，可为空表示不设置会员价
    /// 精确到分（保留2位小数）
    /// 必须大于或等于 0（如果设置）
    /// </remarks>
    [Range(0, double.MaxValue, ErrorMessage = "会员价必须大于或等于0")]
    public decimal? MemberPrice { get; set; }

    /// <summary>
    /// 批发价
    /// </summary>
    /// <remarks>
    /// 批发价格，可为空表示不设置批发价
    /// 精确到分（保留2位小数）
    /// 必须大于或等于 0（如果设置）
    /// </remarks>
    [Range(0, double.MaxValue, ErrorMessage = "批发价必须大于或等于0")]
    public decimal? WholesalePrice { get; set; }

    /// <summary>
    /// 成本价
    /// </summary>
    /// <remarks>
    /// 商品的进货成本，可为空表示不记录成本
    /// 精确到分（保留2位小数）
    /// 必须大于或等于 0（如果设置）
    /// </remarks>
    [Range(0, double.MaxValue, ErrorMessage = "成本价必须大于或等于0")]
    public decimal? CostPrice { get; set; }

    /// <summary>
    /// 库存数量
    /// </summary>
    /// <remarks>
    /// 当前SKU的实际库存数量
    /// 默认为 0，必须大于或等于 0
    /// </remarks>
    [Range(0, int.MaxValue, ErrorMessage = "库存数量必须大于或等于0")]
    public int Stock { get; set; } = 0;
}