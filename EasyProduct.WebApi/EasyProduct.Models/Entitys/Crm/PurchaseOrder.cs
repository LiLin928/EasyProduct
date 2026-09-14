using SqlSugar;
using System;
using EasyProduct.Models.Entitys.Base;
using EasyProduct.Models.Enums.Crm;

namespace EasyProduct.Models.Entitys.Crm;

/// <summary>
/// 采购订单实体类
/// </summary>
/// <remarks>
/// 采购订单主表，用于管理采购订单。
/// 订单编号格式：PO-{year}-{sequence:04d}（如：PO-2026-0001）
///
/// 状态流转：
/// - draft（草稿）→ confirmed（已确认）或 cancelled（已取消）
/// - confirmed（已确认）→ received（已入库）或 cancelled（已取消）
/// - received（已入库）→ completed（已完成）
///
/// 业务规则：
/// - 仅草稿状态可修改、删除
/// - 入库时自动创建入库记录并增加库存
///
/// 金额计算：
/// - subtotalAmount = sum(items.amount)
/// - taxAmount = sum(items.taxAmount)
/// - totalAmount = subtotalAmount + taxAmount
/// </remarks>
[SugarTable("crm_purchase_order", "采购订单表")]
public class PurchaseOrder : BaseEntity
{
    /// <summary>
    /// 订单编号（唯一）
    /// </summary>
    /// <remarks>
    /// 格式：PO-{year}-{sequence:04d}
    /// 例如：PO-2026-0001
    /// </remarks>
    [SugarColumn(Length = 20, ColumnDescription = "订单编号")]
    public string OrderNo { get; set; } = string.Empty;

    /// <summary>
    /// 供应商ID
    /// </summary>
    /// <remarks>
    /// 关联 crm_supplier 表的 Id 字段
    /// </remarks>
    [SugarColumn(ColumnDataType = "varchar(36)", ColumnDescription = "供应商ID")]
    public string SupplierId { get; set; } = string.Empty;

    /// <summary>
    /// 供应商名称（冗余字段）
    /// </summary>
    /// <remarks>
    /// 从 crm_supplier 表复制，避免频繁关联查询
    /// </remarks>
    [SugarColumn(Length = 100, ColumnDescription = "供应商名称")]
    public string SupplierName { get; set; } = string.Empty;

    /// <summary>
    /// 采购员姓名
    /// </summary>
    /// <remarks>
    /// 可选字段，可以为空
    /// </remarks>
    [SugarColumn(Length = 50, ColumnDescription = "采购员姓名", IsNullable = true)]
    public string? BuyerName { get; set; }

    /// <summary>
    /// 币种代码
    /// </summary>
    /// <remarks>
    /// 默认：CNY（人民币）
    /// 关联 crm_currency 表的 Code 字段
    /// </remarks>
    [SugarColumn(Length = 10, ColumnDescription = "币种代码")]
    public string CurrencyCode { get; set; } = "CNY";

    /// <summary>
    /// 币种符号
    /// </summary>
    /// <remarks>
    /// 默认：¥
    /// </remarks>
    [SugarColumn(Length = 10, ColumnDescription = "币种符号")]
    public string CurrencySymbol { get; set; } = "¥";

    /// <summary>
    /// 付款条款
    /// </summary>
    /// <remarks>
    /// 例如：Net 30（30天付款）、Net 60、Prepaid（预付款）
    /// </remarks>
    [SugarColumn(Length = 50, ColumnDescription = "付款条款")]
    public string PaymentTerms { get; set; } = "Net 30";

    /// <summary>
    /// 交货日期
    /// </summary>
    /// <remarks>
    /// 格式：YYYY-MM-DD
    /// </remarks>
    [SugarColumn(ColumnDescription = "交货日期")]
    public DateTime DeliveryDate { get; set; }

    /// <summary>
    /// 订单状态
    /// </summary>
    /// <remarks>
    /// - Draft = 草稿
    /// - Confirmed = 已确认
    /// - Received = 已入库
    /// - Completed = 已完成
    /// - Cancelled = 已取消
    /// </remarks>
    [SugarColumn(ColumnDescription = "订单状态")]
    public PurchaseOrderStatus OrderStatus { get; set; } = PurchaseOrderStatus.Draft;

    /// <summary>
    /// 小计金额
    /// </summary>
    /// <remarks>
    /// 所有明细金额之和：subtotalAmount = sum(items.amount)
    /// </remarks>
    [SugarColumn(ColumnDescription = "小计金额", DecimalDigits = 2)]
    public decimal SubtotalAmount { get; set; } = 0m;

    /// <summary>
    /// 税额
    /// </summary>
    /// <remarks>
    /// 所有明细税额之和：taxAmount = sum(items.taxAmount)
    /// </remarks>
    [SugarColumn(ColumnDescription = "税额", DecimalDigits = 2)]
    public decimal TaxAmount { get; set; } = 0m;

