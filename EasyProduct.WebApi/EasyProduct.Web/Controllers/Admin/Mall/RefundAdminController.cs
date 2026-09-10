using EasyProduct.Common.Base;
using EasyProduct.Models.Dto.Mall.Refund;
using EasyProduct.Business.Mall;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyProduct.Web.Controllers.Admin.Mall;

/// <summary>
/// 退款管理控制器（管理端）
/// </summary>
/// <remarks>
/// 提供退款管理相关的 REST API 接口，包括退款查询、审核、执行等功能。
/// 路由前缀：/api/admin/mall/refund
/// 认证方式：AdminJwt（管理员身份）
/// </remarks>
[ApiController]
[Route("api/admin/mall/refund")]
[Authorize(AuthenticationSchemes = "AdminJwt")]
public class RefundAdminController : BaseController
{
    private readonly IRefundService _refundService;

    /// <summary>
    /// 构造函数
    /// </summary>
    public RefundAdminController(IRefundService refundService)
    {
        _refundService = refundService;
    }

    /// <summary>
    /// 分页查询退款列表
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>退款分页列表</returns>
    /// <remarks>
    /// 查询所有退款列表，支持按退款单号、订单编号、状态和时间范围筛选。
    /// </remarks>
    [HttpGet("list")]
    public async Task<ApiResponse<PageResponse<RefundListDto>>> GetList([FromQuery] RefundQuery query)
    {
        var result = await _refundService.GetRefundListAsync(query);
        return Success(result);
    }

    /// <summary>
    /// 获取退款详情
    /// </summary>
    /// <param name="id">退款ID</param>
    /// <returns>退款详情</returns>
    /// <remarks>
    /// 查询退款详细信息，包含订单项、商品信息、审核信息等。
    /// </remarks>
    [HttpGet("{id}")]
    public async Task<ApiResponse<RefundDetailDto>> GetDetail(string id)
    {
        var result = await _refundService.GetRefundDetailAsync(id);
        return Success(result);
    }

    /// <summary>
    /// 审核退款
    /// </summary>
    /// <param name="id">退款ID</param>
    /// <param name="dto">审核参数</param>
    /// <returns>是否成功</returns>
    /// <remarks>
    /// 管理员审核退款申请，可选择通过或拒绝。
    /// 审核通过后：
    /// - 仅退款类型：直接进入退款中状态
    /// - 退货退款类型：进入退货中状态，等待用户填写物流信息
    /// </remarks>
    [HttpPost("{id}/audit")]
    public async Task<ApiResponse<bool>> Audit(string id, [FromBody] RefundAuditDto dto)
    {
        var result = await _refundService.AuditRefundAsync(id, dto);
        return Success(result);
    }

    /// <summary>
    /// 执行退款
    /// </summary>
    /// <param name="id">退款ID</param>
    /// <returns>是否成功</returns>
    /// <remarks>
    /// 执行退款操作，调用支付平台退款接口。
    /// 退款成功后会更新订单状态和恢复库存。
    /// </remarks>
    [HttpPost("{id}/process")]
    public async Task<ApiResponse<bool>> Process(string id)
    {
        var result = await _refundService.ProcessRefundAsync(id);
        return Success(result);
    }
}