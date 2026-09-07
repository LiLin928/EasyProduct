using System.ComponentModel.DataAnnotations;
using EasyProduct.Models.Enums;

namespace EasyProduct.Models.Dto.Basic;

/// <summary>
/// 用户 DTO
/// </summary>
public class UserDto
{
    /// <summary>
    /// 用户ID
    /// </summary>
    public Guid Id { get; set; }

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
    /// 状态：0=禁用，1=启用
    /// </summary>
    public Status Status { get; set; }

    /// <summary>
    /// 是否删除：0=未删除，1=已删除
    /// </summary>
    public int IsDeleted { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// 创建人ID
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// 更新人ID
    /// </summary>
    public string? UpdatedBy { get; set; }

    /// <summary>
    /// 角色列表
    /// </summary>
    public List<UserRoleDto>? Roles { get; set; }
}

/// <summary>
/// 用户角色 DTO（简化版，用于用户列表显示）
/// </summary>
public class UserRoleDto
{
    /// <summary>
    /// 角色ID
    /// </summary>
    public string RoleId { get; set; } = string.Empty;

    /// <summary>
    /// 角色名称
    /// </summary>
    public string RoleName { get; set; } = string.Empty;

    /// <summary>
    /// 角色编码
    /// </summary>
    public string RoleCode { get; set; } = string.Empty;
}

/// <summary>
/// 创建用户请求 DTO
/// </summary>
public class CreateUserDto
{
    /// <summary>
    /// 用户名
    /// </summary>
    [Required(ErrorMessage = "用户名不能为空")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "用户名长度必须在3-50个字符之间")]
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// 密码
    /// </summary>
    [Required(ErrorMessage = "密码不能为空")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "密码长度必须在6-100个字符之间")]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// 真实姓名
    /// </summary>
    [StringLength(50, ErrorMessage = "真实姓名长度不能超过50个字符")]
    public string? RealName { get; set; }

    /// <summary>
    /// 手机号
    /// </summary>
    [RegularExpression(@"^1[3-9]\d{9}$", ErrorMessage = "手机号格式不正确")]
    public string? Phone { get; set; }

    /// <summary>
    /// 邮箱
    /// </summary>
    [EmailAddress(ErrorMessage = "邮箱格式不正确")]
    public string? Email { get; set; }

    /// <summary>
    /// 部门ID
    /// </summary>
    public string? DeptId { get; set; }

    /// <summary>
    /// 状态：0=禁用，1=启用
    /// </summary>
    public Status Status { get; set; } = Status.Enabled;

    /// <summary>
    /// 角色ID列表
    /// </summary>
    public List<string>? RoleIds { get; set; }
}

/// <summary>
/// 更新用户请求 DTO
/// </summary>
public class UpdateUserDto
{
    /// <summary>
    /// 用户ID
    /// </summary>
    [Required(ErrorMessage = "用户ID不能为空")]
    public Guid Id { get; set; }

    /// <summary>
    /// 密码（可选，不修改密码时不传）
    /// </summary>
    [StringLength(100, MinimumLength = 6, ErrorMessage = "密码长度必须在6-100个字符之间")]
    public string? Password { get; set; }

    /// <summary>
    /// 真实姓名
    /// </summary>
    [StringLength(50, ErrorMessage = "真实姓名长度不能超过50个字符")]
    public string? RealName { get; set; }

    /// <summary>
    /// 手机号
    /// </summary>
    [RegularExpression(@"^1[3-9]\d{9}$", ErrorMessage = "手机号格式不正确")]
    public string? Phone { get; set; }

    /// <summary>
    /// 邮箱
    /// </summary>
    [EmailAddress(ErrorMessage = "邮箱格式不正确")]
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
    /// 状态：0=禁用，1=启用
    /// </summary>
    public Status? Status { get; set; }

    /// <summary>
    /// 角色ID列表（可选，不修改角色时不传）
    /// </summary>
    public List<string>? RoleIds { get; set; }
}

/// <summary>
/// 用户查询 DTO
/// </summary>
public class UserQueryDto
{
    /// <summary>
    /// 页码，从 1 开始
    /// </summary>
    public int PageIndex { get; set; } = 1;

    /// <summary>
    /// 每页数量
    /// </summary>
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// 用户名关键词
    /// </summary>
    public string? Keyword { get; set; }

    /// <summary>
    /// 状态筛选：0=禁用，1=启用
    /// </summary>
    public Status? Status { get; set; }

    /// <summary>
    /// 部门ID筛选
    /// </summary>
    public string? DeptId { get; set; }
}

/// <summary>
/// 重置密码请求 DTO
/// </summary>
public class ResetPasswordDto
{
    /// <summary>
    /// 新密码
    /// </summary>
    [Required(ErrorMessage = "新密码不能为空")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "密码长度必须在6-100个字符之间")]
    public string NewPassword { get; set; } = string.Empty;
}

/// <summary>
/// 分配角色请求 DTO
/// </summary>
public class AssignRolesDto
{
    /// <summary>
    /// 角色ID列表
    /// </summary>
    [Required(ErrorMessage = "角色ID列表不能为空")]
    public List<string> RoleIds { get; set; } = new List<string>();
}