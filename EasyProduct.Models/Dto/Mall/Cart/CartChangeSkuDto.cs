using System.ComponentModel.DataAnnotations;

namespace EasyProduct.Models.Dto.Mall.Cart;

/// <summary>
/// 切换SKU规格DTO
/// </summary>
/// <remarks>
/// 用于在购物车中切换商品的规格（SKU）
/// </remarks>
public class CartChangeSkuDto
{
    /// <summary>
    /// 购物车ID
    /// </summary>
    [Required(ErrorMessage = "购物车ID不能为空")]
    [StringLength(36, ErrorMessage = "购物车ID长度不能超过36个字符")]
    public string CartId { get; set; } = string.Empty;

    /// <summary>
    /// 新SKU ID
    /// </summary>
    [Required(ErrorMessage = "新SKU ID不能为空")]
    [StringLength(36, ErrorMessage = "SKU ID长度不能超过36个字符")]
    public string NewSkuId { get; set; } = string.Empty;
}