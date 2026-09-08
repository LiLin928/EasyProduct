namespace EasyProduct.Models.Dto.Basic.Config;

/// <summary>
/// 系统参数DTO
/// </summary>
public class ConfigDto
{
    /// <summary>
    /// 参数ID
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// 参数名称
    /// </summary>
    public string ConfigName { get; set; } = string.Empty;

    /// <summary>
    /// 参数键名
    /// </summary>
    public string ConfigKey { get; set; } = string.Empty;

    /// <summary>
    /// 参数键值
    /// </summary>
    public string ConfigValue { get; set; } = string.Empty;

    /// <summary>
    /// 系统内置：0=否，1=是
    /// </summary>
    public int ConfigType { get; set; }

    /// <summary>
    /// 状态：0=禁用，1=启用
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    public string? Remark { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// 创建人
    /// </summary>
    public string? CreatedBy { get; set; }
}