using EasyProduct.Models.Dto.Basic.Menu;

namespace EasyProduct.Business.Basic;

/// <summary>
/// 菜单服务接口
/// </summary>
/// <remarks>
/// 提供菜单管理的核心业务功能，包括菜单树的构建、菜单的 CRUD 操作、排序等
/// </remarks>
public interface IMenuService
{
    /// <summary>
    /// 获取菜单树
    /// </summary>
    /// <returns>菜单树列表，按排序字段排序</returns>
    /// <remarks>
    /// 1. 查询所有未删除的菜单
    /// 2. 构建树形结构（递归）
    /// 3. 按排序字段排序
    /// 4. 返回完整的菜单树
    /// </remarks>
    Task<List<MenuTreeDto>> GetTreeAsync();

    /// <summary>
    /// 根据ID获取菜单详情
    /// </summary>
    /// <param name="id">菜单ID</param>
    /// <returns>菜单详情</returns>
    /// <remarks>
    /// 根据菜单ID查询菜单详细信息，包括所有字段
    /// </remarks>
    Task<MenuDto?> GetByIdAsync(string id);

    /// <summary>
    /// 创建菜单
    /// </summary>
    /// <param name="dto">创建菜单参数</param>
    /// <returns>创建成功返回菜单ID</returns>
    /// <exception cref="Common.Error.BusinessException">
    /// 父菜单不存在或菜单编码重复时抛出
    /// </exception>
    /// <remarks>
    /// 1. 验证父菜单存在性（如果有）
    /// 2. 验证菜单编码唯一性
    /// 3. 创建菜单记录
    /// 4. 返回菜单ID
    /// </remarks>
    Task<string> CreateAsync(CreateMenuDto dto);

    /// <summary>
    /// 更新菜单
    /// </summary>
    /// <param name="id">菜单ID</param>
    /// <param name="dto">更新菜单参数</param>
    /// <returns>更新成功返回 true</returns>
    /// <exception cref="Common.Error.BusinessException">
    /// 菜单不存在或编码冲突时抛出
    /// </exception>
    /// <remarks>
    /// 1. 检查菜单是否存在
    /// 2. 验证菜单编码唯一性
    /// 3. 更新菜单信息
    /// 4. 返回更新结果
    /// </remarks>
    Task<bool> UpdateAsync(string id, UpdateMenuDto dto);

    /// <summary>
    /// 删除菜单（软删除）
    /// </summary>
    /// <param name="id">菜单ID</param>
    /// <returns>删除成功返回 true</returns>
    /// <exception cref="Common.Error.BusinessException">
    /// 菜单不存在或有子菜单时抛出
    /// </exception>
    /// <remarks>
    /// 1. 检查菜单是否存在
    /// 2. 检查是否有子菜单
    /// 3. 执行软删除（IsDeleted = 1）
    /// 4. 返回删除结果
    /// </remarks>
    Task<bool> DeleteAsync(string id);

    /// <summary>
    /// 更新菜单排序（拖拽排序）
    /// </summary>
    /// <param name="dragId">被拖拽的菜单ID</param>
    /// <param name="dropId">目标位置的菜单ID</param>
    /// <param name="type">拖拽类型：before=前，after=后，inner=内部</param>
    /// <returns>更新成功返回 true</returns>
    /// <exception cref="Common.Error.BusinessException">
    /// 菜单不存在时抛出
    /// </exception>
    /// <remarks>
    /// 1. 验证菜单存在性
    /// 2. 根据拖拽类型更新菜单的 ParentId 和 Sort
    /// 3. 返回更新结果
    /// </remarks>
    Task<bool> UpdateSortAsync(string dragId, string dropId, string type);
}