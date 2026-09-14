using System.ComponentModel.DataAnnotations;

namespace EasyProduct.Models.Dto.Report.ColumnTemplate;

/// <summary>
/// 报表列模板创建DTO
/// </summary>
/// <remarks>
/// 用于创建新的报表列模板记录
/// 包含必填字段验证和数据格式验证
/// </remarks>
public class RptColumnTemplateCreateDto
{
    /// <summary>
    /// 模板名称
    /// </summary>
    /// <remarks>
    /// 列模板的显示名称，如"商品名称"、"销售数量"
    /// 必填，最大长度 100 个字符
    /// </remarks>
    [Required(ErrorMessage = "模板名称不能为空")]
    [MaxLength(100, ErrorMessage = "模板名称长度不能超过100个字符")]
    public string Name { get; set; } = null!;

    /// <summary>
    /// 字段名
    /// </summary>
    /// <remarks>
    /// 数据库字段名或计算字段名，用于数据绑定
    /// 必填，最大长度 50 个字符
    /// </remarks>
    [Required(ErrorMessage = "字段名不能为空")]
    [MaxLength(50, ErrorMessage = "字段名长度不能超过50个字符")]
    public string Field { get; set; } = null!;

    /// <summary>
    /// 字段类型
    /// </summary>
    /// <remarks>
    /// 支持的类型：string（字符串）、number（数字）、date（日期）、currency（货币）
    /// 必填，最大长度 20 个字符
    /// </remarks>
    [Required(ErrorMessage = "字段类型不能为空")]
    [MaxLength(20, ErrorMessage = "字段类型长度不能超过20个字符")]
    public string Type { get; set; } = "string";

    /// <summary>
    /// 列宽
    /// </summary>
    /// <remarks>
    /// 列的显示宽度，单位为像素
    /// 范围：50-500，默认 150
    /// </remarks>
    [Range(50, 500, ErrorMessage = "列宽必须在50到500之间")]
    public int Width { get; set; } = 150;

    /// <summary>
    /// 格式化规则
    /// </summary>
    /// <remarks>
    /// 数据格式化规则，如日期格式 "yyyy-MM-dd"，数字格式 "#,##0.00"
    /// 可选，最大长度 50 个字符
    /// </remarks>
    [MaxLength(50, ErrorMessage = "格式化规则长度不能超过50个字符")]
    public string? Format { get; set; }

    /// <summary>
    /// 是否可排序
    /// </summary>
    /// <remarks>
    /// 标识该列是否支持排序功能
    /// 默认为 true（可排序）
    /// </remarks>
    public bool Sortable { get; set; } = true;

    /// <summary>
    /// 备注说明
    /// </summary>
    /// <remarks>
    /// 字段的补充说明或使用提示
    /// 可选，最大长度 500 个字符
    /// </remarks>
    [MaxLength(500, ErrorMessage = "备注说明长度不能超过500个字符")]
    public string? Remark { get; set; }
}