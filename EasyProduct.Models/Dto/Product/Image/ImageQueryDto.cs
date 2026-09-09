namespace EasyProduct.Models.Dto.Product.Image;

/// <summary>
/// 商品图片查询参数
/// </summary>
/// <remarks>
/// 用于查询商品图片列表的筛选条件
/// </remarks>
public class ImageQueryDto
{
    /// <summary>
    /// 商品ID
    /// </summary>
    /// <remarks>
    /// 按商品ID筛选，查询该商品的所有图片
    /// </remarks>
    public string? SpuId { get; set; }

    /// <summary>
    /// 是否主图
    /// </summary>
    /// <remarks>
    /// 按主图标记筛选
    /// true = 只查询主图
    /// false = 只查询非主图
    /// null = 查询所有图片
    /// </remarks>
    public bool? IsMain { get; set; }
}