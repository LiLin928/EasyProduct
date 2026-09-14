using EasyProduct.Business.Crm;
using EasyProduct.Common.Base;
using EasyProduct.Models.Dto.Crm;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyProduct.Web.Controllers.Admin.Crm;

/// <summary>
/// 盘点管理控制器（管理端）
/// </summary>
/// <remarks>
/// 提供盘点单的创建、查询、更新、删除、完成盘点等管理功能。
/// 需要管理员权限。
/// </remarks>
[ApiController]
[Route("api/admin/crm/stock-check")]
[Authorize(AuthenticationSchemes = "AdminJwt")]
public class StockCheckController : BaseController
{
    private readonly IStockCheckService _stockCheckService;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="stockCheckService">盘点服务</param>
    public StockCheckController(IStockCheckService stockCheckService)
    {
        _stockCheckService = stockCheckService;
    }

    /// <summary>
    /// 分页查询盘点单列表
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>盘点单分页列表</returns>
    /// <remarks>
    /// 支持按盘点单编号、仓库、状态筛选。
    /// 支持分页查询。
    /// 不包含盘点明细。
    /// </remarks>
    [HttpGet("list")]
    public async Task<ApiResponse<PageResponse<StockCheckDto>>> GetList([FromQuery] StockCheckQueryDto query)
    {
        var result = await _stockCheckService.GetListAsync(query);
        return Success(result);
    }

    /// <summary>
    /// 获取盘点单详情
    /// </summary>
    /// <param name="id">盘点单ID</param>
    /// <returns>盘点单详情（包含明细）</returns>
    [HttpGet("{id}")]
    public async Task<ApiResponse<StockCheckDetailDto>> GetDetail(Guid id)
    {
        var result = await _stockCheckService.GetDetailAsync(id);
        return Success(result);
    }

    /// <summary>
    /// 创建盘点单
    /// </summary>
    /// <param name="dto">创建盘点单参数</param>
    /// <returns>盘点单ID</returns>
    /// <remarks>
    /// 创建新的盘点单。
    /// 盘点单编号自动生成（SC-{year}-{sequence:04d}）。
    /// 自动填充仓库名称（冗余字段）。
    /// 如果不提供明细，则从库存账面自动生成。
    /// 默认状态为草稿（draft）。
    /// </remarks>
    [HttpPost]
    public async Task<ApiResponse<string>> Create([FromBody] CreateStockCheckDto dto)
    {
        var result = await _stockCheckService.CreateAsync(dto);
        return ApiResponse<string>.Success(result.ToString(), "盘点单创建成功");
    }

    /// <summary>
    /// 更新盘点单
    /// </summary>
    /// <param name="id">盘点单ID</param>
    /// <param name="dto">更新盘点单参数</param>
    /// <returns>是否成功</returns>
    /// <remarks>
    /// 仅草稿和盘点中状态可修改。
    /// 更新明细时会重新计算盘点差异。
    /// </remarks>
    [HttpPut("{id}")]
    public async Task<ApiResponse<bool>> Update(Guid id, [FromBody] UpdateStockCheckDto dto)
    {
        var result = await _stockCheckService.UpdateAsync(id, dto);
        return Success(result);
    }

    /// <summary>
    /// 完成盘点
    /// </summary>
    /// <param name="id">盘点单ID</param>
    /// <returns>是否成功</returns>
    /// <remarks>
    /// 完成盘点时会：
    /// 1. 验证所有明细的实盘数量已填写
    /// 2. 根据盘点差异调整库存
    /// 3. 创建出入库流水记录
    /// 4. 更新盘点状态为已完成
    /// </remarks>
    [HttpPost("{id}/complete")]
    public async Task<ApiResponse<bool>> Complete(Guid id)
    {
        var result = await _stockCheckService.CompleteAsync(id);
        return Success(result, "盘点完成");
    }

    /// <summary>
    /// 删除盘点单
    /// </summary>
    /// <param name="id">盘点单ID</param>
    /// <returns>是否成功</returns>
    /// <remarks>
    /// 仅草稿状态可删除。
    /// 使用软删除。
    /// </remarks>
    [HttpDelete("{id}")]
    public async Task<ApiResponse<bool>> Delete(Guid id)
    {
        var result = await _stockCheckService.DeleteAsync(id);
        return Success(result);
    }
}