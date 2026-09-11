using EasyProduct.Common.Base;
using EasyProduct.Common.Error;
using EasyProduct.Models.Dto.Mall.Payment;
using EasyProduct.Models.Dto.Mall.Order;
using EasyProduct.Models.Entitys.Mall;
using EasyProduct.Models.Enums.Mall;
using Mapster;
using SqlSugar;
using Microsoft.Extensions.Logging;

namespace EasyProduct.Business.Mall;

/// <summary>
/// 支付服务实现类
/// </summary>
/// <remarks>
/// 提供支付单创建、查询、回调处理、状态查询等核心业务功能的实现。
/// 支付流程：
/// 1. 创建支付单，生成支付单号
/// 2. 调用第三方支付平台API（模拟）
/// 3. 接收支付回调，更新支付状态
/// 4. 更新订单状态为已支付
///
/// 当前为模拟实现，后续可接入真实的支付平台（微信支付、支付宝等）。
/// </remarks>
public class PaymentService : BaseService, IPaymentService
{
    private readonly IOrderService _orderService;
    private readonly ILogger<PaymentService> _logger;

    /// <summary>
    /// 构造函数
    /// </summary>
    public PaymentService(
        IOrderService orderService,
        ILogger<PaymentService> logger)
    {
        _orderService = orderService;
        _logger = logger;
    }

    #region 创建支付单

    /// <summary>
    /// 创建支付单
    /// </summary>
    /// <param name="memberId">会员ID</param>
    /// <param name="dto">创建支付单参数</param>
    /// <returns>支付单信息</returns>
    /// <exception cref="BusinessException">订单不存在、订单状态不正确、支付方式不支持</exception>
    /// <remarks>
    /// 1. 查询订单信息，验证订单状态
    /// 2. 创建支付单，生成支付单号
    /// 3. 根据支付方式调用对应的支付接口
    /// 4. 返回支付单信息
    /// </remarks>
    public async Task<PaymentDto> CreatePaymentAsync(string memberId, CreatePaymentDto dto)
    {
        // 1. 查询订单信息
        var order = await _db.Queryable<Order>()
            .FirstAsync(o => o.Id.ToString() == dto.OrderId && o.MemberId == memberId && o.IsDeleted == 0);

        if (order == null)
        {
            throw new BusinessException("订单不存在", 404);
        }

        // 2. 验证订单状态
        if (order.Status != OrderStatus.PendingPayment)
        {
            throw new BusinessException("订单状态不允许支付", 400);
        }

        // 3. 解析支付方式
        if (!Enum.TryParse<PaymentMethod>(dto.PaymentMethod, out var paymentMethod))
        {
            throw new BusinessException($"不支持的支付方式：{dto.PaymentMethod}", 400);
        }

        // 4. 创建支付单
        var paymentNo = await GeneratePaymentNoAsync();
        var payment = new Payment
        {
            PaymentNo = paymentNo,
            OrderId = order.Id.ToString(),
            MemberId = memberId,
            Amount = order.PayAmount,
            PaymentMethod = paymentMethod,
            PaymentChannel = dto.PaymentChannel ?? "jsapi",
            Status = EasyProduct.Models.Enums.Mall.PaymentStatus.Pending
        };

        // 5. 保存支付单
        await _db.Insertable(payment).ExecuteCommandAsync();

        _logger.LogInformation("支付单创建成功：支付单号={PaymentNo}，订单号={OrderNo}，金额={Amount}",
            paymentNo, order.OrderNo, payment.Amount);

        return payment.Adapt<PaymentDto>();
    }

