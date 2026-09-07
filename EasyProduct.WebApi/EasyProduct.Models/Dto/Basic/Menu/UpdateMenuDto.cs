using System.ComponentModel.DataAnnotations;
using EasyProduct.Models.Enums;

namespace EasyProduct.Models.Dto.Basic.Menu;

/// <summary>
/// 更新菜单请求 DTO
/// </summary>
/// <remarks>
/// 用于更新菜单信息，所有字段可选
/// </remarks>
public class UpdateMenuDto
{
    /// <summary>
    /// 父级菜单ID
    /// </summary>
    public string? ParentId { get; set; }

    /// <summary>
    /// 菜单名称
    /// </summary>
    [StringLength(50, MinimumLength = 2, ErrorMessage = "菜单名称长度必须在2-50个字符之间")]
    public string? MenuName { get; set; }

    /// <summary>
    /// 菜单编码
    /// </summary>
    [StringLength(50, ErrorMessage = "菜单编码长度不能超过50个字符")]
    [RegularExpression(@"^[a-zA-Z0-9_:]*$", ErrorMessage = "菜单编码只能包含字母、数字、下划线和冒号")]
    public string? MenuCode { get; set; }

    /// <summary>
    /// 路由路径
    /// </summary>
    [StringLength(200, ErrorMessage = "路由路径长度不能超过200个字符")]
    public string? Path { get; set; }

    /// <summary>
    /// 组件路径
    /// </summary>
    [StringLength(200, ErrorMessage = "组件路径长度不能超过200个字符")]
    public string? Component { get; set; }

    /// <summary>
    /// 权限标识
    /// </summary>
    [StringLength(100, ErrorMessage = "权限标识长度不能超过100个字符")]
    public string? Permission { get; set; }

    /// <summary>
    /// 图标
    /// </summary>
    [StringLength(50, ErrorMessage = "图标长度不能超过50个字符")]
    public string? Icon { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    [Range(0, 9999, ErrorMessage = "排序值必须在0-9999之间")]
    public int? Sort { get; set; }

    /// <summary>
    /// 菜单类型：0=目录，1=菜单，2=按钮
    /// </summary>
    [Range(0, 2, ErrorMessage = "菜单类型必须是0（目录）、1（菜单）或2（按钮）")]
    public int? Type { get; set; }

    /// <summary>
    /// 是否可见：0=否，1=是
    /// </summary>
    public Visible? Visible { get; set; }

    /// <summary>
    /// 是否外链：0=否，1=是
    /// </summary>
    [Range(0, 1, ErrorMessage = "是否外链必须是0（否）或1（是）")]
    public int? IsExternal { get; set; }

    /// <summary>
    /// 是否缓存：0=否，1=是
    /// </summary>
    [Range(0, 1, ErrorMessage = "是否缓存必须是0（否）或1（是）")]
    public int? IsCache { get; set; }

    /// <summary>
    /// 状态：0=禁用，1=启用
    /// </summary>
    public Status? Status { get; set; }
}