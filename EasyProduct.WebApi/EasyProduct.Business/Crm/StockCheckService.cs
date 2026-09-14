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
/// 盘点服务实现
/// </summary>
/// <remarks>
/// 提供盘点单的增删改查、完成盘点等功能
/// </remarks>
public class StockCheckService : BaseService, IStockCheckService
{
    /// <summary>
    /// 获取盘点单分页列表
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>盘点单分页结果</returns>
    public async Task<PageResponse<StockCheckDto>> GetListAsync(StockCheckQueryDto query)
    {
        var queryable = _db.Queryable<StockCheck>()
            .Where(c => c.IsDeleted == 0);

        // 编号搜索
        if (!string.IsNullOrWhiteSpace(query.CheckNo))
        {
            queryable = queryable.Where(c => c.CheckNo.ToLower().Contains(query.CheckNo.ToLower()));
        }

        // 仓库筛选
        if (!string.IsNullOrWhiteSpace(query.WarehouseId))
        {
            queryable = queryable.Where(c => c.WarehouseId == query.WarehouseId);
        }

        // 状态筛选
        if (!string.IsNullOrWhiteSpace(query.Status))
        {
            if (Enum.TryParse<StockCheckStatus>(query.Status, true, out var status))
            {
                queryable = queryable.Where(c => c.Status == status);
            }
        }

        // 按创建时间倒序
        queryable = queryable.OrderBy(c => c.CreatedAt, OrderByType.Desc);

        // 分页查询
        RefAsync<int> total = 0;
        var result = await queryable
            .Select(c => new StockCheckDto
            {
                Id = c.Id.ToString(),
                CheckNo = c.CheckNo,
                WarehouseId = c.WarehouseId,
                WarehouseName = c.WarehouseName,
                Checker = c.Checker,
                CheckDate = c.CheckDate.ToString("yyyy-MM-dd"),
                Status = c.Status.ToString().ToLower(),
                StatusName = GetStatusName(c.Status),
                Remark = c.Remark,
                CreatedAt = c.CreatedAt.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                UpdatedAt = c.UpdatedAt.HasValue ? c.UpdatedAt.Value.ToString("yyyy-MM-ddTHH:mm:ssZ") : null
            })
            .ToPageListAsync(query.PageIndex, query.PageSize, total);

        return PageResponse<StockCheckDto>.Create(result, total.Value, query.PageIndex, query.PageSize);
    }

    /// <summary>
    /// 获取盘点单详情
    /// </summary>
    /// <param name="id">盘点单ID</param>
    /// <returns>盘点单详情</returns>
    public async Task<StockCheckDetailDto> GetDetailAsync(Guid id)
    {
        var check = await _db.Queryable<StockCheck>()
            .Where(c => c.Id == id && c.IsDeleted == 0)
            .FirstAsync();

        if (check == null)
        {
            throw new BusinessException("盘点单不存在或已被删除");
        }

        // 查询明细
        var items = await _db.Queryable<StockCheckItem>()
            .Where(i => i.CheckId == check.Id.ToString() && i.IsDeleted == 0)
            .ToListAsync();

        return new StockCheckDetailDto
        {
            Id = check.Id.ToString(),
            CheckNo = check.CheckNo,
            WarehouseId = check.WarehouseId,
            WarehouseName = check.WarehouseName,
            Checker = check.Checker,
            CheckDate = check.CheckDate.ToString("yyyy-MM-dd"),
            Status = check.Status.ToString().ToLower(),
            StatusName = GetStatusName(check.Status),
            Remark = check.Remark,
            Items = items.Select(i => new StockCheckItemDto
            {
                Id = i.Id.ToString(),
                CheckId = i.CheckId,
                SkuCode = i.SkuCode,
                SkuName = i.SkuName,
                Spec = i.Spec,
                Unit = i.Unit,
                SystemQty = i.SystemQty,
                CountedQty = i.CountedQty,
                Diff = i.Diff
            }).ToList(),
            CreatedAt = check.CreatedAt.ToString("yyyy-MM-ddTHH:mm:ssZ"),
            UpdatedAt = check.UpdatedAt.HasValue ? check.UpdatedAt.Value.ToString("yyyy-MM-ddTHH:mm:ssZ") : null
        };
    }

