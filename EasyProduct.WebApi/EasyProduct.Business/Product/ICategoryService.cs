using EasyProduct.Common.Base;
using EasyProduct.Models.Dto.Product.Category;

namespace EasyProduct.Business.Product;

/// <summary>
/// 商品分类服务接口
/// </summary>
/// <remarks>
/// 提供商品分类的增删改查、树形结构查询等功能
/// </remarks>
public interface ICategoryService
{
    /// <summary>
    /// 获取分类树形结构
    /// </summary>
    /// <param name="query">查询参数，支持父分类ID筛选、仅查询启用状态等</param>
    /// <returns>分类树形结构列表</returns>
    Task<List<CategoryTreeDto>> GetCategoryTreeAsync(CategoryTreeQueryDto query);

    /// <summary>
    /// 获取分类列表（扁平）
    /// </summary>
    /// <param name="query">查询参数，支持分页、分类名称、分类编码、状态筛选</param>
    /// <returns>分类分页列表</returns>
    Task<PageResponse<CategoryDto>> GetCategoryListAsync(CategoryQueryDto query);

    /// <summary>
    /// 获取分类详情
    /// </summary>
    /// <param name="id">分类ID</param>
    /// <returns>分类详情</returns>
    /// <exception cref="Common.Error.BusinessException">分类不存在时抛出</exception>
    Task<CategoryDto> GetCategoryByIdAsync(string id);

    /// <summary>
    /// 创建分类
    /// </summary>
    /// <param name="dto">创建分类参数</param>
    /// <returns>创建成功返回 true</returns>
    /// <exception cref="Common.Error.BusinessException">分类编码已存在时抛出</exception>
    Task<bool> CreateCategoryAsync(CreateCategoryDto dto);

    /// <summary>
    /// 更新分类
    /// </summary>
    /// <param name="dto">更新分类参数</param>
    /// <returns>更新成功返回 true</returns>
    /// <exception cref="Common.Error.BusinessException">分类不存在、分类编码已存在、父节点设置为自己时抛出</exception>
    Task<bool> UpdateCategoryAsync(UpdateCategoryDto dto);

    /// <summary>
    /// 删除分类（软删除）
    /// </summary>
    /// <param name="id">分类ID</param>
    /// <returns>删除成功返回 true</returns>
    /// <exception cref="Common.Error.BusinessException">分类不存在、存在子分类、存在关联商品时抛出</exception>
    Task<bool> DeleteCategoryAsync(string id);

    /// <summary>
    /// 更新分类状态
    /// </summary>
    /// <param name="id">分类ID</param>
    /// <param name="status">状态值：0=禁用，1=启用</param>
    /// <returns>更新成功返回 true</returns>
    /// <exception cref="Common.Error.BusinessException">分类不存在时抛出</exception>
    Task<bool> UpdateCategoryStatusAsync(string id, int status);
}