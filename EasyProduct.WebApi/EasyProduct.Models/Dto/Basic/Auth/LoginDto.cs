using System.ComponentModel.DataAnnotations;

namespace EasyProduct.Models.Dto.Basic.Auth;

/// <summary>
/// 登录请求 DTO
/// </summary>
/// <remarks>
/// 用于用户登录认证，包含用户名和密码
/// </remarks>
public class LoginDto
{
    /// <summary>
    /// 用户名
    /// </summary>
    /// <remarks>
    /// 用户登录名，必填，长度限制 1-50 字符
    /// </remarks>
    [Required(ErrorMessage = "用户名不能为空")]
    [StringLength(50, MinimumLength = 1, ErrorMessage = "用户名长度必须在 1-50 个字符之间")]
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// 密码
    /// </summary>
    /// <remarks>
    /// 用户登录密码，必填，长度限制 6-50 字符
    /// </remarks>
    [Required(ErrorMessage = "密码不能为空")]
    [StringLength(50, MinimumLength = 6, ErrorMessage = "密码长度必须在 6-50 个字符之间")]
    public string Password { get; set; } = string.Empty;
}