using EasyProduct.Common.Base;
using EasyProduct.Common.Error;
using EasyProduct.Models.Dto.Crm;
using EasyProduct.Models.Entitys.Crm;
using EasyProduct.Models.Enums;
using Mapster;
using SqlSugar;

namespace EasyProduct.Business.Crm;

/// <summary>
/// 仓库服务实现
/// </summary>
/// <remarks>
/// 提供仓库的增删改查、下拉选项等功能
/// </remarks>
public class WarehouseService : BaseService, IWarehouseService
{
    /// <summary>
    /// 获取仓库下拉选项列表
    /// </summary>
    /// <returns>仓库下拉选项列表</returns>
    public async Task<List<WarehouseOptionDto>> GetWarehouseOptionsAsync()
    {
        var warehouses = await _db.Queryable<Warehouse>()
            .Where(w => w.IsDeleted == 0 && w.Status == Status.Enabled)
            .OrderBy(w => w.CreatedAt, OrderByType.Desc)
            .ToListAsync();

        return warehouses.Select(w => new WarehouseOptionDto
        {
            Id = w.Id.ToString(),
            Code = w.Code,
            Name = w.Name
        }).ToList();
    }

    /// <summary>
    /// 获取仓库分页列表
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>仓库分页结果</returns>
    public async Task<PageResponse<WarehouseDto>> GetListAsync(WarehouseQueryDto query)
    {
        var queryable = _db.Queryable<Warehouse>()
            .Where(w => w.IsDeleted == 0);

        // 编码搜索
        if (!string.IsNullOrWhiteSpace(query.Code))
        {
            queryable = queryable.Where(w => w.Code.ToLower().Contains(query.Code.ToLower()));
        }

        // 名称搜索
        if (!string.IsNullOrWhiteSpace(query.Name))
        {
            queryable = queryable.Where(w => w.Name.ToLower().Contains(query.Name.ToLower()));
        }

        // 状态筛选
        if (!string.IsNullOrWhiteSpace(query.Status))
        {
            if (query.Status.ToLower() == "active")
            {
                queryable = queryable.Where(w => w.Status == Status.Enabled);
            }
            else if (query.Status.ToLower() == "inactive")
            {
                queryable = queryable.Where(w => w.Status == Status.Disabled);
            }
        }

        // 按创建时间倒序
        queryable = queryable.OrderBy(w => w.CreatedAt, OrderByType.Desc);

        // 分页查询
        RefAsync<int> total = 0;
        var result = await queryable
            .Select(w => new WarehouseDto
            {
                Id = w.Id.ToString(),
                Code = w.Code,
                Name = w.Name,
                Address = w.Address,
                Manager = w.Manager,
                Phone = w.Phone,
                Status = w.Status == Status.Enabled ? "active" : "inactive",
                Remark = w.Remark,
                CreatedAt = w.CreatedAt.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                UpdatedAt = w.UpdatedAt.HasValue ? w.UpdatedAt.Value.ToString("yyyy-MM-ddTHH:mm:ssZ") : null
            })
            .ToPageListAsync(query.PageIndex, query.PageSize, total);

        return PageResponse<WarehouseDto>.Create(result, total.Value, query.PageIndex, query.PageSize);
    }

    /// <summary>
    /// 获取仓库详情
    /// </summary>
    /// <param name="id">仓库ID</param>
    /// <returns>仓库详情</returns>
    public async Task<WarehouseDto> GetDetailAsync(Guid id)
    {
        var warehouse = await _db.Queryable<Warehouse>()
            .Where(w => w.Id == id && w.IsDeleted == 0)
            .FirstAsync();

        if (warehouse == null)
        {
            throw new BusinessException("仓库不存在或已被删除");
        }

        return new WarehouseDto
        {
            Id = warehouse.Id.ToString(),
            Code = warehouse.Code,
            Name = warehouse.Name,
            Address = warehouse.Address,
            Manager = warehouse.Manager,
            Phone = warehouse.Phone,
            Status = warehouse.Status == Status.Enabled ? "active" : "inactive",
            Remark = warehouse.Remark,
            CreatedAt = warehouse.CreatedAt.ToString("yyyy-MM-ddTHH:mm:ssZ"),
            UpdatedAt = warehouse.UpdatedAt.HasValue ? warehouse.UpdatedAt.Value.ToString("yyyy-MM-ddTHH:mm:ssZ") : null
        };
    }

    /// <summary>
    /// 创建仓库
    /// </summary>
    /// <param name="dto">创建仓库参数</param>
    /// <returns>新创建的仓库ID</returns>
    public async Task<Guid> CreateAsync(CreateWarehouseDto dto)
    {
        // 检查编码是否已存在
        var exists = await _db.Queryable<Warehouse>()
            .Where(w => w.Code == dto.Code && w.IsDeleted == 0)
            .AnyAsync();

        if (exists)
        {
            throw new BusinessException($"仓库编码 {dto.Code} 已存在");
        }

        var warehouse = new Warehouse
        {
            Code = dto.Code,
            Name = dto.Name,
            Address = dto.Address,
            Manager = dto.Manager,
            Phone = dto.Phone,
            Status = Status.Enabled, // 默认启用
            Remark = dto.Remark
        };

        await _db.Insertable(warehouse).ExecuteCommandIdentityIntoEntityAsync();

        return warehouse.Id;
    }

    /// <summary>
    /// 更新仓库
    /// </summary>
    /// <param name="id">仓库ID</param>
    /// <param name="dto">更新仓库参数</param>
    /// <returns>更新是否成功</returns>
    public async Task<bool> UpdateAsync(Guid id, UpdateWarehouseDto dto)
    {
        var warehouse = await _db.Queryable<Warehouse>()
            .Where(w => w.Id == id && w.IsDeleted == 0)
            .FirstAsync();

        if (warehouse == null)
        {
            throw new BusinessException("仓库不存在或已被删除");
        }

        // 更新字段
        if (!string.IsNullOrWhiteSpace(dto.Name))
        {
            warehouse.Name = dto.Name;
        }

        if (!string.IsNullOrWhiteSpace(dto.Address))
        {
            warehouse.Address = dto.Address;
        }

        if (!string.IsNullOrWhiteSpace(dto.Manager))
        {
            warehouse.Manager = dto.Manager;
        }

        if (!string.IsNullOrWhiteSpace(dto.Phone))
        {
            warehouse.Phone = dto.Phone;
        }

        if (dto.Remark != null)
        {
            warehouse.Remark = dto.Remark;
        }

        warehouse.UpdatedAt = DateTime.UtcNow;

        var result = await _db.Updateable(warehouse).ExecuteCommandAsync();

        return result > 0;
    }

    /// <summary>
    /// 删除仓库
    /// </summary>
    /// <param name="id">仓库ID</param>
    /// <returns>删除是否成功</returns>
    public async Task<bool> DeleteAsync(Guid id)
    {
        var warehouse = await _db.Queryable<Warehouse>()
            .Where(w => w.Id == id && w.IsDeleted == 0)
            .FirstAsync();

        if (warehouse == null)
        {
            throw new BusinessException("仓库不存在或已被删除");
        }

        // 检查是否有关联数据（库存、出入库流水、盘点单）
        // TODO: 待库存模块完成后添加关联检查

        // 软删除
        warehouse.IsDeleted = 1;
        warehouse.UpdatedAt = DateTime.UtcNow;

        var result = await _db.Updateable(warehouse).ExecuteCommandAsync();

        return result > 0;
    }
}