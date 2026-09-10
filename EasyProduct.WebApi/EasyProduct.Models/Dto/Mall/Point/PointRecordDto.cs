using EasyProduct.Models.Enums.Mall;

namespace EasyProduct.Models.Dto.Mall.Point;

/// <summary>
/// 积分流水DTO
/// </summary>
public class PointRecordDto
{
    /// <summary>
    /// 流水ID
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// 会员ID
    /// </summary>
    public string MemberId { get; set; } = string.Empty;

    /// <summary>
    /// 积分类型
    /// </summary>
    public PointType Type { get; set; }

    /// <summary>
    /// 积分来源
    /// </summary>
    public PointSource Source { get; set; }

    /// <summary>
    /// 积分数
    /// </summary>
    public int Points { get; set; }

    /// <summary>
    /// 变动后余额
    /// </summary>
    public int Balance { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    public string? Remark { get; set; }

    /// <summary>
    /// 关联ID
    /// </summary>
    public string? RelatedId { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreateTime { get; set; }
}