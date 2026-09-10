using SqlSugar;
using EasyProduct.Models.Enums.Mall;
using EasyProduct.Models.Entitys.Base;

namespace EasyProduct.Models.Entitys.Mall;

/// <summary>
/// 优惠券实体类
/// </summary>
/// <remarks>
/// 优惠券主表，记录优惠券基本信息、使用规则、发放数量等。
/// 支持两种优惠券类型：满减券、折扣券。
/// 可设置适用商品范围（全场通用或指定商品）。
/// </remarks>
[SugarTable("mall_coupon", "优惠券表")]
public class Coupon : BaseEntity
{
    /// <summary>
    /// 优惠券名称
    /// </summary>
    [SugarColumn(Length = 100, IsNullable = false, ColumnDescription = "优惠券名称")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 优惠券类型
    /// </summary>
    /// <remarks>
    /// 使用 CouponType 枚举：
    /// - FullReduction: 满减券
    /// - Discount: 折扣券
    /// </remarks>
    public CouponType Type { get; set; } = CouponType.FullReduction;

    /// <summary>
    /// 优惠值
    /// </summary>
    /// <remarks>
    /// - 满减券：表示减免金额（元）
    /// - 折扣券：表示折扣比例（如 8.5 表示 8.5 折）
    /// 保留 2 位小数。
    /// </remarks>
    [SugarColumn(Length = 18, DecimalDigits = 2, IsNullable = false, ColumnDescription = "优惠值")]
    public decimal Value { get; set; }

    /// <summary>
    /// 最低消费金额
    /// </summary>
    /// <remarks>
    /// 使用优惠券的最低订单金额门槛。
    /// 保留 2 位小数。
    /// </remarks>
    [SugarColumn(Length = 18, DecimalDigits = 2, IsNullable = false, ColumnDescription = "最低消费金额")]
    public decimal MinAmount { get; set; }

    /// <summary>
    /// 开始时间
    /// </summary>
    /// <remarks>
    /// 优惠券有效期开始时间。
    /// </remarks>
    [SugarColumn(IsNullable = false, ColumnDescription = "开始时间")]
    public DateTime StartTime { get; set; }

    /// <summary>
    /// 结束时间
    /// </summary>
    /// <remarks>
    /// 优惠券有效期结束时间。
    /// </remarks>
    [SugarColumn(IsNullable = false, ColumnDescription = "结束时间")]
    public DateTime EndTime { get; set; }

    /// <summary>
    /// 发放总数
    /// </summary>
    /// <remarks>
    /// 优惠券的总发放数量。
    /// </remarks>
    [SugarColumn(IsNullable = false, ColumnDescription = "发放总数")]
    public int TotalCount { get; set; }

    /// <summary>
    /// 已领取数量
    /// </summary>
    /// <remarks>
    /// 已被用户领取的优惠券数量。
    /// 不能超过 TotalCount。
    /// </remarks>
    [SugarColumn(IsNullable = false, ColumnDescription = "已领取数量")]
    public int ClaimedCount { get; set; } = 0;

    /// <summary>
    /// 已使用数量
    /// </summary>
    /// <remarks>
    /// 已被使用的优惠券数量。
    /// </remarks>
    [SugarColumn(IsNullable = false, ColumnDescription = "已使用数量")]
    public int UsedCount { get; set; } = 0;

    /// <summary>
    /// 适用商品ID列表
    /// </summary>
    /// <remarks>
    /// JSON 数组格式存储适用的商品 SPU ID 列表。
    /// 为空或 null 表示全场通用。
    /// 示例：["spu_id_1", "spu_id_2"]
    /// </remarks>
    [SugarColumn(ColumnDataType = "text", IsNullable = true, ColumnDescription = "适用商品ID列表")]
    public string? ProductIds { get; set; }

    /// <summary>
    /// 优惠券说明
    /// </summary>
    /// <remarks>
    /// 优惠券的使用说明和限制条件。
    /// </remarks>
    [SugarColumn(Length = 500, IsNullable = true, ColumnDescription = "优惠券说明")]
    public string? Description { get; set; }
}