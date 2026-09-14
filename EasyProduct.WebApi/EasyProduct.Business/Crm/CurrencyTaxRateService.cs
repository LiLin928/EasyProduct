using EasyProduct.Common.Base;
using EasyProduct.Common.Error;
using EasyProduct.Models.Dto.Crm;
using EasyProduct.Models.Entitys.Crm;
using EasyProduct.Models.Enums;
using Mapster;
using SqlSugar;

namespace EasyProduct.Business.Crm;

/// <summary>
/// 币种服务实现
/// </summary>
/// <remarks>
/// 提供币种的增删改查功能
/// </remarks>
public class CurrencyService : BaseService, ICurrencyService
{
    /// <summary>
    /// 获取币种分页列表
    /// </summary>
    /// <param name="query">查询参数，包含币种代码、币种名称、状态、分页信息</param>
    /// <returns>币种分页结果</returns>
    public async Task<PageResponse<CurrencyDto>> GetListAsync(CurrencyQueryDto query)
    {
        var queryable = _db.Queryable<Currency>()
            .Where(c => c.IsDeleted == 0);

        // 币种代码搜索
        if (!string.IsNullOrWhiteSpace(query.Code))
        {
            queryable = queryable.Where(c => c.Code.Contains(query.Code));
        }

        // 币种名称搜索
        if (!string.IsNullOrWhiteSpace(query.Name))
        {
            queryable = queryable.Where(c => c.Name.Contains(query.Name));
        }

        // 状态筛选
        if (query.Status.HasValue)
        {
            queryable = queryable.Where(c => c.Status == (Status)query.Status);
        }

        // 按创建时间倒序
        queryable = queryable.OrderBy(c => c.CreatedAt, OrderByType.Desc);

        // 分页查询
        RefAsync<int> total = 0;
        var result = await queryable
            .Select(c => new CurrencyDto
            {
                Id = c.Id.ToString(),
                Code = c.Code,
                Name = c.Name,
                Symbol = c.Symbol,
                ExchangeRate = c.ExchangeRate,
                Status = (int)c.Status,
                StatusName = c.Status == Status.Enabled ? "启用" : "禁用",
                IsDefault = c.IsDefault,
                IsDefaultName = c.IsDefault == 1 ? "是" : "否",
                CreatedAt = c.CreatedAt
            })
            .ToPageListAsync(query.PageIndex, query.PageSize, total);

        return PageResponse<CurrencyDto>.Create(result, total.Value, query.PageIndex, query.PageSize);
    }

    /// <summary>
    /// 获取币种详情
    /// </summary>
    /// <param name="id">币种ID</param>
    /// <returns>币种详情</returns>
    public async Task<CurrencyDto> GetByIdAsync(Guid id)
    {
        var currency = await _db.Queryable<Currency>()
            .Where(c => c.Id == id && c.IsDeleted == 0)
            .FirstAsync();

        if (currency == null)
        {
            throw BusinessException.NotFound("币种不存在");
        }

        return new CurrencyDto
        {
            Id = currency.Id.ToString(),
            Code = currency.Code,
            Name = currency.Name,
            Symbol = currency.Symbol,
            ExchangeRate = currency.ExchangeRate,
            Status = (int)currency.Status,
            StatusName = currency.Status == Status.Enabled ? "启用" : "禁用",
            IsDefault = currency.IsDefault,
            IsDefaultName = currency.IsDefault == 1 ? "是" : "否",
            CreatedAt = currency.CreatedAt
        };
    }

    /// <summary>
    /// 创建币种
    /// </summary>
    /// <param name="dto">创建参数</param>
    /// <returns>新创建的币种ID</returns>
    /// <remarks>
    /// 如果设置为默认币种，会自动取消其他币种的默认标记
    /// </remarks>
    public async Task<Guid> CreateAsync(CreateCurrencyDto dto)
    {
        // 如果设置为默认币种，取消其他币种的默认标记
        if (dto.IsDefault == 1)
        {
            await _db.Updateable<Currency>()
                .SetColumns(c => c.IsDefault == 0)
                .Where(c => c.IsDefault == 1)
                .ExecuteCommandAsync();
        }

        var currency = dto.Adapt<Currency>();
        currency.Id = Guid.NewGuid();
        currency.Status = Status.Enabled;

        await _db.Insertable(currency).ExecuteCommandAsync();

        return currency.Id;
    }

