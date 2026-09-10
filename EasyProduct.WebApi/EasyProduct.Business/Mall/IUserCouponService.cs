using EasyProduct.Common.Base;
using EasyProduct.Models.Dto.Mall.Coupon;

namespace EasyProduct.Business.Mall;

/// <summary>
/// 用户优惠券服务接口
/// </summary>
/// <remarks>
/// 提供用户优惠券的领取、查询、使用等功能。
/// </remarks>
public interface IUserCouponService
{
    #region 领取优惠券

    /// <summary>
    /// 领取优惠券
    /// </summary>
    /// <param name="memberId">会员ID</param>
    /// <param name="couponId">优惠券ID</param>
    /// <returns>用户优惠券ID</returns>
    Task<string> ClaimCouponAsync(string memberId, string couponId);

    #endregion

    #region 查询用户优惠券

    /// <summary>
    /// 获取用户优惠券列表
    /// </summary>
    /// <param name="memberId">会员ID</param>
    /// <param name="status">优惠券状态（可选）</param>
    /// <returns>用户优惠券列表</returns>
    Task<List<UserCouponDto>> GetUserCouponListAsync(string memberId, int? status = null);

    /// <summary>
    /// 获取用户优惠券详情
    /// </summary>
    /// <param name="userCouponId">用户优惠券ID</param>
    /// <returns>用户优惠券详情</returns>
    Task<UserCouponDto> GetUserCouponDetailAsync(string userCouponId);

    /// <summary>
    /// 获取可用优惠券列表（下单时使用）
    /// </summary>
    /// <param name="memberId">会员ID</param>
    /// <param name="orderAmount">订单金额</param>
    /// <returns>可用优惠券列表</returns>
    Task<List<UserCouponDto>> GetAvailableCouponsForOrderAsync(string memberId, decimal orderAmount);

    #endregion

    #region 使用优惠券

    /// <summary>
    /// 使用优惠券（订单服务调用）
    /// </summary>
    /// <param name="userCouponId">用户优惠券ID</param>
    /// <param name="orderId">订单ID</param>
    /// <returns>是否成功</returns>
    Task<bool> UseCouponAsync(string userCouponId, string orderId);

    #endregion
}