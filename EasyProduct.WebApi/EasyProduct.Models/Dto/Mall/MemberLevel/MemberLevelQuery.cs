using System.ComponentModel.DataAnnotations;
using EasyProduct.Models.Dto.Common;

namespace EasyProduct.Models.Dto.Mall.MemberLevel;

/// <summary>
/// 会员等级查询参数
/// </summary>
/// <remarks>
/// 用于会员等级列表查询，支持按关键词、状态筛选
/// 包含分页参数（继承自 PageQuery）
/// </remarks>
public class MemberLevelQuery : PageQuery
{
    /// <summary>
    /// 关键词(等级名称/等级编码)
    /// </summary>
    /// <remarks>
    /// 支持模糊搜索，最大长度 100 个字符
    /// </remarks>
    [MaxLength(100, ErrorMessage = "关键词长度不能超过100个字符")]
    public string? Keyword { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    /// <remarks>
    /// 使用 MemberStatus 枚举：Disabled（0）= 禁用，Enabled（1）= 启用
    /// </remarks>
    public int? Status { get; set; }
}