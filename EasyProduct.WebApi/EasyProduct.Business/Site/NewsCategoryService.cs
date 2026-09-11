using EasyProduct.Common.Base;
using EasyProduct.Common.Error;
using EasyProduct.Models.Dto.Site.NewsCategory;
using EasyProduct.Models.Entitys.Site;
using Mapster;
using Microsoft.Extensions.Logging;
using SqlSugar;

namespace EasyProduct.Business.Site;

/// <summary>
/// 新闻分类服务实现
/// </summary>
/// <remarks>
/// 提供新闻分类的增删改查、唯一性校验等功能
/// </remarks>
public class NewsCategoryService : BaseService, INewsCategoryService
{
    private readonly ILogger<NewsCategoryService> _logger;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="logger">日志服务</param>
    public NewsCategoryService(ILogger<NewsCategoryService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// 获取新闻分类列表（支持分页、筛选）
    /// </summary>
    /// <param name="query">查询参数，包含分页、分类名称、分类编码、状态筛选</param>
    /// <returns>新闻分类列表</returns>
    public async Task<List<NewsCategoryDto>> GetListAsync(NewsCategoryQueryDto query)
    {
        var queryable = _db.Queryable<site_news_category>()
            .Where(x => x.IsDeleted == 0);

        // 按分类名称模糊搜索
        if (!string.IsNullOrEmpty(query.CategoryName))
        {
            queryable = queryable.Where(x => x.CategoryName.Contains(query.CategoryName));
        }

        // 按分类编码精确匹配
        if (!string.IsNullOrEmpty(query.CategoryCode))
        {
            queryable = queryable.Where(x => x.CategoryCode == query.CategoryCode);
        }

        // 按状态筛选
        if (query.Status.HasValue)
        {
            queryable = queryable.Where(x => x.Status == query.Status.Value);
        }

        // 排序：先按排序号，再按创建时间倒序
        var list = await queryable
            .OrderBy(x => x.Sort)
            .OrderBy(x => x.CreatedAt, OrderByType.Desc)
            .Skip((query.PageIndex - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();

        return list.Adapt<List<NewsCategoryDto>>();
    }

    /// <summary>
    /// 获取新闻分类详情
    /// </summary>
    /// <param name="id">分类ID</param>
    /// <returns>新闻分类详情</returns>
    public async Task<NewsCategoryDto> GetByIdAsync(string id)
    {
        var entity = await _db.Queryable<site_news_category>()
            .Where(x => x.Id.ToString() == id && x.IsDeleted == 0)
            .FirstAsync();

        if (entity == null)
        {
            throw new BusinessException("新闻分类不存在", 404);
        }

        return entity.Adapt<NewsCategoryDto>();
    }

    /// <summary>
    /// 创建新闻分类
    /// </summary>
    /// <param name="dto">创建参数</param>
    /// <returns>新分类ID</returns>
    public async Task<string> CreateAsync(CreateNewsCategoryDto dto)
    {
        // 检查分类名称是否已存在
        if (await IsNameExistsAsync(dto.CategoryName))
        {
            throw new BusinessException($"分类名称 {dto.CategoryName} 已存在");
        }

        // 检查分类编码是否已存在（如果提供了分类编码）
        if (!string.IsNullOrEmpty(dto.CategoryCode) && await IsCodeExistsAsync(dto.CategoryCode))
        {
            throw new BusinessException($"分类编码 {dto.CategoryCode} 已存在");
        }

        var entity = dto.Adapt<site_news_category>();
        entity.Id = Guid.NewGuid();
        entity.Status = Models.Enums.Status.Enabled;
        entity.CreatedAt = DateTime.UtcNow;

        await _db.Insertable(entity).ExecuteCommandAsync();

        _logger.LogInformation("创建新闻分类成功：{CategoryName}, ID: {Id}", entity.CategoryName, entity.Id);

        return entity.Id.ToString();
    }

    /// <summary>
    /// 更新新闻分类
    /// </summary>
    /// <param name="dto">更新参数</param>
    /// <returns>是否成功</returns>
    public async Task<bool> UpdateAsync(UpdateNewsCategoryDto dto)
    {
        var entity = await _db.Queryable<site_news_category>()
            .Where(x => x.Id.ToString() == dto.Id && x.IsDeleted == 0)
            .FirstAsync();

        if (entity == null)
        {
            throw new BusinessException("新闻分类不存在", 404);
        }

        // 检查分类名称是否已存在（排除自己）
        if (await IsNameExistsAsync(dto.CategoryName, dto.Id))
        {
            throw new BusinessException($"分类名称 {dto.CategoryName} 已存在");
        }

        // 检查分类编码是否已存在（如果提供了分类编码，排除自己）
        if (!string.IsNullOrEmpty(dto.CategoryCode) && await IsCodeExistsAsync(dto.CategoryCode, dto.Id))
        {
            throw new BusinessException($"分类编码 {dto.CategoryCode} 已存在");
        }

        entity.CategoryName = dto.CategoryName;
        entity.CategoryCode = dto.CategoryCode;
        entity.Sort = dto.Sort;
        entity.Status = (Models.Enums.Status)dto.Status;
        entity.UpdatedAt = DateTime.UtcNow;

        await _db.Updateable(entity).ExecuteCommandAsync();

        _logger.LogInformation("更新新闻分类成功：{CategoryName}, ID: {Id}", entity.CategoryName, entity.Id);

        return true;
    }

    /// <summary>
    /// 删除新闻分类
    /// </summary>
    /// <param name="id">分类ID</param>
    /// <returns>是否成功</returns>
    public async Task<bool> DeleteAsync(string id)
    {
        var entity = await _db.Queryable<site_news_category>()
            .Where(x => x.Id.ToString() == id && x.IsDeleted == 0)
            .FirstAsync();

        if (entity == null)
        {
            throw new BusinessException("新闻分类不存在", 404);
        }

        // 检查该分类下是否有新闻
        var hasNews = await _db.Queryable<site_news>()
            .Where(x => x.CategoryId == id && x.IsDeleted == 0)
            .AnyAsync();

        if (hasNews)
        {
            throw new BusinessException("该分类下存在新闻，不能删除");
        }

        // 软删除
        entity.IsDeleted = 1;
        entity.UpdatedAt = DateTime.UtcNow;

        await _db.Updateable(entity).ExecuteCommandAsync();

        _logger.LogInformation("删除新闻分类成功：{CategoryName}, ID: {Id}", entity.CategoryName, entity.Id);

        return true;
    }

    /// <summary>
    /// 检查分类名称是否已存在
    /// </summary>
    /// <param name="categoryName">分类名称</param>
    /// <param name="excludeId">排除的分类ID（用于更新时排除自己）</param>
    /// <returns>是否已存在</returns>
    public async Task<bool> IsNameExistsAsync(string categoryName, string? excludeId = null)
    {
        var queryable = _db.Queryable<site_news_category>()
            .Where(x => x.CategoryName == categoryName && x.IsDeleted == 0);

        if (!string.IsNullOrEmpty(excludeId))
        {
            queryable = queryable.Where(x => x.Id.ToString() != excludeId);
        }

        return await queryable.AnyAsync();
    }

    /// <summary>
    /// 检查分类编码是否已存在
    /// </summary>
    /// <param name="categoryCode">分类编码</param>
    /// <param name="excludeId">排除的分类ID（用于更新时排除自己）</param>
    /// <returns>是否已存在</returns>
    public async Task<bool> IsCodeExistsAsync(string categoryCode, string? excludeId = null)
    {
        var queryable = _db.Queryable<site_news_category>()
            .Where(x => x.CategoryCode == categoryCode && x.IsDeleted == 0);

        if (!string.IsNullOrEmpty(excludeId))
        {
            queryable = queryable.Where(x => x.Id.ToString() != excludeId);
        }

        return await queryable.AnyAsync();
    }

    /// <summary>
    /// 获取所有启用的新闻分类（用于下拉选择）
    /// </summary>
    /// <returns>新闻分类列表</returns>
    public async Task<List<NewsCategoryDto>> GetEnabledListAsync()
    {
        var list = await _db.Queryable<site_news_category>()
            .Where(x => x.IsDeleted == 0 && x.Status == Models.Enums.Status.Enabled)
            .OrderBy(x => x.Sort)
            .ToListAsync();

        return list.Adapt<List<NewsCategoryDto>>();
    }
}