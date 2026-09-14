using EasyProduct.Business.Crm;
using EasyProduct.Common.Base;
using EasyProduct.Models.Dto.Crm;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyProduct.Web.Controllers.Admin.Crm;

/// <summary>
/// 仓库管理控制器（管理端）
/// </summary>
/// <remarks>
/// 提供仓库的创建、查询、更新、删除等管理功能。
/// 需要管理员权限。
/// </remarks>
[ApiController]
[Route("api/admin/crm/warehouse")]
[Authorize(AuthenticationSchemes = "AdminJwt")]
public class WarehouseController : BaseController
{
    private readonly IWarehouseService _warehouseService;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="warehouseService">仓库服务</param>
    public WarehouseController(IWarehouseService warehouseService)
    {
        _warehouseService = warehouseService;
    }

    /// <summary>
    /// 获取仓库下拉选项列表
    /// </summary>
    /// <returns>仓库下拉选项列表</returns>
    /// <remarks>
    /// 获取状态为启用的仓库列表，用于其他模块选择仓库时的下拉选项。
    /// </remarks>
    [HttpGet("options")]
    public async Task<ApiResponse<List<WarehouseOptionDto>>> GetOptions()
    {
        var result = await _warehouseService.GetWarehouseOptionsAsync();
        return Success(result);
    }

    /// <summary>
    /// 分页查询仓库列表
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>仓库分页列表</returns>
    /// <remarks>
    /// 支持按仓库编码、名称、状态筛选。
    /// 支持分页查询。
    /// </remarks>
    [HttpGet("list")]
    public async Task<ApiResponse<PageResponse<WarehouseDto>>> GetList([FromQuery] WarehouseQueryDto query)
    {
        var result = await _warehouseService.GetListAsync(query);
        return Success(result);
    }

    /// <summary>
    /// 获取仓库详情
    /// </summary>
    /// <param name="id">仓库ID</param>
    /// <returns>仓库详情</returns>
    [HttpGet("{id}")]
    public async Task<ApiResponse<WarehouseDto>> GetDetail(Guid id)
    {
        var result = await _warehouseService.GetDetailAsync(id);
        return Success(result);
    }

    /// <summary>
    /// 创建仓库
    /// </summary>
    /// <param name="dto">创建仓库参数</param>
    /// <returns>仓库ID</returns>
    /// <remarks>
    /// 创建新的仓库。
    /// 编码必须唯一。
    /// 默认状态为启用（active）。
    /// </remarks>
    [HttpPost]
    public async Task<ApiResponse<string>> Create([FromBody] CreateWarehouseDto dto)
    {
        var result = await _warehouseService.CreateAsync(dto);
        return ApiResponse<string>.Success(result.ToString(), "仓库创建成功");
    }

    /// <summary>
    /// 更新仓库
    /// </summary>
    /// <param name="id">仓库ID</param>
    /// <param name="dto">更新仓库参数</param>
    /// <returns>是否成功</returns>
    [HttpPut("{id}")]
    public async Task<ApiResponse<bool>> Update(Guid id, [FromBody] UpdateWarehouseDto dto)
    {
        var result = await _warehouseService.UpdateAsync(id, dto);
        return Success(result);
    }

    /// <summary>
    /// 删除仓库
    /// </summary>
    /// <param name="id">仓库ID</param>
    /// <returns>是否成功</returns>
    /// <remarks>
    /// 使用软删除。
    /// 删除前会检查是否有关联数据。
    /// </remarks>
    [HttpDelete("{id}")]
    public async Task<ApiResponse<bool>> Delete(Guid id)
    {
        var result = await _warehouseService.DeleteAsync(id);
        return Success(result);
    }
}