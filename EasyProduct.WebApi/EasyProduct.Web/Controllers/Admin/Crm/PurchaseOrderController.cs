using EasyProduct.Business.Crm;
using EasyProduct.Common.Base;
using EasyProduct.Models.Dto.Crm;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyProduct.Web.Controllers.Admin.Crm;

/// <summary>
/// 采购订单管理控制器（管理端）
/// </summary>
/// <remarks>
/// 提供采购订单的创建、查询、更新、删除等管理功能。
/// 需要管理员权限。
/// </remarks>
[ApiController]
[Route("api/admin/crm/purchase-order")]
[Authorize(AuthenticationSchemes = "AdminJwt")]
public class PurchaseOrderController : BaseController
{
    private readonly IPurchaseOrderService _purchaseOrderService;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="purchaseOrderService">采购订单服务</param>
    public PurchaseOrderController(IPurchaseOrderService purchaseOrderService)
    {
        _purchaseOrderService = purchaseOrderService;
    }

    /// <summary>
    /// 获取供应商下拉选项列表
    /// </summary>
    /// <returns>供应商下拉选项列表</returns>
    /// <remarks>
    /// 获取状态为启用的供应商列表，用于订单创建时的供应商选择。
    /// </remarks>
    [HttpGet("supplier/options")]
    public async Task<ApiResponse<List<SupplierOptionDto>>> GetSupplierOptions()
    {
        var result = await _purchaseOrderService.GetSupplierOptionsAsync();
        return Success(result);
    }

    /// <summary>
    /// 分页查询采购订单列表
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>订单分页列表</returns>
    /// <remarks>
    /// 支持按订单编号、供应商ID、状态筛选。
    /// 支持分页查询。
    /// 不包含订单明细。
    /// </remarks>
    [HttpGet("list")]
    public async Task<ApiResponse<PageResponse<PurchaseOrderDto>>> GetList([FromQuery] PurchaseOrderQueryDto query)
    {
        var result = await _purchaseOrderService.GetListAsync(query);
        return Success(result);
    }

    /// <summary>
    /// 获取采购订单详情
    /// </summary>
    /// <param name="id">订单ID</param>
    /// <returns>订单详情（包含明细）</returns>
    [HttpGet("{id}")]
    public async Task<ApiResponse<PurchaseOrderDetailDto>> GetDetail(Guid id)
    {
        var result = await _purchaseOrderService.GetDetailAsync(id);
        return Success(result);
    }

    /// <summary>
    /// 创建采购订单
    /// </summary>
    /// <param name="dto">创建订单参数</param>
    /// <returns>订单ID</returns>
    /// <remarks>
    /// 创建新的采购订单。
    /// 订单编号自动生成（PO-{year}-{sequence:04d}）。
    /// 自动填充供应商名称（冗余字段）。
    /// 自动计算明细金额和订单总金额。
    /// 默认状态为草稿（draft）。
    /// </remarks>
    [HttpPost]
    public async Task<ApiResponse<string>> Create([FromBody] CreatePurchaseOrderDto dto)
    {
        var result = await _purchaseOrderService.CreateAsync(dto);
        return ApiResponse<string>.Success(result.ToString(), "采购订单创建成功");
    }

    /// <summary>
    /// 更新采购订单
    /// </summary>
    /// <param name="id">订单ID</param>
    /// <param name="dto">更新订单参数</param>
    /// <returns>是否成功</returns>
    /// <remarks>
    /// 仅草稿状态可修改。
    /// 更新时会重新计算明细金额和订单总金额。
    /// </remarks>
    [HttpPut("{id}")]
    public async Task<ApiResponse<bool>> Update(Guid id, [FromBody] UpdatePurchaseOrderDto dto)
    {
        var result = await _purchaseOrderService.UpdateAsync(id, dto);
        return Success(result);
    }

    /// <summary>
    /// 更新采购订单状态
    /// </summary>
    /// <param name="id">订单ID</param>
    /// <param name="dto">状态更新参数</param>
    /// <returns>是否成功</returns>
    /// <remarks>
    /// 状态流转规则：
    /// - draft → confirmed 或 cancelled
    /// - confirmed → received 或 cancelled
    /// - received → completed
    ///
    /// 入库（received）时会：
    /// 1. 自动创建入库记录
    /// 2. 增加库存
    /// </remarks>
    [HttpPut("{id}/status")]
    public async Task<ApiResponse<bool>> UpdateStatus(Guid id, [FromBody] UpdatePurchaseOrderStatusDto dto)
    {
        var result = await _purchaseOrderService.UpdateStatusAsync(id, dto);
        return Success(result);
    }

    /// <summary>
    /// 删除采购订单
    /// </summary>
    /// <param name="id">订单ID</param>
    /// <returns>是否成功</returns>
    /// <remarks>
    /// 仅草稿状态可删除。
    /// 使用软删除。
    /// </remarks>
    [HttpDelete("{id}")]
    public async Task<ApiResponse<bool>> Delete(Guid id)
    {
        var result = await _purchaseOrderService.DeleteAsync(id);
        return Success(result);
    }
}