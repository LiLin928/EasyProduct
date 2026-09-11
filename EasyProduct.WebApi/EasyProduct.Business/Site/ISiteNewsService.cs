using EasyProduct.Models.Dto.Site.News;

namespace EasyProduct.Business.Site;

/// <summary>
/// 新闻服务接口（官网公开）
/// </summary>
/// <remarks>
/// 提供新闻的公开查询功能，供官网使用
/// 只返回启用状态的新闻
/// </remarks>
public interface ISiteNewsService
{
    /// <summary>
    /// 获取新闻列表（官网公开，只返回启用的新闻）
    /// </summary>
    /// <param name="query">查询参数，包含分页、分类筛选</param>
    /// <returns>新闻列表（不包含内容字段）</returns>
    Task<List<NewsListDto>> GetListAsync(NewsQueryDto query);

    /// <summary>
    /// 获取新闻详情（官网公开，只返回启用的新闻）
    /// </summary>
    /// <param name="id">新闻ID</param>
    /// <returns>新闻详情</returns>
    Task<NewsDto> GetByIdAsync(string id);

    /// <summary>
    /// 获取置顶新闻列表（官网公开，只返回启用的置顶新闻）
    /// </summary>
    /// <param name="count">返回数量，默认5条</param>
    /// <returns>置顶新闻列表</returns>
    Task<List<NewsListDto>> GetTopListAsync(int count = 5);

    /// <summary>
    /// 按分类获取新闻列表（官网公开，只返回启用的新闻）
    /// </summary>
    /// <param name="categoryId">分类ID</param>
    /// <param name="count">返回数量，默认10条</param>
    /// <returns>新闻列表</returns>
    Task<List<NewsListDto>> GetByCategoryAsync(string categoryId, int count = 10);
}

/// <summary>
/// 新闻列表 DTO（官网公开，不包含内容字段）
/// </summary>
/// <remarks>
/// 用于官网新闻列表展示，不包含内容字段以减少数据传输
/// </remarks>
public class NewsListDto
{
    /// <summary>
    /// 新闻ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 分类ID
    /// </summary>
    public string CategoryId { get; set; } = string.Empty;

    /// <summary>
    /// 分类名称
    /// </summary>
    public string? CategoryName { get; set; }

    /// <summary>
    /// 新闻标题
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 新闻摘要
    /// </summary>
    public string? Summary { get; set; }

    /// <summary>
    /// 封面图片
    /// </summary>
    public string? CoverImage { get; set; }

    /// <summary>
    /// 作者
    /// </summary>
    public string? Author { get; set; }

    /// <summary>
    /// 来源
    /// </summary>
    public string? Source { get; set; }

    /// <summary>
    /// 浏览量
    /// </summary>
    public int ViewCount { get; set; }

    /// <summary>
    /// 发布时间
    /// </summary>
    public DateTime? PublishTime { get; set; }

    /// <summary>
    /// 是否置顶
    /// </summary>
    public int IsTop { get; set; }
}