using EasyProduct.Common.Base;
using EasyProduct.Models.Dto.Mall.Order;
using EasyProduct.Business.Mall;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyProduct.Web.Controllers.Admin.Mall;

/// <summary>
/// 订单管理控制器（管理端）
/// </summary>
/// <remarks>
/// 提供订单管理相关的 REST API 接口，包括订单查询、发货、统计等功能。
/// 路由前缀：/api/admin/mall/order
/// 认证方式：AdminJwt（管理员身份）
/// </remarks>
[ApiController]
[Route("api/admin/mall/order")]
[Authorize(AuthenticationSchemes = "AdminJwt")]
public class OrderAdminController : BaseController
{
    private readonly IOrderService _orderService;

    /// <summary>
    /// 构造函数
    /// </summary>
    public OrderAdminController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    /// <summary>
    /// 分页查询订单列表
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>订单分页列表</returns>
    /// <remarks>
    /// 查询所有订单列表，支持按订单号、会员ID、状态和时间范围筛选。
    /// </remarks>
    [HttpGet("list")]
    public async Task<ApiResponse<PageResponse<OrderListDto>>> GetList([FromQuery] OrderQuery query)
    {
        var result = await _orderService.GetOrderListAsync(query);
        return Success(result);
    }

    /// <summary>
    /// 获取订单详情
    /// </summary>
    /// <param name="id">订单ID</param>
    /// <returns>订单详情</returns>
    /// <remarks>
    /// 查询订单详细信息，包含订单项、商品信息等。
    /// </remarks>
    [HttpGet("{id}")]
    public async Task<ApiResponse<OrderDetailDto>> GetDetail(string id)
    {
        var result = await _orderService.GetOrderDetailAsync(id);
        return Success(result);
    }

    /// <summary>
    /// 发货
    /// </summary>
    /// <param name="id">订单ID</param>
    /// <param name="dto">发货参数</param>
    /// <returns>是否成功</returns>
    /// <remarks>
    /// 管理员发货操作，需要填写物流公司和物流单号。
    /// 发货成功后，订单状态会从待发货变更为待收货。
    /// </remarks>
    [HttpPost("{id}/deliver")]
    public async Task<ApiResponse<bool>> Deliver(string id, [FromBody] OrderDeliverDto dto)
    {
        var result = await _orderService.DeliverOrderAsync(id, dto);
        return Success(result);
    }

    /// <summary>
    /// 取消订单
    /// </summary>
    /// <param name="id">订单ID</param>
    /// <param name="dto">取消参数</param>
    /// <returns>是否成功</returns>
    /// <remarks>
    /// 管理员取消订单操作，只有待付款和待发货状态的订单可以取消。
    /// 取消成功后会恢复库存。
    /// </remarks>
    [HttpPost("{id}/cancel")]
    public async Task<ApiResponse<bool>> Cancel(string id, [FromBody] OrderCancelDto dto)
    {
        var operatorId = GetCurrentUserId().ToString();
        var result = await _orderService.CancelOrderAsync(id, dto, operatorId);
        return Success(result);
    }

    /// <summary>
    /// 获取订单统计数据
    /// </summary>
    /// <returns>订单统计数据</returns>
    /// <remarks>
    /// 获取所有订单的统计信息，包括各状态的订单数量、总金额等。
    /// </remarks>
    [HttpGet("statistics")]
    public async Task<ApiResponse<OrderStatisticsDto>> GetStatistics()
    {
        var result = await _orderService.GetOrderStatisticsAsync();
        return Success(result);
    }

    /// <summary>
    /// 根据订单号查询订单详情
    /// </summary>
    /// <param name="orderNo">订单号</param>
    /// <returns>订单详情</returns>
    /// <remarks>
    /// 根据订单号查询订单详细信息。
    /// </remarks>
    [HttpGet("no/{orderNo}")]
    public async Task<ApiResponse<OrderDetailDto>> GetDetailByNo(string orderNo)
    {
        var result = await _orderService.GetOrderDetailByNoAsync(orderNo);
        return Success(result);
    }
}