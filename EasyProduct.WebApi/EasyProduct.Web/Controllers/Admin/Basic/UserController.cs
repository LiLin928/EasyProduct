using EasyProduct.Common.Base;
using EasyProduct.Models.Dto.Basic;
using EasyProduct.Business.Basic;
using EasyProduct.Web.Controllers.Admin.Base;
using Microsoft.AspNetCore.Mvc;

namespace EasyProduct.Web.Controllers.Admin.Basic;

/// <summary>
/// 用户管理控制器
/// </summary>
/// <remarks>
/// 提供用户管理的 CRUD 接口
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
    public async Task<ApiResponse<UserDto?>> GetById(Guid id)
    {
        var result = await _userService.GetByIdAsync(id);
        return Success(result);
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
}
