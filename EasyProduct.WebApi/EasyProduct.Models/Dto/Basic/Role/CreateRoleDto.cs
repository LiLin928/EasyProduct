using System.ComponentModel.DataAnnotations;
using EasyProduct.Models.Enums;

namespace EasyProduct.Models.Dto.Basic;

/// <summary>
/// 创建角色请求 DTO
/// </summary>
public class CreateRoleDto
{
    /// <summary>
    /// 角色名称
    /// </summary>
    [Required(ErrorMessage = "角色名称不能为空")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "角色名称长度必须在2-50个字符之间")]
    public string RoleName { get; set; } = string.Empty;

    /// <summary>
    /// 角色编码
    /// </summary>
    [Required(ErrorMessage = "角色编码不能为空")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "角色编码长度必须在2-50个字符之间")]
    [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "角色编码只能包含字母、数字和下划线")]
    public string RoleCode { get; set; } = string.Empty;

    /// <summary>
    /// 状态：0=禁用，1=启用
    /// </summary>
    public Status Status { get; set; } = Status.Enabled;

    /// <summary>
    /// 排序
    /// </summary>
    [Range(0, 9999, ErrorMessage = "排序值必须在0-9999之间")]
    public int Sort { get; set; } = 0;

    /// <summary>
    /// 描述
    /// </summary>
    [StringLength(200, ErrorMessage = "描述长度不能超过200个字符")]
    public string? Description { get; set; }

    /// <summary>
    /// 菜单ID列表
    /// </summary>
    public List<string>? MenuIds { get; set; }
}