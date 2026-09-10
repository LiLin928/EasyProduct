using EasyProduct.Common.Base;
using EasyProduct.Models.Dto.Mall.Payment;
using EasyProduct.Business.Mall;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyProduct.Web.Controllers.App.Mall;

/// <summary>
/// 支付控制器
/// </summary>
/// <remarks>
/// 提供支付相关的 REST API 接口，包括支付单创建、查询、回调处理等功能。
/// 路由前缀：/api/app/mall/payment
/// 认证方式：MemberJwt（会员身份）/ 匿名（支付回调）
/// </remarks>
[ApiController]
[Route("api/app/mall/payment")]
public class PaymentController : BaseController
{
    private readonly IPaymentService _paymentService;

    /// <summary>
    /// 构造函数
    /// </summary>
    public PaymentController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    /// <summary>
    /// 创建支付单
    /// </summary>
    /// <param name="dto">创建支付单参数</param>
    /// <returns>支付单信息</returns>
    /// <remarks>
    /// 创建支付单，需要提供订单ID和支付方式。
    /// 支付方式：WechatPay=微信支付，Balance=余额支付。
    /// 支付渠道：jsapi=小程序支付，h5=H5支付，native=扫码支付，app=APP支付。
    /// </remarks>
    [HttpPost]
    [Authorize(AuthenticationSchemes = "MemberJwt")]
    public async Task<ApiResponse<PaymentDto>> Create([FromBody] CreatePaymentDto dto)
    {
        var memberId = GetCurrentUserId().ToString();
        var result = await _paymentService.CreatePaymentAsync(memberId, dto);
        return Success(result);
    }

    /// <summary>
    /// 创建微信支付单
    /// </summary>
    /// <param name="orderId">订单ID</param>
    /// <param name="channel">支付渠道，默认 jsapi</param>
    /// <returns>微信支付参数</returns>
    /// <remarks>
    /// 创建微信支付单，返回微信支付所需的参数。
    /// 支付渠道：jsapi=小程序支付，h5=H5支付，native=扫码支付，app=APP支付。
    /// </remarks>
    [HttpPost("wechat/{orderId}")]
    [Authorize(AuthenticationSchemes = "MemberJwt")]
    public async Task<ApiResponse<PaymentWechatResultDto>> CreateWechatPayment(
        string orderId,
        [FromQuery] string channel = "jsapi")
    {
        var memberId = GetCurrentUserId().ToString();
        var result = await _paymentService.CreateWechatPaymentAsync(memberId, orderId, channel);
        return Success(result);
    }

    /// <summary>
    /// 获取会员支付单列表
    /// </summary>
    /// <param name="pageIndex">页码，默认 1</param>
    /// <param name="pageSize">每页数量，默认 10</param>
    /// <returns>支付单分页列表</returns>
    /// <remarks>
    /// 查询当前会员的支付单列表。
    /// </remarks>
    [HttpGet("list")]
    [Authorize(AuthenticationSchemes = "MemberJwt")]
    public async Task<ApiResponse<PageResponse<PaymentDto>>> GetMemberPaymentList(
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 10)
    {
        var memberId = GetCurrentUserId().ToString();
        var query = new PaymentQuery
        {
            MemberId = memberId,
            PageIndex = pageIndex,
            PageSize = pageSize
        };
        var result = await _paymentService.GetPaymentListAsync(query);
        return Success(result);
    }

    /// <summary>
    /// 获取支付单详情
    /// </summary>
    /// <param name="id">支付单ID</param>
    /// <returns>支付单详情</returns>
    /// <remarks>
    /// 查询支付单详细信息。
    /// </remarks>
    [HttpGet("{id}")]
    [Authorize(AuthenticationSchemes = "MemberJwt")]
    public async Task<ApiResponse<PaymentDto>> GetDetail(string id)
    {
        var result = await _paymentService.GetPaymentDetailAsync(id);
        return Success(result);
    }

    /// <summary>
    /// 查询支付状态
    /// </summary>
    /// <param name="id">支付单ID</param>
    /// <returns>支付单信息</returns>
    /// <remarks>
    /// 查询支付状态，会主动同步第三方支付状态。
    /// </remarks>
    [HttpGet("{id}/status")]
    [Authorize(AuthenticationSchemes = "MemberJwt")]
    public async Task<ApiResponse<PaymentDto>> QueryStatus(string id)
    {
        var result = await _paymentService.QueryPaymentStatusAsync(id);
        return Success(result);
    }

    /// <summary>
    /// 微信支付回调
    /// </summary>
    /// <returns>回调响应</returns>
    /// <remarks>
    /// 接收微信支付回调通知，更新支付状态和订单状态。
    /// 该接口为匿名访问，由微信服务器调用。
    /// </remarks>
    [HttpPost("callback/wechat")]
    [AllowAnonymous]
    public async Task<IActionResult> WechatCallback()
    {
        // 读取请求体
        using var reader = new StreamReader(Request.Body);
        var callbackData = await reader.ReadToEndAsync();

        var result = await _paymentService.HandleWechatPaymentCallbackAsync(callbackData);

        // 返回 XML 格式响应
        return Content(result, "application/xml");
    }

    /// <summary>
    /// 关闭支付单
    /// </summary>
    /// <param name="id">支付单ID</param>
    /// <returns>是否成功</returns>
    /// <remarks>
    /// 关闭未支付的支付单，已支付的支付单不能关闭。
    /// </remarks>
    [HttpPost("{id}/close")]
    [Authorize(AuthenticationSchemes = "MemberJwt")]
    public async Task<ApiResponse<bool>> Close(string id)
    {
        var result = await _paymentService.ClosePaymentAsync(id);
        return Success(result);
    }
}