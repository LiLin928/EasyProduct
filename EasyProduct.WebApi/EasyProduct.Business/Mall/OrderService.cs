using EasyProduct.Common.Base;
using EasyProduct.Common.Error;
using EasyProduct.Models.Dto.Mall.Order;
using EasyProduct.Models.Dto.Product.Sku;
using EasyProduct.Models.Entitys.Mall;
using EasyProduct.Models.Enums;
using EasyProduct.Models.Enums.Mall;
using Mapster;
using SqlSugar;
using Microsoft.Extensions.Logging;

namespace EasyProduct.Business.Mall;

/// <summary>
/// 订单服务实现类
/// </summary>
/// <remarks>
/// 提供订单的创建、查询、状态管理、取消等核心业务功能的实现。
/// 订单创建流程：
/// 1. 验证购物车商品或直接购买商品
/// 2. 验证商品库存
/// 3. 计算订单金额
/// 4. 创建订单主表和订单明细表
/// 5. 扣减库存
/// 6. 清空购物车（如果是从购物车下单）
/// </remarks>
public class OrderService : BaseService, IOrderService
{
    private readonly ICartService _cartService;
    private readonly Product.ISkuService _skuService;
    private readonly ILogger<OrderService> _logger;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <remarks>
    /// BaseService 使用属性注入 _db，无需在构造函数中注入。
    /// 其他依赖服务通过构造函数注入。
    /// </remarks>
    public OrderService(
        ICartService cartService,
        Product.ISkuService skuService,
        ILogger<OrderService> logger)
    {
        _cartService = cartService;
        _skuService = skuService;
        _logger = logger;
    }

    #region 创建订单

    /// <summary>
    /// 从购物车创建订单
    /// </summary>
    public async Task<CreateOrderResultDto> CreateOrderFromCartAsync(string memberId, CreateOrderFromCartDto dto)
    {
        // 1. 获取购物车商品列表
        var cartList = await _cartService.GetCartListAsync(memberId);
        if (cartList.Items == null || cartList.Items.Count == 0)
        {
            throw new BusinessException("购物车为空，无法创建订单", 400);
        }

        // 筛选指定的购物车项
        var cartItems = cartList.Items.Where(c => dto.CartIds.Contains(c.Id)).ToList();
        if (cartItems.Count == 0)
        {
            throw new BusinessException("未找到指定的购物车商品", 400);
        }

        // 2. 验证商品并构建订单项数据
        var orderItems = new List<OrderItem>();
        decimal totalAmount = 0;

        foreach (var cartItem in cartItems)
        {
            // 获取SKU信息
            var sku = await _skuService.GetSkuByIdAsync(cartItem.SkuId);
            if (sku == null)
            {
                throw new BusinessException($"商品SKU不存在：{cartItem.SkuId}", 404);
            }

            // 验证库存
            if (sku.Stock < cartItem.Quantity)
            {
                throw new BusinessException($"商品库存不足：{sku.SkuName}，当前库存：{sku.Stock}，购买数量：{cartItem.Quantity}", 400);
            }

            // 构建订单项
            var orderItem = new OrderItem
            {
                SkuId = sku.Id.ToString(),
                ProductName = sku.SpuName ?? "未知商品",
                SkuName = sku.SkuName,
                SkuSpec = sku.SpecJson,
                ProductImage = "", // SkuDto 中没有 Image 字段，暂时留空
                Price = sku.Price,
                Quantity = cartItem.Quantity,
                Subtotal = sku.Price * cartItem.Quantity,
                IsRefunded = 0,
                RefundQuantity = 0
            };

            orderItems.Add(orderItem);
            totalAmount += orderItem.Subtotal;
        }

        // 3. 计算订单金额
        var order = await BuildOrderEntity(memberId, dto.ReceiverName, dto.ReceiverPhone, dto.ReceiverAddress,
            totalAmount, dto.Remark, "cart");

        // 4. 开启事务
        var result = await _db.Ado.UseTranAsync(async () =>
        {
            // 创建订单主表
            await _db.Insertable(order).ExecuteCommandAsync();

            // 创建订单明细表
            foreach (var item in orderItems)
            {
                item.OrderId = order.Id.ToString();
            }
            await _db.Insertable(orderItems).ExecuteCommandAsync();

            // 扣减库存
            foreach (var cartItem in cartItems)
            {
                // 查询当前库存
                var sku = await _skuService.GetSkuByIdAsync(cartItem.SkuId);
                var newStock = sku.Stock - cartItem.Quantity;

                if (newStock < 0)
                {
                    throw new Exception($"商品库存不足：{sku.SkuName}");
                }

                var success = await _skuService.UpdateSkuStockAsync(cartItem.SkuId, newStock);
                if (!success)
                {
                    throw new Exception("库存扣减失败");
                }
            }

            // 清空购物车
            await _cartService.BatchDeleteAsync(dto.CartIds);
        });

        if (!result.IsSuccess)
        {
            _logger.LogError("订单创建失败：{ErrorMessage}", result.ErrorMessage);
            throw new BusinessException($"订单创建失败：{result.ErrorMessage}", 400);
        }

        _logger.LogInformation("订单创建成功：订单号={OrderNo}，会员ID={MemberId}", order.OrderNo, memberId);

        return new CreateOrderResultDto
        {
            OrderId = order.Id.ToString(),
            OrderNo = order.OrderNo
        };
    }

