using EasyProduct.Common.Base;
using EasyProduct.Models.Dto.Crm;

namespace EasyProduct.Business.Crm;

/// <summary>
/// 发票服务接口
/// </summary>
public interface IInvoiceService
{
    Task<PageResponse<InvoiceDto>> GetListAsync(InvoiceQueryDto query);
    Task<InvoiceDto> GetDetailAsync(Guid id);
    Task<Guid> CreateAsync(CreateInvoiceDto dto);
    Task<bool> UpdateAsync(Guid id, UpdateInvoiceDto dto);
    Task<bool> IssueAsync(Guid id);
    Task<bool> VoidAsync(Guid id);
    Task<bool> DeleteAsync(Guid id);
}