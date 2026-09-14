using EasyProduct.Models.Dto.Common;

namespace EasyProduct.Models.Dto.Report.Definition;

/// <summary>
/// 报表定义查询参数
/// </summary>
public class RptDefinitionQuery : PageQuery
{
    /// <summary>
    /// 报表名称（模糊搜索）
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// 报表编码
    /// </summary>
    public string? Code { get; set; }

    /// <summary>
    /// 报表分类
    /// </summary>
    public string? Category { get; set; }

    /// <summary>
    /// 数据源ID
    /// </summary>
    public string? DatasourceId { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    public int? Status { get; set; }
}