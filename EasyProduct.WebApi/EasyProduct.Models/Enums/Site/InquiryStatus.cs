namespace EasyProduct.Models.Enums.Site;

/// <summary>
/// 询价单状态枚举
/// </summary>
public enum InquiryStatus
{
    /// <summary>
    /// 待处理
    /// </summary>
    Pending = 0,

    /// <summary>
    /// 已跟进
    /// </summary>
    Followed = 1,

    /// <summary>
    /// 已转客户
    /// </summary>
    Converted = 2,

    /// <summary>
    /// 已关闭
    /// </summary>
    Closed = 3
}