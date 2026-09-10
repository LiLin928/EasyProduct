using EasyProduct.Models.Dto.Common;

namespace EasyProduct.Models.Dto.Mall.Refund;

/// <summary>
/// 退款查询参数
/// </summary>
/// <remarks>
/// 用于查询退款列表，支持按退款单号、订单编号、状态和时间范围筛选
/// </remarks>
public class RefundQuery : PageQuery
{
    /// <summary>
    /// 退款单号
    /// </summary>
    /// <remarks>
    /// 退款单的唯一编号，支持模糊查询
    /// </remarks>
    public string? RefundNo { get; set; }

    /// <summary>
    /// 订单编号
    /// </summary>
    /// <remarks>
    /// 关联的订单编号，支持模糊查询
    /// </remarks>
    public string? OrderNo { get; set; }

    /// <summary>
    /// 退款状态
    /// </summary>
    /// <remarks>
    /// 退款状态：待审核、已通过、已拒绝等
    /// </remarks>
    public int? Status { get; set; }

    /// <summary>
    /// 开始时间
    /// </summary>
    /// <remarks>
    /// 创建时间的起始时间，用于时间范围筛选
    /// </remarks>
    public DateTime? StartTime { get; set; }

    /// <summary>
    /// 结束时间
    /// </summary>
    /// <remarks>
    /// 创建时间的结束时间，用于时间范围筛选
    /// </remarks>
    public DateTime? EndTime { get; set; }
}