using System.ComponentModel.DataAnnotations;

namespace EasyProduct.Models.Dto.Mall.Member;

/// <summary>
/// 会员查询参数
/// </summary>
/// <remarks>
/// 用于会员列表查询，支持按关键词、会员等级、状态筛选
/// 包含分页参数
/// </remarks>
public class MemberQuery
{
    /// <summary>
    /// 页码，从 1 开始
    /// </summary>
    public int PageIndex { get; set; } = 1;

    /// <summary>
    /// 每页数量
    /// </summary>
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// 关键词(昵称/手机号)
    /// </summary>
    /// <remarks>
    /// 支持模糊搜索，最大长度 100 个字符
    /// </remarks>
    [MaxLength(100, ErrorMessage = "关键词长度不能超过100个字符")]
    public string? Keyword { get; set; }

    /// <summary>
    /// 会员等级ID
    /// </summary>
    /// <remarks>
    /// 精确匹配，GUID 格式
    /// </remarks>
    public Guid? LevelId { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    /// <remarks>
    /// 使用 MemberStatus 枚举：Disabled（0）= 禁用，Enabled（1）= 启用
    /// </remarks>
    public int? Status { get; set; }
}