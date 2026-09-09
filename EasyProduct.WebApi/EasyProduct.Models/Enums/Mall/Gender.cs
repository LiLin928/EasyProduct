namespace EasyProduct.Models.Enums.Mall;

/// <summary>
/// 性别枚举
/// </summary>
/// <remarks>
/// 用于会员的性别字段：
/// - Unknown: 未知或未填写
/// - Male: 男性
/// - Female: 女性
/// </remarks>
public enum Gender
{
    /// <summary>
    /// 未知
    /// </summary>
    /// <remarks>
    /// 性别未知或未填写
    /// </remarks>
    Unknown = 0,

    /// <summary>
    /// 男
    /// </summary>
    /// <remarks>
    /// 男性
    /// </remarks>
    Male = 1,

    /// <summary>
    /// 女
    /// </summary>
    /// <remarks>
    /// 女性
    /// </remarks>
    Female = 2
}