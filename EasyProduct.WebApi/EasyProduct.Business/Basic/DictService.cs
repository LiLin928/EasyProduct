using EasyProduct.Common.Base;
using EasyProduct.Common.Error;
using EasyProduct.Models.Dto.Basic.Dict;
using EasyProduct.Models.Entitys.Basic;
using Mapster;
using Microsoft.Extensions.Logging;
using SqlSugar;

namespace EasyProduct.Business.Basic;

/// <summary>
/// 字典服务实现
/// </summary>
/// <remarks>
/// 提供字典类型和字典数据的增删改查、缓存管理等功能
/// </remarks>
public class DictService : BaseService, IDictService
{
    private readonly ILogger<DictService> _logger;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="logger">日志服务</param>
    public DictService(ILogger<DictService> logger)
    {
        _logger = logger;
    }

    #region 字典类型管理

    /// <summary>
    /// 获取字典类型列表
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>字典类型列表</returns>
    public async Task<List<DictTypeDto>> GetDictTypeListAsync(DictTypeQueryDto query)
    {
        var queryable = _db.Queryable<basic_dict_type>()
            .Where(x => x.IsDeleted == 0);

        if (!string.IsNullOrEmpty(query.DictName))
        {
            queryable = queryable.Where(x => x.DictName.Contains(query.DictName));
        }

        if (!string.IsNullOrEmpty(query.DictType))
        {
            queryable = queryable.Where(x => x.DictType.Contains(query.DictType));
        }

        if (query.Status.HasValue)
        {
            queryable = queryable.Where(x => (int)x.Status == query.Status.Value);
        }

        var list = await queryable
            .OrderBy(x => x.CreatedAt, OrderByType.Desc)
            .ToListAsync();

        return list.Adapt<List<DictTypeDto>>();
    }

    /// <summary>
    /// 获取字典类型详情
    /// </summary>
    /// <param name="id">字典类型ID</param>
    /// <returns>字典类型详情</returns>
    public async Task<DictTypeDto> GetDictTypeByIdAsync(string id)
    {
        var entity = await _db.Queryable<basic_dict_type>()
            .Where(x => x.Id.ToString() == id && x.IsDeleted == 0)
            .FirstAsync();

        if (entity == null)
        {
            throw new BusinessException("字典类型不存在", 404);
        }

        return entity.Adapt<DictTypeDto>();
    }

    /// <summary>
    /// 创建字典类型
    /// </summary>
    /// <param name="dto">创建参数</param>
    /// <returns>新字典类型ID</returns>
    public async Task<string> CreateDictTypeAsync(CreateDictTypeDto dto)
    {
        // 检查字典类型是否已存在
        var exists = await _db.Queryable<basic_dict_type>()
            .Where(x => x.DictType == dto.DictType && x.IsDeleted == 0)
            .AnyAsync();

        if (exists)
        {
            throw new BusinessException($"字典类型 {dto.DictType} 已存在");
        }

        var entity = dto.Adapt<basic_dict_type>();
        entity.Id = Guid.NewGuid();
        entity.Status = (Models.Enums.Status)dto.Status;
        entity.CreatedAt = DateTime.UtcNow;

        await _db.Insertable(entity).ExecuteCommandAsync();

        _logger.LogInformation("创建字典类型成功：{DictType}, ID: {Id}", entity.DictType, entity.Id);

        return entity.Id.ToString();
    }

    /// <summary>
    /// 更新字典类型
    /// </summary>
    /// <param name="dto">更新参数</param>
    /// <returns>是否成功</returns>
    public async Task<bool> UpdateDictTypeAsync(UpdateDictTypeDto dto)
    {
        var entity = await _db.Queryable<basic_dict_type>()
            .Where(x => x.Id.ToString() == dto.Id && x.IsDeleted == 0)
            .FirstAsync();

        if (entity == null)
        {
            throw new BusinessException("字典类型不存在", 404);
        }

        entity.DictName = dto.DictName;
        entity.Status = (Models.Enums.Status)dto.Status;
        entity.Remark = dto.Remark;
        entity.UpdatedAt = DateTime.UtcNow;

        await _db.Updateable(entity).ExecuteCommandAsync();

        _logger.LogInformation("更新字典类型成功：{DictType}, ID: {Id}", entity.DictType, entity.Id);

        return true;
    }

