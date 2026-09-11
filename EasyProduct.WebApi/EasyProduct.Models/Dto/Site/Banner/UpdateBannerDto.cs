using System.ComponentModel.DataAnnotations;

namespace EasyProduct.Models.Dto.Site.Banner;

/// <summary>
/// 更新Banner请求 DTO
/// </summary>
/// <remarks>
/// 用于更新已存在的Banner
/// 必填字段：Id
/// 可选字段：Title、ImageUrl、LinkUrl、LinkType、LinkParam、Position、StartTime、EndTime、Sort、Description、Status
/// </remarks>
public class UpdateBannerDto
{
    /// <summary>
    /// BannerID
    /// </summary>
    /// <remarks>
    /// 必填，GUID 格式
    /// </remarks>
    [Required(ErrorMessage = "BannerID不能为空")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Banner标题
    /// </summary>
    /// <remarks>
    /// 可选，最大长度 100 个字符
    /// </remarks>
    [StringLength(100, MinimumLength = 1, ErrorMessage = "Banner标题长度必须在1-100个字符之间")]
    public string? Title { get; set; }

    /// <summary>
    /// Banner图片URL
    /// </summary>
    /// <remarks>
    /// 可选，最大长度 500 个字符
    /// </remarks>
    [StringLength(500, ErrorMessage = "Banner图片URL长度不能超过500个字符")]
    public string? ImageUrl { get; set; }

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
    /// </remarks>
    [StringLength(20, ErrorMessage = "跳转类型长度不能超过20个字符")]
    public string? LinkType { get; set; }

    /// <summary>
    /// 跳转参数
    /// </summary>
    /// <remarks>
    /// 可选，最大长度 200 个字符
    /// </remarks>
    [StringLength(200, ErrorMessage = "跳转参数长度不能超过200个字符")]
    public string? LinkParam { get; set; }

    /// <summary>
    /// 显示位置
    /// </summary>
    /// <remarks>
    /// 可选，最大长度 50 个字符
    /// </remarks>
    [StringLength(50, ErrorMessage = "显示位置长度不能超过50个字符")]
    public string? Position { get; set; }

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
    /// 数值越小越靠前
    /// </remarks>
    [Range(0, int.MaxValue, ErrorMessage = "排序值必须大于或等于0")]
    public int? Sort { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    /// <remarks>
    /// 可选，最大长度 500 个字符
    /// </remarks>
    [StringLength(500, ErrorMessage = "描述长度不能超过500个字符")]
    public string? Description { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    /// <remarks>
    /// 0=禁用，1=启用
    /// </remarks>
    [Range(0, 1, ErrorMessage = "状态只能是0或1")]
    public int? Status { get; set; }
}