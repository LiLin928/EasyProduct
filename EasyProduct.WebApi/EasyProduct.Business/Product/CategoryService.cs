using EasyProduct.Common.Base;
using EasyProduct.Common.Error;
using EasyProduct.Models.Dto.Product.Category;
using EasyProduct.Models.Entitys.Product;
using EasyProduct.Models.Enums;
using Mapster;
using Microsoft.Extensions.Logging;
using SqlSugar;

namespace EasyProduct.Business.Product;

/// <summary>
/// 商品分类服务实现
/// </summary>
/// <remarks>
/// 提供商品分类的增删改查、树形结构查询等功能
/// 继承 BaseService，使用属性注入获取数据库上下文
/// </remarks>
public class CategoryService : BaseService, ICategoryService
{
    /// <summary>
    /// 日志记录器
    /// </summary>
    private readonly ILogger<CategoryService> _logger;

    /// <summary>
    /// 构造函数，通过依赖注入获取日志记录器
    /// </summary>
    /// <param name="logger">日志记录器</param>
    public CategoryService(ILogger<CategoryService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// 获取分类树形结构
    /// </summary>
    /// <param name="query">查询参数，支持父分类ID筛选、仅查询启用状态等</param>
    /// <returns>分类树形结构列表</returns>
    /// <remarks>
    /// 1. 根据查询条件筛选分类数据
    /// 2. 构建树形结构，支持无限层级
    /// 3. 按排序字段排序
    /// </remarks>
    public async Task<List<CategoryTreeDto>> GetCategoryTreeAsync(CategoryTreeQueryDto query)
    {
        // 1. 构建查询条件
        var queryable = _db.Queryable<product_category>()
            .Where(c => c.IsDeleted == 0);

        // 如果指定了父分类ID，只查询该分类下的子分类
        if (!string.IsNullOrEmpty(query.ParentId))
        {
            queryable = queryable.Where(c => c.ParentId == query.ParentId);
        }

        // 如果仅查询启用的分类
        if (query.OnlyEnabled == true)
        {
            queryable = queryable.Where(c => c.Status == Status.Enabled);
        }

        // 2. 查询所有符合条件的分类
        var categories = await queryable
            .OrderBy(c => c.Sort)
            .OrderBy(c => c.CreatedAt)
            .ToListAsync();

        // 3. 转换为 DTO
        var categoryDtos = categories.Adapt<List<CategoryTreeDto>>();

        // 4. 构建树形结构
        var tree = BuildCategoryTree(categoryDtos, query.ParentId ?? string.Empty);

        return tree;
    }

    /// <summary>
    /// 获取分类列表（扁平）
    /// </summary>
    /// <param name="query">查询参数，支持分页、分类名称、分类编码、状态筛选</param>
    /// <returns>分类分页列表</returns>
    /// <remarks>
    /// 1. 支持按分类名称模糊搜索
    /// 2. 支持按分类编码精确匹配
    /// 3. 支持按状态筛选
    /// 4. 支持分页查询
    /// 5. 按排序字段排序，然后按创建时间倒序
    /// </remarks>
    public async Task<PageResponse<CategoryDto>> GetCategoryListAsync(CategoryQueryDto query)
    {
        // 1. 构建查询条件
        var whereExpr = Expressionable.Create<product_category>()
            .AndIF(!string.IsNullOrEmpty(query.CategoryName), c => c.CategoryName.Contains(query.CategoryName!))
            .AndIF(!string.IsNullOrEmpty(query.CategoryCode), c => c.CategoryCode == query.CategoryCode)
            .AndIF(query.Status.HasValue, c => c.Status == query.Status!.Value)
            .And(c => c.IsDeleted == 0)
            .ToExpression();

        // 2. 分页查询
        var queryable = _db.Queryable<product_category>()
            .Where(whereExpr)
            .OrderBy(c => c.Sort)
            .OrderByDescending(c => c.CreatedAt);

        RefAsync<int> total = 0;
        var categories = await queryable
            .ToPageListAsync(query.PageIndex, query.PageSize, total);

        // 3. 转换为 DTO
        var categoryDtos = categories.Adapt<List<CategoryDto>>();

        // 4. 返回分页结果
        return PageResponse<CategoryDto>.Create(categoryDtos, total.Value, query.PageIndex, query.PageSize);
    }

    /// <summary>
    /// 获取分类详情
    /// </summary>
    /// <param name="id">分类ID</param>
    /// <returns>分类详情</returns>
    /// <exception cref="BusinessException">分类不存在时抛出</exception>
    /// <remarks>
    /// 根据分类ID查询分类详细信息
    /// </remarks>
    public async Task<CategoryDto> GetCategoryByIdAsync(string id)
    {
        // 1. 查询分类
        var category = await _db.Queryable<product_category>()
            .Where(c => c.Id.ToString() == id && c.IsDeleted == 0)
            .FirstAsync();

        // 2. 检查分类是否存在
        if (category == null)
        {
            throw new BusinessException("分类不存在", 404);
        }

        // 3. 转换为 DTO
        return category.Adapt<CategoryDto>();
    }

    /// <summary>
    /// 创建分类
    /// </summary>
    /// <param name="dto">创建分类参数</param>
    /// <returns>创建成功返回 true</returns>
    /// <exception cref="BusinessException">分类编码已存在时抛出</exception>
    /// <remarks>
    /// 1. 检查分类编码唯一性
    /// 2. 自动计算层级和完整路径
    /// 3. 创建分类记录
    /// </remarks>
    public async Task<bool> CreateCategoryAsync(CreateCategoryDto dto)
    {
        // 1. 检查分类编码是否已存在
        if (!string.IsNullOrEmpty(dto.CategoryCode))
        {
            var exists = await _db.Queryable<product_category>()
                .Where(c => c.CategoryCode == dto.CategoryCode && c.IsDeleted == 0)
                .AnyAsync();

            if (exists)
            {
                throw new BusinessException("分类编码已存在", 400);
            }
        }

        // 2. 计算层级和完整路径
        var (level, fullPath) = await CalculateCategoryLevelAndPathAsync(dto.ParentId, dto.CategoryName);

        // 3. 创建分类实体
        var category = dto.Adapt<product_category>();
        category.Id = Guid.NewGuid();
        category.Level = level;
        category.FullPath = fullPath;
        category.Status = Status.Enabled;
        category.IsDeleted = 0;
        category.CreatedAt = DateTime.UtcNow;

        // 4. 插入数据库
        var result = await _db.Insertable(category).ExecuteCommandAsync();

        return result > 0;
    }

    /// <summary>
    /// 更新分类
    /// </summary>
    /// <param name="dto">更新分类参数</param>
    /// <returns>更新成功返回 true</returns>
    /// <exception cref="BusinessException">分类不存在、分类编码已存在、父节点设置为自己时抛出</exception>
    /// <remarks>
    /// 1. 检查分类是否存在
    /// 2. 检查分类编码唯一性（排除自己）
    /// 3. 检查不能将父节点设置为自己
    /// 4. 重新计算层级和完整路径
    /// 5. 更新分类记录
    /// </remarks>
    public async Task<bool> UpdateCategoryAsync(UpdateCategoryDto dto)
    {
        // 1. 查询分类
        var category = await _db.Queryable<product_category>()
            .Where(c => c.Id.ToString() == dto.Id && c.IsDeleted == 0)
            .FirstAsync();

        if (category == null)
        {
            throw new BusinessException("分类不存在", 404);
        }

        // 2. 检查分类编码是否已存在（排除自己）
        if (!string.IsNullOrEmpty(dto.CategoryCode))
        {
            var exists = await _db.Queryable<product_category>()
                .Where(c => c.CategoryCode == dto.CategoryCode && c.Id.ToString() != dto.Id && c.IsDeleted == 0)
                .AnyAsync();

            if (exists)
            {
                throw new BusinessException("分类编码已存在", 400);
            }
        }

        // 3. 检查不能将父节点设置为自己
        if (!string.IsNullOrEmpty(dto.ParentId) && dto.ParentId == dto.Id)
        {
            throw new BusinessException("不能将父分类设置为自己", 400);
        }

        // 4. 更新字段
        category.CategoryName = dto.CategoryName;
        category.ParentId = dto.ParentId;
        category.CategoryCode = dto.CategoryCode;
        category.Icon = dto.Icon;
        category.Image = dto.Image;
        category.Sort = dto.Sort;
        category.ShowInNav = dto.ShowInNav;
        category.Status = dto.Status;

        // 5. 重新计算层级和完整路径
        var (level, fullPath) = await CalculateCategoryLevelAndPathAsync(dto.ParentId, dto.CategoryName);
        category.Level = level;
        category.FullPath = fullPath;
        category.UpdatedAt = DateTime.UtcNow;

        // 6. 更新数据库
        var result = await _db.Updateable(category).ExecuteCommandAsync();

        return result > 0;
    }

    /// <summary>
    /// 删除分类（软删除）
    /// </summary>
    /// <param name="id">分类ID</param>
    /// <returns>删除成功返回 true</returns>
    /// <exception cref="BusinessException">分类不存在、存在子分类、存在关联商品时抛出</exception>
    /// <remarks>
    /// 1. 检查分类是否存在
    /// 2. 检查是否存在子分类
    /// 3. 检查是否存在关联商品（TODO: 等商品表创建）
    /// 4. 软删除分类记录
    /// </remarks>
    public async Task<bool> DeleteCategoryAsync(string id)
    {
        // 1. 查询分类
        var category = await _db.Queryable<product_category>()
            .Where(c => c.Id.ToString() == id && c.IsDeleted == 0)
            .FirstAsync();

        if (category == null)
        {
            throw new BusinessException("分类不存在", 404);
        }

        // 2. 检查是否存在子分类
        var hasChildren = await _db.Queryable<product_category>()
            .Where(c => c.ParentId == id && c.IsDeleted == 0)
            .AnyAsync();

        if (hasChildren)
        {
            throw new BusinessException("该分类下存在子分类，无法删除", 400);
        }

        // 3. 检查是否存在关联商品（TODO: 等商品表创建后实现）
        // var hasProducts = await _db.Queryable<product_product>()
        //     .Where(p => p.CategoryId == id && p.IsDeleted == 0)
        //     .AnyAsync();
        //
        // if (hasProducts)
        // {
        //     throw new BusinessException("该分类下存在商品，无法删除", 400);
        // }

        // 4. 软删除分类
        category.IsDeleted = 1;
        category.UpdatedAt = DateTime.UtcNow;
        category.Status = Status.Disabled;

        var result = await _db.Updateable(category).ExecuteCommandAsync();

        return result > 0;
    }

    /// <summary>
    /// 更新分类状态
    /// </summary>
    /// <param name="id">分类ID</param>
    /// <param name="status">状态值：0=禁用，1=启用</param>
    /// <returns>更新成功返回 true</returns>
    /// <exception cref="BusinessException">分类不存在时抛出</exception>
    /// <remarks>
    /// 更新分类的启用/禁用状态
    /// </remarks>
    public async Task<bool> UpdateCategoryStatusAsync(string id, int status)
    {
        // 1. 查询分类
        var category = await _db.Queryable<product_category>()
            .Where(c => c.Id.ToString() == id && c.IsDeleted == 0)
            .FirstAsync();

        if (category == null)
        {
            throw new BusinessException("分类不存在", 404);
        }

        // 2. 更新状态
        category.Status = (Status)status;
        category.UpdatedAt = DateTime.UtcNow;

        // 3. 更新数据库
        var result = await _db.Updateable(category)
            .UpdateColumns(c => new { c.Status, c.UpdatedAt })
            .ExecuteCommandAsync();

        return result > 0;
    }

    #region 辅助方法

    /// <summary>
    /// 构建分类树形结构
    /// </summary>
    /// <param name="allCategories">所有分类列表</param>
    /// <param name="parentId">父分类ID，空字符串表示根节点</param>
    /// <returns>树形结构分类列表</returns>
    /// <remarks>
    /// 递归构建树形结构：
    /// 1. 从根节点开始（parentId 为空字符串）
    /// 2. 查找所有父分类ID等于指定值的分类
    /// 3. 递归为每个分类查找子分类
    /// </remarks>
    private List<CategoryTreeDto> BuildCategoryTree(List<CategoryTreeDto> allCategories, string parentId)
    {
        // 1. 查找所有父分类ID等于指定值的分类
        var children = allCategories
            .Where(c => (string.IsNullOrEmpty(parentId) && string.IsNullOrEmpty(c.ParentId)) ||
                        (!string.IsNullOrEmpty(parentId) && c.ParentId == parentId))
            .ToList();

        // 2. 为每个分类递归查找子分类
        foreach (var child in children)
        {
            child.Children = BuildCategoryTree(allCategories, child.Id.ToString());
        }

        return children;
    }

    /// <summary>
    /// 计算分类层级和完整路径
    /// </summary>
    /// <param name="parentId">父分类ID</param>
    /// <param name="categoryName">当前分类名称</param>
    /// <returns>层级和完整路径的元组</returns>
    /// <remarks>
    /// 层级计算规则：
    /// - 根分类（无父分类）：层级为 1
    /// - 子分类：层级为父分类层级 + 1
    ///
    /// 完整路径计算规则：
    /// - 根分类：分类名称
    /// - 子分类：父分类完整路径 / 当前分类名称
    /// </remarks>
    private async Task<(int level, string fullPath)> CalculateCategoryLevelAndPathAsync(string? parentId, string categoryName)
    {
        // 1. 如果没有父分类，则为根分类
        if (string.IsNullOrEmpty(parentId) || parentId == "0")
        {
            return (1, categoryName);
        }

        // 2. 查询父分类
        var parentCategory = await _db.Queryable<product_category>()
            .Where(c => c.Id.ToString() == parentId && c.IsDeleted == 0)
            .FirstAsync();

        // 3. 如果父分类不存在，作为根分类处理
        if (parentCategory == null)
        {
            _logger.LogWarning("父分类不存在: {ParentId}，将作为根分类处理", parentId);
            return (1, categoryName);
        }

        // 4. 计算层级和完整路径
        var level = parentCategory.Level + 1;
        var fullPath = $"{parentCategory.FullPath}/{categoryName}";

        return (level, fullPath);
    }

    #endregion
}