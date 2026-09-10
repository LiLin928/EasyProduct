using EasyProduct.Models.Enums.Mall;

namespace EasyProduct.Models.Dto.Mall.Point;

/// <summary>
/// 积分规则DTO
/// </summary>
public class PointRuleDto
{
    /// <summary>
    /// 规则ID
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// 规则名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 规则类型
    /// </summary>
    public PointRuleType Type { get; set; }

    /// <summary>
    /// 积分数
    /// </summary>
    public int Points { get; set; }

    /// <summary>
    /// 是否倍数
    /// </summary>
    public bool IsMultiple { get; set; }

    /// <summary>
    /// 倍数基数
    /// </summary>
    public decimal MultipleBase { get; set; }

    /// <summary>
    /// 最大积分数（单次）
    /// </summary>
    public int MaxPoints { get; set; }

    /// <summary>
    /// 规则描述
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    public bool Status { get; set; }

    /// <summary>
    /// 生效时间
    /// </summary>
    public DateTime StartTime { get; set; }

    /// <summary>
    /// 失效时间
    /// </summary>
    public DateTime EndTime { get; set; }

    /// <summary>
    /// 排序号
    /// </summary>
    public int Sort { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreateTime { get; set; }
}