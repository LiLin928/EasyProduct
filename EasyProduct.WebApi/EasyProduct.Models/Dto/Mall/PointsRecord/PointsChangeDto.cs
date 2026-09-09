using System.ComponentModel.DataAnnotations;

namespace EasyProduct.Models.Dto.Mall.PointsRecord;

/// <summary>
/// 积分变动DTO
/// </summary>
/// <remarks>
/// 用于后台管理员手动调整会员积分
/// 支持增加或减少积分
/// </remarks>
public class PointsChangeDto
{
    /// <summary>
    /// 会员ID
    /// </summary>
    /// <remarks>
    /// 必填，GUID 格式
    /// </remarks>
    [Required(ErrorMessage = "会员ID不能为空")]
    public Guid MemberId { get; set; }

    /// <summary>
    /// 积分类型
    /// </summary>
    /// <remarks>
    /// 必填，使用 PointsType 枚举：
    /// ConsumeEarn（1）= 消费获得
    /// OrderUse（2）= 订单使用
    /// AdminAdjust（3）= 后台调整
    /// SignIn（4）= 签到
    /// RegisterGift（5）= 注册赠送
    /// </remarks>
    [Required(ErrorMessage = "积分类型不能为空")]
    public int PointsType { get; set; }

    /// <summary>
    /// 积分变动
    /// </summary>
    /// <remarks>
    /// 必填，正数为增加积分，负数为减少积分
    /// </remarks>
    [Required(ErrorMessage = "积分变动不能为空")]
    public int Points { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    /// <remarks>
    /// 可选，最大长度 255 个字符
    /// </remarks>
    [MaxLength(255, ErrorMessage = "备注长度不能超过255个字符")]
    public string? Remark { get; set; }
}