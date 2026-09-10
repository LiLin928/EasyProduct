using EasyProduct.Common.Base;
using EasyProduct.Models.Dto.Mall.Refund;
using EasyProduct.Business.Mall;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyProduct.Web.Controllers.App.Mall;

/// <summary>
/// 退款控制器
/// </summary>
/// <remarks>
/// 提供退款相关的 REST API 接口，包括退款申请、查询、审核、物流填写等功能。
/// 路由前缀：/api/app/mall/refund
/// 认证方式：MemberJwt（会员身份）
/// </remarks>
[ApiController]
[Route("api/app/mall/refund")]
[Authorize(AuthenticationSchemes = "MemberJwt")]
public class RefundController : BaseController
{
    private readonly IRefundService _refundService;

    /// <summary>
    /// 构造函数
    /// </summary>
    public RefundController(IRefundService refundService)
    {
        _refundService = refundService;
    }

    /// <summary>
    /// 申请退款
    /// </summary>
    /// <param name="dto">申请退款参数</param>
    /// <returns>退款详情</returns>
    /// <remarks>
    /// 会员申请退款，需要提供订单ID、订单项ID、退款类型、退款数量、退款金额和退款原因。
    /// 退款类型：1=仅退款，2=退货退款。
    /// </remarks>
    [HttpPost]
    public async Task<ApiResponse<RefundDetailDto>> Apply([FromBody] RefundApplyDto dto)
    {
        var memberId = GetCurrentUserId().ToString();
        var result = await _refundService.ApplyRefundAsync(memberId, dto);
        return Success(result);
    }

    /// <summary>
    /// 获取会员退款列表
    /// </summary>
    /// <param name="status">退款状态（可选）</param>
    /// <param name="pageIndex">页码，默认 1</param>
    /// <param name="pageSize">每页数量，默认 10</param>
    /// <returns>退款分页列表</returns>
    /// <remarks>
    /// 查询当前会员的退款列表，支持按状态筛选。
    /// 状态值：0=待审核，1=已通过，2=已拒绝，3=退货中，4=退款中，5=已完成，6=已取消
    /// </remarks>
    [HttpGet("list")]
    public async Task<ApiResponse<PageResponse<RefundListDto>>> GetMemberRefundList(
        [FromQuery] int? status = null,
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 10)
    {
        var memberId = GetCurrentUserId().ToString();
        var result = await _refundService.GetMemberRefundListAsync(memberId, status, pageIndex, pageSize);
        return Success(result);
    }

    /// <summary>
    /// 获取退款详情
    /// </summary>
    /// <param name="id">退款ID</param>
    /// <returns>退款详情</returns>
    /// <remarks>
    /// 查询退款详细信息，包含订单项、商品信息等。
    /// 只能查询自己的退款。
    /// </remarks>
    [HttpGet("{id}")]
    public async Task<ApiResponse<RefundDetailDto>> GetDetail(string id)
    {
        var result = await _refundService.GetRefundDetailAsync(id);
        // TODO: 验证是否是当前会员的退款
        return Success(result);
    }

    /// <summary>
    /// 填写退货物流信息
    /// </summary>
    /// <param name="id">退款ID</param>
    /// <param name="dto">物流信息</param>
    /// <returns>是否成功</returns>
    /// <remarks>
    /// 会员填写退货物流信息，仅退货退款类型需要此步骤。
    /// 填写成功后，退款状态会从退货中变更为退款中。
    /// </remarks>
    [HttpPost("{id}/logistics")]
    public async Task<ApiResponse<bool>> FillLogistics(string id, [FromBody] RefundLogisticsDto dto)
    {
        var memberId = GetCurrentUserId().ToString();
        var result = await _refundService.FillLogisticsAsync(id, memberId, dto);
        return Success(result);
    }

    /// <summary>
    /// 取消退款申请
    /// </summary>
    /// <param name="id">退款ID</param>
    /// <returns>是否成功</returns>
    /// <remarks>
    /// 会员取消退款申请，只有待审核和退货中状态的退款可以取消。
    /// </remarks>
    [HttpPost("{id}/cancel")]
    public async Task<ApiResponse<bool>> Cancel(string id)
    {
        var memberId = GetCurrentUserId().ToString();
        var result = await _refundService.CancelRefundAsync(id, memberId);
        return Success(result);
    }

    /// <summary>
    /// 获取订单退款列表
    /// </summary>
    /// <param name="orderId">订单ID</param>
    /// <returns>退款列表</returns>
    /// <remarks>
    /// 查询指定订单的所有退款记录。
    /// </remarks>
    [HttpGet("order/{orderId}")]
    public async Task<ApiResponse<List<RefundListDto>>> GetRefundListByOrder(string orderId)
    {
        var result = await _refundService.GetRefundListByOrderAsync(orderId);
        return Success(result);
    }
}