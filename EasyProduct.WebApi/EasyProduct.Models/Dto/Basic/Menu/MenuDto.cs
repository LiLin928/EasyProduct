using EasyProduct.Models.Enums;

namespace EasyProduct.Models.Dto.Basic.Menu;

/// <summary>
/// 菜单 DTO
/// </summary>
/// <remarks>
/// 用于返回菜单信息，包含所有菜单字段
/// </remarks>
public class MenuDto
{
    /// <summary>
    /// 菜单ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 父级菜单ID
    /// </summary>
    public string ParentId { get; set; } = string.Empty;

    /// <summary>
    /// 菜单名称
    /// </summary>
    public string MenuName { get; set; } = string.Empty;

    /// <summary>
    /// 菜单编码
    /// </summary>
    public string? MenuCode { get; set; }

    /// <summary>
    /// 路由路径
    /// </summary>
    public string? Path { get; set; }

    /// <summary>
    /// 组件路径
    /// </summary>
    public string? Component { get; set; }

    /// <summary>
    /// 权限标识
    /// </summary>
    public string? Permission { get; set; }

    /// <summary>
    /// 图标
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    public int Sort { get; set; }

    /// <summary>
    /// 菜单类型：0=目录，1=菜单，2=按钮
    /// </summary>
    public int Type { get; set; }

    /// <summary>
    /// 是否可见：0=否，1=是
    /// </summary>
    public Visible Visible { get; set; }

    /// <summary>
    /// 是否外链：0=否，1=是
    /// </summary>
    public int IsExternal { get; set; }

    /// <summary>
    /// 是否缓存：0=否，1=是
    /// </summary>
    public int IsCache { get; set; }

    /// <summary>
    /// 状态：0=禁用，1=启用
    /// </summary>
    public Status Status { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// 子菜单列表（用于树形结构）
    /// </summary>
    public List<MenuDto>? Children { get; set; }
}