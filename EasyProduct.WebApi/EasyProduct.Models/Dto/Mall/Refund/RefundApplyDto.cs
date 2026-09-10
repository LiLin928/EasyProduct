using System.ComponentModel.DataAnnotations;
using EasyProduct.Models.Enums.Mall;

namespace EasyProduct.Models.Dto.Mall.Refund;

/// <summary>
/// 申请退款 DTO
/// </summary>
/// <remarks>
/// 用于会员申请订单退款，包含退款类型、数量、金额和原因等信息
/// </remarks>
public class RefundApplyDto
{
    /// <summary>
    /// 订单ID
    /// </summary>
    /// <remarks>
    /// 需要申请退款的订单ID
    /// </remarks>
    [Required(ErrorMessage = "订单ID不能为空")]
    [StringLength(36, ErrorMessage = "订单ID长度不能超过36个字符")]
    public string OrderId { get; set; } = string.Empty;

    /// <summary>
    /// 订单项ID
    /// </summary>
    /// <remarks>
    /// 需要申请退款的订单项ID
    /// </remarks>
    [Required(ErrorMessage = "订单项ID不能为空")]
    [StringLength(36, ErrorMessage = "订单项ID长度不能超过36个字符")]
    public string OrderItemId { get; set; } = string.Empty;

    /// <summary>
    /// 退款类型
    /// </summary>
    /// <remarks>
    /// 退款类型：仅退款或退货退款
    /// </remarks>
    [Required(ErrorMessage = "退款类型不能为空")]
    public RefundType RefundType { get; set; }

    /// <summary>
    /// 退款数量
    /// </summary>
    /// <remarks>
    /// 申请退款的数量，不能超过订单项的数量
    /// </remarks>
    [Required(ErrorMessage = "退款数量不能为空")]
    [Range(1, int.MaxValue, ErrorMessage = "退款数量必须大于0")]
    public int RefundQuantity { get; set; }

    /// <summary>
    /// 退款金额
    /// </summary>
    /// <remarks>
    /// 申请退款的金额，不能超过订单项的小计金额
    /// </remarks>
    [Required(ErrorMessage = "退款金额不能为空")]
    [Range(0.01, double.MaxValue, ErrorMessage = "退款金额必须大于0")]
    public decimal RefundAmount { get; set; }

    /// <summary>
    /// 退款原因
    /// </summary>
    /// <remarks>
    /// 申请退款的原因说明
    /// </remarks>
    [Required(ErrorMessage = "退款原因不能为空")]
    [StringLength(500, ErrorMessage = "退款原因长度不能超过500")]
    public string RefundReason { get; set; } = string.Empty;
}