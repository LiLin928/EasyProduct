using EasyProduct.Common.Base;
using EasyProduct.Models.Dto.Basic;

namespace EasyProduct.Business.Basic;

/// <summary>
/// 角色服务接口
/// </summary>
public interface IRoleService
{
    /// <summary>
    /// 分页查询角色列表
    /// </summary>
    /// <param name="query">查询参数，包含分页、角色名称、角色编码、状态等</param>
    /// <returns>角色分页列表</returns>
    Task<PageResponse<RoleDto>> GetPageListAsync(RoleQueryDto query);

    /// <summary>
    /// 根据ID获取角色
    /// </summary>
    /// <param name="id">角色ID</param>
    /// <returns>角色信息</returns>
    Task<RoleDto?> GetByIdAsync(Guid id);

    /// <summary>
    /// 创建角色
    /// </summary>
    /// <param name="dto">创建角色参数</param>
    /// <returns>创建成功返回角色ID</returns>
    Task<string> CreateAsync(CreateRoleDto dto);

    /// <summary>
    /// 更新角色
    /// </summary>
    /// <param name="id">角色ID</param>
    /// <param name="dto">更新角色参数</param>
    /// <returns>更新成功返回 true</returns>
    Task<bool> UpdateAsync(Guid id, UpdateRoleDto dto);

    /// <summary>
    /// 删除角色（软删除）
    /// </summary>
    /// <param name="id">角色ID</param>
    /// <returns>删除成功返回 true</returns>
    Task<bool> DeleteAsync(Guid id);

    /// <summary>
    /// 分配菜单权限给角色
    /// </summary>
    /// <param name="roleId">角色ID</param>
    /// <param name="menuIds">菜单ID列表</param>
    /// <returns>分配成功返回 true</returns>
    Task<bool> AssignMenusAsync(Guid roleId, List<string> menuIds);

    /// <summary>
    /// 获取角色的菜单ID列表
    /// </summary>
    /// <param name="roleId">角色ID</param>
    /// <returns>菜单ID列表</returns>
    Task<List<string>> GetRoleMenusAsync(Guid roleId);
}