using System;
using System.ComponentModel.DataAnnotations;

namespace EasyProduct.Models.Dto.Crm;

/// <summary>
/// 供应商 DTO
/// </summary>
/// <remarks>
/// 用于供应商列表查询返回
/// </remarks>
public class SupplierDto
{
    /// <summary>
    /// 供应商ID
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// 供应商编码
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// 供应商名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

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
    /// 开户行
    /// </summary>
    public string? BankName { get; set; }

    /// <summary>
    /// 银行账号
    /// </summary>
    public string? BankAccount { get; set; }

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
/// 创建供应商 DTO
/// </summary>
/// <remarks>
/// 用于创建供应商
/// </remarks>
public class CreateSupplierDto
{
    /// <summary>
    /// 供应商名称
    /// </summary>
    [Required(ErrorMessage = "供应商名称不能为空")]
    [StringLength(100, ErrorMessage = "供应商名称长度不能超过100个字符")]
    public string Name { get; set; } = string.Empty;

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
    /// 开户行
    /// </summary>
    [StringLength(100, ErrorMessage = "开户行长度不能超过100个字符")]
    public string? BankName { get; set; }

    /// <summary>
    /// 银行账号
    /// </summary>
    [StringLength(50, ErrorMessage = "银行账号长度不能超过50个字符")]
    public string? BankAccount { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    [StringLength(500, ErrorMessage = "备注长度不能超过500个字符")]
    public string? Remark { get; set; }
}

/// <summary>
/// 更新供应商 DTO
/// </summary>
/// <remarks>
/// 用于更新供应商
/// </remarks>
public class UpdateSupplierDto
{
    /// <summary>
    /// 供应商名称
    /// </summary>
    [Required(ErrorMessage = "供应商名称不能为空")]
    [StringLength(100, ErrorMessage = "供应商名称长度不能超过100个字符")]
    public string Name { get; set; } = string.Empty;

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
    /// 开户行
    /// </summary>
    [StringLength(100, ErrorMessage = "开户行长度不能超过100个字符")]
    public string? BankName { get; set; }

    /// <summary>
    /// 银行账号
    /// </summary>
    [StringLength(50, ErrorMessage = "银行账号长度不能超过50个字符")]
    public string? BankAccount { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    [StringLength(500, ErrorMessage = "备注长度不能超过500个字符")]
    public string? Remark { get; set; }
}

/// <summary>
/// 供应商查询 DTO
/// </summary>
/// <remarks>
/// 用于供应商列表查询筛选
/// </remarks>
public class SupplierQueryDto
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
    /// 供应商名称（模糊搜索）
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// 供应商编码（模糊搜索）
    /// </summary>
    public string? Code { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    public int? Status { get; set; }

    /// <summary>
    /// 关键词（模糊搜索供应商名称、编码、联系人）
    /// </summary>
    public string? Keyword { get; set; }
}

/// <summary>
/// 供应商资质 DTO
/// </summary>
/// <remarks>
/// 用于供应商资质列表查询返回
/// </remarks>
public class SupplierQualificationDto
{
    /// <summary>
    /// 资质ID
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// 供应商ID
    /// </summary>
    public string SupplierId { get; set; } = string.Empty;

    /// <summary>
    /// 资质类型
    /// </summary>
    public int Type { get; set; }

    /// <summary>
    /// 资质类型名称
    /// </summary>
    public string TypeName { get; set; } = string.Empty;

    /// <summary>
    /// 资质名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 证书编号
    /// </summary>
    public string? CertificateNo { get; set; }

    /// <summary>
    /// 发证日期
    /// </summary>
    public DateTime? IssueDate { get; set; }

    /// <summary>
    /// 有效期
    /// </summary>
    public DateTime? ExpireDate { get; set; }

    /// <summary>
    /// 证件图片URL
    /// </summary>
    public string? ImageUrl { get; set; }

    /// <summary>
    /// 资质状态
    /// </summary>
    public int QualificationStatus { get; set; }

    /// <summary>
    /// 资质状态名称
    /// </summary>
    public string QualificationStatusName { get; set; } = string.Empty;

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// 创建供应商资质 DTO
/// </summary>
/// <remarks>
/// 用于创建供应商资质
/// </remarks>
public class CreateSupplierQualificationDto
{
    /// <summary>
    /// 供应商ID
    /// </summary>
    [Required(ErrorMessage = "供应商ID不能为空")]
    public string SupplierId { get; set; } = string.Empty;

    /// <summary>
    /// 资质类型
    /// </summary>
    [Required(ErrorMessage = "资质类型不能为空")]
    public int Type { get; set; }

    /// <summary>
    /// 资质名称
    /// </summary>
    [Required(ErrorMessage = "资质名称不能为空")]
    [StringLength(100, ErrorMessage = "资质名称长度不能超过100个字符")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 证书编号
    /// </summary>
    [StringLength(50, ErrorMessage = "证书编号长度不能超过50个字符")]
    public string? CertificateNo { get; set; }

    /// <summary>
    /// 发证日期
    /// </summary>
    public DateTime? IssueDate { get; set; }

    /// <summary>
    /// 有效期
    /// </summary>
    public DateTime? ExpireDate { get; set; }

    /// <summary>
    /// 证件图片URL
    /// </summary>
    [StringLength(500, ErrorMessage = "证件图片URL长度不能超过500个字符")]
    public string? ImageUrl { get; set; }
}

/// <summary>
/// 更新供应商资质 DTO
/// </summary>
/// <remarks>
/// 用于更新供应商资质
/// </remarks>
public class UpdateSupplierQualificationDto
{
    /// <summary>
    /// 资质类型
    /// </summary>
    [Required(ErrorMessage = "资质类型不能为空")]
    public int Type { get; set; }

    /// <summary>
    /// 资质名称
    /// </summary>
    [Required(ErrorMessage = "资质名称不能为空")]
    [StringLength(100, ErrorMessage = "资质名称长度不能超过100个字符")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 证书编号
    /// </summary>
    [StringLength(50, ErrorMessage = "证书编号长度不能超过50个字符")]
    public string? CertificateNo { get; set; }

    /// <summary>
    /// 发证日期
    /// </summary>
    public DateTime? IssueDate { get; set; }

    /// <summary>
    /// 有效期
    /// </summary>
    public DateTime? ExpireDate { get; set; }

    /// <summary>
    /// 证件图片URL
    /// </summary>
    [StringLength(500, ErrorMessage = "证件图片URL长度不能超过500个字符")]
    public string? ImageUrl { get; set; }
}