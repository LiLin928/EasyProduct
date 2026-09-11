using SqlSugar;
using EasyProduct.Models.Entitys.Base;

namespace EasyProduct.Models.Entitys.Site;

/// <summary>
/// Banner实体
/// </summary>
/// <remarks>
/// 对应数据库表 site_banner
/// 继承 BaseEntity 基类，自动包含 Id、IsDeleted、Status、CreatedAt、UpdatedAt、CreatedBy、UpdatedBy 字段
/// 用于管理网站轮播图、广告图等信息
/// </remarks>
[SugarTable("site_banner", "Banner表")]
public class site_banner : BaseEntity
{
    /// <summary>
    /// Banner标题
    /// </summary>
    /// <remarks>
    /// Banner的显示标题，用于后台管理和标识
    /// 必填字段，最大长度100字符
    /// </remarks>
    [SugarColumn(Length = 100, ColumnDescription = "Banner标题")]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Banner图片URL
    /// </summary>
    /// <remarks>
    /// Banner图片的URL地址，必填字段
    /// 最大长度500字符
    /// </remarks>
    [SugarColumn(Length = 500, ColumnDescription = "Banner图片")]
    public string ImageUrl { get; set; } = string.Empty;

    /// <summary>
    /// 跳转链接
    /// </summary>
    /// <remarks>
    /// 点击Banner后跳转的URL地址
    /// 可选字段，最大长度500字符
    /// </remarks>
    [SugarColumn(Length = 500, IsNullable = true, ColumnDescription = "跳转链接")]
    public string? LinkUrl { get; set; }

    /// <summary>
    /// 跳转类型
    /// </summary>
    /// <remarks>
    /// Banner点击后的跳转类型
    /// 可选字段，最大长度20字符
    /// 枚举值：link（外链）、product（商品详情）、category（分类列表）、page（页面）
    /// 默认为 link
    /// </remarks>
    [SugarColumn(Length = 20, IsNullable = true, ColumnDescription = "跳转类型")]
    public string? LinkType { get; set; } = "link";

    /// <summary>
    /// 跳转参数
    /// </summary>
    /// <remarks>
    /// 根据跳转类型存储相关参数
    /// - link: 存储完整URL
    /// - product: 存储商品ID
    /// - category: 存储分类ID
    /// - page: 存储页面路径
    /// 可选字段，最大长度200字符
    /// </remarks>
    [SugarColumn(Length = 200, IsNullable = true, ColumnDescription = "跳转参数")]
    public string? LinkParam { get; set; }

    /// <summary>
    /// 显示位置
    /// </summary>
    /// <remarks>
    /// Banner显示的位置标识
    /// 可选字段，最大长度50字符
    /// 常见值：home（首页）、product_list（商品列表）、detail（详情页）
    /// 默认为 home
    /// </remarks>
    [SugarColumn(Length = 50, IsNullable = true, ColumnDescription = "显示位置")]
    public string? Position { get; set; } = "home";

    /// <summary>
    /// 开始时间
    /// </summary>
    /// <remarks>
    /// Banner开始显示的时间
    /// 可选字段，为空表示立即显示
    /// </remarks>
    [SugarColumn(IsNullable = true, ColumnDescription = "开始时间")]
    public DateTime? StartTime { get; set; }

    /// <summary>
    /// 结束时间
    /// </summary>
    /// <remarks>
    /// Banner结束显示的时间
    /// 可选字段，为空表示永久显示
    /// </remarks>
    [SugarColumn(IsNullable = true, ColumnDescription = "结束时间")]
    public DateTime? EndTime { get; set; }

    /// <summary>
    /// 排序号
    /// </summary>
    /// <remarks>
    /// 用于控制Banner在列表中的显示顺序
    /// 数值越小越靠前，默认值为0
    /// </remarks>
    [SugarColumn(ColumnDescription = "排序号")]
    public int Sort { get; set; } = 0;

    /// <summary>
    /// 描述
    /// </summary>
    /// <remarks>
    /// Banner的详细描述说明
    /// 可选字段，最大长度500字符
    /// </remarks>
    [SugarColumn(Length = 500, IsNullable = true, ColumnDescription = "描述")]
    public string? Description { get; set; }
}