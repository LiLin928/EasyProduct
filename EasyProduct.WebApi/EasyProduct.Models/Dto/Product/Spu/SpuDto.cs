using EasyProduct.Models.Enums;
using EasyProduct.Models.Enums.Product;

namespace EasyProduct.Models.Dto.Product.Spu;

/// <summary>
/// 商品主档 DTO
/// </summary>
/// <remarks>
/// 用于返回商品主档（SPU）信息，包含所有商品字段
/// 包含分类名称用于显示
/// </remarks>
public class SpuDto
{
    /// <summary>
    /// 商品ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 商品名称
    /// </summary>
    public string SpuName { get; set; } = string.Empty;

    /// <summary>
    /// 商品编码
    /// </summary>
    /// <remarks>
    /// 商品的唯一编码，用于业务系统内部标识
    /// </remarks>
    public string? SpuCode { get; set; }

    /// <summary>
    /// 分类ID
    /// </summary>
    /// <remarks>
    /// 关联 product_category 表的 Id 字段
    /// </remarks>
    public string? CategoryId { get; set; }

    /// <summary>
    /// 分类名称
    /// </summary>
    /// <remarks>
    /// 用于显示的分类名称，方便前端展示
    /// </remarks>
    public string? CategoryName { get; set; }

    /// <summary>
    /// 主图
    /// </summary>
    /// <remarks>
    /// 商品展示的主图片URL
    /// </remarks>
    public string? MainImage { get; set; }

    /// <summary>
    /// 商品图片列表
    /// </summary>
    /// <remarks>
    /// JSON 数组格式存储多张图片URL
    /// 示例：["url1", "url2", "url3"]
    /// </remarks>
    public string? Images { get; set; }

    /// <summary>
    /// 商品描述
    /// </summary>
    /// <remarks>
    /// 商品的详细描述，支持富文本
    /// </remarks>
    public string? Description { get; set; }

    /// <summary>
    /// 计量单位
    /// </summary>
    /// <remarks>
    /// 商品的计量单位，如：件、个、台、套等
    /// </remarks>
    public string? Unit { get; set; }

    /// <summary>
    /// 商品类型
    /// </summary>
    /// <remarks>
    /// 使用 SpuType 枚举：
    /// Physical（1）= 实物商品
    /// Virtual（2）= 虚拟商品
    /// Ticket（3）= 票品
    /// </remarks>
    public SpuType SpuType { get; set; }

    /// <summary>
    /// 品牌
    /// </summary>
    /// <remarks>
    /// 商品的品牌名称
    /// </remarks>
    public string? Brand { get; set; }

    /// <summary>
    /// 规格模板
    /// </summary>
    /// <remarks>
    /// JSON 格式存储规格模板配置
    /// 示例：[{"name":"颜色","values":["红色","蓝色"]},{"name":"尺寸","values":["S","M","L"]}]
    /// </remarks>
    public string? SpecTemplate { get; set; }

    /// <summary>
    /// 排序序号
    /// </summary>
    /// <remarks>
    /// 数值越小越靠前，用于商品列表排序
    /// </remarks>
    public int Sort { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    /// <remarks>
    /// 使用 Status 枚举：Disabled（0）= 禁用，Enabled（1）= 启用
    /// </remarks>
    public Status Status { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// 创建人ID
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// 更新人ID
    /// </summary>
    public string? UpdatedBy { get; set; }
}