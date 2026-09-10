using System.ComponentModel.DataAnnotations;

namespace EasyProduct.Models.Dto.Mall.Refund;

/// <summary>
/// 退款审核 DTO
/// </summary>
/// <remarks>
/// 用于管理员审核退款申请，可选择通过或拒绝
/// </remarks>
public class RefundAuditDto
{
    /// <summary>
    /// 是否通过
    /// </summary>
    /// <remarks>
    /// true 表示通过审核，false 表示拒绝退款
    /// </remarks>
    [Required(ErrorMessage = "审核结果不能为空")]
    public bool Approved { get; set; }

    /// <summary>
    /// 审核备注
    /// </summary>
    /// <remarks>
    /// 审核时填写的备注信息，拒绝时需要说明拒绝原因
    /// </remarks>
    [StringLength(500, ErrorMessage = "审核备注长度不能超过500")]
    public string? AuditRemark { get; set; }
}