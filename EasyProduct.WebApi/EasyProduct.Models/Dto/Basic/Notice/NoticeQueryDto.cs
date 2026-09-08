namespace EasyProduct.Models.Dto.Basic.Notice;

/// <summary>
/// 公告查询参数
/// </summary>
public class NoticeQueryDto
{
    /// <summary>
    /// 公告标题（模糊搜索）
    /// </summary>
    public string? NoticeTitle { get; set; }

    /// <summary>
    /// 公告类型：1=通知，2=公告
    /// </summary>
    public int? NoticeType { get; set; }

    /// <summary>
    /// 是否置顶：0=否，1=是
    /// </summary>
    public int? TopFlag { get; set; }

    /// <summary>
    /// 状态：0=禁用，1=启用
    /// </summary>
    public int? Status { get; set; }

    /// <summary>
    /// 页码（从1开始）
    /// </summary>
    public int PageIndex { get; set; } = 1;

    /// <summary>
    /// 每页条数
    /// </summary>
    public int PageSize { get; set; } = 10;
}