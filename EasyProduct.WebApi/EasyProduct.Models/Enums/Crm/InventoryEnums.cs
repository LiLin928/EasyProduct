namespace EasyProduct.Models.Enums.Crm;

/// <summary>
/// 仓库状态枚举
/// </summary>
/// <remarks>
/// 用于标识仓库的启用状态
/// </remarks>
public enum WarehouseStatus
{
    /// <summary>
    /// 启用
    /// </summary>
    Active = 1,

    /// <summary>
    /// 停用
    /// </summary>
    Inactive = 0
}

/// <summary>
/// 库存流水类型枚举
/// </summary>
/// <remarks>
/// 用于标识出入库流水的类型
/// </remarks>
public enum StockRecordType
{
    /// <summary>
    /// 入库
    /// </summary>
    In = 1,

    /// <summary>
    /// 出库
    /// </summary>
    Out = 2
}

/// <summary>
/// 库存流水来源类型枚举
/// </summary>
/// <remarks>
/// 用于标识库存流水的来源单据类型
/// </remarks>
public enum StockRecordSourceType
{
    /// <summary>
    /// 采购入库
    /// </summary>
    PurchaseIn = 1,

    /// <summary>
    /// 销售出库
    /// </summary>
    SalesOut = 2,

    /// <summary>
    /// 商城出库
    /// </summary>
    MallOut = 3,

    /// <summary>
    /// 盘点调整
    /// </summary>
    CheckAdjust = 4,

    /// <summary>
    /// 冲销退回
    /// </summary>
    ReversalReturn = 5
}

/// <summary>
/// 盘点单状态枚举
/// </summary>
/// <remarks>
/// 用于标识盘点单的状态流转
/// </remarks>
public enum StockCheckStatus
{
    /// <summary>
    /// 草稿
    /// </summary>
    Draft = 1,

    /// <summary>
    /// 盘点中
    /// </summary>
    Counting = 2,

    /// <summary>
    /// 已完成
    /// </summary>
    Completed = 3
}

/// <summary>
/// 库存预警类型枚举
/// </summary>
/// <remarks>
/// 用于标识库存预警的类型
/// </remarks>
public enum StockAlertType
{
    /// <summary>
    /// 低库存预警
    /// </summary>
    Low = 1,

    /// <summary>
    /// 高库存预警
    /// </summary>
    High = 2
}

/// <summary>
/// 库存预警状态枚举
/// </summary>
/// <remarks>
/// 用于标识库存预警的处理状态
/// </remarks>
public enum StockAlertStatus
{
    /// <summary>
    /// 待处理
    /// </summary>
    Pending = 1,

    /// <summary>
    /// 已解决
    /// </summary>
    Resolved = 2
}