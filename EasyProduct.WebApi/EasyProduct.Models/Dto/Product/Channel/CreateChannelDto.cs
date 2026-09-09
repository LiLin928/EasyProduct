using System.ComponentModel.DataAnnotations;

namespace EasyProduct.Models.Dto.Product.Channel;

/// <summary>
/// 创建渠道发布参数
/// </summary>
/// <remarks>
/// 用于创建新的渠道发布记录
/// 必填字段：SpuId、ChannelCode
/// 可选字段：Status、Sort、Price、ShowPrice、ShowStock、PublishTime、UnpublishTime
/// </remarks>
public class CreateChannelDto
{
    /// <summary>
    /// 商品ID
    /// </summary>
    /// <remarks>
    /// 要发布到渠道的商品ID
    /// </remarks>
    [Required(ErrorMessage = "商品ID不能为空")]
    public string SpuId { get; set; } = string.Empty;

    /// <summary>
    /// 渠道编码
    /// </summary>
    /// <remarks>
    /// 发布到的渠道编码（site/miniapp/b2b）
    /// </remarks>
    [Required(ErrorMessage = "渠道编码不能为空")]
    [MaxLength(20, ErrorMessage = "渠道编码不能超过20个字符")]
    public string ChannelCode { get; set; } = string.Empty;

    /// <summary>
    /// 上架状态
    /// </summary>
    /// <remarks>
    /// 0=下架，1=上架
    /// </remarks>
    [Range(0, 1, ErrorMessage = "上架状态只能是0或1")]
    public int Status { get; set; } = 1;

    /// <summary>
    /// 渠道排序
    /// </summary>
    /// <remarks>
    /// 商品在该渠道的显示顺序
    /// </remarks>
    [Range(0, int.MaxValue, ErrorMessage = "排序值必须大于等于0")]
    public int Sort { get; set; } = 0;

    /// <summary>
    /// 渠道价格（可选）
    /// </summary>
    /// <remarks>
    /// 该渠道的特殊价格，为空则使用SKU价格
    /// </remarks>
    [Range(0, double.MaxValue, ErrorMessage = "价格必须大于等于0")]
    public decimal? Price { get; set; }

    /// <summary>
    /// 是否显示价格
    /// </summary>
    /// <remarks>
    /// 控制该渠道是否显示价格
    /// </remarks>
    public bool ShowPrice { get; set; } = true;

    /// <summary>
    /// 是否显示库存
    /// </summary>
    /// <remarks>
    /// 控制该渠道是否显示库存
    /// </remarks>
    public bool ShowStock { get; set; } = true;

    /// <summary>
    /// 发布时间
    /// </summary>
    /// <remarks>
    /// 商品在该渠道的发布时间
    /// </remarks>
    public DateTime? PublishTime { get; set; }

    /// <summary>
    /// 下架时间
    /// </summary>
    /// <remarks>
    /// 商品在该渠道的下架时间
    /// </remarks>
    public DateTime? UnpublishTime { get; set; }
}