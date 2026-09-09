namespace EasyProduct.Models.Dto.Mall.PointsRecord;

/// <summary>
/// 积分记录DTO
/// </summary>
/// <remarks>
/// 用于返回积分记录信息，包含所有积分记录字段
/// 包含会员昵称和积分类型名称用于显示
/// </remarks>
public class PointsRecordDto
{
    /// <summary>
    /// 主键ID
    /// </summary>
    /// <remarks>
    /// GUID 字符串格式
    /// </remarks>
    public string Id { get; set; } = null!;

    /// <summary>
    /// 会员ID
    /// </summary>
    /// <remarks>
    /// GUID 字符串格式
    /// </remarks>
    public string MemberId { get; set; } = null!;

    /// <summary>
    /// 会员昵称
    /// </summary>
    /// <remarks>
    /// 用于显示的会员昵称，方便前端展示
    /// </remarks>
    public string? MemberNickname { get; set; }

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
    public int PointsType { get; set; }

    /// <summary>
    /// 积分类型名称
    /// </summary>
    /// <remarks>
    /// 用于显示的积分类型名称，方便前端展示
    /// </remarks>
    public string PointsTypeName { get; set; } = null!;

    /// <summary>
    /// 积分变动
    /// </summary>
    /// <remarks>
    /// 正数为获得，负数为使用
    /// </remarks>
    public int Points { get; set; }

    /// <summary>
    /// 变动后余额
    /// </summary>
    public int Balance { get; set; }

    /// <summary>
    /// 关联订单号
    /// </summary>
    public string? OrderNo { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    public string? Remark { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreateTime { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime? UpdateTime { get; set; }
}