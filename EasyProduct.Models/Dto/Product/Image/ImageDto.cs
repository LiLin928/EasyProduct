namespace EasyProduct.Models.Dto.Product.Image;

/// <summary>
/// 商品图片DTO
/// </summary>
/// <remarks>
/// 商品图片的完整信息，用于返回给前端展示
/// 包含图片的基本信息、关联信息、排序信息等
/// </remarks>
public class ImageDto
{
    /// <summary>
    /// 图片ID
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// 商品ID
    /// </summary>
    public string SpuId { get; set; } = string.Empty;

    /// <summary>
    /// 图片URL
    /// </summary>
    public string ImageUrl { get; set; } = string.Empty;

    /// <summary>
    /// 缩略图URL
    /// </summary>
    public string? ThumbnailUrl { get; set; }

    /// <summary>
    /// 图片名称
    /// </summary>
    public string? ImageName { get; set; }

    /// <summary>
    /// 图片大小（字节）
    /// </summary>
    public int? ImageSize { get; set; }

    /// <summary>
    /// 图片类型
    /// </summary>
    public string? ImageType { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    public int Sort { get; set; }

    /// <summary>
    /// 是否主图
    /// </summary>
    public bool IsMain { get; set; }

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