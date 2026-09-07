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
    /// <param name="query">查询参数，包含分页、关键词、状态等</param>
    /// <returns>用户分页列表</returns>
    public async Task<PageResponse<UserDto>> GetPageListAsync(UserQueryDto query)
    {
        var whereExpr = Expressionable.Create<basic_user>()
            .AndIF(!string.IsNullOrEmpty(query.Keyword), u => u.UserName.Contains(query.Keyword!) || u.RealName!.Contains(query.Keyword!))
            .AndIF(query.Status.HasValue, u => u.Status == query.Status!.Value)
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

        return entity.Adapt<UserDto>();
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
        entity.Status = Status.Enabled;
        entity.IsDeleted = 0;
        entity.CreatedAt = DateTime.UtcNow;

        return await InsertAsync(entity);
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

        return await UpdateAsync(entity) > 0;
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
}