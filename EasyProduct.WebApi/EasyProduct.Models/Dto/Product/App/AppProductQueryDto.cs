namespace EasyProduct.Models.Dto.Product.App;

/// <summary>
/// 会员端商品查询参数
/// </summary>
/// <remarks>
/// 用于会员端商品列表查询，支持分类、关键词、价格区间、品牌筛选和排序
/// </remarks>
public class AppProductQueryDto
{
    /// <summary>
    /// 分类 ID
    /// </summary>
    /// <remarks>
    /// 指定分类 ID 时，查询该分类及其子分类下的商品
    /// </remarks>
    public string? CategoryId { get; set; }

    /// <summary>
    /// 搜索关键词
    /// </summary>
    /// <remarks>
    /// 支持商品名称、商品编码模糊搜索
    /// </remarks>
    public string? Keyword { get; set; }

    /// <summary>
    /// 最低价格
    /// </summary>
    /// <remarks>
    /// 价格区间筛选下限
    /// </remarks>
    public decimal? MinPrice { get; set; }

    /// <summary>
    /// 最高价格
    /// </summary>
    /// <remarks>
    /// 价格区间筛选上限
    /// </remarks>
    public decimal? MaxPrice { get; set; }

    /// <summary>
    /// 品牌
    /// </summary>
    /// <remarks>
    /// 品牌名称筛选
    /// </remarks>
    public string? Brand { get; set; }

    /// <summary>
    /// 排序字段（price/sales/createTime）
    /// </summary>
    /// <remarks>
    /// price: 价格排序
    /// sales: 销量排序
    /// createTime: 创建时间排序（默认）
    /// </remarks>
    public string SortBy { get; set; } = "createTime";

    /// <summary>
    /// 排序方式（asc/desc）
    /// </summary>
    /// <remarks>
    /// asc: 升序
    /// desc: 降序（默认）
    /// </remarks>
    public string SortOrder { get; set; } = "desc";

    /// <summary>
    /// 页码
    /// </summary>
    /// <remarks>
    /// 从 1 开始，默认第 1 页
    /// </remarks>
    public int PageIndex { get; set; } = 1;

    /// <summary>
    /// 每页数量
    /// </summary>
    /// <remarks>
    /// 默认 10 条，最大 100 条
    /// </remarks>
    public int PageSize { get; set; } = 10;
}