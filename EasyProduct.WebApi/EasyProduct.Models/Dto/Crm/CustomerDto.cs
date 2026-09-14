using System;
using System.ComponentModel.DataAnnotations;

namespace EasyProduct.Models.Dto.Crm;

/// <summary>
/// 客户 DTO
/// </summary>
/// <remarks>
/// 用于客户列表查询返回
/// </remarks>
public class CustomerDto
{
    /// <summary>
    /// 客户ID
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// 客户编码
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// 客户名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 客户类型
    /// </summary>
    public int Type { get; set; }

    /// <summary>
    /// 客户类型名称
    /// </summary>
    public string TypeName { get; set; } = string.Empty;

    /// <summary>
    /// 客户来源
    /// </summary>
    public int Source { get; set; }

    /// <summary>
    /// 客户来源名称
    /// </summary>
    public string SourceName { get; set; } = string.Empty;

    /// <summary>
    /// 状态
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// 状态名称
    /// </summary>
    public string StatusName { get; set; } = string.Empty;

    /// <summary>
    /// 联系人姓名
    /// </summary>
    public string? ContactName { get; set; }

    /// <summary>
    /// 联系电话
    /// </summary>
    public string? ContactPhone { get; set; }

    /// <summary>
    /// 联系邮箱
    /// </summary>
    public string? ContactEmail { get; set; }

    /// <summary>
    /// 地址
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// 归属业务员ID
    /// </summary>
    public string? BusinessUserId { get; set; }

    /// <summary>
    /// 归属业务员姓名
    /// </summary>
    public string? BusinessUserName { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    public string? Remark { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// 创建客户 DTO
/// </summary>
/// <remarks>
/// 用于创建客户
/// </remarks>
public class CreateCustomerDto
{
    /// <summary>
    /// 客户名称
    /// </summary>
    [Required(ErrorMessage = "客户名称不能为空")]
    [StringLength(100, ErrorMessage = "客户名称长度不能超过100个字符")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 客户类型
    /// </summary>
    [Required(ErrorMessage = "客户类型不能为空")]
    public int Type { get; set; }

    /// <summary>
    /// 客户来源
    /// </summary>
    [Required(ErrorMessage = "客户来源不能为空")]
    public int Source { get; set; }

    /// <summary>
    /// 联系人姓名
    /// </summary>
    [StringLength(50, ErrorMessage = "联系人姓名长度不能超过50个字符")]
    public string? ContactName { get; set; }

    /// <summary>
    /// 联系电话
    /// </summary>
    [StringLength(20, ErrorMessage = "联系电话长度不能超过20个字符")]
    public string? ContactPhone { get; set; }

    /// <summary>
    /// 联系邮箱
    /// </summary>
    [StringLength(100, ErrorMessage = "联系邮箱长度不能超过100个字符")]
    [EmailAddress(ErrorMessage = "邮箱格式不正确")]
    public string? ContactEmail { get; set; }

    /// <summary>
    /// 地址
    /// </summary>
    [StringLength(500, ErrorMessage = "地址长度不能超过500个字符")]
    public string? Address { get; set; }

    /// <summary>
    /// 归属业务员ID
    /// </summary>
    public string? BusinessUserId { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    [StringLength(500, ErrorMessage = "备注长度不能超过500个字符")]
    public string? Remark { get; set; }
}

/// <summary>
/// 更新客户 DTO
/// </summary>
/// <remarks>
/// 用于更新客户
/// </remarks>
public class UpdateCustomerDto
{
    /// <summary>
    /// 客户名称
    /// </summary>
    [Required(ErrorMessage = "客户名称不能为空")]
    [StringLength(100, ErrorMessage = "客户名称长度不能超过100个字符")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 客户类型
    /// </summary>
    [Required(ErrorMessage = "客户类型不能为空")]
    public int Type { get; set; }

    /// <summary>
    /// 联系人姓名
    /// </summary>
    [StringLength(50, ErrorMessage = "联系人姓名长度不能超过50个字符")]
    public string? ContactName { get; set; }

    /// <summary>
    /// 联系电话
    /// </summary>
    [StringLength(20, ErrorMessage = "联系电话长度不能超过20个字符")]
    public string? ContactPhone { get; set; }

    /// <summary>
    /// 联系邮箱
    /// </summary>
    [StringLength(100, ErrorMessage = "联系邮箱长度不能超过100个字符")]
    [EmailAddress(ErrorMessage = "邮箱格式不正确")]
    public string? ContactEmail { get; set; }

    /// <summary>
    /// 地址
    /// </summary>
    [StringLength(500, ErrorMessage = "地址长度不能超过500个字符")]
    public string? Address { get; set; }

    /// <summary>
    /// 归属业务员ID
    /// </summary>
    public string? BusinessUserId { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    [StringLength(500, ErrorMessage = "备注长度不能超过500个字符")]
    public string? Remark { get; set; }
}

/// <summary>
/// 客户查询 DTO
/// </summary>
/// <remarks>
/// 用于客户列表查询筛选
/// </remarks>
public class CustomerQueryDto
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
    /// 客户名称（模糊搜索）
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// 客户编码（模糊搜索）
    /// </summary>
    public string? Code { get; set; }

    /// <summary>
    /// 客户类型
    /// </summary>
    public int? Type { get; set; }

    /// <summary>
    /// 客户来源
    /// </summary>
    public int? Source { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    public int? Status { get; set; }

    /// <summary>
    /// 归属业务员ID
    /// </summary>
    public string? BusinessUserId { get; set; }

    /// <summary>
    /// 关键词（模糊搜索客户名称、编码、联系人）
    /// </summary>
    public string? Keyword { get; set; }
}