    /// <summary>
    /// 删除字典类型
    /// </summary>
    /// <param name="id">字典类型ID</param>
    /// <returns>是否成功</returns>
    public async Task<bool> DeleteDictTypeAsync(string id)
    {
        var entity = await _db.Queryable<basic_dict_type>()
            .Where(x => x.Id.ToString() == id && x.IsDeleted == 0)
            .FirstAsync();

        if (entity == null)
        {
            throw new BusinessException("字典类型不存在", 404);
        }

        // 检查是否有字典数据
        var hasData = await _db.Queryable<basic_dict_data>()
            .Where(x => x.DictType == entity.DictType && x.IsDeleted == 0)
            .AnyAsync();

        if (hasData)
        {
            throw new BusinessException("该字典类型下存在字典数据，不能删除");
        }

        // 软删除
        entity.IsDeleted = 1;
        entity.UpdatedAt = DateTime.UtcNow;

        await _db.Updateable(entity).ExecuteCommandAsync();

        _logger.LogInformation("删除字典类型成功：{DictType}, ID: {Id}", entity.DictType, entity.Id);

        return true;
    }

    #endregion

    #region 字典数据管理

    /// <summary>
    /// 获取字典数据列表
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>字典数据列表</returns>
    public async Task<List<DictDataDto>> GetDictDataListAsync(DictDataQueryDto query)
    {
        var queryable = _db.Queryable<basic_dict_data>()
            .Where(x => x.DictType == query.DictType && x.IsDeleted == 0);

        if (!string.IsNullOrEmpty(query.DictLabel))
        {
            queryable = queryable.Where(x => x.DictLabel.Contains(query.DictLabel));
        }

        if (query.Status.HasValue)
        {
            queryable = queryable.Where(x => (int)x.Status == query.Status.Value);
        }

        var list = await queryable
            .OrderBy(x => x.Sort)
            .OrderBy(x => x.CreatedAt, OrderByType.Desc)
            .ToListAsync();

        return list.Adapt<List<DictDataDto>>();
    }

    /// <summary>
    /// 根据字典类型获取字典数据（用于前端下拉框等）
    /// </summary>
    /// <param name="dictType">字典类型</param>
    /// <returns>字典数据列表</returns>
    public async Task<List<DictDataDto>> GetDictDataByTypeAsync(string dictType)
    {
        var list = await _db.Queryable<basic_dict_data>()
            .Where(x => x.DictType == dictType && x.IsDeleted == 0 && x.Status == Models.Enums.Status.Enabled)
            .OrderBy(x => x.Sort)
            .ToListAsync();

        return list.Adapt<List<DictDataDto>>();
    }

    /// <summary>
    /// 获取所有字典类型及数据（用于前端缓存）
    /// </summary>
    /// <returns>字典类型及数据列表</returns>
    public async Task<List<DictTypeWithDataDto>> GetAllDictWithDataAsync()
    {
        // 查询所有启用的字典类型
        var dictTypes = await _db.Queryable<basic_dict_type>()
            .Where(x => x.IsDeleted == 0 && x.Status == Models.Enums.Status.Enabled)
            .ToListAsync();

        if (!dictTypes.Any())
        {
            return new List<DictTypeWithDataDto>();
        }

        // 查询所有启用的字典数据
        var dictTypeStrings = dictTypes.Select(x => x.DictType).ToList();
        var dictDataList = await _db.Queryable<basic_dict_data>()
            .Where(x => dictTypeStrings.Contains(x.DictType) && x.IsDeleted == 0 && x.Status == Models.Enums.Status.Enabled)
            .OrderBy(x => x.Sort)
            .ToListAsync();

        // 组装结果
        var result = dictTypes.Select(dt => new DictTypeWithDataDto
        {
            DictType = dt.DictType,
            DictName = dt.DictName,
            DataList = dictDataList
                .Where(dd => dd.DictType == dt.DictType)
                .Adapt<List<DictDataDto>>()
        }).ToList();

        return result;
    }

