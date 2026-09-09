namespace EasyProduct.Models.Dto.Mall.Cart;

/// <summary>
/// 批量修改购物车选中状态DTO
/// </summary>
/// <remarks>
/// 用于批量修改购物车中多个商品选中状态的请求参数
/// </remarks>
public class CartUpdateSelectedDto
{
    /// <summary>
    /// 购物车ID列表
    /// </summary>
    /// <remarks>
    /// 需要修改选中状态的购物车项ID集合
    /// </remarks>
    public List<string> Ids { get; set; } = new();

    /// <summary>
    /// 选中状态
    /// </summary>
    /// <remarks>
    /// 0=未选中，1=已选中
    /// </remarks>
    public int Selected { get; set; }
}