using SqlSugar;
using System;
using EasyProduct.Models.Entitys.Base;
using EasyProduct.Models.Enums.Crm;

namespace EasyProduct.Models.Entitys.Crm;

/// <summary>
/// 出入库流水实体类
/// </summary>
/// <remarks>
/// 出入库流水表，用于记录库存的所有变动历史。
///
/// 业务规则：
/// - 每次库存变动都会创建一条流水记录
/// - 流水记录不可修改、不可删除（审计追溯）
/// - type 字段标识入库或出库
/// - sourceType 字段标识流水来源（采购入库、销售出库等）
/// - sourceOrderNo 字段记录来源单据编号
///
/// 数据关联：
/// - 仓库（crm_warehouse）：通过 warehouseId 关联
/// - SKU（product_sku）：通过 skuCode 关联
/// - 库存账面（crm_stock）：流水记录影响库存数量
/// </remarks>
[SugarTable("crm_stock_record", "出入库流水表")]
public class StockRecord : BaseEntity
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
    /// 出入库类型
    /// </summary>
    /// <remarks>
    /// - In = 入库
    /// - Out = 出库
    /// </remarks>
    [SugarColumn(ColumnDescription = "出入库类型")]
    public StockRecordType Type { get; set; } = StockRecordType.In;

    /// <summary>
    /// 流水来源类型
    /// </summary>
    /// <remarks>
    /// - PurchaseIn = 采购入库
    /// - SalesOut = 销售出库
    /// - MallOut = 商城出库
    /// - CheckAdjust = 盘点调整
    /// - ReversalReturn = 冲销退回
    /// </remarks>
    [SugarColumn(ColumnDescription = "流水来源类型")]
    public StockRecordSourceType SourceType { get; set; } = StockRecordSourceType.PurchaseIn;

    /// <summary>
    /// 来源单据编号
    /// </summary>
    /// <remarks>
    /// 记录来源单据的编号，用于审计追溯
    /// 例如：PO-2026-0001（采购订单）、SO-2026-0003（销售订单）
    /// </remarks>
    [SugarColumn(Length = 50, ColumnDescription = "来源单据编号")]
    public string SourceOrderNo { get; set; } = string.Empty;

    /// <summary>
    /// 数量
    /// </summary>
    /// <remarks>
    /// 出入库的数量，始终为正数
    /// 通过 type 字段区分入库和出库
    /// </remarks>
    [SugarColumn(ColumnDescription = "数量")]
    public int Quantity { get; set; } = 0;

    /// <summary>
    /// 操作人
    /// </summary>
    /// <remarks>
    /// 执行出入库操作的人员姓名
    /// 例如：李建国、张伟、系统自动
    /// </remarks>
    [SugarColumn(Length = 50, ColumnDescription = "操作人")]
    public string Operator { get; set; } = string.Empty;

    /// <summary>
    /// 备注
    /// </summary>
    /// <remarks>
    /// 可选字段，记录出入库的详细说明
    /// 例如：采购入库、销售出库、盘点调整
    /// </remarks>
    [SugarColumn(Length = 500, ColumnDescription = "备注", IsNullable = true)]
    public string? Remark { get; set; }
}