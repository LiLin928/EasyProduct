using System.ComponentModel.DataAnnotations;

namespace EasyProduct.Models.Dto.Mall.Cart;

/// <summary>
/// 批量添加到购物车DTO
/// </summary>
/// <remarks>
/// 用于批量添加商品到购物车，包含多个购物车项
/// </remarks>
public class CartBatchAddDto
{
    /// <summary>
    /// 购物车项列表
    /// </summary>
    [Required(ErrorMessage = "购物车项列表不能为空")]
    public List<CartAddDto> Items { get; set; } = new();
}