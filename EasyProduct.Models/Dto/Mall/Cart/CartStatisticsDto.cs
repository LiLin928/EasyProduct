namespace EasyProduct.Models.Dto.Mall.Cart;

/// <summary>
/// 购物车统计数据DTO
/// </summary>
/// <remarks>
/// 用于返回购物车统计分析数据，包括总购物车项数量、会员数量、平均商品数、热门商品TOP10
/// </remarks>
public class CartStatisticsDto
{
    /// <summary>
    /// 总购物车项数量
    /// </summary>
    public int TotalCartItems { get; set; }

    /// <summary>
    /// 有购物车的会员数量
    /// </summary>
    public int MemberCount { get; set; }

    /// <summary>
    /// 平均每会员购物车商品数
    /// </summary>
    public decimal AvgItemsPerMember { get; set; }

    /// <summary>
    /// 热门商品TOP10（按购物车中出现次数）
    /// </summary>
    public List<HotProductDto> HotProducts { get; set; } = new();
}