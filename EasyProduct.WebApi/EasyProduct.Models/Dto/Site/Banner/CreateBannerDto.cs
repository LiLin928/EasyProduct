using System.ComponentModel.DataAnnotations;

namespace EasyProduct.Models.Dto.Site.Banner;

/// <summary>
/// 创建Banner请求 DTO
/// </summary>
/// <remarks>
/// 用于创建新的Banner
/// 必填字段：Title、ImageUrl
/// 可选字段：LinkUrl、LinkType、LinkParam、Position、StartTime、EndTime、Sort、Description
/// </remarks>
public class CreateBannerDto
{
    /// <summary>
    /// Banner标题
    /// </summary>
    /// <remarks>
    /// 必填，最大长度 100 个字符
    /// </remarks>
    [Required(ErrorMessage = "Banner标题不能为空")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "Banner标题长度必须在1-100个字符之间")]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Banner图片URL
    /// </summary>
    /// <remarks>
    /// 必填，最大长度 500 个字符
    /// </remarks>
    [Required(ErrorMessage = "Banner图片URL不能为空")]
    [StringLength(500, ErrorMessage = "Banner图片URL长度不能超过500个字符")]
    public string ImageUrl { get; set; } = string.Empty;

    /// <summary>
    /// 跳转链接
    /// </summary>
    /// <remarks>
    /// 可选，最大长度 500 个字符
    /// </remarks>
    [StringLength(500, ErrorMessage = "跳转链接长度不能超过500个字符")]
    public string? LinkUrl { get; set; }

    /// <summary>
    /// 跳转类型
    /// </summary>
    /// <remarks>
    /// 可选，最大长度 20 个字符
    /// 枚举值：link（外链）、product（商品详情）、category（分类列表）、page（页面）
    /// 默认为 link
    /// </remarks>
    [StringLength(20, ErrorMessage = "跳转类型长度不能超过20个字符")]
    public string? LinkType { get; set; } = "link";

    /// <summary>
    /// 跳转参数
    /// </summary>
    /// <remarks>
    /// 可选，最大长度 200 个字符
    /// 根据跳转类型存储相关参数
    /// </remarks>
    [StringLength(200, ErrorMessage = "跳转参数长度不能超过200个字符")]
    public string? LinkParam { get; set; }

    /// <summary>
    /// 显示位置
    /// </summary>
    /// <remarks>
    /// 可选，最大长度 50 个字符
    /// 常见值：home（首页）、product_list（商品列表）、detail（详情页）
    /// 默认为 home
    /// </remarks>
    [StringLength(50, ErrorMessage = "显示位置长度不能超过50个字符")]
    public string? Position { get; set; } = "home";

    /// <summary>
    /// 开始时间
    /// </summary>
    /// <remarks>
    /// 可选，为空表示立即显示
    /// </remarks>
    public DateTime? StartTime { get; set; }

    /// <summary>
    /// 结束时间
    /// </summary>
    /// <remarks>
    /// 可选，为空表示永久显示
    /// </remarks>
    public DateTime? EndTime { get; set; }

    /// <summary>
    /// 排序号
    /// </summary>
    /// <remarks>
    /// 数值越小越靠前，默认为 0
    /// </remarks>
    [Range(0, int.MaxValue, ErrorMessage = "排序值必须大于或等于0")]
    public int Sort { get; set; } = 0;

    /// <summary>
    /// 描述
    /// </summary>
    /// <remarks>
    /// 可选，最大长度 500 个字符
    /// </remarks>
    [StringLength(500, ErrorMessage = "描述长度不能超过500个字符")]
    public string? Description { get; set; }
}