using System.ComponentModel.DataAnnotations;

namespace EasyProduct.Models.Dto.Basic.Dict;

/// <summary>
/// 创建字典类型参数
/// </summary>
public class CreateDictTypeDto
{
    /// <summary>
    /// 字典名称
    /// </summary>
    [Required(ErrorMessage = "字典名称不能为空")]
    [MaxLength(100, ErrorMessage = "字典名称不能超过100个字符")]
    public string DictName { get; set; } = string.Empty;

    /// <summary>
    /// 字典类型（唯一标识）
    /// </summary>
    [Required(ErrorMessage = "字典类型不能为空")]
    [MaxLength(100, ErrorMessage = "字典类型不能超过100个字符")]
    [RegularExpression(@"^[a-z_]+$", ErrorMessage = "字典类型只能包含小写字母和下划线")]
    public string DictType { get; set; } = string.Empty;

    /// <summary>
    /// 状态：0=禁用，1=启用
    /// </summary>
    [Range(0, 1, ErrorMessage = "状态值只能是0或1")]
    public int Status { get; set; } = 1;

    /// <summary>
    /// 备注
    /// </summary>
    [MaxLength(500)]
    public string? Remark { get; set; }
}

/// <summary>
/// 创建字典数据参数
/// </summary>
public class CreateDictDataDto
{
    /// <summary>
    /// 字典类型
    /// </summary>
    [Required(ErrorMessage = "字典类型不能为空")]
    [MaxLength(100)]
    public string DictType { get; set; } = string.Empty;

    /// <summary>
    /// 字典标签
    /// </summary>
    [Required(ErrorMessage = "字典标签不能为空")]
    [MaxLength(100, ErrorMessage = "字典标签不能超过100个字符")]
    public string DictLabel { get; set; } = string.Empty;

    /// <summary>
    /// 字典值
    /// </summary>
    [Required(ErrorMessage = "字典值不能为空")]
    [MaxLength(100, ErrorMessage = "字典值不能超过100个字符")]
    public string DictValue { get; set; } = string.Empty;

    /// <summary>
    /// 排序
    /// </summary>
    [Range(0, int.MaxValue, ErrorMessage = "排序值必须大于等于0")]
    public int Sort { get; set; } = 0;

    /// <summary>
    /// 状态：0=禁用，1=启用
    /// </summary>
    [Range(0, 1, ErrorMessage = "状态值只能是0或1")]
    public int Status { get; set; } = 1;

    /// <summary>
    /// 备注
    /// </summary>
    [MaxLength(500)]
    public string? Remark { get; set; }
}