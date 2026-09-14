using SqlSugar;
using System;
using EasyProduct.Models.Entitys.Base;
using EasyProduct.Models.Enums.Crm;

namespace EasyProduct.Models.Entitys.Crm;

/// <summary>
/// 盘点单实体类
/// </summary>
/// <remarks>
/// 盘点单主表，用于管理仓库库存盘点。
///
/// 业务规则：
/// - 盘点单编号格式：SC-{year}-{sequence:04d}
/// - 状态流转：draft → counting → completed
/// - 完成盘点时自动调整库存并创建流水记录
/// - 盘点差异 = 实盘数量 - 系统数量
///
/// 数据关联：
/// - 仓库（crm_warehouse）：通过 warehouseId 关联
/// - 盘点明细（crm_stock_check_item）：一个盘点单包含多个明细
/// - 库存账面（crm_stock）：完成盘点时调整库存
/// </remarks>
[SugarTable("crm_stock_check", "盘点单表")]
public class StockCheck : BaseEntity
{
    /// <summary>
    /// 盘点单编号（唯一）
    /// </summary>
    /// <remarks>
    /// 格式：SC-{year}-{sequence:04d}
    /// 例如：SC-2026-0001
    /// </remarks>
    [SugarColumn(Length = 20, ColumnDescription = "盘点单编号")]
    public string CheckNo { get; set; } = string.Empty;

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
    /// 盘点人
    /// </summary>
    /// <remarks>
    /// 执行盘点操作的人员姓名
    /// 例如：李建国、王芳
    /// </remarks>
    [SugarColumn(Length = 50, ColumnDescription = "盘点人")]
    public string Checker { get; set; } = string.Empty;

    /// <summary>
    /// 盘点日期
    /// </summary>
    /// <remarks>
    /// 格式：YYYY-MM-DD
    /// </remarks>
    [SugarColumn(ColumnDescription = "盘点日期")]
    public DateTime CheckDate { get; set; }

    /// <summary>
    /// 盘点状态
    /// </summary>
    /// <remarks>
    /// - Draft = 草稿
    /// - Counting = 盘点中
    /// - Completed = 已完成
    /// </remarks>
    [SugarColumn(ColumnDescription = "盘点状态")]
    public StockCheckStatus Status { get; set; } = StockCheckStatus.Draft;

    /// <summary>
    /// 备注
    /// </summary>
    /// <remarks>
    /// 可选字段，记录盘点的详细说明
    /// 例如：月度盘点、季度大盘点
    /// </remarks>
    [SugarColumn(Length = 500, ColumnDescription = "备注", IsNullable = true)]
    public string? Remark { get; set; }
}

/// <summary>
/// 盘点明细实体类
/// </summary>
/// <remarks>
/// 盘点明细表，用于记录盘点单的具体商品明细。
///
/// 业务规则：
/// - 系统数量：从库存账面获取
/// - 实盘数量：盘点时实际清点的数量
/// - 盘点差异 = 实盘数量 - 系统数量
/// - 完成盘点时，根据差异调整库存
/// </remarks>
[SugarTable("crm_stock_check_item", "盘点明细表")]
public class StockCheckItem : BaseEntity
{
    /// <summary>
    /// 盘点单ID
    /// </summary>
    /// <remarks>
    /// 关联 crm_stock_check 表的 Id 字段
    /// </remarks>
    [SugarColumn(ColumnDataType = "varchar(36)", ColumnDescription = "盘点单ID")]
    public string CheckId { get; set; } = string.Empty;

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
    /// 系统数量
    /// </summary>
    /// <remarks>
    /// 从库存账面获取的系统库存数量
    /// 创建盘点单时自动填充
    /// </remarks>
    [SugarColumn(ColumnDescription = "系统数量")]
    public int SystemQty { get; set; } = 0;

    /// <summary>
    /// 实盘数量
    /// </summary>
    /// <remarks>
    /// 盘点时实际清点的数量
    /// 初始值为 0，盘点时填写
    /// </remarks>
    [SugarColumn(ColumnDescription = "实盘数量")]
    public int CountedQty { get; set; } = 0;

    /// <summary>
    /// 盘点差异
    /// </summary>
    /// <remarks>
    /// diff = countedQty - systemQty
    /// 正数表示盘盈，负数表示盘亏
    /// 完成盘点时根据此值调整库存
    /// </remarks>
    [SugarColumn(ColumnDescription = "盘点差异")]
    public int Diff { get; set; } = 0;
}