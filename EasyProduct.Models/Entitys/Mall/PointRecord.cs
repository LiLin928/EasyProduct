using SqlSugar;
using EasyProduct.Models.Enums.Mall;

namespace EasyProduct.Models.Entitys.Mall;

/// <summary>
/// 积分流水实体
/// </summary>
/// <remarks>
/// 记录会员积分变动明细，包括积分收入、支出、冻结、解冻等
/// </remarks>
[SugarTable("mall_point_record")]
public class PointRecord : BaseEntity
{
    /// <summary>
    /// 会员ID
    /// </summary>
    [SugarColumn(Length = 36)]
    public string MemberId { get; set; } = string.Empty;

    /// <summary>
    /// 积分类型
    /// </summary>
    /// <remarks>
    /// 1=收入，2=支出，3=冻结，4=解冻
    /// </remarks>
    public PointType Type { get; set; }

    /// <summary>
    /// 积分来源
    /// </summary>
    /// <remarks>
    /// 1=下单赠送，2=评价赠送，3=签到赠送，4=邀请赠送，5=兑换优惠券，6=订单抵扣，7=系统调整，8=积分过期
    /// </remarks>
    public PointSource Source { get; set; }

    /// <summary>
    /// 积分数
    /// </summary>
    /// <remarks>
    /// 正数表示收入，负数表示支出
    /// </remarks>
    public int Points { get; set; }

    /// <summary>
    /// 变动后余额
    /// </summary>
    /// <remarks>
    /// 记录本次变动后的可用积分余额
    /// </remarks>
    public int Balance { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    [SugarColumn(Length = 500, IsNullable = true)]
    public string? Remark { get; set; }

    /// <summary>
    /// 关联ID
    /// </summary>
    /// <remarks>
    /// 关联业务单号，如订单号、兑换ID等
    /// </remarks>
    [SugarColumn(Length = 36, IsNullable = true)]
    public string? RelatedId { get; set; }
}