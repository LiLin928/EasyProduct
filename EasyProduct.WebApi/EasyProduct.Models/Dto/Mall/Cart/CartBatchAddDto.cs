namespace EasyProduct.Models.Dto.Mall.Cart;

/// <summary>
/// 批量添加到购物车DTO
/// </summary>
/// <remarks>
/// 用于批量添加多个商品到购物车的请求参数
/// </remarks>
public class CartBatchAddDto
{
    /// <summary>
    /// 商品列表
    /// </summary>
    public List<CartAddDto> Items { get; set; } = new();
}