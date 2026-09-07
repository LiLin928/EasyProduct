using EasyProduct.Common.Base;
using EasyProduct.Common.Error;
using EasyProduct.Models.Dto.Basic.Menu;
using EasyProduct.Models.Entitys.Basic;
using EasyProduct.Models.Enums;
using Mapster;
using SqlSugar;

namespace EasyProduct.Business.Basic;

/// <summary>
/// 菜单服务实现
/// </summary>
/// <remarks>
/// 提供菜单管理的核心业务功能，包括菜单树的构建、菜单的 CRUD 操作、排序等
/// 继承 BaseService&lt;basic_menu&gt;，使用 SqlSugar 进行数据库操作
/// </remarks>
public class MenuService : BaseService<basic_menu>, IMenuService
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
    public async Task<List<MenuTreeDto>> GetTreeAsync()
    {
        // 查询所有未删除的菜单，按排序字段排序
        var menus = await _db.Queryable<basic_menu>()
            .Where(m => m.IsDeleted == 0)
            .OrderBy(m => m.Sort)
            .ToListAsync();

        // 使用 Mapster 进行对象映射
        var menuDtos = menus.Adapt<List<MenuTreeDto>>();

        // 构建树形结构
        return BuildMenuTree(menuDtos, string.Empty);
    }

    /// <summary>
    /// 根据ID获取菜单详情
    /// </summary>
    /// <param name="id">菜单ID</param>
    /// <returns>菜单详情</returns>
    /// <remarks>
    /// 根据菜单ID查询菜单详细信息，包括所有字段
    /// </remarks>
    public async Task<MenuDto?> GetByIdAsync(string id)
    {
        var menu = await _db.Queryable<basic_menu>()
            .Where(m => m.Id.ToString() == id && m.IsDeleted == 0)
            .FirstAsync();

        if (menu == null)
            return null;

        // 使用 Mapster 进行对象映射
        return menu.Adapt<MenuDto>();
    }

    /// <summary>
    /// 创建菜单
    /// </summary>
    /// <param name="dto">创建菜单参数</param>
    /// <returns>创建成功返回菜单ID</returns>
    /// <exception cref="BusinessException">父菜单不存在或菜单编码重复时抛出</exception>
    /// <remarks>
    /// 1. 验证父菜单存在性（如果有）
    /// 2. 验证菜单编码唯一性
    /// 3. 创建菜单记录
    /// 4. 返回菜单ID
    /// </remarks>
    public async Task<string> CreateAsync(CreateMenuDto dto)
    {
        // 验证父菜单存在性
        if (!string.IsNullOrEmpty(dto.ParentId) && dto.ParentId != "0")
        {
            var parentExists = await ExistsAsync(m => m.Id.ToString() == dto.ParentId && m.IsDeleted == 0);
            if (!parentExists)
            {
                throw new BusinessException("父菜单不存在", 404);
            }
        }

        // 验证菜单编码唯一性
        if (!string.IsNullOrEmpty(dto.MenuCode))
        {
            var codeExists = await ExistsAsync(m => m.MenuCode == dto.MenuCode && m.IsDeleted == 0);
            if (codeExists)
            {
                throw new BusinessException("菜单编码已存在", 400);
            }
        }

        // 创建菜单实体
        var entity = dto.Adapt<basic_menu>();
        entity.Id = Guid.NewGuid();
        entity.ParentId = dto.ParentId ?? string.Empty;
        entity.IsDeleted = 0;
        entity.CreatedAt = DateTime.UtcNow;

        // 插入菜单
        await InsertAsync(entity);

        return entity.Id.ToString();
    }

    /// <summary>
    /// 更新菜单
    /// </summary>
    /// <param name="id">菜单ID</param>
    /// <param name="dto">更新菜单参数</param>
    /// <returns>更新成功返回 true</returns>
    /// <exception cref="BusinessException">菜单不存在或编码冲突时抛出</exception>
    /// <remarks>
    /// 1. 检查菜单是否存在
    /// 2. 验证菜单编码唯一性
    /// 3. 更新菜单信息
    /// 4. 返回更新结果
    /// </remarks>
    public async Task<bool> UpdateAsync(string id, UpdateMenuDto dto)
    {
        // 查询菜单
        var entity = await _db.Queryable<basic_menu>()
            .Where(m => m.Id.ToString() == id && m.IsDeleted == 0)
            .FirstAsync();

        if (entity == null)
        {
            throw new BusinessException("菜单不存在", 404);
        }

        // 验证父菜单存在性
        if (!string.IsNullOrEmpty(dto.ParentId) && dto.ParentId != "0" && dto.ParentId != entity.ParentId)
        {
            var parentExists = await ExistsAsync(m => m.Id.ToString() == dto.ParentId && m.IsDeleted == 0);
            if (!parentExists)
            {
                throw new BusinessException("父菜单不存在", 404);
            }

            // 不能将自己设置为自己的父菜单
            if (dto.ParentId == id)
            {
                throw new BusinessException("不能将菜单设置为自己的子菜单", 400);
            }
        }

        // 验证菜单编码唯一性
        if (!string.IsNullOrEmpty(dto.MenuCode) && dto.MenuCode != entity.MenuCode)
        {
            var codeExists = await ExistsAsync(m => m.MenuCode == dto.MenuCode && m.Id.ToString() != id && m.IsDeleted == 0);
            if (codeExists)
            {
                throw new BusinessException("菜单编码已存在", 400);
            }
        }

        // 更新字段
        if (!string.IsNullOrEmpty(dto.ParentId))
            entity.ParentId = dto.ParentId;
        if (!string.IsNullOrEmpty(dto.MenuName))
            entity.MenuName = dto.MenuName;
        if (dto.MenuCode != null)
            entity.MenuCode = dto.MenuCode;
        if (dto.Path != null)
            entity.Path = dto.Path;
        if (dto.Component != null)
            entity.Component = dto.Component;
        if (dto.Permission != null)
            entity.Permission = dto.Permission;
        if (dto.Icon != null)
            entity.Icon = dto.Icon;
        if (dto.Sort.HasValue)
            entity.Sort = dto.Sort.Value;
        if (dto.Type.HasValue)
            entity.Type = dto.Type.Value;
        if (dto.Visible.HasValue)
            entity.Visible = dto.Visible.Value;
        if (dto.IsExternal.HasValue)
            entity.IsExternal = dto.IsExternal.Value;
        if (dto.IsCache.HasValue)
            entity.IsCache = dto.IsCache.Value;
        if (dto.Status.HasValue)
            entity.Status = dto.Status.Value;

        entity.UpdatedAt = DateTime.UtcNow;

        // 更新菜单
        return await UpdateAsync(entity) > 0;
    }

    /// <summary>
    /// 删除菜单（软删除）
    /// </summary>
    /// <param name="id">菜单ID</param>
    /// <returns>删除成功返回 true</returns>
    /// <exception cref="BusinessException">菜单不存在或有子菜单时抛出</exception>
    /// <remarks>
    /// 1. 检查菜单是否存在
    /// 2. 检查是否有子菜单
    /// 3. 执行软删除（IsDeleted = 1）
    /// 4. 返回删除结果
    /// </remarks>
    public async Task<bool> DeleteAsync(string id)
    {
        // 查询菜单
        var entity = await _db.Queryable<basic_menu>()
            .Where(m => m.Id.ToString() == id && m.IsDeleted == 0)
            .FirstAsync();

        if (entity == null)
        {
            throw new BusinessException("菜单不存在", 404);
        }

        // 检查是否有子菜单
        var hasChildren = await ExistsAsync(m => m.ParentId == id && m.IsDeleted == 0);
        if (hasChildren)
        {
            throw new BusinessException("该菜单下存在子菜单，无法删除", 400);
        }

        // 软删除
        entity.IsDeleted = 1;
        entity.UpdatedAt = DateTime.UtcNow;
        entity.Status = Status.Disabled;

        return await UpdateAsync(entity) > 0;
    }

    /// <summary>
    /// 更新菜单排序（拖拽排序）
    /// </summary>
    /// <param name="dragId">被拖拽的菜单ID</param>
    /// <param name="dropId">目标位置的菜单ID</param>
    /// <param name="type">拖拽类型：before=前，after=后，inner=内部</param>
    /// <returns>更新成功返回 true</returns>
    /// <exception cref="BusinessException">菜单不存在时抛出</exception>
    /// <remarks>
    /// 1. 验证菜单存在性
    /// 2. 根据拖拽类型更新菜单的 ParentId 和 Sort
    /// 3. 返回更新结果
    /// </remarks>
    public async Task<bool> UpdateSortAsync(string dragId, string dropId, string type)
    {
        // 查询被拖拽的菜单
        var dragMenu = await _db.Queryable<basic_menu>()
            .Where(m => m.Id.ToString() == dragId && m.IsDeleted == 0)
            .FirstAsync();

        if (dragMenu == null)
        {
            throw new BusinessException("被拖拽的菜单不存在", 404);
        }

        // 查询目标菜单
        var dropMenu = await _db.Queryable<basic_menu>()
            .Where(m => m.Id.ToString() == dropId && m.IsDeleted == 0)
            .FirstAsync();

        if (dropMenu == null)
        {
            throw new BusinessException("目标菜单不存在", 404);
        }

        // 不能将自己拖拽到自己
        if (dragId == dropId)
        {
            throw new BusinessException("不能将菜单拖拽到自己", 400);
        }

        // 根据拖拽类型更新菜单
        switch (type.ToLower())
        {
            case "before":
                // 拖拽到目标菜单之前
                dragMenu.ParentId = dropMenu.ParentId;
                dragMenu.Sort = dropMenu.Sort - 1;
                break;

            case "after":
                // 拖拽到目标菜单之后
                dragMenu.ParentId = dropMenu.ParentId;
                dragMenu.Sort = dropMenu.Sort + 1;
                break;

            case "inner":
                // 拖拽到目标菜单内部
                dragMenu.ParentId = dropId;

                // 查询目标菜单下的最大排序值
                var maxSort = await _db.Queryable<basic_menu>()
                    .Where(m => m.ParentId == dropId && m.IsDeleted == 0)
                    .MaxAsync(m => m.Sort);

                dragMenu.Sort = maxSort + 1;
                break;

            default:
                throw new BusinessException("无效的拖拽类型", 400);
        }

        dragMenu.UpdatedAt = DateTime.UtcNow;

        // 更新菜单
        return await UpdateAsync(dragMenu) > 0;
    }

    /// <summary>
    /// 构建菜单树（递归）
    /// </summary>
    /// <param name="allMenus">所有菜单列表</param>
    /// <param name="parentId">父菜单ID</param>
    /// <returns>菜单树列表</returns>
    /// <remarks>
    /// 递归构建菜单树形结构：
    /// 1. 筛选出指定父菜单ID的所有子菜单
    /// 2. 对每个子菜单递归查找其子菜单
    /// 3. 返回树形结构
    /// </remarks>
    private List<MenuTreeDto> BuildMenuTree(List<MenuTreeDto> allMenus, string parentId)
    {
        var tree = new List<MenuTreeDto>();

        // 筛选出指定父菜单的所有子菜单
        var children = allMenus
            .Where(m => m.ParentId == parentId)
            .ToList();

        foreach (var child in children)
        {
            // 递归查找子菜单
            child.Children = BuildMenuTree(allMenus, child.Id.ToString());
            tree.Add(child);
        }

        return tree;
    }
}