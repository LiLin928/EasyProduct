using System.ComponentModel.DataAnnotations;
using EasyProduct.Models.Enums;

namespace EasyProduct.Models.Dto.Site.Banner;

/// <summary>
/// Banner查询 DTO
/// </summary>
/// <remarks>
/// 用于Banner列表查询，支持按标题、显示位置、状态、时间范围筛选
/// 包含分页参数
/// </remarks>
public class BannerQueryDto
{
    /// <summary>
    /// 页码，从 1 开始
    /// </summary>
    public int PageIndex { get; set; } = 1;

    /// <summary>
    /// 每页数量
    /// </summary>
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// 标题关键词
    /// </summary>
    /// <remarks>
    /// 支持模糊搜索，最大长度 100 个字符
    /// </remarks>
    [StringLength(100, ErrorMessage = "标题长度不能超过100个字符")]
    public string? Title { get; set; }

    /// <summary>
    /// 显示位置
    /// </summary>
    /// <remarks>
    /// 精确匹配显示位置
    /// 常见值：home（首页）、product_list（商品列表）、detail（详情页）
    /// </remarks>
    [StringLength(50, ErrorMessage = "显示位置长度不能超过50个字符")]
    public string? Position { get; set; }

    /// <summary>
    /// 状态筛选
    /// </summary>
    /// <remarks>
    /// 使用 Status 枚举：Disabled（0）= 禁用，Enabled（1）= 启用
    /// </remarks>
    public Status? Status { get; set; }

    /// <summary>
    /// 开始时间-起
    /// </summary>
    /// <remarks>
    /// 筛选开始时间大于等于此时间的Banner
    /// </remarks>
    public DateTime? StartTimeBegin { get; set; }

    /// <summary>
    /// 开始时间-止
    /// </summary>
    /// <remarks>
    /// 筛选开始时间小于等于此时间的Banner
    /// </remarks>
    public DateTime? StartTimeEnd { get; set; }

    /// <summary>
    /// 结束时间-起
    /// </summary>
    /// <remarks>
    /// 筛选结束时间大于等于此时间的Banner
    /// </remarks>
    public DateTime? EndTimeBegin { get; set; }

    /// <summary>
    /// 结束时间-止
    /// </summary>
    /// <remarks>
    /// 筛选结束时间小于等于此时间的Banner
    /// </remarks>
    public DateTime? EndTimeEnd { get; set; }
}