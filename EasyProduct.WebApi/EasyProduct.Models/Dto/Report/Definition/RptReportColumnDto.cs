namespace EasyProduct.Models.Dto.Report.Definition;

/// <summary>
/// 报表列配置
/// </summary>
public class RptReportColumnDto
{
    /// <summary>
    /// 字段名
    /// </summary>
    public string Field { get; set; } = string.Empty;

    /// <summary>
    /// 列标题
    /// </summary>
    public string Label { get; set; } = string.Empty;

    /// <summary>
    /// 字段类型：string/number/date/currency
    /// </summary>
    public string Type { get; set; } = "string";

    /// <summary>
    /// 列宽
    /// </summary>
    public int Width { get; set; } = 150;

    /// <summary>
    /// 格式化规则
    /// </summary>
    public string? Format { get; set; }

    /// <summary>
    /// 是否可排序
    /// </summary>
    public bool Sortable { get; set; } = true;
}