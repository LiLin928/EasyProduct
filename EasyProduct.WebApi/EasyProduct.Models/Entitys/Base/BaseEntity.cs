using SqlSugar;
using System;
using System.ComponentModel.DataAnnotations;
using EasyProduct.Models.Enums;

namespace EasyProduct.Models.Entitys.Base;

/// <summary>
/// 实体基类，所有业务实体类继承此类
/// </summary>
/// <remarks>
/// 包含通用字段：
/// - Id: GUID 主键
/// - IsDeleted: 软删除标记
/// - CreatedAt: 创建时间
/// - UpdatedAt: 更新时间
/// - CreatedBy: 创建人ID
/// - UpdatedBy: 更新人ID
///
/// 使用示例：
/// <code>
/// [SugarTable("basic_user", "用户表")]
/// public class basic_user : BaseEntity
/// {
///     [SugarColumn(Length = 50)]
///     public string UserName { get; set; } = string.Empty;
///     // 其他业务字段...
/// }
/// </code>
/// </remarks>
public abstract class BaseEntity
{
    /// <summary>
    /// 主键ID（GUID）
    /// </summary>
    /// <remarks>
    /// 使用 GUID 作为主键，避免数据库自增 ID 的分布式问题。
    /// 自动生成 GUID，无需手动赋值。
    /// </remarks>
    [SugarColumn(IsPrimaryKey = true, ColumnDescription = "主键ID")]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// 是否删除（软删除标记）
    /// </summary>
    /// <remarks>
    /// 使用 int 类型：0=未删除，1=已删除。
    /// 查询时默认过滤 IsDeleted = 1 的记录。
    /// </remarks>
    [SugarColumn(ColumnDescription = "是否删除")]
    public int IsDeleted { get; set; } = 0;

    /// <summary>
    /// 状态
    /// </summary>
    /// <remarks>
    /// 使用 Status 枚举：0=禁用，1=启用。
    /// 默认为启用状态。
    /// </remarks>
    [SugarColumn(ColumnDescription = "状态")]
    public Status Status { get; set; } = Status.Enabled;

    /// <summary>
    /// 创建时间
    /// </summary>
    /// <remarks>
    /// - 记录创建时的时间戳
    /// - 插入时自动赋值为当前时间（UTC）
    /// </remarks>
    [SugarColumn(ColumnDescription = "创建时间")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 更新时间
    /// </summary>
    /// <remarks>
    /// - 记录最后一次更新的时间戳
    /// - 插入时为 null，更新时自动赋值为当前时间（UTC）
    /// </remarks>
    [SugarColumn(ColumnDescription = "更新时间", IsNullable = true)]
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// 创建人ID
    /// </summary>
    /// <remarks>
    /// - 记录创建该记录的用户ID（GUID 字符串）
    /// - 可为空，系统自动创建的记录此字段为空
    /// </remarks>
    [SugarColumn(ColumnDataType = "varchar(36)", ColumnDescription = "创建人ID", IsNullable = true)]
    public string? CreatedBy { get; set; }

    /// <summary>
    /// 更新人ID
    /// </summary>
    /// <remarks>
    /// - 记录最后一次更新该记录的用户ID（GUID 字符串）
    /// - 可为空，系统自动更新的记录此字段为空
    /// </remarks>
    [SugarColumn(ColumnDataType = "varchar(36)", ColumnDescription = "更新人ID", IsNullable = true)]
    public string? UpdatedBy { get; set; }
}