using EasyProduct.Common.Base;
using EasyProduct.Models.Dto.Mall.Coupon;

namespace EasyProduct.Business.Mall;

/// <summary>
/// 优惠券服务接口
/// </summary>
/// <remarks>
/// 提供优惠券的创建、查询、更新、删除等管理功能。
/// 支持满减券和折扣券两种类型。
/// </remarks>
public interface ICouponService
{
    #region 创建优惠券

    /// <summary>
    /// 创建优惠券
    /// </summary>
    /// <param name="dto">创建优惠券参数</param>
    /// <returns>优惠券ID</returns>
    Task<string> CreateCouponAsync(CreateCouponDto dto);

    #endregion

    #region 查询优惠券

    /// <summary>
    /// 分页查询优惠券列表
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>优惠券分页列表</returns>
    Task<PageResponse<CouponDto>> GetCouponListAsync(CouponQuery query);

    /// <summary>
    /// 获取优惠券详情
    /// </summary>
    /// <param name="couponId">优惠券ID</param>
    /// <returns>优惠券详情</returns>
    Task<CouponDto> GetCouponDetailAsync(string couponId);

    /// <summary>
    /// 获取可领取的优惠券列表
    /// </summary>
    /// <returns>可领取的优惠券列表</returns>
    Task<List<CouponDto>> GetAvailableCouponsAsync();

    #endregion

    #region 更新优惠券

    /// <summary>
    /// 更新优惠券
    /// </summary>
    /// <param name="couponId">优惠券ID</param>
    /// <param name="dto">更新参数</param>
    /// <returns>是否成功</returns>
    Task<bool> UpdateCouponAsync(string couponId, CreateCouponDto dto);

    /// <summary>
    /// 删除优惠券
    /// </summary>
    /// <param name="couponId">优惠券ID</param>
    /// <returns>是否成功</returns>
    Task<bool> DeleteCouponAsync(string couponId);

    #endregion

    #region 优惠券统计

    /// <summary>
    /// 获取优惠券统计数据
    /// </summary>
    /// <param name="couponId">优惠券ID</param>
    /// <returns>统计数据</returns>
    Task<CouponStatisticsDto> GetCouponStatisticsAsync(string couponId);

    #endregion
}