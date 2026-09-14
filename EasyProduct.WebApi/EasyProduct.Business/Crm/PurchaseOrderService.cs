using EasyProduct.Common.Base;
using EasyProduct.Common.Error;
using EasyProduct.Models.Dto.Crm;
using EasyProduct.Models.Entitys.Crm;
using EasyProduct.Models.Enums;
using EasyProduct.Models.Enums.Crm;
using Mapster;
using SqlSugar;

namespace EasyProduct.Business.Crm;

/// <summary>
/// 采购订单服务实现
/// </summary>
/// <remarks>
/// 提供采购订单的增删改查、状态流转、金额计算等功能
/// </remarks>
public class PurchaseOrderService : BaseService, IPurchaseOrderService
{
    /// <summary>
    /// 获取供应商下拉选项列表
    /// </summary>
    /// <returns>供应商下拉选项列表</returns>
    public async Task<List<SupplierOptionDto>> GetSupplierOptionsAsync()
    {
        var suppliers = await _db.Queryable<Supplier>()
            .Where(s => s.IsDeleted == 0 && s.Status == Status.Enabled)
            .OrderBy(s => s.CreatedAt, OrderByType.Desc)
            .ToListAsync();

        return suppliers.Select(s => new SupplierOptionDto
        {
            Id = s.Id.ToString(),
            Name = s.Name
        }).ToList();
    }

    /// <summary>
    /// 获取采购订单分页列表
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>订单分页结果</returns>
    public async Task<PageResponse<PurchaseOrderDto>> GetListAsync(PurchaseOrderQueryDto query)
    {
        var queryable = _db.Queryable<PurchaseOrder>()
            .Where(o => o.IsDeleted == 0);

        // 订单编号搜索
        if (!string.IsNullOrWhiteSpace(query.OrderNo))
        {
            queryable = queryable.Where(o => o.OrderNo.ToLower().Contains(query.OrderNo.ToLower()));
        }

        // 供应商筛选
        if (!string.IsNullOrWhiteSpace(query.SupplierId))
        {
            queryable = queryable.Where(o => o.SupplierId == query.SupplierId);
        }

        // 状态筛选
        if (!string.IsNullOrWhiteSpace(query.Status))
        {
            if (Enum.TryParse<PurchaseOrderStatus>(query.Status, true, out var status))
            {
                queryable = queryable.Where(o => o.OrderStatus == status);
            }
        }

        // 按创建时间倒序
        queryable = queryable.OrderBy(o => o.CreatedAt, OrderByType.Desc);

        // 分页查询
        RefAsync<int> total = 0;
        var result = await queryable
            .Select(o => new PurchaseOrderDto
            {
                Id = o.Id.ToString(),
                OrderNo = o.OrderNo,
                SupplierId = o.SupplierId,
                SupplierName = o.SupplierName,
                BuyerName = o.BuyerName,
                CurrencyCode = o.CurrencyCode,
                CurrencySymbol = o.CurrencySymbol,
                PaymentTerms = o.PaymentTerms,
                DeliveryDate = o.DeliveryDate.ToString("yyyy-MM-dd"),
                Status = o.OrderStatus.ToString().ToLower(),
                StatusName = GetStatusName(o.OrderStatus),
                SubtotalAmount = o.SubtotalAmount,
                TaxAmount = o.TaxAmount,
                TotalAmount = o.TotalAmount,
                Remark = o.Remark,
                CreatedAt = o.CreatedAt.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                UpdatedAt = o.UpdatedAt.HasValue ? o.UpdatedAt.Value.ToString("yyyy-MM-ddTHH:mm:ssZ") : null
            })
            .ToPageListAsync(query.PageIndex, query.PageSize, total);

        return PageResponse<PurchaseOrderDto>.Create(result, total.Value, query.PageIndex, query.PageSize);
    }

