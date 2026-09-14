namespace EasyProduct.Models.Dto.Report.ColumnTemplate;

/// <summary>
/// 报表列模板DTO
/// </summary>
/// <remarks>
/// 用于返回报表列模板信息，包含所有字段
/// Sortable 字段在 DTO 中转换为 bool 类型（实体中为 int）
/// </remarks>
public class RptColumnTemplateDto
{
    /// <summary>
    /// 主键ID
    /// </summary>
    /// <remarks>
    /// GUID 字符串格式
    /// </remarks>
    public string Id { get; set; } = null!;

    /// <summary>
    /// 模板名称
    /// </summary>
    /// <remarks>
    /// 列模板的显示名称，如"商品名称"、"销售数量"
    /// </remarks>
    public string Name { get; set; } = null!;

    /// <summary>
    /// 字段名
    /// </summary>
    /// <remarks>
    /// 数据库字段名或计算字段名，用于数据绑定
    /// </remarks>
    public string Field { get; set; } = null!;

    /// <summary>
    /// 字段类型
    /// </summary>
    /// <remarks>
    /// 支持 string（字符串）、number（数字）、date（日期）、currency（货币）
    /// </remarks>
    public string Type { get; set; } = null!;

    /// <summary>
    /// 列宽
    /// </summary>
    /// <remarks>
    /// 列的显示宽度，单位为像素，默认 150
    /// </remarks>
    public int Width { get; set; }

    /// <summary>
    /// 格式化规则
    /// </summary>
    /// <remarks>
    /// 数据格式化规则，如日期格式 "yyyy-MM-dd"，数字格式 "#,##0.00"
    /// </remarks>
    public string? Format { get; set; }

    /// <summary>
    /// 是否可排序
    /// </summary>
    /// <remarks>
    /// 标识该列是否支持排序功能
    /// </remarks>
    public bool Sortable { get; set; }

    /// <summary>
    /// 备注说明
    /// </summary>
    /// <remarks>
    /// 字段的补充说明或使用提示
    /// </remarks>
    public string? Remark { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreateTime { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime? UpdateTime { get; set; }
}