namespace EasyProduct.Models.Dto.Mall.Cart;

/// <summary>
/// 热门商品DTO
/// </summary>
/// <remarks>
/// 用于购物车统计中展示热门商品信息
/// </remarks>
public class HotProductDto
{
    /// <summary>
    /// 商品ID
    /// </summary>
    public string ProductId { get; set; } = string.Empty;

    /// <summary>
    /// 商品名称
    /// </summary>
    public string ProductName { get; set; } = string.Empty;

    /// <summary>
    /// 出现在购物车中的次数
    /// </summary>
    public int CartCount { get; set; }
}