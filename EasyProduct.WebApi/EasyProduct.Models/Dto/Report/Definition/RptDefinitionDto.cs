namespace EasyProduct.Models.Dto.Report.Definition;

/// <summary>
/// 报表定义详情
/// </summary>
public class RptDefinitionDto
{
    /// <summary>
    /// 主键ID
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// 报表名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 报表编码
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// 报表分类
    /// </summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// 数据源ID
    /// </summary>
    public string DatasourceId { get; set; } = string.Empty;

    /// <summary>
    /// 数据源名称
    /// </summary>
    public string DatasourceName { get; set; } = string.Empty;

    /// <summary>
    /// 查询SQL
    /// </summary>
    public string SqlTemplate { get; set; } = string.Empty;

    /// <summary>
    /// 图表类型
    /// </summary>
    public string ChartType { get; set; } = string.Empty;

    /// <summary>
    /// 列配置
    /// </summary>
    public List<RptReportColumnDto> Columns { get; set; } = new();

    /// <summary>
    /// 状态
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// 备注说明
    /// </summary>
    public string? Remark { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreateTime { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime UpdateTime { get; set; }
}