    /// <summary>
    /// 获取采购订单详情
    /// </summary>
    /// <param name="id">订单ID</param>
    /// <returns>订单详情</returns>
    public async Task<PurchaseOrderDetailDto> GetDetailAsync(Guid id)
    {
        var order = await _db.Queryable<PurchaseOrder>()
            .Where(o => o.Id == id && o.IsDeleted == 0)
            .FirstAsync();

        if (order == null)
        {
            throw BusinessException.NotFound("采购订单不存在");
        }

        // 查询订单明细
        var items = await _db.Queryable<PurchaseOrderItem>()
            .Where(i => i.OrderId == id.ToString() && i.IsDeleted == 0)
            .OrderBy(i => i.CreatedAt)
            .ToListAsync();

        return new PurchaseOrderDetailDto
        {
            Id = order.Id.ToString(),
            OrderNo = order.OrderNo,
            SupplierId = order.SupplierId,
            SupplierName = order.SupplierName,
            BuyerName = order.BuyerName,
            CurrencyCode = order.CurrencyCode,
            CurrencySymbol = order.CurrencySymbol,
            PaymentTerms = order.PaymentTerms,
            DeliveryDate = order.DeliveryDate.ToString("yyyy-MM-dd"),
            Status = order.OrderStatus.ToString().ToLower(),
            StatusName = GetStatusName(order.OrderStatus),
            SubtotalAmount = order.SubtotalAmount,
            TaxAmount = order.TaxAmount,
            TotalAmount = order.TotalAmount,
            Remark = order.Remark,
            CreatedAt = order.CreatedAt.ToString("yyyy-MM-ddTHH:mm:ssZ"),
            UpdatedAt = order.UpdatedAt.HasValue ? order.UpdatedAt.Value.ToString("yyyy-MM-ddTHH:mm:ssZ") : null,
            Items = items.Select(i => new PurchaseOrderItemDto
            {
                Id = i.Id.ToString(),
                OrderId = i.OrderId,
                SkuCode = i.SkuCode,
                SkuName = i.SkuName,
                ProductName = i.ProductName,
                Spec = i.Spec,
                Price = i.Price,
                Quantity = i.Quantity,
                TaxRateCode = i.TaxRateCode,
                TaxRate = i.TaxRate,
                Amount = i.Amount,
                TaxAmount = i.TaxAmount,
                TotalAmount = i.TotalAmount,
                WarehouseId = i.WarehouseId
            }).ToList()
        };
    }

    /// <summary>
    /// 创建采购订单
    /// </summary>
    /// <param name="dto">创建订单参数</param>
    /// <returns>新创建的订单ID</returns>
    public async Task<Guid> CreateAsync(CreatePurchaseOrderDto dto)
    {
        // 查询供应商
        var supplier = await _db.Queryable<Supplier>()
            .Where(s => s.Id.ToString() == dto.SupplierId && s.IsDeleted == 0)
            .FirstAsync();

        if (supplier == null)
        {
            throw BusinessException.NotFound("供应商不存在");
        }

        // 生成订单编号
        var orderNo = await GenerateOrderNoAsync();

        // 创建订单
        var orderId = Guid.NewGuid();
        var order = new PurchaseOrder
        {
            Id = orderId,
            OrderNo = orderNo,
            SupplierId = dto.SupplierId,
            SupplierName = supplier.Name,
            BuyerName = dto.BuyerName,
            CurrencyCode = dto.CurrencyCode ?? "CNY",
            CurrencySymbol = dto.CurrencySymbol ?? "¥",
            PaymentTerms = dto.PaymentTerms ?? "Net 30",
            DeliveryDate = string.IsNullOrWhiteSpace(dto.DeliveryDate)
                ? DateTime.Now.AddDays(10)
                : DateTime.Parse(dto.DeliveryDate),
            OrderStatus = PurchaseOrderStatus.Draft,
            Remark = dto.Remark,
            Status = Status.Enabled
        };

        // 创建订单明细并计算金额
        var itemEntities = new List<PurchaseOrderItem>();
        foreach (var itemDto in dto.Items)
        {
            var item = itemDto.Adapt<PurchaseOrderItem>();
            item.Id = Guid.NewGuid();
            item.OrderId = orderId.ToString();
            item.SkuName = itemDto.SkuName ?? itemDto.ProductName;
            item.Status = Status.Enabled;

            // 计算金额
            item.Amount = Math.Round(item.Price * item.Quantity, 2);
            item.TaxAmount = Math.Round(item.Amount * item.TaxRate / 100, 2);
            item.TotalAmount = Math.Round(item.Amount + item.TaxAmount, 2);

            itemEntities.Add(item);
        }

        // 计算订单总金额
        order.SubtotalAmount = Math.Round(itemEntities.Sum(i => i.Amount), 2);
        order.TaxAmount = Math.Round(itemEntities.Sum(i => i.TaxAmount), 2);
        order.TotalAmount = Math.Round(order.SubtotalAmount + order.TaxAmount, 2);

        // 保存到数据库
        await _db.Insertable(order).ExecuteCommandAsync();
        await _db.Insertable(itemEntities).ExecuteCommandAsync();

        return orderId;
    }

