namespace EasyProduct.Models.Dto.Report.Datasource;

/// <summary>
/// 数据源连接测试结果
/// </summary>
public class RptConnectionTestResultDto
{
    /// <summary>
    /// 是否成功
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// 提示信息
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// 错误信息
    /// </summary>
    public string? ErrorMessage { get; set; }
}