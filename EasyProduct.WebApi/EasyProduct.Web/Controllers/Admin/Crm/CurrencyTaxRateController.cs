using EasyProduct.Business.Crm;
using EasyProduct.Common.Base;
using EasyProduct.Models.Dto.Crm;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyProduct.Web.Controllers.Admin.Crm;

/// <summary>
/// 币种管理控制器（管理端）
/// </summary>
/// <remarks>
/// 提供币种的创建、查询、更新、删除等管理功能。
/// 需要管理员权限。
/// </remarks>
[ApiController]
[Route("api/admin/crm/currency")]
[Authorize(AuthenticationSchemes = "AdminJwt")]
public class CurrencyController : BaseController
{
    private readonly ICurrencyService _currencyService;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="currencyService">币种服务</param>
    public CurrencyController(ICurrencyService currencyService)
    {
        _currencyService = currencyService;
    }

    /// <summary>
    /// 创建币种
    /// </summary>
    /// <param name="dto">创建币种参数</param>
    /// <returns>币种ID</returns>
    /// <remarks>
    /// 创建新的币种。
    /// 如果设置为默认币种，会自动取消其他币种的默认标记。
    /// </remarks>
    [HttpPost]
    public async Task<ApiResponse<string>> Create([FromBody] CreateCurrencyDto dto)
    {
        var result = await _currencyService.CreateAsync(dto);
        return ApiResponse<string>.Success(result.ToString(), "币种创建成功");
    }

    /// <summary>
    /// 分页查询币种列表
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>币种分页列表</returns>
    /// <remarks>
    /// 支持按币种代码、币种名称、状态筛选。
    /// 支持分页查询。
    /// </remarks>
    [HttpGet("list")]
    public async Task<ApiResponse<PageResponse<CurrencyDto>>> GetList([FromQuery] CurrencyQueryDto query)
    {
        var result = await _currencyService.GetListAsync(query);
        return Success(result);
    }

    /// <summary>
    /// 获取币种详情
    /// </summary>
    /// <param name="id">币种ID</param>
    /// <returns>币种详情</returns>
    [HttpGet("{id}")]
    public async Task<ApiResponse<CurrencyDto>> GetDetail(Guid id)
    {
        var result = await _currencyService.GetByIdAsync(id);
        return Success(result);
    }

    /// <summary>
    /// 更新币种
    /// </summary>
    /// <param name="id">币种ID</param>
    /// <param name="dto">更新币种参数</param>
    /// <returns>是否成功</returns>
    /// <remarks>
    /// 如果设置为默认币种，会自动取消其他币种的默认标记。
    /// </remarks>
    [HttpPut("{id}")]
    public async Task<ApiResponse<bool>> Update(Guid id, [FromBody] UpdateCurrencyDto dto)
    {
        var result = await _currencyService.UpdateAsync(id, dto);
        return Success(result);
    }

    /// <summary>
    /// 删除币种
    /// </summary>
    /// <param name="id">币种ID</param>
    /// <returns>是否成功</returns>
    /// <remarks>
    /// 软删除币种。
    /// </remarks>
    [HttpDelete("{id}")]
    public async Task<ApiResponse<bool>> Delete(Guid id)
    {
        var result = await _currencyService.DeleteAsync(id);
        return Success(result);
    }

    /// <summary>
    /// 启用/禁用币种
    /// </summary>
    /// <param name="id">币种ID</param>
    /// <param name="status">状态（0=禁用，1=启用）</param>
    /// <returns>是否成功</returns>
    [HttpPatch("{id}/status")]
    public async Task<ApiResponse<bool>> UpdateStatus(Guid id, [FromQuery] int status)
    {
        var result = await _currencyService.UpdateStatusAsync(id, status);
        return Success(result);
    }

    /// <summary>
    /// 获取启用的币种列表（下拉选择用）
    /// </summary>
    /// <returns>币种列表</returns>
    /// <remarks>
    /// 获取启用的币种列表，用于下拉选择。
    /// </remarks>
    [HttpGet("active")]
    public async Task<ApiResponse<List<CurrencyDto>>> GetActiveList()
    {
        var result = await _currencyService.GetActiveListAsync();
        return Success(result);
    }
}

