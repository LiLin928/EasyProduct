using SqlSugar;
using System;
using EasyProduct.Models.Entitys.Base;
using EasyProduct.Models.Enums.Crm;

namespace EasyProduct.Models.Entitys.Crm;

[SugarTable("crm_payment", "收付款表")]
public class Payment : BaseEntity
{
    [SugarColumn(Length = 20, ColumnDescription = "收付款编号")]
    public string PaymentNo { get; set; } = string.Empty;

    [SugarColumn(ColumnDescription = "收付款类型")]
    public PaymentType Type { get; set; } = PaymentType.Receipt;

    [SugarColumn(ColumnDescription = "订单类型")]
    public InvoiceOrderType OrderType { get; set; } = InvoiceOrderType.Sales;

    [SugarColumn(Length = 20, ColumnDescription = "订单编号")]
    public string OrderNo { get; set; } = string.Empty;

    [SugarColumn(Length = 200, ColumnDescription = "往来单位名称")]
    public string PartyName { get; set; } = string.Empty;

    [SugarColumn(DecimalDigits = 2, ColumnDescription = "金额")]
    public decimal Amount { get; set; } = 0m;

    [SugarColumn(ColumnDescription = "付款方式")]
    public PaymentMethod Method { get; set; } = PaymentMethod.Bank;

    [SugarColumn(ColumnDescription = "收付款状态")]
    public PaymentStatus Status { get; set; } = PaymentStatus.Draft;

    [SugarColumn(Length = 500, IsNullable = true, ColumnDescription = "备注")]
    public string? Remark { get; set; }
}