    /// <summary>
    /// 创建盘点单
    /// </summary>
    /// <param name="dto">创建盘点单参数</param>
    /// <returns>新创建的盘点单ID</returns>
    public async Task<Guid> CreateAsync(CreateStockCheckDto dto)
    {
        // 获取仓库信息
        var warehouse = await _db.Queryable<Warehouse>()
            .Where(w => w.Id.ToString() == dto.WarehouseId && w.IsDeleted == 0)
            .FirstAsync();

        if (warehouse == null)
        {
            throw new BusinessException("仓库不存在或已被删除");
        }

        // 生成盘点单编号
        var checkNo = await GenerateCheckNoAsync();

        // 解析盘点日期
        var checkDate = !string.IsNullOrWhiteSpace(dto.CheckDate)
            ? DateTime.Parse(dto.CheckDate)
            : DateTime.Today;

        var check = new StockCheck
        {
            CheckNo = checkNo,
            WarehouseId = dto.WarehouseId,
            WarehouseName = warehouse.Name,
            Checker = dto.Checker,
            CheckDate = checkDate,
            Status = StockCheckStatus.Draft,
            Remark = dto.Remark
        };

        // 插入盘点单
        await _db.Insertable(check).ExecuteCommandIdentityIntoEntityAsync();

        // 创建明细
        if (dto.Items != null && dto.Items.Count > 0)
        {
            // 使用提供的明细
            foreach (var itemDto in dto.Items)
            {
                var item = new StockCheckItem
                {
                    CheckId = check.Id.ToString(),
                    SkuCode = itemDto.SkuCode,
                    SkuName = itemDto.SkuName ?? itemDto.SkuCode,
                    Spec = itemDto.Spec ?? "",
                    Unit = itemDto.Unit ?? "个"
                };

                // 从库存账面获取系统数量
                var stock = await _db.Queryable<Stock>()
                    .Where(s => s.WarehouseId == dto.WarehouseId && s.SkuCode == itemDto.SkuCode && s.IsDeleted == 0)
                    .FirstAsync();

                if (stock != null)
                {
                    item.SkuName = stock.SkuName;
                    item.Spec = stock.Spec;
                    item.Unit = stock.Unit;
                    item.SystemQty = stock.Total;
                }

                await _db.Insertable(item).ExecuteCommandIdentityIntoEntityAsync();
            }
        }
        else
        {
            // 从库存账面自动生成明细
            var stocks = await _db.Queryable<Stock>()
                .Where(s => s.WarehouseId == dto.WarehouseId && s.IsDeleted == 0)
                .ToListAsync();

            foreach (var stock in stocks)
            {
                var item = new StockCheckItem
                {
                    CheckId = check.Id.ToString(),
                    SkuCode = stock.SkuCode,
                    SkuName = stock.SkuName,
                    Spec = stock.Spec,
                    Unit = stock.Unit,
                    SystemQty = stock.Total
                };

                await _db.Insertable(item).ExecuteCommandIdentityIntoEntityAsync();
            }
        }

        return check.Id;
    }

    /// <summary>
    /// 更新盘点单
    /// </summary>
    /// <param name="id">盘点单ID</param>
    /// <param name="dto">更新盘点单参数</param>
    /// <returns>更新是否成功</returns>
    public async Task<bool> UpdateAsync(Guid id, UpdateStockCheckDto dto)
    {
        var check = await _db.Queryable<StockCheck>()
            .Where(c => c.Id == id && c.IsDeleted == 0)
            .FirstAsync();

        if (check == null)
        {
            throw new BusinessException("盘点单不存在或已被删除");
        }

        // 验证状态
        if (check.Status == StockCheckStatus.Completed)
        {
            throw new BusinessException("已完成的盘点单不可修改");
        }

        // 更新字段
        if (!string.IsNullOrWhiteSpace(dto.Checker))
        {
            check.Checker = dto.Checker;
        }

        if (!string.IsNullOrWhiteSpace(dto.CheckDate))
        {
            check.CheckDate = DateTime.Parse(dto.CheckDate);
        }

        if (dto.Remark != null)
        {
            check.Remark = dto.Remark;
        }

        check.UpdatedAt = DateTime.UtcNow;

        // 更新明细
        if (dto.Items != null && dto.Items.Count > 0)
        {
            foreach (var itemDto in dto.Items)
            {
                if (!string.IsNullOrWhiteSpace(itemDto.Id))
                {
                    // 更新现有明细
                    var item = await _db.Queryable<StockCheckItem>()
                        .Where(i => i.Id.ToString() == itemDto.Id && i.IsDeleted == 0)
                        .FirstAsync();

                    if (item != null)
                    {
                        item.CountedQty = itemDto.CountedQty;
                        item.Diff = itemDto.CountedQty - item.SystemQty;
                        item.UpdatedAt = DateTime.UtcNow;

                        await _db.Updateable(item).ExecuteCommandAsync();
                    }
                }
            }
        }

        var result = await _db.Updateable(check).ExecuteCommandAsync();

        return result > 0;
    }

