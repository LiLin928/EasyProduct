using System.ComponentModel.DataAnnotations;

namespace EasyProduct.Models.Dto.Mall.MemberLevel;

/// <summary>
/// 会员等级创建DTO
/// </summary>
/// <remarks>
/// 用于创建新的会员等级记录
/// LevelName、LevelCode、Level 为必填字段
/// </remarks>
public class MemberLevelCreateDto
{
    /// <summary>
    /// 等级名称
    /// </summary>
    /// <remarks>
    /// 等级的显示名称，如"黄金会员"、"钻石会员"
    /// 最大长度 50 个字符
    /// </remarks>
    [Required(ErrorMessage = "等级名称不能为空")]
    [MaxLength(50, ErrorMessage = "等级名称长度不能超过50个字符")]
    public string LevelName { get; set; } = null!;

    /// <summary>
    /// 等级编码
    /// </summary>
    /// <remarks>
    /// 等级的唯一编码，如"GOLD"、"DIAMOND"
    /// 最大长度 50 个字符
    /// </remarks>
    [Required(ErrorMessage = "等级编码不能为空")]
    [MaxLength(50, ErrorMessage = "等级编码长度不能超过50个字符")]
    public string LevelCode { get; set; } = null!;

    /// <summary>
    /// 等级数值
    /// </summary>
    /// <remarks>
    /// 等级数值越大，等级越高
    /// 必须大于等于 1
    /// </remarks>
    [Required(ErrorMessage = "等级数值不能为空")]
    [Range(1, int.MaxValue, ErrorMessage = "等级数值必须大于等于1")]
    public int Level { get; set; }

    /// <summary>
    /// 最低积分要求
    /// </summary>
    /// <remarks>
    /// 达到该等级所需的最低积分
    /// 必须大于等于 0
    /// </remarks>
    [Range(0, int.MaxValue, ErrorMessage = "最低积分必须大于等于0")]
    public int MinPoints { get; set; }

    /// <summary>
    /// 最高积分上限
    /// </summary>
    /// <remarks>
    /// 该等级的最高积分，超过此积分将升级
    /// 必须大于等于最低积分要求
    /// </remarks>
    [Range(0, int.MaxValue, ErrorMessage = "最高积分必须大于等于0")]
    public int MaxPoints { get; set; }

    /// <summary>
    /// 折扣率(0.00-1.00)
    /// </summary>
    /// <remarks>
    /// 该等级会员享受的折扣率
    /// 1.00 表示无折扣，0.90 表示 9 折
    /// 默认值为 1.00
    /// </remarks>
    [Range(0.00, 1.00, ErrorMessage = "折扣率必须在0.00到1.00之间")]
    public decimal DiscountRate { get; set; } = 1.00m;

    /// <summary>
    /// 等级图标
    /// </summary>
    /// <remarks>
    /// 等级图标的 URL 地址
    /// 最大长度 255 个字符
    /// </remarks>
    [MaxLength(255, ErrorMessage = "等级图标URL长度不能超过255个字符")]
    public string? Icon { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    /// <remarks>
    /// 使用 MemberStatus 枚举：Disabled（0）= 禁用，Enabled（1）= 启用
    /// 默认为启用状态
    /// </remarks>
    public int Status { get; set; } = 1;
}