    /// <summary>
    /// 更新采购订单
    /// </summary>
    /// <param name="id">订单ID</param>
    /// <param name="dto">更新订单参数</param>
    /// <returns>更新是否成功</returns>
    public async Task<bool> UpdateAsync(Guid id, UpdatePurchaseOrderDto dto)
    {
        var order = await _db.Queryable<PurchaseOrder>()
            .Where(o => o.Id == id && o.IsDeleted == 0)
            .FirstAsync();

        if (order == null)
        {
            throw BusinessException.NotFound("采购订单不存在");
        }

        // 仅草稿状态可修改
        if (order.OrderStatus != PurchaseOrderStatus.Draft)
        {
            throw BusinessException.BadRequest("仅草稿状态可修改");
        }

        // 更新订单基本信息
        if (dto.BuyerName != null)
            order.BuyerName = dto.BuyerName;
        if (dto.PaymentTerms != null)
            order.PaymentTerms = dto.PaymentTerms;
        if (dto.DeliveryDate != null)
            order.DeliveryDate = DateTime.Parse(dto.DeliveryDate);
        if (dto.Remark != null)
            order.Remark = dto.Remark;

        // 更新订单明细
        if (dto.Items != null && dto.Items.Count > 0)
        {
            // 删除旧明细
            await _db.Deleteable<PurchaseOrderItem>()
                .Where(i => i.OrderId == id.ToString())
                .ExecuteCommandAsync();

            // 创建新明细
            var itemEntities = new List<PurchaseOrderItem>();
            foreach (var itemDto in dto.Items)
            {
                var item = itemDto.Adapt<PurchaseOrderItem>();
                item.Id = Guid.NewGuid();
                item.OrderId = id.ToString();
                item.SkuName = itemDto.SkuName ?? itemDto.ProductName;
                item.Status = Status.Enabled;

                // 计算金额
                item.Amount = Math.Round(item.Price * item.Quantity, 2);
                item.TaxAmount = Math.Round(item.Amount * item.TaxRate / 100, 2);
                item.TotalAmount = Math.Round(item.Amount + item.TaxAmount, 2);

                itemEntities.Add(item);
            }

            // 重新计算订单总金额
            order.SubtotalAmount = Math.Round(itemEntities.Sum(i => i.Amount), 2);
            order.TaxAmount = Math.Round(itemEntities.Sum(i => i.TaxAmount), 2);
            order.TotalAmount = Math.Round(order.SubtotalAmount + order.TaxAmount, 2);

            await _db.Insertable(itemEntities).ExecuteCommandAsync();
        }

        order.UpdatedAt = DateTime.UtcNow;
        return await _db.Updateable(order).ExecuteCommandAsync() > 0;
    }

    /// <summary>
    /// 更新采购订单状态
    /// </summary>
    /// <param name="id">订单ID</param>
    /// <param name="dto">状态更新参数</param>
    /// <returns>更新是否成功</returns>
    public async Task<bool> UpdateStatusAsync(Guid id, UpdatePurchaseOrderStatusDto dto)
    {
        var order = await _db.Queryable<PurchaseOrder>()
            .Where(o => o.Id == id && o.IsDeleted == 0)
            .FirstAsync();

        if (order == null)
        {
            throw BusinessException.NotFound("采购订单不存在");
        }

        // 验证状态流转
        if (!ValidateStatusTransition(order.OrderStatus.ToString().ToLower(), dto.Status.ToLower()))
        {
            throw BusinessException.BadRequest($"无法从 {order.OrderStatus} 转换到 {dto.Status}");
        }

        // 解析新状态
        if (!Enum.TryParse<PurchaseOrderStatus>(dto.Status, true, out var newStatus))
        {
            throw BusinessException.BadRequest($"无效的状态：{dto.Status}");
        }

        // 入库时处理库存
        if (newStatus == PurchaseOrderStatus.Received)
        {
            await ProcessPurchaseInboundAsync(id);
        }

        // 更新状态
        order.OrderStatus = newStatus;
        order.UpdatedAt = DateTime.UtcNow;

        return await _db.Updateable(order).ExecuteCommandAsync() > 0;
    }

