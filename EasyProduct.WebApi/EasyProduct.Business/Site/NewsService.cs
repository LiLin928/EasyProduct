using EasyProduct.Common.Base;
using EasyProduct.Common.Error;
using EasyProduct.Models.Dto.Site.News;
using EasyProduct.Models.Entitys.Site;
using Mapster;
using Microsoft.Extensions.Logging;
using SqlSugar;

namespace EasyProduct.Business.Site;

/// <summary>
/// 新闻服务实现（管理端）
/// </summary>
/// <remarks>
/// 提供新闻的增删改查、唯一性校验等功能，供管理端使用
/// </remarks>
public class NewsService : BaseService, INewsService
{
    private readonly ILogger<NewsService> _logger;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="logger">日志服务</param>
    public NewsService(ILogger<NewsService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// 获取新闻列表（支持分页、筛选）
    /// </summary>
    /// <param name="query">查询参数，包含分页、关键词、分类、状态、置顶、发布时间筛选</param>
    /// <returns>新闻列表</returns>
    public async Task<List<NewsDto>> GetListAsync(NewsQueryDto query)
    {
        var queryable = _db.Queryable<site_news>()
            .LeftJoin<site_news_category>((n, c) => n.CategoryId == c.Id.ToString())
            .Where((n, c) => n.IsDeleted == 0);

        // 按关键词模糊搜索（标题和摘要）
        if (!string.IsNullOrEmpty(query.Keyword))
        {
            queryable = queryable.Where((n, c) => n.Title.Contains(query.Keyword) || n.Summary.Contains(query.Keyword));
        }

        // 按分类ID筛选
        if (!string.IsNullOrEmpty(query.CategoryId))
        {
            queryable = queryable.Where((n, c) => n.CategoryId == query.CategoryId);
        }

        // 按状态筛选
        if (query.Status.HasValue)
        {
            queryable = queryable.Where((n, c) => n.Status == query.Status.Value);
        }

        // 按置顶筛选
        if (query.IsTop.HasValue)
        {
            queryable = queryable.Where((n, c) => n.IsTop == query.IsTop.Value);
        }

        // 按发布时间范围筛选
        if (query.PublishTimeStart.HasValue)
        {
            queryable = queryable.Where((n, c) => n.PublishTime >= query.PublishTimeStart.Value);
        }

        if (query.PublishTimeEnd.HasValue)
        {
            queryable = queryable.Where((n, c) => n.PublishTime <= query.PublishTimeEnd.Value);
        }

        // 排序：置顶优先，然后按排序号，最后按发布时间倒序
        var list = await queryable
            .OrderBy((n, c) => n.IsTop, OrderByType.Desc)
            .OrderBy((n, c) => n.Sort)
            .OrderBy((n, c) => n.PublishTime, OrderByType.Desc)
            .Select((n, c) => new NewsDto
            {
                Id = n.Id,
                CategoryId = n.CategoryId,
                CategoryName = c.CategoryName,
                Title = n.Title,
                Summary = n.Summary,
                Content = n.Content,
                CoverImage = n.CoverImage,
                Author = n.Author,
                Source = n.Source,
                ViewCount = n.ViewCount,
                PublishTime = n.PublishTime,
                IsTop = n.IsTop,
                Sort = n.Sort,
                Status = n.Status,
                CreatedAt = n.CreatedAt,
                UpdatedAt = n.UpdatedAt,
                CreatedBy = n.CreatedBy
            })
            .Skip((query.PageIndex - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();

        return list;
    }

    /// <summary>
    /// 获取新闻详情
    /// </summary>
    /// <param name="id">新闻ID</param>
    /// <returns>新闻详情</returns>
    public async Task<NewsDto> GetByIdAsync(string id)
    {
        var entity = await _db.Queryable<site_news>()
            .LeftJoin<site_news_category>((n, c) => n.CategoryId == c.Id.ToString())
            .Where((n, c) => n.Id.ToString() == id && n.IsDeleted == 0)
            .Select((n, c) => new NewsDto
            {
                Id = n.Id,
                CategoryId = n.CategoryId,
                CategoryName = c.CategoryName,
                Title = n.Title,
                Summary = n.Summary,
                Content = n.Content,
                CoverImage = n.CoverImage,
                Author = n.Author,
                Source = n.Source,
                ViewCount = n.ViewCount,
                PublishTime = n.PublishTime,
                IsTop = n.IsTop,
                Sort = n.Sort,
                Status = n.Status,
                CreatedAt = n.CreatedAt,
                UpdatedAt = n.UpdatedAt,
                CreatedBy = n.CreatedBy
            })
            .FirstAsync();

        if (entity == null)
        {
            throw new BusinessException("新闻不存在", 404);
        }

        return entity;
    }

    /// <summary>
    /// 创建新闻
    /// </summary>
    /// <param name="dto">创建参数</param>
    /// <returns>新新闻ID</returns>
    public async Task<string> CreateAsync(CreateNewsDto dto)
    {
        // 检查分类是否存在
        var categoryExists = await _db.Queryable<site_news_category>()
            .Where(x => x.Id.ToString() == dto.CategoryId && x.IsDeleted == 0)
            .AnyAsync();

        if (!categoryExists)
        {
            throw new BusinessException($"分类ID {dto.CategoryId} 不存在");
        }

        var entity = dto.Adapt<site_news>();
        entity.Id = Guid.NewGuid();
        entity.Status = Models.Enums.Status.Enabled;
        entity.CreatedAt = DateTime.UtcNow;

        // 如果没有设置发布时间，则默认为当前时间
        if (!entity.PublishTime.HasValue)
        {
            entity.PublishTime = DateTime.UtcNow;
        }

        await _db.Insertable(entity).ExecuteCommandAsync();

        _logger.LogInformation("创建新闻成功：{Title}, ID: {Id}", entity.Title, entity.Id);

        return entity.Id.ToString();
    }

    /// <summary>
    /// 更新新闻
    /// </summary>
    /// <param name="dto">更新参数</param>
    /// <returns>是否成功</returns>
    public async Task<bool> UpdateAsync(UpdateNewsDto dto)
    {
        var entity = await _db.Queryable<site_news>()
            .Where(x => x.Id.ToString() == dto.Id && x.IsDeleted == 0)
            .FirstAsync();

        if (entity == null)
        {
            throw new BusinessException("新闻不存在", 404);
        }

        // 检查分类是否存在（如果提供了分类ID）
        if (!string.IsNullOrEmpty(dto.CategoryId))
        {
            var categoryExists = await _db.Queryable<site_news_category>()
                .Where(x => x.Id.ToString() == dto.CategoryId && x.IsDeleted == 0)
                .AnyAsync();

            if (!categoryExists)
            {
                throw new BusinessException($"分类ID {dto.CategoryId} 不存在");
            }

            entity.CategoryId = dto.CategoryId;
        }

        // 更新字段（只更新非空字段）
        if (!string.IsNullOrEmpty(dto.Title)) entity.Title = dto.Title;
        if (dto.Summary != null) entity.Summary = dto.Summary;
        if (!string.IsNullOrEmpty(dto.Content)) entity.Content = dto.Content;
        if (dto.CoverImage != null) entity.CoverImage = dto.CoverImage;
        if (dto.Author != null) entity.Author = dto.Author;
        if (dto.Source != null) entity.Source = dto.Source;
        if (dto.PublishTime.HasValue) entity.PublishTime = dto.PublishTime;
        if (dto.IsTop.HasValue) entity.IsTop = dto.IsTop.Value;
        if (dto.Sort.HasValue) entity.Sort = dto.Sort.Value;
        if (dto.Status.HasValue) entity.Status = (Models.Enums.Status)dto.Status.Value;

        entity.UpdatedAt = DateTime.UtcNow;

        await _db.Updateable(entity).ExecuteCommandAsync();

        _logger.LogInformation("更新新闻成功：{Title}, ID: {Id}", entity.Title, entity.Id);

        return true;
    }

    /// <summary>
    /// 删除新闻
    /// </summary>
    /// <param name="id">新闻ID</param>
    /// <returns>是否成功</returns>
    public async Task<bool> DeleteAsync(string id)
    {
        var entity = await _db.Queryable<site_news>()
            .Where(x => x.Id.ToString() == id && x.IsDeleted == 0)
            .FirstAsync();

        if (entity == null)
        {
            throw new BusinessException("新闻不存在", 404);
        }

        // 软删除
        entity.IsDeleted = 1;
        entity.UpdatedAt = DateTime.UtcNow;

        await _db.Updateable(entity).ExecuteCommandAsync();

        _logger.LogInformation("删除新闻成功：{Title}, ID: {Id}", entity.Title, entity.Id);

        return true;
    }

    /// <summary>
    /// 更新新闻状态
    /// </summary>
    /// <param name="id">新闻ID</param>
    /// <param name="status">状态：0=禁用，1=启用</param>
    /// <returns>是否成功</returns>
    public async Task<bool> UpdateStatusAsync(string id, int status)
    {
        var entity = await _db.Queryable<site_news>()
            .Where(x => x.Id.ToString() == id && x.IsDeleted == 0)
            .FirstAsync();

        if (entity == null)
        {
            throw new BusinessException("新闻不存在", 404);
        }

        entity.Status = (Models.Enums.Status)status;
        entity.UpdatedAt = DateTime.UtcNow;

        await _db.Updateable(entity).ExecuteCommandAsync();

        _logger.LogInformation("更新新闻状态成功：{Title}, Status: {Status}, ID: {Id}", entity.Title, status, entity.Id);

        return true;
    }

    /// <summary>
    /// 设置新闻置顶
    /// </summary>
    /// <param name="id">新闻ID</param>
    /// <param name="isTop">是否置顶：0=不置顶，1=置顶</param>
    /// <returns>是否成功</returns>
    public async Task<bool> SetTopAsync(string id, int isTop)
    {
        var entity = await _db.Queryable<site_news>()
            .Where(x => x.Id.ToString() == id && x.IsDeleted == 0)
            .FirstAsync();

        if (entity == null)
        {
            throw new BusinessException("新闻不存在", 404);
        }

        entity.IsTop = isTop;
        entity.UpdatedAt = DateTime.UtcNow;

        await _db.Updateable(entity).ExecuteCommandAsync();

        _logger.LogInformation("设置新闻置顶成功：{Title}, IsTop: {IsTop}, ID: {Id}", entity.Title, isTop, entity.Id);

        return true;
    }

    /// <summary>
    /// 增加新闻浏览量
    /// </summary>
    /// <param name="id">新闻ID</param>
    /// <returns>是否成功</returns>
    public async Task<bool> IncrementViewCountAsync(string id)
    {
        var entity = await _db.Queryable<site_news>()
            .Where(x => x.Id.ToString() == id && x.IsDeleted == 0)
            .FirstAsync();

        if (entity == null)
        {
            throw new BusinessException("新闻不存在", 404);
        }

        entity.ViewCount += 1;

        await _db.Updateable(entity).UpdateColumns(x => x.ViewCount).ExecuteCommandAsync();

        _logger.LogDebug("新闻浏览量+1：{Title}, ViewCount: {ViewCount}, ID: {Id}", entity.Title, entity.ViewCount, entity.Id);

        return true;
    }
}