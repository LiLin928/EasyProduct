using EasyProduct.Models.Dto.Common;

namespace EasyProduct.Models.Dto.Mall.Payment;

/// <summary>
/// 支付单查询参数
/// </summary>
public class PaymentQuery : PageQuery
{
    /// <summary>
    /// 支付单号
    /// </summary>
    public string? PaymentNo { get; set; }

    /// <summary>
    /// 订单ID
    /// </summary>
    public string? OrderId { get; set; }

    /// <summary>
    /// 会员ID
    /// </summary>
    public string? MemberId { get; set; }

    /// <summary>
    /// 支付状态
    /// </summary>
    public int? Status { get; set; }

    /// <summary>
    /// 支付方式
    /// </summary>
    public int? PaymentMethod { get; set; }

    /// <summary>
    /// 开始时间
    /// </summary>
    public DateTime? StartTime { get; set; }

    /// <summary>
    /// 结束时间
    /// </summary>
    public DateTime? EndTime { get; set; }
}