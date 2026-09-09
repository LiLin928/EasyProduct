namespace EasyProduct.Models.Dto.Product.Channel;

/// <summary>
/// 渠道发布查询参数
/// </summary>
/// <remarks>
/// 用于查询渠道发布列表的筛选条件
/// </remarks>
public class ChannelQueryDto
{
    /// <summary>
    /// 商品ID
    /// </summary>
    /// <remarks>
    /// 按商品ID筛选，查询该商品的所有渠道发布情况
    /// </remarks>
    public string? SpuId { get; set; }

    /// <summary>
    /// 渠道编码
    /// </summary>
    /// <remarks>
    /// 按渠道编码筛选（site/miniapp/b2b）
    /// </remarks>
    public string? ChannelCode { get; set; }

    /// <summary>
    /// 上架状态
    /// </summary>
    /// <remarks>
    /// 按上架状态筛选
    /// 0=下架，1=上架
    /// </remarks>
    public int? Status { get; set; }
}