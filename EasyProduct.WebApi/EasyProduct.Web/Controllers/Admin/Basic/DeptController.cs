using EasyProduct.Business.Basic;
using EasyProduct.Common.Base;
using EasyProduct.Models.Dto.Basic.Dept;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyProduct.Web.Controllers.Admin.Basic;

/// <summary>
/// 部门管理控制器
/// </summary>
/// <remarks>
/// 提供部门的增删改查、树形结构查询等功能
/// 管理端接口，需要 Admin JWT 认证
/// </remarks>
[ApiController]
[Route("api/admin/basic/dept")]
[Authorize(AuthenticationSchemes = "AdminJwt")]
public class DeptController : BaseController
{
    private readonly IDeptService _deptService;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="deptService">部门服务</param>
    public DeptController(IDeptService deptService)
    {
        _deptService = deptService;
    }

    /// <summary>
    /// 获取部门树形结构
    /// </summary>
    /// <param name="parentId">父部门ID（可选，不传则查询全部）</param>
    /// <param name="onlyEnabled">是否只查询启用的部门</param>
    /// <returns>部门树形列表</returns>
    /// <remarks>
    /// 获取部门的树形结构，用于构建部门树
    /// 支持按父部门ID筛选，支持只查询启用的部门
    /// </remarks>
    [HttpGet("tree")]
    public async Task<ApiResponse<List<DeptTreeDto>>> GetDeptTree([FromQuery] string? parentId, [FromQuery] bool? onlyEnabled)
    {
        var query = new DeptTreeQueryDto
        {
            ParentId = parentId,
            OnlyEnabled = onlyEnabled
        };

        var result = await _deptService.GetDeptTreeAsync(query);
        return Success(result);
    }

    /// <summary>
    /// 获取部门列表（扁平）
    /// </summary>
    /// <param name="name">部门名称（模糊搜索）</param>
    /// <param name="code">部门编码（模糊搜索）</param>
    /// <param name="status">状态：0=禁用，1=启用</param>
    /// <returns>部门列表</returns>
    /// <remarks>
    /// 获取部门的扁平列表，支持按名称、编码、状态筛选
    /// </remarks>
    [HttpGet("list")]
    public async Task<ApiResponse<List<DeptDto>>> GetDeptList([FromQuery] string? name, [FromQuery] string? code, [FromQuery] int? status)
    {
        var query = new DeptQueryDto
        {
            Name = name,
            Code = code,
            Status = status
        };

        var result = await _deptService.GetDeptListAsync(query);
        return Success(result);
    }

    /// <summary>
    /// 获取部门详情
    /// </summary>
    /// <param name="id">部门ID</param>
    /// <returns>部门详情</returns>
    /// <remarks>
    /// 根据ID获取部门的详细信息
    /// </remarks>
    [HttpGet("{id}")]
    public async Task<ApiResponse<DeptDto>> GetDeptById(string id)
    {
        var result = await _deptService.GetDeptByIdAsync(id);
        return Success(result!);
    }

    /// <summary>
    /// 创建部门
    /// </summary>
    /// <param name="dto">创建参数</param>
    /// <returns>新部门ID</returns>
    /// <remarks>
    /// 创建新的部门
    /// </remarks>
    [HttpPost]
    public async Task<ApiResponse<string>> CreateDept([FromBody] CreateDeptDto dto)
    {
        var result = await _deptService.CreateDeptAsync(dto);
        return Success(result, "部门创建成功");
    }

    /// <summary>
    /// 更新部门
    /// </summary>
    /// <param name="id">部门ID</param>
    /// <param name="dto">更新参数</param>
    /// <returns>是否成功</returns>
    /// <remarks>
    /// 更新部门信息
    /// </remarks>
    [HttpPut("{id}")]
    public async Task<ApiResponse<bool>> UpdateDept(string id, [FromBody] UpdateDeptDto dto)
    {
        dto.Id = id;
        var result = await _deptService.UpdateDeptAsync(dto);
        return Success(result, "部门更新成功");
    }

    /// <summary>
    /// 删除部门
    /// </summary>
    /// <param name="id">部门ID</param>
    /// <returns>是否成功</returns>
    /// <remarks>
    /// 删除部门（软删除）
    /// 注意：如果部门下存在子部门或用户，则不能删除
    /// </remarks>
    [HttpDelete("{id}")]
    public async Task<ApiResponse<bool>> DeleteDept(string id)
    {
        var result = await _deptService.DeleteDeptAsync(id);
        return Success(result, "部门删除成功");
    }

    /// <summary>
    /// 更新部门状态
    /// </summary>
    /// <param name="id">部门ID</param>
    /// <param name="status">状态：0=禁用，1=启用</param>
    /// <returns>是否成功</returns>
    /// <remarks>
    /// 启用或禁用部门
    /// </remarks>
    [HttpPut("{id}/status")]
    public async Task<ApiResponse<bool>> UpdateDeptStatus(string id, [FromQuery] int status)
    {
        var result = await _deptService.UpdateDeptStatusAsync(id, status);
        return Success(result, "部门状态更新成功");
    }

    /// <summary>
    /// 获取部门下的用户列表
    /// </summary>
    /// <param name="id">部门ID</param>
    /// <returns>用户列表</returns>
    /// <remarks>
    /// 获取指定部门下的所有用户
    /// </remarks>
    [HttpGet("{id}/users")]
    public async Task<ApiResponse<List<DeptUserDto>>> GetDeptUsers(string id)
    {
        var result = await _deptService.GetDeptUsersAsync(id);
        return Success(result);
    }
}