namespace EasyProduct.Models.Enums.Mall;

/// <summary>
/// 积分规则类型枚举
/// </summary>
public enum PointRuleType
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
    Invite = 4
}