using EasyProduct.Models.Enums.Mall;

namespace EasyProduct.Models.Dto.Mall.Order;

/// <summary>
/// 订单详情 DTO
/// </summary>
/// <remarks>
/// 用于订单详情展示，包含订单完整信息、收货信息、支付信息、物流信息和订单项列表
/// </remarks>
public class OrderDetailDto
{
    /// <summary>
    /// 订单ID
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// 订单编号
    /// </summary>
    public string OrderNo { get; set; } = string.Empty;

    /// <summary>
    /// 会员ID
    /// </summary>
    public string MemberId { get; set; } = string.Empty;

    /// <summary>
    /// 会员名称
    /// </summary>
    public string? MemberName { get; set; }

    /// <summary>
    /// 会员电话
    /// </summary>
    public string? MemberPhone { get; set; }

    /// <summary>
    /// 订单状态
    /// </summary>
    public OrderStatus Status { get; set; }

    /// <summary>
    /// 状态文本
    /// </summary>
    /// <remarks>
    /// 订单状态的中文描述，用于前端显示
    /// </remarks>
    public string StatusText { get; set; } = string.Empty;

    /// <summary>
    /// 收货人姓名
    /// </summary>
    public string ReceiverName { get; set; } = string.Empty;

    /// <summary>
    /// 收货人电话
    /// </summary>
    public string ReceiverPhone { get; set; } = string.Empty;

    /// <summary>
    /// 收货地址
    /// </summary>
    /// <remarks>
    /// 完整收货地址（省市区 + 详细地址）
    /// </remarks>
    public string ReceiverAddress { get; set; } = string.Empty;

    /// <summary>
    /// 订单总金额
    /// </summary>
    /// <remarks>
    /// 订单商品总金额（不含运费、优惠）
    /// </remarks>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// 运费
    /// </summary>
    public decimal FreightAmount { get; set; }

    /// <summary>
    /// 优惠金额
    /// </summary>
    /// <remarks>
    /// 优惠券折扣金额
    /// </remarks>
    public decimal DiscountAmount { get; set; }

    /// <summary>
    /// 实付金额
    /// </summary>
    /// <remarks>
    /// 订单实际支付金额（总金额 + 运费 - 优惠金额）
    /// </remarks>
    public decimal PayAmount { get; set; }

    /// <summary>
    /// 优惠券名称
    /// </summary>
    public string? CouponName { get; set; }

    /// <summary>
    /// 物流公司
    /// </summary>
    public string? LogisticsCompany { get; set; }

    /// <summary>
    /// 物流单号
    /// </summary>
    public string? LogisticsNo { get; set; }

    /// <summary>
    /// 会员备注
    /// </summary>
    /// <remarks>
    /// 下单时会员填写的备注信息
    /// </remarks>
    public string? Remark { get; set; }

    /// <summary>
    /// 后台备注
    /// </summary>
    /// <remarks>
    /// 管理员填写的内部备注信息，会员不可见
    /// </remarks>
    public string? AdminRemark { get; set; }

    /// <summary>
    /// 订单项列表
    /// </summary>
    public List<OrderItemDto> Items { get; set; } = new();

    /// <summary>
    /// 支付时间
    /// </summary>
    public DateTime? PaymentTime { get; set; }

    /// <summary>
    /// 发货时间
    /// </summary>
    public DateTime? DeliveryTime { get; set; }

    /// <summary>
    /// 收货时间
    /// </summary>
    public DateTime? ReceiveTime { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreateTime { get; set; }
}