namespace EasyProduct.Models.Dto.Mall.Order;

/// <summary>
/// 订单项 DTO
/// </summary>
/// <remarks>
/// 用于展示订单中的商品项信息，包含商品基本信息、SKU信息、价格和数量
/// </remarks>
public class OrderItemDto
{
    /// <summary>
    /// 订单项ID
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// SKU ID
    /// </summary>
    public string SkuId { get; set; } = string.Empty;

    /// <summary>
    /// 商品名称
    /// </summary>
    public string ProductName { get; set; } = string.Empty;

    /// <summary>
    /// SKU名称
    /// </summary>
    public string SkuName { get; set; } = string.Empty;

    /// <summary>
    /// SKU规格
    /// </summary>
    /// <remarks>
    /// SKU规格信息，如颜色、尺码等
    /// </remarks>
    public string? SkuSpec { get; set; }

    /// <summary>
    /// 商品图片
    /// </summary>
    public string ProductImage { get; set; } = string.Empty;

    /// <summary>
    /// 单价
    /// </summary>
    /// <remarks>
    /// 下单时的商品单价
    /// </remarks>
    public decimal Price { get; set; }

    /// <summary>
    /// 数量
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// 小计
    /// </summary>
    /// <remarks>
    /// 单价 * 数量
    /// </remarks>
    public decimal Subtotal { get; set; }

    /// <summary>
    /// 是否已退款
    /// </summary>
    public bool IsRefunded { get; set; }

    /// <summary>
    /// 已退款数量
    /// </summary>
    /// <remarks>
    /// 已退款的数量，可能部分退款
    /// </remarks>
    public int RefundQuantity { get; set; }
}