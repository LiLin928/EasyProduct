using System.ComponentModel.DataAnnotations;
using EasyProduct.Models.Enums.Mall;

namespace EasyProduct.Models.Dto.Mall.Point;

/// <summary>
/// 更新积分规则DTO
/// </summary>
public class UpdatePointRuleDto
{
    /// <summary>
    /// 规则ID
    /// </summary>
    [Required(ErrorMessage = "规则ID不能为空")]
    [StringLength(36, ErrorMessage = "规则ID长度不能超过36个字符")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// 规则名称
    /// </summary>
    [Required(ErrorMessage = "规则名称不能为空")]
    [StringLength(100, ErrorMessage = "规则名称长度不能超过100个字符")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 规则类型
    /// </summary>
    [Required(ErrorMessage = "规则类型不能为空")]
    public PointRuleType Type { get; set; }

    /// <summary>
    /// 积分数
    /// </summary>
    [Required(ErrorMessage = "积分数不能为空")]
    [Range(1, int.MaxValue, ErrorMessage = "积分数必须大于0")]
    public int Points { get; set; }

    /// <summary>
    /// 是否倍数
    /// </summary>
    public bool IsMultiple { get; set; }

    /// <summary>
    /// 倍数基数
    /// </summary>
    [Range(0.01, double.MaxValue, ErrorMessage = "倍数基数必须大于0")]
    public decimal MultipleBase { get; set; }

    /// <summary>
    /// 最大积分数（单次）
    /// </summary>
    [Range(0, int.MaxValue, ErrorMessage = "最大积分数不能为负数")]
    public int MaxPoints { get; set; }

    /// <summary>
    /// 规则描述
    /// </summary>
    [StringLength(500, ErrorMessage = "规则描述长度不能超过500个字符")]
    public string? Description { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    public bool Status { get; set; }

    /// <summary>
    /// 生效时间
    /// </summary>
    [Required(ErrorMessage = "生效时间不能为空")]
    public DateTime StartTime { get; set; }

    /// <summary>
    /// 失效时间
    /// </summary>
    [Required(ErrorMessage = "失效时间不能为空")]
    public DateTime EndTime { get; set; }

    /// <summary>
    /// 排序号
    /// </summary>
    public int Sort { get; set; }
}