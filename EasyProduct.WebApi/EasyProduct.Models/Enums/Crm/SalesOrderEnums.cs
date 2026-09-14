namespace EasyProduct.Models.Enums.Crm;

/// <summary>
/// 销售订单状态枚举
/// </summary>
/// <remarks>
/// 订单状态流转规则：
/// - draft（草稿）→ confirmed（已确认）或 cancelled（已取消）
/// - confirmed（已确认）→ shipped（已发货）或 cancelled（已取消）
/// - shipped（已发货）→ completed（已完成）
/// - completed（已完成）→ 终态，不可变更
/// - cancelled（已取消）→ 终态，不可变更
///
/// 注意：枚举值使用字符串字面量，与 mockjs 保持一致
/// </remarks>
public enum SalesOrderStatus
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
    /// - 可转换为已发货或已取消
    /// </remarks>
    Confirmed = 2,

    /// <summary>
    /// 已发货
    /// </summary>
    /// <remarks>
    /// - 发货前会检查库存
    /// - 发货时自动创建出库记录并扣减库存
    /// - 可转换为已完成
    /// </remarks>
    Shipped = 3,

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