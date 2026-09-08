namespace EasyProduct.Models.Dto.Basic.Notice;

/// <summary>
/// 公告DTO
/// </summary>
public class NoticeDto
{
    /// <summary>
    /// 公告ID
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// 公告标题
    /// </summary>
    public string NoticeTitle { get; set; } = string.Empty;

    /// <summary>
    /// 公告内容（富文本）
    /// </summary>
    public string NoticeContent { get; set; } = string.Empty;

    /// <summary>
    /// 公告类型：1=通知，2=公告
    /// </summary>
    public int NoticeType { get; set; }

    /// <summary>
    /// 公告类型名称
    /// </summary>
    public string NoticeTypeName { get; set; } = string.Empty;

    /// <summary>
    /// 是否置顶：0=否，1=是
    /// </summary>
    public int TopFlag { get; set; }

    /// <summary>
    /// 状态：0=禁用，1=启用
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// 发布时间
    /// </summary>
    public DateTime? PublishTime { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// 创建人
    /// </summary>
    public string? CreatedBy { get; set; }
}