    /// <summary>
    /// 完成盘点
    /// </summary>
    /// <param name="id">盘点单ID</param>
    /// <returns>完成是否成功</returns>
    public async Task<bool> CompleteAsync(Guid id)
    {
        var check = await _db.Queryable<StockCheck>()
            .Where(c => c.Id == id && c.IsDeleted == 0)
            .FirstAsync();

        if (check == null)
        {
            throw new BusinessException("盘点单不存在或已被删除");
        }

        // 验证状态
        if (check.Status == StockCheckStatus.Completed)
        {
            throw new BusinessException("盘点单已完成");
        }

        // 查询明细
        var items = await _db.Queryable<StockCheckItem>()
            .Where(i => i.CheckId == check.Id.ToString() && i.IsDeleted == 0)
            .ToListAsync();

        // 验证所有明细的实盘数量已填写
        if (items.Any(i => i.CountedQty == 0))
        {
            throw new BusinessException("存在未填写实盘数量的明细项");
        }

        // 调整库存并创建流水
        foreach (var item in items)
        {
            if (item.Diff != 0)
            {
                // 调整库存
                var stock = await _db.Queryable<Stock>()
                    .Where(s => s.WarehouseId == check.WarehouseId && s.SkuCode == item.SkuCode && s.IsDeleted == 0)
                    .FirstAsync();

                if (stock != null)
                {
                    stock.Available += item.Diff;
                    stock.Total += item.Diff;
                    stock.UpdatedAt = DateTime.UtcNow;

                    await _db.Updateable(stock).ExecuteCommandAsync();
                }

                // TODO: 创建出入库流水记录（待集成）
            }
        }

        // 更新盘点状态
        check.Status = StockCheckStatus.Completed;
        check.UpdatedAt = DateTime.UtcNow;

        var result = await _db.Updateable(check).ExecuteCommandAsync();

        return result > 0;
    }

    /// <summary>
    /// 删除盘点单
    /// </summary>
    /// <param name="id">盘点单ID</param>
    /// <returns>删除是否成功</returns>
    public async Task<bool> DeleteAsync(Guid id)
    {
        var check = await _db.Queryable<StockCheck>()
            .Where(c => c.Id == id && c.IsDeleted == 0)
            .FirstAsync();

        if (check == null)
        {
            throw new BusinessException("盘点单不存在或已被删除");
        }

        // 验证状态
        if (check.Status != StockCheckStatus.Draft)
        {
            throw new BusinessException("仅草稿状态的盘点单可删除");
        }

        // 软删除盘点单
        check.IsDeleted = 1;
        check.UpdatedAt = DateTime.UtcNow;

        var result = await _db.Updateable(check).ExecuteCommandAsync();

        return result > 0;
    }

    /// <summary>
    /// 生成盘点单编号
    /// </summary>
    /// <returns>盘点单编号（格式：SC-{year}-{sequence:04d}）</returns>
    private async Task<string> GenerateCheckNoAsync()
    {
        var year = DateTime.Now.Year;

        // 查询当年最大序号
        var maxCheck = await _db.Queryable<StockCheck>()
            .Where(c => c.CheckNo.StartsWith($"SC-{year}-"))
            .OrderBy(c => c.CheckNo, OrderByType.Desc)
            .FirstAsync();

        int sequence = 1;
        if (maxCheck != null)
        {
            // 解析编号获取序号
            var parts = maxCheck.CheckNo.Split('-');
            if (parts.Length == 3 && int.TryParse(parts[2], out var lastSeq))
            {
                sequence = lastSeq + 1;
            }
        }

        return $"SC-{year}-{sequence:0000}";
    }

    /// <summary>
    /// 获取状态名称
    /// </summary>
    private string GetStatusName(StockCheckStatus status)
    {
        return status switch
        {
            StockCheckStatus.Draft => "草稿",
            StockCheckStatus.Counting => "盘点中",
            StockCheckStatus.Completed => "已完成",
            _ => "未知"
        };
    }
}