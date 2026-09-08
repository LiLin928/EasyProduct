using SqlSugar;
using EasyProduct.Models.Entitys.Base;

namespace EasyProduct.Models.Entitys.Ops;

/// <summary>
/// 操作日志实体
/// </summary>
/// <remarks>
/// 对应数据库表 ops_operate_log
/// 继承 BaseEntity 基类，自动包含 Id、IsDeleted、Status、CreatedAt、UpdatedAt、CreatedBy、UpdatedBy 字段
/// 用于记录用户操作日志，包括操作时间、IP、请求参数等信息
/// </remarks>
[SugarTable("ops_operate_log", "操作日志表")]
public class ops_operate_log : BaseEntity
{
    /// <summary>
    /// 操作模块
    /// </summary>
    /// <remarks>
    /// 如：用户管理、角色管理等
    /// </remarks>
    [SugarColumn(Length = 50, ColumnDescription = "操作模块")]
    public string Module { get; set; } = string.Empty;

    /// <summary>
    /// 操作对象
    /// </summary>
    /// <remarks>
    /// 如：用户、角色等
    /// </remarks>
    [SugarColumn(Length = 100, IsNullable = true, ColumnDescription = "操作对象")]
    public string? Target { get; set; }

    /// <summary>
    /// 操作类型
    /// </summary>
    /// <remarks>
    /// 如：新增、修改、删除、查询等
    /// </remarks>
    [SugarColumn(Length = 50, ColumnDescription = "操作类型")]
    public string Action { get; set; } = string.Empty;

    /// <summary>
    /// HTTP 请求方法
    /// </summary>
    [SugarColumn(Length = 10, ColumnDescription = "HTTP请求方法")]
    public string Method { get; set; } = string.Empty;

    /// <summary>
    /// 请求URL
    /// </summary>
    [SugarColumn(Length = 500, ColumnDescription = "请求URL")]
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// 请求参数
    /// </summary>
    [SugarColumn(ColumnDataType = "TEXT", IsNullable = true, ColumnDescription = "请求参数")]
    public string? Params { get; set; }

    /// <summary>
    /// IP地址
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = true, ColumnDescription = "IP地址")]
    public string? Ip { get; set; }

    /// <summary>
    /// 用户代理
    /// </summary>
    [SugarColumn(Length = 500, IsNullable = true, ColumnDescription = "用户代理")]
    public string? UserAgent { get; set; }

    /// <summary>
    /// 操作用户ID
    /// </summary>
    [SugarColumn(ColumnDataType = "varchar(36)", ColumnDescription = "操作用户ID")]
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// 操作用户名
    /// </summary>
    [SugarColumn(Length = 50, ColumnDescription = "操作用户名")]
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// 操作状态
    /// </summary>
    /// <remarks>
    /// 0=失败，1=成功
    /// </remarks>
    [SugarColumn(ColumnDescription = "操作状态：0=失败，1=成功")]
    public int OperateStatus { get; set; } = 1;

    /// <summary>
    /// 错误信息
    /// </summary>
    [SugarColumn(ColumnDataType = "TEXT", IsNullable = true, ColumnDescription = "错误信息")]
    public string? ErrorMsg { get; set; }

    /// <summary>
    /// 执行时长（毫秒）
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "执行时长（毫秒）")]
    public int? Duration { get; set; }

    /// <summary>
    /// 追踪ID
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = true, ColumnDescription = "追踪ID")]
    public string? TraceId { get; set; }
}