using EasyProduct.Common.Base;
using EasyProduct.Common.Error;
using EasyProduct.Models.Dto.Basic.Config;
using EasyProduct.Models.Entitys.Basic;
using Mapster;
using Microsoft.Extensions.Logging;
using SqlSugar;

namespace EasyProduct.Business.Basic;

/// <summary>
/// 系统参数服务实现
/// </summary>
/// <remarks>
/// 提供系统参数的增删改查、根据键名获取值等功能
/// </remarks>
public class ConfigService : BaseService, IConfigService
{
    private readonly ILogger<ConfigService> _logger;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="logger">日志服务</param>
    public ConfigService(ILogger<ConfigService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// 获取系统参数列表
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>系统参数列表</returns>
    public async Task<List<ConfigDto>> GetConfigListAsync(ConfigQueryDto query)
    {
        var queryable = _db.Queryable<basic_config>()
            .Where(x => x.IsDeleted == 0);

        // 参数名称模糊搜索
        if (!string.IsNullOrEmpty(query.ConfigName))
        {
            queryable = queryable.Where(x => x.ConfigName.Contains(query.ConfigName));
        }

        // 参数键名模糊搜索
        if (!string.IsNullOrEmpty(query.ConfigKey))
        {
            queryable = queryable.Where(x => x.ConfigKey.Contains(query.ConfigKey));
        }

        // 系统内置筛选
        if (query.ConfigType.HasValue)
        {
            queryable = queryable.Where(x => x.ConfigType == query.ConfigType.Value);
        }

        // 状态筛选
        if (query.Status.HasValue)
        {
            queryable = queryable.Where(x => (int)x.Status == query.Status.Value);
        }

        var list = await queryable
            .OrderBy(x => x.CreatedAt, OrderByType.Desc)
            .ToListAsync();

        return list.Adapt<List<ConfigDto>>();
    }

    /// <summary>
    /// 获取系统参数详情
    /// </summary>
    /// <param name="id">参数ID</param>
    /// <returns>系统参数详情</returns>
    public async Task<ConfigDto> GetConfigByIdAsync(string id)
    {
        var entity = await _db.Queryable<basic_config>()
            .Where(x => x.Id.ToString() == id && x.IsDeleted == 0)
            .FirstAsync();

        if (entity == null)
        {
            throw new BusinessException("系统参数不存在", 404);
        }

        return entity.Adapt<ConfigDto>();
    }

    /// <summary>
    /// 根据参数键名获取参数值
    /// </summary>
    /// <param name="configKey">参数键名</param>
    /// <returns>参数键值</returns>
    public async Task<string> GetConfigValueByKeyAsync(string configKey)
    {
        var entity = await _db.Queryable<basic_config>()
            .Where(x => x.ConfigKey == configKey && x.IsDeleted == 0 && x.Status == Models.Enums.Status.Enabled)
            .FirstAsync();

        if (entity == null)
        {
            throw new BusinessException($"系统参数 {configKey} 不存在或未启用", 404);
        }

        return entity.ConfigValue;
    }

    /// <summary>
    /// 创建系统参数
    /// </summary>
    /// <param name="dto">创建参数</param>
    /// <returns>新参数ID</returns>
    public async Task<string> CreateConfigAsync(CreateConfigDto dto)
    {
        // 检查参数键名是否已存在
        var exists = await _db.Queryable<basic_config>()
            .Where(x => x.ConfigKey == dto.ConfigKey && x.IsDeleted == 0)
            .AnyAsync();

        if (exists)
        {
            throw new BusinessException($"参数键名 {dto.ConfigKey} 已存在");
        }

        var entity = dto.Adapt<basic_config>();
        entity.Id = Guid.NewGuid();
        entity.Status = (Models.Enums.Status)dto.Status;
        entity.CreatedAt = DateTime.UtcNow;

        await _db.Insertable(entity).ExecuteCommandAsync();

        _logger.LogInformation("创建系统参数成功：{ConfigKey}, ID: {Id}", entity.ConfigKey, entity.Id);

        return entity.Id.ToString();
    }

    /// <summary>
    /// 更新系统参数
    /// </summary>
    /// <param name="dto">更新参数</param>
    /// <returns>是否成功</returns>
    public async Task<bool> UpdateConfigAsync(UpdateConfigDto dto)
    {
        var entity = await _db.Queryable<basic_config>()
            .Where(x => x.Id.ToString() == dto.Id && x.IsDeleted == 0)
            .FirstAsync();

        if (entity == null)
        {
            throw new BusinessException("系统参数不存在", 404);
        }

        // 系统内置参数不允许修改键名，但允许修改键值
        if (entity.ConfigType == 1)
        {
            _logger.LogWarning("修改系统内置参数：{ConfigKey}", entity.ConfigKey);
        }

        entity.ConfigName = dto.ConfigName;
        entity.ConfigValue = dto.ConfigValue;
        entity.Status = (Models.Enums.Status)dto.Status;
        entity.Remark = dto.Remark;
        entity.UpdatedAt = DateTime.UtcNow;

        await _db.Updateable(entity).ExecuteCommandAsync();

        _logger.LogInformation("更新系统参数成功：{ConfigKey}, ID: {Id}", entity.ConfigKey, entity.Id);

        return true;
    }

    /// <summary>
    /// 删除系统参数
    /// </summary>
    /// <param name="id">参数ID</param>
    /// <returns>是否成功</returns>
    public async Task<bool> DeleteConfigAsync(string id)
    {
        var entity = await _db.Queryable<basic_config>()
            .Where(x => x.Id.ToString() == id && x.IsDeleted == 0)
            .FirstAsync();

        if (entity == null)
        {
            throw new BusinessException("系统参数不存在", 404);
        }

        // 系统内置参数不允许删除
        if (entity.ConfigType == 1)
        {
            throw new BusinessException("系统内置参数不允许删除");
        }

        // 软删除
        entity.IsDeleted = 1;
        entity.UpdatedAt = DateTime.UtcNow;

        await _db.Updateable(entity).ExecuteCommandAsync();

        _logger.LogInformation("删除系统参数成功：{ConfigKey}, ID: {Id}", entity.ConfigKey, entity.Id);

        return true;
    }
}