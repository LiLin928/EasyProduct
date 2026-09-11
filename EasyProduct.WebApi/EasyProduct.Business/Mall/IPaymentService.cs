using EasyProduct.Common.Base;
using EasyProduct.Models.Dto.Mall.Payment;

namespace EasyProduct.Business.Mall;

/// <summary>
/// 支付服务接口
/// </summary>
/// <remarks>
/// 提供支付单创建、查询、回调处理、状态查询等核心业务功能。
/// 支付流程：创建支付单 → 调用第三方支付 → 接收回调 → 更新状态。
/// 支持微信支付、余额支付等多种支付方式。
/// </remarks>
public interface IPaymentService
{
    #region 创建支付单

    /// <summary>
    /// 创建支付单
    /// </summary>
    /// <param name="memberId">会员ID</param>
    /// <param name="dto">创建支付单参数</param>
    /// <returns>支付单信息</returns>
    Task<PaymentDto> CreatePaymentAsync(string memberId, CreatePaymentDto dto);

    /// <summary>
    /// 创建微信支付单
    /// </summary>
    /// <param name="memberId">会员ID</param>
    /// <param name="orderId">订单ID</param>
    /// <param name="channel">支付渠道（jsapi/h5/native/app）</param>
    /// <returns>微信支付参数</returns>
    Task<PaymentWechatResultDto> CreateWechatPaymentAsync(string memberId, string orderId, string channel = "jsapi");

    #endregion

    #region 查询支付单

    /// <summary>
    /// 分页查询支付单列表
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>支付单分页列表</returns>
    Task<PageResponse<PaymentDto>> GetPaymentListAsync(PaymentQuery query);

    /// <summary>
    /// 获取支付单详情
    /// </summary>
    /// <param name="paymentId">支付单ID</param>
    /// <returns>支付单详情</returns>
    Task<PaymentDto> GetPaymentDetailAsync(string paymentId);

    /// <summary>
    /// 根据支付单号获取支付单详情
    /// </summary>
    /// <param name="paymentNo">支付单号</param>
    /// <returns>支付单详情</returns>
    Task<PaymentDto> GetPaymentDetailByNoAsync(string paymentNo);

    /// <summary>
    /// 根据订单ID获取支付单列表
    /// </summary>
    /// <param name="orderId">订单ID</param>
    /// <returns>支付单列表</returns>
    Task<List<PaymentDto>> GetPaymentListByOrderAsync(string orderId);

    #endregion

    #region 支付回调

    /// <summary>
    /// 处理支付回调
    /// </summary>
    /// <param name="dto">支付回调参数</param>
    /// <returns>是否成功</returns>
    Task<bool> HandlePaymentCallbackAsync(PaymentCallbackDto dto);

    /// <summary>
    /// 微信支付回调处理
    /// </summary>
    /// <param name="callbackData">回调数据（JSON）</param>
    /// <returns>回调响应</returns>
    Task<string> HandleWechatPaymentCallbackAsync(string callbackData);

    #endregion

    #region 支付状态查询

    /// <summary>
    /// 查询支付状态
    /// </summary>
    /// <param name="paymentId">支付单ID</param>
    /// <returns>支付单信息</returns>
    Task<PaymentDto> QueryPaymentStatusAsync(string paymentId);

    /// <summary>
    /// 同步支付状态（从第三方平台）
    /// </summary>
    /// <param name="paymentNo">支付单号</param>
    /// <returns>是否成功</returns>
    Task<bool> SyncPaymentStatusAsync(string paymentNo);

    #endregion

    #region 支付关闭

    /// <summary>
    /// 关闭支付单
    /// </summary>
    /// <param name="paymentId">支付单ID</param>
    /// <returns>是否成功</returns>
    Task<bool> ClosePaymentAsync(string paymentId);

    #endregion

    #region 支付超时处理

    /// <summary>
    /// 获取超时未支付的支付单列表
    /// </summary>
    /// <param name="timeoutMinutes">超时时间（分钟）</param>
    /// <returns>超时支付单列表</returns>
    /// <remarks>
    /// 查询超过指定时间仍未支付的支付单（状态为 pending），用于定时任务关闭超时订单。
    /// </remarks>
    Task<List<PaymentDto>> GetTimeoutPaymentsAsync(int timeoutMinutes);

    #endregion
}