using System.ComponentModel.DataAnnotations;

namespace EasyProduct.Models.Dto.Site.Contact;

/// <summary>
/// 回复留言DTO
/// </summary>
public class ReplyContactDto
{
    /// <summary>
    /// 留言ID
    /// </summary>
    [Required(ErrorMessage = "留言ID不能为空")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// 回复内容
    /// </summary>
    [Required(ErrorMessage = "回复内容不能为空")]
    public string Reply { get; set; } = string.Empty;

    /// <summary>
    /// 回复内容（英文）
    /// </summary>
    public string? ReplyEn { get; set; }
}