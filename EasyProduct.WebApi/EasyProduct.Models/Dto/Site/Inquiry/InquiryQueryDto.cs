namespace EasyProduct.Models.Dto.Site.Inquiry;

/// <summary>
/// 询价单查询DTO
/// </summary>
public class InquiryQueryDto
{
    /// <summary>
    /// 询价单号
    /// </summary>
    public string? InquiryNo { get; set; }

    /// <summary>
    /// 公司名称（模糊搜索）
    /// </summary>
    public string? CompanyName { get; set; }

    /// <summary>
    /// 联系人姓名（模糊搜索）
    /// </summary>
    public string? ContactName { get; set; }

    /// <summary>
    /// 联系电话（模糊搜索）
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// 电子邮箱（模糊搜索）
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// 状态：0=待处理，1=已跟进，2=已转客户，3=已关闭
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