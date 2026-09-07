using EasyProduct.Business.Basic;
using EasyProduct.Common.Base;
using EasyProduct.Models.Dto.Basic.Menu;
using EasyProduct.Web.Controllers.Admin.Base;
using Microsoft.AspNetCore.Mvc;

namespace EasyProduct.Web.Controllers.Admin.Basic;

/// <summary>
/// 菜单管理控制器
/// </summary>
/// <remarks>
/// 提供菜单管理的 API 接口，包括菜单树、创建、更新、删除、排序等功能
/// 所有接口需要管理员权限（Admin JWT）
/// </remarks>
public class MenuController : AdminControllerBase
{
    /// <summary>
    /// 菜单服务接口（属性注入）
    /// </summary>
    public IMenuService _menuService { get; set; } = null!;

    /// <summary>
    /// 获取菜单树
    /// </summary>
    /// <returns>菜单树列表</returns>
    /// <remarks>
    /// 返回完整的菜单树结构，用于前端渲染侧边栏菜单
    /// </remarks>
    [HttpGet("tree")]
    public async Task<ApiResponse<List<MenuTreeDto>>> GetTree()
    {
        var result = await _menuService.GetTreeAsync();
        return Success(result);
    }

    /// <summary>
    /// 根据ID获取菜单详情
    /// </summary>
    /// <param name="id">菜单ID</param>
    /// <returns>菜单详情</returns>
    [HttpGet("{id}")]
    public async Task<ApiResponse<MenuDto>> GetById(string id)
    {
        var result = await _menuService.GetByIdAsync(id);
        return Success(result!);
    }

    /// <summary>
    /// 创建菜单
    /// </summary>
    /// <param name="dto">创建菜单参数</param>
    /// <returns>创建结果，返回菜单ID</returns>
    [HttpPost]
    public async Task<ApiResponse<string>> Create([FromBody] CreateMenuDto dto)
    {
        var result = await _menuService.CreateAsync(dto);
        return Success(result, "创建成功");
    }

    /// <summary>
    /// 更新菜单
    /// </summary>
    /// <param name="id">菜单ID</param>
    /// <param name="dto">更新菜单参数</param>
    /// <returns>更新结果</returns>
    [HttpPut("{id}")]
    public async Task<ApiResponse<object>> Update(string id, [FromBody] UpdateMenuDto dto)
    {
        var result = await _menuService.UpdateAsync(id, dto);
        return result ? Success("更新成功") : Error<object>("更新失败");
    }

    /// <summary>
    /// 删除菜单
    /// </summary>
    /// <param name="id">菜单ID</param>
    /// <returns>删除结果</returns>
    [HttpDelete("{id}")]
    public async Task<ApiResponse<object>> Delete(string id)
    {
        var result = await _menuService.DeleteAsync(id);
        return result ? Success("删除成功") : Error<object>("删除失败");
    }

    /// <summary>
    /// 更新菜单排序（拖拽排序）
    /// </summary>
    /// <param name="dto">排序参数</param>
    /// <returns>排序结果</returns>
    [HttpPost("sort")]
    public async Task<ApiResponse<object>> UpdateSort([FromBody] UpdateSortDto dto)
    {
        var result = await _menuService.UpdateSortAsync(dto.DragId, dto.DropId, dto.Type);
        return result ? Success("排序成功") : Error<object>("排序失败");
    }
}

/// <summary>
/// 更新菜单排序请求 DTO
/// </summary>
public class UpdateSortDto
{
    /// <summary>
    /// 被拖拽的菜单ID
    /// </summary>
    public string DragId { get; set; } = string.Empty;

    /// <summary>
    /// 目标位置的菜单ID
    /// </summary>
    public string DropId { get; set; } = string.Empty;

    /// <summary>
    /// 拖拽类型：before=前，after=后，inner=内部
    /// </summary>
    public string Type { get; set; } = string.Empty;
}