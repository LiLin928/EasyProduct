using System.ComponentModel.DataAnnotations;
using EasyProduct.Models.Enums;
using EasyProduct.Models.Enums.Product;

namespace EasyProduct.Models.Dto.Product.Spu;

/// <summary>
/// 更新商品主档请求 DTO
/// </summary>
/// <remarks>
/// 用于更新商品主档（SPU）信息
/// 必填字段：Id、SpuName、SpuType
/// 可选字段：SpuCode、CategoryId、MainImage、Images、Description、Unit、Brand、SpecTemplate、Sort、Status
/// </remarks>
public class UpdateSpuDto
{
    /// <summary>
    /// 商品ID
    /// </summary>
    /// <remarks>
    /// 必填，GUID 字符串格式
    /// </remarks>
    [Required(ErrorMessage = "商品ID不能为空")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// 商品名称
    /// </summary>
    /// <remarks>
    /// 必填，最大长度 200 个字符
    /// </remarks>
    [Required(ErrorMessage = "商品名称不能为空")]
    [StringLength(200, MinimumLength = 1, ErrorMessage = "商品名称长度必须在1-200个字符之间")]
    public string SpuName { get; set; } = string.Empty;

    /// <summary>
    /// 商品编码
    /// </summary>
    /// <remarks>
    /// 商品的唯一编码，用于业务系统内部标识
    /// 最大长度 50 个字符
    /// </remarks>
    [StringLength(50, ErrorMessage = "商品编码长度不能超过50个字符")]
    public string? SpuCode { get; set; }

    /// <summary>
    /// 分类ID
    /// </summary>
    /// <remarks>
    /// 关联 product_category 表的 Id 字段
    /// GUID 字符串格式，最大长度 36 个字符
    /// </remarks>
    [StringLength(36, ErrorMessage = "分类ID长度不能超过36个字符")]
    public string? CategoryId { get; set; }

    /// <summary>
    /// 主图
    /// </summary>
    /// <remarks>
    /// 商品展示的主图片URL
    /// 最大长度 500 个字符
    /// </remarks>
    [StringLength(500, ErrorMessage = "主图长度不能超过500个字符")]
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
    /// 最大长度 20 个字符
    /// </remarks>
    [StringLength(20, ErrorMessage = "计量单位长度不能超过20个字符")]
    public string? Unit { get; set; }

    /// <summary>
    /// 商品类型
    /// </summary>
    /// <remarks>
    /// 必填，使用 SpuType 枚举：
    /// Physical（1）= 实物商品
    /// Virtual（2）= 虚拟商品
    /// Ticket（3）= 票品
    /// </remarks>
    [Required(ErrorMessage = "商品类型不能为空")]
    public SpuType SpuType { get; set; } = SpuType.Physical;

    /// <summary>
    /// 品牌
    /// </summary>
    /// <remarks>
    /// 商品的品牌名称
    /// 最大长度 100 个字符
    /// </remarks>
    [StringLength(100, ErrorMessage = "品牌长度不能超过100个字符")]
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
    /// 默认为 0
    /// </remarks>
    [Range(0, int.MaxValue, ErrorMessage = "排序值必须大于或等于0")]
    public int Sort { get; set; } = 0;

    /// <summary>
    /// 状态
    /// </summary>
    /// <remarks>
    /// 使用 Status 枚举：Disabled（0）= 禁用，Enabled（1）= 启用
    /// 默认为启用状态
    /// </remarks>
    public Status Status { get; set; } = Status.Enabled;
}