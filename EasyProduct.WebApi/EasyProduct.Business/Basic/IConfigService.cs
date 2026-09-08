using EasyProduct.Common.Base;
using EasyProduct.Models.Dto.Basic.Config;

namespace EasyProduct.Business.Basic;

/// <summary>
/// 系统参数服务接口
/// </summary>
/// <remarks>
/// 提供系统参数的增删改查、根据键名获取值等功能
/// </remarks>
public interface IConfigService
{
    /// <summary>
    /// 获取系统参数列表
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>系统参数列表</returns>
    Task<List<ConfigDto>> GetConfigListAsync(ConfigQueryDto query);

    /// <summary>
    /// 获取系统参数详情
    /// </summary>
    /// <param name="id">参数ID</param>
    /// <returns>系统参数详情</returns>
    Task<ConfigDto> GetConfigByIdAsync(string id);

    /// <summary>
    /// 根据参数键名获取参数值
    /// </summary>
    /// <param name="configKey">参数键名</param>
    /// <returns>参数键值</returns>
    Task<string> GetConfigValueByKeyAsync(string configKey);

    /// <summary>
    /// 创建系统参数
    /// </summary>
    /// <param name="dto">创建参数</param>
    /// <returns>新参数ID</returns>
    Task<string> CreateConfigAsync(CreateConfigDto dto);

    /// <summary>
    /// 更新系统参数
    /// </summary>
    /// <param name="dto">更新参数</param>
    /// <returns>是否成功</returns>
    Task<bool> UpdateConfigAsync(UpdateConfigDto dto);

    /// <summary>
    /// 删除系统参数
    /// </summary>
    /// <param name="id">参数ID</param>
    /// <returns>是否成功</returns>
    Task<bool> DeleteConfigAsync(string id);
}