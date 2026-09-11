using EasyProduct.Common.Base;
using EasyProduct.Common.Error;
using EasyProduct.Models.Dto.Site.News;
using EasyProduct.Models.Entitys.Site;
using Mapster;
using Microsoft.Extensions.Logging;
using SqlSugar;

namespace EasyProduct.Business.Site;

/// <summary>
/// 新闻服务实现（官网公开）
/// </summary>
/// <remarks>
/// 提供新闻的公开查询功能，供官网使用
/// 只返回启用状态的新闻
/// </remarks>
public class SiteNewsService : BaseService, ISiteNewsService
{
    private readonly ILogger<SiteNewsService> _logger;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="logger">日志服务</param>
    public SiteNewsService(ILogger<SiteNewsService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// 获取新闻列表（官网公开，只返回启用的新闻）
    /// </summary>
    /// <param name="query">查询参数，包含分页、分类筛选</param>
    /// <returns>新闻列表（不包含内容字段）</returns>
    public async Task<List<NewsListDto>> GetListAsync(NewsQueryDto query)
    {
        var queryable = _db.Queryable<site_news>()
            .LeftJoin<site_news_category>((n, c) => n.CategoryId == c.Id.ToString())
            .Where((n, c) => n.IsDeleted == 0 && n.Status == Models.Enums.Status.Enabled);

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
            .Select((n, c) => new NewsListDto
            {
                Id = n.Id,
                CategoryId = n.CategoryId,
                CategoryName = c.CategoryName,
                Title = n.Title,
                Summary = n.Summary,
                CoverImage = n.CoverImage,
                Author = n.Author,
                Source = n.Source,
                ViewCount = n.ViewCount,
                PublishTime = n.PublishTime,
                IsTop = n.IsTop
            })
            .Skip((query.PageIndex - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();

        return list;
    }

    /// <summary>
    /// 获取新闻详情（官网公开，只返回启用的新闻）
    /// </summary>
    /// <param name="id">新闻ID</param>
    /// <returns>新闻详情</returns>
    public async Task<NewsDto> GetByIdAsync(string id)
    {
        var entity = await _db.Queryable<site_news>()
            .LeftJoin<site_news_category>((n, c) => n.CategoryId == c.Id.ToString())
            .Where((n, c) => n.Id.ToString() == id && n.IsDeleted == 0 && n.Status == Models.Enums.Status.Enabled)
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
            throw new BusinessException("新闻不存在或未发布", 404);
        }

        // 增加浏览量
        await _db.Updateable<site_news>()
            .SetColumns(x => x.ViewCount == x.ViewCount + 1)
            .Where(x => x.Id.ToString() == id)
            .ExecuteCommandAsync();

        _logger.LogDebug("官网查看新闻，浏览量+1：{Title}, ID: {Id}", entity.Title, entity.Id);

        return entity;
    }

    /// <summary>
    /// 获取置顶新闻列表（官网公开，只返回启用的置顶新闻）
    /// </summary>
    /// <param name="count">返回数量，默认5条</param>
    /// <returns>置顶新闻列表</returns>
    public async Task<List<NewsListDto>> GetTopListAsync(int count = 5)
    {
        var list = await _db.Queryable<site_news>()
            .LeftJoin<site_news_category>((n, c) => n.CategoryId == c.Id.ToString())
            .Where((n, c) => n.IsDeleted == 0 && n.Status == Models.Enums.Status.Enabled && n.IsTop == 1)
            .OrderBy((n, c) => n.Sort)
            .OrderBy((n, c) => n.PublishTime, OrderByType.Desc)
            .Select((n, c) => new NewsListDto
            {
                Id = n.Id,
                CategoryId = n.CategoryId,
                CategoryName = c.CategoryName,
                Title = n.Title,
                Summary = n.Summary,
                CoverImage = n.CoverImage,
                Author = n.Author,
                Source = n.Source,
                ViewCount = n.ViewCount,
                PublishTime = n.PublishTime,
                IsTop = n.IsTop
            })
            .Take(count)
            .ToListAsync();

        return list;
    }

    /// <summary>
    /// 按分类获取新闻列表（官网公开，只返回启用的新闻）
    /// </summary>
    /// <param name="categoryId">分类ID</param>
    /// <param name="count">返回数量，默认10条</param>
    /// <returns>新闻列表</returns>
    public async Task<List<NewsListDto>> GetByCategoryAsync(string categoryId, int count = 10)
    {
        var list = await _db.Queryable<site_news>()
            .LeftJoin<site_news_category>((n, c) => n.CategoryId == c.Id.ToString())
            .Where((n, c) => n.IsDeleted == 0 && n.Status == Models.Enums.Status.Enabled && n.CategoryId == categoryId)
            .OrderBy((n, c) => n.IsTop, OrderByType.Desc)
            .OrderBy((n, c) => n.Sort)
            .OrderBy((n, c) => n.PublishTime, OrderByType.Desc)
            .Select((n, c) => new NewsListDto
            {
                Id = n.Id,
                CategoryId = n.CategoryId,
                CategoryName = c.CategoryName,
                Title = n.Title,
                Summary = n.Summary,
                CoverImage = n.CoverImage,
                Author = n.Author,
                Source = n.Source,
                ViewCount = n.ViewCount,
                PublishTime = n.PublishTime,
                IsTop = n.IsTop
            })
            .Take(count)
            .ToListAsync();

        return list;
    }
}