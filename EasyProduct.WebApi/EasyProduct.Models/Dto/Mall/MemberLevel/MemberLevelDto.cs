namespace EasyProduct.Models.Dto.Mall.MemberLevel;

/// <summary>
/// 会员等级DTO
/// </summary>
/// <remarks>
/// 用于返回会员等级信息，包含所有等级字段
/// </remarks>
public class MemberLevelDto
{
    /// <summary>
    /// 主键ID
    /// </summary>
    /// <remarks>
    /// GUID 字符串格式
    /// </remarks>
    public string Id { get; set; } = null!;

    /// <summary>
    /// 等级名称
    /// </summary>
    public string LevelName { get; set; } = null!;

    /// <summary>
    /// 等级编码
    /// </summary>
    public string LevelCode { get; set; } = null!;

    /// <summary>
    /// 等级数值
    /// </summary>
    /// <remarks>
    /// 等级数值越大，等级越高
    /// </remarks>
    public int Level { get; set; }

    /// <summary>
    /// 最低积分要求
    /// </summary>
    /// <remarks>
    /// 达到该等级所需的最低积分
    /// </remarks>
    public int MinPoints { get; set; }

    /// <summary>
    /// 最高积分上限
    /// </summary>
    /// <remarks>
    /// 该等级的最高积分，超过此积分将升级
    /// </remarks>
    public int MaxPoints { get; set; }

    /// <summary>
    /// 折扣率(0.00-1.00)
    /// </summary>
    /// <remarks>
    /// 该等级会员享受的折扣率，1.00 表示无折扣，0.90 表示 9 折
    /// </remarks>
    public decimal DiscountRate { get; set; }

    /// <summary>
    /// 等级图标
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    /// <remarks>
    /// 使用 MemberStatus 枚举：Disabled（0）= 禁用，Enabled（1）= 启用
    /// </remarks>
    public int Status { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreateTime { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime? UpdateTime { get; set; }
}