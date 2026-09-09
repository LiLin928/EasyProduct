using SqlSugar;
using EasyProduct.Models.Enums.Mall;
using EasyProduct.Models.Entitys.Base;

namespace EasyProduct.Models.Entitys.Mall;

/// <summary>
/// 积分记录实体
/// </summary>
[SugarTable("mall_points_record", "积分记录表")]
public class PointsRecord : BaseEntity
{
    /// <summary>
    /// 会员ID
    /// </summary>
    public Guid MemberId { get; set; }

    /// <summary>
    /// 积分类型
    /// </summary>
    public PointsType PointsType { get; set; }

    /// <summary>
    /// 积分变动(正数为获得，负数为使用)
    /// </summary>
    public int Points { get; set; }

    /// <summary>
    /// 变动后余额
    /// </summary>
    public int Balance { get; set; }

    /// <summary>
    /// 关联订单号
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = true)]
    public string? OrderNo { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    [SugarColumn(Length = 255, IsNullable = true)]
    public string? Remark { get; set; }
}