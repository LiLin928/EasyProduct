namespace EasyProduct.Models.Dto.Product.App;

/// <summary>
/// 会员端商品列表 DTO
/// </summary>
/// <remarks>
/// 用于会员端商品列表展示，包含基本信息、价格、销量、分类等
/// </remarks>
public class AppProductListDto
{
    /// <summary>
    /// 商品 ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 商品名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 主图
    /// </summary>
    /// <remarks>
    /// 商品展示的主图片 URL
    /// </remarks>
    public string? MainImage { get; set; }

    /// <summary>
    /// 最低价格
    /// </summary>
    /// <remarks>
    /// 商品所有 SKU 中的最低价格
    /// </remarks>
    public decimal MinPrice { get; set; }

    /// <summary>
    /// 最高价格
    /// </summary>
    /// <remarks>
    /// 商品所有 SKU 中的最高价格（可选，单 SKU 时为 null）
    /// </remarks>
    public decimal? MaxPrice { get; set; }

    /// <summary>
    /// 会员价
    /// </summary>
    /// <remarks>
    /// 会员专享价格（可选）
    /// </remarks>
    public decimal? MemberPrice { get; set; }

    /// <summary>
    /// 销量
    /// </summary>
    /// <remarks>
    /// 商品总销量统计
    /// </remarks>
    public int SalesCount { get; set; }

    /// <summary>
    /// 分类名称
    /// </summary>
    /// <remarks>
    /// 商品所属分类名称
    /// </remarks>
    public string? CategoryName { get; set; }
}
