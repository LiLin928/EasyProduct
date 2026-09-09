namespace EasyProduct.Models.Dto.Mall.Cart;

/// <summary>
/// 购物车列表DTO（含统计信息）
/// </summary>
/// <remarks>
/// 用于返回会员购物车完整信息，包含购物车项列表和统计信息（总数量、选中数量、总金额、失效商品数量）
/// </remarks>
public class CartListDto
{
    /// <summary>
    /// 购物车项列表
    /// </summary>
    public List<CartItemDto> Items { get; set; } = new();

    /// <summary>
    /// 总数量
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// 选中数量
    /// </summary>
    public int SelectedCount { get; set; }

    /// <summary>
    /// 总金额（仅选中商品）
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// 失效商品数量
    /// </summary>
    public int InvalidCount { get; set; }
}