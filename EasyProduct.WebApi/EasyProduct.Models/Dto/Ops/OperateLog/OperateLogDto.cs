namespace EasyProduct.Models.Dto.Ops.OperateLog;

/// <summary>
/// 操作日志 DTO
/// </summary>
/// <remarks>
/// 用于展示操作日志详细信息，包括操作人、操作内容、请求参数等
/// </remarks>
public class OperateLogDto
{
    /// <summary>
    /// 日志ID
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// 操作模块
    /// </summary>
    /// <remarks>
    /// 如：用户管理、角色管理等
    /// </remarks>
    public string Module { get; set; } = string.Empty;

    /// <summary>
    /// 操作对象
    /// </summary>
    /// <remarks>
    /// 如：用户、角色等
    /// </remarks>
    public string? Target { get; set; }

    /// <summary>
    /// 操作类型
    /// </summary>
    /// <remarks>
    /// 如：新增、修改、删除、查询等
    /// </remarks>
    public string Action { get; set; } = string.Empty;

    /// <summary>
    /// HTTP 请求方法
    /// </summary>
    public string Method { get; set; } = string.Empty;

    /// <summary>
    /// 请求URL
    /// </summary>
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// 请求参数
    /// </summary>
    public string? Params { get; set; }

    /// <summary>
    /// IP地址
    /// </summary>
    public string? Ip { get; set; }

    /// <summary>
    /// 用户代理
    /// </summary>
    public string? UserAgent { get; set; }

    /// <summary>
    /// 操作用户ID
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// 操作用户名
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// 操作状态
    /// </summary>
    /// <remarks>
    /// 0=失败，1=成功
    /// </remarks>
    public int OperateStatus { get; set; }

    /// <summary>
    /// 错误信息
    /// </summary>
    public string? ErrorMsg { get; set; }

    /// <summary>
    /// 执行时长（毫秒）
    /// </summary>
    public int? Duration { get; set; }

    /// <summary>
    /// 追踪ID
    /// </summary>
    public string? TraceId { get; set; }

    /// <summary>
    /// 创建时间（操作时间）
    /// </summary>
    public DateTime CreatedAt { get; set; }
}