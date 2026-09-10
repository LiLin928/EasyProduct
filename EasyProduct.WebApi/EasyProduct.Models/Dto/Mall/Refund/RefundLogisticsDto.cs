using System.ComponentModel.DataAnnotations;

namespace EasyProduct.Models.Dto.Mall.Refund;

/// <summary>
/// 退款物流 DTO
/// </summary>
/// <remarks>
/// 用于会员填写退货物流信息
/// </remarks>
public class RefundLogisticsDto
{
    /// <summary>
    /// 物流公司
    /// </summary>
    /// <remarks>
    /// 物流公司名称，如顺丰、中通、圆通等
    /// </remarks>
    [Required(ErrorMessage = "物流公司不能为空")]
    [StringLength(50, ErrorMessage = "物流公司长度不能超过50")]
    public string LogisticsCompany { get; set; } = string.Empty;

    /// <summary>
    /// 物流单号
    /// </summary>
    /// <remarks>
    /// 物流公司的快递单号
    /// </remarks>
    [Required(ErrorMessage = "物流单号不能为空")]
    [StringLength(50, ErrorMessage = "物流单号长度不能超过50")]
    public string LogisticsNo { get; set; } = string.Empty;
}