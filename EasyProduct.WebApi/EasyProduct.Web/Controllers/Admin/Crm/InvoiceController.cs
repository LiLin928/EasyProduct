using EasyProduct.Business.Crm;
using EasyProduct.Common.Base;
using EasyProduct.Models.Dto.Crm;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyProduct.Web.Controllers.Admin.Crm;

/// <summary>
/// 发票管理控制器（管理端）
/// </summary>
[ApiController]
[Route("api/admin/crm/invoice")]
[Authorize(AuthenticationSchemes = "AdminJwt")]
public class InvoiceController : BaseController
{
    private readonly IInvoiceService _invoiceService;

    public InvoiceController(IInvoiceService invoiceService)
    {
        _invoiceService = invoiceService;
    }

    /// <summary>
    /// 分页查询发票列表
    /// </summary>
    [HttpGet("list")]
    public async Task<ApiResponse<PageResponse<InvoiceDto>>> GetList([FromQuery] InvoiceQueryDto query)
    {
        var result = await _invoiceService.GetListAsync(query);
        return Success(result);
    }

    /// <summary>
    /// 获取发票详情
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ApiResponse<InvoiceDto>> GetDetail(Guid id)
    {
        var result = await _invoiceService.GetDetailAsync(id);
        return Success(result);
    }

    /// <summary>
    /// 创建发票
    /// </summary>
    [HttpPost]
    public async Task<ApiResponse<string>> Create([FromBody] CreateInvoiceDto dto)
    {
        var result = await _invoiceService.CreateAsync(dto);
        return ApiResponse<string>.Success(result.ToString(), "发票创建成功");
    }

    /// <summary>
    /// 更新发票
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ApiResponse<bool>> Update(Guid id, [FromBody] UpdateInvoiceDto dto)
    {
        var result = await _invoiceService.UpdateAsync(id, dto);
        return Success(result);
    }

    /// <summary>
    /// 开具发票
    /// </summary>
    [HttpPost("{id}/issue")]
    public async Task<ApiResponse<bool>> Issue(Guid id)
    {
        var result = await _invoiceService.IssueAsync(id);
        return Success(result, "发票已开具");
    }

    /// <summary>
    /// 作废发票
    /// </summary>
    [HttpPost("{id}/void")]
    public async Task<ApiResponse<bool>> Void(Guid id)
    {
        var result = await _invoiceService.VoidAsync(id);
        return Success(result, "发票已作废");
    }

    /// <summary>
    /// 删除发票
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ApiResponse<bool>> Delete(Guid id)
    {
        var result = await _invoiceService.DeleteAsync(id);
        return Success(result);
    }
}