/// <summary>
/// 税率管理控制器（管理端）
/// </summary>
/// <remarks>
/// 提供税率的创建、查询、更新、删除等管理功能。
/// 需要管理员权限。
/// </remarks>
[ApiController]
[Route("api/admin/crm/tax-rate")]
[Authorize(AuthenticationSchemes = "AdminJwt")]
public class TaxRateController : BaseController
{
    private readonly ITaxRateService _taxRateService;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="taxRateService">税率服务</param>
    public TaxRateController(ITaxRateService taxRateService)
    {
        _taxRateService = taxRateService;
    }

    /// <summary>
    /// 创建税率
    /// </summary>
    /// <param name="dto">创建税率参数</param>
    /// <returns>税率ID</returns>
    /// <remarks>
    /// 创建新的税率。
    /// 税率值为 0-1 之间的小数（如 0.13 表示 13%）。
    /// </remarks>
    [HttpPost]
    public async Task<ApiResponse<string>> Create([FromBody] CreateTaxRateDto dto)
    {
        var result = await _taxRateService.CreateAsync(dto);
        return ApiResponse<string>.Success(result.ToString(), "税率创建成功");
    }

    /// <summary>
    /// 分页查询税率列表
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>税率分页列表</returns>
    /// <remarks>
    /// 支持按税率名称、状态筛选。
    /// 支持分页查询。
    /// </remarks>
    [HttpGet("list")]
    public async Task<ApiResponse<PageResponse<TaxRateDto>>> GetList([FromQuery] TaxRateQueryDto query)
    {
        var result = await _taxRateService.GetListAsync(query);
        return Success(result);
    }

    /// <summary>
    /// 获取税率详情
    /// </summary>
    /// <param name="id">税率ID</param>
    /// <returns>税率详情</returns>
    [HttpGet("{id}")]
    public async Task<ApiResponse<TaxRateDto>> GetDetail(Guid id)
    {
        var result = await _taxRateService.GetByIdAsync(id);
        return Success(result);
    }

    /// <summary>
    /// 更新税率
    /// </summary>
    /// <param name="id">税率ID</param>
    /// <param name="dto">更新税率参数</param>
    /// <returns>是否成功</returns>
    [HttpPut("{id}")]
    public async Task<ApiResponse<bool>> Update(Guid id, [FromBody] UpdateTaxRateDto dto)
    {
        var result = await _taxRateService.UpdateAsync(id, dto);
        return Success(result);
    }

    /// <summary>
    /// 删除税率
    /// </summary>
    /// <param name="id">税率ID</param>
    /// <returns>是否成功</returns>
    /// <remarks>
    /// 软删除税率。
    /// </remarks>
    [HttpDelete("{id}")]
    public async Task<ApiResponse<bool>> Delete(Guid id)
    {
        var result = await _taxRateService.DeleteAsync(id);
        return Success(result);
    }

    /// <summary>
    /// 启用/禁用税率
    /// </summary>
    /// <param name="id">税率ID</param>
    /// <param name="status">状态（0=禁用，1=启用）</param>
    /// <returns>是否成功</returns>
    [HttpPatch("{id}/status")]
    public async Task<ApiResponse<bool>> UpdateStatus(Guid id, [FromQuery] int status)
    {
        var result = await _taxRateService.UpdateStatusAsync(id, status);
        return Success(result);
    }

    /// <summary>
    /// 获取启用的税率列表（下拉选择用）
    /// </summary>
    /// <returns>税率列表</returns>
    /// <remarks>
    /// 获取启用的税率列表，用于下拉选择。
    /// </remarks>
    [HttpGet("active")]
    public async Task<ApiResponse<List<TaxRateDto>>> GetActiveList()
    {
        var result = await _taxRateService.GetActiveListAsync();
        return Success(result);
    }
}