    /// <summary>
    /// 直接购买创建订单
    /// </summary>
    public async Task<CreateOrderResultDto> CreateOrderDirectAsync(string memberId, CreateOrderDirectDto dto)
    {
        // 1. 获取SKU信息
        var sku = await _skuService.GetSkuByIdAsync(dto.SkuId);
        if (sku == null)
        {
            throw new BusinessException($"商品SKU不存在：{dto.SkuId}", 404);
        }

        // 2. 验证库存
        if (sku.Stock < dto.Quantity)
        {
            throw new BusinessException($"商品库存不足：{sku.SkuName}，当前库存：{sku.Stock}，购买数量：{dto.Quantity}", 400);
        }

        // 3. 构建订单项
        var orderItem = new OrderItem
        {
            SkuId = sku.Id.ToString(),
            ProductName = sku.SpuName ?? "未知商品",
            SkuName = sku.SkuName,
            SkuSpec = sku.SpecJson,
            ProductImage = "", // SkuDto 中没有 Image 字段，暂时留空
            Price = sku.Price,
            Quantity = dto.Quantity,
            Subtotal = sku.Price * dto.Quantity,
            IsRefunded = 0,
            RefundQuantity = 0
        };

        // 4. 构建订单主表
        var order = await BuildOrderEntity(memberId, dto.ReceiverName, dto.ReceiverPhone, dto.ReceiverAddress,
            orderItem.Subtotal, dto.Remark, "direct");

        // 5. 开启事务
        var result = await _db.Ado.UseTranAsync(async () =>
        {
            // 创建订单主表
            await _db.Insertable(order).ExecuteCommandAsync();

            // 创建订单明细表
            orderItem.OrderId = order.Id.ToString();
            await _db.Insertable(orderItem).ExecuteCommandAsync();

            // 扣减库存
            var currentSku = await _skuService.GetSkuByIdAsync(dto.SkuId);
            var newStock = currentSku.Stock - dto.Quantity;

            if (newStock < 0)
            {
                throw new Exception($"商品库存不足：{currentSku.SkuName}");
            }

            var success = await _skuService.UpdateSkuStockAsync(dto.SkuId, newStock);
            if (!success)
            {
                throw new Exception("库存扣减失败");
            }
        });

        if (!result.IsSuccess)
        {
            _logger.LogError("订单创建失败：{ErrorMessage}", result.ErrorMessage);
            throw new BusinessException($"订单创建失败：{result.ErrorMessage}", 400);
        }

        _logger.LogInformation("订单创建成功：订单号={OrderNo}，会员ID={MemberId}", order.OrderNo, memberId);

        return new CreateOrderResultDto
        {
            OrderId = order.Id.ToString(),
            OrderNo = order.OrderNo
        };
    }

    #endregion

    #region 查询订单

