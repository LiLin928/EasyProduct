using EasyProduct.Common.Base;
using EasyProduct.Common.Error;
using EasyProduct.Models.Dto.Mall.Refund;
using EasyProduct.Models.Dto.Mall.Order;
using EasyProduct.Models.Entitys.Mall;
using EasyProduct.Models.Enums.Mall;
using Mapster;
using SqlSugar;
using Microsoft.Extensions.Logging;

namespace EasyProduct.Business.Mall;

/// <summary>
/// 退款服务实现类
/// </summary>
/// <remarks>
/// 提供退款申请、审核、查询、物流填写、退款执行等核心业务功能的实现。
/// 退款流程：
/// 1. 会员申请退款
///    - 验证订单状态和订单项
///    - 验证退款金额和数量
///    - 创建退款单
///    - 更新订单状态为退款中
/// 2. 管理员审核退款
///    - 审核通过：进入下一步（退货或退款）
///    - 审核拒绝：退款结束
/// 3. 填写物流信息（退货退款类型）
/// 4. 执行退款
///    - 调用支付平台退款接口
///    - 更新退款状态
///    - 更新订单状态
///    - 恢复库存（如果需要）
/// </remarks>
public class RefundService : BaseService, IRefundService
{
    private readonly IOrderService _orderService;
    private readonly IPaymentService _paymentService;
    private readonly Product.ISkuService _skuService;
    private readonly ILogger<RefundService> _logger;

    /// <summary>
    /// 构造函数
    /// </summary>
    public RefundService(
        IOrderService orderService,
        IPaymentService paymentService,
        Product.ISkuService skuService,
        ILogger<RefundService> logger)
    {
        _orderService = orderService;
        _paymentService = paymentService;
        _skuService = skuService;
        _logger = logger;
    }

    #region 申请退款

    /// <summary>
    /// 申请退款
    /// </summary>
    public async Task<RefundDetailDto> ApplyRefundAsync(string memberId, RefundApplyDto dto)
    {
        // 1. 查询订单信息
        var order = await _db.Queryable<Order>()
            .FirstAsync(o => o.Id.ToString() == dto.OrderId && o.MemberId == memberId && o.IsDeleted == 0);

        if (order == null)
        {
            throw new BusinessException("订单不存在", 404);
        }

        // 2. 验证订单状态
        if (order.Status != OrderStatus.PendingDelivery &&
            order.Status != OrderStatus.PendingReceive &&
            order.Status != OrderStatus.Completed)
        {
            throw new BusinessException("订单状态不允许申请退款", 400);
        }

        // 3. 查询订单项
        var orderItem = await _db.Queryable<OrderItem>()
            .FirstAsync(oi => oi.Id.ToString() == dto.OrderItemId && oi.OrderId == dto.OrderId && oi.IsDeleted == 0);

        if (orderItem == null)
        {
            throw new BusinessException("订单项不存在", 404);
        }

        // 4. 验证退款数量
        if (dto.RefundQuantity > orderItem.Quantity)
        {
            throw new BusinessException($"退款数量不能超过购买数量，购买数量：{orderItem.Quantity}", 400);
        }

        // 5. 验证退款金额
        var maxRefundAmount = orderItem.Price * dto.RefundQuantity;
        if (dto.RefundAmount > maxRefundAmount)
        {
            throw new BusinessException($"退款金额不能超过商品金额，最大退款金额：{maxRefundAmount}", 400);
        }

        // 6. 检查是否已有进行中的退款申请
        var existingRefund = await _db.Queryable<Refund>()
            .Where(r => r.OrderItemId == dto.OrderItemId && r.MemberId == memberId &&
                   r.Status != RefundStatus.Rejected && r.Status != RefundStatus.Cancelled && r.Status != RefundStatus.Completed &&
                   r.IsDeleted == 0)
            .FirstAsync();

        if (existingRefund != null)
        {
            throw new BusinessException("该订单项已有进行中的退款申请", 400);
        }

        // 7. 创建退款单
        var refundNo = await GenerateRefundNoAsync();
        var refund = new Refund
        {
            RefundNo = refundNo,
            OrderId = dto.OrderId,
            OrderItemId = dto.OrderItemId,
            MemberId = memberId,
            RefundType = dto.RefundType,
            RefundAmount = dto.RefundAmount,
            RefundQuantity = dto.RefundQuantity,
            RefundReason = dto.RefundReason,
            Status = RefundStatus.Pending,
            AuditStatus = AuditStatus.Pending
        };

        // 8. 开启事务
        var result = await _db.Ado.UseTranAsync(async () =>
        {
            // 保存退款单
            await _db.Insertable(refund).ExecuteCommandAsync();

            // 更新订单状态为退款中
            await _orderService.UpdateOrderStatusAsync(dto.OrderId, OrderStatus.Refunding.ToString(), "会员申请退款");
        });

        if (!result.IsSuccess)
        {
            _logger.LogError("退款申请失败：{ErrorMessage}", result.ErrorMessage);
            throw new BusinessException($"退款申请失败：{result.ErrorMessage}", 400);
        }

        _logger.LogInformation("退款申请成功：退款单号={RefundNo}，订单号={OrderNo}，金额={Amount}",
            refundNo, order.OrderNo, dto.RefundAmount);

        // 返回退款详情
        return await GetRefundDetailAsync(refund.Id.ToString());
    }

