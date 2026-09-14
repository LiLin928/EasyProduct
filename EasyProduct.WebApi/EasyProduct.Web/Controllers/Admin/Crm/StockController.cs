using EasyProduct.Business.Crm;
using EasyProduct.Common.Base;
using EasyProduct.Models.Dto.Crm;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyProduct.Web.Controllers.Admin.Crm;

/// <summary>
/// 库存管理控制器（管理端）
/// </summary>
/// <remarks>
/// 提供库存查询、调整等管理功能。
/// 需要管理员权限。
/// </remarks>
[ApiController]
[Route("api/admin/crm/stock")]
[Authorize(AuthenticationSchemes = "AdminJwt")]
public class StockController : BaseController
{
    private readonly IStockService _stockService;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="stockService">库存服务</param>
    public StockController(IStockService stockService)
    {
        _stockService = stockService;
    }

    /// <summary>
    /// 分页查询库存列表
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>库存分页列表</returns>
    /// <remarks>
    /// 支持按仓库、SKU编码、SKU名称筛选。
    /// 支持分页查询。
    /// </remarks>
    [HttpGet("list")]
    public async Task<ApiResponse<PageResponse<StockDto>>> GetList([FromQuery] StockQueryDto query)
    {
        var result = await _stockService.GetListAsync(query);
        return Success(result);
    }

    /// <summary>
    /// 获取库存详情
    /// </summary>
    /// <param name="id">库存ID</param>
    /// <returns>库存详情</returns>
    [HttpGet("{id}")]
    public async Task<ApiResponse<StockDto>> GetDetail(Guid id)
    {
        var result = await _stockService.GetDetailAsync(id);
        return Success(result);
    }

    /// <summary>
    /// 库存调整
    /// </summary>
    /// <param name="dto">库存调整参数</param>
    /// <returns>是否成功</returns>
    /// <remarks>
    /// 调整库存数量。
    /// 正数表示增加库存，负数表示减少库存。
    /// 会创建出入库流水记录。
    /// 会检查库存预警。
    /// </remarks>
    [HttpPost("adjust")]
    public async Task<ApiResponse<bool>> Adjust([FromBody] StockAdjustDto dto)
    {
        var result = await _stockService.AdjustAsync(dto);
        return Success(result);
    }
}