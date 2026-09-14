using System.ComponentModel.DataAnnotations;
using EasyProduct.Models.Dto.Common;

namespace EasyProduct.Models.Dto.Report.ColumnTemplate;

/// <summary>
/// 报表列模板查询参数
/// </summary>
/// <remarks>
/// 用于报表列模板列表查询，支持按模板名称、字段类型筛选
/// 包含分页参数（继承自 PageQuery）
/// </remarks>
public class RptColumnTemplateQuery : PageQuery
{
    /// <summary>
    /// 模板名称
    /// </summary>
    /// <remarks>
    /// 支持模糊搜索，最大长度 100 个字符
    /// </remarks>
    [MaxLength(100, ErrorMessage = "模板名称长度不能超过100个字符")]
    public string? Name { get; set; }

    /// <summary>
    /// 字段类型
    /// </summary>
    /// <remarks>
    /// 支持的类型：string（字符串）、number（数字）、date（日期）、currency（货币）
    /// </remarks>
    [MaxLength(20, ErrorMessage = "字段类型长度不能超过20个字符")]
    public string? Type { get; set; }
}