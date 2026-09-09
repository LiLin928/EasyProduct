using SqlSugar;
using EasyProduct.Models.Enums.Mall;
using EasyProduct.Models.Entitys.Base;

namespace EasyProduct.Models.Entitys.Mall;

/// <summary>
/// 支付记录实体类
/// </summary>
/// <remarks>
/// 支付记录表，记录订单支付信息、支付渠道、支付状态等。
/// 一个订单可能对应多条支付记录（支付失败重试场景）。
/// 支付成功后，更新订单状态和支付时间。
/// </remarks>
[SugarTable("mall_payment", "支付记录表")]
public class Payment : BaseEntity
{
    /// <summary>
    /// 支付单号
    /// </summary>
    /// <remarks>
    /// 业务支付单号，按规则生成，格式如：PAY + YYYYMMDDHHMMSS + 随机数。
    /// 用于对外展示，不使用 GUID。
    /// </remarks>
    [SugarColumn(Length = 50, IsNullable = false)]
    public string PaymentNo { get; set; } = string.Empty;

    /// <summary>
    /// 订单ID
    /// </summary>
    /// <remarks>
    /// 关联订单表 mall_order 的 Id 字段。
    /// 支付记录归属于该订单。
    /// </remarks>
    [SugarColumn(Length = 36, IsNullable = false)]
    public string OrderId { get; set; } = string.Empty;

    /// <summary>
    /// 会员ID
    /// </summary>
    /// <remarks>
    /// 关联会员表 mall_member 的 Id 字段。
    /// 支付记录归属于该会员。
    /// </remarks>
    [SugarColumn(Length = 36, IsNullable = false)]
    public string MemberId { get; set; } = string.Empty;

    /// <summary>
    /// 支付金额
    /// </summary>
    /// <remarks>
    /// 本次支付的金额，保留 2 位小数。
    /// 通常等于订单的实付金额。
    /// </remarks>
    [SugarColumn(Length = 18, DecimalDigits = 2, IsNullable = false)]
    public decimal Amount { get; set; }

    /// <summary>
    /// 支付方式
    /// </summary>
    /// <remarks>
    /// 使用 PaymentMethod 枚举：
    /// - WechatPay: 微信支付
    /// - Balance: 余额支付
    /// </remarks>
    public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.WechatPay;

    /// <summary>
    /// 支付渠道
    /// </summary>
    /// <remarks>
    /// 支付渠道标识：
    /// - jsapi: 微信小程序支付
    /// - h5: H5支付
    /// - native: 扫码支付
    /// - app: APP支付
    /// </remarks>
    [SugarColumn(Length = 20, IsNullable = false)]
    public string PaymentChannel { get; set; } = string.Empty;

    /// <summary>
    /// 支付状态
    /// </summary>
    /// <remarks>
    /// 使用 PaymentStatus 枚举：
    /// - Pending: 待支付
    /// - Success: 支付成功
    /// - Failed: 支付失败
    /// - Refunded: 已退款
    /// </remarks>
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

    /// <summary>
    /// 第三方交易号
    /// </summary>
    /// <remarks>
    /// 第三方支付平台的交易号，如微信支付的交易号。
    /// 支付成功后由第三方返回。
    /// 未支付或支付失败时为 null。
    /// </remarks>
    [SugarColumn(Length = 100, IsNullable = true)]
    public string? ThirdPartyNo { get; set; }

    /// <summary>
    /// 支付时间
    /// </summary>
    /// <remarks>
    /// 支付成功后的时间戳。
    /// 未支付或支付失败时为 null。
    /// </remarks>
    [SugarColumn(IsNullable = true)]
    public DateTime? PaymentTime { get; set; }
}