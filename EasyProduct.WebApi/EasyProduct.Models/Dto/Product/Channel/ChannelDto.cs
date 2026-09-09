namespace EasyProduct.Models.Dto.Product.Channel;

/// <summary>
/// 渠道发布DTO
/// </summary>
/// <remarks>
/// 渠道发布的完整信息，用于返回给前端展示
/// 包含渠道的基本信息、商品信息、上架状态、价格设置等
/// </remarks>
public class ChannelDto
{
    /// <summary>
    /// 渠道发布ID
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// 商品ID
    /// </summary>
    public string SpuId { get; set; } = string.Empty;

    /// <summary>
    /// 商品名称
    /// </summary>
    /// <remarks>
    /// 关联的商品名称（查询时填充）
    /// </remarks>
    public string? SpuName { get; set; }

    /// <summary>
    /// 渠道编码
    /// </summary>
    public string ChannelCode { get; set; } = string.Empty;

    /// <summary>
    /// 渠道名称
    /// </summary>
    /// <remarks>
    /// 渠道的中文名称（查询时填充）
    /// </remarks>
    public string? ChannelName { get; set; }

    /// <summary>
    /// 上架状态
    /// </summary>
    /// <remarks>
    /// 0=下架，1=上架
    /// </remarks>
    public int Status { get; set; }

    /// <summary>
    /// 渠道排序
    /// </summary>
    public int Sort { get; set; }

    /// <summary>
    /// 渠道价格
    /// </summary>
    /// <remarks>
    /// 该渠道的特殊价格
    /// </remarks>
    public decimal? Price { get; set; }

    /// <summary>
    /// 是否显示价格
    /// </summary>
    public bool ShowPrice { get; set; }

    /// <summary>
    /// 是否显示库存
    /// </summary>
    public bool ShowStock { get; set; }

    /// <summary>
    /// 发布时间
    /// </summary>
    public DateTime? PublishTime { get; set; }

    /// <summary>
    /// 下架时间
    /// </summary>
    public DateTime? UnpublishTime { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// 创建人
    /// </summary>
    public string? CreatedBy { get; set; }
}