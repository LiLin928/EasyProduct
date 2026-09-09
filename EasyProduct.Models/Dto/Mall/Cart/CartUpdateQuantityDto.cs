using System.ComponentModel.DataAnnotations;

namespace EasyProduct.Models.Dto.Mall.Cart;

/// <summary>
/// 修改购物车数量DTO
/// </summary>
/// <remarks>
/// 用于修改购物车中商品的数量
/// </remarks>
public class CartUpdateQuantityDto
{
    /// <summary>
    /// 购物车ID
    /// </summary>
    [Required(ErrorMessage = "购物车ID不能为空")]
    [StringLength(36, ErrorMessage = "购物车ID长度不能超过36个字符")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// 新数量
    /// </summary>
    [Required(ErrorMessage = "数量不能为空")]
    [Range(1, int.MaxValue, ErrorMessage = "数量必须大于0")]
    public int Quantity { get; set; }
}