    #endregion

    #region 查询退款

    /// <summary>
    /// 分页查询退款列表
    /// </summary>
    public async Task<PageResponse<RefundListDto>> GetRefundListAsync(RefundQuery query)
    {
        var queryable = _db.Queryable<Refund>()
            .LeftJoin<Order>((r, o) => r.OrderId == o.Id.ToString())
            .Where((r, o) => r.IsDeleted == 0)
            .WhereIF(!string.IsNullOrEmpty(query.RefundNo), (r, o) => r.RefundNo.Contains(query.RefundNo!))
            .WhereIF(!string.IsNullOrEmpty(query.OrderNo), (r, o) => o.OrderNo.Contains(query.OrderNo!))
            .WhereIF(query.Status.HasValue, (r, o) => (int)r.Status == query.Status.Value)
            .WhereIF(query.StartTime.HasValue, (r, o) => r.CreatedAt >= query.StartTime)
            .WhereIF(query.EndTime.HasValue, (r, o) => r.CreatedAt <= query.EndTime)
            .OrderBy((r, o) => r.CreatedAt, OrderByType.Desc)
            .Select((r, o) => new RefundListDto
            {
                Id = r.Id.ToString(),
                RefundNo = r.RefundNo,
                OrderNo = o.OrderNo,
                RefundType = r.RefundType,
                RefundTypeText = r.RefundType == RefundType.RefundOnly ? "仅退款" : "退货退款",
                RefundAmount = r.RefundAmount,
                Status = r.Status,
                StatusText = GetStatusText(r.Status),
                CreateTime = r.CreatedAt
            });

        RefAsync<int> totalCount = 0;
        var refunds = await queryable.ToPageListAsync(query.PageIndex, query.PageSize, totalCount);

        return new PageResponse<RefundListDto>
        {
            List = refunds,
            Total = totalCount.Value,
            PageIndex = query.PageIndex,
            PageSize = query.PageSize
        };
    }

    /// <summary>
    /// 获取退款详情
    /// </summary>
    public async Task<RefundDetailDto> GetRefundDetailAsync(string refundId)
    {
        var refund = await _db.Queryable<Refund>()
            .FirstAsync(r => r.Id.ToString() == refundId && r.IsDeleted == 0);

        if (refund == null)
        {
            throw new BusinessException("退款单不存在", 404);
        }

        // 查询订单信息
        var order = await _db.Queryable<Order>()
            .FirstAsync(o => o.Id.ToString() == refund.OrderId && o.IsDeleted == 0);

        // 查询订单项信息
        var orderItem = await _db.Queryable<OrderItem>()
            .FirstAsync(oi => oi.Id.ToString() == refund.OrderItemId && oi.IsDeleted == 0);

        var dto = new RefundDetailDto
        {
            Id = refund.Id.ToString(),
            RefundNo = refund.RefundNo,
            OrderNo = order?.OrderNo ?? "",
            RefundType = refund.RefundType,
            RefundTypeText = refund.RefundType == RefundType.RefundOnly ? "仅退款" : "退货退款",
            RefundQuantity = refund.RefundQuantity,
            RefundAmount = refund.RefundAmount,
            RefundReason = refund.RefundReason,
            Status = refund.Status,
            StatusText = GetStatusText(refund.Status),
            AuditRemark = refund.AuditRemark,
            LogisticsCompany = refund.LogisticsCompany,
            LogisticsNo = refund.LogisticsNo,
            OrderItem = orderItem?.Adapt<OrderItemDto>(),
            CreateTime = refund.CreatedAt
        };

        return dto;
    }

