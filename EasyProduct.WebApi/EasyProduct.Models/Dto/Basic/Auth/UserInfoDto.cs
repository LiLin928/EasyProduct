namespace EasyProduct.Models.Dto.Basic.Auth;

/// <summary>
/// 用户信息 DTO
/// </summary>
/// <remarks>
/// 包含用户的基本信息、角色列表和权限列表
/// 用于前端用户信息展示和权限控制
/// </remarks>
public class UserInfoDto
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
    public string? RealName { get; set; }

    /// <summary>
    /// 手机号
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// 邮箱
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// 头像URL
    /// </summary>
    public string? Avatar { get; set; }

    /// <summary>
    /// 部门ID
    /// </summary>
    public string? DeptId { get; set; }

    /// <summary>
    /// 部门名称
    /// </summary>
    public string? DeptName { get; set; }

    /// <summary>
    /// 简介
    /// </summary>
    public string? Introduction { get; set; }

    /// <summary>
    /// 角色列表
    /// </summary>
    /// <remarks>
    /// 用户拥有的角色编码列表，如 ["admin", "user"]
    /// </remarks>
    public List<string> Roles { get; set; } = new();

    /// <summary>
    /// 权限列表
    /// </summary>
    /// <remarks>
    /// 用户拥有的权限标识列表，如 ["basic:user:add", "basic:user:edit"]
    /// </remarks>
    public List<string> Permissions { get; set; } = new();
}