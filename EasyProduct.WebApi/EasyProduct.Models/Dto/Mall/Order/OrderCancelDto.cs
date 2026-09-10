using System.ComponentModel.DataAnnotations;

namespace EasyProduct.Models.Dto.Mall.Order;

/// <summary>
/// 订单取消 DTO
/// </summary>
public class OrderCancelDto
{
    /// <summary>
    /// 取消原因
    /// </summary>
    [StringLength(500, ErrorMessage = "取消原因长度不能超过500")]
    public string? CancelReason { get; set; }
}