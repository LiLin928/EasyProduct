namespace EasyProduct.Models.Dto.Mall.PointsRecord;

/// <summary>
/// 积分记录查询参数
/// </summary>
/// <remarks>
/// 用于积分记录列表查询，支持按会员ID、积分类型、时间范围筛选
/// 包含分页参数
/// </remarks>
public class PointsRecordQuery
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
    /// 会员ID
    /// </summary>
    /// <remarks>
    /// 精确匹配，GUID 格式
    /// </remarks>
    public Guid? MemberId { get; set; }

    /// <summary>
    /// 积分类型
    /// </summary>
    /// <remarks>
    /// 使用 PointsType 枚举：
    /// ConsumeEarn（1）= 消费获得
    /// OrderUse（2）= 订单使用
    /// AdminAdjust（3）= 后台调整
    /// SignIn（4）= 签到
    /// RegisterGift（5）= 注册赠送
    /// </remarks>
    public int? PointsType { get; set; }

    /// <summary>
    /// 开始时间
    /// </summary>
    /// <remarks>
    /// 查询创建时间大于等于此时间的记录
    /// </remarks>
    public DateTime? StartTime { get; set; }

    /// <summary>
    /// 结束时间
    /// </summary>
    /// <remarks>
    /// 查询创建时间小于等于此时间的记录
    /// </remarks>
    public DateTime? EndTime { get; set; }
}