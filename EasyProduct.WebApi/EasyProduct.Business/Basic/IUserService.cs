using EasyProduct.Common.Base;
using EasyProduct.Models.Dto.Basic;

namespace EasyProduct.Business.Basic;

/// <summary>
/// 用户服务接口
/// </summary>
public interface IUserService
{
    /// <summary>
    /// 获取用户分页列表
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>用户分页列表</returns>
    Task<PageResponse<UserDto>> GetPageListAsync(UserQueryDto query);

    /// <summary>
    /// 根据ID获取用户
    /// </summary>
    /// <param name="id">用户ID</param>
    /// <returns>用户信息</returns>
    Task<UserDto?> GetByIdAsync(Guid id);

    /// <summary>
    /// 创建用户
    /// </summary>
    /// <param name="dto">创建用户参数</param>
    /// <returns>创建成功返回 true</returns>
    Task<bool> CreateAsync(CreateUserDto dto);

    /// <summary>
    /// 更新用户
    /// </summary>
    /// <param name="dto">更新用户参数</param>
    /// <returns>更新成功返回 true</returns>
    Task<bool> UpdateAsync(UpdateUserDto dto);

    /// <summary>
    /// 删除用户
    /// </summary>
    /// <param name="id">用户ID</param>
    /// <returns>删除成功返回 true</returns>
    Task<bool> DeleteAsync(Guid id);
}
