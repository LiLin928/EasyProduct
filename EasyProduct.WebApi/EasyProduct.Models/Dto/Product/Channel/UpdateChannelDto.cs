using System.ComponentModel.DataAnnotations;

namespace EasyProduct.Models.Dto.Product.Channel;

/// <summary>
/// 更新渠道发布参数
/// </summary>
/// <remarks>
/// 用于更新已存在的渠道发布信息
/// 必填字段：Id
/// 可选字段：Status、Sort、Price、ShowPrice、ShowStock、PublishTime、UnpublishTime
/// </remarks>
public class UpdateChannelDto
{
    /// <summary>
    /// 渠道发布ID
    /// </summary>
    /// <remarks>
    /// 要更新的渠道发布ID
    /// </remarks>
    [Required(ErrorMessage = "渠道发布ID不能为空")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// 上架状态
    /// </summary>
    /// <remarks>
    /// 0=下架，1=上架
    /// </remarks>
    [Range(0, 1, ErrorMessage = "上架状态只能是0或1")]
    public int Status { get; set; }

    /// <summary>
    /// 渠道排序
    /// </summary>
    /// <remarks>
    /// 商品在该渠道的显示顺序
    /// </remarks>
    [Range(0, int.MaxValue, ErrorMessage = "排序值必须大于等于0")]
    public int Sort { get; set; }

    /// <summary>
    /// 渠道价格（可选）
    /// </summary>
    /// <remarks>
    /// 该渠道的特殊价格
    /// </remarks>
    [Range(0, double.MaxValue, ErrorMessage = "价格必须大于等于0")]
    public decimal? Price { get; set; }

    /// <summary>
    /// 是否显示价格
    /// </summary>
    /// <remarks>
    /// 控制该渠道是否显示价格
    /// </remarks>
    public bool ShowPrice { get; set; }

    /// <summary>
    /// 是否显示库存
    /// </summary>
    /// <remarks>
    /// 控制该渠道是否显示库存
    /// </remarks>
    public bool ShowStock { get; set; }

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