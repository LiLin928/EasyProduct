namespace EasyProduct.Models.Dto.Mall.Cart;

/// <summary>
/// 切换购物车SKU规格DTO
/// </summary>
/// <remarks>
/// 用于在购物车中切换商品规格的请求参数
/// </remarks>
public class CartChangeSkuDto
{
    /// <summary>
    /// 购物车ID
    /// </summary>
    /// <remarks>
    /// 需要切换规格的购物车项ID
    /// </remarks>
    public string CartId { get; set; } = string.Empty;

    /// <summary>
    /// 新的SKU ID
    /// </summary>
    /// <remarks>
    /// 目标SKU的ID，必须属于同一个SPU
    /// </remarks>
    public string NewSkuId { get; set; } = string.Empty;
}