    /// <summary>
    /// 获取字典数据详情
    /// </summary>
    /// <param name="id">字典数据ID</param>
    /// <returns>字典数据详情</returns>
    public async Task<DictDataDto> GetDictDataByIdAsync(string id)
    {
        var entity = await _db.Queryable<basic_dict_data>()
            .Where(x => x.Id.ToString() == id && x.IsDeleted == 0)
            .FirstAsync();

        if (entity == null)
        {
            throw new BusinessException("字典数据不存在", 404);
        }

        return entity.Adapt<DictDataDto>();
    }

    /// <summary>
    /// 创建字典数据
    /// </summary>
    /// <param name="dto">创建参数</param>
    /// <returns>新字典数据ID</returns>
    public async Task<string> CreateDictDataAsync(CreateDictDataDto dto)
    {
        // 检查字典类型是否存在
        var dictTypeExists = await _db.Queryable<basic_dict_type>()
            .Where(x => x.DictType == dto.DictType && x.IsDeleted == 0)
            .AnyAsync();

        if (!dictTypeExists)
        {
            throw new BusinessException($"字典类型 {dto.DictType} 不存在");
        }

        // 检查同一字典类型下字典值是否重复
        var valueExists = await _db.Queryable<basic_dict_data>()
            .Where(x => x.DictType == dto.DictType && x.DictValue == dto.DictValue && x.IsDeleted == 0)
            .AnyAsync();

        if (valueExists)
        {
            throw new BusinessException($"字典值 {dto.DictValue} 在该字典类型下已存在");
        }

        var entity = dto.Adapt<basic_dict_data>();
        entity.Id = Guid.NewGuid();
        entity.Status = (Models.Enums.Status)dto.Status;
        entity.CreatedAt = DateTime.UtcNow;

        await _db.Insertable(entity).ExecuteCommandAsync();

        _logger.LogInformation("创建字典数据成功：{DictType}/{DictValue}, ID: {Id}",
            entity.DictType, entity.DictValue, entity.Id);

        return entity.Id.ToString();
    }

    /// <summary>
    /// 更新字典数据
    /// </summary>
    /// <param name="dto">更新参数</param>
    /// <returns>是否成功</returns>
    public async Task<bool> UpdateDictDataAsync(UpdateDictDataDto dto)
    {
        var entity = await _db.Queryable<basic_dict_data>()
            .Where(x => x.Id.ToString() == dto.Id && x.IsDeleted == 0)
            .FirstAsync();

        if (entity == null)
        {
            throw new BusinessException("字典数据不存在", 404);
        }

        // 检查字典值是否重复（排除自己）
        var valueExists = await _db.Queryable<basic_dict_data>()
            .Where(x => x.DictType == entity.DictType && x.DictValue == dto.DictValue &&
                        x.Id.ToString() != dto.Id && x.IsDeleted == 0)
            .AnyAsync();

        if (valueExists)
        {
            throw new BusinessException($"字典值 {dto.DictValue} 在该字典类型下已存在");
        }

        entity.DictLabel = dto.DictLabel;
        entity.DictValue = dto.DictValue;
        entity.Sort = dto.Sort;
        entity.Status = (Models.Enums.Status)dto.Status;
        entity.Remark = dto.Remark;
        entity.UpdatedAt = DateTime.UtcNow;

        await _db.Updateable(entity).ExecuteCommandAsync();

        _logger.LogInformation("更新字典数据成功：{DictType}/{DictValue}, ID: {Id}",
            entity.DictType, entity.DictValue, entity.Id);

        return true;
    }

    /// <summary>
    /// 删除字典数据
    /// </summary>
    /// <param name="id">字典数据ID</param>
    /// <returns>是否成功</returns>
    public async Task<bool> DeleteDictDataAsync(string id)
    {
        var entity = await _db.Queryable<basic_dict_data>()
            .Where(x => x.Id.ToString() == id && x.IsDeleted == 0)
            .FirstAsync();

        if (entity == null)
        {
            throw new BusinessException("字典数据不存在", 404);
        }

        // 软删除
        entity.IsDeleted = 1;
        entity.UpdatedAt = DateTime.UtcNow;

        await _db.Updateable(entity).ExecuteCommandAsync();

        _logger.LogInformation("删除字典数据成功：{DictType}/{DictValue}, ID: {Id}",
            entity.DictType, entity.DictValue, entity.Id);

        return true;
    }

    #endregion
}