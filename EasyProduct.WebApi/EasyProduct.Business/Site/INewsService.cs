using EasyProduct.Models.Dto.Site.News;

namespace EasyProduct.Business.Site;

/// <summary>
/// 新闻服务接口（管理端）
/// </summary>
/// <remarks>
/// 提供新闻的增删改查、唯一性校验等功能，供管理端使用
/// </remarks>
public interface INewsService
{
    /// <summary>
    /// 获取新闻列表（支持分页、筛选）
    /// </summary>
    /// <param name="query">查询参数，包含分页、关键词、分类、状态、置顶、发布时间筛选</param>
    /// <returns>新闻列表</returns>
    Task<List<NewsDto>> GetListAsync(NewsQueryDto query);

    /// <summary>
    /// 获取新闻详情
    /// </summary>
    /// <param name="id">新闻ID</param>
    /// <returns>新闻详情</returns>
    Task<NewsDto> GetByIdAsync(string id);

    /// <summary>
    /// 创建新闻
    /// </summary>
    /// <param name="dto">创建参数</param>
    /// <returns>新新闻ID</returns>
    Task<string> CreateAsync(CreateNewsDto dto);

    /// <summary>
    /// 更新新闻
    /// </summary>
    /// <param name="dto">更新参数</param>
    /// <returns>是否成功</returns>
    Task<bool> UpdateAsync(UpdateNewsDto dto);

    /// <summary>
    /// 删除新闻
    /// </summary>
    /// <param name="id">新闻ID</param>
    /// <returns>是否成功</returns>
    Task<bool> DeleteAsync(string id);

    /// <summary>
    /// 更新新闻状态
    /// </summary>
    /// <param name="id">新闻ID</param>
    /// <param name="status">状态：0=禁用，1=启用</param>
    /// <returns>是否成功</returns>
    Task<bool> UpdateStatusAsync(string id, int status);

    /// <summary>
    /// 设置新闻置顶
    /// </summary>
    /// <param name="id">新闻ID</param>
    /// <param name="isTop">是否置顶：0=不置顶，1=置顶</param>
    /// <returns>是否成功</returns>
    Task<bool> SetTopAsync(string id, int isTop);

    /// <summary>
    /// 增加新闻浏览量
    /// </summary>
    /// <param name="id">新闻ID</param>
    /// <returns>是否成功</returns>
    Task<bool> IncrementViewCountAsync(string id);
}