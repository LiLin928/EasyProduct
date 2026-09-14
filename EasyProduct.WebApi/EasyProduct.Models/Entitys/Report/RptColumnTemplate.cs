using System.ComponentModel.DataAnnotations;
using EasyProduct.Models.Entitys.Base;
using SqlSugar;

namespace EasyProduct.Models.Entitys.Report;

/// <summary>
/// 报表列模板
/// </summary>
[SugarTable("rpt_column_template", "报表列模板表")]
public class RptColumnTemplate : BaseEntity
{
    /// <summary>
    /// 模板名称
    /// </summary>
    [SugarColumn(Length = 100)]
    [Required(ErrorMessage = "模板名称不能为空")]
    [MaxLength(100, ErrorMessage = "模板名称不能超过100个字符")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 字段名
    /// </summary>
    [SugarColumn(Length = 50)]
    [Required(ErrorMessage = "字段名不能为空")]
    public string Field { get; set; } = string.Empty;

    /// <summary>
    /// 字段类型：string/number/date/currency
    /// </summary>
    [SugarColumn(Length = 20)]
    [Required(ErrorMessage = "字段类型不能为空")]
    public string Type { get; set; } = "string";

    /// <summary>
    /// 列宽
    /// </summary>
    public int Width { get; set; } = 150;

    /// <summary>
    /// 格式化规则
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = true)]
    public string? Format { get; set; }

    /// <summary>
    /// 是否可排序：0-否，1-是
    /// </summary>
    public int Sortable { get; set; } = 1;

    /// <summary>
    /// 备注说明
    /// </summary>
    [SugarColumn(Length = 500, IsNullable = true)]
    public string? Remark { get; set; }
}