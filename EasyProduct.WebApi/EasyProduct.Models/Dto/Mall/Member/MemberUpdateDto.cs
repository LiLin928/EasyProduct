using System.ComponentModel.DataAnnotations;

namespace EasyProduct.Models.Dto.Mall.Member;

/// <summary>
/// 会员更新DTO
/// </summary>
/// <remarks>
/// 用于更新会员信息
/// 所有字段均为可选，只更新传入的字段
/// </remarks>
public class MemberUpdateDto
{
    /// <summary>
    /// 昵称
    /// </summary>
    /// <remarks>
    /// 最大长度 100 个字符
    /// </remarks>
    [MaxLength(100, ErrorMessage = "昵称长度不能超过100个字符")]
    public string? Nickname { get; set; }

    /// <summary>
    /// 头像URL
    /// </summary>
    /// <remarks>
    /// 最大长度 500 个字符
    /// </remarks>
    [MaxLength(500, ErrorMessage = "头像URL长度不能超过500个字符")]
    public string? Avatar { get; set; }

    /// <summary>
    /// 性别
    /// </summary>
    /// <remarks>
    /// 使用 Gender 枚举：Unknown（0）= 未知，Male（1）= 男，Female（2）= 女
    /// </remarks>
    public int? Gender { get; set; }

    /// <summary>
    /// 手机号
    /// </summary>
    /// <remarks>
    /// 最大长度 20 个字符
    /// 必须符合中国大陆手机号格式：1 开头，共 11 位
    /// </remarks>
    [MaxLength(20, ErrorMessage = "手机号长度不能超过20个字符")]
    [RegularExpression(@"^1[3-9]\d{9}$", ErrorMessage = "手机号格式不正确")]
    public string? Phone { get; set; }

    /// <summary>
    /// 真实姓名
    /// </summary>
    /// <remarks>
    /// 最大长度 50 个字符
    /// </remarks>
    [MaxLength(50, ErrorMessage = "真实姓名长度不能超过50个字符")]
    public string? RealName { get; set; }

    /// <summary>
    /// 生日
    /// </summary>
    public DateTime? Birthday { get; set; }

    /// <summary>
    /// 邮箱
    /// </summary>
    /// <remarks>
    /// 最大长度 100 个字符
    /// 必须符合邮箱格式
    /// </remarks>
    [MaxLength(100, ErrorMessage = "邮箱长度不能超过100个字符")]
    [EmailAddress(ErrorMessage = "邮箱格式不正确")]
    public string? Email { get; set; }

    /// <summary>
    /// 会员等级ID
    /// </summary>
    /// <remarks>
    /// 关联 mall_member_level 表的 Id 字段
    /// GUID 格式
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