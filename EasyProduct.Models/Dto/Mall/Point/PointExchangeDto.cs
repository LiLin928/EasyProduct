using System.ComponentModel.DataAnnotations;

namespace EasyProduct.Models.Dto.Mall.Point;

/// <summary>
/// 积分兑换DTO
/// </summary>
public class PointExchangeDto
{
    /// <summary>
    /// 兑换ID
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// 会员ID
    /// </summary>
    public string MemberId { get; set; } = string.Empty;

    /// <summary>
    /// 兑换类型
    /// </summary>
    public int ExchangeType { get; set; }

    /// <summary>
    /// 优惠券ID
    /// </summary>
    public string? CouponId { get; set; }

    /// <summary>
    /// 消耗积分
    /// </summary>
    public int Points { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    public bool Status { get; set; }

    /// <summary>
    /// 失败原因
    /// </summary>
    public string? FailReason { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreateTime { get; set; }
}