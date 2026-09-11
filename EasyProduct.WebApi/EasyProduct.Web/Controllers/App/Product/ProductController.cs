using EasyProduct.Business.Product;
using EasyProduct.Common.Base;
using EasyProduct.Models.Dto.Product.App;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace EasyProduct.Web.Controllers.App.Product;

/// <summary>
/// 会员端商品控制器
/// </summary>
/// <remarks>
/// 提供小程序会员的商品查询接口，包括商品列表、详情、分类、搜索、热销、新品推荐等功能
/// 所有接口需要会员权限（MemberJwt 认证）
/// </remarks>
[ApiController]
[Route("api/app/product")]
[Authorize(AuthenticationSchemes = "MemberJwt")]
public class ProductController : BaseController
{
    private readonly IProductQueryService _productQueryService;
    private readonly ILogger<ProductController> _logger;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="productQueryService">商品查询服务接口</param>
    /// <param name="logger">日志记录器</param>
    public ProductController(
        IProductQueryService productQueryService,
        ILogger<ProductController> logger)
    {
        _productQueryService = productQueryService;
        _logger = logger;
    }

    /// <summary>
    /// 获取商品列表（支持筛选、排序、分页）
    /// </summary>
    /// <param name="query">查询参数，包含分类、关键词、价格区间、排序等</param>
    /// <returns>商品列表分页结果</returns>
    /// <remarks>
    /// 支持按分类、关键词、价格区间筛选
    /// 支持按价格、销量、创建时间排序
    /// 支持分页查询（默认 pageIndex=1, pageSize=10）
    /// </remarks>
    [HttpGet]
    [EnableRateLimiting("AppPolicy")]
    public async Task<ApiResponse<PageResponse<AppProductListDto>>> GetList([FromQuery] AppProductQueryDto query)
    {
        var result = await _productQueryService.GetProductListAsync(query);
        return Success(result);
    }

    /// <summary>
    /// 获取商品详情
    /// </summary>
    /// <param name="id">商品 ID（GUID 格式）</param>
    /// <returns>商品详情，包含基本信息、SKU 列表、规格模板等</returns>
    /// <remarks>
    /// 返回完整的商品信息，包括：
    /// - 基本信息（名称、编码、描述、品牌等）
    /// - 图片列表
    /// - SKU 列表（价格、库存、规格）
    /// - 规格模板
    /// - 销量统计
    /// </remarks>
    [HttpGet("{id}")]
    [EnableRateLimiting("AppPolicy")]
    public async Task<ApiResponse<AppProductDetailDto>> GetDetail(string id)
    {
        var result = await _productQueryService.GetProductDetailAsync(id);
        return Success(result);
    }

    /// <summary>
    /// 获取商品分类（树形结构）
    /// </summary>
    /// <returns>分类树，包含所有分类的层级结构</returns>
    /// <remarks>
    /// 返回树形结构的分类列表，用于会员端分类导航
    /// 包含分类 ID、名称、图标、图片及子分类
    /// </remarks>
    [HttpGet("categories")]
    [EnableRateLimiting("AppPolicy")]
    public async Task<ApiResponse<List<CategoryTreeDto>>> GetCategories()
    {
        var result = await _productQueryService.GetCategoryTreeAsync();
        return Success(result);
    }

    /// <summary>
    /// 搜索商品
    /// </summary>
    /// <param name="keyword">搜索关键词，匹配商品名称、编码</param>
    /// <returns>匹配的商品列表</returns>
    /// <remarks>
    /// 支持商品名称、编码的模糊搜索
    /// 返回最多前 50 个匹配结果
    /// </remarks>
    [HttpGet("search")]
    [EnableRateLimiting("AppPolicy")]
    public async Task<ApiResponse<List<AppProductListDto>>> Search([FromQuery] string keyword)
    {
        var result = await _productQueryService.SearchProductsAsync(keyword);
        return Success(result);
    }

    /// <summary>
    /// 获取热销商品
    /// </summary>
    /// <param name="limit">返回数量限制，默认 10 个，最大 50 个</param>
    /// <returns>热销商品列表，按销量倒序排列</returns>
    /// <remarks>
    /// 返回销量最高的商品列表
    /// 用于首页推荐、热销榜单等场景
    /// </remarks>
    [HttpGet("hot")]
    [EnableRateLimiting("AppPolicy")]
    public async Task<ApiResponse<List<AppProductListDto>>> GetHot([FromQuery] int limit = 10)
    {
        // 限制最大返回数量为 50
        if (limit > 50) limit = 50;
        if (limit < 1) limit = 10;

        var result = await _productQueryService.GetHotProductsAsync(limit);
        return Success(result);
    }

    /// <summary>
    /// 获取新品推荐
    /// </summary>
    /// <param name="limit">返回数量限制，默认 10 个，最大 50 个</param>
    /// <returns>新品推荐列表，按创建时间倒序排列</returns>
    /// <remarks>
    /// 返回最近创建的商品列表
    /// 用于首页推荐、新品上市等场景
    /// </remarks>
    [HttpGet("new")]
    [EnableRateLimiting("AppPolicy")]
    public async Task<ApiResponse<List<AppProductListDto>>> GetNew([FromQuery] int limit = 10)
    {
        // 限制最大返回数量为 50
        if (limit > 50) limit = 50;
        if (limit < 1) limit = 10;

        var result = await _productQueryService.GetNewProductsAsync(limit);
        return Success(result);
    }
}