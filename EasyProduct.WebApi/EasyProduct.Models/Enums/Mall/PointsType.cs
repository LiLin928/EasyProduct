namespace EasyProduct.Models.Enums.Mall;

/// <summary>
/// 积分类型枚举
/// </summary>
/// <remarks>
/// 用于标识积分流水的来源类型：
/// - ConsumeEarn: 消费获得积分
/// - OrderUse: 订单使用积分抵扣
/// - AdminAdjust: 后台管理员手动调整
/// - SignIn: 签到获得积分
/// - RegisterGift: 注册赠送积分
/// </remarks>
public enum PointsType
{
    /// <summary>
    /// 消费获得
    /// </summary>
    /// <remarks>
    /// 会员购买商品时获得的积分奖励
    /// </remarks>
    ConsumeEarn = 1,

    /// <summary>
    /// 订单使用
    /// </summary>
    /// <remarks>
    /// 订单支付时使用积分抵扣
    /// </remarks>
    OrderUse = 2,

    /// <summary>
    /// 后台调整
    /// </summary>
    /// <remarks>
    /// 管理员在后台手动调整会员积分
    /// </remarks>
    AdminAdjust = 3,

    /// <summary>
    /// 签到
    /// </summary>
    /// <remarks>
    /// 会员签到获得的积分奖励
    /// </remarks>
    SignIn = 4,

    /// <summary>
    /// 注册赠送
    /// </summary>
    /// <remarks>
    /// 新会员注册时赠送的积分
    /// </remarks>
    RegisterGift = 5
}