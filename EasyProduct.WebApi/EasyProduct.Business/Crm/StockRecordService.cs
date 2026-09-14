using EasyProduct.Common.Base;
using EasyProduct.Models.Dto.Crm;
using EasyProduct.Models.Entitys.Crm;
using EasyProduct.Models.Enums.Crm;
using Mapster;
using SqlSugar;

namespace EasyProduct.Business.Crm;

/// <summary>
/// 出入库流水服务实现
/// </summary>
/// <remarks>
/// 提供出入库流水的查询、创建等功能
/// </remarks>
public class StockRecordService : BaseService, IStockRecordService
{
    /// <summary>
    /// 获取出入库流水分页列表
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>流水分页结果</returns>
    public async Task<PageResponse<StockRecordDto>> GetListAsync(StockRecordQueryDto query)
    {
        var queryable = _db.Queryable<StockRecord>()
            .Where(r => r.IsDeleted == 0);

        // 仓库筛选
        if (!string.IsNullOrWhiteSpace(query.WarehouseId))
        {
            queryable = queryable.Where(r => r.WarehouseId == query.WarehouseId);
        }

        // SKU编码搜索
        if (!string.IsNullOrWhiteSpace(query.SkuCode))
        {
            queryable = queryable.Where(r => r.SkuCode.ToLower().Contains(query.SkuCode.ToLower()));
        }

        // 出入库类型筛选
        if (!string.IsNullOrWhiteSpace(query.Type))
        {
            if (Enum.TryParse<StockRecordType>(query.Type, true, out var type))
            {
                queryable = queryable.Where(r => r.Type == type);
            }
        }

        // 流水来源类型筛选
        if (!string.IsNullOrWhiteSpace(query.SourceType))
        {
            if (Enum.TryParse<StockRecordSourceType>(query.SourceType, true, out var sourceType))
            {
                queryable = queryable.Where(r => r.SourceType == sourceType);
            }
        }

        // 来源单据编号搜索
        if (!string.IsNullOrWhiteSpace(query.SourceOrderNo))
        {
            queryable = queryable.Where(r => r.SourceOrderNo.ToLower().Contains(query.SourceOrderNo.ToLower()));
        }

        // 按创建时间倒序
        queryable = queryable.OrderBy(r => r.CreatedAt, OrderByType.Desc);

        // 分页查询
        RefAsync<int> total = 0;
        var result = await queryable
            .Select(r => new StockRecordDto
            {
                Id = r.Id.ToString(),
                WarehouseId = r.WarehouseId,
                WarehouseName = r.WarehouseName,
                SkuCode = r.SkuCode,
                SkuName = r.SkuName,
                Spec = r.Spec,
                Unit = r.Unit,
                Type = r.Type.ToString().ToLower(),
                TypeName = GetTypeName(r.Type),
                SourceType = GetSourceTypeString(r.SourceType),
                SourceTypeName = GetSourceTypeName(r.SourceType),
                SourceOrderNo = r.SourceOrderNo,
                Quantity = r.Quantity,
                Operator = r.Operator,
                Remark = r.Remark,
                CreatedAt = r.CreatedAt.ToString("yyyy-MM-ddTHH:mm:ssZ")
            })
            .ToPageListAsync(query.PageIndex, query.PageSize, total);

        return PageResponse<StockRecordDto>.Create(result, total.Value, query.PageIndex, query.PageSize);
    }

    /// <summary>
    /// 创建出入库流水记录
    /// </summary>
    /// <param name="dto">创建流水参数</param>
    /// <returns>新创建的流水ID</returns>
    public async Task<Guid> CreateAsync(CreateStockRecordDto dto)
    {
        // 解析出入库类型
        if (!Enum.TryParse<StockRecordType>(dto.Type, true, out var type))
        {
            throw new ArgumentException($"无效的出入库类型：{dto.Type}");
        }

        // 解析流水来源类型
        if (!Enum.TryParse<StockRecordSourceType>(dto.SourceType, true, out var sourceType))
        {
            throw new ArgumentException($"无效的流水来源类型：{dto.SourceType}");
        }

        var record = new StockRecord
        {
            WarehouseId = dto.WarehouseId,
            WarehouseName = dto.WarehouseName,
            SkuCode = dto.SkuCode,
            SkuName = dto.SkuName,
            Spec = dto.Spec,
            Unit = dto.Unit,
            Type = type,
            SourceType = sourceType,
            SourceOrderNo = dto.SourceOrderNo,
            Quantity = dto.Quantity,
            Operator = dto.Operator,
            Remark = dto.Remark
        };

        await _db.Insertable(record).ExecuteCommandIdentityIntoEntityAsync();

        return record.Id;
    }

    /// <summary>
    /// 获取出入库类型名称
    /// </summary>
    private string GetTypeName(StockRecordType type)
    {
        return type switch
        {
            StockRecordType.In => "入库",
            StockRecordType.Out => "出库",
            _ => "未知"
        };
    }

    /// <summary>
    /// 获取流水来源类型字符串（与 mockjs 保持一致）
    /// </summary>
    private string GetSourceTypeString(StockRecordSourceType sourceType)
    {
        return sourceType switch
        {
            StockRecordSourceType.PurchaseIn => "purchase_in",
            StockRecordSourceType.SalesOut => "sales_out",
            StockRecordSourceType.MallOut => "mall_out",
            StockRecordSourceType.CheckAdjust => "check_adjust",
            StockRecordSourceType.ReversalReturn => "reversal_return",
            _ => "unknown"
        };
    }

    /// <summary>
    /// 获取流水来源类型名称
    /// </summary>
    private string GetSourceTypeName(StockRecordSourceType sourceType)
    {
        return sourceType switch
        {
            StockRecordSourceType.PurchaseIn => "采购入库",
            StockRecordSourceType.SalesOut => "销售出库",
            StockRecordSourceType.MallOut => "商城出库",
            StockRecordSourceType.CheckAdjust => "盘点调整",
            StockRecordSourceType.ReversalReturn => "冲销退回",
            _ => "未知"
        };
    }
}