    /// <summary>
    /// 分页查询订单列表
    /// </summary>
    public async Task<PageResponse<OrderListDto>> GetOrderListAsync(OrderQuery query)
    {
        var queryable = _db.Queryable<Order>()
            .WhereIF(!string.IsNullOrEmpty(query.MemberId), o => o.MemberId == query.MemberId)
            .WhereIF(query.Status.HasValue, o => (int)o.Status == query.Status.Value)
            .WhereIF(!string.IsNullOrEmpty(query.OrderNo), o => o.OrderNo.Contains(query.OrderNo!))
            .WhereIF(query.StartTime.HasValue, o => o.CreatedAt >= query.StartTime)
            .WhereIF(query.EndTime.HasValue, o => o.CreatedAt <= query.EndTime)
            .OrderBy(o => o.CreatedAt, OrderByType.Desc);

        RefAsync<int> totalCount = 0;
        var orders = await queryable.ToPageListAsync(query.PageIndex, query.PageSize, totalCount);

        var dtos = orders.Adapt<List<OrderListDto>>();

        return new PageResponse<OrderListDto>
        {
            List = dtos,
            Total = totalCount.Value,
            PageIndex = query.PageIndex,
            PageSize = query.PageSize
        };
    }

    /// <summary>
    /// 获取订单详情
    /// </summary>
    public async Task<OrderDetailDto> GetOrderDetailAsync(string orderId)
    {
        var order = await _db.Queryable<Order>()
            .FirstAsync(o => o.Id.ToString() == orderId && o.IsDeleted == 0);

        if (order == null)
        {
            throw new BusinessException("订单不存在", 404);
        }

        // 查询订单项
        var orderItems = await _db.Queryable<OrderItem>()
            .Where(oi => oi.OrderId == orderId && oi.IsDeleted == 0)
            .ToListAsync();

        var dto = order.Adapt<OrderDetailDto>();
        dto.Items = orderItems.Adapt<List<OrderItemDto>>();

        return dto;
    }

    /// <summary>
    /// 根据订单号获取订单详情
    /// </summary>
    public async Task<OrderDetailDto> GetOrderDetailByNoAsync(string orderNo)
    {
        var order = await _db.Queryable<Order>()
            .FirstAsync(o => o.OrderNo == orderNo && o.IsDeleted == 0);

        if (order == null)
        {
            throw new BusinessException("订单不存在", 404);
        }

        // 查询订单项
        var orderItems = await _db.Queryable<OrderItem>()
            .Where(oi => oi.OrderId == order.Id.ToString() && oi.IsDeleted == 0)
            .ToListAsync();

        var dto = order.Adapt<OrderDetailDto>();
        dto.Items = orderItems.Adapt<List<OrderItemDto>>();

        return dto;
    }

    /// <summary>
    /// 获取会员订单列表
    /// </summary>
    public async Task<PageResponse<OrderListDto>> GetMemberOrderListAsync(string memberId, string? status = null, int pageIndex = 1, int pageSize = 10)
    {
        var query = new OrderQuery
        {
            MemberId = memberId,
            PageIndex = pageIndex,
            PageSize = pageSize
        };

        // 解析状态字符串为枚举值
        if (!string.IsNullOrEmpty(status) && int.TryParse(status, out var statusValue))
        {
            query.Status = statusValue;
        }

        return await GetOrderListAsync(query);
    }

    /// <summary>
    /// 获取订单统计数据
    /// </summary>
    public async Task<OrderStatisticsDto> GetOrderStatisticsAsync(string? memberId = null)
    {
        var query = _db.Queryable<Order>()
            .WhereIF(!string.IsNullOrEmpty(memberId), o => o.MemberId == memberId);

        var stats = new OrderStatisticsDto
        {
            TotalOrders = await query.Clone().Where(o => o.IsDeleted == 0).CountAsync(),
            PendingPayment = await query.Clone().Where(o => o.Status == OrderStatus.PendingPayment && o.IsDeleted == 0).CountAsync(),
            PendingDelivery = await query.Clone().Where(o => o.Status == OrderStatus.PendingDelivery && o.IsDeleted == 0).CountAsync(),
            PendingReceive = await query.Clone().Where(o => o.Status == OrderStatus.PendingReceive && o.IsDeleted == 0).CountAsync(),
            Completed = await query.Clone().Where(o => o.Status == OrderStatus.Completed && o.IsDeleted == 0).CountAsync(),
            Cancelled = await query.Clone().Where(o => o.Status == OrderStatus.Cancelled && o.IsDeleted == 0).CountAsync()
        };

        return stats;
    }

