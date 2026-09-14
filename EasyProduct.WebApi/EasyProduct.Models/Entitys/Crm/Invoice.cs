using SqlSugar;
using System;
using EasyProduct.Models.Entitys.Base;
using EasyProduct.Models.Enums.Crm;

namespace EasyProduct.Models.Entitys.Crm;

/// <summary>
/// 发票实体类
/// </summary>
/// <remarks>
/// 发票表，用于管理销售开票和采购进项。
///
/// 业务规则：
/// - 发票编号格式：INV-{year}-{sequence:04d}
/// - 销项发票关联销售订单，进项发票关联采购订单
/// - 发票金额 = 未税金额 + 税额
/// - 仅草稿状态可修改和删除
/// - 已开具的发票可作废
/// </remarks>
[SugarTable("crm_invoice", "发票表")]
public class Invoice : BaseEntity
{
    /// <summary>
    /// 发票编号（唯一）
    /// </summary>
    [SugarColumn(Length = 20, ColumnDescription = "发票编号")]
    public string InvoiceNo { get; set; } = string.Empty;

    /// <summary>
    /// 发票类型
    /// </summary>
    [SugarColumn(ColumnDescription = "发票类型")]
    public InvoiceType Type { get; set; } = InvoiceType.Output;

    /// <summary>
    /// 订单类型
    /// </summary>
    [SugarColumn(ColumnDescription = "订单类型")]
    public InvoiceOrderType OrderType { get; set; } = InvoiceOrderType.Sales;

    /// <summary>
    /// 订单编号
    /// </summary>
    [SugarColumn(Length = 20, ColumnDescription = "订单编号")]
    public string OrderNo { get; set; } = string.Empty;

    /// <summary>
    /// 往来单位名称
    /// </summary>
    [SugarColumn(Length = 200, ColumnDescription = "往来单位名称")]
    public string PartyName { get; set; } = string.Empty;

    /// <summary>
    /// 未税金额
    /// </summary>
    [SugarColumn(DecimalDigits = 2, ColumnDescription = "未税金额")]
    public decimal Amount { get; set; } = 0m;

    /// <summary>
    /// 税率（百分比）
    /// </summary>
    [SugarColumn(DecimalDigits = 2, ColumnDescription = "税率")]
    public decimal TaxRate { get; set; } = 0m;

    /// <summary>
    /// 税额
    /// </summary>
    [SugarColumn(DecimalDigits = 2, ColumnDescription = "税额")]
    public decimal TaxAmount { get; set; } = 0m;

    /// <summary>
    /// 价税合计
    /// </summary>
    [SugarColumn(DecimalDigits = 2, ColumnDescription = "价税合计")]
    public decimal Total { get; set; } = 0m;

    /// <summary>
    /// 开票日期
    /// </summary>
    [SugarColumn(ColumnDescription = "开票日期")]
    public DateTime IssueDate { get; set; }

    /// <summary>
    /// 发票状态
    /// </summary>
    [SugarColumn(ColumnDescription = "发票状态")]
    public InvoiceStatus Status { get; set; } = InvoiceStatus.Draft;

    /// <summary>
    /// 备注
    /// </summary>
    [SugarColumn(Length = 500, IsNullable = true, ColumnDescription = "备注")]
    public string? Remark { get; set; }
}