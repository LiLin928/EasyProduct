using System;
using System.ComponentModel.DataAnnotations;

namespace EasyProduct.Models.Dto.Crm;

/// <summary>
/// 库存账面 DTO（列表返回）
/// </summary>
/// <remarks>
/// 用于库存列表查询返回
/// </remarks>
public class StockDto
{
    /// <summary>
    /// 库存ID
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
    /// 可用数量
    /// </summary>
    public int Available { get; set; }

    /// <summary>
    /// 锁定数量
    /// </summary>
    public int Locked { get; set; }

    /// <summary>
    /// 总数量
    /// </summary>
    public int Total { get; set; }

    /// <summary>
    /// 库存下限
    /// </summary>
    public int MinLimit { get; set; }

    /// <summary>
    /// 库存上限
    /// </summary>
    public int MaxLimit { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    public string? UpdatedAt { get; set; }
}

/// <summary>
/// 库存查询 DTO
/// </summary>
/// <remarks>
/// 用于库存列表查询筛选
/// </remarks>
public class StockQueryDto
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
    /// SKU名称（模糊搜索）
    /// </summary>
    public string? SkuName { get; set; }
}

/// <summary>
/// 库存调整 DTO
/// </summary>
/// <remarks>
/// 用于库存调整（盘点调整、其他调整）
/// </remarks>
public class StockAdjustDto
{
    /// <summary>
    /// 仓库ID
    /// </summary>
    [Required(ErrorMessage = "仓库ID不能为空")]
    public string WarehouseId { get; set; } = string.Empty;

    /// <summary>
    /// SKU编码
    /// </summary>
    [Required(ErrorMessage = "SKU编码不能为空")]
    [StringLength(50, ErrorMessage = "SKU编码长度不能超过50个字符")]
    public string SkuCode { get; set; } = string.Empty;

    /// <summary>
    /// 调整数量
    /// </summary>
    /// <remarks>
    /// 正数表示增加库存，负数表示减少库存
    /// 例如：10 表示增加 10 个，-5 表示减少 5 个
    /// </remarks>
    [Required(ErrorMessage = "调整数量不能为空")]
    public int Quantity { get; set; }

    /// <summary>
    /// 调整原因
    /// </summary>
    /// <remarks>
    /// 例如：盘点调整、报损、报溢等
    /// </remarks>
    [Required(ErrorMessage = "调整原因不能为空")]
    [StringLength(500, ErrorMessage = "调整原因长度不能超过500个字符")]
    public string Reason { get; set; } = string.Empty;
}