namespace EasyProduct.Models.Dto.Product.App;

/// <summary>
/// 会员端商品详情 DTO
/// </summary>
/// <remarks>
/// 用于会员端商品详情展示，包含完整商品信息、SKU 列表、规格模板等
/// </remarks>
public class AppProductDetailDto
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
    /// 商品编码
    /// </summary>
    public string? Code { get; set; }

    /// <summary>
    /// 商品描述
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 主图
    /// </summary>
    /// <remarks>
    /// 商品展示的主图片 URL
    /// </remarks>
    public string? MainImage { get; set; }

    /// <summary>
    /// 图片列表
    /// </summary>
    /// <remarks>
    /// 商品所有图片 URL 列表
    /// </remarks>
    public List<string>? Images { get; set; }

    /// <summary>
    /// 分类 ID
    /// </summary>
    public string? CategoryId { get; set; }

    /// <summary>
    /// 分类名称
    /// </summary>
    public string? CategoryName { get; set; }

    /// <summary>
    /// 品牌
    /// </summary>
    public string? Brand { get; set; }

    /// <summary>
    /// 计量单位
    /// </summary>
    public string? Unit { get; set; }

    /// <summary>
    /// SKU 列表
    /// </summary>
    /// <remarks>
    /// 商品的所有 SKU 变体信息
    /// </remarks>
    public List<AppSkuDto> Skus { get; set; } = new();

    /// <summary>
    /// 规格模板
    /// </summary>
    /// <remarks>
    /// 商品的规格定义（如颜色、尺寸等）
    /// </remarks>
    public SpecTemplateDto? SpecTemplate { get; set; }

    /// <summary>
    /// 销量统计
    /// </summary>
    /// <remarks>
    /// 商品的销量统计数据
    /// </remarks>
    public SalesStatisticsDto? SalesStats { get; set; }
}