    /// <summary>
    /// 创建微信支付单
    /// </summary>
    /// <param name="memberId">会员ID</param>
    /// <param name="orderId">订单ID</param>
    /// <param name="channel">支付渠道（jsapi/h5/native/app）</param>
    /// <returns>微信支付参数</returns>
    /// <remarks>
    /// 模拟微信支付，返回模拟的支付参数。
    /// 实际项目中需要调用微信支付API获取真实的支付参数。
    /// </remarks>
    public async Task<PaymentWechatResultDto> CreateWechatPaymentAsync(string memberId, string orderId, string channel = "jsapi")
    {
        // 创建支付单
        var createPaymentDto = new CreatePaymentDto
        {
            OrderId = orderId,
            PaymentMethod = PaymentMethod.WechatPay.ToString(),
            PaymentChannel = channel
        };

        var payment = await CreatePaymentAsync(memberId, createPaymentDto);

        // 模拟返回微信支付参数
        // 实际项目中需要：
        // 1. 调用微信支付统一下单API
        // 2. 获取预支付交易会话标识（prepay_id）
        // 3. 生成签名
        var wechatResult = new PaymentWechatResultDto
        {
            TimeStamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
            NonceStr = Guid.NewGuid().ToString("N"),
            Package = $"prepay_id=wx{DateTime.Now:yyyyMMddHHmmss}{new Random().Next(100000, 999999)}",
            SignType = "RSA",
            PaySign = "mock_pay_sign_" + Guid.NewGuid().ToString("N")
        };

        _logger.LogInformation("微信支付参数生成成功：支付单号={PaymentNo}，渠道={Channel}",
            payment.PaymentNo, channel);

        return wechatResult;
    }

    #endregion

    #region 查询支付单

    /// <summary>
    /// 分页查询支付单列表
    /// </summary>
    public async Task<PageResponse<PaymentDto>> GetPaymentListAsync(PaymentQuery query)
    {
        var queryable = _db.Queryable<Payment>()
            .WhereIF(!string.IsNullOrEmpty(query.PaymentNo), p => p.PaymentNo.Contains(query.PaymentNo!))
            .WhereIF(!string.IsNullOrEmpty(query.OrderId), p => p.OrderId == query.OrderId)
            .WhereIF(!string.IsNullOrEmpty(query.MemberId), p => p.MemberId == query.MemberId)
            .WhereIF(query.Status.HasValue, p => (int)p.Status == query.Status.Value)
            .WhereIF(query.PaymentMethod.HasValue, p => (int)p.PaymentMethod == query.PaymentMethod.Value)
            .WhereIF(query.StartTime.HasValue, p => p.CreatedAt >= query.StartTime)
            .WhereIF(query.EndTime.HasValue, p => p.CreatedAt <= query.EndTime)
            .OrderBy(p => p.CreatedAt, OrderByType.Desc);

        RefAsync<int> totalCount = 0;
        var payments = await queryable.ToPageListAsync(query.PageIndex, query.PageSize, totalCount);

        var dtos = payments.Adapt<List<PaymentDto>>();

        return new PageResponse<PaymentDto>
        {
            List = dtos,
            Total = totalCount.Value,
            PageIndex = query.PageIndex,
            PageSize = query.PageSize
        };
    }

    /// <summary>
    /// 获取支付单详情
    /// </summary>
    public async Task<PaymentDto> GetPaymentDetailAsync(string paymentId)
    {
        var payment = await _db.Queryable<Payment>()
            .FirstAsync(p => p.Id.ToString() == paymentId && p.IsDeleted == 0);

        if (payment == null)
        {
            throw new BusinessException("支付单不存在", 404);
        }

        return payment.Adapt<PaymentDto>();
    }

    /// <summary>
    /// 根据支付单号获取支付单详情
    /// </summary>
    public async Task<PaymentDto> GetPaymentDetailByNoAsync(string paymentNo)
    {
        var payment = await _db.Queryable<Payment>()
            .FirstAsync(p => p.PaymentNo == paymentNo && p.IsDeleted == 0);

        if (payment == null)
        {
            throw new BusinessException("支付单不存在", 404);
        }

        return payment.Adapt<PaymentDto>();
    }

    /// <summary>
    /// 根据订单ID获取支付单列表
    /// </summary>
    public async Task<List<PaymentDto>> GetPaymentListByOrderAsync(string orderId)
    {
        var payments = await _db.Queryable<Payment>()
            .Where(p => p.OrderId == orderId && p.IsDeleted == 0)
            .OrderBy(p => p.CreatedAt, OrderByType.Desc)
            .ToListAsync();

        return payments.Adapt<List<PaymentDto>>();
    }

    #endregion

    #region 支付回调

