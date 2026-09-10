namespace EasyProduct.Models.Dto.Mall.Order;

/// <summary>
/// 创建订单结果 DTO
/// </summary>
public class CreateOrderResultDto
{
    /// <summary>
    /// 订单ID
    /// </summary>
    public string OrderId { get; set; } = string.Empty;

    /// <summary>
    /// 订单编号
    /// </summary>
    public string OrderNo { get; set; } = string.Empty;

    /// <summary>
    /// 订单总金额
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// 实付金额
    /// </summary>
    public decimal PayAmount { get; set; }
}