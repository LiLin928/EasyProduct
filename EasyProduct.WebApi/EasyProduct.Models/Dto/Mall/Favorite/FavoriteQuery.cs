namespace EasyProduct.Models.Dto.Mall.Favorite;

/// <summary>
/// 收藏查询参数
/// </summary>
/// <remarks>
/// 用于收藏列表查询，支持按会员ID、商品SPU ID筛选
/// 包含分页参数
/// </remarks>
public class FavoriteQuery
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
    /// 会员ID
    /// </summary>
    /// <remarks>
    /// 精确匹配，GUID 格式
    /// </remarks>
    public Guid? MemberId { get; set; }

    /// <summary>
    /// 商品SPU ID
    /// </summary>
    /// <remarks>
    /// 精确匹配，GUID 格式
    /// </remarks>
    public Guid? SpuId { get; set; }
}