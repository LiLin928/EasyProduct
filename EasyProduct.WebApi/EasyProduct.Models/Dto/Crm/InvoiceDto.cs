using System;
using System.ComponentModel.DataAnnotations;

namespace EasyProduct.Models.Dto.Crm;

/// <summary>
/// 发票 DTO（列表返回）
/// </summary>
public class InvoiceDto
{
    public string Id { get; set; } = string.Empty;
    public string InvoiceNo { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string TypeName { get; set; } = string.Empty;
    public string OrderType { get; set; } = string.Empty;
    public string OrderTypeName { get; set; } = string.Empty;
    public string OrderNo { get; set; } = string.Empty;
    public string PartyName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal TaxRate { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal Total { get; set; }
    public string IssueDate { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string StatusName { get; set; } = string.Empty;
    public string? Remark { get; set; }
    public string CreatedAt { get; set; } = string.Empty;
    public string? UpdatedAt { get; set; }
}

/// <summary>
/// 创建发票 DTO
/// </summary>
public class CreateInvoiceDto
{
    [Required(ErrorMessage = "发票类型不能为空")]
    public string Type { get; set; } = string.Empty;

    [Required(ErrorMessage = "订单类型不能为空")]
    public string OrderType { get; set; } = string.Empty;

    [Required(ErrorMessage = "订单编号不能为空")]
    public string OrderNo { get; set; } = string.Empty;

    [Required(ErrorMessage = "往来单位名称不能为空")]
    public string PartyName { get; set; } = string.Empty;

    [Required(ErrorMessage = "未税金额不能为空")]
    public decimal Amount { get; set; }

    [Required(ErrorMessage = "税率不能为空")]
    public decimal TaxRate { get; set; }

    public string? IssueDate { get; set; }
    public string? Remark { get; set; }
}

/// <summary>
/// 更新发票 DTO
/// </summary>
public class UpdateInvoiceDto
{
    public decimal? Amount { get; set; }
    public decimal? TaxRate { get; set; }
    public string? IssueDate { get; set; }
    public string? Remark { get; set; }
}

/// <summary>
/// 发票查询 DTO
/// </summary>
public class InvoiceQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? InvoiceNo { get; set; }
    public string? Type { get; set; }
    public string? OrderType { get; set; }
    public string? OrderNo { get; set; }
    public string? PartyName { get; set; }
    public string? Status { get; set; }
}