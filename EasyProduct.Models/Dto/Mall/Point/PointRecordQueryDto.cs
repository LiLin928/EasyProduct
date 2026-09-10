using EasyProduct.Models.Enums.Mall;

namespace EasyProduct.Models.Dto.Mall.Point;

/// <summary>
/// 积分流水查询DTO
/// </summary>
public class PointRecordQueryDto
{
    /// <summary>
    /// 会员ID
    /// </summary>
    public string? MemberId { get; set; }

    /// <summary>
    /// 积分类型
    /// </summary>
    public PointType? Type { get; set; }

    /// <summary>
    /// 积分来源
    /// </summary>
    public PointSource? Source { get; set; }

    /// <summary>
    /// 开始时间
    /// </summary>
    public DateTime? StartTime { get; set; }

    /// <summary>
    /// 结束时间
    /// </summary>
    public DateTime? EndTime { get; set; }

    /// <summary>
    /// 页码
    /// </summary>
    public int PageIndex { get; set; } = 1;

    /// <summary>
    /// 每页条数
    /// </summary>
    public int PageSize { get; set; } = 10;
}