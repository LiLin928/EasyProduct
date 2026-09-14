using SqlSugar;
using System;
using EasyProduct.Models.Entitys.Base;
using EasyProduct.Models.Enums.Crm;

namespace EasyProduct.Models.Entitys.Crm;

/// <summary>
/// 销售订单实体类
/// </summary>
/// <remarks>
/// 销售订单主表，用于管理 B2B 销售订单。
/// 订单编号格式：SO-{year}-{sequence:04d}（如：SO-2026-0001）
///
/// 状态流转：
/// - draft（草稿）→ confirmed（已确认）或 cancelled（已取消）
/// - confirmed（已确认）→ shipped（已发货）或 cancelled（已取消）
/// - shipped（已发货）→ completed（已完成）
///
/// 业务规则：
/// - 仅草稿状态可修改、删除
/// - 发货前检查库存是否充足
/// - 发货时自动创建出库记录并扣减库存
///
/// 金额计算：
/// - subtotalAmount = sum(items.amount)
/// - taxAmount = sum(items.taxAmount)
/// - totalAmount = subtotalAmount + taxAmount
/// </remarks>
[SugarTable("crm_sales_order", "销售订单表")]
public class SalesOrder : BaseEntity
{
    /// <summary>
    /// 订单编号（唯一）
    /// </summary>
    /// <remarks>
    /// 格式：SO-{year}-{sequence:04d}
    /// 例如：SO-2026-0001
    /// </remarks>
    [SugarColumn(Length = 20, ColumnDescription = "订单编号")]
    public string OrderNo { get; set; } = string.Empty;

    /// <summary>
    /// 客户ID
    /// </summary>
    /// <remarks>
    /// 关联 crm_customer 表的 Id 字段
    /// </remarks>
    [SugarColumn(ColumnDataType = "varchar(36)", ColumnDescription = "客户ID")]
    public string CustomerId { get; set; } = string.Empty;

    /// <summary>
    /// 客户名称（冗余字段）
    /// </summary>
    /// <remarks>
    /// 从 crm_customer 表复制，避免频繁关联查询
    /// </remarks>
    [SugarColumn(Length = 100, ColumnDescription = "客户名称")]
    public string CustomerName { get; set; } = string.Empty;

    /// <summary>
    /// 业务员姓名
    /// </summary>
    /// <remarks>
    /// 可选字段，可以为空
    /// </remarks>
    [SugarColumn(Length = 50, ColumnDescription = "业务员姓名", IsNullable = true)]
    public string? SalesPersonName { get; set; }

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
    /// 例如：Net 30（30天付款）、Net 45、Prepaid（预付款）、COD（货到付款）
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
    /// - Shipped = 已发货
    /// - Completed = 已完成
    /// - Cancelled = 已取消
    /// </remarks>
    [SugarColumn(ColumnDescription = "订单状态")]
    public SalesOrderStatus OrderStatus { get; set; } = SalesOrderStatus.Draft;

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
/// 销售订单明细实体类
/// </summary>
/// <remarks>
/// 销售订单明细表，用于管理订单的商品明细。
///
/// 金额计算：
/// - amount = price × quantity
/// - taxAmount = amount × taxRate / 100
/// - totalAmount = amount + taxAmount
/// </remarks>
[SugarTable("crm_sales_order_item", "销售订单明细表")]
public class SalesOrderItem : BaseEntity
{
    /// <summary>
    /// 订单ID
    /// </summary>
    /// <remarks>
    /// 关联 crm_sales_order 表的 Id 字段
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
    /// 商品销售单价
    /// </remarks>
    [SugarColumn(ColumnDescription = "单价", DecimalDigits = 2)]
    public decimal Price { get; set; } = 0m;

    /// <summary>
    /// 数量
    /// </summary>
    /// <remarks>
    /// 购买数量
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