using EasyProduct.Business.Basic;
using EasyProduct.Common.Base;
using EasyProduct.Models.Dto.Basic.Config;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyProduct.Web.Controllers.Admin.Basic;

/// <summary>
/// 系统参数管理控制器
/// </summary>
/// <remarks>
/// 提供系统参数的增删改查、根据键名获取值功能
/// 管理端接口，需要 Admin JWT 认证
/// </remarks>
[ApiController]
[Route("api/admin/basic/config")]
[Authorize(AuthenticationSchemes = "AdminJwt")]
public class ConfigController : BaseController
{
    private readonly IConfigService _configService;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="configService">系统参数服务</param>
    public ConfigController(IConfigService configService)
    {
        _configService = configService;
    }

    /// <summary>
    /// 获取系统参数列表
    /// </summary>
    /// <param name="configName">参数名称（模糊搜索）</param>
    /// <param name="configKey">参数键名（模糊搜索）</param>
    /// <param name="configType">系统内置：0=否，1=是</param>
    /// <param name="status">状态：0=禁用，1=启用</param>
    /// <returns>系统参数列表</returns>
    /// <remarks>
    /// 获取系统参数的列表，支持按名称、键名、内置标记、状态筛选
    /// </remarks>
    [HttpGet("list")]
    public async Task<ApiResponse<List<ConfigDto>>> GetConfigList(
        [FromQuery] string? configName,
        [FromQuery] string? configKey,
        [FromQuery] int? configType,
        [FromQuery] int? status)
    {
        var query = new ConfigQueryDto
        {
            ConfigName = configName,
            ConfigKey = configKey,
            ConfigType = configType,
            Status = status
        };

        var result = await _configService.GetConfigListAsync(query);
        return Success(result);
    }

    /// <summary>
    /// 获取系统参数详情
    /// </summary>
    /// <param name="id">参数ID</param>
    /// <returns>系统参数详情</returns>
    /// <remarks>
    /// 根据ID获取系统参数的详细信息
    /// </remarks>
    [HttpGet("{id}")]
    public async Task<ApiResponse<ConfigDto>> GetConfigById(string id)
    {
        var result = await _configService.GetConfigByIdAsync(id);
        return Success(result!);
    }

    /// <summary>
    /// 根据参数键名获取参数值
    /// </summary>
    /// <param name="configKey">参数键名</param>
    /// <returns>参数键值</returns>
    /// <remarks>
    /// 根据参数键名获取启用的参数值，用于前端获取系统配置
    /// </remarks>
    [HttpGet("key/{configKey}")]
    [AllowAnonymous] // 允许匿名访问，前端需要获取系统配置
    public async Task<ApiResponse<string>> GetConfigValueByKey(string configKey)
    {
        var result = await _configService.GetConfigValueByKeyAsync(configKey);
        return Success<string>(result);
    }

    /// <summary>
    /// 创建系统参数
    /// </summary>
    /// <param name="dto">创建参数</param>
    /// <returns>新参数ID</returns>
    /// <remarks>
    /// 创建新的系统参数
    /// </remarks>
    [HttpPost]
    public async Task<ApiResponse<string>> CreateConfig([FromBody] CreateConfigDto dto)
    {
        var result = await _configService.CreateConfigAsync(dto);
        return Success(result, "系统参数创建成功");
    }

    /// <summary>
    /// 更新系统参数
    /// </summary>
    /// <param name="id">参数ID</param>
    /// <param name="dto">更新参数</param>
    /// <returns>是否成功</returns>
    /// <remarks>
    /// 更新系统参数信息
    /// 系统内置参数允许修改键值，但不允许删除
    /// </remarks>
    [HttpPut("{id}")]
    public async Task<ApiResponse<bool>> UpdateConfig(string id, [FromBody] UpdateConfigDto dto)
    {
        dto.Id = id;
        var result = await _configService.UpdateConfigAsync(dto);
        return Success(result, "系统参数更新成功");
    }

    /// <summary>
    /// 删除系统参数
    /// </summary>
    /// <param name="id">参数ID</param>
    /// <returns>是否成功</returns>
    /// <remarks>
    /// 删除系统参数（软删除）
    /// 注意：系统内置参数不允许删除
    /// </remarks>
    [HttpDelete("{id}")]
    public async Task<ApiResponse<bool>> DeleteConfig(string id)
    {
        var result = await _configService.DeleteConfigAsync(id);
        return Success(result, "系统参数删除成功");
    }
}