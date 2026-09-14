using EasyProduct.Business.Crm;
using EasyProduct.Common.Base;
using EasyProduct.Models.Dto.Crm;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyProduct.Web.Controllers.Admin.Crm;

/// <summary>
/// 销售订单管理控制器（管理端）
/// </summary>
/// <remarks>
/// 提供销售订单的创建、查询、更新、删除等管理功能。
/// 需要管理员权限。
/// </remarks>
[ApiController]
[Route("api/admin/crm/sales-order")]
[Authorize(AuthenticationSchemes = "AdminJwt")]
public class SalesOrderController : BaseController
{
    private readonly ISalesOrderService _salesOrderService;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="salesOrderService">销售订单服务</param>
    public SalesOrderController(ISalesOrderService salesOrderService)
    {
        _salesOrderService = salesOrderService;
    }

    /// <summary>
    /// 获取客户下拉选项列表
    /// </summary>
    /// <returns>客户下拉选项列表</returns>
    /// <remarks>
    /// 获取状态为启用的客户列表，用于订单创建时的客户选择。
    /// </remarks>
    [HttpGet("customer/options")]
    public async Task<ApiResponse<List<CustomerOptionDto>>> GetCustomerOptions()
    {
        var result = await _salesOrderService.GetCustomerOptionsAsync();
        return Success(result);
    }

    /// <summary>
    /// 分页查询销售订单列表
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>订单分页列表</returns>
    /// <remarks>
    /// 支持按订单编号、客户ID、状态筛选。
    /// 支持分页查询。
    /// 不包含订单明细。
    /// </remarks>
    [HttpGet("list")]
    public async Task<ApiResponse<PageResponse<SalesOrderDto>>> GetList([FromQuery] SalesOrderQueryDto query)
    {
        var result = await _salesOrderService.GetListAsync(query);
        return Success(result);
    }

    /// <summary>
    /// 获取销售订单详情
    /// </summary>
    /// <param name="id">订单ID</param>
    /// <returns>订单详情（包含明细）</returns>
    [HttpGet("{id}")]
    public async Task<ApiResponse<SalesOrderDetailDto>> GetDetail(Guid id)
    {
        var result = await _salesOrderService.GetDetailAsync(id);
        return Success(result);
    }

    /// <summary>
    /// 创建销售订单
    /// </summary>
    /// <param name="dto">创建订单参数</param>
    /// <returns>订单ID</returns>
    /// <remarks>
    /// 创建新的销售订单。
    /// 订单编号自动生成（SO-{year}-{sequence:04d}）。
    /// 自动填充客户名称（冗余字段）。
    /// 自动计算明细金额和订单总金额。
    /// 默认状态为草稿（draft）。
    /// </remarks>
    [HttpPost]
    public async Task<ApiResponse<string>> Create([FromBody] CreateSalesOrderDto dto)
    {
        var result = await _salesOrderService.CreateAsync(dto);
        return ApiResponse<string>.Success(result.ToString(), "销售订单创建成功");
    }

    /// <summary>
    /// 更新销售订单
    /// </summary>
    /// <param name="id">订单ID</param>
    /// <param name="dto">更新订单参数</param>
    /// <returns>是否成功</returns>
    /// <remarks>
    /// 仅草稿状态可修改。
    /// 更新时会重新计算明细金额和订单总金额。
    /// </remarks>
    [HttpPut("{id}")]
    public async Task<ApiResponse<bool>> Update(Guid id, [FromBody] UpdateSalesOrderDto dto)
    {
        var result = await _salesOrderService.UpdateAsync(id, dto);
        return Success(result);
    }

    /// <summary>
    /// 更新销售订单状态
    /// </summary>
    /// <param name="id">订单ID</param>
    /// <param name="dto">状态更新参数</param>
    /// <returns>是否成功</returns>
    /// <remarks>
    /// 状态流转规则：
    /// - draft → confirmed 或 cancelled
    /// - confirmed → shipped 或 cancelled
    /// - shipped → completed
    ///
    /// 发货（shipped）时会：
    /// 1. 检查库存是否充足
    /// 2. 自动创建出库记录
    /// 3. 扣减库存
    /// </remarks>
    [HttpPut("{id}/status")]
    public async Task<ApiResponse<bool>> UpdateStatus(Guid id, [FromBody] UpdateSalesOrderStatusDto dto)
    {
        var result = await _salesOrderService.UpdateStatusAsync(id, dto);
        return Success(result);
    }

    /// <summary>
    /// 删除销售订单
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
        var result = await _salesOrderService.DeleteAsync(id);
        return Success(result);
    }
}