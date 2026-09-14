namespace EasyProduct.Models.Enums.Crm;

/// <summary>
/// 发票类型枚举
/// </summary>
/// <remarks>
/// 用于标识发票的开票方向
/// </remarks>
public enum InvoiceType
{
    /// <summary>
    /// 销项发票（销售开票）
    /// </summary>
    Output = 1,

    /// <summary>
    /// 进项发票（采购收票）
    /// </summary>
    Input = 2
}

/// <summary>
/// 发票订单类型枚举
/// </summary>
/// <remarks>
/// 用于标识发票关联的订单类型
/// </remarks>
public enum InvoiceOrderType
{
    /// <summary>
    /// 销售订单
    /// </summary>
    Sales = 1,

    /// <summary>
    /// 采购订单
    /// </summary>
    Purchase = 2
}

/// <summary>
/// 发票状态枚举
/// </summary>
/// <remarks>
/// 发票状态流转规则：
/// - draft（草稿）→ issued（已开具）或 voided（已作废）
/// - issued（已开具）→ voided（已作废）
/// - voided（已作废）→ 终态
///
/// 注意：枚举值使用字符串字面量，与 mockjs 保持一致
/// </remarks>
public enum InvoiceStatus
{
    /// <summary>
    /// 草稿
    /// </summary>
    /// <remarks>
    /// - 可修改
    /// - 可删除
    /// - 可开具或作废
    /// </remarks>
    Draft = 1,

    /// <summary>
    /// 已开具
    /// </summary>
    /// <remarks>
    /// - 不可修改
    /// - 不可删除
    /// - 可作废
    /// </remarks>
    Issued = 2,

    /// <summary>
    /// 已作废
    /// </summary>
    /// <remarks>
    /// - 终态，不可变更
    /// </remarks>
    Voided = 3
}

/// <summary>
/// 收付款类型枚举
/// </summary>
/// <remarks>
/// 用于标识收付款的方向
/// </remarks>
public enum PaymentType
{
    /// <summary>
    /// 收款
    /// </summary>
    Receipt = 1,

    /// <summary>
    /// 付款
    /// </summary>
    Payment = 2
}

/// <summary>
/// 收付款方式枚举
/// </summary>
/// <remarks>
/// 用于标识收付款的方式
/// </remarks>
public enum PaymentMethod
{
    /// <summary>
    /// 现金
    /// </summary>
    Cash = 1,

    /// <summary>
    /// 银行转账
    /// </summary>
    Bank = 2,

    /// <summary>
    /// 微信支付
    /// </summary>
    Wechat = 3
}

/// <summary>
/// 收付款状态枚举
/// </summary>
/// <remarks>
/// 收付款状态流转规则：
/// - draft（草稿）→ confirmed（已确认）或 voided（已作废）
/// - confirmed（已确认）→ voided（已作废）
/// - voided（已作废）→ 终态
/// </remarks>
public enum PaymentStatus
{
    /// <summary>
    /// 草稿
    /// </summary>
    Draft = 1,

    /// <summary>
    /// 已确认
    /// </summary>
    Confirmed = 2,

    /// <summary>
    /// 已作废
    /// </summary>
    Voided = 3
}

/// <summary>
/// 应收应付账龄枚举
/// </summary>
/// <remarks>
/// 用于账龄分析
/// </remarks>
public enum Aging
{
    /// <summary>
    /// 0-30天
    /// </summary>
    Days0to30 = 1,

    /// <summary>
    /// 31-60天
    /// </summary>
    Days31to60 = 2,

    /// <summary>
    /// 61-90天
    /// </summary>
    Days61to90 = 3,

    /// <summary>
    /// 90天以上
    /// </summary>
    Days90Plus = 4
}

/// <summary>
/// 应收应付状态枚举
/// </summary>
/// <remarks>
/// 用于标识应收应付的结算状态
/// </remarks>
public enum ArapStatus
{
    /// <summary>
    /// 已结清
    /// </summary>
    Settled = 1,

    /// <summary>
    /// 未结清
    /// </summary>
    Unsettled = 2
}

/// <summary>
/// 固定资产状态枚举
/// </summary>
/// <remarks>
/// 用于标识固定资产的使用状态
/// </remarks>
public enum AssetStatus
{
    /// <summary>
    /// 在用
    /// </summary>
    Active = 1,

    /// <summary>
    /// 已报废
    /// </summary>
    Scrapped = 2
}

/// <summary>
/// 折旧方法枚举
/// </summary>
/// <remarks>
/// 用于标识固定资产的折旧计算方法
/// </remarks>
public enum DepreciationMethod
{
    /// <summary>
    /// 直线法
    /// </summary>
    StraightLine = 1
}