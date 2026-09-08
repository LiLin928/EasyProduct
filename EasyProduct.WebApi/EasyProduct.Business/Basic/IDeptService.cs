using EasyProduct.Models.Dto.Basic.Dept;

namespace EasyProduct.Business.Basic;

/// <summary>
/// 部门服务接口
/// </summary>
/// <remarks>
/// 提供部门的增删改查、树形结构查询等功能
/// </remarks>
public interface IDeptService
{
    /// <summary>
    /// 获取部门树形结构
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>部门树形列表</returns>
    Task<List<DeptTreeDto>> GetDeptTreeAsync(DeptTreeQueryDto query);

    /// <summary>
    /// 获取部门列表（扁平）
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>部门列表</returns>
    Task<List<DeptDto>> GetDeptListAsync(DeptQueryDto query);

    /// <summary>
    /// 获取部门详情
    /// </summary>
    /// <param name="id">部门ID</param>
    /// <returns>部门详情</returns>
    Task<DeptDto> GetDeptByIdAsync(string id);

    /// <summary>
    /// 创建部门
    /// </summary>
    /// <param name="dto">创建参数</param>
    /// <returns>新部门ID</returns>
    Task<string> CreateDeptAsync(CreateDeptDto dto);

    /// <summary>
    /// 更新部门
    /// </summary>
    /// <param name="dto">更新参数</param>
    /// <returns>是否成功</returns>
    Task<bool> UpdateDeptAsync(UpdateDeptDto dto);

    /// <summary>
    /// 删除部门
    /// </summary>
    /// <param name="id">部门ID</param>
    /// <returns>是否成功</returns>
    Task<bool> DeleteDeptAsync(string id);

    /// <summary>
    /// 更新部门状态
    /// </summary>
    /// <param name="id">部门ID</param>
    /// <param name="status">状态：0=禁用，1=启用</param>
    /// <returns>是否成功</returns>
    Task<bool> UpdateDeptStatusAsync(string id, int status);

    /// <summary>
    /// 获取部门下的用户列表
    /// </summary>
    /// <param name="deptId">部门ID</param>
    /// <returns>用户列表</returns>
    Task<List<DeptUserDto>> GetDeptUsersAsync(string deptId);
}

/// <summary>
/// 部门用户DTO
/// </summary>
public class DeptUserDto
{
    /// <summary>
    /// 用户ID
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// 用户名
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// 真实姓名
    /// </summary>
    public string RealName { get; set; } = string.Empty;

    /// <summary>
    /// 邮箱
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// 手机号
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    public int Status { get; set; }
}