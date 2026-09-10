using EasyProduct.Common.Base;
using EasyProduct.Models.Dto.Basic;
using EasyProduct.Models.Dto.Basic.Profile;
using EasyProduct.Business.Basic;
using EasyProduct.Web.Controllers.Admin.Base;
using Microsoft.AspNetCore.Mvc;

namespace EasyProduct.Web.Controllers.Admin.Basic;

/// <summary>
/// 用户管理控制器
/// </summary>
/// <remarks>
/// 提供用户管理的 CRUD 接口，包括用户列表、创建、更新、删除、重置密码、分配角色等功能
/// </remarks>
public class UserController : AdminControllerBase
{
    /// <summary>
    /// 用户服务接口（属性注入）
    /// </summary>
    public IUserService _userService { get; set; } = null!;

    /// <summary>
    /// 获取用户分页列表
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>用户分页列表</returns>
    [HttpGet]
    public async Task<ApiResponse<PageResponse<UserDto>>> GetPageList([FromQuery] UserQueryDto query)
    {
        var result = await _userService.GetPageListAsync(query);
        return Success(result);
    }

    /// <summary>
    /// 根据ID获取用户
    /// </summary>
    /// <param name="id">用户ID</param>
    /// <returns>用户信息</returns>
    [HttpGet("{id}")]
    public async Task<ApiResponse<UserDto>> GetById(Guid id)
    {
        var result = await _userService.GetByIdAsync(id);
        return Success(result!);
    }

    /// <summary>
    /// 创建用户
    /// </summary>
    /// <param name="dto">创建用户参数</param>
    /// <returns>创建结果</returns>
    [HttpPost]
    public async Task<ApiResponse<object>> Create([FromBody] CreateUserDto dto)
    {
        var result = await _userService.CreateAsync(dto);
        return result ? Success("创建成功") : Error<object>("创建失败");
    }

    /// <summary>
    /// 更新用户
    /// </summary>
    /// <param name="dto">更新用户参数</param>
    /// <returns>更新结果</returns>
    [HttpPut]
    public async Task<ApiResponse<object>> Update([FromBody] UpdateUserDto dto)
    {
        var result = await _userService.UpdateAsync(dto);
        return result ? Success("更新成功") : Error<object>("更新失败");
    }

    /// <summary>
    /// 删除用户
    /// </summary>
    /// <param name="id">用户ID</param>
    /// <returns>删除结果</returns>
    [HttpDelete("{id}")]
    public async Task<ApiResponse<object>> Delete(Guid id)
    {
        var result = await _userService.DeleteAsync(id);
        return result ? Success("删除成功") : Error<object>("删除失败");
    }

    /// <summary>
    /// 重置用户密码
    /// </summary>
    /// <param name="id">用户ID</param>
    /// <param name="dto">重置密码参数</param>
    /// <returns>重置结果</returns>
    [HttpPost("{id}/reset-password")]
    public async Task<ApiResponse<object>> ResetPassword(Guid id, [FromBody] ResetPasswordDto dto)
    {
        var result = await _userService.ResetPasswordAsync(id, dto.NewPassword);
        return result ? Success("密码重置成功") : Error<object>("密码重置失败");
    }

    /// <summary>
    /// 分配用户角色
    /// </summary>
    /// <param name="id">用户ID</param>
    /// <param name="dto">分配角色参数</param>
    /// <returns>分配结果</returns>
    [HttpPost("{id}/assign-roles")]
    public async Task<ApiResponse<object>> AssignRoles(Guid id, [FromBody] AssignRolesDto dto)
    {
        var result = await _userService.AssignRolesAsync(id, dto.RoleIds);
        return result ? Success("角色分配成功") : Error<object>("角色分配失败");
    }

    #region 个人中心

    /// <summary>
    /// 获取个人信息
    /// </summary>
    /// <returns>个人信息</returns>
    /// <remarks>
    /// 获取当前登录用户的个人信息，包括用户名、真实姓名、手机号、邮箱、头像等
    /// </remarks>
    [HttpGet("profile")]
    public async Task<ApiResponse<ProfileDto>> GetProfile()
    {
        var userId = GetCurrentUserId().ToString().ToString();
        var result = await _userService.GetProfileAsync(userId);
        return Success(result);
    }

    /// <summary>
    /// 更新个人信息
    /// </summary>
    /// <param name="dto">更新参数</param>
    /// <returns>更新结果</returns>
    /// <remarks>
    /// 更新当前登录用户的个人信息，可修改真实姓名、手机号、邮箱、头像等
    /// </remarks>
    [HttpPut("profile")]
    public async Task<ApiResponse<object>> UpdateProfile([FromBody] UpdateProfileDto dto)
    {
        var userId = GetCurrentUserId().ToString().ToString();
        var result = await _userService.UpdateProfileAsync(userId, dto);
        return result ? Success("更新成功") : Error<object>("更新失败");
    }

    /// <summary>
    /// 修改密码
    /// </summary>
    /// <param name="dto">修改密码参数</param>
    /// <returns>修改结果</returns>
    /// <remarks>
    /// 修改当前登录用户的密码，需要验证旧密码
    /// </remarks>
    [HttpPost("change-password")]
    public async Task<ApiResponse<object>> ChangePassword([FromBody] ChangePasswordDto dto)
    {
        var userId = GetCurrentUserId().ToString().ToString();
        var result = await _userService.ChangePasswordAsync(userId, dto);
        return result ? Success("密码修改成功") : Error<object>("密码修改失败");
    }

    #endregion
}
