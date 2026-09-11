namespace EasyProduct.Models.Dto.Product.App;

/// <summary>
/// 销量统计 DTO
/// </summary>
/// <remarks>
/// 用于展示商品的销量统计信息
/// </remarks>
public class SalesStatisticsDto
{
    /// <summary>
    /// 总销量
    /// </summary>
    /// <remarks>
    /// 商品累计总销量
    /// </remarks>
    public int TotalSales { get; set; }

    /// <summary>
    /// 月销量
    /// </summary>
    /// <remarks>
    /// 最近 30 天销量
    /// </remarks>
    public int MonthSales { get; set; }
}