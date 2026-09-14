namespace EasyProduct.Models.Enums.Crm;

/// <summary>
/// 采购订单状态枚举
/// </summary>
/// <remarks>
/// 订单状态流转规则：
/// - draft（草稿）→ confirmed（已确认）或 cancelled（已取消）
/// - confirmed（已确认）→ received（已入库）或 cancelled（已取消）
/// - received（已入库）→ completed（已完成）
/// - completed（已完成）→ 终态
/// - cancelled（已取消）→ 终态
///
/// 注意：枚举值使用字符串字面量，与 mockjs 保持一致
/// </remarks>
public enum PurchaseOrderStatus
{
    /// <summary>
    /// 草稿
    /// </summary>
    /// <remarks>
    /// - 可修改
    /// - 可删除
    /// - 可转换为已确认或已取消
    /// </remarks>
    Draft = 1,

    /// <summary>
    /// 已确认
    /// </summary>
    /// <remarks>
    /// - 不可修改
    /// - 不可删除
    /// - 可转换为已入库或已取消
    /// </remarks>
    Confirmed = 2,

    /// <summary>
    /// 已入库
    /// </summary>
    /// <remarks>
    /// - 入库时自动创建入库记录并增加库存
    /// - 可转换为已完成
    /// </remarks>
    Received = 3,

    /// <summary>
    /// 已完成
    /// </summary>
    /// <remarks>
    /// - 终态，不可变更
    /// </remarks>
    Completed = 4,

    /// <summary>
    /// 已取消
    /// </summary>
    /// <remarks>
    /// - 终态，不可变更
    /// </remarks>
    Cancelled = 5
}