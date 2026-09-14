using System.ComponentModel.DataAnnotations;

namespace EasyProduct.Models.Dto.Report.Definition;

/// <summary>
/// 更新报表定义参数
/// </summary>
public class RptDefinitionUpdateDto
{
    /// <summary>
    /// 报表名称
    /// </summary>
    [Required(ErrorMessage = "报表名称不能为空")]
    [MaxLength(100, ErrorMessage = "报表名称不能超过100个字符")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 报表编码
    /// </summary>
    [Required(ErrorMessage = "报表编码不能为空")]
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// 报表分类
    /// </summary>
    [Required(ErrorMessage = "报表分类不能为空")]
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// 数据源ID
    /// </summary>
    [Required(ErrorMessage = "数据源不能为空")]
    public string DatasourceId { get; set; } = string.Empty;

    /// <summary>
    /// 查询SQL
    /// </summary>
    [Required(ErrorMessage = "查询SQL不能为空")]
    public string SqlTemplate { get; set; } = string.Empty;

    /// <summary>
    /// 图表类型
    /// </summary>
    public string ChartType { get; set; } = "table";

    /// <summary>
    /// 列配置
    /// </summary>
    public List<RptReportColumnDto> Columns { get; set; } = new();

    /// <summary>
    /// 状态
    /// </summary>
    public int Status { get; set; } = 1;

    /// <summary>
    /// 备注说明
    /// </summary>
    [MaxLength(500, ErrorMessage = "备注不能超过500个字符")]
    public string? Remark { get; set; }
}