    /// <summary>
    /// 更新币种
    /// </summary>
    /// <param name="id">币种ID</param>
    /// <param name="dto">更新参数</param>
    /// <returns>更新是否成功</returns>
    /// <remarks>
    /// 如果设置为默认币种，会自动取消其他币种的默认标记
    /// </remarks>
    public async Task<bool> UpdateAsync(Guid id, UpdateCurrencyDto dto)
    {
        var currency = await _db.Queryable<Currency>()
            .Where(c => c.Id == id && c.IsDeleted == 0)
            .FirstAsync();

        if (currency == null)
        {
            throw BusinessException.NotFound("币种不存在");
        }

        // 如果设置为默认币种，取消其他币种的默认标记
        if (dto.IsDefault == 1 && currency.IsDefault == 0)
        {
            await _db.Updateable<Currency>()
                .SetColumns(c => c.IsDefault == 0)
                .Where(c => c.IsDefault == 1)
                .ExecuteCommandAsync();
        }

        // 更新字段
        currency.Code = dto.Code;
        currency.Name = dto.Name;
        currency.Symbol = dto.Symbol;
        currency.ExchangeRate = dto.ExchangeRate;
        currency.IsDefault = dto.IsDefault;
        currency.UpdatedAt = DateTime.UtcNow;

        return await _db.Updateable(currency).ExecuteCommandAsync() > 0;
    }

    /// <summary>
    /// 删除币种
    /// </summary>
    /// <param name="id">币种ID</param>
    /// <returns>删除是否成功</returns>
    /// <remarks>
    /// 使用软删除，将 IsDeleted 字段设置为 1
    /// </remarks>
    public async Task<bool> DeleteAsync(Guid id)
    {
        var currency = await _db.Queryable<Currency>()
            .Where(c => c.Id == id && c.IsDeleted == 0)
            .FirstAsync();

        if (currency == null)
        {
            throw BusinessException.NotFound("币种不存在");
        }

        // 软删除
        currency.IsDeleted = 1;
        currency.UpdatedAt = DateTime.UtcNow;

        return await _db.Updateable(currency).ExecuteCommandAsync() > 0;
    }

    /// <summary>
    /// 启用/禁用币种
    /// </summary>
    /// <param name="id">币种ID</param>
    /// <param name="status">状态（0=禁用，1=启用）</param>
    /// <returns>操作是否成功</returns>
    public async Task<bool> UpdateStatusAsync(Guid id, int status)
    {
        var currency = await _db.Queryable<Currency>()
            .Where(c => c.Id == id && c.IsDeleted == 0)
            .FirstAsync();

        if (currency == null)
        {
            throw BusinessException.NotFound("币种不存在");
        }

        currency.Status = (Status)status;
        currency.UpdatedAt = DateTime.UtcNow;

        return await _db.Updateable(currency).ExecuteCommandAsync() > 0;
    }

    /// <summary>
    /// 获取启用的币种列表（下拉选择用）
    /// </summary>
    /// <returns>币种列表</returns>
    public async Task<List<CurrencyDto>> GetActiveListAsync()
    {
        var currencies = await _db.Queryable<Currency>()
            .Where(c => c.IsDeleted == 0 && c.Status == Status.Enabled)
            .OrderBy(c => c.Code)
            .ToListAsync();

        return currencies.Select(c => new CurrencyDto
        {
            Id = c.Id.ToString(),
            Code = c.Code,
            Name = c.Name,
            Symbol = c.Symbol,
            ExchangeRate = c.ExchangeRate,
            IsDefault = c.IsDefault
        }).ToList();
    }
}

/// <summary>
/// 税率服务实现
/// </summary>
/// <remarks>
/// 提供税率的增删改查功能
/// </remarks>
public class TaxRateService : BaseService, ITaxRateService
{
    /// <summary>
    /// 获取税率分页列表
    /// </summary>
    /// <param name="query">查询参数，包含税率名称、状态、分页信息</param>
    /// <returns>税率分页结果</returns>
    public async Task<PageResponse<TaxRateDto>> GetListAsync(TaxRateQueryDto query)
    {
        var queryable = _db.Queryable<TaxRate>()
            .Where(t => t.IsDeleted == 0);

        // 税率名称搜索
        if (!string.IsNullOrWhiteSpace(query.Name))
        {
            queryable = queryable.Where(t => t.Name.Contains(query.Name));
        }

        // 状态筛选
        if (query.Status.HasValue)
        {
            queryable = queryable.Where(t => t.Status == (Status)query.Status);
        }

        // 按创建时间倒序
        queryable = queryable.OrderBy(t => t.CreatedAt, OrderByType.Desc);

        // 分页查询
        RefAsync<int> total = 0;
        var result = await queryable
            .Select(t => new TaxRateDto
            {
                Id = t.Id.ToString(),
                Name = t.Name,
                Rate = t.Rate,
                RatePercent = $"{t.Rate:P}",
                Description = t.Description,
                Status = (int)t.Status,
                StatusName = t.Status == Status.Enabled ? "启用" : "禁用",
                CreatedAt = t.CreatedAt
            })
            .ToPageListAsync(query.PageIndex, query.PageSize, total);

        return PageResponse<TaxRateDto>.Create(result, total.Value, query.PageIndex, query.PageSize);
    }

