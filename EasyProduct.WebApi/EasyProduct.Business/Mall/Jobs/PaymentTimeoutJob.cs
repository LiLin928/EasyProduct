using Quartz;
using EasyProduct.Models.Dto.Mall.Payment;
using Microsoft.Extensions.Logging;

namespace EasyProduct.Business.Mall.Jobs;

/// <summary>
/// 支付超时检查任务
/// </summary>
/// <remarks>
/// 定时检查超过指定时间未支付的支付单，并关闭这些支付单及对应的订单。
/// 执行频率：每 5 分钟执行一次。
/// 超时时间：30 分钟（可通过参数调整）。
/// </remarks>
public class PaymentTimeoutJob : IJob
{
    private readonly IPaymentService _paymentService;
    private readonly IWxPayService _wxPayService;
    private readonly IOrderService _orderService;
    private readonly ILogger<PaymentTimeoutJob> _logger;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="paymentService">支付服务</param>
    /// <param name="wxPayService">微信支付服务</param>
    /// <param name="orderService">订单服务</param>
    /// <param name="logger">日志记录器</param>
    public PaymentTimeoutJob(
        IPaymentService paymentService,
        IWxPayService wxPayService,
        IOrderService orderService,
        ILogger<PaymentTimeoutJob> logger)
    {
        _paymentService = paymentService;
        _wxPayService = wxPayService;
        _orderService = orderService;
        _logger = logger;
    }

    /// <summary>
    /// 执行任务
    /// </summary>
    /// <param name="context">任务执行上下文</param>
    /// <remarks>
    /// 1. 查询超过 30 分钟未支付的支付单
    /// 2. 逐个关闭超时支付单
    /// 3. 更新对应的订单状态为已取消
    /// 4. 记录处理结果日志
    /// </remarks>
    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("===== 开始检查超时支付单 =====");

        try
        {
            // 1. 查询超过 30 分钟未支付的支付单
            var timeoutPayments = await _paymentService.GetTimeoutPaymentsAsync(30);

            if (timeoutPayments.Count == 0)
            {
                _logger.LogInformation("没有超时支付单，检查完成");
                return;
            }

            _logger.LogInformation("发现 {Count} 个超时支付单，开始处理", timeoutPayments.Count);

            // 2. 关闭超时支付单
            var successCount = 0;
            var failCount = 0;

            foreach (var payment in timeoutPayments)
            {
                try
                {
                    // 关闭微信支付订单
                    var closed = await _wxPayService.CloseOrderAsync(payment.Id);

                    if (closed)
                    {
                        // 更新订单状态为已取消
                        await _orderService.UpdateOrderStatusAsync(
                            payment.OrderId,
                            "cancelled",
                            "支付超时，系统自动取消订单"
                        );

                        successCount++;
                        _logger.LogInformation("支付单关闭成功：支付单ID={PaymentId}，订单ID={OrderId}",
                            payment.Id, payment.OrderId);
                    }
                    else
                    {
                        failCount++;
                        _logger.LogWarning("支付单关闭失败：支付单ID={PaymentId}", payment.Id);
                    }
                }
                catch (Exception ex)
                {
                    failCount++;
                    _logger.LogError(ex, "关闭支付单异常：支付单ID={PaymentId}", payment.Id);
                }
            }

            _logger.LogInformation("===== 检查超时支付单完成 ===== 成功关闭 {SuccessCount} 个，失败 {FailCount} 个",
                successCount, failCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "检查超时支付单任务执行异常");
        }
    }
}