    /// <summary>
    /// 总金额
    /// </summary>
    /// <remarks>
    /// totalAmount = subtotalAmount + taxAmount
    /// </remarks>
    [SugarColumn(ColumnDescription = "总金额", DecimalDigits = 2)]
    public decimal TotalAmount { get; set; } = 0m;

    /// <summary>
    /// 备注
    /// </summary>
    /// <remarks>
    /// 可选字段，可以为空
    /// </remarks>
    [SugarColumn(Length = 500, ColumnDescription = "备注", IsNullable = true)]
    public string? Remark { get; set; }
}

/// <summary>
/// 采购订单明细实体类
/// </summary>
/// <remarks>
/// 采购订单明细表，用于管理订单的商品明细。
///
/// 金额计算：
/// - amount = price × quantity
/// - taxAmount = amount × taxRate / 100
/// - totalAmount = amount + taxAmount
/// </remarks>
[SugarTable("crm_purchase_order_item", "采购订单明细表")]
public class PurchaseOrderItem : BaseEntity
{
    /// <summary>
    /// 订单ID
    /// </summary>
    /// <remarks>
    /// 关联 crm_purchase_order 表的 Id 字段
    /// </remarks>
    [SugarColumn(ColumnDataType = "varchar(36)", ColumnDescription = "订单ID")]
    public string OrderId { get; set; } = string.Empty;

    /// <summary>
    /// SKU编码
    /// </summary>
    /// <remarks>
    /// 关联 product_sku 表的 Code 字段
    /// </remarks>
    [SugarColumn(Length = 50, ColumnDescription = "SKU编码")]
    public string SkuCode { get; set; } = string.Empty;

    /// <summary>
    /// SKU名称
    /// </summary>
    /// <remarks>
    /// 从 product_sku 表复制，避免频繁关联查询
    /// </remarks>
    [SugarColumn(Length = 200, ColumnDescription = "SKU名称")]
    public string SkuName { get; set; } = string.Empty;

    /// <summary>
    /// 产品名称
    /// </summary>
    /// <remarks>
    /// 从 product_sku 表复制，避免频繁关联查询
    /// </remarks>
    [SugarColumn(Length = 200, ColumnDescription = "产品名称")]
    public string ProductName { get; set; } = string.Empty;

    /// <summary>
    /// 规格
    /// </summary>
    /// <remarks>
    /// 可选字段，可以为空
    /// </remarks>
    [SugarColumn(Length = 200, ColumnDescription = "规格", IsNullable = true)]
    public string? Spec { get; set; }

    /// <summary>
    /// 单价
    /// </summary>
    /// <remarks>
    /// 商品采购单价
    /// </remarks>
    [SugarColumn(ColumnDescription = "单价", DecimalDigits = 2)]
    public decimal Price { get; set; } = 0m;

    /// <summary>
    /// 数量
    /// </summary>
    /// <remarks>
    /// 采购数量
    /// </remarks>
    [SugarColumn(ColumnDescription = "数量")]
    public int Quantity { get; set; } = 0;

    /// <summary>
    /// 税率代码
    /// </summary>
    /// <remarks>
    /// 关联 crm_tax_rate 表的 Code 字段
    /// </remarks>
    [SugarColumn(Length = 10, ColumnDescription = "税率代码")]
    public string TaxRateCode { get; set; } = string.Empty;

    /// <summary>
    /// 税率（百分比）
    /// </summary>
    /// <remarks>
    /// 例如：13.00 表示 13%
    /// </remarks>
    [SugarColumn(ColumnDescription = "税率（百分比）", DecimalDigits = 2)]
    public decimal TaxRate { get; set; } = 0m;

    /// <summary>
    /// 金额
    /// </summary>
    /// <remarks>
    /// amount = price × quantity
    /// </remarks>
    [SugarColumn(ColumnDescription = "金额", DecimalDigits = 2)]
    public decimal Amount { get; set; } = 0m;

    /// <summary>
    /// 税额
    /// </summary>
    /// <remarks>
    /// taxAmount = amount × taxRate / 100
    /// </remarks>
    [SugarColumn(ColumnDescription = "税额", DecimalDigits = 2)]
    public decimal TaxAmount { get; set; } = 0m;

    /// <summary>
    /// 总金额
    /// </summary>
    /// <remarks>
    /// totalAmount = amount + taxAmount
    /// </remarks>
    [SugarColumn(ColumnDescription = "总金额", DecimalDigits = 2)]
    public decimal TotalAmount { get; set; } = 0m;

    /// <summary>
    /// 仓库ID
    /// </summary>
    /// <remarks>
    /// 关联 crm_warehouse 表的 Id 字段
    /// </remarks>
    [SugarColumn(ColumnDataType = "varchar(36)", ColumnDescription = "仓库ID")]
    public string WarehouseId { get; set; } = string.Empty;
}