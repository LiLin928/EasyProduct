using EasyProduct.Common.Base;
using EasyProduct.Common.Error;
using EasyProduct.Models.Dto.Product.App;
using EasyProduct.Models.Entitys.Product;
using EasyProduct.Models.Enums;
using Microsoft.Extensions.Logging;
using SqlSugar;
using System.Text.Json;

// 明确使用 App 命名空间的 CategoryTreeDto，避免与 Category 命名空间冲突
using AppCategoryTreeDto = EasyProduct.Models.Dto.Product.App.CategoryTreeDto;

namespace EasyProduct.Business.Product;

/// <summary>
/// 商品查询服务实现（会员端）
/// </summary>
/// <remarks>
/// 封装会员端商品查询逻辑，复用现有的 ISpuService、ICategoryService、ISkuService
/// 提供商品列表、详情、分类、搜索、热销、新品推荐等功能
/// </remarks>
public class ProductQueryService : BaseService, IProductQueryService
{
    /// <summary>
    /// 日志记录器
    /// </summary>
    private readonly ILogger<ProductQueryService> _logger;

    /// <summary>
    /// 构造函数，通过依赖注入获取日志记录器
    /// </summary>
    /// <param name="logger">日志记录器</param>
    public ProductQueryService(ILogger<ProductQueryService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// 查询商品列表（会员端）
    /// </summary>
    /// <param name="query">查询参数，包含分类、关键词、价格区间、排序等</param>
    /// <returns>商品列表分页结果</returns>
    /// <remarks>
    /// 1. 支持按分类、关键词、价格区间、品牌筛选
    /// 2. 支持按价格、销量、创建时间排序
    /// 3. 支持分页查询
    /// 4. 关联查询分类名称
    /// 5. 关联查询 SKU 获取价格范围
    /// 6. 仅返回启用状态的商品
    /// </remarks>
    public async Task<PageResponse<AppProductListDto>> GetProductListAsync(AppProductQueryDto query)
    {
        // 1. 构建查询条件 - 查询 SPU 和 Category 的关联数据
        var queryable = _db.Queryable<product_spu, product_category>((s, c) => new JoinQueryInfos(
                JoinType.Left, s.CategoryId == c.Id.ToString() && c.IsDeleted == 0
            ))
            .Where((s, c) => s.IsDeleted == 0 && s.Status == Status.Enabled);

        // 2. 应用筛选条件
        // 分类筛选
        if (!string.IsNullOrEmpty(query.CategoryId))
        {
            // TODO: 支持查询子分类的商品（需要递归查询子分类 ID 列表）
            queryable = queryable.Where((s, c) => s.CategoryId == query.CategoryId);
        }

        // 关键词搜索
        if (!string.IsNullOrEmpty(query.Keyword))
        {
            queryable = queryable.Where((s, c) =>
                s.SpuName.Contains(query.Keyword) ||
                (s.SpuCode != null && s.SpuCode.Contains(query.Keyword)));
        }

        // 品牌筛选
        if (!string.IsNullOrEmpty(query.Brand))
        {
            queryable = queryable.Where((s, c) => s.Brand == query.Brand);
        }

        // 3. 应用排序
        queryable = ApplySorting(queryable, query.SortBy, query.SortOrder);

        // 4. 分页查询
        RefAsync<int> total = 0;
        var spuList = await queryable
            .Select((s, c) => new
            {
                s.Id,
                s.SpuName,
                s.MainImage,
                s.CategoryId,
                CategoryName = c.CategoryName,
                s.CreatedAt
            })
            .ToPageListAsync(query.PageIndex, query.PageSize, total);

        // 5. 查询每个商品的 SKU 价格范围和销量
        var result = new List<AppProductListDto>();
        foreach (var spu in spuList)
        {
            // 查询该商品的 SKU 列表
            var skus = await _db.Queryable<product_sku>()
                .Where(sku => sku.SpuId == spu.Id.ToString() && sku.IsDeleted == 0 && sku.Status == Status.Enabled)
                .ToListAsync();

            // 计算价格范围
            var minPrice = skus.Any() ? skus.Min(s => s.Price) : 0;
            var maxPrice = skus.Any() ? skus.Max(s => s.Price) : (decimal?)null;
            var memberPrice = skus.Where(s => s.MemberPrice.HasValue).Min(s => s.MemberPrice);

            // TODO: 计算销量（需要关联订单数据）
            var salesCount = 0;

            // 价格区间筛选（在内存中过滤，因为价格在 SKU 表）
            if (query.MinPrice.HasValue && minPrice < query.MinPrice.Value)
            {
                continue;
            }
            if (query.MaxPrice.HasValue && minPrice > query.MaxPrice.Value)
            {
                continue;
            }

            result.Add(new AppProductListDto
            {
                Id = spu.Id,
                Name = spu.SpuName,
                MainImage = spu.MainImage,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                MemberPrice = memberPrice,
                SalesCount = salesCount,
                CategoryName = spu.CategoryName
            });
        }

        // 6. 返回分页结果
        return PageResponse<AppProductListDto>.Create(result, total.Value, query.PageIndex, query.PageSize);
    }

    /// <summary>
    /// 获取商品详情（会员端）
    /// </summary>
    /// <param name="productId">商品 ID（GUID 格式）</param>
    /// <returns>商品详情，包含基本信息、SKU 列表、规格模板等</returns>
    /// <exception cref="BusinessException">商品不存在时抛出</exception>
    /// <remarks>
    /// 1. 查询商品基本信息
    /// 2. 关联查询分类信息
    /// 3. 查询 SKU 列表
    /// 4. 构建规格模板
    /// 5. 统计销量信息
    /// 6. 仅返回启用状态的商品
    /// </remarks>
    public async Task<AppProductDetailDto> GetProductDetailAsync(string productId)
    {
        // 1. 查询商品基本信息，关联分类表
        var spu = await _db.Queryable<product_spu, product_category>((s, c) => new JoinQueryInfos(
                JoinType.Left, s.CategoryId == c.Id.ToString() && c.IsDeleted == 0
            ))
            .Where((s, c) => s.Id.ToString() == productId && s.IsDeleted == 0 && s.Status == Status.Enabled)
            .Select((s, c) => new
            {
                s.Id,
                s.SpuName,
                s.SpuCode,
                s.MainImage,
                s.Images,
                s.Description,
                s.CategoryId,
                CategoryName = c.CategoryName,
                s.Brand,
                s.Unit,
                s.SpecTemplate
            })
            .FirstAsync();

        // 2. 检查商品是否存在
        if (spu == null)
        {
            throw new BusinessException("商品不存在", 404);
        }

        // 3. 查询 SKU 列表
        var skuList = await _db.Queryable<product_sku>()
            .Where(sku => sku.SpuId == productId && sku.IsDeleted == 0 && sku.Status == Status.Enabled)
            .OrderBy(sku => sku.Price)
            .ToListAsync();

        // 4. 转换 SKU DTO
        var skuDtos = skuList.Select(sku => new AppSkuDto
        {
            Id = sku.Id,
            Name = sku.SkuName,
            Code = sku.SkuCode,
            Barcode = sku.Barcode,
            Price = sku.Price,
            MemberPrice = sku.MemberPrice,
            Stock = sku.Stock,
            Specs = ParseSpecJson(sku.SpecJson)
        }).ToList();

        // 5. 解析图片列表
        var images = ParseImageList(spu.Images);

        // 6. 解析规格模板
        var specTemplate = ParseSpecTemplate(spu.SpecTemplate);

        // 7. TODO: 统计销量（需要关联订单数据）
        var salesStats = new SalesStatisticsDto
        {
            TotalSales = 0,
            MonthSales = 0
        };

        // 8. 构建返回结果
        return new AppProductDetailDto
        {
            Id = spu.Id,
            Name = spu.SpuName,
            Code = spu.SpuCode,
            Description = spu.Description,
            MainImage = spu.MainImage,
            Images = images,
            CategoryId = spu.CategoryId,
            CategoryName = spu.CategoryName,
            Brand = spu.Brand,
            Unit = spu.Unit,
            Skus = skuDtos,
            SpecTemplate = specTemplate,
            SalesStats = salesStats
        };
    }

    /// <summary>
    /// 获取商品分类树
    /// </summary>
    /// <returns>分类树，包含所有分类的层级结构</returns>
    /// <remarks>
    /// 1. 查询所有启用的分类
    /// 2. 构建树形结构
    /// 3. 返回结果（会员端简化版）
    /// </remarks>
    public async Task<List<AppCategoryTreeDto>> GetCategoryTreeAsync()
    {
        // 1. 查询所有启用的分类
        var categories = await _db.Queryable<product_category>()
            .Where(c => c.IsDeleted == 0 && c.Status == Status.Enabled)
            .OrderBy(c => c.Sort)
            .OrderBy(c => c.CreatedAt)
            .ToListAsync();

        // 2. 转换为会员端简化版 DTO
        var categoryDtos = categories.Select(c => new AppCategoryTreeDto
        {
            Id = c.Id,
            Name = c.CategoryName,
            Icon = c.Icon,
            Image = c.Image,
            Children = new List<AppCategoryTreeDto>()
        }).ToList();

        // 3. 构建树形结构
        var tree = BuildCategoryTree(categoryDtos, string.Empty);

        return tree;
    }

    /// <summary>
    /// 搜索商品
    /// </summary>
    /// <param name="keyword">搜索关键词，匹配商品名称、编码</param>
    /// <returns>匹配的商品列表</returns>
    /// <remarks>
    /// 1. 关键词模糊匹配（商品名称、编码）
    /// 2. 返回前 50 条结果
    /// 3. 仅返回启用状态的商品
    /// </remarks>
    public async Task<List<AppProductListDto>> SearchProductsAsync(string keyword)
    {
        if (string.IsNullOrWhiteSpace(keyword))
        {
            return new List<AppProductListDto>();
        }

        // 1. 模糊搜索商品名称和编码
        var spuList = await _db.Queryable<product_spu, product_category>((s, c) => new JoinQueryInfos(
                JoinType.Left, s.CategoryId == c.Id.ToString() && c.IsDeleted == 0
            ))
            .Where((s, c) => s.IsDeleted == 0 && s.Status == Status.Enabled)
            .Where((s, c) => s.SpuName.Contains(keyword) || (s.SpuCode != null && s.SpuCode.Contains(keyword)))
            .OrderByDescending((s, c) => s.CreatedAt)
            .Take(50)
            .Select((s, c) => new
            {
                s.Id,
                s.SpuName,
                s.MainImage,
                CategoryName = c.CategoryName
            })
            .ToListAsync();

        // 2. 查询每个商品的 SKU 价格范围
        var result = new List<AppProductListDto>();
        foreach (var spu in spuList)
        {
            var skus = await _db.Queryable<product_sku>()
                .Where(sku => sku.SpuId == spu.Id.ToString() && sku.IsDeleted == 0 && sku.Status == Status.Enabled)
                .ToListAsync();

            var minPrice = skus.Any() ? skus.Min(s => s.Price) : 0;
            var maxPrice = skus.Any() ? skus.Max(s => s.Price) : (decimal?)null;
            var memberPrice = skus.Where(s => s.MemberPrice.HasValue).Min(s => s.MemberPrice);

            // TODO: 计算销量
            var salesCount = 0;

            result.Add(new AppProductListDto
            {
                Id = spu.Id,
                Name = spu.SpuName,
                MainImage = spu.MainImage,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                MemberPrice = memberPrice,
                SalesCount = salesCount,
                CategoryName = spu.CategoryName
            });
        }

        return result;
    }

    /// <summary>
    /// 获取热销商品
    /// </summary>
    /// <param name="limit">返回数量限制，默认 10 个</param>
    /// <returns>热销商品列表，按销量倒序排列</returns>
    /// <remarks>
    /// TODO: 实现热销商品查询逻辑
    /// 1. 按销量倒序排序（需要订单数据）
    /// 2. 返回前 N 条
    /// 3. 暂时返回最新商品
    /// </remarks>
    public async Task<List<AppProductListDto>> GetHotProductsAsync(int limit = 10)
    {
        // TODO: 按销量倒序排序（需要订单数据）
        // 暂时返回最新商品
        return await GetNewProductsAsync(limit);
    }

    /// <summary>
    /// 获取新品推荐
    /// </summary>
    /// <param name="limit">返回数量限制，默认 10 个</param>
    /// <returns>新品推荐列表，按创建时间倒序排列</returns>
    /// <remarks>
    /// 1. 按创建时间倒序排序
    /// 2. 返回前 N 条
    /// 3. 仅返回启用状态的商品
    /// </remarks>
    public async Task<List<AppProductListDto>> GetNewProductsAsync(int limit = 10)
    {
        // 1. 按创建时间倒序查询商品
        var spuList = await _db.Queryable<product_spu, product_category>((s, c) => new JoinQueryInfos(
                JoinType.Left, s.CategoryId == c.Id.ToString() && c.IsDeleted == 0
            ))
            .Where((s, c) => s.IsDeleted == 0 && s.Status == Status.Enabled)
            .OrderByDescending((s, c) => s.CreatedAt)
            .Take(limit)
            .Select((s, c) => new
            {
                s.Id,
                s.SpuName,
                s.MainImage,
                CategoryName = c.CategoryName
            })
            .ToListAsync();

        // 2. 查询每个商品的 SKU 价格范围
        var result = new List<AppProductListDto>();
        foreach (var spu in spuList)
        {
            var skus = await _db.Queryable<product_sku>()
                .Where(sku => sku.SpuId == spu.Id.ToString() && sku.IsDeleted == 0 && sku.Status == Status.Enabled)
                .ToListAsync();

            var minPrice = skus.Any() ? skus.Min(s => s.Price) : 0;
            var maxPrice = skus.Any() ? skus.Max(s => s.Price) : (decimal?)null;
            var memberPrice = skus.Where(s => s.MemberPrice.HasValue).Min(s => s.MemberPrice);

            // TODO: 计算销量
            var salesCount = 0;

            result.Add(new AppProductListDto
            {
                Id = spu.Id,
                Name = spu.SpuName,
                MainImage = spu.MainImage,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                MemberPrice = memberPrice,
                SalesCount = salesCount,
                CategoryName = spu.CategoryName
            });
        }

        return result;
    }

    #region 私有辅助方法

    /// <summary>
    /// 应用排序规则
    /// </summary>
    /// <param name="queryable">查询对象</param>
    /// <param name="sortBy">排序字段</param>
    /// <param name="sortOrder">排序方式</param>
    /// <returns>排序后的查询对象</returns>
    private ISugarQueryable<product_spu, product_category> ApplySorting(
        ISugarQueryable<product_spu, product_category> queryable,
        string sortBy,
        string sortOrder)
    {
        // 默认按创建时间倒序
        if (string.IsNullOrEmpty(sortBy))
        {
            sortBy = "createTime";
        }

        // 排序方式，默认降序
        var isDesc = sortOrder?.ToLower() == "desc";

        return sortBy.ToLower() switch
        {
            "price" => isDesc
                ? queryable.OrderByDescending((s, c) => s.CreatedAt) // TODO: 按 SKU 价格排序（需要子查询）
                : queryable.OrderBy((s, c) => s.CreatedAt),
            "sales" => isDesc
                ? queryable.OrderByDescending((s, c) => s.CreatedAt) // TODO: 按销量排序（需要订单数据）
                : queryable.OrderBy((s, c) => s.CreatedAt),
            "createtime" => isDesc
                ? queryable.OrderByDescending((s, c) => s.CreatedAt)
                : queryable.OrderBy((s, c) => s.CreatedAt),
            _ => queryable.OrderByDescending((s, c) => s.CreatedAt)
        };
    }

    /// <summary>
    /// 构建分类树形结构
    /// </summary>
    /// <param name="allCategories">所有分类列表</param>
    /// <param name="parentId">父分类 ID</param>
    /// <returns>树形结构的分类列表</returns>
    private List<AppCategoryTreeDto> BuildCategoryTree(List<AppCategoryTreeDto> allCategories, string parentId)
    {
        return allCategories
            .Where(c => GetParentId(c, parentId))
            .Select(c =>
            {
                c.Children = BuildCategoryTree(allCategories, c.Id.ToString());
                return c;
            })
            .ToList();
    }

    /// <summary>
    /// 获取父分类 ID（辅助方法）
    /// </summary>
    /// <remarks>
    /// 由于会员端 DTO 不包含 ParentId 字段，需要从数据库实体中获取
    /// 当前实现简化处理，直接返回所有分类（待优化）
    /// </remarks>
    private bool GetParentId(AppCategoryTreeDto category, string parentId)
    {
        // TODO: 优化分类树构建逻辑，需要从数据库实体中获取 ParentId
        // 当前简化实现：直接返回所有分类
        return true;
    }

    /// <summary>
    /// 解析图片列表 JSON
    /// </summary>
    /// <param name="imagesJson">图片列表 JSON 字符串</param>
    /// <returns>图片 URL 列表</returns>
    private List<string>? ParseImageList(string? imagesJson)
    {
        if (string.IsNullOrWhiteSpace(imagesJson))
        {
            return null;
        }

        try
        {
            return JsonSerializer.Deserialize<List<string>>(imagesJson);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "解析图片列表 JSON 失败: {Json}", imagesJson);
            return null;
        }
    }

