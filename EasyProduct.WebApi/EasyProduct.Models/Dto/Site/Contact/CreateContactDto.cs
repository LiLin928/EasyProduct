using System.ComponentModel.DataAnnotations;

namespace EasyProduct.Models.Dto.Site.Contact;

/// <summary>
/// 创建留言DTO
/// </summary>
public class CreateContactDto
{
    /// <summary>
    /// 姓名
    /// </summary>
    [Required(ErrorMessage = "姓名不能为空")]
    [StringLength(50, ErrorMessage = "姓名长度不能超过50字符")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 姓名（英文）
    /// </summary>
    [StringLength(50, ErrorMessage = "英文姓名长度不能超过50字符")]
    public string? NameEn { get; set; }

    /// <summary>
    /// 联系电话
    /// </summary>
    [StringLength(20, ErrorMessage = "联系电话长度不能超过20字符")]
    public string? Phone { get; set; }

    /// <summary>
    /// 电子邮箱
    /// </summary>
    [StringLength(100, ErrorMessage = "电子邮箱长度不能超过100字符")]
    [EmailAddress(ErrorMessage = "电子邮箱格式不正确")]
    public string? Email { get; set; }

    /// <summary>
    /// 公司名称
    /// </summary>
    [StringLength(200, ErrorMessage = "公司名称长度不能超过200字符")]
    public string? Company { get; set; }

    /// <summary>
    /// 公司名称（英文）
    /// </summary>
    [StringLength(200, ErrorMessage = "公司英文名称长度不能超过200字符")]
    public string? CompanyEn { get; set; }

    /// <summary>
    /// 主题
    /// </summary>
    [StringLength(200, ErrorMessage = "主题长度不能超过200字符")]
    public string? Subject { get; set; }

    /// <summary>
    /// 主题（英文）
    /// </summary>
    [StringLength(200, ErrorMessage = "英文主题长度不能超过200字符")]
    public string? SubjectEn { get; set; }

    /// <summary>
    /// 留言内容
    /// </summary>
    [Required(ErrorMessage = "留言内容不能为空")]
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// 留言内容（英文）
    /// </summary>
    public string? MessageEn { get; set; }
}