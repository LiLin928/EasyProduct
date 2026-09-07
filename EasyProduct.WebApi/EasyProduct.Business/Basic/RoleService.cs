using EasyProduct.Common.Base;
using EasyProduct.Common.Error;
using EasyProduct.Models.Dto.Basic;
using EasyProduct.Models.Entitys.Basic;
using EasyProduct.Models.Enums;
using Mapster;
using SqlSugar;

namespace EasyProduct.Business.Basic;

/// <summary>
/// 角色服务实现
/// </summary>
public class RoleService : BaseService<basic_role>, IRoleService
{
    /// <summary>
    /// 分页查询角色列表（支持多条件查询和排序）
    /// </summary>
    /// <param name="query">查询参数，包含分页、角色名称、角色编码、状态等</param>
    /// <returns>角色分页列表</returns>
    public async Task<PageResponse<RoleDto>> GetPageListAsync(RoleQueryDto query)
    {
        var whereExpr = Expressionable.Create<basic_role>()
            .AndIF(!string.IsNullOrEmpty(query.RoleName), r => r.RoleName.Contains(query.RoleName!))
            .AndIF(!string.IsNullOrEmpty(query.RoleCode), r => r.RoleCode.Contains(query.RoleCode!))
            .AndIF(query.Status.HasValue, r => r.Status == query.Status!.Value)
            .And(r => r.IsDeleted == 0)
            .ToExpression();

        var result = await GetPageListAsync(
            query.PageIndex,
            query.PageSize,
            whereExpr,
            r => r.Sort,
            isAsc: true);

        // 使用 Mapster 进行对象映射
        var dtoList = result.List.Adapt<List<RoleDto>>();

        // 查询每个角色的菜单列表
        foreach (var roleDto in dtoList)
        {
            roleDto.MenuIds = await GetRoleMenusAsync(roleDto.Id);
        }

        return PageResponse<RoleDto>.Create(dtoList, result.Total, result.PageIndex, result.PageSize);
    }

    /// <summary>
    /// 根据ID获取角色
    /// </summary>
    /// <param name="id">角色ID</param>
    /// <returns>角色信息</returns>
    public new async Task<RoleDto?> GetByIdAsync(Guid id)
    {
        var entity = await base.GetByIdAsync(id);
        if (entity == null || entity.IsDeleted == 1)
            return null;

        var roleDto = entity.Adapt<RoleDto>();

        // 查询角色的菜单列表
        roleDto.MenuIds = await GetRoleMenusAsync(id);

        return roleDto;
    }

    /// <summary>
    /// 创建角色
    /// </summary>
    /// <param name="dto">创建角色参数</param>
    /// <returns>创建成功返回角色ID</returns>
    /// <exception cref="BusinessException">角色编码已存在时抛出</exception>
    public async Task<string> CreateAsync(CreateRoleDto dto)
    {
        // 检查角色编码是否已存在
        if (await ExistsAsync(r => r.RoleCode == dto.RoleCode && r.IsDeleted == 0))
        {
            throw new BusinessException("角色编码已存在", 400);
        }

        // 检查角色名称是否已存在
        if (await ExistsAsync(r => r.RoleName == dto.RoleName && r.IsDeleted == 0))
        {
            throw new BusinessException("角色名称已存在", 400);
        }

        var entity = dto.Adapt<basic_role>();
        entity.Id = Guid.NewGuid();
        entity.Status = dto.Status;
        entity.IsDeleted = 0;
        entity.CreatedAt = DateTime.UtcNow;

        // 使用事务创建角色和菜单关联
        await ExecuteTransactionAsync(async () =>
        {
            // 插入角色
            await InsertAsync(entity);

            // 插入角色菜单关联
            if (dto.MenuIds != null && dto.MenuIds.Count > 0)
            {
                await InsertRoleMenusAsync(entity.Id.ToString(), dto.MenuIds);
            }
        });

        return entity.Id.ToString();
    }

