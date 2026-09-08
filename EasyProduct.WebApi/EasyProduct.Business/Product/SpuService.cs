using EasyProduct.Common.Base;
using EasyProduct.Common.Error;
using EasyProduct.Models.Dto.Product.Spu;
using EasyProduct.Models.Entitys.Product;
using EasyProduct.Models.Enums;
using Mapster;
using Microsoft.Extensions.Logging;
using SqlSugar;

namespace EasyProduct.Business.Product;

/// <summary>
/// 商品主档服务实现
/// </summary>
/// <remarks>
/// 提供商品主档（SPU）的增删改查、分页查询、状态更新等功能
/// 继承 BaseService，使用属性注入获取数据库上下文
/// </remarks>
public class SpuService : BaseService, ISpuService
{
    /// <summary>
    /// 日志记录器
    /// </summary>
    private readonly ILogger<SpuService> _logger;

    /// <summary>
    /// 构造函数，通过依赖注入获取日志记录器
    /// </summary>
    /// <param name="logger">日志记录器</param>
    public SpuService(ILogger<SpuService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// 获取商品主档分页列表
    /// </summary>
    /// <param name="query">查询参数，支持商品名称、商品编码、分类、类型、状态筛选</param>
    /// <returns>商品主档分页列表</returns>
    /// <remarks>
    /// 1. 支持按商品名称模糊搜索
    /// 2. 支持按商品编码模糊搜索
    /// 3. 支持按分类精确匹配
    /// 4. 支持按商品类型筛选
    /// 5. 支持按状态筛选
    /// 6. 支持分页查询
    /// 7. 按排序字段排序，然后按创建时间倒序
    /// 8. 关联分类表获取分类名称
    /// </remarks>
    public async Task<PageResponse<SpuDto>> GetSpuPageListAsync(SpuQueryDto query)
    {
        // 1. 构建查询条件
        var whereExpr = Expressionable.Create<product_spu>()
            .AndIF(!string.IsNullOrEmpty(query.SpuName), s => s.SpuName.Contains(query.SpuName!))
            .AndIF(!string.IsNullOrEmpty(query.SpuCode), s => s.SpuCode!.Contains(query.SpuCode!))
            .AndIF(!string.IsNullOrEmpty(query.CategoryId), s => s.CategoryId == query.CategoryId)
            .AndIF(query.SpuType.HasValue, s => s.SpuType == query.SpuType!.Value)
            .AndIF(query.Status.HasValue, s => s.Status == query.Status!.Value)
            .And(s => s.IsDeleted == 0)
            .ToExpression();

        // 2. 分页查询，关联分类表获取分类名称
        var queryable = _db.Queryable<product_spu, product_category>((s, c) => new JoinQueryInfos(
                JoinType.Left, s.CategoryId == c.Id.ToString() && c.IsDeleted == 0
            ))
            .Where((s, c) => s.IsDeleted == 0)
            .WhereIF(!string.IsNullOrEmpty(query.SpuName), (s, c) => s.SpuName.Contains(query.SpuName!))
            .WhereIF(!string.IsNullOrEmpty(query.SpuCode), (s, c) => s.SpuCode!.Contains(query.SpuCode!))
            .WhereIF(!string.IsNullOrEmpty(query.CategoryId), (s, c) => s.CategoryId == query.CategoryId)
            .WhereIF(query.SpuType.HasValue, (s, c) => s.SpuType == query.SpuType!.Value)
            .WhereIF(query.Status.HasValue, (s, c) => s.Status == query.Status!.Value)
            .OrderBy((s, c) => s.Sort)
            .OrderByDescending((s, c) => s.CreatedAt)
            .Select((s, c) => new SpuDto
            {
                Id = s.Id,
                SpuName = s.SpuName,
                SpuCode = s.SpuCode,
                CategoryId = s.CategoryId,
                CategoryName = c.CategoryName,
                MainImage = s.MainImage,
                Images = s.Images,
                Description = s.Description,
                Unit = s.Unit,
                SpuType = s.SpuType,
                Brand = s.Brand,
                SpecTemplate = s.SpecTemplate,
                Sort = s.Sort,
                Status = s.Status,
                CreatedAt = s.CreatedAt,
                UpdatedAt = s.UpdatedAt,
                CreatedBy = s.CreatedBy,
                UpdatedBy = s.UpdatedBy
            });

        RefAsync<int> total = 0;
        var spuList = await queryable
            .ToPageListAsync(query.PageIndex, query.PageSize, total);

        // 3. 返回分页结果
        return PageResponse<SpuDto>.Create(spuList, total.Value, query.PageIndex, query.PageSize);
    }

    /// <summary>
    /// 获取商品主档详情
    /// </summary>
    /// <param name="id">商品ID</param>
    /// <returns>商品主档详情</returns>
    /// <exception cref="BusinessException">商品不存在时抛出</exception>
    /// <remarks>
    /// 根据商品ID查询商品详细信息，包含分类名称
    /// </remarks>
    public async Task<SpuDto> GetSpuByIdAsync(string id)
    {
        // 1. 查询商品，关联分类表获取分类名称
        var spu = await _db.Queryable<product_spu, product_category>((s, c) => new JoinQueryInfos(
                JoinType.Left, s.CategoryId == c.Id.ToString() && c.IsDeleted == 0
            ))
            .Where((s, c) => s.Id.ToString() == id && s.IsDeleted == 0)
            .Select((s, c) => new SpuDto
            {
                Id = s.Id,
                SpuName = s.SpuName,
                SpuCode = s.SpuCode,
                CategoryId = s.CategoryId,
                CategoryName = c.CategoryName,
                MainImage = s.MainImage,
                Images = s.Images,
                Description = s.Description,
                Unit = s.Unit,
                SpuType = s.SpuType,
                Brand = s.Brand,
                SpecTemplate = s.SpecTemplate,
                Sort = s.Sort,
                Status = s.Status,
                CreatedAt = s.CreatedAt,
                UpdatedAt = s.UpdatedAt,
                CreatedBy = s.CreatedBy,
                UpdatedBy = s.UpdatedBy
            })
            .FirstAsync();

        // 2. 检查商品是否存在
        if (spu == null)
        {
            throw new BusinessException("商品不存在", 404);
        }

        // 3. 返回商品详情
        return spu;
    }

    /// <summary>
    /// 按分类查询商品主档列表
    /// </summary>
    /// <param name="categoryId">分类ID</param>
    /// <returns>商品主档列表</returns>
    /// <remarks>
    /// 查询指定分类下的所有商品（包含子分类的商品需递归查询）
    /// </remarks>
    public async Task<List<SpuDto>> GetSpuListByCategoryAsync(string categoryId)
    {
        // 1. 查询指定分类下的所有商品，关联分类表获取分类名称
        var spuList = await _db.Queryable<product_spu, product_category>((s, c) => new JoinQueryInfos(
                JoinType.Left, s.CategoryId == c.Id.ToString() && c.IsDeleted == 0
            ))
            .Where((s, c) => s.CategoryId == categoryId && s.IsDeleted == 0 && s.Status == Status.Enabled)
            .OrderBy((s, c) => s.Sort)
            .OrderByDescending((s, c) => s.CreatedAt)
            .Select((s, c) => new SpuDto
            {
                Id = s.Id,
                SpuName = s.SpuName,
                SpuCode = s.SpuCode,
                CategoryId = s.CategoryId,
                CategoryName = c.CategoryName,
                MainImage = s.MainImage,
                Images = s.Images,
                Description = s.Description,
                Unit = s.Unit,
                SpuType = s.SpuType,
                Brand = s.Brand,
                SpecTemplate = s.SpecTemplate,
                Sort = s.Sort,
                Status = s.Status,
                CreatedAt = s.CreatedAt,
                UpdatedAt = s.UpdatedAt,
                CreatedBy = s.CreatedBy,
                UpdatedBy = s.UpdatedBy
            })
            .ToListAsync();

        // 2. 返回商品列表
        return spuList;
    }

    /// <summary>
    /// 创建商品主档
    /// </summary>
    /// <param name="dto">创建商品主档参数</param>
    /// <returns>创建成功返回 true</returns>
    /// <exception cref="BusinessException">商品编码已存在时抛出</exception>
    /// <remarks>
    /// 1. 检查商品编码唯一性
    /// 2. 创建商品记录
    /// 3. 默认状态为启用
    /// </remarks>
    public async Task<bool> CreateSpuAsync(CreateSpuDto dto)
    {
        // 1. 检查商品编码是否已存在
        if (!string.IsNullOrEmpty(dto.SpuCode))
        {
            var exists = await _db.Queryable<product_spu>()
                .Where(s => s.SpuCode == dto.SpuCode && s.IsDeleted == 0)
                .AnyAsync();

            if (exists)
            {
                throw new BusinessException("商品编码已存在", 400);
            }
        }

        // 2. 创建商品实体
        var spu = dto.Adapt<product_spu>();
        spu.Id = Guid.NewGuid();
        spu.Status = Status.Enabled;
        spu.IsDeleted = 0;
        spu.CreatedAt = DateTime.UtcNow;

        // 3. 插入数据库
        var result = await _db.Insertable(spu).ExecuteCommandAsync();

        return result > 0;
    }

    /// <summary>
    /// 更新商品主档
    /// </summary>
    /// <param name="dto">更新商品主档参数</param>
    /// <returns>更新成功返回 true</returns>
    /// <exception cref="BusinessException">商品不存在、商品编码已存在时抛出</exception>
    /// <remarks>
    /// 1. 检查商品是否存在
    /// 2. 检查商品编码唯一性（排除自己）
    /// 3. 更新商品记录
    /// </remarks>
    public async Task<bool> UpdateSpuAsync(UpdateSpuDto dto)
    {
        // 1. 查询商品
        var spu = await _db.Queryable<product_spu>()
            .Where(s => s.Id.ToString() == dto.Id && s.IsDeleted == 0)
            .FirstAsync();

        if (spu == null)
        {
            throw new BusinessException("商品不存在", 404);
        }

        // 2. 检查商品编码是否已存在（排除自己）
        if (!string.IsNullOrEmpty(dto.SpuCode))
        {
            var exists = await _db.Queryable<product_spu>()
                .Where(s => s.SpuCode == dto.SpuCode && s.Id.ToString() != dto.Id && s.IsDeleted == 0)
                .AnyAsync();

            if (exists)
            {
                throw new BusinessException("商品编码已存在", 400);
            }
        }

        // 3. 更新字段
        spu.SpuName = dto.SpuName;
        spu.SpuCode = dto.SpuCode;
        spu.CategoryId = dto.CategoryId;
        spu.MainImage = dto.MainImage;
        spu.Images = dto.Images;
        spu.Description = dto.Description;
        spu.Unit = dto.Unit;
        spu.SpuType = dto.SpuType;
        spu.Brand = dto.Brand;
        spu.SpecTemplate = dto.SpecTemplate;
        spu.Sort = dto.Sort;
        spu.Status = dto.Status;
        spu.UpdatedAt = DateTime.UtcNow;

        // 4. 更新数据库
        var result = await _db.Updateable(spu).ExecuteCommandAsync();

        return result > 0;
    }

    /// <summary>
    /// 删除商品主档（软删除）
    /// </summary>
    /// <param name="id">商品ID</param>
    /// <returns>删除成功返回 true</returns>
    /// <exception cref="BusinessException">商品不存在、存在关联SKU时抛出</exception>
    /// <remarks>
    /// 1. 检查商品是否存在
    /// 2. 检查是否存在关联SKU（TODO: 等批次 3 实现后添加）
    /// 3. 软删除商品记录
    /// </remarks>
    public async Task<bool> DeleteSpuAsync(string id)
    {
        // 1. 查询商品
        var spu = await _db.Queryable<product_spu>()
            .Where(s => s.Id.ToString() == id && s.IsDeleted == 0)
            .FirstAsync();

        if (spu == null)
        {
            throw new BusinessException("商品不存在", 404);
        }

        // 2. 检查是否存在关联SKU（TODO: 等批次 3 实现后添加）
        // var hasSkus = await _db.Queryable<product_sku>()
        //     .Where(sku => sku.SpuId == id && sku.IsDeleted == 0)
        //     .AnyAsync();
        //
        // if (hasSkus)
        // {
        //     throw new BusinessException("该商品下存在SKU，无法删除", 400);
        // }

        // 3. 软删除商品
        spu.IsDeleted = 1;
        spu.UpdatedAt = DateTime.UtcNow;
        spu.Status = Status.Disabled;

        var result = await _db.Updateable(spu).ExecuteCommandAsync();

        return result > 0;
    }

    /// <summary>
    /// 更新商品主档状态
    /// </summary>
    /// <param name="id">商品ID</param>
    /// <param name="status">状态值：0=禁用，1=启用</param>
    /// <returns>更新成功返回 true</returns>
    /// <exception cref="BusinessException">商品不存在时抛出</exception>
    /// <remarks>
    /// 更新商品的启用/禁用状态
    /// </remarks>
    public async Task<bool> UpdateSpuStatusAsync(string id, int status)
    {
        // 1. 查询商品
        var spu = await _db.Queryable<product_spu>()
            .Where(s => s.Id.ToString() == id && s.IsDeleted == 0)
            .FirstAsync();

        if (spu == null)
        {
            throw new BusinessException("商品不存在", 404);
        }

        // 2. 更新状态
        spu.Status = (Status)status;
        spu.UpdatedAt = DateTime.UtcNow;

        // 3. 更新数据库
        var result = await _db.Updateable(spu)
            .UpdateColumns(s => new { s.Status, s.UpdatedAt })
            .ExecuteCommandAsync();

        return result > 0;
    }
}