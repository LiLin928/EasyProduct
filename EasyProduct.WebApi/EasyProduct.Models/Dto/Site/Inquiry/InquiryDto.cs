namespace EasyProduct.Models.Dto.Site.Inquiry;

/// <summary>
/// 询价单DTO
/// </summary>
public class InquiryDto
{
    /// <summary>
    /// 询价单ID
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// 询价单号
    /// </summary>
    public string InquiryNo { get; set; } = string.Empty;

    /// <summary>
    /// 公司名称
    /// </summary>
    public string CompanyName { get; set; } = string.Empty;

    /// <summary>
    /// 公司名称（英文）
    /// </summary>
    public string? CompanyNameEn { get; set; }

    /// <summary>
    /// 联系人姓名
    /// </summary>
    public string ContactName { get; set; } = string.Empty;

    /// <summary>
    /// 联系人姓名（英文）
    /// </summary>
    public string? ContactNameEn { get; set; }

    /// <summary>
    /// 联系电话
    /// </summary>
    public string Phone { get; set; } = string.Empty;

    /// <summary>
    /// 电子邮箱
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// 国家
    /// </summary>
    public string? Country { get; set; }

    /// <summary>
    /// 省份
    /// </summary>
    public string? Province { get; set; }

    /// <summary>
    /// 城市
    /// </summary>
    public string? City { get; set; }

    /// <summary>
    /// 详细地址
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// 详细地址（英文）
    /// </summary>
    public string? AddressEn { get; set; }

    /// <summary>
    /// 备注信息
    /// </summary>
    public string? Remark { get; set; }

    /// <summary>
    /// 状态：0=待处理，1=已跟进，2=已转客户，3=已关闭
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// 客户ID
    /// </summary>
    public string? CustomerId { get; set; }

    /// <summary>
    /// 跟进时间
    /// </summary>
    public DateTime? FollowedAt { get; set; }

    /// <summary>
    /// 跟进人ID
    /// </summary>
    public string? FollowedBy { get; set; }

    /// <summary>
    /// 转客户时间
    /// </summary>
    public DateTime? ConvertedAt { get; set; }

    /// <summary>
    /// 转客户操作人ID
    /// </summary>
    public string? ConvertedBy { get; set; }

    /// <summary>
    /// 关闭时间
    /// </summary>
    public DateTime? ClosedAt { get; set; }

    /// <summary>
    /// 关闭操作人ID
    /// </summary>
    public string? ClosedBy { get; set; }

    /// <summary>
    /// 关闭原因
    /// </summary>
    public string? CloseReason { get; set; }

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

    /// <summary>
    /// 询价明细列表
    /// </summary>
    public List<InquiryItemDto>? Items { get; set; }
}