using SqlSugar;
using EasyProduct.Models.Entitys.Base;
using EasyProduct.Models.Enums;

namespace EasyProduct.Models.Entitys.Basic;

/// <summary>
/// 部门实体
/// </summary>
/// <remarks>
/// 对应数据库表 basic_dept
/// 继承 BaseEntity 基类，自动包含 Id、IsDeleted、Status、CreatedAt、UpdatedAt、CreatedBy、UpdatedBy 字段
/// 用于存储组织架构中的部门信息，支持树形结构
/// </remarks>
[SugarTable("basic_dept", "部门表")]
public class basic_dept : BaseEntity
{
    /// <summary>
    /// 部门名称
    /// </summary>
    [SugarColumn(Length = 100, ColumnDescription = "部门名称")]
    public string DeptName { get; set; } = string.Empty;

    /// <summary>
    /// 上级部门ID
    /// </summary>
    /// <remarks>
    /// 上级部门ID，根部门为空字符串或"0"
    /// </remarks>
    [SugarColumn(ColumnDataType = "varchar(36)", IsNullable = true, ColumnDescription = "上级部门ID")]
    public string? ParentId { get; set; }

    /// <summary>
    /// 部门编码
    /// </summary>
    /// <remarks>
    /// 部门的唯一编码标识
    /// </remarks>
    [SugarColumn(Length = 50, IsNullable = true, ColumnDescription = "部门编码")]
    public string? DeptCode { get; set; }

    /// <summary>
    /// 部门负责人ID
    /// </summary>
    /// <remarks>
    /// 部门负责人的用户ID
    /// </remarks>
    [SugarColumn(ColumnDataType = "varchar(36)", IsNullable = true, ColumnDescription = "部门负责人ID")]
    public string? LeaderId { get; set; }

    /// <summary>
    /// 部门负责人姓名
    /// </summary>
    /// <remarks>
    /// 冗余字段，方便显示，从用户表同步
    /// </remarks>
    [SugarColumn(Length = 50, IsNullable = true, ColumnDescription = "部门负责人姓名")]
    public string? LeaderName { get; set; }

    /// <summary>
    /// 联系电话
    /// </summary>
    [SugarColumn(Length = 20, IsNullable = true, ColumnDescription = "联系电话")]
    public string? Phone { get; set; }

    /// <summary>
    /// 部门邮箱
    /// </summary>
    [SugarColumn(Length = 100, IsNullable = true, ColumnDescription = "部门邮箱")]
    public string? Email { get; set; }

    /// <summary>
    /// 部门完整路径
    /// </summary>
    /// <remarks>
    /// 如：总公司/技术部/前端组，用于快速定位和显示
    /// </remarks>
    [SugarColumn(Length = 500, IsNullable = true, ColumnDescription = "部门完整路径")]
    public string? FullPath { get; set; }

    /// <summary>
    /// 部门层级
    /// </summary>
    /// <remarks>
    /// 1=一级（总公司），2=二级，3=三级...
    /// </remarks>
    [SugarColumn(ColumnDescription = "部门层级")]
    public int Level { get; set; } = 1;

    /// <summary>
    /// 排序
    /// </summary>
    /// <remarks>
    /// 部门显示顺序，数值越小越靠前
    /// </remarks>
    [SugarColumn(ColumnDescription = "排序")]
    public int Sort { get; set; } = 0;

    /// <summary>
    /// 描述
    /// </summary>
    [SugarColumn(Length = 500, IsNullable = true, ColumnDescription = "描述")]
    public string? Description { get; set; }
}