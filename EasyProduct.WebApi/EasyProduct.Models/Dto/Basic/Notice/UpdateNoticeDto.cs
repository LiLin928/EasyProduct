using System.ComponentModel.DataAnnotations;

namespace EasyProduct.Models.Dto.Basic.Notice;

/// <summary>
/// 更新公告参数
/// </summary>
public class UpdateNoticeDto
{
    /// <summary>
    /// 公告ID
    /// </summary>
    [Required(ErrorMessage = "公告ID不能为空")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// 公告标题
    /// </summary>
    [Required(ErrorMessage = "公告标题不能为空")]
    [MaxLength(200, ErrorMessage = "公告标题不能超过200个字符")]
    public string NoticeTitle { get; set; } = string.Empty;

    /// <summary>
    /// 公告内容（富文本）
    /// </summary>
    [Required(ErrorMessage = "公告内容不能为空")]
    public string NoticeContent { get; set; } = string.Empty;

    /// <summary>
    /// 公告类型：1=通知，2=公告
    /// </summary>
    [Range(1, 2, ErrorMessage = "公告类型只能是1或2")]
    public int NoticeType { get; set; } = 1;

    /// <summary>
    /// 是否置顶：0=否，1=是
    /// </summary>
    [Range(0, 1, ErrorMessage = "置顶标记只能是0或1")]
    public int TopFlag { get; set; } = 0;

    /// <summary>
    /// 状态：0=禁用，1=启用
    /// </summary>
    [Range(0, 1, ErrorMessage = "状态值只能是0或1")]
    public int Status { get; set; } = 1;
}