using System.ComponentModel.DataAnnotations;
using EasyProduct.Models.Dto.Common;

namespace EasyProduct.Models.Dto.Mall.Cart;

/// <summary>
/// 购物车查询参数
/// </summary>
/// <remarks>
/// 用于购物车列表查询（Admin端），支持按会员ID、关键词、选中状态筛选
/// 包含分页参数（继承自 PageQuery）
/// </remarks>
public class CartQuery : PageQuery
{
    /// <summary>
    /// 会员ID（Admin 端使用）
    /// </summary>
    /// <remarks>
    /// 精确匹配，GUID 格式
    /// </remarks>
    [StringLength(36, ErrorMessage = "会员ID长度不能超过36个字符")]
    public string? MemberId { get; set; }

    /// <summary>
    /// 关键词（商品名称/SKU编码）
    /// </summary>
    /// <remarks>
    /// 支持模糊搜索，最大长度 200 个字符
    /// </remarks>
    [StringLength(200, ErrorMessage = "关键词长度不能超过200个字符")]
    public string? Keyword { get; set; }

    /// <summary>
    /// 是否只查询选中商品
    /// </summary>
    /// <remarks>
    /// 0=未选中，1=已选中
    /// </remarks>
    public int? Selected { get; set; }
}