    /// <summary>
    /// 根据退款单号获取退款详情
    /// </summary>
    public async Task<RefundDetailDto> GetRefundDetailByNoAsync(string refundNo)
    {
        var refund = await _db.Queryable<Refund>()
            .FirstAsync(r => r.RefundNo == refundNo && r.IsDeleted == 0);

        if (refund == null)
        {
            throw new BusinessException("退款单不存在", 404);
        }

        return await GetRefundDetailAsync(refund.Id.ToString());
    }

    /// <summary>
    /// 获取会员退款列表
    /// </summary>
    public async Task<PageResponse<RefundListDto>> GetMemberRefundListAsync(string memberId, int? status = null, int pageIndex = 1, int pageSize = 10)
    {
        var query = new RefundQuery
        {
            Status = status,
            PageIndex = pageIndex,
            PageSize = pageSize
        };

        var queryable = _db.Queryable<Refund>()
            .LeftJoin<Order>((r, o) => r.OrderId == o.Id.ToString())
            .Where((r, o) => r.MemberId == memberId && r.IsDeleted == 0)
            .WhereIF(!string.IsNullOrEmpty(query.RefundNo), (r, o) => r.RefundNo.Contains(query.RefundNo!))
            .WhereIF(!string.IsNullOrEmpty(query.OrderNo), (r, o) => o.OrderNo.Contains(query.OrderNo!))
            .WhereIF(query.Status.HasValue, (r, o) => (int)r.Status == query.Status.Value)
            .WhereIF(query.StartTime.HasValue, (r, o) => r.CreatedAt >= query.StartTime)
            .WhereIF(query.EndTime.HasValue, (r, o) => r.CreatedAt <= query.EndTime)
            .OrderBy((r, o) => r.CreatedAt, OrderByType.Desc)
            .Select((r, o) => new RefundListDto
            {
                Id = r.Id.ToString(),
                RefundNo = r.RefundNo,
                OrderNo = o.OrderNo,
                RefundType = r.RefundType,
                RefundTypeText = r.RefundType == RefundType.RefundOnly ? "仅退款" : "退货退款",
                RefundAmount = r.RefundAmount,
                Status = r.Status,
                StatusText = GetStatusText(r.Status),
                CreateTime = r.CreatedAt
            });

        RefAsync<int> totalCount = 0;
        var refunds = await queryable.ToPageListAsync(query.PageIndex, query.PageSize, totalCount);

        return new PageResponse<RefundListDto>
        {
            List = refunds,
            Total = totalCount.Value,
            PageIndex = query.PageIndex,
            PageSize = query.PageSize
        };
    }

    /// <summary>
    /// 根据订单ID获取退款列表
    /// </summary>
    public async Task<List<RefundListDto>> GetRefundListByOrderAsync(string orderId)
    {
        var refunds = await _db.Queryable<Refund>()
            .LeftJoin<Order>((r, o) => r.OrderId == o.Id.ToString())
            .Where((r, o) => r.OrderId == orderId && r.IsDeleted == 0)
            .OrderBy((r, o) => r.CreatedAt, OrderByType.Desc)
            .Select((r, o) => new RefundListDto
            {
                Id = r.Id.ToString(),
                RefundNo = r.RefundNo,
                OrderNo = o.OrderNo,
                RefundType = r.RefundType,
                RefundTypeText = r.RefundType == RefundType.RefundOnly ? "仅退款" : "退货退款",
                RefundAmount = r.RefundAmount,
                Status = r.Status,
                StatusText = GetStatusText(r.Status),
                CreateTime = r.CreatedAt
            })
            .ToListAsync();

        return refunds;
    }

    #endregion

    #region 退款审核

