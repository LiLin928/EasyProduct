using EasyProduct.Common.Base;
using EasyProduct.Models.Dto.Mall.Order;
using EasyProduct.Business.Mall;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyProduct.Web.Controllers.App.Mall;

/// <summary>
/// 订单控制器
/// </summary>
/// <remarks>
/// 提供订单相关的 REST API 接口，包括订单创建、查询、取消、发货、确认收货等功能。
/// 路由前缀：/api/app/mall/order
/// 认证方式：MemberJwt（会员身份）
/// </remarks>
[ApiController]
[Route("api/app/mall/order")]
[Authorize(AuthenticationSchemes = "MemberJwt")]
public class OrderController : BaseController
{
    private readonly IOrderService _orderService;

    /// <summary>
    /// 构造函数
    /// </summary>
    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    /// <summary>
    /// 从购物车创建订单
    /// </summary>
    /// <param name="dto">创建订单参数</param>
    /// <returns>订单创建结果</returns>
    /// <remarks>
    /// 从购物车创建订单，需要提供购物车项ID列表、收货信息等。
    /// 创建成功后会自动扣减库存并清空购物车。
    /// </remarks>
    [HttpPost]
    public async Task<ApiResponse<CreateOrderResultDto>> CreateFromCart([FromBody] CreateOrderFromCartDto dto)
    {
        var memberId = GetCurrentUserId().ToString();
        var result = await _orderService.CreateOrderFromCartAsync(memberId, dto);
        return Success(result);
    }

    /// <summary>
    /// 直接购买创建订单
    /// </summary>
    /// <param name="dto">创建订单参数</param>
    /// <returns>订单创建结果</returns>
    /// <remarks>
    /// 直接购买创建订单，适用于商品详情页立即购买场景。
    /// 需要提供商品SKU ID、购买数量、收货信息等。
    /// </remarks>
    [HttpPost("direct")]
    public async Task<ApiResponse<CreateOrderResultDto>> CreateDirect([FromBody] CreateOrderDirectDto dto)
    {
        var memberId = GetCurrentUserId().ToString();
        var result = await _orderService.CreateOrderDirectAsync(memberId, dto);
        return Success(result);
    }

    /// <summary>
    /// 获取会员订单列表
    /// </summary>
    /// <param name="status">订单状态（可选）</param>
    /// <param name="pageIndex">页码，默认 1</param>
    /// <param name="pageSize">每页数量，默认 10</param>
    /// <returns>订单分页列表</returns>
    /// <remarks>
    /// 查询当前会员的订单列表，支持按状态筛选。
    /// 状态值：0=待付款，1=待发货，2=待收货，3=已完成，4=已取消，5=退款中，6=已退款
    /// </remarks>
    [HttpGet("list")]
    public async Task<ApiResponse<PageResponse<OrderListDto>>> GetMemberOrderList(
        [FromQuery] string? status = null,
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 10)
    {
        var memberId = GetCurrentUserId().ToString();
        var result = await _orderService.GetMemberOrderListAsync(memberId, status, pageIndex, pageSize);
        return Success(result);
    }

    /// <summary>
    /// 获取订单详情
    /// </summary>
    /// <param name="id">订单ID</param>
    /// <returns>订单详情</returns>
    /// <remarks>
    /// 查询订单详细信息，包含订单项、商品信息等。
    /// 只能查询自己的订单。
    /// </remarks>
    [HttpGet("{id}")]
    public async Task<ApiResponse<OrderDetailDto>> GetDetail(string id)
    {
        var result = await _orderService.GetOrderDetailAsync(id);
        // TODO: 验证是否是当前会员的订单
        return Success(result);
    }

    /// <summary>
    /// 取消订单
    /// </summary>
    /// <param name="id">订单ID</param>
    /// <param name="dto">取消参数</param>
    /// <returns>是否成功</returns>
    /// <remarks>
    /// 会员取消订单，只有待付款和待发货状态的订单可以取消。
    /// 取消成功后会恢复库存。
    /// </remarks>
    [HttpPost("{id}/cancel")]
    public async Task<ApiResponse<bool>> Cancel(string id, [FromBody] OrderCancelDto dto)
    {
        var memberId = GetCurrentUserId().ToString();
        // TODO: 验证是否是当前会员的订单
        var result = await _orderService.CancelOrderAsync(id, dto, memberId);
        return Success(result);
    }

    /// <summary>
    /// 确认收货
    /// </summary>
    /// <param name="id">订单ID</param>
    /// <returns>是否成功</returns>
    /// <remarks>
    /// 会员确认收货，订单状态会从待收货变更为已完成。
    /// 只有待收货状态的订单可以确认收货。
    /// </remarks>
    [HttpPost("{id}/confirm")]
    public async Task<ApiResponse<bool>> ConfirmReceive(string id)
    {
        var memberId = GetCurrentUserId().ToString();
        // TODO: 验证是否是当前会员的订单
        var result = await _orderService.ConfirmReceiveAsync(id);
        return Success(result);
    }

    /// <summary>
    /// 获取订单统计数据
    /// </summary>
    /// <returns>订单统计数据</returns>
    /// <remarks>
    /// 获取当前会员的订单统计信息，包括各状态的订单数量。
    /// </remarks>
    [HttpGet("statistics")]
    public async Task<ApiResponse<OrderStatisticsDto>> GetStatistics()
    {
        var memberId = GetCurrentUserId().ToString();
        var result = await _orderService.GetOrderStatisticsAsync(memberId);
        return Success(result);
    }
}