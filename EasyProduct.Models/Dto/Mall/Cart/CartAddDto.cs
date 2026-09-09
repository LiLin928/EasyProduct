using System.ComponentModel.DataAnnotations;

namespace EasyProduct.Models.Dto.Mall.Cart;

/// <summary>
/// 添加到购物车DTO
/// </summary>
/// <remarks>
/// 用于添加商品到购物车，包含 SKU ID 和数量
/// </remarks>
public class CartAddDto
{
    /// <summary>
    /// SKU ID
    /// </summary>
    [Required(ErrorMessage = "SKU ID不能为空")]
    [StringLength(36, ErrorMessage = "SKU ID长度不能超过36个字符")]
    public string SkuId { get; set; } = string.Empty;

    /// <summary>
    /// 数量
    /// </summary>
    [Required(ErrorMessage = "数量不能为空")]
    [Range(1, int.MaxValue, ErrorMessage = "数量必须大于0")]
    public int Quantity { get; set; } = 1;
}