using EasyProduct.Business.Basic;
using EasyProduct.Common.Base;
using EasyProduct.Models.Dto.Basic;
using EasyProduct.Web.Controllers.Admin.Base;
using Microsoft.AspNetCore.Mvc;

namespace EasyProduct.Web.Controllers.Admin.Basic;

/// <summary>
/// 角色管理控制器
/// </summary>
/// <remarks>
/// 提供角色管理的 CRUD 接口，包括角色列表、创建、更新、删除、分配菜单权限等功能
/// </remarks>
public class RoleController : AdminControllerBase
{
    /// <summary>
    /// 角色服务接口（属性注入）
    /// </summary>
    public IRoleService _roleService { get; set; } = null!;

    /// <summary>
    /// 获取角色分页列表
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>角色分页列表</returns>
    [HttpGet]
    public async Task<ApiResponse<PageResponse<RoleDto>>> GetPageList([FromQuery] RoleQueryDto query)
    {
        var result = await _roleService.GetPageListAsync(query);
        return Success(result);
    }

    /// <summary>
    /// 根据ID获取角色
    /// </summary>
    /// <param name="id">角色ID</param>
    /// <returns>角色信息</returns>
    [HttpGet("{id}")]
    public async Task<ApiResponse<RoleDto>> GetById(Guid id)
    {
        var result = await _roleService.GetByIdAsync(id);
        return Success(result!);
    }

    /// <summary>
    /// 创建角色
    /// </summary>
    /// <param name="dto">创建角色参数</param>
    /// <returns>创建结果，返回角色ID</returns>
    [HttpPost]
    public async Task<ApiResponse<string>> Create([FromBody] CreateRoleDto dto)
    {
        var result = await _roleService.CreateAsync(dto);
        return Success(result, "创建成功");
    }

    /// <summary>
    /// 更新角色
    /// </summary>
    /// <param name="id">角色ID</param>
    /// <param name="dto">更新角色参数</param>
    /// <returns>更新结果</returns>
    [HttpPut("{id}")]
    public async Task<ApiResponse<object>> Update(Guid id, [FromBody] UpdateRoleDto dto)
    {
        var result = await _roleService.UpdateAsync(id, dto);
        return result ? Success("更新成功") : Error<object>("更新失败");
    }

    /// <summary>
    /// 删除角色
    /// </summary>
    /// <param name="id">角色ID</param>
    /// <returns>删除结果</returns>
    [HttpDelete("{id}")]
    public async Task<ApiResponse<object>> Delete(Guid id)
    {
        var result = await _roleService.DeleteAsync(id);
        return result ? Success("删除成功") : Error<object>("删除失败");
    }

    /// <summary>
    /// 分配菜单权限给角色
    /// </summary>
    /// <param name="id">角色ID</param>
    /// <param name="dto">分配菜单参数</param>
    /// <returns>分配结果</returns>
    [HttpPost("{id}/assign-menus")]
    public async Task<ApiResponse<object>> AssignMenus(Guid id, [FromBody] AssignMenusDto dto)
    {
        var result = await _roleService.AssignMenusAsync(id, dto.MenuIds);
        return result ? Success("菜单分配成功") : Error<object>("菜单分配失败");
    }

    /// <summary>
    /// 获取角色的菜单列表
    /// </summary>
    /// <param name="id">角色ID</param>
    /// <returns>菜单ID列表</returns>
    [HttpGet("{id}/menus")]
    public async Task<ApiResponse<List<string>>> GetRoleMenus(Guid id)
    {
        var result = await _roleService.GetRoleMenusAsync(id);
        return Success(result);
    }
}

/// <summary>
/// 分配菜单请求 DTO
/// </summary>
public class AssignMenusDto
{
    /// <summary>
    /// 菜单ID列表
    /// </summary>
    public List<string> MenuIds { get; set; } = new List<string>();
}