    /// <summary>
    /// 处理支付回调
    /// </summary>
    /// <param name="dto">支付回调参数</param>
    /// <returns>是否成功</returns>
    /// <exception cref="BusinessException">支付单不存在、支付单状态不正确</exception>
    /// <remarks>
    /// 1. 查询支付单信息
    /// 2. 验证支付单状态
    /// 3. 更新支付单状态
    /// 4. 更新订单状态为已支付
    /// </remarks>
    public async Task<bool> HandlePaymentCallbackAsync(PaymentCallbackDto dto)
    {
        // 1. 查询支付单信息
        var payment = await _db.Queryable<Payment>()
            .FirstAsync(p => p.PaymentNo == dto.PaymentNo && p.IsDeleted == 0);

        if (payment == null)
        {
            throw new BusinessException("支付单不存在", 404);
        }

        // 2. 验证支付单状态
        if (payment.Status != EasyProduct.Models.Enums.Mall.PaymentStatus.Pending)
        {
            // 支付单已处理，幂等性保证
            _logger.LogWarning("支付单已处理：支付单号={PaymentNo}，当前状态={Status}",
                dto.PaymentNo, payment.Status);
            return true;
        }

        // 3. 解析支付状态
        var paymentStatus = dto.Status.ToLower() == "success" ? EasyProduct.Models.Enums.Mall.PaymentStatus.Success : EasyProduct.Models.Enums.Mall.PaymentStatus.Failed;

        // 4. 开启事务
        var result = await _db.Ado.UseTranAsync(async () =>
        {
            // 更新支付单状态
            payment.Status = paymentStatus;
            payment.ThirdPartyNo = dto.ThirdPartyNo;
            payment.PaymentTime = dto.PaymentTime ?? DateTime.Now;
            payment.UpdatedAt = DateTime.Now;

            await _db.Updateable(payment).ExecuteCommandAsync();

            // 如果支付成功，更新订单状态
            if (paymentStatus == EasyProduct.Models.Enums.Mall.PaymentStatus.Success)
            {
                await _orderService.MarkOrderPaidAsync(payment.OrderId, payment.PaymentTime!.Value);
            }
        });

        if (!result.IsSuccess)
        {
            _logger.LogError("支付回调处理失败：{ErrorMessage}", result.ErrorMessage);
            throw new BusinessException($"支付回调处理失败：{result.ErrorMessage}", 400);
        }

        _logger.LogInformation("支付回调处理成功：支付单号={PaymentNo}，状态={Status}",
            dto.PaymentNo, paymentStatus);

        return true;
    }