    /// <summary>
    /// 解析规格组合 JSON
    /// </summary>
    /// <param name="specJson">规格组合 JSON 字符串</param>
    /// <returns>规格项列表</returns>
    private List<SpecItemDto>? ParseSpecJson(string? specJson)
    {
        if (string.IsNullOrWhiteSpace(specJson))
        {
            return null;
        }

        try
        {
            return JsonSerializer.Deserialize<List<SpecItemDto>>(specJson);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "解析规格组合 JSON 失败: {Json}", specJson);
            return null;
        }
    }

    /// <summary>
    /// 解析规格模板 JSON
    /// </summary>
    /// <param name="specTemplateJson">规格模板 JSON 字符串</param>
    /// <returns>规格模板 DTO</returns>
    private SpecTemplateDto? ParseSpecTemplate(string? specTemplateJson)
    {
        if (string.IsNullOrWhiteSpace(specTemplateJson))
        {
            return null;
        }

        try
        {
            var specs = JsonSerializer.Deserialize<List<SpecDefinitionDto>>(specTemplateJson);
            return new SpecTemplateDto
            {
                Specs = specs ?? new List<SpecDefinitionDto>()
            };
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "解析规格模板 JSON 失败: {Json}", specTemplateJson);
            return null;
        }
    }

    #endregion
}