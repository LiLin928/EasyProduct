using System;
using System.ComponentModel.DataAnnotations;

namespace EasyProduct.Models.Dto.Crm;

/// <summary>
/// 出入库流水 DTO（列表返回）
/// </summary>
/// <remarks>
/// 用于流水列表查询返回
/// </remarks>
public class StockRecordDto
{
    /// <summary>
    /// 流水ID
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// 仓库ID
    /// </summary>
    public string WarehouseId { get; set; } = string.Empty;

    /// <summary>
    /// 仓库名称
    /// </summary>
    public string WarehouseName { get; set; } = string.Empty;

    /// <summary>
    /// SKU编码
    /// </summary>
    public string SkuCode { get; set; } = string.Empty;

    /// <summary>
    /// SKU名称
    /// </summary>
    public string SkuName { get; set; } = string.Empty;

    /// <summary>
    /// 规格
    /// </summary>
    public string Spec { get; set; } = string.Empty;

    /// <summary>
    /// 单位
    /// </summary>
    public string Unit { get; set; } = string.Empty;

    /// <summary>
    /// 出入库类型
    /// </summary>
    /// <remarks>
    /// 字符串字面量：'in' 或 'out'
    /// </remarks>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// 出入库类型名称
    /// </summary>
    public string TypeName { get; set; } = string.Empty;

    /// <summary>
    /// 流水来源类型
    /// </summary>
    /// <remarks>
    /// 字符串字面量：'purchase_in'、'sales_out'、'mall_out'、'check_adjust'、'reversal_return'
    /// </remarks>
    public string SourceType { get; set; } = string.Empty;

    /// <summary>
    /// 流水来源类型名称
    /// </summary>
    public string SourceTypeName { get; set; } = string.Empty;

    /// <summary>
    /// 来源单据编号
    /// </summary>
    public string SourceOrderNo { get; set; } = string.Empty;

    /// <summary>
    /// 数量
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// 操作人
    /// </summary>
    public string Operator { get; set; } = string.Empty;

    /// <summary>
    /// 备注
    /// </summary>
    public string? Remark { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public string CreatedAt { get; set; } = string.Empty;
}

/// <summary>
/// 出入库流水查询 DTO
/// </summary>
/// <remarks>
/// 用于流水列表查询筛选
/// </remarks>
public class StockRecordQueryDto
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
    /// 仓库ID
    /// </summary>
    public string? WarehouseId { get; set; }

    /// <summary>
    /// SKU编码（模糊搜索）
    /// </summary>
    public string? SkuCode { get; set; }

    /// <summary>
    /// 出入库类型
    /// </summary>
    /// <remarks>
    /// 可选值：'in' 或 'out'
    /// </remarks>
    public string? Type { get; set; }

    /// <summary>
    /// 流水来源类型
    /// </summary>
    /// <remarks>
    /// 可选值：'purchase_in'、'sales_out'、'mall_out'、'check_adjust'、'reversal_return'
    /// </remarks>
    public string? SourceType { get; set; }

    /// <summary>
    /// 来源单据编号（模糊搜索）
    /// </summary>
    public string? SourceOrderNo { get; set; }
}

/// <summary>
/// 创建出入库流水 DTO
/// </summary>
/// <remarks>
/// 用于内部创建流水记录（不对外暴露 API）
/// </remarks>
public class CreateStockRecordDto
{
    /// <summary>
    /// 仓库ID
    /// </summary>
    [Required(ErrorMessage = "仓库ID不能为空")]
    public string WarehouseId { get; set; } = string.Empty;

    /// <summary>
    /// 仓库名称
    /// </summary>
    [Required(ErrorMessage = "仓库名称不能为空")]
    public string WarehouseName { get; set; } = string.Empty;

    /// <summary>
    /// SKU编码
    /// </summary>
    [Required(ErrorMessage = "SKU编码不能为空")]
    public string SkuCode { get; set; } = string.Empty;

    /// <summary>
    /// SKU名称
    /// </summary>
    [Required(ErrorMessage = "SKU名称不能为空")]
    public string SkuName { get; set; } = string.Empty;

    /// <summary>
    /// 规格
    /// </summary>
    public string Spec { get; set; } = string.Empty;

    /// <summary>
    /// 单位
    /// </summary>
    public string Unit { get; set; } = string.Empty;

    /// <summary>
    /// 出入库类型
    /// </summary>
    [Required(ErrorMessage = "出入库类型不能为空")]
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// 流水来源类型
    /// </summary>
    [Required(ErrorMessage = "流水来源类型不能为空")]
    public string SourceType { get; set; } = string.Empty;

    /// <summary>
    /// 来源单据编号
    /// </summary>
    [Required(ErrorMessage = "来源单据编号不能为空")]
    public string SourceOrderNo { get; set; } = string.Empty;

    /// <summary>
    /// 数量
    /// </summary>
    [Required(ErrorMessage = "数量不能为空")]
    [Range(1, int.MaxValue, ErrorMessage = "数量必须大于0")]
    public int Quantity { get; set; }

    /// <summary>
    /// 操作人
    /// </summary>
    [Required(ErrorMessage = "操作人不能为空")]
    public string Operator { get; set; } = string.Empty;

    /// <summary>
    /// 备注
    /// </summary>
    public string? Remark { get; set; }
}