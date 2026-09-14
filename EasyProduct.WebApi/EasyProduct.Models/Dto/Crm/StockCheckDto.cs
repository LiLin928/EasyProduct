using System;
using System.ComponentModel.DataAnnotations;

namespace EasyProduct.Models.Dto.Crm;

/// <summary>
/// 盘点单 DTO（列表返回）
/// </summary>
/// <remarks>
/// 用于盘点单列表查询返回，不包含明细列表
/// </remarks>
public class StockCheckDto
{
    /// <summary>
    /// 盘点单ID
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// 盘点单编号
    /// </summary>
    public string CheckNo { get; set; } = string.Empty;

    /// <summary>
    /// 仓库ID
    /// </summary>
    public string WarehouseId { get; set; } = string.Empty;

    /// <summary>
    /// 仓库名称
    /// </summary>
    public string WarehouseName { get; set; } = string.Empty;

    /// <summary>
    /// 盘点人
    /// </summary>
    public string Checker { get; set; } = string.Empty;

    /// <summary>
    /// 盘点日期
    /// </summary>
    public string CheckDate { get; set; } = string.Empty;

    /// <summary>
    /// 盘点状态
    /// </summary>
    /// <remarks>
    /// 字符串字面量：'draft'、'counting'、'completed'
    /// </remarks>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// 盘点状态名称
    /// </summary>
    public string StatusName { get; set; } = string.Empty;

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
/// 盘点单详情 DTO
/// </summary>
/// <remarks>
/// 用于盘点单详情查询返回，包含明细列表
/// </remarks>
public class StockCheckDetailDto : StockCheckDto
{
    /// <summary>
    /// 盘点明细列表
    /// </summary>
    public List<StockCheckItemDto> Items { get; set; } = new();
}

/// <summary>
/// 盘点明细 DTO
/// </summary>
/// <remarks>
/// 用于盘点明细的返回和传递
/// </remarks>
public class StockCheckItemDto
{
    /// <summary>
    /// 明细ID
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// 盘点单ID
    /// </summary>
    public string CheckId { get; set; } = string.Empty;

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
    /// 系统数量
    /// </summary>
    public int SystemQty { get; set; }

    /// <summary>
    /// 实盘数量
    /// </summary>
    public int CountedQty { get; set; }

    /// <summary>
    /// 盘点差异
    /// </summary>
    public int Diff { get; set; }
}

/// <summary>
/// 创建盘点单 DTO
/// </summary>
/// <remarks>
/// 用于创建盘点单
/// </remarks>
public class CreateStockCheckDto
{
    /// <summary>
    /// 仓库ID
    /// </summary>
    [Required(ErrorMessage = "仓库ID不能为空")]
    public string WarehouseId { get; set; } = string.Empty;

    /// <summary>
    /// 盘点人
    /// </summary>
    [Required(ErrorMessage = "盘点人不能为空")]
    [StringLength(50, ErrorMessage = "盘点人长度不能超过50个字符")]
    public string Checker { get; set; } = string.Empty;

    /// <summary>
    /// 盘点日期（YYYY-MM-DD）
    /// </summary>
    public string? CheckDate { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    [StringLength(500, ErrorMessage = "备注长度不能超过500个字符")]
    public string? Remark { get; set; }

    /// <summary>
    /// 盘点明细列表
    /// </summary>
    /// <remarks>
    /// 可选，如果不提供则从库存账面自动生成
    /// </remarks>
    public List<CreateStockCheckItemDto>? Items { get; set; }
}

/// <summary>
/// 创建盘点明细 DTO
/// </summary>
/// <remarks>
/// 用于创建盘点明细
/// </remarks>
public class CreateStockCheckItemDto
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
    /// 规格
    /// </summary>
    [StringLength(200, ErrorMessage = "规格长度不能超过200个字符")]
    public string? Spec { get; set; }

    /// <summary>
    /// 单位
    /// </summary>
    [StringLength(20, ErrorMessage = "单位长度不能超过20个字符")]
    public string? Unit { get; set; }
}

/// <summary>
/// 更新盘点单 DTO
/// </summary>
/// <remarks>
/// 用于更新盘点单（仅草稿和盘点中状态可修改）
/// </remarks>
public class UpdateStockCheckDto
{
    /// <summary>
    /// 盘点人
    /// </summary>
    [StringLength(50, ErrorMessage = "盘点人长度不能超过50个字符")]
    public string? Checker { get; set; }

    /// <summary>
    /// 盘点日期（YYYY-MM-DD）
    /// </summary>
    public string? CheckDate { get; set; }

    /// <summary>
    /// 盘点明细列表
    /// </summary>
    public List<UpdateStockCheckItemDto>? Items { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    [StringLength(500, ErrorMessage = "备注长度不能超过500个字符")]
    public string? Remark { get; set; }
}

/// <summary>
/// 更新盘点明细 DTO
/// </summary>
/// <remarks>
/// 用于更新盘点明细（填写实盘数量）
/// </remarks>
public class UpdateStockCheckItemDto
{
    /// <summary>
    /// 明细ID
    /// </summary>
    public string? Id { get; set; }

    /// <summary>
    /// 实盘数量
    /// </summary>
    [Range(0, int.MaxValue, ErrorMessage = "实盘数量必须大于或等于0")]
    public int CountedQty { get; set; }
}

/// <summary>
/// 盘点单查询 DTO
/// </summary>
/// <remarks>
/// 用于盘点单列表查询筛选
/// </remarks>
public class StockCheckQueryDto
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
    /// 盘点单编号（模糊搜索）
    /// </summary>
    public string? CheckNo { get; set; }

    /// <summary>
    /// 仓库ID
    /// </summary>
    public string? WarehouseId { get; set; }

    /// <summary>
    /// 盘点状态
    /// </summary>
    /// <remarks>
    /// 可选值：'draft'、'counting'、'completed'
    /// </remarks>
    public string? Status { get; set; }
}