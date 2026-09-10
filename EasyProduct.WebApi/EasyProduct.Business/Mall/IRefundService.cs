using EasyProduct.Common.Base;
using EasyProduct.Models.Dto.Mall.Refund;

namespace EasyProduct.Business.Mall;

/// <summary>
/// 退款服务接口
/// </summary>
/// <remarks>
/// 提供退款申请、审核、查询、物流填写、退款执行等核心业务功能。
/// 支持两种退款类型：
/// - 仅退款（RefundOnly）：无需退货，直接退款
/// - 退货退款（ReturnAndRefund）：需要先退货，再退款
///
/// 退款流程：
/// 1. 会员申请退款
/// 2. 管理员审核
/// 3. 审核通过后，会员填写物流信息（退货退款类型）
/// 4. 管理员确认收货
/// 5. 执行退款
/// 6. 更新订单状态
/// </remarks>
public interface IRefundService
{
    #region 申请退款

    /// <summary>
    /// 申请退款
    /// </summary>
    /// <param name="memberId">会员ID</param>
    /// <param name="dto">申请退款参数</param>
    /// <returns>退款单信息</returns>
    Task<RefundDetailDto> ApplyRefundAsync(string memberId, RefundApplyDto dto);

    #endregion

    #region 查询退款

    /// <summary>
    /// 分页查询退款列表
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>退款分页列表</returns>
    Task<PageResponse<RefundListDto>> GetRefundListAsync(RefundQuery query);

    /// <summary>
    /// 获取退款详情
    /// </summary>
    /// <param name="refundId">退款ID</param>
    /// <returns>退款详情</returns>
    Task<RefundDetailDto> GetRefundDetailAsync(string refundId);

    /// <summary>
    /// 根据退款单号获取退款详情
    /// </summary>
    /// <param name="refundNo">退款单号</param>
    /// <returns>退款详情</returns>
    Task<RefundDetailDto> GetRefundDetailByNoAsync(string refundNo);

    /// <summary>
    /// 获取会员退款列表
    /// </summary>
    /// <param name="memberId">会员ID</param>
    /// <param name="status">退款状态（可选）</param>
    /// <param name="pageIndex">页码</param>
    /// <param name="pageSize">每页数量</param>
    /// <returns>退款分页列表</returns>
    Task<PageResponse<RefundListDto>> GetMemberRefundListAsync(string memberId, int? status = null, int pageIndex = 1, int pageSize = 10);

    /// <summary>
    /// 根据订单ID获取退款列表
    /// </summary>
    /// <param name="orderId">订单ID</param>
    /// <returns>退款列表</returns>
    Task<List<RefundListDto>> GetRefundListByOrderAsync(string orderId);

    #endregion

    #region 退款审核

    /// <summary>
    /// 审核退款
    /// </summary>
    /// <param name="refundId">退款ID</param>
    /// <param name="dto">审核参数</param>
    /// <returns>是否成功</returns>
    Task<bool> AuditRefundAsync(string refundId, RefundAuditDto dto);

    #endregion

    #region 物流信息

    /// <summary>
    /// 填写退货物流信息
    /// </summary>
    /// <param name="refundId">退款ID</param>
    /// <param name="memberId">会员ID</param>
    /// <param name="dto">物流信息</param>
    /// <returns>是否成功</returns>
    Task<bool> FillLogisticsAsync(string refundId, string memberId, RefundLogisticsDto dto);

    #endregion

    #region 退款执行

    /// <summary>
    /// 执行退款
    /// </summary>
    /// <param name="refundId">退款ID</param>
    /// <returns>是否成功</returns>
    Task<bool> ProcessRefundAsync(string refundId);

    /// <summary>
    /// 取消退款申请
    /// </summary>
    /// <param name="refundId">退款ID</param>
    /// <param name="memberId">会员ID</param>
    /// <returns>是否成功</returns>
    Task<bool> CancelRefundAsync(string refundId, string memberId);

    #endregion
}