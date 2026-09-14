using System;
using System.ComponentModel.DataAnnotations;

namespace EasyProduct.Models.Dto.Crm;

/// <summary>
/// 币种 DTO
/// </summary>
/// <remarks>
/// 用于币种列表查询返回
/// </remarks>
public class CurrencyDto
{
    /// <summary>
    /// 币种ID
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// 币种代码
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// 币种名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 符号
    /// </summary>
    public string Symbol { get; set; } = string.Empty;

    /// <summary>
    /// 汇率
    /// </summary>
    public decimal ExchangeRate { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// 状态名称
    /// </summary>
    public string StatusName { get; set; } = string.Empty;

    /// <summary>
    /// 是否默认币种
    /// </summary>
    public int IsDefault { get; set; }

    /// <summary>
    /// 是否默认币种名称
    /// </summary>
    public string IsDefaultName { get; set; } = string.Empty;

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// 创建币种 DTO
/// </summary>
/// <remarks>
/// 用于创建币种
/// </remarks>
public class CreateCurrencyDto
{
    /// <summary>
    /// 币种代码
    /// </summary>
    [Required(ErrorMessage = "币种代码不能为空")]
    [StringLength(10, ErrorMessage = "币种代码长度不能超过10个字符")]
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// 币种名称
    /// </summary>
    [Required(ErrorMessage = "币种名称不能为空")]
    [StringLength(50, ErrorMessage = "币种名称长度不能超过50个字符")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 符号
    /// </summary>
    [Required(ErrorMessage = "符号不能为空")]
    [StringLength(10, ErrorMessage = "符号长度不能超过10个字符")]
    public string Symbol { get; set; } = string.Empty;

    /// <summary>
    /// 汇率
    /// </summary>
    [Required(ErrorMessage = "汇率不能为空")]
    [Range(0.000001, 999999.999999, ErrorMessage = "汇率必须在0.000001到999999.999999之间")]
    public decimal ExchangeRate { get; set; } = 1.0m;

    /// <summary>
    /// 是否默认币种
    /// </summary>
    public int IsDefault { get; set; } = 0;
}

/// <summary>
/// 更新币种 DTO
/// </summary>
/// <remarks>
/// 用于更新币种
/// </remarks>
public class UpdateCurrencyDto
{
    /// <summary>
    /// 币种代码
    /// </summary>
    [Required(ErrorMessage = "币种代码不能为空")]
    [StringLength(10, ErrorMessage = "币种代码长度不能超过10个字符")]
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// 币种名称
    /// </summary>
    [Required(ErrorMessage = "币种名称不能为空")]
    [StringLength(50, ErrorMessage = "币种名称长度不能超过50个字符")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 符号
    /// </summary>
    [Required(ErrorMessage = "符号不能为空")]
    [StringLength(10, ErrorMessage = "符号长度不能超过10个字符")]
    public string Symbol { get; set; } = string.Empty;

    /// <summary>
    /// 汇率
    /// </summary>
    [Required(ErrorMessage = "汇率不能为空")]
    [Range(0.000001, 999999.999999, ErrorMessage = "汇率必须在0.000001到999999.999999之间")]
    public decimal ExchangeRate { get; set; } = 1.0m;

    /// <summary>
    /// 是否默认币种
    /// </summary>
    public int IsDefault { get; set; } = 0;
}

/// <summary>
/// 币种查询 DTO
/// </summary>
/// <remarks>
/// 用于币种列表查询筛选
/// </remarks>
public class CurrencyQueryDto
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
    /// 币种代码（模糊搜索）
    /// </summary>
    public string? Code { get; set; }

    /// <summary>
    /// 币种名称（模糊搜索）
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    public int? Status { get; set; }
}

/// <summary>
/// 税率 DTO
/// </summary>
/// <remarks>
/// 用于税率列表查询返回
/// </remarks>
public class TaxRateDto
{
    /// <summary>
    /// 税率ID
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// 税率名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 税率
    /// </summary>
    public decimal Rate { get; set; }

    /// <summary>
    /// 税率百分比
    /// </summary>
    public string RatePercent { get; set; } = string.Empty;

    /// <summary>
    /// 描述
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// 状态名称
    /// </summary>
    public string StatusName { get; set; } = string.Empty;

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// 创建税率 DTO
/// </summary>
/// <remarks>
/// 用于创建税率
/// </remarks>
public class CreateTaxRateDto
{
    /// <summary>
    /// 税率名称
    /// </summary>
    [Required(ErrorMessage = "税率名称不能为空")]
    [StringLength(50, ErrorMessage = "税率名称长度不能超过50个字符")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 税率
    /// </summary>
    [Required(ErrorMessage = "税率不能为空")]
    [Range(0, 1, ErrorMessage = "税率必须在0到1之间")]
    public decimal Rate { get; set; } = 0.0m;

    /// <summary>
    /// 描述
    /// </summary>
    [StringLength(200, ErrorMessage = "描述长度不能超过200个字符")]
    public string? Description { get; set; }
}

/// <summary>
/// 更新税率 DTO
/// </summary>
/// <remarks>
/// 用于更新税率
/// </remarks>
public class UpdateTaxRateDto
{
    /// <summary>
    /// 税率名称
    /// </summary>
    [Required(ErrorMessage = "税率名称不能为空")]
    [StringLength(50, ErrorMessage = "税率名称长度不能超过50个字符")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 税率
    /// </summary>
    [Required(ErrorMessage = "税率不能为空")]
    [Range(0, 1, ErrorMessage = "税率必须在0到1之间")]
    public decimal Rate { get; set; } = 0.0m;

    /// <summary>
    /// 描述
    /// </summary>
    [StringLength(200, ErrorMessage = "描述长度不能超过200个字符")]
    public string? Description { get; set; }
}

/// <summary>
/// 税率查询 DTO
/// </summary>
/// <remarks>
/// 用于税率列表查询筛选
/// </remarks>
public class TaxRateQueryDto
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
    /// 税率名称（模糊搜索）
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    public int? Status { get; set; }
}