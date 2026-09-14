using SqlSugar;
using System;
using EasyProduct.Models.Entitys.Base;
using EasyProduct.Models.Enums.Crm;

namespace EasyProduct.Models.Entitys.Crm;

[SugarTable("crm_arap", "应收应付台账表")]
public class Arap : BaseEntity
{
    [SugarColumn(ColumnDescription = "订单类型")]
    public InvoiceOrderType OrderType { get; set; } = InvoiceOrderType.Sales;

    [SugarColumn(Length = 20, ColumnDescription = "订单编号")]
    public string OrderNo { get; set; } = string.Empty;

    [SugarColumn(Length = 200, ColumnDescription = "往来单位名称")]
    public string PartyName { get; set; } = string.Empty;

    [SugarColumn(DecimalDigits = 2, ColumnDescription = "应收/付金额")]
    public decimal Receivable { get; set; } = 0m;

    [SugarColumn(DecimalDigits = 2, ColumnDescription = "已收/付金额")]
    public decimal Received { get; set; } = 0m;

    [SugarColumn(DecimalDigits = 2, ColumnDescription = "未收/付余额")]
    public decimal Balance { get; set; } = 0m;

    [SugarColumn(ColumnDescription = "账龄")]
    public Aging Aging { get; set; } = Aging.Days0to30;

    [SugarColumn(ColumnDescription = "结算状态")]
    public ArapStatus Status { get; set; } = ArapStatus.Unsettled;
}