    #endregion

    #region 订单状态管理

    /// <summary>
    /// 取消订单
    /// </summary>
    public async Task<bool> CancelOrderAsync(string orderId, OrderCancelDto dto, string? operatorId = null)
    {
        var order = await _db.Queryable<Order>()
            .FirstAsync(o => o.Id.ToString() == orderId && o.IsDeleted == 0);

        if (order == null)
        {
            throw new BusinessException("订单不存在", 404);
        }

        // 验证订单状态
        if (order.Status != OrderStatus.PendingPayment && order.Status != OrderStatus.PendingDelivery)
        {
            throw new BusinessException("订单状态不允许取消", 400);
        }

        // 查询订单项
        var orderItems = await _db.Queryable<OrderItem>()
            .Where(oi => oi.OrderId == orderId && oi.IsDeleted == 0)
            .ToListAsync();

        // 开启事务
        var result = await _db.Ado.UseTranAsync(async () =>
        {
            // 恢复库存
            foreach (var item in orderItems)
            {
                // 查询当前库存
                var sku = await _skuService.GetSkuByIdAsync(item.SkuId);
                var newStock = sku.Stock + item.Quantity;

                var success = await _skuService.UpdateSkuStockAsync(item.SkuId, newStock);
                if (!success)
                {
                    throw new Exception("库存恢复失败");
                }
            }

            // 更新订单状态
            order.Status = OrderStatus.Cancelled;
            order.CancelTime = DateTime.Now;
            order.AdminRemark = dto.CancelReason;
            order.UpdatedAt = DateTime.Now;

            await _db.Updateable(order).ExecuteCommandAsync();
        });

        if (!result.IsSuccess)
        {
            _logger.LogError("订单取消失败：{ErrorMessage}", result.ErrorMessage);
            throw new BusinessException($"订单取消失败：{result.ErrorMessage}", 400);
        }

        _logger.LogInformation("订单取消成功：订单号={OrderNo}", order.OrderNo);
        return true;
    }

    /// <summary>
    /// 发货
    /// </summary>
    public async Task<bool> DeliverOrderAsync(string orderId, OrderDeliverDto dto)
    {
        var order = await _db.Queryable<Order>()
            .FirstAsync(o => o.Id.ToString() == orderId && o.IsDeleted == 0);

        if (order == null)
        {
            throw new BusinessException("订单不存在", 404);
        }

        // 验证订单状态
        if (order.Status != OrderStatus.PendingDelivery)
        {
            throw new BusinessException("订单状态不允许发货", 400);
        }

        // 更新订单状态
        order.Status = OrderStatus.PendingReceive;
        order.DeliveryTime = DateTime.Now;
        order.LogisticsCompany = dto.LogisticsCompany;
        order.LogisticsNo = dto.LogisticsNo;
        order.UpdatedAt = DateTime.Now;

        var success = await _db.Updateable(order).ExecuteCommandAsync() > 0;

        if (success)
        {
            _logger.LogInformation("订单发货成功：订单号={OrderNo}，物流公司={LogisticsCompany}，物流单号={LogisticsNo}",
                order.OrderNo, dto.LogisticsCompany, dto.LogisticsNo);
        }

        return success;
    }

