using System.ComponentModel.DataAnnotations;
using EasyProduct.Models.Enums;

namespace EasyProduct.Models.Dto.Product.Sku;

/// <summary>
/// 商品SKU查询 DTO
/// </summary>
/// <remarks>
/// 用于 SKU 列表查询，支持按 SKU 名称、SKU 编码、SPU ID、状态筛选
/// 包含分页参数
/// </remarks>
public class SkuQueryDto
{
    /// <summary>
    /// 页码，从 1 开始
    /// </summary>
    public int PageIndex { get; set; } = 1;

    /// <summary>
    /// 每页数量
    /// </summary>
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// SKU名称关键词
    /// </summary>
    /// <remarks>
    /// 支持模糊搜索，最大长度 200 个字符
    /// </remarks>
    [StringLength(200, ErrorMessage = "SKU名称长度不能超过200个字符")]
    public string? SkuName { get; set; }

    /// <summary>
    /// SKU编码
    /// </summary>
    /// <remarks>
    /// 支持模糊搜索，最大长度 50 个字符
    /// </remarks>
    [StringLength(50, ErrorMessage = "SKU编码长度不能超过50个字符")]
    public string? SkuCode { get; set; }

    /// <summary>
    /// 商品SPU ID
    /// </summary>
    /// <remarks>
    /// 精确匹配，GUID 字符串格式
    /// </remarks>
    [StringLength(36, ErrorMessage = "SPU ID长度不能超过36个字符")]
    public string? SpuId { get; set; }

    /// <summary>
    /// 状态筛选
    /// </summary>
    /// <remarks>
    /// 使用 Status 枚举：Disabled（0）= 禁用，Enabled（1）= 启用
    /// </remarks>
    public Status? Status { get; set; }
}