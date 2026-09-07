using SqlSugar;
using EasyProduct.Models.Entitys.Base;
using EasyProduct.Models.Enums;

namespace EasyProduct.Models.Entitys.Basic;

/// <summary>
/// 菜单实体
/// </summary>
/// <remarks>
/// 对应数据库表 basic_menu
/// 继承 BaseEntity 基类，自动包含 Id、IsDeleted、Status、CreatedAt、UpdatedAt、CreatedBy、UpdatedBy 字段
/// 用于存储系统菜单信息，支持树形结构的菜单和按钮权限配置
/// </remarks>
[SugarTable("basic_menu", "菜单表")]
public class basic_menu : BaseEntity
{
    /// <summary>
    /// 父级菜单ID
    /// </summary>
    /// <remarks>
    /// 父级菜单ID，根菜单为空字符串或"0"
    /// 用于构建菜单树形结构
    /// </remarks>
    [SugarColumn(ColumnDataType = "varchar(36)", ColumnDescription = "父级菜单ID")]
    public string ParentId { get; set; } = string.Empty;

    /// <summary>
    /// 菜单名称
    /// </summary>
    /// <remarks>
    /// 菜单的显示名称/标题
    /// </remarks>
    [SugarColumn(Length = 50, ColumnDescription = "菜单名称")]
    public string MenuName { get; set; } = string.Empty;

    /// <summary>
    /// 菜单编码
    /// </summary>
    /// <remarks>
    /// 菜单的唯一编码标识，用于路由 name 属性和权限控制
    /// 如：SystemUser、SystemRole
    /// </remarks>
    [SugarColumn(Length = 50, IsNullable = true, ColumnDescription = "菜单编码")]
    public string? MenuCode { get; set; }

    /// <summary>
    /// 路由路径
    /// </summary>
    /// <remarks>
    /// 前端路由路径，如：/system/user
    /// </remarks>
    [SugarColumn(Length = 200, IsNullable = true, ColumnDescription = "路由路径")]
    public string? Path { get; set; }

    /// <summary>
    /// 组件路径
    /// </summary>
    /// <remarks>
    /// 前端组件路径，相对于 views 目录
    /// 如：basic/user/index
    /// </remarks>
    [SugarColumn(Length = 200, IsNullable = true, ColumnDescription = "组件路径")]
    public string? Component { get; set; }

    /// <summary>
    /// 权限标识
    /// </summary>
    /// <remarks>
    /// 权限标识字符串，用于按钮级权限控制
    /// 格式：模块:页面:操作，如：basic:user:add
    /// </remarks>
    [SugarColumn(Length = 100, IsNullable = true, ColumnDescription = "权限标识")]
    public string? Permission { get; set; }

    /// <summary>
    /// 图标
    /// </summary>
    /// <remarks>
    /// 菜单图标名称，如：Setting、User
    /// </remarks>
    [SugarColumn(Length = 50, IsNullable = true, ColumnDescription = "图标")]
    public string? Icon { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    /// <remarks>
    /// 菜单显示顺序，数值越小越靠前
    /// </remarks>
    [SugarColumn(ColumnDescription = "排序")]
    public int Sort { get; set; } = 0;

    /// <summary>
    /// 菜单类型
    /// </summary>
    /// <remarks>
    /// 菜单类型：
    /// 0=目录（可展开的菜单组）
    /// 1=菜单（具体的页面）
    /// 2=按钮（具体的操作权限）
    /// </remarks>
    [SugarColumn(ColumnDescription = "菜单类型：0=目录，1=菜单，2=按钮")]
    public int Type { get; set; } = 1;

    /// <summary>
    /// 是否可见
    /// </summary>
    /// <remarks>
    /// 是否在菜单中显示：
    /// 0=不可见
    /// 1=可见
    /// 不可见的菜单不在侧边栏显示，但路由仍可访问
    /// </remarks>
    [SugarColumn(ColumnDescription = "是否可见：0=否，1=是")]
    public Visible Visible { get; set; } = Visible.Yes;

    /// <summary>
    /// 是否外链
    /// </summary>
    /// <remarks>
    /// 是否为外部链接：
    /// 0=否
    /// 1=是
    /// 外链会在新标签页打开
    /// </remarks>
    [SugarColumn(ColumnDescription = "是否外链：0=否，1=是")]
    public int IsExternal { get; set; } = 0;

    /// <summary>
    /// 是否缓存
    /// </summary>
    /// <remarks>
    /// 是否缓存页面状态：
    /// 0=不缓存
    /// 1=缓存
    /// 开启后会 keep-alive
    /// </remarks>
    [SugarColumn(ColumnDescription = "是否缓存：0=否，1=是")]
    public int IsCache { get; set; } = 0;
}