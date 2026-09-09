namespace EasyProduct.Models.Dto.Mall.Favorite;

/// <summary>
/// 收藏DTO
/// </summary>
/// <remarks>
/// 用于返回收藏信息，包含所有收藏字段
/// 包含商品名称、图片、价格用于显示
/// </remarks>
public class FavoriteDto
{
    /// <summary>
    /// 主键ID
    /// </summary>
    /// <remarks>
    /// GUID 字符串格式
    /// </remarks>
    public string Id { get; set; } = null!;

    /// <summary>
    /// 会员ID
    /// </summary>
    /// <remarks>
    /// GUID 字符串格式
    /// </remarks>
    public string MemberId { get; set; } = null!;

    /// <summary>
    /// 商品SPU ID
    /// </summary>
    /// <remarks>
    /// GUID 字符串格式
    /// </remarks>
    public string SpuId { get; set; } = null!;

    /// <summary>
    /// 商品名称
    /// </summary>
    /// <remarks>
    /// 用于显示的商品名称，方便前端展示
    /// </remarks>
    public string? SpuName { get; set; }

    /// <summary>
    /// 商品图片
    /// </summary>
    /// <remarks>
    /// 用于显示的商品主图，方便前端展示
    /// </remarks>
    public string? SpuImage { get; set; }

    /// <summary>
    /// 商品价格
    /// </summary>
    /// <remarks>
    /// 用于显示的商品价格，方便前端展示
    /// </remarks>
    public decimal? Price { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreateTime { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime? UpdateTime { get; set; }
}