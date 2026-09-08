namespace EasyProduct.Models.Dto.Ops.LoginLog;

/// <summary>
/// 登录日志查询参数
/// </summary>
/// <remarks>
/// 用于登录日志列表的分页查询，支持多条件筛选
/// </remarks>
public class LoginLogQueryDto
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
    /// 用户名（模糊搜索）
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// 登录IP（模糊搜索）
    /// </summary>
    public string? LoginIp { get; set; }

    /// <summary>
    /// 登录状态：0=失败，1=成功
    /// </summary>
    public int? LoginStatus { get; set; }

    /// <summary>
    /// 开始时间（登录时间范围查询）
    /// </summary>
    public DateTime? StartTime { get; set; }

    /// <summary>
    /// 结束时间（登录时间范围查询）
    /// </summary>
    public DateTime? EndTime { get; set; }
}