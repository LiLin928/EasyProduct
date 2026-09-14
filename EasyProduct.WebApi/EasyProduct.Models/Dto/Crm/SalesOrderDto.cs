using System;
using System.ComponentModel.DataAnnotations;

namespace EasyProduct.Models.Dto.Crm;

/// <summary>
/// 销售订单 DTO（列表返回）
/// </summary>
/// <remarks>
/// 用于订单列表查询返回，不包含明细列表
/// </remarks>
public class SalesOrderDto
{
    /// <summary>
    /// 订单ID
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// 订单编号
    /// </summary>
    public string OrderNo { get; set; } = string.Empty;

    /// <summary>
    /// 客户ID
    /// </summary>
    public string CustomerId { get; set; } = string.Empty;

    /// <summary>
    /// 客户名称
    /// </summary>
    public string CustomerName { get; set; } = string.Empty;

    /// <summary>
    /// 业务员姓名
    /// </summary>
    public string? SalesPersonName { get; set; }

    /// <summary>
    /// 币种代码
    /// </summary>
    public string CurrencyCode { get; set; } = string.Empty;

    /// <summary>
    /// 币种符号
    /// </summary>
    public string CurrencySymbol { get; set; } = string.Empty;

    /// <summary>
    /// 付款条款
    /// </summary>
    public string PaymentTerms { get; set; } = string.Empty;

    /// <summary>
    /// 交货日期（YYYY-MM-DD）
    /// </summary>
    public string DeliveryDate { get; set; } = string.Empty;

    /// <summary>
    /// 订单状态
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// 订单状态名称
    /// </summary>
    public string StatusName { get; set; } = string.Empty;

    /// <summary>
    /// 小计金额
    /// </summary>
    public decimal SubtotalAmount { get; set; }

    /// <summary>
    /// 税额
    /// </summary>
    public decimal TaxAmount { get; set; }

    /// <summary>
    /// 总金额
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    public string? Remark { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public string CreatedAt { get; set; } = string.Empty;

    /// <summary>
    /// 更新时间
    /// </summary>
    public string? UpdatedAt { get; set; }
}

/// <summary>
/// 销售订单详情 DTO
/// </summary>
/// <remarks>
/// 用于订单详情查询返回，包含明细列表
/// </remarks>
public class SalesOrderDetailDto : SalesOrderDto
{
    /// <summary>
    /// 订单明细列表
    /// </summary>
    public List<SalesOrderItemDto> Items { get; set; } = new();
}

/// <summary>
/// 销售订单明细 DTO
/// </summary>
/// <remarks>
/// 用于订单明细的返回和传递
/// </remarks>
public class SalesOrderItemDto
{
    /// <summary>
    /// 明细ID
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// 订单ID
    /// </summary>
    public string OrderId { get; set; } = string.Empty;

    /// <summary>
    /// SKU编码
    /// </summary>
    public string SkuCode { get; set; } = string.Empty;

    /// <summary>
    /// SKU名称
    /// </summary>
    public string SkuName { get; set; } = string.Empty;

    /// <summary>
    /// 产品名称
    /// </summary>
    public string ProductName { get; set; } = string.Empty;

    /// <summary>
    /// 规格
    /// </summary>
    public string? Spec { get; set; }

    /// <summary>
    /// 单价
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// 数量
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// 税率代码
    /// </summary>
    public string TaxRateCode { get; set; } = string.Empty;

    /// <summary>
    /// 税率（百分比）
    /// </summary>
    public decimal TaxRate { get; set; }

    /// <summary>
    /// 金额
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// 税额
    /// </summary>
    public decimal TaxAmount { get; set; }

    /// <summary>
    /// 总金额
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// 仓库ID
    /// </summary>
    public string WarehouseId { get; set; } = string.Empty;
}

/// <summary>
/// 创建销售订单 DTO
/// </summary>
/// <remarks>
/// 用于创建销售订单
/// </remarks>
public class CreateSalesOrderDto
{
    /// <summary>
    /// 客户ID
    /// </summary>
    [Required(ErrorMessage = "客户ID不能为空")]
    public string CustomerId { get; set; } = string.Empty;

    /// <summary>
    /// 业务员姓名
    /// </summary>
    [StringLength(50, ErrorMessage = "业务员姓名长度不能超过50个字符")]
    public string? SalesPersonName { get; set; }

    /// <summary>
    /// 币种代码
    /// </summary>
    [StringLength(10, ErrorMessage = "币种代码长度不能超过10个字符")]
    public string? CurrencyCode { get; set; }

    /// <summary>
    /// 币种符号
    /// </summary>
    [StringLength(10, ErrorMessage = "币种符号长度不能超过10个字符")]
    public string? CurrencySymbol { get; set; }

    /// <summary>
    /// 付款条款
    /// </summary>
    [StringLength(50, ErrorMessage = "付款条款长度不能超过50个字符")]
    public string? PaymentTerms { get; set; }

    /// <summary>
    /// 交货日期（YYYY-MM-DD）
    /// </summary>
    public string? DeliveryDate { get; set; }

