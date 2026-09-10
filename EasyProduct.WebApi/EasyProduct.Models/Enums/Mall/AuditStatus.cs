namespace EasyProduct.Models.Enums.Mall;

/// <summary>
/// 审核状态枚举
/// </summary>
/// <remarks>
/// 用于退款审核、退货审核等场景，记录审核环节的状态。
///
/// 状态说明：
/// - Pending: 待审核，表示申请已提交，等待管理员审核
/// - Approved: 已通过，表示审核通过，可以进入下一步处理
/// - Rejected: 已拒绝，表示审核被拒绝，申请被驳回
/// </remarks>
public enum AuditStatus
{
    /// <summary>
    /// 待审核
    /// </summary>
    Pending = 0,

    /// <summary>
    /// 已通过
    /// </summary>
    Approved = 1,

    /// <summary>
    /// 已拒绝
    /// </summary>
    Rejected = 2
}