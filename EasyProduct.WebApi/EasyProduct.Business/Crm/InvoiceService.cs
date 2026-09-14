using EasyProduct.Common.Base;
using EasyProduct.Common.Error;
using EasyProduct.Models.Dto.Crm;
using EasyProduct.Models.Entitys.Crm;
using EasyProduct.Models.Enums.Crm;
using SqlSugar;

namespace EasyProduct.Business.Crm;

/// <summary>
/// 发票服务实现
/// </summary>
public class InvoiceService : BaseService, IInvoiceService
{
    public async Task<PageResponse<InvoiceDto>> GetListAsync(InvoiceQueryDto query)
    {
        var queryable = _db.Queryable<Invoice>().Where(i => i.IsDeleted == 0);

        if (!string.IsNullOrWhiteSpace(query.InvoiceNo))
            queryable = queryable.Where(i => i.InvoiceNo.ToLower().Contains(query.InvoiceNo.ToLower()));
        if (!string.IsNullOrWhiteSpace(query.Type) && Enum.TryParse<InvoiceType>(query.Type, true, out var type))
            queryable = queryable.Where(i => i.Type == type);
        if (!string.IsNullOrWhiteSpace(query.OrderType) && Enum.TryParse<InvoiceOrderType>(query.OrderType, true, out var orderType))
            queryable = queryable.Where(i => i.OrderType == orderType);
        if (!string.IsNullOrWhiteSpace(query.OrderNo))
            queryable = queryable.Where(i => i.OrderNo.ToLower().Contains(query.OrderNo.ToLower()));
        if (!string.IsNullOrWhiteSpace(query.PartyName))
            queryable = queryable.Where(i => i.PartyName.ToLower().Contains(query.PartyName.ToLower()));
        if (!string.IsNullOrWhiteSpace(query.Status) && Enum.TryParse<InvoiceStatus>(query.Status, true, out var status))
            queryable = queryable.Where(i => i.Status == status);

        queryable = queryable.OrderBy(i => i.CreatedAt, OrderByType.Desc);

        RefAsync<int> total = 0;
        var result = await queryable.Select(i => new InvoiceDto
        {
            Id = i.Id.ToString(),
            InvoiceNo = i.InvoiceNo,
            Type = i.Type.ToString().ToLower(),
            TypeName = i.Type == InvoiceType.Output ? "销项发票" : "进项发票",
            OrderType = i.OrderType.ToString().ToLower(),
            OrderTypeName = i.OrderType == InvoiceOrderType.Sales ? "销售订单" : "采购订单",
            OrderNo = i.OrderNo,
            PartyName = i.PartyName,
            Amount = i.Amount,
            TaxRate = i.TaxRate,
            TaxAmount = i.TaxAmount,
            Total = i.Total,
            IssueDate = i.IssueDate.ToString("yyyy-MM-dd"),
            Status = i.Status.ToString().ToLower(),
            StatusName = GetStatusName(i.Status),
            Remark = i.Remark,
            CreatedAt = i.CreatedAt.ToString("yyyy-MM-ddTHH:mm:ssZ"),
            UpdatedAt = i.UpdatedAt.HasValue ? i.UpdatedAt.Value.ToString("yyyy-MM-ddTHH:mm:ssZ") : null
        }).ToPageListAsync(query.PageIndex, query.PageSize, total);

        return PageResponse<InvoiceDto>.Create(result, total.Value, query.PageIndex, query.PageSize);
    }

    public async Task<InvoiceDto> GetDetailAsync(Guid id)
    {
        var invoice = await _db.Queryable<Invoice>().Where(i => i.Id == id && i.IsDeleted == 0).FirstAsync();
        if (invoice == null) throw new BusinessException("发票不存在或已被删除");

        return new InvoiceDto
        {
            Id = invoice.Id.ToString(),
            InvoiceNo = invoice.InvoiceNo,
            Type = invoice.Type.ToString().ToLower(),
            TypeName = invoice.Type == InvoiceType.Output ? "销项发票" : "进项发票",
            OrderType = invoice.OrderType.ToString().ToLower(),
            OrderTypeName = invoice.OrderType == InvoiceOrderType.Sales ? "销售订单" : "采购订单",
            OrderNo = invoice.OrderNo,
            PartyName = invoice.PartyName,
            Amount = invoice.Amount,
            TaxRate = invoice.TaxRate,
            TaxAmount = invoice.TaxAmount,
            Total = invoice.Total,
            IssueDate = invoice.IssueDate.ToString("yyyy-MM-dd"),
            Status = invoice.Status.ToString().ToLower(),
            StatusName = GetStatusName(invoice.Status),
            Remark = invoice.Remark,
            CreatedAt = invoice.CreatedAt.ToString("yyyy-MM-ddTHH:mm:ssZ"),
            UpdatedAt = invoice.UpdatedAt.HasValue ? invoice.UpdatedAt.Value.ToString("yyyy-MM-ddTHH:mm:ssZ") : null
        };
    }

