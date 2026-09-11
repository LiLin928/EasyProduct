using EasyProduct.Models.Dto.Site.NewsCategory;

namespace EasyProduct.Business.Site;

/// <summary>
/// 新闻分类服务接口
/// </summary>
/// <remarks>
/// 提供新闻分类的增删改查、唯一性校验等功能
/// </remarks>
public interface INewsCategoryService
{
    /// <summary>
    /// 获取新闻分类列表（支持分页、筛选）
    /// </summary>
    /// <param name="query">查询参数，包含分页、分类名称、分类编码、状态筛选</param>
    /// <returns>新闻分类列表</returns>
    Task<List<NewsCategoryDto>> GetListAsync(NewsCategoryQueryDto query);

    /// <summary>
    /// 获取新闻分类详情
    /// </summary>
    /// <param name="id">分类ID</param>
    /// <returns>新闻分类详情</returns>
    Task<NewsCategoryDto> GetByIdAsync(string id);

    /// <summary>
    /// 创建新闻分类
    /// </summary>
    /// <param name="dto">创建参数</param>
    /// <returns>新分类ID</returns>
    Task<string> CreateAsync(CreateNewsCategoryDto dto);

    /// <summary>
    /// 更新新闻分类
    /// </summary>
    /// <param name="dto">更新参数</param>
    /// <returns>是否成功</returns>
    Task<bool> UpdateAsync(UpdateNewsCategoryDto dto);

    /// <summary>
    /// 删除新闻分类
    /// </summary>
    /// <param name="id">分类ID</param>
    /// <returns>是否成功</returns>
    Task<bool> DeleteAsync(string id);

    /// <summary>
    /// 检查分类名称是否已存在
    /// </summary>
    /// <param name="categoryName">分类名称</param>
    /// <param name="excludeId">排除的分类ID（用于更新时排除自己）</param>
    /// <returns>是否已存在</returns>
    Task<bool> IsNameExistsAsync(string categoryName, string? excludeId = null);

    /// <summary>
    /// 检查分类编码是否已存在
    /// </summary>
    /// <param name="categoryCode">分类编码</param>
    /// <param name="excludeId">排除的分类ID（用于更新时排除自己）</param>
    /// <returns>是否已存在</returns>
    Task<bool> IsCodeExistsAsync(string categoryCode, string? excludeId = null);

    /// <summary>
    /// 获取所有启用的新闻分类（用于下拉选择）
    /// </summary>
    /// <returns>新闻分类列表</returns>
    Task<List<NewsCategoryDto>> GetEnabledListAsync();
}