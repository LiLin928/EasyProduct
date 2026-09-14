using System;
using System.ComponentModel.DataAnnotations;

namespace EasyProduct.Models.Dto.Crm;

/// <summary>
/// 客户联系人 DTO
/// </summary>
/// <remarks>
/// 用于客户联系人列表查询返回
/// </remarks>
public class CustomerContactDto
{
    /// <summary>
    /// 联系人ID
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// 客户ID
    /// </summary>
    public string CustomerId { get; set; } = string.Empty;

    /// <summary>
    /// 联系人姓名
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 联系电话
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// 邮箱
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// 职位
    /// </summary>
    public string? Position { get; set; }

    /// <summary>
    /// 是否主要联系人
    /// </summary>
    public int IsPrimary { get; set; }

    /// <summary>
    /// 是否主要联系人名称
    /// </summary>
    public string IsPrimaryName { get; set; } = string.Empty;

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// 创建客户联系人 DTO
/// </summary>
/// <remarks>
/// 用于创建客户联系人
/// </remarks>
public class CreateCustomerContactDto
{
    /// <summary>
    /// 客户ID
    /// </summary>
    [Required(ErrorMessage = "客户ID不能为空")]
    public string CustomerId { get; set; } = string.Empty;

    /// <summary>
    /// 联系人姓名
    /// </summary>
    [Required(ErrorMessage = "联系人姓名不能为空")]
    [StringLength(50, ErrorMessage = "联系人姓名长度不能超过50个字符")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 联系电话
    /// </summary>
    [StringLength(20, ErrorMessage = "联系电话长度不能超过20个字符")]
    public string? Phone { get; set; }

    /// <summary>
    /// 邮箱
    /// </summary>
    [StringLength(100, ErrorMessage = "邮箱长度不能超过100个字符")]
    [EmailAddress(ErrorMessage = "邮箱格式不正确")]
    public string? Email { get; set; }

    /// <summary>
    /// 职位
    /// </summary>
    [StringLength(50, ErrorMessage = "职位长度不能超过50个字符")]
    public string? Position { get; set; }

    /// <summary>
    /// 是否主要联系人
    /// </summary>
    public int IsPrimary { get; set; } = 0;
}

/// <summary>
/// 更新客户联系人 DTO
/// </summary>
/// <remarks>
/// 用于更新客户联系人
/// </remarks>
public class UpdateCustomerContactDto
{
    /// <summary>
    /// 联系人姓名
    /// </summary>
    [Required(ErrorMessage = "联系人姓名不能为空")]
    [StringLength(50, ErrorMessage = "联系人姓名长度不能超过50个字符")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 联系电话
    /// </summary>
    [StringLength(20, ErrorMessage = "联系电话长度不能超过20个字符")]
    public string? Phone { get; set; }

    /// <summary>
    /// 邮箱
    /// </summary>
    [StringLength(100, ErrorMessage = "邮箱长度不能超过100个字符")]
    [EmailAddress(ErrorMessage = "邮箱格式不正确")]
    public string? Email { get; set; }

    /// <summary>
    /// 职位
    /// </summary>
    [StringLength(50, ErrorMessage = "职位长度不能超过50个字符")]
    public string? Position { get; set; }

    /// <summary>
    /// 是否主要联系人
    /// </summary>
    public int IsPrimary { get; set; } = 0;
}

/// <summary>
/// 客户地址 DTO
/// </summary>
/// <remarks>
/// 用于客户地址列表查询返回
/// </remarks>
public class CustomerAddressDto
{
    /// <summary>
    /// 地址ID
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// 客户ID
    /// </summary>
    public string CustomerId { get; set; } = string.Empty;

    /// <summary>
    /// 收货人姓名
    /// </summary>
    public string ReceiverName { get; set; } = string.Empty;

    /// <summary>
    /// 联系电话
    /// </summary>
    public string Phone { get; set; } = string.Empty;

    /// <summary>
    /// 省份
    /// </summary>
    public string Province { get; set; } = string.Empty;

    /// <summary>
    /// 城市
    /// </summary>
    public string City { get; set; } = string.Empty;

    /// <summary>
    /// 区县
    /// </summary>
    public string? District { get; set; }

