namespace EasyProduct.Models.Dto.Mall.Cart;

/// <summary>
/// 修改购物车数量DTO
/// </summary>
/// <remarks>
/// 用于修改购物车中商品数量的请求参数
/// </remarks>
public class CartUpdateQuantityDto
{
    /// <summary>
    /// 新的数量
    /// </summary>
    /// <remarks>
    /// 必须大于0，且不能超过库存上限
    /// </remarks>
    public int Quantity { get; set; }
}