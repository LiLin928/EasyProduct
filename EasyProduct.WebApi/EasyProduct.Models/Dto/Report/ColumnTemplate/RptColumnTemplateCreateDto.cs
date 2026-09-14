using System.ComponentModel.DataAnnotations;

namespace EasyProduct.Models.Dto.Report.ColumnTemplate;

/// <summary>
/// 创建列模板参数
/// </summary>
public class RptColumnTemplateCreateDto
{
    /// <summary>
    /// 模板名称
    /// </summary>
    [Required(ErrorMessage = "模板名称不能为空")]
    [MaxLength(100, ErrorMessage = "模板名称不能超过100个字符")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 字段名
    /// </summary>
    [Required(ErrorMessage = "字段名不能为空")]
    [MaxLength(50, ErrorMessage = "字段名不能超过50个字符")]
    public string Field { get; set; } = string.Empty;

    /// <summary>
    /// 字段类型
    /// </summary>
    [Required(ErrorMessage = "字段类型不能为空")]
    public string Type { get; set; } = "string";

    /// <summary>
    /// 列宽
    /// </summary>
    [Range(50, 500, ErrorMessage = "列宽必须在50-500之间")]
    public int Width { get; set; } = 150;

    /// <summary>
    /// 格式化规则
    /// </summary>
    [MaxLength(50, ErrorMessage = "格式化规则不能超过50个字符")]
    public string? Format { get; set; }

    /// <summary>
    /// 是否可排序
    /// </summary>
    public bool Sortable { get; set; } = true;

    /// <summary>
    /// 备注说明
    /// </summary>
    [MaxLength(500, ErrorMessage = "备注不能超过500个字符")]
    public string? Remark { get; set; }
}