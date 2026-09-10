using EasyProduct.Models.Enums.Mall;

namespace EasyProduct.Models.Dto.Mall.Point;

/// <summary>
/// 积分规则查询DTO
/// </summary>
public class PointRuleQueryDto
{
    /// <summary>
    /// 规则名称（模糊查询）
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// 规则类型
    /// </summary>
    public PointRuleType? Type { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    public bool? Status { get; set; }

    /// <summary>
    /// 页码
    /// </summary>
    public int PageIndex { get; set; } = 1;

    /// <summary>
    /// 每页条数
    /// </summary>
    public int PageSize { get; set; } = 10;
}