    /// <summary>
    /// 详细地址
    /// </summary>
    public string DetailAddress { get; set; } = string.Empty;

    /// <summary>
    /// 完整地址
    /// </summary>
    public string FullAddress { get; set; } = string.Empty;

    /// <summary>
    /// 是否默认地址
    /// </summary>
    public int IsDefault { get; set; }

    /// <summary>
    /// 是否默认地址名称
    /// </summary>
    public string IsDefaultName { get; set; } = string.Empty;

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// 创建客户地址 DTO
/// </summary>
/// <remarks>
/// 用于创建客户地址
/// </remarks>
public class CreateCustomerAddressDto
{
    /// <summary>
    /// 客户ID
    /// </summary>
    [Required(ErrorMessage = "客户ID不能为空")]
    public string CustomerId { get; set; } = string.Empty;

    /// <summary>
    /// 收货人姓名
    /// </summary>
    [Required(ErrorMessage = "收货人姓名不能为空")]
    [StringLength(50, ErrorMessage = "收货人姓名长度不能超过50个字符")]
    public string ReceiverName { get; set; } = string.Empty;

    /// <summary>
    /// 联系电话
    /// </summary>
    [Required(ErrorMessage = "联系电话不能为空")]
    [StringLength(20, ErrorMessage = "联系电话长度不能超过20个字符")]
    public string Phone { get; set; } = string.Empty;

    /// <summary>
    /// 省份
    /// </summary>
    [Required(ErrorMessage = "省份不能为空")]
    [StringLength(50, ErrorMessage = "省份长度不能超过50个字符")]
    public string Province { get; set; } = string.Empty;

    /// <summary>
    /// 城市
    /// </summary>
    [Required(ErrorMessage = "城市不能为空")]
    [StringLength(50, ErrorMessage = "城市长度不能超过50个字符")]
    public string City { get; set; } = string.Empty;

    /// <summary>
    /// 区县
    /// </summary>
    [StringLength(50, ErrorMessage = "区县长度不能超过50个字符")]
    public string? District { get; set; }

    /// <summary>
    /// 详细地址
    /// </summary>
    [Required(ErrorMessage = "详细地址不能为空")]
    [StringLength(200, ErrorMessage = "详细地址长度不能超过200个字符")]
    public string DetailAddress { get; set; } = string.Empty;

    /// <summary>
    /// 是否默认地址
    /// </summary>
    public int IsDefault { get; set; } = 0;
}

/// <summary>
/// 更新客户地址 DTO
/// </summary>
/// <remarks>
/// 用于更新客户地址
/// </remarks>
public class UpdateCustomerAddressDto
{
    /// <summary>
    /// 收货人姓名
    /// </summary>
    [Required(ErrorMessage = "收货人姓名不能为空")]
    [StringLength(50, ErrorMessage = "收货人姓名长度不能超过50个字符")]
    public string ReceiverName { get; set; } = string.Empty;

    /// <summary>
    /// 联系电话
    /// </summary>
    [Required(ErrorMessage = "联系电话不能为空")]
    [StringLength(20, ErrorMessage = "联系电话长度不能超过20个字符")]
    public string Phone { get; set; } = string.Empty;

    /// <summary>
    /// 省份
    /// </summary>
    [Required(ErrorMessage = "省份不能为空")]
    [StringLength(50, ErrorMessage = "省份长度不能超过50个字符")]
    public string Province { get; set; } = string.Empty;

    /// <summary>
    /// 城市
    /// </summary>
    [Required(ErrorMessage = "城市不能为空")]
    [StringLength(50, ErrorMessage = "城市长度不能超过50个字符")]
    public string City { get; set; } = string.Empty;

    /// <summary>
    /// 区县
    /// </summary>
    [StringLength(50, ErrorMessage = "区县长度不能超过50个字符")]
    public string? District { get; set; }

    /// <summary>
    /// 详细地址
    /// </summary>
    [Required(ErrorMessage = "详细地址不能为空")]
    [StringLength(200, ErrorMessage = "详细地址长度不能超过200个字符")]
    public string DetailAddress { get; set; } = string.Empty;

    /// <summary>
    /// 是否默认地址
    /// </summary>
    public int IsDefault { get; set; } = 0;
}