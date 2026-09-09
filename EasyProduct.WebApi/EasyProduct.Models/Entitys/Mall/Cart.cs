using System.ComponentModel.DataAnnotations;
using SqlSugar;
using EasyProduct.Models.Entitys.Base;

namespace EasyProduct.Models.Entitys.Mall;

/// <summary>
/// 购物车实体
/// </summary>
[SugarTable("mall_cart", "购物车表")]
public class Cart : BaseEntity
{
    /// <summary>
    /// 会员ID
    /// </summary>
    [SugarColumn(Length = 36, IsNullable = false)]
    [Required]
    public string MemberId { get; set; } = string.Empty;

    /// <summary>
    /// SKU ID
    /// </summary>
    [SugarColumn(Length = 36, IsNullable = false)]
    [Required]
    public string SkuId { get; set; } = string.Empty;

    /// <summary>
    /// 数量
    /// </summary>
    [SugarColumn(IsNullable = false)]
    [Range(1, int.MaxValue, ErrorMessage = "数量必须大于0")]
    public int Quantity { get; set; } = 1;

    /// <summary>
    /// 是否选中：0=否，1=是
    /// </summary>
    [SugarColumn(IsNullable = false)]
    public int Selected { get; set; } = 1;
}