    /// <summary>
    /// 更新角色
    /// </summary>
    /// <param name="id">角色ID</param>
    /// <param name="dto">更新角色参数</param>
    /// <returns>更新成功返回 true</returns>
    /// <exception cref="BusinessException">角色不存在或编码/名称冲突时抛出</exception>
    public async Task<bool> UpdateAsync(Guid id, UpdateRoleDto dto)
    {
        var entity = await base.GetByIdAsync(id);
        if (entity == null || entity.IsDeleted == 1)
        {
            throw new BusinessException("角色不存在", 404);
        }

        // 检查角色编码是否与其他角色冲突
        if (!string.IsNullOrEmpty(dto.RoleCode) && dto.RoleCode != entity.RoleCode)
        {
            if (await ExistsAsync(r => r.RoleCode == dto.RoleCode && r.Id != id && r.IsDeleted == 0))
            {
                throw new BusinessException("角色编码已存在", 400);
            }
        }

        // 检查角色名称是否与其他角色冲突
        if (!string.IsNullOrEmpty(dto.RoleName) && dto.RoleName != entity.RoleName)
        {
            if (await ExistsAsync(r => r.RoleName == dto.RoleName && r.Id != id && r.IsDeleted == 0))
            {
                throw new BusinessException("角色名称已存在", 400);
            }
        }

        // 更新字段
        if (!string.IsNullOrEmpty(dto.RoleName))
            entity.RoleName = dto.RoleName;
        if (!string.IsNullOrEmpty(dto.RoleCode))
            entity.RoleCode = dto.RoleCode;
        if (dto.Status.HasValue)
            entity.Status = dto.Status.Value;
        if (dto.Sort.HasValue)
            entity.Sort = dto.Sort.Value;
        if (dto.Description != null)
            entity.Description = dto.Description;

        entity.UpdatedAt = DateTime.UtcNow;

        // 使用事务更新角色和菜单关联
        return await ExecuteTransactionAsync(async () =>
        {
            // 更新角色
            await UpdateAsync(entity);

            // 更新角色菜单关联
            if (dto.MenuIds != null)
            {
                // 删除旧的菜单关联
                await _db.Deleteable<basic_role_menu>()
                    .Where(rm => rm.RoleId == id.ToString())
                    .ExecuteCommandAsync();

                // 插入新的菜单关联
                if (dto.MenuIds.Count > 0)
                {
                    await InsertRoleMenusAsync(id.ToString(), dto.MenuIds);
                }
            }
        });
    }

    /// <summary>
    /// 删除角色（软删除）
    /// </summary>
    /// <param name="id">角色ID</param>
    /// <returns>删除成功返回 true</returns>
    /// <exception cref="BusinessException">角色不存在或有关联用户时抛出</exception>
    public new async Task<bool> DeleteAsync(Guid id)
    {
        var entity = await base.GetByIdAsync(id);
        if (entity == null || entity.IsDeleted == 1)
        {
            throw new BusinessException("角色不存在", 404);
        }

        // 检查是否有用户关联此角色
        var hasUserRelation = await _db.Queryable<basic_user_role>()
            .AnyAsync(ur => ur.RoleId == id.ToString() && ur.IsDeleted == 0);

        if (hasUserRelation)
        {
            throw new BusinessException("该角色已分配给用户，无法删除", 400);
        }

        entity.IsDeleted = 1;
        entity.UpdatedAt = DateTime.UtcNow;
        entity.Status = Status.Disabled;

        return await UpdateAsync(entity) > 0;
    }

    /// <summary>
    /// 分配菜单权限给角色
    /// </summary>
    /// <param name="roleId">角色ID</param>
    /// <param name="menuIds">菜单ID列表</param>
    /// <returns>分配成功返回 true</returns>
    /// <exception cref="BusinessException">角色不存在时抛出</exception>
    public async Task<bool> AssignMenusAsync(Guid roleId, List<string> menuIds)
    {
        var entity = await base.GetByIdAsync(roleId);
        if (entity == null || entity.IsDeleted == 1)
        {
            throw new BusinessException("角色不存在", 404);
        }

        // 使用事务更新菜单关联
        return await ExecuteTransactionAsync(async () =>
        {
            // 删除旧的菜单关联
            await _db.Deleteable<basic_role_menu>()
                .Where(rm => rm.RoleId == roleId.ToString())
                .ExecuteCommandAsync();

            // 插入新的菜单关联
            if (menuIds.Count > 0)
            {
                await InsertRoleMenusAsync(roleId.ToString(), menuIds);
            }
        });
    }

    /// <summary>
    /// 获取角色的菜单ID列表
    /// </summary>
    /// <param name="roleId">角色ID</param>
    /// <returns>菜单ID列表</returns>
    public async Task<List<string>> GetRoleMenusAsync(Guid roleId)
    {
        var menuIds = await _db.Queryable<basic_role_menu>()
            .Where(rm => rm.RoleId == roleId.ToString() && rm.IsDeleted == 0)
            .Select(rm => rm.MenuId)
            .ToListAsync();

        return menuIds;
    }

    /// <summary>
    /// 插入角色菜单关联
    /// </summary>
    /// <param name="roleId">角色ID</param>
    /// <param name="menuIds">菜单ID列表</param>
    private async Task InsertRoleMenusAsync(string roleId, List<string> menuIds)
    {
        var roleMenus = menuIds.Select(menuId => new basic_role_menu
        {
            Id = Guid.NewGuid(),
            RoleId = roleId,
            MenuId = menuId,
            Status = Status.Enabled,
            IsDeleted = 0,
            CreatedAt = DateTime.UtcNow
        }).ToList();

        await _db.Insertable(roleMenus).ExecuteCommandAsync();
    }
}