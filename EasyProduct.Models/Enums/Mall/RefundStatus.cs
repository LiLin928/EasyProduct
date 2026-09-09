namespace EasyProduct.Models.Enums.Mall;

/// <summary>
/// 退款状态枚举
/// </summary>
public enum RefundStatus
{
    /// <summary>
    /// 待审核
    /// </summary>
    Pending = 0,

    /// <summary>
    /// 已通过
    /// </summary>
    Approved = 1,

    /// <summary>
    /// 已拒绝
    /// </summary>
    Rejected = 2,

    /// <summary>
    /// 退货中
    /// </summary>
    Returning = 3,

    /// <summary>
    /// 退款中
    /// </summary>
    Refunding = 4,

    /// <summary>
    /// 已完成
    /// </summary>
    Completed = 5,

    /// <summary>
    /// 已取消
    /// </summary>
    Cancelled = 6
}