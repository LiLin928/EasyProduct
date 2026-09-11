using EasyProduct.Models.Enums;

namespace EasyProduct.Models.Dto.Site.Banner;

/// <summary>
/// Banner DTO
/// </summary>
/// <remarks>
/// 用于返回Banner信息，包含所有Banner字段
/// 对应实体类 site_banner
/// </remarks>
public class BannerDto
{
    /// <summary>
    /// BannerID
    /// </summary>
    /// <remarks>
    /// GUID 主键，唯一标识
    /// </remarks>
    public Guid Id { get; set; }

    /// <summary>
    /// Banner标题
    /// </summary>
    /// <remarks>
    /// Banner的显示标题，用于后台管理和标识
    /// </remarks>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Banner图片URL
    /// </summary>
    /// <remarks>
    /// Banner图片的URL地址
    /// </remarks>
    public string ImageUrl { get; set; } = string.Empty;

    /// <summary>
    /// 跳转链接
    /// </summary>
    /// <remarks>
    /// 点击Banner后跳转的URL地址
    /// </remarks>
    public string? LinkUrl { get; set; }

    /// <summary>
    /// 跳转类型
    /// </summary>
    /// <remarks>
    /// Banner点击后的跳转类型
    /// 枚举值：link（外链）、product（商品详情）、category（分类列表）、page（页面）
    /// </remarks>
    public string? LinkType { get; set; }

    /// <summary>
    /// 跳转参数
    /// </summary>
    /// <remarks>
    /// 根据跳转类型存储相关参数
    /// - link: 存储完整URL
    /// - product: 存储商品ID
    /// - category: 存储分类ID
    /// - page: 存储页面路径
    /// </remarks>
    public string? LinkParam { get; set; }

    /// <summary>
    /// 显示位置
    /// </summary>
    /// <remarks>
    /// Banner显示的位置标识
    /// 常见值：home（首页）、product_list（商品列表）、detail（详情页）
    /// </remarks>
    public string? Position { get; set; }

    /// <summary>
    /// 开始时间
    /// </summary>
    /// <remarks>
    /// Banner开始显示的时间
    /// </remarks>
    public DateTime? StartTime { get; set; }

    /// <summary>
    /// 结束时间
    /// </summary>
    /// <remarks>
    /// Banner结束显示的时间
    /// </remarks>
    public DateTime? EndTime { get; set; }

    /// <summary>
    /// 排序号
    /// </summary>
    /// <remarks>
    /// 用于控制Banner在列表中的显示顺序
    /// 数值越小越靠前
    /// </remarks>
    public int Sort { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    /// <remarks>
    /// Banner的详细描述说明
    /// </remarks>
    public string? Description { get; set; }

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
}