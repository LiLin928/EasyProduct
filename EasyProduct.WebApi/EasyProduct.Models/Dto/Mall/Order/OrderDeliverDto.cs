using System.ComponentModel.DataAnnotations;

namespace EasyProduct.Models.Dto.Mall.Order;

/// <summary>
/// 订单发货 DTO
/// </summary>
public class OrderDeliverDto
{
    /// <summary>
    /// 物流公司
    /// </summary>
    [Required(ErrorMessage = "物流公司不能为空")]
    [StringLength(50, ErrorMessage = "物流公司长度不能超过50")]
    public string LogisticsCompany { get; set; } = string.Empty;

    /// <summary>
    /// 物流单号
    /// </summary>
    [Required(ErrorMessage = "物流单号不能为空")]
    [StringLength(50, ErrorMessage = "物流单号长度不能超过50")]
    public string LogisticsNo { get; set; } = string.Empty;
}