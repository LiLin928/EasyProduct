using SqlSugar;
using EasyProduct.Models.Entitys.Base;

namespace EasyProduct.Models.Entitys.Mall;

/// <summary>
/// 收藏实体
/// </summary>
[SugarTable("mall_favorite", "收藏表")]
public class Favorite : BaseEntity
{
    /// <summary>
    /// 会员ID
    /// </summary>
    public Guid MemberId { get; set; }

    /// <summary>
    /// 商品SPU ID
    /// </summary>
    public Guid SpuId { get; set; }
}