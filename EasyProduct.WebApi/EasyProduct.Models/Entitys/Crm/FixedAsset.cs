using SqlSugar;
using System;
using EasyProduct.Models.Entitys.Base;
using EasyProduct.Models.Enums.Crm;

namespace EasyProduct.Models.Entitys.Crm;

[SugarTable("crm_fixed_asset", "固定资产表")]
public class FixedAsset : BaseEntity
{
    [SugarColumn(Length = 20, ColumnDescription = "资产编号")]
    public string AssetNo { get; set; } = string.Empty;

    [SugarColumn(Length = 200, ColumnDescription = "资产名称")]
    public string Name { get; set; } = string.Empty;

    [SugarColumn(Length = 50, ColumnDescription = "资产类别")]
    public string Category { get; set; } = string.Empty;

    [SugarColumn(DecimalDigits = 2, ColumnDescription = "原值")]
    public decimal OriginalValue { get; set; } = 0m;

    [SugarColumn(ColumnDescription = "购置日期")]
    public DateTime PurchaseDate { get; set; }

    [SugarColumn(ColumnDescription = "折旧方法")]
    public DepreciationMethod DepreciationMethod { get; set; } = DepreciationMethod.StraightLine;

    [SugarColumn(DecimalDigits = 2, ColumnDescription = "残值")]
    public decimal SalvageValue { get; set; } = 0m;

    [SugarColumn(ColumnDescription = "使用年限")]
    public int UsefulYears { get; set; } = 0;

    [SugarColumn(DecimalDigits = 2, ColumnDescription = "当前价值")]
    public decimal CurrentValue { get; set; } = 0m;

    [SugarColumn(ColumnDescription = "资产状态")]
    public AssetStatus Status { get; set; } = AssetStatus.Active;

    [SugarColumn(Length = 500, IsNullable = true, ColumnDescription = "备注")]
    public string? Remark { get; set; }
}
