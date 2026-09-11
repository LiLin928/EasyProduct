namespace EasyProduct.Models.Dto.Site.Contact;

/// <summary>
/// 留言DTO
/// </summary>
public class ContactDto
{
    /// <summary>
    /// 留言ID
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// 姓名
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 姓名（英文）
    /// </summary>
    public string? NameEn { get; set; }

    /// <summary>
    /// 联系电话
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// 电子邮箱
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// 公司名称
    /// </summary>
    public string? Company { get; set; }

    /// <summary>
    /// 公司名称（英文）
    /// </summary>
    public string? CompanyEn { get; set; }

    /// <summary>
    /// 主题
    /// </summary>
    public string? Subject { get; set; }

    /// <summary>
    /// 主题（英文）
    /// </summary>
    public string? SubjectEn { get; set; }

    /// <summary>
    /// 留言内容
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// 留言内容（英文）
    /// </summary>
    public string? MessageEn { get; set; }

    /// <summary>
    /// 状态：0=未读，1=已读，2=已回复
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// 回复内容
    /// </summary>
    public string? Reply { get; set; }

    /// <summary>
    /// 回复内容（英文）
    /// </summary>
    public string? ReplyEn { get; set; }

    /// <summary>
    /// 回复时间
    /// </summary>
    public DateTime? RepliedAt { get; set; }

    /// <summary>
    /// 回复人ID
    /// </summary>
    public string? RepliedBy { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}