using SqlSugar;
using EasyProduct.Models.Entitys.Base;
using EasyProduct.Models.Enums;

namespace EasyProduct.Models.Entitys.Basic;

/// <summary>
/// 角色实体
/// </summary>
/// <remarks>
/// 对应数据库表 basic_role
/// 继承 BaseEntity 基类，自动包含 Id、IsDeleted、Status、CreatedAt、UpdatedAt、CreatedBy、UpdatedBy 字段
/// 用于存储系统角色信息，支持 RBAC 权限管理
/// </remarks>
[SugarTable("basic_role", "角色表")]
public class basic_role : BaseEntity
{
    /// <summary>
    /// 角色名称
    /// </summary>
    /// <remarks>
    /// 角色的显示名称，如"管理员"、"普通用户"
    /// </remarks>
    [SugarColumn(Length = 50, ColumnDescription = "角色名称")]
    public string RoleName { get; set; } = string.Empty;

    /// <summary>
    /// 角色编码
    /// </summary>
    /// <remarks>
    /// 角色的唯一编码标识，用于程序中判断角色权限
    /// 如：admin、user、guest
    /// </remarks>
    [SugarColumn(Length = 50, ColumnDescription = "角色编码")]
    public string RoleCode { get; set; } = string.Empty;

    /// <summary>
    /// 描述
    /// </summary>
    /// <remarks>
    /// 角色的详细描述信息
    /// </remarks>
    [SugarColumn(Length = 200, IsNullable = true, ColumnDescription = "描述")]
    public string? Description { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    /// <remarks>
    /// 角色显示顺序，数值越小越靠前
    /// </remarks>
    [SugarColumn(ColumnDescription = "排序")]
    public int Sort { get; set; } = 0;
}