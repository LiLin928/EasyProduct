namespace EasyProduct.Models.Dto.Mall.Order;

/// <summary>
/// 订单统计 DTO
/// </summary>
public class OrderStatisticsDto
{
    /// <summary>
    /// 总订单数
    /// </summary>
    public int TotalOrders { get; set; }

    /// <summary>
    /// 待付款订单数
    /// </summary>
    public int PendingPayment { get; set; }

    /// <summary>
    /// 待发货订单数
    /// </summary>
    public int PendingDelivery { get; set; }

    /// <summary>
    /// 待收货订单数
    /// </summary>
    public int PendingReceive { get; set; }

    /// <summary>
    /// 已完成订单数
    /// </summary>
    public int Completed { get; set; }

    /// <summary>
    /// 已取消订单数
    /// </summary>
    public int Cancelled { get; set; }

    /// <summary>
    /// 退款中订单数
    /// </summary>
    public int Refunding { get; set; }

    /// <summary>
    /// 总金额
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// 今日订单数
    /// </summary>
    public int TodayOrders { get; set; }

    /// <summary>
    /// 今日金额
    /// </summary>
    public decimal TodayAmount { get; set; }
}