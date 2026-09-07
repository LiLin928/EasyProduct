using SqlSugar;
using EasyProduct.Models.Entitys.Base;

namespace EasyProduct.Models.Entitys.Basic;

/// <summary>
/// 刷新令牌实体
/// </summary>
/// <remarks>
/// 对应数据库表 basic_refresh_token
/// 继承 BaseEntity 基类，自动包含 Id、IsDeleted、Status、CreatedAt、UpdatedAt、CreatedBy、UpdatedBy 字段
/// 用于存储用户的刷新令牌，支持令牌撤销和过期管理
/// </remarks>
[SugarTable("basic_refresh_token", "刷新令牌表")]
public class basic_refresh_token : BaseEntity
{
    /// <summary>
    /// 用户ID
    /// </summary>
    /// <remarks>
    /// 令牌所属用户的ID，关联 basic_user 表
    /// </remarks>
    [SugarColumn(ColumnDataType = "varchar(36)", ColumnDescription = "用户ID")]
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// 刷新令牌
    /// </summary>
    /// <remarks>
    /// 用于刷新 Access Token 的令牌值
    /// </remarks>
    [SugarColumn(Length = 500, ColumnDescription = "刷新令牌")]
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// 过期时间
    /// </summary>
    /// <remarks>
    /// 刷新令牌的过期时间，超过此时间后令牌失效
    /// </remarks>
    [SugarColumn(ColumnDescription = "过期时间")]
    public DateTime ExpiresAt { get; set; }

    /// <summary>
    /// 是否已撤销
    /// </summary>
    /// <remarks>
    /// 标记令牌是否已被撤销：
    /// 0=未撤销
    /// 1=已撤销
    /// </remarks>
    [SugarColumn(ColumnDescription = "是否已撤销：0=否，1=是")]
    public int IsRevoked { get; set; } = 0;

    /// <summary>
    /// 设备信息
    /// </summary>
    /// <remarks>
    /// 创建令牌时的设备信息（User-Agent）
    /// </remarks>
    [SugarColumn(Length = 500, IsNullable = true, ColumnDescription = "设备信息")]
    public string? DeviceInfo { get; set; }

    /// <summary>
    /// IP地址
    /// </summary>
    /// <remarks>
    /// 创建令牌时的 IP 地址
    /// </remarks>
    [SugarColumn(Length = 50, IsNullable = true, ColumnDescription = "IP地址")]
    public string? IpAddress { get; set; }
}