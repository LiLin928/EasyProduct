using SqlSugar;
using EasyProduct.Models.Entitys.Base;

namespace EasyProduct.Models.Entitys.Basic;

/// <summary>
/// 角色菜单关联实体
/// </summary>
/// <remarks>
/// 对应数据库表 basic_role_menu
/// 继承 BaseEntity 基类，自动包含 Id、IsDeleted、Status、CreatedAt、UpdatedAt、CreatedBy、UpdatedBy 字段
/// 用于建立角色与菜单的多对多关系，一个角色可以拥有多个菜单权限
/// </remarks>
[SugarTable("basic_role_menu", "角色菜单关联表")]
public class basic_role_menu : BaseEntity
{
    /// <summary>
    /// 角色ID
    /// </summary>
    /// <remarks>
    /// 关联的角色ID，外键关联 basic_role 表
    /// </remarks>
    [SugarColumn(ColumnDataType = "varchar(36)", ColumnDescription = "角色ID")]
    public string RoleId { get; set; } = string.Empty;

    /// <summary>
    /// 菜单ID
    /// </summary>
    /// <remarks>
    /// 关联的菜单ID，外键关联 basic_menu 表
    /// </remarks>
    [SugarColumn(ColumnDataType = "varchar(36)", ColumnDescription = "菜单ID")]
    public string MenuId { get; set; } = string.Empty;
}