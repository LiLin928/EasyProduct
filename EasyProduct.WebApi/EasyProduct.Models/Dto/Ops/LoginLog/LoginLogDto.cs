namespace EasyProduct.Models.Dto.Ops.LoginLog;

/// <summary>
/// 登录日志 DTO
/// </summary>
/// <remarks>
/// 用于展示登录日志详细信息，包括登录用户、登录IP、设备信息等
/// </remarks>
public class LoginLogDto
{
    /// <summary>
    /// 日志ID
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// 用户ID
    /// </summary>
    /// <remarks>
    /// 登录用户的ID，关联 basic_user 表
    /// </remarks>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// 用户名
    /// </summary>
    /// <remarks>
    /// 登录用户名，冗余字段便于查询
    /// </remarks>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// 登录IP地址
    /// </summary>
    public string? LoginIp { get; set; }

    /// <summary>
    /// 登录地点
    /// </summary>
    /// <remarks>
    /// 根据 IP 解析出的地理位置
    /// </remarks>
    public string? LoginLocation { get; set; }

    /// <summary>
    /// 浏览器类型
    /// </summary>
    public string? Browser { get; set; }

    /// <summary>
    /// 操作系统
    /// </summary>
    public string? Os { get; set; }

    /// <summary>
    /// 登录状态
    /// </summary>
    /// <remarks>
    /// 0=失败，1=成功
    /// </remarks>
    public int LoginStatus { get; set; }

    /// <summary>
    /// 登录消息
    /// </summary>
    /// <remarks>
    /// 登录结果描述，如"登录成功"、"密码错误"等
    /// </remarks>
    public string? LoginMessage { get; set; }

    /// <summary>
    /// 登录时间
    /// </summary>
    public DateTime LoginTime { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; }
}