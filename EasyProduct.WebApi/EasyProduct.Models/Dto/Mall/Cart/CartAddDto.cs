namespace EasyProduct.Models.Dto.Mall.Cart;

/// <summary>
/// 添加到购物车DTO
/// </summary>
/// <remarks>
/// 用于添加商品到购物车的请求参数
/// </remarks>
public class CartAddDto
{
    /// <summary>
    /// SKU ID
    /// </summary>
    public string SkuId { get; set; } = string.Empty;

    /// <summary>
    /// 数量（默认为1）
    /// </summary>
    public int Quantity { get; set; } = 1;

    /// <summary>
    /// 是否选中（默认为1-已选中）
    /// </summary>
    public int Selected { get; set; } = 1;
}