    /// <summary>
    /// 微信支付回调处理
    /// </summary>
    /// <param name="callbackData">回调数据（JSON）</param>
    /// <returns>回调响应</returns>
    /// <remarks>
    /// 模拟微信支付回调处理。
    /// 实际项目中需要：
    /// 1. 验证回调签名
    /// 2. 解析回调数据
    /// 3. 更新支付状态
    /// 4. 返回响应给微信服务器
    /// </remarks>
    public async Task<string> HandleWechatPaymentCallbackAsync(string callbackData)
    {
        try
        {
            // 实际项目中需要解析微信回调数据
            // 这里模拟处理
            _logger.LogInformation("收到微信支付回调：{CallbackData}", callbackData);

            // 模拟解析回调数据
            var dto = new PaymentCallbackDto
            {
                PaymentNo = "PAY" + DateTime.Now.ToString("yyyyMMddHHmmss"),
                ThirdPartyNo = "wx" + DateTime.Now.ToString("yyyyMMddHHmmss"),
                Status = "success",
                PaymentTime = DateTime.Now,
                RawData = callbackData
            };

            // 处理回调
            await HandlePaymentCallbackAsync(dto);

            // 返回成功响应给微信服务器
            return "<xml><return_code><![CDATA[SUCCESS]]></return_code><return_msg><![CDATA[OK]]></return_msg></xml>";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "微信支付回调处理失败");
            // 返回失败响应
            return "<xml><return_code><![CDATA[FAIL]]></return_code><return_msg><![CDATA[FAIL]]></return_msg></xml>";
        }
    }

    #endregion

    #region 支付状态查询

    /// <summary>
    /// 查询支付状态
    /// </summary>
    public async Task<PaymentDto> QueryPaymentStatusAsync(string paymentId)
    {
        var payment = await GetPaymentDetailAsync(paymentId);

        // 如果支付单状态是待支付，可以主动查询第三方支付状态
        if (payment.Status == EasyProduct.Models.Enums.Mall.PaymentStatus.Pending.ToString())
        {
            await SyncPaymentStatusAsync(payment.PaymentNo);
            payment = await GetPaymentDetailAsync(paymentId);
        }

        return payment;
    }

    /// <summary>
    /// 同步支付状态（从第三方平台）
    /// </summary>
    /// <param name="paymentNo">支付单号</param>
    /// <returns>是否成功</returns>
    /// <remarks>
    /// 模拟从第三方支付平台查询支付状态。
    /// 实际项目中需要调用第三方支付平台的查询接口。
    /// </remarks>
    public async Task<bool> SyncPaymentStatusAsync(string paymentNo)
    {
        var payment = await _db.Queryable<Payment>()
            .FirstAsync(p => p.PaymentNo == paymentNo && p.IsDeleted == 0);

        if (payment == null)
        {
            throw new BusinessException("支付单不存在", 404);
        }

        // 模拟查询第三方支付状态
        // 实际项目中需要调用微信支付查询接口
        _logger.LogInformation("同步支付状态：支付单号={PaymentNo}", paymentNo);

        // 这里不做任何更新，仅记录日志
        return true;
    }

    #endregion

    #region 支付关闭

    /// <summary>
    /// 关闭支付单
    /// </summary>
    /// <param name="paymentId">支付单ID</param>
    /// <returns>是否成功</returns>
    /// <exception cref="BusinessException">支付单不存在、支付单状态不正确</exception>
    /// <remarks>
    /// 关闭未支付的支付单，已支付的支付单不能关闭。
    /// 实际项目中需要调用第三方支付平台的关闭接口。
    /// </remarks>
    public async Task<bool> ClosePaymentAsync(string paymentId)
    {
        var payment = await _db.Queryable<Payment>()
            .FirstAsync(p => p.Id.ToString() == paymentId && p.IsDeleted == 0);

        if (payment == null)
        {
            throw new BusinessException("支付单不存在", 404);
        }

        // 验证支付单状态
        if (payment.Status != EasyProduct.Models.Enums.Mall.PaymentStatus.Pending)
        {
            throw new BusinessException("支付单状态不允许关闭", 400);
        }

        // 更新支付单状态为失败
        payment.Status = EasyProduct.Models.Enums.Mall.PaymentStatus.Failed;
        payment.UpdatedAt = DateTime.Now;

        var success = await _db.Updateable(payment).ExecuteCommandAsync() > 0;

        if (success)
        {
            _logger.LogInformation("支付单关闭成功：支付单号={PaymentNo}", payment.PaymentNo);
        }

        return success;
    }

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
    public async Task<List<PaymentDto>> GetTimeoutPaymentsAsync(int timeoutMinutes)
    {
        var cutoffTime = DateTime.Now.AddMinutes(-timeoutMinutes);

        var payments = await _db.Queryable<Payment>()
            .Where(p => p.Status == EasyProduct.Models.Enums.Mall.PaymentStatus.Pending && p.CreatedAt < cutoffTime && p.IsDeleted == 0)
            .OrderBy(p => p.CreatedAt, OrderByType.Asc)
            .ToListAsync();

        _logger.LogInformation("查询到 {Count} 个超时未支付支付单（超过 {Minutes} 分钟）", payments.Count, timeoutMinutes);

        return payments.Adapt<List<PaymentDto>>();
    }

    #endregion

    #region 私有方法

    /// <summary>
    /// 生成支付单号
    /// </summary>
    /// <returns>支付单号</returns>
    /// <remarks>
    /// 格式：PAY + 年月日时分秒 + 6位序号
    /// 示例：PAY20260910152530123456
    /// </remarks>
    private async Task<string> GeneratePaymentNoAsync()
    {
        // 查询今天的支付单数量
        var today = DateTime.Today;
        var count = await _db.Queryable<Payment>()
            .Where(p => p.CreatedAt >= today && p.IsDeleted == 0)
            .CountAsync();

        // 格式：PAY + 年月日时分秒 + 6位序号
        var paymentNo = $"PAY{DateTime.Now:yyyyMMddHHmmss}{(count + 1):D6}";

        return paymentNo;
    }

    #endregion
}