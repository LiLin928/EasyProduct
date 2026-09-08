namespace EasyProduct.Models.Dto.Ops.OperateLog;

/// <summary>
/// 操作日志查询参数
/// </summary>
/// <remarks>
/// 用于操作日志列表的分页查询，支持多条件筛选
/// </remarks>
public class OperateLogQueryDto
{
    /// <summary>
    /// 页码，从 1 开始
    /// </summary>
    public int PageIndex { get; set; } = 1;

    /// <summary>
    /// 每页数量
    /// </summary>
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// 操作模块（模糊搜索）
    /// </summary>
    public string? Module { get; set; }

    /// <summary>
    /// 操作类型（模糊搜索）
    /// </summary>
    public string? Action { get; set; }

    /// <summary>
    /// 操作用户名（模糊搜索）
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// 操作状态：0=失败，1=成功
    /// </summary>
    public int? OperateStatus { get; set; }

    /// <summary>
    /// 开始时间（操作时间范围查询）
    /// </summary>
    public DateTime? StartTime { get; set; }

    /// <summary>
    /// 结束时间（操作时间范围查询）
    /// </summary>
    public DateTime? EndTime { get; set; }
}