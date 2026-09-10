using EasyProduct.Models.Enums.Mall;

namespace EasyProduct.Models.Dto.Mall.Order;

/// <summary>
/// 订单列表项 DTO
/// </summary>
/// <remarks>
/// 用于订单列表展示，包含订单基本信息、会员信息和金额统计
/// </remarks>
public class OrderListDto
{
    /// <summary>
    /// 订单ID
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// 订单编号
    /// </summary>
    public string OrderNo { get; set; } = string.Empty;

    /// <summary>
    /// 会员名称
    /// </summary>
    public string? MemberName { get; set; }

    /// <summary>
    /// 会员电话
    /// </summary>
    public string? MemberPhone { get; set; }

    /// <summary>
    /// 订单状态
    /// </summary>
    public OrderStatus Status { get; set; }

    /// <summary>
    /// 状态文本
    /// </summary>
    /// <remarks>
    /// 订单状态的中文描述，用于前端显示
    /// </remarks>
    public string StatusText { get; set; } = string.Empty;

    /// <summary>
    /// 订单总金额
    /// </summary>
    /// <remarks>
    /// 订单商品总金额（不含运费、优惠）
    /// </remarks>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// 实付金额
    /// </summary>
    /// <remarks>
    /// 订单实际支付金额（含运费、优惠后）
    /// </remarks>
    public decimal PayAmount { get; set; }

    /// <summary>
    /// 商品项数量
    /// </summary>
    /// <remarks>
    /// 订单中包含的商品项总数
    /// </remarks>
    public int ItemCount { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreateTime { get; set; }
}