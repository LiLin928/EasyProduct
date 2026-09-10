namespace EasyProduct.Models.Dto.Mall.Point;

/// <summary>
/// 积分余额DTO
/// </summary>
public class PointBalanceDto
{
    /// <summary>
    /// 会员ID
    /// </summary>
    public string MemberId { get; set; } = string.Empty;

    /// <summary>
    /// 累计积分
    /// </summary>
    public int TotalPoints { get; set; }

    /// <summary>
    /// 可用积分
    /// </summary>
    public int AvailablePoints { get; set; }

    /// <summary>
    /// 冻结积分
    /// </summary>
    public int FrozenPoints { get; set; }
}