    public async Task<Guid> CreateAsync(CreateInvoiceDto dto)
    {
        if (!Enum.TryParse<InvoiceType>(dto.Type, true, out var type))
            throw new BusinessException($"无效的发票类型：{dto.Type}");
        if (!Enum.TryParse<InvoiceOrderType>(dto.OrderType, true, out var orderType))
            throw new BusinessException($"无效的订单类型：{dto.OrderType}");

        var invoice = new Invoice
        {
            InvoiceNo = await GenerateInvoiceNoAsync(),
            Type = type,
            OrderType = orderType,
            OrderNo = dto.OrderNo,
            PartyName = dto.PartyName,
            Amount = dto.Amount,
            TaxRate = dto.TaxRate,
            TaxAmount = Math.Round(dto.Amount * dto.TaxRate / 100, 2),
            Total = Math.Round(dto.Amount * (1 + dto.TaxRate / 100), 2),
            IssueDate = !string.IsNullOrWhiteSpace(dto.IssueDate) ? DateTime.Parse(dto.IssueDate) : DateTime.Today,
            Status = InvoiceStatus.Draft,
            Remark = dto.Remark
        };

        await _db.Insertable(invoice).ExecuteCommandIdentityIntoEntityAsync();
        return invoice.Id;
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateInvoiceDto dto)
    {
        var invoice = await _db.Queryable<Invoice>().Where(i => i.Id == id && i.IsDeleted == 0).FirstAsync();
        if (invoice == null) throw new BusinessException("发票不存在或已被删除");
        if (invoice.Status != InvoiceStatus.Draft) throw new BusinessException("仅草稿状态可修改");

        if (dto.Amount.HasValue) invoice.Amount = dto.Amount.Value;
        if (dto.TaxRate.HasValue) invoice.TaxRate = dto.TaxRate.Value;
        if (dto.IssueDate != null) invoice.IssueDate = DateTime.Parse(dto.IssueDate);
        if (dto.Remark != null) invoice.Remark = dto.Remark;

        invoice.TaxAmount = Math.Round(invoice.Amount * invoice.TaxRate / 100, 2);
        invoice.Total = Math.Round(invoice.Amount * (1 + invoice.TaxRate / 100), 2);
        invoice.UpdatedAt = DateTime.UtcNow;

        return await _db.Updateable(invoice).ExecuteCommandAsync() > 0;
    }

    public async Task<bool> IssueAsync(Guid id)
    {
        var invoice = await _db.Queryable<Invoice>().Where(i => i.Id == id && i.IsDeleted == 0).FirstAsync();
        if (invoice == null) throw new BusinessException("发票不存在或已被删除");
        if (invoice.Status != InvoiceStatus.Draft) throw new BusinessException("仅草稿状态可开具");

        invoice.Status = InvoiceStatus.Issued;
        invoice.UpdatedAt = DateTime.UtcNow;
        return await _db.Updateable(invoice).ExecuteCommandAsync() > 0;
    }

    public async Task<bool> VoidAsync(Guid id)
    {
        var invoice = await _db.Queryable<Invoice>().Where(i => i.Id == id && i.IsDeleted == 0).FirstAsync();
        if (invoice == null) throw new BusinessException("发票不存在或已被删除");
        if (invoice.Status == InvoiceStatus.Voided) throw new BusinessException("发票已作废");

        invoice.Status = InvoiceStatus.Voided;
        invoice.UpdatedAt = DateTime.UtcNow;
        return await _db.Updateable(invoice).ExecuteCommandAsync() > 0;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var invoice = await _db.Queryable<Invoice>().Where(i => i.Id == id && i.IsDeleted == 0).FirstAsync();
        if (invoice == null) throw new BusinessException("发票不存在或已被删除");
        if (invoice.Status != InvoiceStatus.Draft) throw new BusinessException("仅草稿状态可删除");

        invoice.IsDeleted = 1;
        invoice.UpdatedAt = DateTime.UtcNow;
        return await _db.Updateable(invoice).ExecuteCommandAsync() > 0;
    }

    private async Task<string> GenerateInvoiceNoAsync()
    {
        var year = DateTime.Now.Year;
        var max = await _db.Queryable<Invoice>().Where(i => i.InvoiceNo.StartsWith($"INV-{year}-")).OrderBy(i => i.InvoiceNo, OrderByType.Desc).FirstAsync();
        int seq = 1;
        if (max != null && int.TryParse(max.InvoiceNo.Split('-')[2], out var last)) seq = last + 1;
        return $"INV-{year}-{seq:0000}";
    }

    private string GetStatusName(InvoiceStatus status) => status switch
    {
        InvoiceStatus.Draft => "草稿",
        InvoiceStatus.Issued => "已开具",
        InvoiceStatus.Voided => "已作废",
        _ => "未知"
    };
}