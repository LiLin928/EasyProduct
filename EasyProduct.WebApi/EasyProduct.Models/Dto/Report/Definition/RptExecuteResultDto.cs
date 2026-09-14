namespace EasyProduct.Models.Dto.Report.Definition;

/// <summary>
/// 报表执行结果
/// </summary>
public class RptExecuteResultDto
{
    /// <summary>
    /// 列配置
    /// </summary>
    public List<RptReportColumnDto> Columns { get; set; } = new();

    /// <summary>
    /// 数据行
    /// </summary>
    public List<Dictionary<string, object>> Rows { get; set; } = new();

    /// <summary>
    /// 总条数
    /// </summary>
    public int Total { get; set; }
}