using EasyProduct.Common.Base;
using EasyProduct.Common.Error;
using EasyProduct.Models.Dto.Basic;
using EasyProduct.Models.Entitys.Basic;
using EasyProduct.Models.Enums;
using Mapster;
using SqlSugar;

namespace EasyProduct.Business.Basic;

/// <summary>
/// 用户服务实现
/// </summary>
public class UserService : BaseService<basic_user>, IUserService
{
    /// <summary>
    /// 分页查询用户列表（支持多条件查询和排序）
    /// </summary>
    /// <param name="query">查询参数，包含分页、关键词、状态、部门等</param>
    /// <returns>用户分页列表</returns>
    public async Task<PageResponse<UserDto>> GetPageListAsync(UserQueryDto query)
    {
        var whereExpr = Expressionable.Create<basic_user>()
            .AndIF(!string.IsNullOrEmpty(query.Keyword), u => u.UserName.Contains(query.Keyword!) || u.RealName!.Contains(query.Keyword!))
            .AndIF(query.Status.HasValue, u => u.Status == query.Status!.Value)
            .AndIF(!string.IsNullOrEmpty(query.DeptId), u => u.DeptId == query.DeptId)
            .And(u => u.IsDeleted == 0)
            .ToExpression();

        var result = await GetPageListAsync(
            query.PageIndex,
            query.PageSize,
            whereExpr,
            u => u.CreatedAt,
            isAsc: false);

        // 使用 Mapster 进行对象映射
        var dtoList = result.List.Adapt<List<UserDto>>();

        // 查询每个用户的角色列表
        foreach (var userDto in dtoList)
        {
            userDto.Roles = await GetUserRolesAsync(userDto.Id);
        }

        return PageResponse<UserDto>.Create(dtoList, result.Total, result.PageIndex, result.PageSize);
    }

    /// <summary>
    /// 根据ID获取用户
    /// </summary>
    /// <param name="id">用户ID</param>
    /// <returns>用户信息</returns>
    public new async Task<UserDto?> GetByIdAsync(Guid id)
    {
        var entity = await base.GetByIdAsync(id);
        if (entity == null || entity.IsDeleted == 1)
            return null;

        var userDto = entity.Adapt<UserDto>();

        // 查询用户的角色列表
        userDto.Roles = await GetUserRolesAsync(id);

        return userDto;
    }

    /// <summary>
    /// 创建用户
    /// </summary>
    /// <param name="dto">创建用户参数</param>
    /// <returns>创建成功返回 true</returns>
    /// <exception cref="BusinessException">用户名已存在时抛出</exception>
    public async Task<bool> CreateAsync(CreateUserDto dto)
    {
        // 检查用户名是否已存在
        if (await ExistsAsync(u => u.UserName == dto.UserName && u.IsDeleted == 0))
        {
            throw new BusinessException("用户名已存在", 400);
        }

        var entity = dto.Adapt<basic_user>();
        entity.Id = Guid.NewGuid();
        entity.Password = BCrypt.Net.BCrypt.HashPassword(dto.Password);
        entity.Status = dto.Status;
        entity.IsDeleted = 0;
        entity.CreatedAt = DateTime.UtcNow;

        // 使用事务创建用户和角色关联
        return await ExecuteTransactionAsync(async () =>
        {
            // 插入用户
            await InsertAsync(entity);

            // 插入用户角色关联
            if (dto.RoleIds != null && dto.RoleIds.Count > 0)
            {
                await InsertUserRolesAsync(entity.Id.ToString(), dto.RoleIds);
            }
        });
    }