    /// <summary>
    /// 审核退款
    /// </summary>
    public async Task<bool> AuditRefundAsync(string refundId, RefundAuditDto dto)
    {
        var refund = await _db.Queryable<Refund>()
            .FirstAsync(r => r.Id.ToString() == refundId && r.IsDeleted == 0);

        if (refund == null)
        {
            throw new BusinessException("退款单不存在", 404);
        }

        // 验证退款状态
        if (refund.Status != RefundStatus.Pending)
        {
            throw new BusinessException("退款单状态不允许审核", 400);
        }

        // 更新退款状态
        if (dto.Approved)
        {
            // 审核通过
            if (refund.RefundType == RefundType.RefundOnly)
            {
                // 仅退款：直接进入退款中状态
                refund.Status = RefundStatus.Refunding;
            }
            else
            {
                // 退货退款：进入退货中状态，等待用户填写物流信息
                refund.Status = RefundStatus.Returning;
            }

            refund.AuditStatus = AuditStatus.Approved;
        }
        else
        {
            // 审核拒绝
            refund.Status = RefundStatus.Rejected;
            refund.AuditStatus = AuditStatus.Rejected;

            // 更新订单状态
            var order = await _db.Queryable<Order>()
                .FirstAsync(o => o.Id.ToString() == refund.OrderId && o.IsDeleted == 0);

            if (order != null && order.Status == OrderStatus.Refunding)
            {
                await _orderService.UpdateOrderStatusAsync(refund.OrderId, order.PaymentTime.HasValue ?
                    OrderStatus.PendingDelivery.ToString() : OrderStatus.PendingPayment.ToString(), "退款申请被拒绝");
            }
        }

        refund.AuditRemark = dto.AuditRemark;
        refund.AuditTime = DateTime.Now;
        refund.UpdatedAt = DateTime.Now;

        var success = await _db.Updateable(refund).ExecuteCommandAsync() > 0;

        if (success)
        {
            _logger.LogInformation("退款审核成功：退款单号={RefundNo}，审核结果={Approved}",
                refund.RefundNo, dto.Approved ? "通过" : "拒绝");
        }

        return success;
    }

    #endregion

    #region 物流信息

    /// <summary>
    /// 填写退货物流信息
    /// </summary>
    public async Task<bool> FillLogisticsAsync(string refundId, string memberId, RefundLogisticsDto dto)
    {
        var refund = await _db.Queryable<Refund>()
            .FirstAsync(r => r.Id.ToString() == refundId && r.MemberId == memberId && r.IsDeleted == 0);

        if (refund == null)
        {
            throw new BusinessException("退款单不存在", 404);
        }

        // 验证退款状态
        if (refund.Status != RefundStatus.Returning)
        {
            throw new BusinessException("退款单状态不允许填写物流信息", 400);
        }

        // 更新物流信息
        refund.LogisticsCompany = dto.LogisticsCompany;
        refund.LogisticsNo = dto.LogisticsNo;
        refund.Status = RefundStatus.Refunding; // 进入退款中状态
        refund.UpdatedAt = DateTime.Now;

        var success = await _db.Updateable(refund).ExecuteCommandAsync() > 0;

        if (success)
        {
            _logger.LogInformation("退货物流信息填写成功：退款单号={RefundNo}，物流公司={LogisticsCompany}，物流单号={LogisticsNo}",
                refund.RefundNo, dto.LogisticsCompany, dto.LogisticsNo);
        }

        return success;
    }

    #endregion

    #region 退款执行

