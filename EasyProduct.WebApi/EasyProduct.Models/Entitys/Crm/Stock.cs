using SqlSugar;
using System;
using EasyProduct.Models.Entitys.Base;

namespace EasyProduct.Models.Entitys.Crm;

/// <summary>
/// 库存账面实体类
/// </summary>
/// <remarks>
/// 库存账面表，用于管理各仓库的 SKU 库存数量。
///
/// 业务规则：
/// - 同一仓库同一 SKU 只有一条库存记录
/// - total = available + locked（总数量 = 可用数量 + 锁定数量）
/// - 库存预警：available 低于 minLimit 或高于 maxLimit 时触发预警
/// - 库存调整：通过盘点或其他方式调整库存时更新此表
///
/// 数据关联：
/// - 仓库（crm_warehouse）：通过 warehouseId 关联
/// - SKU（product_sku）：通过 skuCode 关联
/// - 出入库流水（crm_stock_record）：记录库存变动历史
/// - 盘点明细（crm_stock_check_item）：盘点时对比库存
/// </remarks>
[SugarTable("crm_stock", "库存账面表")]
public class Stock : BaseEntity
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
    /// 单位
    /// </summary>
    /// <remarks>
    /// 从 product_sku 表复制
    /// 例如：个、件、台
    /// </remarks>
    [SugarColumn(Length = 20, ColumnDescription = "单位")]
    public string Unit { get; set; } = string.Empty;

    /// <summary>
    /// 可用数量
    /// </summary>
    /// <remarks>
    /// 可以销售或出库的数量
    /// available = total - locked
    /// </remarks>
    [SugarColumn(ColumnDescription = "可用数量")]
    public int Available { get; set; } = 0;

    /// <summary>
    /// 锁定数量
    /// </summary>
    /// <remarks>
    /// 已下单但未出库的数量（订单锁定）
    /// locked = sum(未出库订单数量)
    /// </remarks>
    [SugarColumn(ColumnDescription = "锁定数量")]
    public int Locked { get; set; } = 0;

    /// <summary>
    /// 总数量
    /// </summary>
    /// <remarks>
    /// 实际库存总数
    /// total = available + locked
    /// </remarks>
    [SugarColumn(ColumnDescription = "总数量")]
    public int Total { get; set; } = 0;

    /// <summary>
    /// 库存下限
    /// </summary>
    /// <remarks>
    /// 当 available 低于此值时触发低库存预警
    /// 例如：100
    /// </remarks>
    [SugarColumn(ColumnDescription = "库存下限")]
    public int MinLimit { get; set; } = 0;

    /// <summary>
    /// 库存上限
    /// </summary>
    /// <remarks>
    /// 当 available 高于此值时触发高库存预警
    /// 例如：1000
    /// </remarks>
    [SugarColumn(ColumnDescription = "库存上限")]
    public int MaxLimit { get; set; } = 0;
}