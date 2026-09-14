using EasyProduct.Business.Crm;
using EasyProduct.Common.Base;
using EasyProduct.Models.Dto.Crm;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyProduct.Web.Controllers.Admin.Crm;

/// <summary>
/// 库存预警管理控制器（管理端）
/// </summary>
/// <remarks>
/// 提供库存预警的查询、解决等管理功能。
/// 需要管理员权限。
/// </remarks>
[ApiController]
[Route("api/admin/crm/stock-alert")]
[Authorize(AuthenticationSchemes = "AdminJwt")]
public class StockAlertController : BaseController
{
    private readonly IStockAlertService _stockAlertService;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="stockAlertService">库存预警服务</param>
    public StockAlertController(IStockAlertService stockAlertService)
    {
        _stockAlertService = stockAlertService;
    }

    /// <summary>
    /// 分页查询库存预警列表
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>预警分页列表</returns>
    /// <remarks>
    /// 支持按仓库、SKU编码、预警类型、预警状态筛选。
    /// 支持分页查询。
    /// </remarks>
    [HttpGet("list")]
    public async Task<ApiResponse<PageResponse<StockAlertDto>>> GetList([FromQuery] StockAlertQueryDto query)
    {
        var result = await _stockAlertService.GetListAsync(query);
        return Success(result);
    }

    /// <summary>
    /// 解决库存预警
    /// </summary>
    /// <param name="id">预警ID</param>
    /// <param name="dto">解决预警参数</param>
    /// <returns>是否成功</returns>
    /// <remarks>
    /// 解决预警时会：
    /// 1. 更新预警状态为已解决
    /// 2. 记录解决时间
    /// 3. 添加备注说明
    /// </remarks>
    [HttpPost("{id}/resolve")]
    public async Task<ApiResponse<bool>> Resolve(Guid id, [FromBody] ResolveAlertDto dto)
    {
        var result = await _stockAlertService.ResolveAsync(id, dto);
        return Success(result, "预警已解决");
    }
}