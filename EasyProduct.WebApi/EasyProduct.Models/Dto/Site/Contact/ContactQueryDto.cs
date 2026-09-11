namespace EasyProduct.Models.Dto.Site.Contact;

/// <summary>
/// 留言查询DTO
/// </summary>
public class ContactQueryDto
{
    /// <summary>
    /// 姓名（模糊搜索）
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// 联系电话（模糊搜索）
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// 电子邮箱（模糊搜索）
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// 公司名称（模糊搜索）
    /// </summary>
    public string? Company { get; set; }

    /// <summary>
    /// 主题（模糊搜索）
    /// </summary>
    public string? Subject { get; set; }

    /// <summary>
    /// 状态：0=未读，1=已读，2=已回复
    /// </summary>
    public int? Status { get; set; }

    /// <summary>
    /// 开始时间（创建时间范围查询）
    /// </summary>
    public DateTime? StartTime { get; set; }

    /// <summary>
    /// 结束时间（创建时间范围查询）
    /// </summary>
    public DateTime? EndTime { get; set; }

    /// <summary>
    /// 页码（从1开始）
    /// </summary>
    public int PageIndex { get; set; } = 1;

    /// <summary>
    /// 每页条数
    /// </summary>
    public int PageSize { get; set; } = 10;
}