    /// <summary>
    /// 获取税率详情
    /// </summary>
    /// <param name="id">税率ID</param>
    /// <returns>税率详情</returns>
    public async Task<TaxRateDto> GetByIdAsync(Guid id)
    {
        var taxRate = await _db.Queryable<TaxRate>()
            .Where(t => t.Id == id && t.IsDeleted == 0)
            .FirstAsync();

        if (taxRate == null)
        {
            throw BusinessException.NotFound("税率不存在");
        }

        return new TaxRateDto
        {
            Id = taxRate.Id.ToString(),
            Name = taxRate.Name,
            Rate = taxRate.Rate,
            RatePercent = $"{taxRate.Rate:P}",
            Description = taxRate.Description,
            Status = (int)taxRate.Status,
            StatusName = taxRate.Status == Status.Enabled ? "启用" : "禁用",
            CreatedAt = taxRate.CreatedAt
        };
    }

    /// <summary>
    /// 创建税率
    /// </summary>
    /// <param name="dto">创建参数</param>
    /// <returns>新创建的税率ID</returns>
    public async Task<Guid> CreateAsync(CreateTaxRateDto dto)
    {
        var taxRate = dto.Adapt<TaxRate>();
        taxRate.Id = Guid.NewGuid();
        taxRate.Status = Status.Enabled;

        await _db.Insertable(taxRate).ExecuteCommandAsync();

        return taxRate.Id;
    }

    /// <summary>
    /// 更新税率
    /// </summary>
    /// <param name="id">税率ID</param>
    /// <param name="dto">更新参数</param>
    /// <returns>更新是否成功</returns>
    public async Task<bool> UpdateAsync(Guid id, UpdateTaxRateDto dto)
    {
        var taxRate = await _db.Queryable<TaxRate>()
            .Where(t => t.Id == id && t.IsDeleted == 0)
            .FirstAsync();

        if (taxRate == null)
        {
            throw BusinessException.NotFound("税率不存在");
        }

        // 更新字段
        taxRate.Name = dto.Name;
        taxRate.Rate = dto.Rate;
        taxRate.Description = dto.Description;
        taxRate.UpdatedAt = DateTime.UtcNow;

        return await _db.Updateable(taxRate).ExecuteCommandAsync() > 0;
    }

    /// <summary>
    /// 删除税率
    /// </summary>
    /// <param name="id">税率ID</param>
    /// <returns>删除是否成功</returns>
    /// <remarks>
    /// 使用软删除，将 IsDeleted 字段设置为 1
    /// </remarks>
    public async Task<bool> DeleteAsync(Guid id)
    {
        var taxRate = await _db.Queryable<TaxRate>()
            .Where(t => t.Id == id && t.IsDeleted == 0)
            .FirstAsync();

        if (taxRate == null)
        {
            throw BusinessException.NotFound("税率不存在");
        }

        // 软删除
        taxRate.IsDeleted = 1;
        taxRate.UpdatedAt = DateTime.UtcNow;

        return await _db.Updateable(taxRate).ExecuteCommandAsync() > 0;
    }

    /// <summary>
    /// 启用/禁用税率
    /// </summary>
    /// <param name="id">税率ID</param>
    /// <param name="status">状态（0=禁用，1=启用）</param>
    /// <returns>操作是否成功</returns>
    public async Task<bool> UpdateStatusAsync(Guid id, int status)
    {
        var taxRate = await _db.Queryable<TaxRate>()
            .Where(t => t.Id == id && t.IsDeleted == 0)
            .FirstAsync();

        if (taxRate == null)
        {
            throw BusinessException.NotFound("税率不存在");
        }

        taxRate.Status = (Status)status;
        taxRate.UpdatedAt = DateTime.UtcNow;

        return await _db.Updateable(taxRate).ExecuteCommandAsync() > 0;
    }

    /// <summary>
    /// 获取启用的税率列表（下拉选择用）
    /// </summary>
    /// <returns>税率列表</returns>
    public async Task<List<TaxRateDto>> GetActiveListAsync()
    {
        var taxRates = await _db.Queryable<TaxRate>()
            .Where(t => t.IsDeleted == 0 && t.Status == Status.Enabled)
            .OrderBy(t => t.Rate)
            .ToListAsync();

        return taxRates.Select(t => new TaxRateDto
        {
            Id = t.Id.ToString(),
            Name = t.Name,
            Rate = t.Rate,
            RatePercent = $"{t.Rate:P}",
            Description = t.Description
        }).ToList();
    }
}