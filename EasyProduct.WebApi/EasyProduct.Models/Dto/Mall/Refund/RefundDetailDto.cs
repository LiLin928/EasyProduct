using EasyProduct.Models.Dto.Mall.Order;
using EasyProduct.Models.Enums.Mall;

namespace EasyProduct.Models.Dto.Mall.Refund;

/// <summary>
/// 退款详情 DTO
/// </summary>
/// <remarks>
/// 用于展示退款详细信息，包含退款基本信息、物流信息和订单项信息
/// </remarks>
public class RefundDetailDto
{
    /// <summary>
    /// 退款ID
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// 退款单号
    /// </summary>
    /// <remarks>
    /// 退款单的唯一编号，格式为 RF + 时间戳
    /// </remarks>
    public string RefundNo { get; set; } = string.Empty;

    /// <summary>
    /// 订单编号
    /// </summary>
    /// <remarks>
    /// 关联的订单编号
    /// </remarks>
    public string OrderNo { get; set; } = string.Empty;

    /// <summary>
    /// 退款类型
    /// </summary>
    /// <remarks>
    /// 退款类型：仅退款或退货退款
    /// </remarks>
    public RefundType RefundType { get; set; }

    /// <summary>
    /// 退款类型文本
    /// </summary>
    /// <remarks>
    /// 退款类型的中文描述，如"仅退款"、"退货退款"
    /// </remarks>
    public string RefundTypeText { get; set; } = string.Empty;

    /// <summary>
    /// 退款数量
    /// </summary>
    /// <remarks>
    /// 申请退款的数量
    /// </remarks>
    public int RefundQuantity { get; set; }

    /// <summary>
    /// 退款金额
    /// </summary>
    /// <remarks>
    /// 申请退款的金额
    /// </remarks>
    public decimal RefundAmount { get; set; }

    /// <summary>
    /// 退款原因
    /// </summary>
    /// <remarks>
    /// 申请退款的原因说明
    /// </remarks>
    public string RefundReason { get; set; } = string.Empty;

    /// <summary>
    /// 退款状态
    /// </summary>
    /// <remarks>
    /// 退款状态：待审核、已通过、已拒绝等
    /// </remarks>
    public RefundStatus Status { get; set; }

    /// <summary>
    /// 状态文本
    /// </summary>
    /// <remarks>
    /// 退款状态的中文描述，如"待审核"、"已通过"等
    /// </remarks>
    public string StatusText { get; set; } = string.Empty;

    /// <summary>
    /// 审核备注
    /// </summary>
    /// <remarks>
    /// 管理员审核退款时填写的备注信息
    /// </remarks>
    public string? AuditRemark { get; set; }

    /// <summary>
    /// 物流公司
    /// </summary>
    /// <remarks>
    /// 退货时填写的物流公司名称
    /// </remarks>
    public string? LogisticsCompany { get; set; }

    /// <summary>
    /// 物流单号
    /// </summary>
    /// <remarks>
    /// 退货时填写的物流单号
    /// </remarks>
    public string? LogisticsNo { get; set; }

    /// <summary>
    /// 订单项信息
    /// </summary>
    /// <remarks>
    /// 关联的订单项详细信息，包含商品名称、SKU信息、价格等
    /// </remarks>
    public OrderItemDto? OrderItem { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    /// <remarks>
    /// 退款申请的创建时间
    /// </remarks>
    public DateTime CreateTime { get; set; }
}