namespace EasyProduct.Models.Dto.Mall.Cart;

/// <summary>
/// 购物车项DTO（含商品详情）
/// </summary>
/// <remarks>
/// 用于返回购物车中的单个商品项，包含商品信息、价格、库存、失效状态等完整信息
/// </remarks>
public class CartItemDto
{
    /// <summary>
    /// 购物车ID
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// SKU ID
    /// </summary>
    public string SkuId { get; set; } = string.Empty;

    /// <summary>
    /// SPU ID
    /// </summary>
    public string SpuId { get; set; } = string.Empty;

    /// <summary>
    /// 商品名称
    /// </summary>
    public string ProductName { get; set; } = string.Empty;

    /// <summary>
    /// SKU编码
    /// </summary>
    public string SkuCode { get; set; } = string.Empty;

    /// <summary>
    /// 规格组合（如："红色 / XL"）
    /// </summary>
    public string SpecText { get; set; } = string.Empty;

    /// <summary>
    /// 主图URL
    /// </summary>
    public string MainImage { get; set; } = string.Empty;

    /// <summary>
    /// 零售价
    /// </summary>
    public decimal RetailPrice { get; set; }

    /// <summary>
    /// 会员价（如有）
    /// </summary>
    public decimal? MemberPrice { get; set; }

    /// <summary>
    /// 实际售价（会员价优先）
    /// </summary>
    public decimal ActualPrice { get; set; }

    /// <summary>
    /// 数量
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// 小计金额
    /// </summary>
    public decimal Subtotal { get; set; }

    /// <summary>
    /// 库存数量
    /// </summary>
    public int Stock { get; set; }

    /// <summary>
    /// 是否有库存
    /// </summary>
    public bool InStock { get; set; }

    /// <summary>
    /// SKU状态：0=禁用，1=启用
    /// </summary>
    public int SkuStatus { get; set; }

    /// <summary>
    /// 是否失效（SKU禁用或库存不足）
    /// </summary>
    public bool IsInvalid { get; set; }

    /// <summary>
    /// 失效原因
    /// </summary>
    public string? InvalidReason { get; set; }

    /// <summary>
    /// 是否选中
    /// </summary>
    public int Selected { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreateTime { get; set; }
}