    /// <summary>
    /// 执行退款
    /// </summary>
    public async Task<bool> ProcessRefundAsync(string refundId)
    {
        var refund = await _db.Queryable<Refund>()
            .FirstAsync(r => r.Id.ToString() == refundId && r.IsDeleted == 0);

        if (refund == null)
        {
            throw new BusinessException("退款单不存在", 404);
        }

        // 验证退款状态
        if (refund.Status != RefundStatus.Refunding)
        {
            throw new BusinessException("退款单状态不允许执行退款", 400);
        }

        // 查询订单信息
        var order = await _db.Queryable<Order>()
            .FirstAsync(o => o.Id.ToString() == refund.OrderId && o.IsDeleted == 0);

        if (order == null)
        {
            throw new BusinessException("订单不存在", 404);
        }

        // 查询订单项信息
        var orderItem = await _db.Queryable<OrderItem>()
            .FirstAsync(oi => oi.Id.ToString() == refund.OrderItemId && oi.IsDeleted == 0);

        // 开启事务
        var result = await _db.Ado.UseTranAsync(async () =>
        {
            // TODO: 调用支付平台退款接口（模拟）
            // 实际项目中需要调用真实的支付平台退款接口
            _logger.LogInformation("调用支付平台退款接口：退款单号={RefundNo}，金额={Amount}",
                refund.RefundNo, refund.RefundAmount);

            // 更新退款单状态
            refund.Status = RefundStatus.Completed;
            refund.RefundTime = DateTime.Now;
            refund.UpdatedAt = DateTime.Now;

            await _db.Updateable(refund).ExecuteCommandAsync();

            // 更新订单项退款标记
            if (orderItem != null)
            {
                orderItem.IsRefunded = 1;
                orderItem.RefundQuantity += refund.RefundQuantity;
                orderItem.UpdatedAt = DateTime.Now;

                await _db.Updateable(orderItem).ExecuteCommandAsync();
            }

            // 检查订单是否全部退款
            var allOrderItems = await _db.Queryable<OrderItem>()
                .Where(oi => oi.OrderId == refund.OrderId && oi.IsDeleted == 0)
                .ToListAsync();

            var allRefunded = allOrderItems.All(oi => oi.IsRefunded == 1 && oi.RefundQuantity == oi.Quantity);

            // 更新订单状态
            if (allRefunded)
            {
                await _orderService.UpdateOrderStatusAsync(refund.OrderId, OrderStatus.Refunded.ToString(), "订单全部退款完成");
            }
            else
            {
                // 部分退款，订单状态保持不变或恢复到之前的状态
                _logger.LogInformation("订单部分退款：订单号={OrderNo}", order.OrderNo);
            }

            // 恢复库存（如果需要）
            if (orderItem != null)
            {
                var sku = await _skuService.GetSkuByIdAsync(orderItem.SkuId);
                if (sku != null)
                {
                    var newStock = sku.Stock + refund.RefundQuantity;
                    await _skuService.UpdateSkuStockAsync(orderItem.SkuId, newStock);
                    _logger.LogInformation("库存恢复：SKU ID={SkuId}，恢复数量={Quantity}，新库存={NewStock}",
                        orderItem.SkuId, refund.RefundQuantity, newStock);
                }
            }
        });

        if (!result.IsSuccess)
        {
            _logger.LogError("退款执行失败：{ErrorMessage}", result.ErrorMessage);
            throw new BusinessException($"退款执行失败：{result.ErrorMessage}", 400);
        }

        _logger.LogInformation("退款执行成功：退款单号={RefundNo}", refund.RefundNo);
        return true;
    }

    /// <summary>
    /// 取消退款申请
    /// </summary>
    public async Task<bool> CancelRefundAsync(string refundId, string memberId)
    {
        var refund = await _db.Queryable<Refund>()
            .FirstAsync(r => r.Id.ToString() == refundId && r.MemberId == memberId && r.IsDeleted == 0);

        if (refund == null)
        {
            throw new BusinessException("退款单不存在", 404);
        }

        // 验证退款状态
        if (refund.Status != RefundStatus.Pending && refund.Status != RefundStatus.Returning)
        {
            throw new BusinessException("退款单状态不允许取消", 400);
        }

        // 更新退款单状态
        refund.Status = RefundStatus.Cancelled;
        refund.UpdatedAt = DateTime.Now;

        var success = await _db.Updateable(refund).ExecuteCommandAsync() > 0;

        if (success)
        {
            // 更新订单状态
            var order = await _db.Queryable<Order>()
                .FirstAsync(o => o.Id.ToString() == refund.OrderId && o.IsDeleted == 0);

            if (order != null && order.Status == OrderStatus.Refunding)
            {
                await _orderService.UpdateOrderStatusAsync(refund.OrderId, order.PaymentTime.HasValue ?
                    OrderStatus.PendingDelivery.ToString() : OrderStatus.PendingPayment.ToString(), "退款申请已取消");
            }

            _logger.LogInformation("退款申请取消成功：退款单号={RefundNo}", refund.RefundNo);
        }

        return success;
    }

    #endregion

    #region 私有方法

    /// <summary>
    /// 生成退款单号
    /// </summary>
    private async Task<string> GenerateRefundNoAsync()
    {
        // 查询今天的退款单数量
        var today = DateTime.Today;
        var count = await _db.Queryable<Refund>()
            .Where(r => r.CreatedAt >= today && r.IsDeleted == 0)
            .CountAsync();

        // 格式：RF + 年月日时分秒 + 6位序号
        var refundNo = $"RF{DateTime.Now:yyyyMMddHHmmss}{(count + 1):D6}";

        return refundNo;
    }

    /// <summary>
    /// 获取状态文本
    /// </summary>
    private static string GetStatusText(RefundStatus status)
    {
        return status switch
        {
            RefundStatus.Pending => "待审核",
            RefundStatus.Approved => "已通过",
            RefundStatus.Rejected => "已拒绝",
            RefundStatus.Returning => "退货中",
            RefundStatus.Refunding => "退款中",
            RefundStatus.Completed => "已完成",
            RefundStatus.Cancelled => "已取消",
            _ => "未知状态"
        };
    }

    #endregion
}