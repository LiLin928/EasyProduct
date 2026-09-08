using System.ComponentModel.DataAnnotations;
using EasyProduct.Models.Enums;
using EasyProduct.Models.Enums.Product;

namespace EasyProduct.Models.Dto.Product.Spu;

/// <summary>
/// 商品主档查询 DTO
/// </summary>
/// <remarks>
/// 用于 SPU 列表查询，支持按商品名称、商品编码、分类、类型、状态筛选
/// 包含分页参数
/// </remarks>
public class SpuQueryDto
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
    /// 商品名称关键词
    /// </summary>
    /// <remarks>
    /// 支持模糊搜索，最大长度 200 个字符
    /// </remarks>
    [StringLength(200, ErrorMessage = "商品名称长度不能超过200个字符")]
    public string? SpuName { get; set; }

    /// <summary>
    /// 商品编码
    /// </summary>
    /// <remarks>
    /// 支持模糊搜索，最大长度 50 个字符
    /// </remarks>
    [StringLength(50, ErrorMessage = "商品编码长度不能超过50个字符")]
    public string? SpuCode { get; set; }

    /// <summary>
    /// 分类ID
    /// </summary>
    /// <remarks>
    /// 精确匹配，GUID 字符串格式
    /// </remarks>
    [StringLength(36, ErrorMessage = "分类ID长度不能超过36个字符")]
    public string? CategoryId { get; set; }

    /// <summary>
    /// 商品类型
    /// </summary>
    /// <remarks>
    /// 使用 SpuType 枚举：Physical（1）= 实物商品，Virtual（2）= 虚拟商品，Ticket（3）= 票品
    /// </remarks>
    public SpuType? SpuType { get; set; }

    /// <summary>
    /// 状态筛选
    /// </summary>
    /// <remarks>
    /// 使用 Status 枚举：Disabled（0）= 禁用，Enabled（1）= 启用
    /// </remarks>
    public Status? Status { get; set; }
}