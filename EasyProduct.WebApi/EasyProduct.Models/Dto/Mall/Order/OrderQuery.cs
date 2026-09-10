using EasyProduct.Models.Dto.Common;

namespace EasyProduct.Models.Dto.Mall.Order;

/// <summary>
/// 订单查询参数
/// </summary>
/// <remarks>
/// 用于管理端和小程序端查询订单列表，支持多条件筛选和分页
/// </remarks>
public class OrderQuery : PageQuery
{
    /// <summary>
    /// 订单编号
    /// </summary>
    /// <remarks>
    /// 精确匹配订单编号
    /// </remarks>
    public string? OrderNo { get; set; }

    /// <summary>
    /// 会员ID
    /// </summary>
    /// <remarks>
    /// 按会员筛选订单，精确匹配会员ID
    /// </remarks>
    public string? MemberId { get; set; }

    /// <summary>
    /// 订单状态
    /// </summary>
    /// <remarks>
    /// 按订单状态筛选，对应 OrderStatus 枚举值
    /// </remarks>
    public int? Status { get; set; }

    /// <summary>
    /// 关键词（会员名称/电话）
    /// </summary>
    /// <remarks>
    /// 模糊搜索会员名称或电话号码
    /// </remarks>
    public string? Keyword { get; set; }

    /// <summary>
    /// 开始时间
    /// </summary>
    /// <remarks>
    /// 订单创建时间的起始时间，用于时间范围查询
    /// </remarks>
    public DateTime? StartTime { get; set; }

    /// <summary>
    /// 结束时间
    /// </summary>
    /// <remarks>
    /// 订单创建时间的结束时间，用于时间范围查询
    /// </remarks>
    public DateTime? EndTime { get; set; }
}