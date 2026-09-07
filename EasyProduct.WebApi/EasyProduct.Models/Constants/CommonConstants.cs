namespace EasyProduct.Models.Constants;

/// <summary>
/// 通用状态常量
/// </summary>
/// <remarks>
/// 用于所有实体的 Status 字段
/// </remarks>
public static class CommonStatus
{
    /// <summary>
    /// 启用/正常
    /// </summary>
    public const int Enabled = 1;

    /// <summary>
    /// 禁用/停用
    /// </summary>
    public const int Disabled = 0;
}

/// <summary>
/// 删除状态常量
/// </summary>
/// <remarks>
/// 用于所有实体的 IsDeleted 字段
/// </remarks>
public static class DeleteStatus
{
    /// <summary>
    /// 未删除
    /// </summary>
    public const int NotDeleted = 0;

    /// <summary>
    /// 已删除
    /// </summary>
    public const int Deleted = 1;
}

/// <summary>
/// 用户状态常量
/// </summary>
public static class UserStatus
{
    /// <summary>
    /// 活跃
    /// </summary>
    public const int Active = 1;

    /// <summary>
    /// 禁用
    /// </summary>
    public const int Inactive = 0;
}

/// <summary>
/// 订单状态常量
/// </summary>
public static class OrderStatus
{
    /// <summary>
    /// 待支付
    /// </summary>
    public const int Pending = 10;

    /// <summary>
    /// 已支付
    /// </summary>
    public const int Paid = 20;

    /// <summary>
    /// 已发货
    /// </summary>
    public const int Shipped = 30;

    /// <summary>
    /// 已完成
    /// </summary>
    public const int Completed = 40;

    /// <summary>
    /// 已取消
    /// </summary>
    public const int Cancelled = 50;

    /// <summary>
    /// 已退款
    /// </summary>
    public const int Refunded = 60;
}

/// <summary>
/// 支付状态常量
/// </summary>
public static class PaymentStatus
{
    /// <summary>
    /// 待支付
    /// </summary>
    public const int Pending = 10;

    /// <summary>
    /// 支付成功
    /// </summary>
    public const int Success = 20;

    /// <summary>
    /// 支付失败
    /// </summary>
    public const int Failed = 30;

    /// <summary>
    /// 已退款
    /// </summary>
    public const int Refunded = 40;
}

/// <summary>
/// 支付方式常量
/// </summary>
public static class PaymentMethod
{
    /// <summary>
    /// 微信支付
    /// </summary>
    public const int WeChat = 1;

    /// <summary>
    /// 支付宝
    /// </summary>
    public const int Alipay = 2;

    /// <summary>
    /// 银行转账
    /// </summary>
    public const int Bank = 3;

    /// <summary>
    /// 现金
    /// </summary>
    public const int Cash = 4;
}

/// <summary>
/// 积分类型常量
/// </summary>
public static class PointsType
{
    /// <summary>
    /// 获得
    /// </summary>
    public const int Earn = 1;

    /// <summary>
    /// 消费
    /// </summary>
    public const int Spend = 2;
}

/// <summary>
/// 优惠券类型常量
/// </summary>
public static class CouponType
{
    /// <summary>
    /// 固定金额
    /// </summary>
    public const int Fixed = 1;

    /// <summary>
    /// 百分比折扣
    /// </summary>
    public const int Percent = 2;
}