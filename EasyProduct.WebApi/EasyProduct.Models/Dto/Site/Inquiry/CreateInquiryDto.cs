using System.ComponentModel.DataAnnotations;

namespace EasyProduct.Models.Dto.Site.Inquiry;

/// <summary>
/// 创建询价单DTO
/// </summary>
public class CreateInquiryDto
{
    /// <summary>
    /// 公司名称
    /// </summary>
    [Required(ErrorMessage = "公司名称不能为空")]
    [StringLength(200, ErrorMessage = "公司名称长度不能超过200字符")]
    public string CompanyName { get; set; } = string.Empty;

    /// <summary>
    /// 公司名称（英文）
    /// </summary>
    [StringLength(200, ErrorMessage = "公司英文名称长度不能超过200字符")]
    public string? CompanyNameEn { get; set; }

    /// <summary>
    /// 联系人姓名
    /// </summary>
    [Required(ErrorMessage = "联系人姓名不能为空")]
    [StringLength(50, ErrorMessage = "联系人姓名长度不能超过50字符")]
    public string ContactName { get; set; } = string.Empty;

    /// <summary>
    /// 联系人姓名（英文）
    /// </summary>
    [StringLength(50, ErrorMessage = "联系人英文姓名长度不能超过50字符")]
    public string? ContactNameEn { get; set; }

    /// <summary>
    /// 联系电话
    /// </summary>
    [Required(ErrorMessage = "联系电话不能为空")]
    [StringLength(20, ErrorMessage = "联系电话长度不能超过20字符")]
    public string Phone { get; set; } = string.Empty;

    /// <summary>
    /// 电子邮箱
    /// </summary>
    [StringLength(100, ErrorMessage = "电子邮箱长度不能超过100字符")]
    [EmailAddress(ErrorMessage = "电子邮箱格式不正确")]
    public string? Email { get; set; }

    /// <summary>
    /// 国家
    /// </summary>
    [StringLength(50, ErrorMessage = "国家长度不能超过50字符")]
    public string? Country { get; set; }

    /// <summary>
    /// 省份
    /// </summary>
    [StringLength(50, ErrorMessage = "省份长度不能超过50字符")]
    public string? Province { get; set; }

    /// <summary>
    /// 城市
    /// </summary>
    [StringLength(50, ErrorMessage = "城市长度不能超过50字符")]
    public string? City { get; set; }

    /// <summary>
    /// 详细地址
    /// </summary>
    [StringLength(500, ErrorMessage = "详细地址长度不能超过500字符")]
    public string? Address { get; set; }

    /// <summary>
    /// 详细地址（英文）
    /// </summary>
    [StringLength(500, ErrorMessage = "详细英文地址长度不能超过500字符")]
    public string? AddressEn { get; set; }

    /// <summary>
    /// 备注信息
    /// </summary>
    public string? Remark { get; set; }

    /// <summary>
    /// 询价明细列表
    /// </summary>
    public List<CreateInquiryItemDto>? Items { get; set; }
}

/// <summary>
/// 创建询价明细DTO
/// </summary>
public class CreateInquiryItemDto
{
    /// <summary>
    /// 产品名称
    /// </summary>
    [Required(ErrorMessage = "产品名称不能为空")]
    [StringLength(200, ErrorMessage = "产品名称长度不能超过200字符")]
    public string ProductName { get; set; } = string.Empty;

    /// <summary>
    /// 产品名称（英文）
    /// </summary>
    [StringLength(200, ErrorMessage = "产品英文名称长度不能超过200字符")]
    public string? ProductNameEn { get; set; }

    /// <summary>
    /// 产品编码
    /// </summary>
    [StringLength(50, ErrorMessage = "产品编码长度不能超过50字符")]
    public string? ProductCode { get; set; }

    /// <summary>
    /// 规格型号
    /// </summary>
    [StringLength(200, ErrorMessage = "规格型号长度不能超过200字符")]
    public string? Specification { get; set; }

    /// <summary>
    /// 数量
    /// </summary>
    [Range(1, int.MaxValue, ErrorMessage = "数量必须大于0")]
    public int? Quantity { get; set; }

    /// <summary>
    /// 单位
    /// </summary>
    [StringLength(20, ErrorMessage = "单位长度不能超过20字符")]
    public string? Unit { get; set; }

    /// <summary>
    /// 备注信息
    /// </summary>
    public string? Remark { get; set; }
}