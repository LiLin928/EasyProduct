using EasyProduct.Common.Base;
using EasyProduct.Common.Error;
using EasyProduct.Models.Dto.Crm;
using EasyProduct.Models.Entitys.Crm;
using EasyProduct.Models.Enums.Crm;
using Mapster;
using SqlSugar;

namespace EasyProduct.Business.Crm;

/// <summary>
/// 库存预警服务实现
/// </summary>
/// <remarks>
/// 提供库存预警的查询、解决等功能
/// </remarks>
public class StockAlertService : BaseService, IStockAlertService
{
    /// <summary>
    /// 获取库存预警分页列表
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>预警分页结果</returns>
    public async Task<PageResponse<StockAlertDto>> GetListAsync(StockAlertQueryDto query)
    {
        var queryable = _db.Queryable<StockAlert>()
            .Where(a => a.IsDeleted == 0);

        // 仓库筛选
        if (!string.IsNullOrWhiteSpace(query.WarehouseId))
        {
            queryable = queryable.Where(a => a.WarehouseId == query.WarehouseId);
        }

        // SKU编码搜索
        if (!string.IsNullOrWhiteSpace(query.SkuCode))
        {
            queryable = queryable.Where(a => a.SkuCode.ToLower().Contains(query.SkuCode.ToLower()));
        }

        // 预警类型筛选
        if (!string.IsNullOrWhiteSpace(query.AlertType))
        {
            if (Enum.TryParse<StockAlertType>(query.AlertType, true, out var alertType))
            {
                queryable = queryable.Where(a => a.AlertType == alertType);
            }
        }

        // 预警状态筛选
        if (!string.IsNullOrWhiteSpace(query.Status))
        {
            if (Enum.TryParse<StockAlertStatus>(query.Status, true, out var status))
            {
                queryable = queryable.Where(a => a.Status == status);
            }
        }

        // 按创建时间倒序
        queryable = queryable.OrderBy(a => a.CreatedAt, OrderByType.Desc);

        // 分页查询
        RefAsync<int> total = 0;
        var result = await queryable
            .Select(a => new StockAlertDto
            {
                Id = a.Id.ToString(),
                WarehouseId = a.WarehouseId,
                WarehouseName = a.WarehouseName,
                SkuCode = a.SkuCode,
                SkuName = a.SkuName,
                Spec = a.Spec,
                Available = a.Available,
                MinLimit = a.MinLimit,
                MaxLimit = a.MaxLimit,
                AlertType = a.AlertType.ToString().ToLower(),
                AlertTypeName = GetAlertTypeName(a.AlertType),
                Status = a.Status.ToString().ToLower(),
                StatusName = GetStatusName(a.Status),
                CreatedAt = a.CreatedAt.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                ResolvedAt = a.ResolvedAt.HasValue ? a.ResolvedAt.Value.ToString("yyyy-MM-ddTHH:mm:ssZ") : null,
                Remark = a.Remark
            })
            .ToPageListAsync(query.PageIndex, query.PageSize, total);

        return PageResponse<StockAlertDto>.Create(result, total.Value, query.PageIndex, query.PageSize);
    }

    /// <summary>
    /// 解决库存预警
    /// </summary>
    /// <param name="id">预警ID</param>
    /// <param name="dto">解决预警参数</param>
    /// <returns>解决是否成功</returns>
    public async Task<bool> ResolveAsync(Guid id, ResolveAlertDto dto)
    {
        var alert = await _db.Queryable<StockAlert>()
            .Where(a => a.Id == id && a.IsDeleted == 0)
            .FirstAsync();

        if (alert == null)
        {
            throw new BusinessException("预警记录不存在或已被删除");
        }

        // 验证状态
        if (alert.Status == StockAlertStatus.Resolved)
        {
            throw new BusinessException("预警已解决");
        }

        // 更新预警状态
        alert.Status = StockAlertStatus.Resolved;
        alert.ResolvedAt = DateTime.UtcNow;
        alert.Remark = dto.Remark ?? alert.Remark;
        alert.UpdatedAt = DateTime.UtcNow;

        var result = await _db.Updateable(alert).ExecuteCommandAsync();

        return result > 0;
    }

    /// <summary>
    /// 检查库存预警
    /// </summary>
    /// <param name="warehouseId">仓库ID</param>
    /// <param name="skuCode">SKU编码</param>
    public async Task CheckAlertAsync(string warehouseId, string skuCode)
    {
        // 查询库存账面
        var stock = await _db.Queryable<Stock>()
            .Where(s => s.WarehouseId == warehouseId && s.SkuCode == skuCode && s.IsDeleted == 0)
            .FirstAsync();

        if (stock == null)
        {
            return;
        }

        // 查询仓库信息
        var warehouse = await _db.Queryable<Warehouse>()
            .Where(w => w.Id.ToString() == warehouseId && w.IsDeleted == 0)
            .FirstAsync();

        if (warehouse == null)
        {
            return;
        }

        // 检查是否触发预警
        StockAlertType? alertType = null;
        string remark = string.Empty;

        if (stock.Available < stock.MinLimit)
        {
            // 低库存预警
            alertType = StockAlertType.Low;
            remark = "库存低于下限，请及时补货";
        }
        else if (stock.Available > stock.MaxLimit)
        {
            // 高库存预警
            alertType = StockAlertType.High;
            remark = "库存接近上限，建议促销消化";
        }

        if (alertType.HasValue)
        {
            // 检查是否已存在相同预警
            var existingAlert = await _db.Queryable<StockAlert>()
                .Where(a => a.WarehouseId == warehouseId && a.SkuCode == skuCode
                    && a.AlertType == alertType.Value && a.Status == StockAlertStatus.Pending
                    && a.IsDeleted == 0)
                .FirstAsync();

            if (existingAlert == null)
            {
                // 创建预警记录
                var alert = new StockAlert
                {
                    WarehouseId = warehouseId,
                    WarehouseName = warehouse.Name,
                    SkuCode = stock.SkuCode,
                    SkuName = stock.SkuName,
                    Spec = stock.Spec,
                    Available = stock.Available,
                    MinLimit = stock.MinLimit,
                    MaxLimit = stock.MaxLimit,
                    AlertType = alertType.Value,
                    Status = StockAlertStatus.Pending,
                    Remark = remark
                };

                await _db.Insertable(alert).ExecuteCommandIdentityIntoEntityAsync();
            }
            else
            {
                // 更新现有预警的库存数量
                existingAlert.Available = stock.Available;
                existingAlert.UpdatedAt = DateTime.UtcNow;

                await _db.Updateable(existingAlert).ExecuteCommandAsync();
            }
        }
    }

    /// <summary>
    /// 获取预警类型名称
    /// </summary>
    private string GetAlertTypeName(StockAlertType alertType)
    {
        return alertType switch
        {
            StockAlertType.Low => "低库存预警",
            StockAlertType.High => "高库存预警",
            _ => "未知"
        };
    }

    /// <summary>
    /// 获取预警状态名称
    /// </summary>
    private string GetStatusName(StockAlertStatus status)
    {
        return status switch
        {
            StockAlertStatus.Pending => "待处理",
            StockAlertStatus.Resolved => "已解决",
            _ => "未知"
        };
    }
}