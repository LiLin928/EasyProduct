using System.ComponentModel.DataAnnotations;

namespace EasyProduct.Models.Dto.Report.ColumnTemplate;

/// <summary>
/// 报表列模板更新DTO
/// </summary>
/// <remarks>
/// 用于更新报表列模板记录
/// 所有字段均为可选，只更新传入的字段
/// </remarks>
public class RptColumnTemplateUpdateDto
{
    /// <summary>
    /// 模板名称
    /// </summary>
    /// <remarks>
    /// 列模板的显示名称，如"商品名称"、"销售数量"
    /// 最大长度 100 个字符
    /// </remarks>
    [MaxLength(100, ErrorMessage = "模板名称长度不能超过100个字符")]
    public string? Name { get; set; }

    /// <summary>
    /// 字段名
    /// </summary>
    /// <remarks>
    /// 数据库字段名或计算字段名，用于数据绑定
    /// 最大长度 50 个字符
    /// </remarks>
    [MaxLength(50, ErrorMessage = "字段名长度不能超过50个字符")]
    public string? Field { get; set; }

    /// <summary>
    /// 字段类型
    /// </summary>
    /// <remarks>
    /// 支持的类型：string（字符串）、number（数字）、date（日期）、currency（货币）
    /// 最大长度 20 个字符
    /// </remarks>
    [MaxLength(20, ErrorMessage = "字段类型长度不能超过20个字符")]
    public string? Type { get; set; }

    /// <summary>
    /// 列宽
    /// </summary>
    /// <remarks>
    /// 列的显示宽度，单位为像素
    /// 范围：50-500
    /// </remarks>
    [Range(50, 500, ErrorMessage = "列宽必须在50到500之间")]
    public int? Width { get; set; }

    /// <summary>
    /// 格式化规则
    /// </summary>
    /// <remarks>
    /// 数据格式化规则，如日期格式 "yyyy-MM-dd"，数字格式 "#,##0.00"
    /// 最大长度 50 个字符
    /// </remarks>
    [MaxLength(50, ErrorMessage = "格式化规则长度不能超过50个字符")]
    public string? Format { get; set; }

    /// <summary>
    /// 是否可排序
    /// </summary>
    /// <remarks>
    /// 标识该列是否支持排序功能
    /// </remarks>
    public bool? Sortable { get; set; }

    /// <summary>
    /// 备注说明
    /// </summary>
    /// <remarks>
    /// 字段的补充说明或使用提示
    /// 最大长度 500 个字符
    /// </remarks>
    [MaxLength(500, ErrorMessage = "备注说明长度不能超过500个字符")]
    public string? Remark { get; set; }
}