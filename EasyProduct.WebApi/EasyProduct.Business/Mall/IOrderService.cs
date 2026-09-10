using EasyProduct.Common.Base;
using EasyProduct.Models.Dto.Mall.Order;

namespace EasyProduct.Business.Mall;

/// <summary>
/// 订单服务接口
/// </summary>
/// <remarks>
/// 提供订单的创建、查询、状态管理、取消等核心业务功能。
/// 订单流程：创建订单 → 支付 → 发货 → 收货 → 完成。
/// 支持订单取消、退款等操作。
/// </remarks>
public interface IOrderService
{
    #region 创建订单

    /// <summary>
    /// 从购物车创建订单
    /// </summary>
    /// <param name="memberId">会员ID</param>
    /// <param name="dto">创建订单参数</param>
    /// <returns>订单创建结果，包含订单ID和订单号</returns>
    Task<CreateOrderResultDto> CreateOrderFromCartAsync(string memberId, CreateOrderFromCartDto dto);

    /// <summary>
    /// 直接购买创建订单
    /// </summary>
    /// <param name="memberId">会员ID</param>
    /// <param name="dto">创建订单参数</param>
    /// <returns>订单创建结果，包含订单ID和订单号</returns>
    Task<CreateOrderResultDto> CreateOrderDirectAsync(string memberId, CreateOrderDirectDto dto);

    #endregion

    #region 查询订单

    /// <summary>
    /// 分页查询订单列表
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>订单分页列表</returns>
    Task<PageResponse<OrderListDto>> GetOrderListAsync(OrderQuery query);

    /// <summary>
    /// 获取订单详情
    /// </summary>
    /// <param name="orderId">订单ID</param>
    /// <returns>订单详情</returns>
    Task<OrderDetailDto> GetOrderDetailAsync(string orderId);

    /// <summary>
    /// 根据订单号获取订单详情
    /// </summary>
    /// <param name="orderNo">订单号</param>
    /// <returns>订单详情</returns>
    Task<OrderDetailDto> GetOrderDetailByNoAsync(string orderNo);

    /// <summary>
    /// 获取会员订单列表
    /// </summary>
    /// <param name="memberId">会员ID</param>
    /// <param name="status">订单状态（可选）</param>
    /// <param name="pageIndex">页码</param>
    /// <param name="pageSize">每页数量</param>
    /// <returns>订单分页列表</returns>
    Task<PageResponse<OrderListDto>> GetMemberOrderListAsync(string memberId, string? status = null, int pageIndex = 1, int pageSize = 10);

    /// <summary>
    /// 获取订单统计数据
    /// </summary>
    /// <param name="memberId">会员ID（可选，Admin查询时为null）</param>
    /// <returns>订单统计数据</returns>
    Task<OrderStatisticsDto> GetOrderStatisticsAsync(string? memberId = null);

    #endregion

    #region 订单状态管理

    /// <summary>
    /// 取消订单
    /// </summary>
    /// <param name="orderId">订单ID</param>
    /// <param name="dto">取消参数</param>
    /// <param name="operatorId">操作人ID（可选，Admin操作时传入）</param>
    /// <returns>是否成功</returns>
    Task<bool> CancelOrderAsync(string orderId, OrderCancelDto dto, string? operatorId = null);

    /// <summary>
    /// 发货
    /// </summary>
    /// <param name="orderId">订单ID</param>
    /// <param name="dto">发货参数</param>
    /// <returns>是否成功</returns>
    Task<bool> DeliverOrderAsync(string orderId, OrderDeliverDto dto);

    /// <summary>
    /// 确认收货
    /// </summary>
    /// <param name="orderId">订单ID</param>
    /// <returns>是否成功</returns>
    Task<bool> ConfirmReceiveAsync(string orderId);

    /// <summary>
    /// 更新订单状态（系统内部调用）
    /// </summary>
    /// <param name="orderId">订单ID</param>
    /// <param name="status">新状态</param>
    /// <param name="remark">备注</param>
    /// <returns>是否成功</returns>
    Task<bool> UpdateOrderStatusAsync(string orderId, string status, string? remark = null);

    #endregion

    #region 订单支付

    /// <summary>
    /// 标记订单为已支付（支付服务调用）
    /// </summary>
    /// <param name="orderId">订单ID</param>
    /// <param name="paymentTime">支付时间</param>
    /// <returns>是否成功</returns>
    Task<bool> MarkOrderPaidAsync(string orderId, DateTime paymentTime);

    #endregion
}