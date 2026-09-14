using EasyProduct.Business.Crm;
using EasyProduct.Common.Base;
using EasyProduct.Models.Dto.Crm;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyProduct.Web.Controllers.Admin.Crm;

/// <summary>
/// 出入库流水管理控制器（管理端）
/// </summary>
/// <remarks>
/// 提供出入库流水的查询功能。
/// 需要管理员权限。
/// 注意：流水记录不支持创建、修改、删除（仅查询）。
/// </remarks>
[ApiController]
[Route("api/admin/crm/stock-record")]
[Authorize(AuthenticationSchemes = "AdminJwt")]
public class StockRecordController : BaseController
{
    private readonly IStockRecordService _stockRecordService;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="stockRecordService">出入库流水服务</param>
    public StockRecordController(IStockRecordService stockRecordService)
    {
        _stockRecordService = stockRecordService;
    }

    /// <summary>
    /// 分页查询出入库流水列表
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>流水分页列表</returns>
    /// <remarks>
    /// 支持按仓库、SKU编码、出入库类型、来源类型、来源单据编号筛选。
    /// 支持分页查询。
    /// </remarks>
    [HttpGet("list")]
    public async Task<ApiResponse<PageResponse<StockRecordDto>>> GetList([FromQuery] StockRecordQueryDto query)
    {
        var result = await _stockRecordService.GetListAsync(query);
        return Success(result);
    }
}