    /// <summary>
    /// 更新用户
    /// </summary>
    /// <param name="dto">更新用户参数</param>
    /// <returns>更新成功返回 true</returns>
    /// <exception cref="BusinessException">用户不存在时抛出</exception>
    public async Task<bool> UpdateAsync(UpdateUserDto dto)
    {
        var entity = await base.GetByIdAsync(dto.Id);
        if (entity == null || entity.IsDeleted == 1)
        {
            throw new BusinessException("用户不存在", 404);
        }

        // 更新字段
        if (!string.IsNullOrEmpty(dto.Password))
            entity.Password = BCrypt.Net.BCrypt.HashPassword(dto.Password);
        if (!string.IsNullOrEmpty(dto.RealName))
            entity.RealName = dto.RealName;
        if (!string.IsNullOrEmpty(dto.Phone))
            entity.Phone = dto.Phone;
        if (!string.IsNullOrEmpty(dto.Email))
            entity.Email = dto.Email;
        if (!string.IsNullOrEmpty(dto.Avatar))
            entity.Avatar = dto.Avatar;
        if (!string.IsNullOrEmpty(dto.DeptId))
            entity.DeptId = dto.DeptId;
        if (dto.Status.HasValue)
            entity.Status = dto.Status.Value;

        entity.UpdatedAt = DateTime.UtcNow;

        // 使用事务更新用户和角色关联
        return await ExecuteTransactionAsync(async () =>
        {
            // 更新用户
            await UpdateAsync(entity);

            // 更新用户角色关联
            if (dto.RoleIds != null)
            {
                // 删除旧的角色关联
                await _db.Deleteable<basic_user_role>()
                    .Where(ur => ur.UserId == dto.Id.ToString())
                    .ExecuteCommandAsync();

                // 插入新的角色关联
                if (dto.RoleIds.Count > 0)
                {
                    await InsertUserRolesAsync(dto.Id.ToString(), dto.RoleIds);
                }
            }
        });
    }

    /// <summary>
    /// 删除用户（软删除）
    /// </summary>
    /// <param name="id">用户ID</param>
    /// <returns>删除成功返回 true</returns>
    public new async Task<bool> DeleteAsync(Guid id)
    {
        var entity = await base.GetByIdAsync(id);
        if (entity == null || entity.IsDeleted == 1)
            return false;

        entity.IsDeleted = 1;
        entity.UpdatedAt = DateTime.UtcNow;
        entity.Status = Status.Disabled;

        return await UpdateAsync(entity) > 0;
    }

    /// <summary>
    /// 重置用户密码
    /// </summary>
    /// <param name="id">用户ID</param>
    /// <param name="newPassword">新密码</param>
    /// <returns>重置成功返回 true</returns>
    /// <exception cref="BusinessException">用户不存在时抛出</exception>
    public async Task<bool> ResetPasswordAsync(Guid id, string newPassword)
    {
        var entity = await base.GetByIdAsync(id);
        if (entity == null || entity.IsDeleted == 1)
        {
            throw new BusinessException("用户不存在", 404);
        }

        entity.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
        entity.UpdatedAt = DateTime.UtcNow;

        return await UpdateAsync(entity) > 0;
    }

    /// <summary>
    /// 分配用户角色
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="roleIds">角色ID列表</param>
    /// <returns>分配成功返回 true</returns>
    /// <exception cref="BusinessException">用户不存在时抛出</exception>
    public async Task<bool> AssignRolesAsync(Guid userId, List<string> roleIds)
    {
        var entity = await base.GetByIdAsync(userId);
        if (entity == null || entity.IsDeleted == 1)
        {
            throw new BusinessException("用户不存在", 404);
        }

        // 使用事务更新角色关联
        return await ExecuteTransactionAsync(async () =>
        {
            // 删除旧的角色关联
            await _db.Deleteable<basic_user_role>()
                .Where(ur => ur.UserId == userId.ToString())
                .ExecuteCommandAsync();

            // 插入新的角色关联
            if (roleIds.Count > 0)
            {
                await InsertUserRolesAsync(userId.ToString(), roleIds);
            }
        });
    }

    /// <summary>
    /// 查询用户的角色列表
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <returns>角色列表</returns>
    private async Task<List<UserRoleDto>> GetUserRolesAsync(Guid userId)
    {
        var roles = await _db.Queryable<basic_user_role, basic_role>((ur, r) => new JoinQueryInfos(
            JoinType.Left, ur.RoleId == r.Id.ToString()
        ))
        .Where((ur, r) => ur.UserId == userId.ToString() && ur.IsDeleted == 0 && r.IsDeleted == 0)
        .Select((ur, r) => new UserRoleDto
        {
            RoleId = r.Id.ToString(),
            RoleName = r.RoleName,
            RoleCode = r.RoleCode
        })
        .ToListAsync();

        return roles;
    }

    /// <summary>
    /// 插入用户角色关联
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="roleIds">角色ID列表</param>
    private async Task InsertUserRolesAsync(string userId, List<string> roleIds)
    {
        var userRoles = roleIds.Select(roleId => new basic_user_role
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            RoleId = roleId,
            Status = Status.Enabled,
            IsDeleted = 0,
            CreatedAt = DateTime.UtcNow
        }).ToList();

        await _db.Insertable(userRoles).ExecuteCommandAsync();
    }
}