using System.ComponentModel.DataAnnotations;

namespace EasyProduct.Models.Dto.Product.Channel;

/// <summary>
/// 批量发布参数
/// </summary>
/// <remarks>
/// 用于批量将商品发布到多个渠道
/// </remarks>
public class BatchPublishDto
{
    /// <summary>
    /// 商品ID
    /// </summary>
    /// <remarks>
    /// 要发布的商品ID
    /// </remarks>
    [Required(ErrorMessage = "商品ID不能为空")]
    public string SpuId { get; set; } = string.Empty;

    /// <summary>
    /// 渠道编码列表
    /// </summary>
    /// <remarks>
    /// 要发布到的渠道编码列表（site/miniapp/b2b）
    /// </remarks>
    [Required(ErrorMessage = "渠道编码列表不能为空")]
    public List<string> ChannelCodes { get; set; } = new();

    /// <summary>
    /// 上架状态
    /// </summary>
    /// <remarks>
    /// 0=下架，1=上架
    /// </remarks>
    [Range(0, 1, ErrorMessage = "上架状态只能是0或1")]
    public int Status { get; set; } = 1;
}