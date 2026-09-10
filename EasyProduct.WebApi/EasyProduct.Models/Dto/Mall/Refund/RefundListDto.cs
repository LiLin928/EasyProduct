using EasyProduct.Models.Enums.Mall;

namespace EasyProduct.Models.Dto.Mall.Refund;

/// <summary>
/// 退款列表项 DTO
/// </summary>
/// <remarks>
/// 用于展示退款列表，包含退款基本信息和状态
/// </remarks>
public class RefundListDto
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
    /// 退款金额
    /// </summary>
    /// <remarks>
    /// 申请退款的金额
    /// </remarks>
    public decimal RefundAmount { get; set; }

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
    /// 创建时间
    /// </summary>
    /// <remarks>
    /// 退款申请的创建时间
    /// </remarks>
    public DateTime CreateTime { get; set; }
}