using System.ComponentModel.DataAnnotations;

namespace EasyProduct.Models.Dto.Mall.Refund;

/// <summary>
/// 退款验货 DTO
/// </summary>
/// <remarks>
/// 用于管理员验收退货商品，确认是否通过验收
/// </remarks>
public class RefundInspectDto
{
    /// <summary>
    /// 是否验收通过
    /// </summary>
    /// <remarks>
    /// true 表示验收通过，可以退款；false 表示验收不通过
    /// </remarks>
    [Required(ErrorMessage = "验收结果不能为空")]
    public bool Passed { get; set; }

    /// <summary>
    /// 验收备注
    /// </summary>
    /// <remarks>
    /// 验收时填写的备注信息，不通过时需要说明原因
    /// </remarks>
    [StringLength(500, ErrorMessage = "验收备注长度不能超过500")]
    public string? InspectRemark { get; set; }
}