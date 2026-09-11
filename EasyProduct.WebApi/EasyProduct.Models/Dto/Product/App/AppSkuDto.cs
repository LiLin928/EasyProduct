namespace EasyProduct.Models.Dto.Product.App;

/// <summary>
/// 会员端 SKU DTO
/// </summary>
/// <remarks>
/// 用于会员端商品详情中的 SKU 信息展示
/// </remarks>
public class AppSkuDto
{
    /// <summary>
    /// SKU ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// SKU 名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// SKU 编码
    /// </summary>
    public string? Code { get; set; }

    /// <summary>
    /// 条码
    /// </summary>
    public string? Barcode { get; set; }

    /// <summary>
    /// 零售价
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// 会员价
    /// </summary>
    /// <remarks>
    /// 会员专享价格（可选）
    /// </remarks>
    public decimal? MemberPrice { get; set; }

    /// <summary>
    /// 库存
    /// </summary>
    public int Stock { get; set; }

    /// <summary>
    /// 规格列表
    /// </summary>
    /// <remarks>
    /// SKU 对应的规格组合，如 [{"name":"颜色","value":"红色"},{"name":"尺寸","value":"L"}]
    /// </remarks>
    public List<SpecItemDto>? Specs { get; set; }
}