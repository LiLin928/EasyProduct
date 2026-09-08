using System.ComponentModel.DataAnnotations;

namespace EasyProduct.Models.Dto.Basic.Profile;

/// <summary>
/// 个人信息 DTO
/// </summary>
/// <remarks>
/// 用于展示当前登录用户的个人信息
/// </remarks>
public class ProfileDto
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
    /// 状态：0=禁用，1=启用
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// 更新个人信息请求 DTO
/// </summary>
/// <remarks>
/// 用于更新当前登录用户的个人信息
/// </remarks>
public class UpdateProfileDto
{
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
    [StringLength(500, ErrorMessage = "头像URL长度不能超过500个字符")]
    public string? Avatar { get; set; }
}

/// <summary>
/// 修改密码请求 DTO
/// </summary>
/// <remarks>
/// 用于修改当前登录用户的密码
/// </remarks>
public class ChangePasswordDto
{
    /// <summary>
    /// 旧密码
    /// </summary>
    [Required(ErrorMessage = "旧密码不能为空")]
    public string OldPassword { get; set; } = string.Empty;

    /// <summary>
    /// 新密码
    /// </summary>
    [Required(ErrorMessage = "新密码不能为空")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "密码长度必须在6-100个字符之间")]
    public string NewPassword { get; set; } = string.Empty;

    /// <summary>
    /// 确认新密码
    /// </summary>
    [Required(ErrorMessage = "确认密码不能为空")]
    [Compare("NewPassword", ErrorMessage = "两次输入的密码不一致")]
    public string ConfirmPassword { get; set; } = string.Empty;
}