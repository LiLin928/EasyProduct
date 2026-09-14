using System;
using System.ComponentModel.DataAnnotations;

namespace EasyProduct.Models.Dto.Crm;

/// <summary>
/// 库存预警 DTO（列表返回）
/// </summary>
/// <remarks>
/// 用于预警列表查询返回
/// </remarks>
public class StockAlertDto
{
    /// <summary>
    /// 预警ID
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
    /// 可用数量
    /// </summary>
    public int Available { get; set; }

    /// <summary>
    /// 库存下限
    /// </summary>
    public int MinLimit { get; set; }

    /// <summary>
    /// 库存上限
    /// </summary>
    public int MaxLimit { get; set; }

    /// <summary>
    /// 预警类型
    /// </summary>
    /// <remarks>
    /// 字符串字面量：'low' 或 'high'
    /// </remarks>
    public string AlertType { get; set; } = string.Empty;

    /// <summary>
    /// 预警类型名称
    /// </summary>
    public string AlertTypeName { get; set; } = string.Empty;

    /// <summary>
    /// 预警状态
    /// </summary>
    /// <remarks>
    /// 字符串字面量：'pending' 或 'resolved'
    /// </remarks>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// 预警状态名称
    /// </summary>
    public string StatusName { get; set; } = string.Empty;

    /// <summary>
    /// 创建时间
    /// </summary>
    public string CreatedAt { get; set; } = string.Empty;

    /// <summary>
    /// 解决时间
    /// </summary>
    public string? ResolvedAt { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    public string? Remark { get; set; }
}

/// <summary>
/// 库存预警查询 DTO
/// </summary>
/// <remarks>
/// 用于预警列表查询筛选
/// </remarks>
public class StockAlertQueryDto
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
    /// 预警类型
    /// </summary>
    /// <remarks>
    /// 可选值：'low' 或 'high'
    /// </remarks>
    public string? AlertType { get; set; }

    /// <summary>
    /// 预警状态
    /// </summary>
    /// <remarks>
    /// 可选值：'pending' 或 'resolved'
    /// </remarks>
    public string? Status { get; set; }
}

/// <summary>
/// 解决预警 DTO
/// </summary>
/// <remarks>
/// 用于解决库存预警
/// </remarks>
public class ResolveAlertDto
{
    /// <summary>
    /// 备注
    /// </summary>
    /// <remarks>
    /// 记录解决预警的说明
    /// 例如：已补货、已促销消化
    /// </remarks>
    [StringLength(500, ErrorMessage = "备注长度不能超过500个字符")]
    public string? Remark { get; set; }
}