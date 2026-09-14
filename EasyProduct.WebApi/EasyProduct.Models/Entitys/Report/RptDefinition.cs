using System.ComponentModel.DataAnnotations;
using EasyProduct.Models.Constants;
using EasyProduct.Models.Entitys.Base;
using SqlSugar;

namespace EasyProduct.Models.Entitys.Report;

/// <summary>
/// 报表定义
/// </summary>
[SugarTable("rpt_definition", "报表定义表")]
public class RptDefinition : BaseEntity
{
    /// <summary>
    /// 报表名称
    /// </summary>
    [SugarColumn(Length = 100)]
    [Required(ErrorMessage = "报表名称不能为空")]
    [MaxLength(100, ErrorMessage = "报表名称不能超过100个字符")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 报表编码（唯一标识）
    /// </summary>
    [SugarColumn(Length = 50)]
    [Required(ErrorMessage = "报表编码不能为空")]
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// 报表分类
    /// </summary>
    [SugarColumn(Length = 50)]
    [Required(ErrorMessage = "报表分类不能为空")]
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// 数据源ID
    /// </summary>
    [Required(ErrorMessage = "数据源不能为空")]
    public Guid DatasourceId { get; set; }

    /// <summary>
    /// 查询SQL（支持参数化）
    /// </summary>
    [SugarColumn(ColumnDataType = "text")]
    [Required(ErrorMessage = "查询SQL不能为空")]
    public string SqlTemplate { get; set; } = string.Empty;

    /// <summary>
    /// 图表类型：table/line/bar/pie
    /// </summary>
    [SugarColumn(Length = 20)]
    public string ChartType { get; set; } = ReportConstants.ChartType.Table;

    /// <summary>
    /// 列定义（JSON数组）
    /// </summary>
    [SugarColumn(ColumnDataType = "text")]
    public string Columns { get; set; } = "[]";

    /// <summary>
    /// 筛选条件（JSON数组）
    /// </summary>
    [SugarColumn(ColumnDataType = "text")]
    public string Filters { get; set; } = "[]";

    /// <summary>
    /// 状态：1-草稿，2-已发布，3-已归档
    /// </summary>
    [SugarColumn(ColumnDescription = "状态")]
    public int Status { get; set; } = ReportConstants.ReportStatus.Draft;

    /// <summary>
    /// 备注说明
    /// </summary>
    [SugarColumn(Length = 500, IsNullable = true)]
    public string? Remark { get; set; }
}