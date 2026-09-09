namespace EasyProduct.Models.Dto.Mall.Cart;

/// <summary>
/// 购物车统计数据DTO
/// </summary>
/// <remarks>
/// 用于返回购物车的统计数据，包含总商品数、总金额、会员活跃度等信息
/// </remarks>
public class CartStatisticsDto
{
    /// <summary>
    /// 总商品数（所有会员购物车中的商品数量总和）
    /// </summary>
    public int TotalItems { get; set; }

    /// <summary>
    /// 总金额（所有购物车商品的金额总和）
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// 有购物车的会员数
    /// </summary>
    public int MemberCount { get; set; }

    /// <summary>
    /// 平均每位会员的商品数
    /// </summary>
    public decimal AvgItemsPerMember { get; set; }

    /// <summary>
    /// 平均每位会员的购物车金额
    /// </summary>
    public decimal AvgAmountPerMember { get; set; }

    /// <summary>
    /// 失效商品数
    /// </summary>
    public int InvalidItems { get; set; }

    /// <summary>
    /// 今日新增购物车商品数
    /// </summary>
    public int TodayNewItems { get; set; }

    /// <summary>
    /// 本周新增购物车商品数
    /// </summary>
    public int WeekNewItems { get; set; }

    /// <summary>
    /// 本月新增购物车商品数
    /// </summary>
    public int MonthNewItems { get; set; }
}