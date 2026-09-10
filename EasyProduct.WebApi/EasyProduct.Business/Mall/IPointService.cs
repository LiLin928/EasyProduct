using EasyProduct.Common.Base;
using EasyProduct.Models.Dto.Mall.Point;
using EasyProduct.Models.Enums.Mall;

namespace EasyProduct.Business.Mall;

/// <summary>
/// 积分服务接口
/// </summary>
/// <remarks>
/// 提供积分规则管理、积分发放、积分消费、积分查询和统计等功能。
/// 积分涉及资金流水，所有写操作必须在事务中执行。
/// </remarks>
public interface IPointService
{
    #region 积分规则管理

    /// <summary>
    /// 创建积分规则
    /// </summary>
    /// <param name="dto">创建积分规则参数</param>
    /// <returns>规则ID</returns>
    Task<string> CreateRuleAsync(CreatePointRuleDto dto);

    /// <summary>
    /// 更新积分规则
    /// </summary>
    /// <param name="dto">更新积分规则参数</param>
    /// <returns>是否成功</returns>
    Task<bool> UpdateRuleAsync(UpdatePointRuleDto dto);

    /// <summary>
    /// 删除积分规则
    /// </summary>
    /// <param name="ruleId">规则ID</param>
    /// <returns>是否成功</returns>
    Task<bool> DeleteRuleAsync(string ruleId);

    /// <summary>
    /// 分页查询积分规则列表
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>积分规则分页列表</returns>
    Task<PageResponse<PointRuleDto>> GetRuleListAsync(PointRuleQueryDto query);

    /// <summary>
    /// 获取积分规则详情
    /// </summary>
    /// <param name="ruleId">规则ID</param>
    /// <returns>积分规则详情</returns>
    Task<PointRuleDto> GetRuleDetailAsync(string ruleId);

    /// <summary>
    /// 获取启用的积分规则列表
    /// </summary>
    /// <param name="type">规则类型（可选）</param>
    /// <returns>启用的积分规则列表</returns>
    Task<List<PointRuleDto>> GetActiveRulesAsync(PointRuleType? type = null);

    #endregion

    #region 积分发放

    /// <summary>
    /// 下单赠送积分
    /// </summary>
    /// <param name="memberId">会员ID</param>
    /// <param name="orderId">订单ID</param>
    /// <param name="orderAmount">订单金额</param>
    /// <returns>赠送积分数</returns>
    /// <remarks>
    /// 根据下单规则计算并发放积分。
    /// 如果规则设置为按金额倍数，则积分数 = 订单金额 / 倍数基数 × 积分数。
    /// 如果规则设置最大积分限制，则不超过最大值。
    /// </remarks>
    Task<int> GrantOrderPointsAsync(string memberId, string orderId, decimal orderAmount);

    /// <summary>
    /// 评价赠送积分
    /// </summary>
    /// <param name="memberId">会员ID</param>
    /// <param name="orderId">订单ID</param>
    /// <returns>赠送积分数</returns>
    Task<int> GrantReviewPointsAsync(string memberId, string orderId);

    /// <summary>
    /// 签到赠送积分
    /// </summary>
    /// <param name="memberId">会员ID</param>
    /// <returns>赠送积分数</returns>
    /// <remarks>
    /// 签到积分规则可以扩展为连续签到递增。
    /// </remarks>
    Task<int> GrantCheckInPointsAsync(string memberId);

    /// <summary>
    /// 邀请赠送积分
    /// </summary>
    /// <param name="memberId">会员ID</param>
    /// <param name="inviteeId">被邀请人ID</param>
    /// <returns>赠送积分数</returns>
    Task<int> GrantInvitePointsAsync(string memberId, string inviteeId);

    /// <summary>
    /// 系统调整积分
    /// </summary>
    /// <param name="memberId">会员ID</param>
    /// <param name="points">积分数（正数增加，负数减少）</param>
    /// <param name="remark">备注</param>
    /// <returns>是否成功</returns>
    Task<bool> AdjustPointsAsync(string memberId, int points, string remark);

    #endregion

    #region 积分消费

    /// <summary>
    /// 兑换优惠券
    /// </summary>
    /// <param name="memberId">会员ID</param>
    /// <param name="couponId">优惠券ID</param>
    /// <returns>兑换记录ID</returns>
    /// <remarks>
    /// 检查会员积分余额是否充足。
    /// 创建积分兑换记录。
    /// 扣减积分。
    /// 发放优惠券给会员。
    /// </remarks>
    Task<string> ExchangeCouponAsync(string memberId, string couponId);

    /// <summary>
    /// 订单抵扣积分
    /// </summary>
    /// <param name="memberId">会员ID</param>
    /// <param name="orderId">订单ID</param>
    /// <param name="points">使用积分数</param>
    /// <returns>抵扣金额</returns>
    /// <remarks>
    /// 检查会员积分余额是否充足。
    /// 冻结积分。
    /// 计算抵扣金额（如 100 积分 = 1 元）。
    /// 返回抵扣金额供订单创建时使用。
    /// </remarks>
    Task<decimal> DeductOrderPointsAsync(string memberId, string orderId, int points);

    /// <summary>
    /// 取消订单解冻积分
    /// </summary>
    /// <param name="memberId">会员ID</param>
    /// <param name="orderId">订单ID</param>
    /// <returns>是否成功</returns>
    Task<bool> UnfreezeOrderPointsAsync(string memberId, string orderId);

    #endregion

    #region 积分查询

    /// <summary>
    /// 获取会员积分余额
    /// </summary>
    /// <param name="memberId">会员ID</param>
    /// <returns>积分余额信息</returns>
    Task<PointBalanceDto> GetMemberBalanceAsync(string memberId);

    /// <summary>
    /// 分页查询积分流水
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>积分流水分页列表</returns>
    Task<PageResponse<PointRecordDto>> GetRecordListAsync(PointRecordQueryDto query);

    /// <summary>
    /// 获取积分兑换记录
    /// </summary>
    /// <param name="memberId">会员ID</param>
    /// <param name="pageIndex">页码</param>
    /// <param name="pageSize">每页条数</param>
    /// <returns>兑换记录分页列表</returns>
    Task<PageResponse<PointExchangeDto>> GetExchangeListAsync(string memberId, int pageIndex = 1, int pageSize = 10);

    #endregion

    #region 积分统计

    /// <summary>
    /// 获取会员积分统计
    /// </summary>
    /// <param name="memberId">会员ID</param>
    /// <returns>统计信息</returns>
    /// <remarks>
    /// 包含：累计积分、可用积分、冻结积分、即将过期积分等。
    /// </remarks>
    Task<Dictionary<string, object>> GetMemberPointsStatisticsAsync(string memberId);

    #endregion
}