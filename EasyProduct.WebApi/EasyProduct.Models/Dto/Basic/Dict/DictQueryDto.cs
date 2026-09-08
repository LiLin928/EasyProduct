using System.ComponentModel.DataAnnotations;

namespace EasyProduct.Models.Dto.Basic.Dict;

/// <summary>
/// 字典类型查询参数
/// </summary>
public class DictTypeQueryDto
{
    /// <summary>
    /// 字典名称（模糊搜索）
    /// </summary>
    [MaxLength(100)]
    public string? DictName { get; set; }

    /// <summary>
    /// 字典类型（模糊搜索）
    /// </summary>
    [MaxLength(100)]
    public string? DictType { get; set; }

    /// <summary>
    /// 状态：0=禁用，1=启用
    /// </summary>
    public int? Status { get; set; }
}

/// <summary>
/// 字典数据查询参数
/// </summary>
public class DictDataQueryDto
{
    /// <summary>
    /// 字典类型（必填）
    /// </summary>
    [Required(ErrorMessage = "字典类型不能为空")]
    [MaxLength(100)]
    public string DictType { get; set; } = string.Empty;

    /// <summary>
    /// 字典标签（模糊搜索）
    /// </summary>
    [MaxLength(100)]
    public string? DictLabel { get; set; }

    /// <summary>
    /// 状态：0=禁用，1=启用
    /// </summary>
    public int? Status { get; set; }
}