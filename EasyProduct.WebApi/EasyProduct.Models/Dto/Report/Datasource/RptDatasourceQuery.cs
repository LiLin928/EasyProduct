using EasyProduct.Models.Dto.Common;

namespace EasyProduct.Models.Dto.Report.Datasource;

/// <summary>
/// 数据源查询参数
/// </summary>
public class RptDatasourceQuery : PageQuery
{
    /// <summary>
    /// 数据源名称（模糊搜索）
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// 数据源类型
    /// </summary>
    public string? Type { get; set; }

    /// <summary>
    /// 连接状态
    /// </summary>
    public int? Status { get; set; }
}