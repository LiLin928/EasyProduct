namespace EasyProduct.Models.Dto.Site.Inquiry;

/// <summary>
/// 询价明细DTO
/// </summary>
public class InquiryItemDto
{
    /// <summary>
    /// 明细ID
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// 询价单ID
    /// </summary>
    public string InquiryId { get; set; } = string.Empty;

    /// <summary>
    /// 产品名称
    /// </summary>
    public string ProductName { get; set; } = string.Empty;

    /// <summary>
    /// 产品名称（英文）
    /// </summary>
    public string? ProductNameEn { get; set; }

    /// <summary>
    /// 产品编码
    /// </summary>
    public string? ProductCode { get; set; }

    /// <summary>
    /// 规格型号
    /// </summary>
    public string? Specification { get; set; }

    /// <summary>
    /// 数量
    /// </summary>
    public int? Quantity { get; set; }

    /// <summary>
    /// 单位
    /// </summary>
    public string? Unit { get; set; }

    /// <summary>
    /// 备注信息
    /// </summary>
    public string? Remark { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; }
}