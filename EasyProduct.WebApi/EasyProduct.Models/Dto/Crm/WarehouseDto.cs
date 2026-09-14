using System;
using System.ComponentModel.DataAnnotations;

namespace EasyProduct.Models.Dto.Crm;

/// <summary>
/// 仓库 DTO（列表返回）
/// </summary>
/// <remarks>
/// 用于仓库列表查询返回
/// </remarks>
public class WarehouseDto
{
    /// <summary>
    /// 仓库ID
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// 仓库编码
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// 仓库名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 仓库地址
    /// </summary>
    public string Address { get; set; } = string.Empty;

    /// <summary>
    /// 负责人姓名
    /// </summary>
    public string Manager { get; set; } = string.Empty;

    /// <summary>
    /// 联系电话
    /// </summary>
    public string Phone { get; set; } = string.Empty;

    /// <summary>
    /// 仓库状态
    /// </summary>
    /// <remarks>
    /// 字符串字面量：'active' 或 'inactive'
    /// 与 BaseEntity.Status 枚举的映射：
    /// - Status.Enabled (1) → 'active'
    /// - Status.Disabled (0) → 'inactive'
    /// </remarks>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// 备注
    /// </summary>
    public string? Remark { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public string CreatedAt { get; set; } = string.Empty;

    /// <summary>
    /// 更新时间
    /// </summary>
    public string? UpdatedAt { get; set; }
}

/// <summary>
/// 创建仓库 DTO
/// </summary>
/// <remarks>
/// 用于创建仓库
/// </remarks>
public class CreateWarehouseDto
{
    /// <summary>
    /// 仓库编码
    /// </summary>
    [Required(ErrorMessage = "仓库编码不能为空")]
    [StringLength(20, ErrorMessage = "仓库编码长度不能超过20个字符")]
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// 仓库名称
    /// </summary>
    [Required(ErrorMessage = "仓库名称不能为空")]
    [StringLength(100, ErrorMessage = "仓库名称长度不能超过100个字符")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 仓库地址
    /// </summary>
    [Required(ErrorMessage = "仓库地址不能为空")]
    [StringLength(200, ErrorMessage = "仓库地址长度不能超过200个字符")]
    public string Address { get; set; } = string.Empty;

    /// <summary>
    /// 负责人姓名
    /// </summary>
    [Required(ErrorMessage = "负责人姓名不能为空")]
    [StringLength(50, ErrorMessage = "负责人姓名长度不能超过50个字符")]
    public string Manager { get; set; } = string.Empty;

    /// <summary>
    /// 联系电话
    /// </summary>
    [Required(ErrorMessage = "联系电话不能为空")]
    [StringLength(20, ErrorMessage = "联系电话长度不能超过20个字符")]
    public string Phone { get; set; } = string.Empty;

    /// <summary>
    /// 备注
    /// </summary>
    [StringLength(500, ErrorMessage = "备注长度不能超过500个字符")]
    public string? Remark { get; set; }
}

/// <summary>
/// 更新仓库 DTO
/// </summary>
/// <remarks>
/// 用于更新仓库
/// </remarks>
public class UpdateWarehouseDto
{
    /// <summary>
    /// 仓库名称
    /// </summary>
    [StringLength(100, ErrorMessage = "仓库名称长度不能超过100个字符")]
    public string? Name { get; set; }

    /// <summary>
    /// 仓库地址
    /// </summary>
    [StringLength(200, ErrorMessage = "仓库地址长度不能超过200个字符")]
    public string? Address { get; set; }

    /// <summary>
    /// 负责人姓名
    /// </summary>
    [StringLength(50, ErrorMessage = "负责人姓名长度不能超过50个字符")]
    public string? Manager { get; set; }

    /// <summary>
    /// 联系电话
    /// </summary>
    [StringLength(20, ErrorMessage = "联系电话长度不能超过20个字符")]
    public string? Phone { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    [StringLength(500, ErrorMessage = "备注长度不能超过500个字符")]
    public string? Remark { get; set; }
}

/// <summary>
/// 仓库查询 DTO
/// </summary>
/// <remarks>
/// 用于仓库列表查询筛选
/// </remarks>
public class WarehouseQueryDto
{
    /// <summary>
    /// 页码（从1开始）
    /// </summary>
    public int PageIndex { get; set; } = 1;

    /// <summary>
    /// 每页条数
    /// </summary>
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// 仓库编码（模糊搜索）
    /// </summary>
    public string? Code { get; set; }

    /// <summary>
    /// 仓库名称（模糊搜索）
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// 仓库状态
    /// </summary>
    /// <remarks>
    /// 可选值：'active' 或 'inactive'
    /// </remarks>
    public string? Status { get; set; }
}

/// <summary>
/// 仓库下拉选项 DTO
/// </summary>
/// <remarks>
/// 用于其他模块选择仓库时的下拉选项
/// 仅返回启用状态的仓库
/// </remarks>
public class WarehouseOptionDto
{
    /// <summary>
    /// 仓库ID
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// 仓库编码
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// 仓库名称
    /// </summary>
    public string Name { get; set; } = string.Empty;
}