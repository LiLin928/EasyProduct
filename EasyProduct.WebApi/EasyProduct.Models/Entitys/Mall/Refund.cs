using SqlSugar;
using EasyProduct.Models.Enums.Mall;
using EasyProduct.Models.Entitys.Base;

namespace EasyProduct.Models.Entitys.Mall;

/// <summary>
/// 退款记录实体类
/// </summary>
/// <remarks>
/// 用于记录订单退款信息，包括退款类型、金额、状态等。
/// 支持两种退款类型：
/// - 仅退款：RefundOnly
/// - 退货退款：ReturnAndRefund
///
/// 退款流程状态流转：
/// Pending（待审核）-> Approved（已通过）-> Refunding（退款中）-> Completed（已完成）
/// 或
/// Pending（待审核）-> Approved（已通过）-> Returning（退货中）-> Refunding（退款中）-> Completed（已完成）
/// </remarks>
[SugarTable("mall_refund", "退款记录表")]
public class Refund : BaseEntity
{
    /// <summary>
    /// 退款单号
    /// </summary>
    /// <remarks>
    /// 唯一标识一次退款申请，格式：RF + 时间戳 + 随机数
    /// </remarks>
    [SugarColumn(Length = 50, IsNullable = false)]
    public string RefundNo { get; set; } = string.Empty;

    /// <summary>
    /// 订单ID
    /// </summary>
    /// <remarks>
    /// 关联订单表主键，表示该退款申请所属的订单
    /// </remarks>
    [SugarColumn(Length = 36, IsNullable = false)]
    public string OrderId { get; set; } = string.Empty;

    /// <summary>
    /// 订单项ID
    /// </summary>
    /// <remarks>
    /// 关联订单项表主键，表示该退款申请具体针对哪个商品
    /// </remarks>
    [SugarColumn(Length = 36, IsNullable = false)]
    public string OrderItemId { get; set; } = string.Empty;

    /// <summary>
    /// 会员ID
    /// </summary>
    /// <remarks>
    /// 申请退款的会员ID，关联会员表主键
    /// </remarks>
    [SugarColumn(Length = 36, IsNullable = false)]
    public string MemberId { get; set; } = string.Empty;

    /// <summary>
    /// 退款类型
    /// </summary>
    /// <remarks>
    /// 取值范围：
    /// - RefundOnly (1): 仅退款，不退货
    /// - ReturnAndRefund (2): 退货退款，需要先退货
    /// </remarks>
    public RefundType RefundType { get; set; } = RefundType.RefundOnly;

    /// <summary>
    /// 退款金额
    /// </summary>
    /// <remarks>
    /// 实际退款给用户的金额，单位：元
    /// 保留 2 位小数，不能超过订单项实际支付金额
    /// </remarks>
    [SugarColumn(Length = 18, DecimalDigits = 2, IsNullable = false)]
    public decimal RefundAmount { get; set; }

    /// <summary>
    /// 退款数量
    /// </summary>
    /// <remarks>
    /// 申请退款的商品数量，不能超过订单项购买数量
    /// </remarks>
    [SugarColumn(IsNullable = false)]
    public int RefundQuantity { get; set; }

    /// <summary>
    /// 退款原因
    /// </summary>
    /// <remarks>
    /// 用户填写的退款原因说明，最多 500 字符
    /// </remarks>
    [SugarColumn(Length = 500, IsNullable = false)]
    public string RefundReason { get; set; } = string.Empty;

    /// <summary>
    /// 退款状态
    /// </summary>
    /// <remarks>
    /// 状态流转：
    /// - Pending (0): 待审核
    /// - Approved (1): 已通过
    /// - Rejected (2): 已拒绝
    /// - Returning (3): 退货中（仅退货退款类型）
    /// - Refunding (4): 退款中
    /// - Completed (5): 已完成
    /// - Cancelled (6): 已取消
    /// </remarks>
    public RefundStatus Status { get; set; } = RefundStatus.Pending;

    /// <summary>
    /// 物流公司
    /// </summary>
    /// <remarks>
    /// 退货退款时，用户填写退货物流公司名称
    /// 仅退货退款类型时使用
    /// </remarks>
    [SugarColumn(Length = 50, IsNullable = true)]
    public string? LogisticsCompany { get; set; }

    /// <summary>
    /// 物流单号
    /// </summary>
    /// <remarks>
    /// 退货退款时，用户填写退货物流单号
    /// 仅退货退款类型时使用
    /// </remarks>
    [SugarColumn(Length = 50, IsNullable = true)]
    public string? LogisticsNo { get; set; }

    /// <summary>
    /// 审核备注
    /// </summary>
    /// <remarks>
    /// 管理员审核退款申请时填写的备注信息
    /// 可用于记录审核通过/拒绝的原因
    /// </remarks>
    [SugarColumn(Length = 500, IsNullable = true)]
    public string? AuditRemark { get; set; }

    /// <summary>
    /// 审核时间
    /// </summary>
    /// <remarks>
    /// 管理员审核退款申请的时间
    /// </remarks>
    [SugarColumn(IsNullable = true)]
    public DateTime? AuditTime { get; set; }

    /// <summary>
    /// 退款时间
    /// </summary>
    /// <remarks>
    /// 实际完成退款操作的时间
    /// </remarks>
    [SugarColumn(IsNullable = true)]
    public DateTime? RefundTime { get; set; }
}