    /// <summary>
    /// 删除采购订单
    /// </summary>
    /// <param name="id">订单ID</param>
    /// <returns>删除是否成功</returns>
    public async Task<bool> DeleteAsync(Guid id)
    {
        var order = await _db.Queryable<PurchaseOrder>()
            .Where(o => o.Id == id && o.IsDeleted == 0)
            .FirstAsync();

        if (order == null)
        {
            throw BusinessException.NotFound("采购订单不存在");
        }

        // 仅草稿状态可删除
        if (order.OrderStatus != PurchaseOrderStatus.Draft)
        {
            throw BusinessException.BadRequest("仅草稿状态可删除");
        }

        // 软删除订单和明细
        order.IsDeleted = 1;
        order.UpdatedAt = DateTime.UtcNow;

        await _db.Updateable(order).ExecuteCommandAsync();

        await _db.Updateable<PurchaseOrderItem>()
            .SetColumns(i => i.IsDeleted == 1)
            .Where(i => i.OrderId == id.ToString())
            .ExecuteCommandAsync();

        return true;
    }

    #region 私有方法

    /// <summary>
    /// 生成订单编号
    /// </summary>
    /// <returns>订单编号</returns>
    private async Task<string> GenerateOrderNoAsync()
    {
        var year = DateTime.Now.Year;
        var prefix = $"PO-{year}-";

        // 查询当年最大序号
        var lastOrder = await _db.Queryable<PurchaseOrder>()
            .Where(o => o.OrderNo.StartsWith(prefix))
            .OrderBy(o => o.OrderNo, OrderByType.Desc)
            .FirstAsync();

        int sequence = 1;
        if (lastOrder != null)
        {
            var lastOrderNo = lastOrder.OrderNo;
            var lastSequence = int.Parse(lastOrderNo.Substring(prefix.Length));
            sequence = lastSequence + 1;
        }

        return $"{prefix}{sequence:D4}";
    }

    /// <summary>
    /// 验证状态流转是否合法
    /// </summary>
    /// <param name="currentStatus">当前状态</param>
    /// <param name="newStatus">新状态</param>
    /// <returns>是否合法</returns>
    private bool ValidateStatusTransition(string currentStatus, string newStatus)
    {
        var validTransitions = new Dictionary<string, string[]>
        {
            { "draft", new[] { "confirmed", "cancelled" } },
            { "confirmed", new[] { "received", "cancelled" } },
            { "received", new[] { "completed" } },
            { "completed", Array.Empty<string>() },
            { "cancelled", Array.Empty<string>() }
        };

        if (validTransitions.TryGetValue(currentStatus, out var allowedStatuses))
        {
            return allowedStatuses.Contains(newStatus);
        }

        return false;
    }

    /// <summary>
    /// 处理采购入库
    /// </summary>
    /// <param name="orderId">订单ID</param>
    /// <remarks>
    /// 注意：此方法需要库存模块支持，当前不执行实际操作。
    /// 待库存模块完成后，需要实现：
    /// 1. 创建入库记录（stock_record）
    /// 2. 增加库存（stock）
    /// </remarks>
    private async Task ProcessPurchaseInboundAsync(Guid orderId)
    {
        // TODO: 待库存模块完成后实现真实逻辑
        // 1. 查询订单明细
        // 2. 为每个明细项创建入库记录
        // 3. 增加对应仓库的库存

        await Task.CompletedTask;
    }

    /// <summary>
    /// 获取状态名称
    /// </summary>
    /// <param name="status">状态枚举</param>
    /// <returns>状态名称</returns>
    private string GetStatusName(PurchaseOrderStatus status)
    {
        return status switch
        {
            PurchaseOrderStatus.Draft => "草稿",
            PurchaseOrderStatus.Confirmed => "已确认",
            PurchaseOrderStatus.Received => "已入库",
            PurchaseOrderStatus.Completed => "已完成",
            PurchaseOrderStatus.Cancelled => "已取消",
            _ => "未知"
        };
    }

    #endregion
}