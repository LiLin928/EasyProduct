using SqlSugar;
using EasyProduct.Models.Entitys.Base;

namespace EasyProduct.Models.Entitys.Basic;

/// <summary>
/// 用户角色关联实体
/// </summary>
/// <remarks>
/// 对应数据库表 basic_user_role
/// 继承 BaseEntity 基类，自动包含 Id、IsDeleted、Status、CreatedAt、UpdatedAt、CreatedBy、UpdatedBy 字段
/// 用于建立用户与角色的多对多关系，一个用户可以拥有多个角色
/// </remarks>
[SugarTable("basic_user_role", "用户角色关联表")]
public class basic_user_role : BaseEntity
{
    /// <summary>
    /// 用户ID
    /// </summary>
    /// <remarks>
    /// 关联的用户ID，外键关联 basic_user 表
    /// </remarks>
    [SugarColumn(ColumnDataType = "varchar(36)", ColumnDescription = "用户ID")]
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// 角色ID
    /// </summary>
    /// <remarks>
    /// 关联的角色ID，外键关联 basic_role 表
    /// </remarks>
    [SugarColumn(ColumnDataType = "varchar(36)", ColumnDescription = "角色ID")]
    public string RoleId { get; set; } = string.Empty;
}