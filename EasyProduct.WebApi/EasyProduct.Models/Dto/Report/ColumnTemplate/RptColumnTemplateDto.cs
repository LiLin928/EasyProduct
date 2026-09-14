namespace EasyProduct.Models.Dto.Report.ColumnTemplate;

/// <summary>
/// 列模板详情
/// </summary>
public class RptColumnTemplateDto
{
    /// <summary>
    /// 主键ID
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// 模板名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 字段名
    /// </summary>
    public string Field { get; set; } = string.Empty;

    /// <summary>
    /// 字段类型
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// 列宽
    /// </summary>
    public int Width { get; set; }

    /// <summary>
    /// 格式化规则
    /// </summary>
    public string? Format { get; set; }

    /// <summary>
    /// 是否可排序
    /// </summary>
    public bool Sortable { get; set; }

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