    /// <summary>
    /// 订单明细列表
    /// </summary>
    [Required(ErrorMessage = "订单明细不能为空")]
    public List<CreateSalesOrderItemDto> Items { get; set; } = new();

    /// <summary>
    /// 备注
    /// </summary>
    [StringLength(500, ErrorMessage = "备注长度不能超过500个字符")]
    public string? Remark { get; set; }
}

/// <summary>
/// 创建销售订单明细 DTO
/// </summary>
/// <remarks>
/// 用于创建订单明细
/// </remarks>
public class CreateSalesOrderItemDto
{
    /// <summary>
    /// SKU编码
    /// </summary>
    [Required(ErrorMessage = "SKU编码不能为空")]
    [StringLength(50, ErrorMessage = "SKU编码长度不能超过50个字符")]
    public string SkuCode { get; set; } = string.Empty;

    /// <summary>
    /// SKU名称
    /// </summary>
    [StringLength(200, ErrorMessage = "SKU名称长度不能超过200个字符")]
    public string? SkuName { get; set; }

    /// <summary>
    /// 产品名称
    /// </summary>
    [Required(ErrorMessage = "产品名称不能为空")]
    [StringLength(200, ErrorMessage = "产品名称长度不能超过200个字符")]
    public string ProductName { get; set; } = string.Empty;

    /// <summary>
    /// 规格
    /// </summary>
    [StringLength(200, ErrorMessage = "规格长度不能超过200个字符")]
    public string? Spec { get; set; }

    /// <summary>
    /// 单价
    /// </summary>
    [Required(ErrorMessage = "单价不能为空")]
    [Range(0, 999999999.99, ErrorMessage = "单价必须在0到999999999.99之间")]
    public decimal Price { get; set; }

    /// <summary>
    /// 数量
    /// </summary>
    [Required(ErrorMessage = "数量不能为空")]
    [Range(1, int.MaxValue, ErrorMessage = "数量必须大于0")]
    public int Quantity { get; set; }

    /// <summary>
    /// 税率代码
    /// </summary>
    [Required(ErrorMessage = "税率代码不能为空")]
    [StringLength(10, ErrorMessage = "税率代码长度不能超过10个字符")]
    public string TaxRateCode { get; set; } = string.Empty;

    /// <summary>
    /// 税率（百分比）
    /// </summary>
    [Required(ErrorMessage = "税率不能为空")]
    [Range(0, 100, ErrorMessage = "税率必须在0到100之间")]
    public decimal TaxRate { get; set; }

    /// <summary>
    /// 仓库ID
    /// </summary>
    public string? WarehouseId { get; set; }
}

/// <summary>
/// 更新销售订单 DTO
/// </summary>
/// <remarks>
/// 用于更新销售订单（仅草稿状态可修改）
/// </remarks>
public class UpdateSalesOrderDto
{
    /// <summary>
    /// 业务员姓名
    /// </summary>
    [StringLength(50, ErrorMessage = "业务员姓名长度不能超过50个字符")]
    public string? SalesPersonName { get; set; }

    /// <summary>
    /// 付款条款
    /// </summary>
    [StringLength(50, ErrorMessage = "付款条款长度不能超过50个字符")]
    public string? PaymentTerms { get; set; }

    /// <summary>
    /// 交货日期（YYYY-MM-DD）
    /// </summary>
    public string? DeliveryDate { get; set; }

    /// <summary>
    /// 订单明细列表
    /// </summary>
    public List<CreateSalesOrderItemDto>? Items { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    [StringLength(500, ErrorMessage = "备注长度不能超过500个字符")]
    public string? Remark { get; set; }
}

/// <summary>
/// 更新销售订单状态 DTO
/// </summary>
/// <remarks>
/// 用于更新订单状态
/// </remarks>
public class UpdateSalesOrderStatusDto
{
    /// <summary>
    /// 新状态
    /// </summary>
    /// <remarks>
    /// 可选值：draft、confirmed、shipped、completed、cancelled
    /// </remarks>
    [Required(ErrorMessage = "状态不能为空")]
    public string Status { get; set; } = string.Empty;
}

/// <summary>
/// 销售订单查询 DTO
/// </summary>
/// <remarks>
/// 用于订单列表查询筛选
/// </remarks>
public class SalesOrderQueryDto
{
    /// <summary>
    /// 页码（从1开始）
    /// </summary>
    public int PageIndex { get; set; } = 1;

    /// <summary>
    /// 每页条数
    /// </summary>
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// 订单编号（模糊搜索）
    /// </summary>
    public string? OrderNo { get; set; }

    /// <summary>
    /// 客户ID
    /// </summary>
    public string? CustomerId { get; set; }

    /// <summary>
    /// 订单状态
    /// </summary>
    public string? Status { get; set; }
}

/// <summary>
/// 客户下拉选项 DTO
/// </summary>
/// <remarks>
/// 用于订单创建时的客户下拉选择
/// </remarks>
public class CustomerOptionDto
{
    /// <summary>
    /// 客户ID
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// 客户名称
    /// </summary>
    public string Name { get; set; } = string.Empty;
}