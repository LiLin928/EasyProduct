namespace EasyProduct.Models.Enums.Mall;

/// <summary>
/// 积分来源枚举
/// </summary>
public enum PointSource
{
    /// <summary>
    /// 下单赠送
    /// </summary>
    Order = 1,

    /// <summary>
    /// 评价赠送
    /// </summary>
    Review = 2,

    /// <summary>
    /// 签到赠送
    /// </summary>
    CheckIn = 3,

    /// <summary>
    /// 邀请赠送
    /// </summary>
    Invite = 4,

    /// <summary>
    /// 兑换优惠券
    /// </summary>
    ExchangeCoupon = 5,

    /// <summary>
    /// 订单抵扣
    /// </summary>
    OrderDeduction = 6,

    /// <summary>
    /// 系统调整
    /// </summary>
    SystemAdjust = 7,

    /// <summary>
    /// 积分过期
    /// </summary>
    Expired = 8
}