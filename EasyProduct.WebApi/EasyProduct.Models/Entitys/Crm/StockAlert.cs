using SqlSugar;
using System;
using EasyProduct.Models.Entitys.Base;
using EasyProduct.Models.Enums.Crm;

namespace EasyProduct.Models.Entitys.Crm;

/// <summary>
/// 库存预警实体类
/// </summary>
/// <remarks>
/// 库存预警表，用于管理库存预警信息。
///
/// 业务规则：
/// - 当库存低于下限或高于上限时自动创建预警
/// - 预警类型：low（低库存）、high（高库存）
/// - 预警状态：pending（待处理）、resolved（已解决）
/// - 解决预警后记录解决时间
///
/// 数据关联：
/// - 仓库（crm_warehouse）：通过 warehouseId 关联
/// - SKU（product_sku）：通过 skuCode 关联
/// - 库存账面（crm_stock）：预警来源于库存监控
/// </remarks>
[SugarTable("crm_stock_alert", "库存预警表")]
public class StockAlert : BaseEntity
{
    /// <summary>
    /// 仓库ID
    /// </summary>
    /// <remarks>
    /// 关联 crm_warehouse 表的 Id 字段
    /// </remarks>
    [SugarColumn(ColumnDataType = "varchar(36)", ColumnDescription = "仓库ID")]
    public string WarehouseId { get; set; } = string.Empty;

    /// <summary>
    /// 仓库名称（冗余字段）
    /// </summary>
    /// <remarks>
    /// 从 crm_warehouse 表复制，避免频繁关联查询
    /// </remarks>
    [SugarColumn(Length = 100, ColumnDescription = "仓库名称")]
    public string WarehouseName { get; set; } = string.Empty;

    /// <summary>
    /// SKU编码
    /// </summary>
    /// <remarks>
    /// 关联 product_sku 表的 Code 字段
    /// </remarks>
    [SugarColumn(Length = 50, ColumnDescription = "SKU编码")]
    public string SkuCode { get; set; } = string.Empty;

    /// <summary>
    /// SKU名称（冗余字段）
    /// </summary>
    /// <remarks>
    /// 从 product_sku 表复制，避免频繁关联查询
    /// </remarks>
    [SugarColumn(Length = 200, ColumnDescription = "SKU名称")]
    public string SkuName { get; set; } = string.Empty;

    /// <summary>
    /// 规格
    /// </summary>
    /// <remarks>
    /// 从 product_sku 表复制
    /// 例如：黑色/主动降噪
    /// </remarks>
    [SugarColumn(Length = 200, ColumnDescription = "规格")]
    public string Spec { get; set; } = string.Empty;

    /// <summary>
    /// 可用数量
    /// </summary>
    /// <remarks>
    /// 触发预警时的库存数量
    /// </remarks>
    [SugarColumn(ColumnDescription = "可用数量")]
    public int Available { get; set; } = 0;

    /// <summary>
    /// 库存下限
    /// </summary>
    /// <remarks>
    /// 从库存账面复制
    /// </remarks>
    [SugarColumn(ColumnDescription = "库存下限")]
    public int MinLimit { get; set; } = 0;

    /// <summary>
    /// 库存上限
    /// </summary>
    /// <remarks>
    /// 从库存账面复制
    /// </remarks>
    [SugarColumn(ColumnDescription = "库存上限")]
    public int MaxLimit { get; set; } = 0;

    /// <summary>
    /// 预警类型
    /// </summary>
    /// <remarks>
    /// - Low = 低库存预警（available 低于 minLimit）
    /// - High = 高库存预警（available 高于 maxLimit）
    /// </remarks>
    [SugarColumn(ColumnDescription = "预警类型")]
    public StockAlertType AlertType { get; set; } = StockAlertType.Low;

    /// <summary>
    /// 预警状态
    /// </summary>
    /// <remarks>
    /// - Pending = 待处理
    /// - Resolved = 已解决
    /// </remarks>
    [SugarColumn(ColumnDescription = "预警状态")]
    public StockAlertStatus Status { get; set; } = StockAlertStatus.Pending;

    /// <summary>
    /// 解决时间
    /// </summary>
    /// <remarks>
    /// 解决预警时记录的时间
    /// </remarks>
    [SugarColumn(ColumnDescription = "解决时间", IsNullable = true)]
    public DateTime? ResolvedAt { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    /// <remarks>
    /// 可选字段，记录预警的详细说明
    /// 例如：库存低于下限，请及时补货
    /// </remarks>
    [SugarColumn(Length = 500, ColumnDescription = "备注", IsNullable = true)]
    public string? Remark { get; set; }
}