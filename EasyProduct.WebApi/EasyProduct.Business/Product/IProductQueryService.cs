using EasyProduct.Common.Base;
using EasyProduct.Models.Dto.Product.App;

namespace EasyProduct.Business.Product;

/// <summary>
/// 商品查询服务接口（会员端）
/// </summary>
/// <remarks>
/// 封装会员端商品查询逻辑，复用现有的 ISpuService、ICategoryService、ISkuService
/// 提供商品列表、详情、分类、搜索、热销、新品推荐等功能
/// </remarks>
public interface IProductQueryService
{
    /// <summary>
    /// 查询商品列表（会员端）
    /// </summary>
    /// <param name="query">查询参数，包含分类、关键词、价格区间、排序等</param>
    /// <returns>商品列表分页结果</returns>
    /// <remarks>
    /// 支持按分类、关键词、价格区间筛选
    /// 支持按价格、销量、创建时间排序
    /// 支持分页查询
    /// </remarks>
    Task<PageResponse<AppProductListDto>> GetProductListAsync(AppProductQueryDto query);

    /// <summary>
    /// 获取商品详情（会员端）
    /// </summary>
    /// <param name="productId">商品 ID（GUID 格式）</param>
    /// <returns>商品详情，包含基本信息、SKU 列表、规格模板等</returns>
    /// <remarks>
    /// 返回完整的商品信息，包括：
    /// - 基本信息（名称、编码、描述、品牌等）
    /// - 图片列表
    /// - SKU 列表（价格、库存、规格）
    /// - 规格模板
    /// - 销量统计
    /// </remarks>
    Task<AppProductDetailDto> GetProductDetailAsync(string productId);

    /// <summary>
    /// 获取商品分类树
    /// </summary>
    /// <returns>分类树，包含所有分类的层级结构</returns>
    /// <remarks>
    /// 返回树形结构的分类列表，用于会员端分类导航
    /// </remarks>
    Task<List<CategoryTreeDto>> GetCategoryTreeAsync();

    /// <summary>
    /// 搜索商品
    /// </summary>
    /// <param name="keyword">搜索关键词，匹配商品名称、编码</param>
    /// <returns>匹配的商品列表</returns>
    /// <remarks>
    /// 支持商品名称、编码的模糊搜索
    /// 返回最多前 50 个匹配结果
    /// </remarks>
    Task<List<AppProductListDto>> SearchProductsAsync(string keyword);

    /// <summary>
    /// 获取热销商品
    /// </summary>
    /// <param name="limit">返回数量限制，默认 10 个</param>
    /// <returns>热销商品列表，按销量倒序排列</returns>
    /// <remarks>
    /// 返回销量最高的商品列表
    /// 用于首页推荐、热销榜单等场景
    /// </remarks>
    Task<List<AppProductListDto>> GetHotProductsAsync(int limit = 10);

    /// <summary>
    /// 获取新品推荐
    /// </summary>
    /// <param name="limit">返回数量限制，默认 10 个</param>
    /// <returns>新品推荐列表，按创建时间倒序排列</returns>
    /// <remarks>
    /// 返回最近创建的商品列表
    /// 用于首页推荐、新品上市等场景
    /// </remarks>
    Task<List<AppProductListDto>> GetNewProductsAsync(int limit = 10);
}