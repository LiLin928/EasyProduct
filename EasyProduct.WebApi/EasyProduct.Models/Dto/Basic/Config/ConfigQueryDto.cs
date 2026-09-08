namespace EasyProduct.Models.Dto.Basic.Config;

/// <summary>
/// 系统参数查询参数
/// </summary>
public class ConfigQueryDto
{
    /// <summary>
    /// 参数名称（模糊搜索）
    /// </summary>
    public string? ConfigName { get; set; }

    /// <summary>
    /// 参数键名（模糊搜索）
    /// </summary>
    public string? ConfigKey { get; set; }

    /// <summary>
    /// 系统内置：0=否，1=是
    /// </summary>
    public int? ConfigType { get; set; }

    /// <summary>
    /// 状态：0=禁用，1=启用
    /// </summary>
    public int? Status { get; set; }
}