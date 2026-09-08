using System.ComponentModel.DataAnnotations;

namespace EasyProduct.Models.Dto.Basic.Config;

/// <summary>
/// 创建系统参数参数
/// </summary>
public class CreateConfigDto
{
    /// <summary>
    /// 参数名称
    /// </summary>
    [Required(ErrorMessage = "参数名称不能为空")]
    [MaxLength(100, ErrorMessage = "参数名称不能超过100个字符")]
    public string ConfigName { get; set; } = string.Empty;

    /// <summary>
    /// 参数键名（唯一标识）
    /// </summary>
    [Required(ErrorMessage = "参数键名不能为空")]
    [MaxLength(100, ErrorMessage = "参数键名不能超过100个字符")]
    [RegularExpression(@"^[a-z_]+$", ErrorMessage = "参数键名只能包含小写字母和下划线")]
    public string ConfigKey { get; set; } = string.Empty;

    /// <summary>
    /// 参数键值
    /// </summary>
    [Required(ErrorMessage = "参数键值不能为空")]
    [MaxLength(500, ErrorMessage = "参数键值不能超过500个字符")]
    public string ConfigValue { get; set; } = string.Empty;

    /// <summary>
    /// 系统内置：0=否，1=是
    /// </summary>
    [Range(0, 1, ErrorMessage = "系统内置标记只能是0或1")]
    public int ConfigType { get; set; } = 0;

    /// <summary>
    /// 状态：0=禁用，1=启用
    /// </summary>
    [Range(0, 1, ErrorMessage = "状态值只能是0或1")]
    public int Status { get; set; } = 1;

    /// <summary>
    /// 备注
    /// </summary>
    [MaxLength(500, ErrorMessage = "备注不能超过500个字符")]
    public string? Remark { get; set; }
}