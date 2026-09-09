using System.ComponentModel.DataAnnotations;

namespace EasyProduct.Models.Dto.Mall.Cart;

/// <summary>
/// 修改选中状态DTO
/// </summary>
/// <remarks>
/// 用于批量修改购物车商品的选中状态
/// </remarks>
public class CartUpdateSelectedDto
{
    /// <summary>
    /// 购物车ID列表
    /// </summary>
    [Required(ErrorMessage = "购物车ID列表不能为空")]
    public List<string> Ids { get; set; } = new();

    /// <summary>
    /// 是否选中：0=否，1=是
    /// </summary>
    [Required(ErrorMessage = "选中状态不能为空")]
    [Range(0, 1, ErrorMessage = "选中状态只能为0或1")]
    public int Selected { get; set; }
}