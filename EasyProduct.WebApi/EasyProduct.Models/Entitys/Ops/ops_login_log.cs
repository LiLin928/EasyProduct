using SqlSugar;
using EasyProduct.Models.Entitys.Base;

namespace EasyProduct.Models.Entitys.Ops;

/// <summary>
/// 登录日志实体
/// </summary>
/// <remarks>
/// 对应数据库表 ops_login_log
/// 继承 BaseEntity 基类，自动包含 Id、IsDeleted、Status、CreatedAt、UpdatedAt、CreatedBy、UpdatedBy 字段
/// 用于记录用户登录日志，包括登录时间、IP、设备等信息
/// </remarks>
[SugarTable("ops_login_log", "登录日志表")]
public class ops_login_log : BaseEntity
{
    /// <summary>
    /// 用户ID
    /// </summary>
    /// <remarks>
    /// 登录用户的ID，关联 basic_user 表
    /// </remarks>
    [SugarColumn(ColumnDataType = "varchar(36)", ColumnDescription = "用户ID")]
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// 用户名
    /// </summary>
    /// <remarks>
    /// 登录用户名，冗余字段便于查询
    /// </remarks>
    [SugarColumn(Length = 50, ColumnDescription = "用户名")]
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// 登录IP地址
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = true, ColumnDescription = "登录IP地址")]
    public string? LoginIp { get; set; }

    /// <summary>
    /// 登录地点
    /// </summary>
    /// <remarks>
    /// 根据 IP 解析出的地理位置，可为空
    /// </remarks>
    [SugarColumn(Length = 200, IsNullable = true, ColumnDescription = "登录地点")]
    public string? LoginLocation { get; set; }

    /// <summary>
    /// 浏览器类型
    /// </summary>
    [SugarColumn(Length = 200, IsNullable = true, ColumnDescription = "浏览器类型")]
    public string? Browser { get; set; }

    /// <summary>
    /// 操作系统
    /// </summary>
    [SugarColumn(Length = 200, IsNullable = true, ColumnDescription = "操作系统")]
    public string? Os { get; set; }

    /// <summary>
    /// 登录状态
    /// </summary>
    /// <remarks>
    /// 0=失败，1=成功
    /// </remarks>
    [SugarColumn(ColumnDescription = "登录状态：0=失败，1=成功")]
    public int LoginStatus { get; set; } = 1;

    /// <summary>
    /// 登录消息
    /// </summary>
    /// <remarks>
    /// 登录结果描述，如"登录成功"、"密码错误"等
    /// </remarks>
    [SugarColumn(Length = 500, IsNullable = true, ColumnDescription = "登录消息")]
    public string? LoginMessage { get; set; }

    /// <summary>
    /// 登录时间
    /// </summary>
    /// <remarks>
    /// 用户登录的时间戳
    /// </remarks>
    [SugarColumn(ColumnDescription = "登录时间")]
    public DateTime LoginTime { get; set; } = DateTime.UtcNow;
}