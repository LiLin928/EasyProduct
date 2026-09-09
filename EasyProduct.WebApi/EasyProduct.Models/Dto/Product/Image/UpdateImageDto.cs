using System.ComponentModel.DataAnnotations;

namespace EasyProduct.Models.Dto.Product.Image;

/// <summary>
/// 更新商品图片参数
/// </summary>
/// <remarks>
/// 用于更新已存在的商品图片信息
/// 必填字段：Id、ImageUrl
/// 可选字段：ThumbnailUrl、ImageName、Sort、IsMain
/// </remarks>
public class UpdateImageDto
{
    /// <summary>
    /// 图片ID
    /// </summary>
    /// <remarks>
    /// 要更新的图片ID
    /// </remarks>
    [Required(ErrorMessage = "图片ID不能为空")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// 图片URL
    /// </summary>
    /// <remarks>
    /// 图片的完整访问地址
    /// </remarks>
    [Required(ErrorMessage = "图片URL不能为空")]
    [MaxLength(500, ErrorMessage = "图片URL不能超过500个字符")]
    public string ImageUrl { get; set; } = string.Empty;

    /// <summary>
    /// 缩略图URL
    /// </summary>
    /// <remarks>
    /// 图片的缩略版本URL
    /// </remarks>
    [MaxLength(500)]
    public string? ThumbnailUrl { get; set; }

    /// <summary>
    /// 图片名称
    /// </summary>
    /// <remarks>
    /// 图片的原始文件名或自定义名称
    /// </remarks>
    [MaxLength(200)]
    public string? ImageName { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    /// <remarks>
    /// 图片在同商品下的显示顺序，数值越小越靠前
    /// </remarks>
    [Range(0, int.MaxValue, ErrorMessage = "排序值必须大于等于0")]
    public int Sort { get; set; }

    /// <summary>
    /// 是否主图
    /// </summary>
    /// <remarks>
    /// 标识该图片是否为商品的主图
    /// </remarks>
    public bool IsMain { get; set; }
}