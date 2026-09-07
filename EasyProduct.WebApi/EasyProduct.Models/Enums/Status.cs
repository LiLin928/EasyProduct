namespace EasyProduct.Models.Enums;

/// <summary>
/// 通用状态枚举
/// </summary>
/// <remarks>
/// 用于所有实体的 Status 字段
/// 0=禁用，1=启用
/// </remarks>
public enum Status
{
    /// <summary>
    /// 禁用
    /// </summary>
    Disabled = 0,

    /// <summary>
    /// 启用
    /// </summary>
    Enabled = 1
}

/// <summary>
/// 可见性枚举
/// </summary>
/// <remarks>
/// 用于菜单、按钮等是否可见字段
/// 0=否，1=是
/// </remarks>
public enum Visible
{
    /// <summary>
    /// 否（不可见）
    /// </summary>
    No = 0,

    /// <summary>
    /// 是（可见）
    /// </summary>
    Yes = 1
}