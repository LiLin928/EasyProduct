using SqlSugar;
using EasyProduct.Models.Enums.Mall;
using EasyProduct.Models.Entitys.Base;

namespace EasyProduct.Models.Entitys.Mall;

/// <summary>
/// 订单实体类
/// </summary>
/// <remarks>
/// 订单主表，记录订单基本信息、收货信息、金额信息、状态流转等。
/// 订单编号规则：按业务规则生成唯一编号。
/// 订单状态流转：待付款 → 待发货 → 待收货 → 已完成。
/// 支持取消、退款操作。
/// </remarks>
[SugarTable("mall_order", "订单表")]
public class Order : BaseEntity
{
    /// <summary>
    /// 订单编号
    /// </summary>
    /// <remarks>
    /// 业务订单号，按规则生成，格式如：YYYYMMDDHHMMSS + 随机数。
    /// 用于对外展示，不使用 GUID。
    /// </remarks>
    [SugarColumn(Length = 50, IsNullable = false)]
    public string OrderNo { get; set; } = string.Empty;

    /// <summary>
    /// 会员ID
    /// </summary>
    /// <remarks>
    /// 关联会员表 mall_member 的 Id 字段。
    /// 订单归属于该会员。
    /// </remarks>
    [SugarColumn(Length = 36, IsNullable = false)]
    public string MemberId { get; set; } = string.Empty;

    /// <summary>
    /// 收货人姓名
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = false)]
    public string ReceiverName { get; set; } = string.Empty;

    /// <summary>
    /// 收货人电话
    /// </summary>
    [SugarColumn(Length = 20, IsNullable = false)]
    public string ReceiverPhone { get; set; } = string.Empty;

    /// <summary>
    /// 收货地址
    /// </summary>
    /// <remarks>
    /// 完整收货地址，包含省市区街道门牌号。
    /// </remarks>
    [SugarColumn(Length = 200, IsNullable = false)]
    public string ReceiverAddress { get; set; } = string.Empty;

    /// <summary>
    /// 订单总金额
    /// </summary>
    /// <remarks>
    /// 商品总金额 + 运费 - 优惠金额 = 实付金额。
    /// 保留 2 位小数。
    /// </remarks>
    [SugarColumn(Length = 18, DecimalDigits = 2, IsNullable = false)]
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// 实付金额
    /// </summary>
    /// <remarks>
    /// 订单总金额 - 优惠金额 = 实付金额。
    /// 保留 2 位小数。
    /// </remarks>
    [SugarColumn(Length = 18, DecimalDigits = 2, IsNullable = false)]
    public decimal PayAmount { get; set; }

    /// <summary>
    /// 运费
    /// </summary>
    /// <remarks>
    /// 物流运费，保留 2 位小数。
    /// </remarks>
    [SugarColumn(Length = 18, DecimalDigits = 2, IsNullable = false)]
    public decimal FreightAmount { get; set; }

    /// <summary>
    /// 优惠金额
    /// </summary>
    /// <remarks>
    /// 优惠券抵扣金额 + 活动优惠金额。
    /// 保留 2 位小数。
    /// </remarks>
    [SugarColumn(Length = 18, DecimalDigits = 2, IsNullable = false)]
    public decimal DiscountAmount { get; set; }

    /// <summary>
    /// 订单状态
    /// </summary>
    /// <remarks>
    /// 使用 OrderStatus 枚举：
    /// - PendingPayment: 待付款
    /// - PendingDelivery: 待发货
    /// - PendingReceive: 待收货
    /// - Completed: 已完成
    /// - Cancelled: 已取消
    /// - Refunding: 退款中
    /// - Refunded: 已退款
    /// </remarks>
    public OrderStatus Status { get; set; } = OrderStatus.PendingPayment;

    /// <summary>
    /// 支付时间
    /// </summary>
    /// <remarks>
    /// 订单支付成功后的时间戳。
    /// 未支付时为 null。
    /// </remarks>
    [SugarColumn(IsNullable = true)]
    public DateTime? PaymentTime { get; set; }

    /// <summary>
    /// 发货时间
    /// </summary>
    /// <remarks>
    /// 商家发货后的时间戳。
    /// 未发货时为 null。
    /// </remarks>
    [SugarColumn(IsNullable = true)]
    public DateTime? DeliveryTime { get; set; }

    /// <summary>
    /// 收货时间
    /// </summary>
    /// <remarks>
    /// 会员确认收货的时间戳。
    /// 未收货时为 null。
    /// </remarks>
    [SugarColumn(IsNullable = true)]
    public DateTime? ReceiveTime { get; set; }

    /// <summary>
    /// 完成时间
    /// </summary>
    /// <remarks>
    /// 订单完成的时间戳（订单完成后不可再修改）。
    /// 未完成时为 null。
    /// </remarks>
    [SugarColumn(IsNullable = true)]
    public DateTime? CompleteTime { get; set; }

    /// <summary>
    /// 取消时间
    /// </summary>
    /// <remarks>
    /// 订单取消的时间戳。
    /// 未取消时为 null。
    /// </remarks>
    [SugarColumn(IsNullable = true)]
    public DateTime? CancelTime { get; set; }

    /// <summary>
    /// 物流公司
    /// </summary>
    /// <remarks>
    /// 物流公司名称，如：顺丰、中通、圆通等。
    /// 未发货时为 null。
    /// </remarks>
    [SugarColumn(Length = 50, IsNullable = true)]
    public string? LogisticsCompany { get; set; }

    /// <summary>
    /// 物流单号
    /// </summary>
    /// <remarks>
    /// 物流公司提供的快递单号。
    /// 未发货时为 null。
    /// </remarks>
    [SugarColumn(Length = 50, IsNullable = true)]
    public string? LogisticsNo { get; set; }

    /// <summary>
    /// 优惠券ID
    /// </summary>
    /// <remarks>
    /// 关联优惠券表。
    /// 未使用优惠券时为 null。
    /// </remarks>
    [SugarColumn(IsNullable = true)]
    public Guid? CouponId { get; set; }

    /// <summary>
    /// 优惠券名称
    /// </summary>
    /// <remarks>
    /// 冗余字段，用于显示优惠券名称。
    /// 未使用优惠券时为 null。
    /// </remarks>
    [SugarColumn(Length = 100, IsNullable = true)]
    public string? CouponName { get; set; }

    /// <summary>
    /// 会员备注
    /// </summary>
    /// <remarks>
    /// 下单时会员填写的备注信息。
    /// </remarks>
    [SugarColumn(Length = 500, IsNullable = true)]
    public string? Remark { get; set; }

    /// <summary>
    /// 后台备注
    /// </summary>
    /// <remarks>
    /// 管理员添加的备注信息（会员不可见）。
    /// </remarks>
    [SugarColumn(Length = 500, IsNullable = true)]
    public string? AdminRemark { get; set; }

    /// <summary>
    /// 订单来源
    /// </summary>
    /// <remarks>
    /// 订单来源标识：
    /// - cart: 从购物车下单
    /// - direct: 直接购买
    /// </remarks>
    [SugarColumn(Length = 20, IsNullable = false)]
    public string Source { get; set; } = "cart";
}