    /// <summary>
    /// 确认收货
    /// </summary>
    public async Task<bool> ConfirmReceiveAsync(string orderId)
    {
        var order = await _db.Queryable<Order>()
            .FirstAsync(o => o.Id.ToString() == orderId && o.IsDeleted == 0);

        if (order == null)
        {
            throw new BusinessException("订单不存在", 404);
        }

        // 验证订单状态
        if (order.Status != OrderStatus.PendingReceive)
        {
            throw new BusinessException("订单状态不允许确认收货", 400);
        }

        // 更新订单状态
        order.Status = OrderStatus.Completed;
        order.ReceiveTime = DateTime.Now;
        order.CompleteTime = DateTime.Now;
        order.UpdatedAt = DateTime.Now;

        var success = await _db.Updateable(order).ExecuteCommandAsync() > 0;

        if (success)
        {
            _logger.LogInformation("订单确认收货成功：订单号={OrderNo}", order.OrderNo);
        }

        return success;
    }

    /// <summary>
    /// 更新订单状态（系统内部调用）
    /// </summary>
    public async Task<bool> UpdateOrderStatusAsync(string orderId, string status, string? remark = null)
    {
        var order = await _db.Queryable<Order>()
            .FirstAsync(o => o.Id.ToString() == orderId && o.IsDeleted == 0);

        if (order == null)
        {
            throw new BusinessException("订单不存在", 404);
        }

        // 解析状态
        if (!Enum.TryParse<OrderStatus>(status, out var newStatus))
        {
            throw new BusinessException($"无效的订单状态：{status}", 400);
        }

        // 更新订单状态
        order.Status = newStatus;
        order.AdminRemark = remark;
        order.UpdatedAt = DateTime.Now;

        var success = await _db.Updateable(order).ExecuteCommandAsync() > 0;

        if (success)
        {
            _logger.LogInformation("订单状态更新成功：订单号={OrderNo}，新状态={Status}", order.OrderNo, status);
        }

        return success;
    }

    #endregion

    #region 订单支付

    /// <summary>
    /// 标记订单为已支付（支付服务调用）
    /// </summary>
    public async Task<bool> MarkOrderPaidAsync(string orderId, DateTime paymentTime)
    {
        var order = await _db.Queryable<Order>()
            .FirstAsync(o => o.Id.ToString() == orderId && o.IsDeleted == 0);

        if (order == null)
        {
            throw new BusinessException("订单不存在", 404);
        }

        // 验证订单状态
        if (order.Status != OrderStatus.PendingPayment)
        {
            throw new BusinessException("订单状态不允许支付", 400);
        }

        // 更新订单状态
        order.Status = OrderStatus.PendingDelivery;
        order.PaymentTime = paymentTime;
        order.UpdatedAt = DateTime.Now;

        var success = await _db.Updateable(order).ExecuteCommandAsync() > 0;

        if (success)
        {
            _logger.LogInformation("订单支付成功：订单号={OrderNo}，支付时间={PaymentTime}", order.OrderNo, paymentTime);
        }

        return success;
    }

    #endregion

    #region 私有方法

    /// <summary>
    /// 构建订单实体
    /// </summary>
    private async Task<Order> BuildOrderEntity(string memberId, string receiverName, string receiverPhone,
        string receiverAddress, decimal totalAmount, string? remark, string source)
    {
        // 生成订单号
        var orderNo = await GenerateOrderNoAsync();

        return new Order
        {
            OrderNo = orderNo,
            MemberId = memberId,
            ReceiverName = receiverName,
            ReceiverPhone = receiverPhone,
            ReceiverAddress = receiverAddress,
            TotalAmount = totalAmount,
            PayAmount = totalAmount,
            FreightAmount = 0,
            DiscountAmount = 0,
            Status = OrderStatus.PendingPayment,
            Source = source,
            Remark = remark
        };
    }

    /// <summary>
    /// 生成订单号
    /// </summary>
    /// <returns>订单号</returns>
    /// <remarks>
    /// 格式：年月日时分秒 + 6位随机数
    /// 示例：20260910152530123456
    /// </remarks>
    private async Task<string> GenerateOrderNoAsync()
    {
        // 查询今天的订单数量
        var today = DateTime.Today;
        var count = await _db.Queryable<Order>()
            .Where(o => o.CreatedAt >= today && o.IsDeleted == 0)
            .CountAsync();

        // 格式：年月日时分秒 + 6位序号
        var orderNo = $"{DateTime.Now:yyyyMMddHHmmss}{(count + 1):D6}";

        return orderNo;
    }

    #endregion
}