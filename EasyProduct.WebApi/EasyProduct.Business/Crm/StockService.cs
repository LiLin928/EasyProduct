using EasyProduct.Common.Base;
using EasyProduct.Common.Error;
using EasyProduct.Models.Dto.Crm;
using EasyProduct.Models.Entitys.Crm;
using EasyProduct.Models.Enums;
using Mapster;
using SqlSugar;

namespace EasyProduct.Business.Crm;

/// <summary>
/// 库存服务实现
/// </summary>
/// <remarks>
/// 提供库存查询、调整、检查等功能
/// </remarks>
public class StockService : BaseService, IStockService
{
    /// <summary>
    /// 获取库存分页列表
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>库存分页结果</returns>
    public async Task<PageResponse<StockDto>> GetListAsync(StockQueryDto query)
    {
        var queryable = _db.Queryable<Stock>()
            .Where(s => s.IsDeleted == 0);

        // 仓库筛选
        if (!string.IsNullOrWhiteSpace(query.WarehouseId))
        {
            queryable = queryable.Where(s => s.WarehouseId == query.WarehouseId);
        }

        // SKU编码搜索
        if (!string.IsNullOrWhiteSpace(query.SkuCode))
        {
            queryable = queryable.Where(s => s.SkuCode.ToLower().Contains(query.SkuCode.ToLower()));
        }

        // SKU名称搜索
        if (!string.IsNullOrWhiteSpace(query.SkuName))
        {
            queryable = queryable.Where(s => s.SkuName.ToLower().Contains(query.SkuName.ToLower()));
        }

        // 按更新时间倒序
        queryable = queryable.OrderBy(s => s.UpdatedAt, OrderByType.Desc);

        // 分页查询
        RefAsync<int> total = 0;
        var result = await queryable
            .Select(s => new StockDto
            {
                Id = s.Id.ToString(),
                WarehouseId = s.WarehouseId,
                WarehouseName = s.WarehouseName,
                SkuCode = s.SkuCode,
                SkuName = s.SkuName,
                Spec = s.Spec,
                Unit = s.Unit,
                Available = s.Available,
                Locked = s.Locked,
                Total = s.Total,
                MinLimit = s.MinLimit,
                MaxLimit = s.MaxLimit,
                UpdatedAt = s.UpdatedAt.HasValue ? s.UpdatedAt.Value.ToString("yyyy-MM-ddTHH:mm:ssZ") : null
            })
            .ToPageListAsync(query.PageIndex, query.PageSize, total);

        return PageResponse<StockDto>.Create(result, total.Value, query.PageIndex, query.PageSize);
    }

    /// <summary>
    /// 获取库存详情
    /// </summary>
    /// <param name="id">库存ID</param>
    /// <returns>库存详情</returns>
    public async Task<StockDto> GetDetailAsync(Guid id)
    {
        var stock = await _db.Queryable<Stock>()
            .Where(s => s.Id == id && s.IsDeleted == 0)
            .FirstAsync();

        if (stock == null)
        {
            throw new BusinessException("库存记录不存在或已被删除");
        }

        return new StockDto
        {
            Id = stock.Id.ToString(),
            WarehouseId = stock.WarehouseId,
            WarehouseName = stock.WarehouseName,
            SkuCode = stock.SkuCode,
            SkuName = stock.SkuName,
            Spec = stock.Spec,
            Unit = stock.Unit,
            Available = stock.Available,
            Locked = stock.Locked,
            Total = stock.Total,
            MinLimit = stock.MinLimit,
            MaxLimit = stock.MaxLimit,
            UpdatedAt = stock.UpdatedAt.HasValue ? stock.UpdatedAt.Value.ToString("yyyy-MM-ddTHH:mm:ssZ") : null
        };
    }

    /// <summary>
    /// 库存调整
    /// </summary>
    /// <param name="dto">库存调整参数</param>
    /// <returns>调整是否成功</returns>
    public async Task<bool> AdjustAsync(StockAdjustDto dto)
    {
        // 查找库存记录
        var stock = await _db.Queryable<Stock>()
            .Where(s => s.WarehouseId == dto.WarehouseId
                && s.SkuCode == dto.SkuCode
                && s.IsDeleted == 0)
            .FirstAsync();

        if (stock == null)
        {
            // 如果不存在，创建新的库存记录
            // 需要从仓库表和 SKU 表获取信息
            var warehouse = await _db.Queryable<Warehouse>()
                .Where(w => w.Id.ToString() == dto.WarehouseId && w.IsDeleted == 0)
                .FirstAsync();

            if (warehouse == null)
            {
                throw new BusinessException("仓库不存在或已被删除");
            }

            // 注意：这里需要 SKU 模块支持，暂时使用占位数据
            stock = new Stock
            {
                WarehouseId = dto.WarehouseId,
                WarehouseName = warehouse.Name,
                SkuCode = dto.SkuCode,
                SkuName = dto.SkuCode, // TODO: 从 SKU 表获取
                Spec = "", // TODO: 从 SKU 表获取
                Unit = "个", // TODO: 从 SKU 表获取
                Available = 0,
                Locked = 0,
                Total = 0
            };

            await _db.Insertable(stock).ExecuteCommandIdentityIntoEntityAsync();
        }

        // 更新库存数量
        if (dto.Quantity > 0)
        {
            // 增加库存
            stock.Available += dto.Quantity;
            stock.Total += dto.Quantity;
        }
        else
        {
            // 减少库存
            var reduceQuantity = Math.Abs(dto.Quantity);
            if (stock.Available < reduceQuantity)
            {
                throw new BusinessException($"库存不足：可用库存 {stock.Available}，需要减少 {reduceQuantity}");
            }
            stock.Available -= reduceQuantity;
            stock.Total -= reduceQuantity;
        }

        stock.UpdatedAt = DateTime.UtcNow;

        var result = await _db.Updateable(stock).ExecuteCommandAsync();

        // TODO: 创建出入库流水记录（待批次 3 完成）
        // TODO: 检查库存预警（待批次 5 完成）

        return result > 0;
    }

    /// <summary>
    /// 检查库存是否充足
    /// </summary>
    /// <param name="warehouseId">仓库ID</param>
    /// <param name="skuCode">SKU编码</param>
    /// <param name="quantity">所需数量</param>
    /// <returns>库存是否充足</returns>
    public async Task<bool> CheckStockAsync(string warehouseId, string skuCode, int quantity)
    {
        var stock = await _db.Queryable<Stock>()
            .Where(s => s.WarehouseId == warehouseId
                && s.SkuCode == skuCode
                && s.IsDeleted == 0)
            .FirstAsync();

        if (stock == null)
        {
            return false;
        }

        return stock.Available >= quantity;
    }
}