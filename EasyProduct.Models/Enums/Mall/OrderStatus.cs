namespace EasyProduct.Models.Enums.Mall;

/// <summary>
/// 订单状态枚举
/// </summary>
public enum OrderStatus
{
    /// <summary>
    /// 待付款
    /// </summary>
    PendingPayment = 0,

    /// <summary>
    /// 待发货
    /// </summary>
    PendingDelivery = 1,

    /// <summary>
    /// 待收货
    /// </summary>
    PendingReceive = 2,

    /// <summary>
    /// 已完成
    /// </summary>
    Completed = 3,

    /// <summary>
    /// 已取消
    /// </summary>
    Cancelled = 4,

    /// <summary>
    /// 退款中
    /// </summary>
    Refunding = 5,

    /// <summary>
    /// 已退款
    /// </summary>
    Refunded = 6
}