using System.ComponentModel.DataAnnotations;

namespace EasyProduct.Models.Dto.Basic.Dept;

/// <summary>
/// 部门查询参数
/// </summary>
public class DeptQueryDto
{
    /// <summary>
    /// 部门名称（模糊搜索）
    /// </summary>
    [MaxLength(100)]
    public string? Name { get; set; }

    /// <summary>
    /// 部门编码（模糊搜索）
    /// </summary>
    [MaxLength(50)]
    public string? Code { get; set; }

    /// <summary>
    /// 状态：0=禁用，1=启用
    /// </summary>
    public int? Status { get; set; }
}

/// <summary>
/// 部门树形结构查询参数
/// </summary>
public class DeptTreeQueryDto
{
    /// <summary>
    /// 父部门ID（可选，不传则查询全部）
    /// </summary>
    public string? ParentId { get; set; }

    /// <summary>
    /// 是否只查询启用的部门
    /// </summary>
    public bool? OnlyEnabled { get; set; }
}