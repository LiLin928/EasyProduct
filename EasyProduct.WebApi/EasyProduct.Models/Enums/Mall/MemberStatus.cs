namespace EasyProduct.Models.Enums.Mall;

/// <summary>
/// 会员状态枚举
/// </summary>
/// <remarks>
/// 用于会员的状态标识：
/// - Disabled: 禁用状态，会员无法登录和使用服务
/// - Enabled: 启用状态，会员正常使用所有功能
/// </remarks>
public enum MemberStatus
{
    /// <summary>
    /// 禁用
    /// </summary>
    /// <remarks>
    /// 会员被禁用，无法登录和使用服务
    /// </remarks>
    Disabled = 0,

    /// <summary>
    /// 启用
    /// </summary>
    /// <remarks>
    /// 会员正常状态，可以使用所有功能
    /// </remarks>
    Enabled = 1
}