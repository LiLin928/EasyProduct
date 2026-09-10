using EasyProduct.Common.Base;
using EasyProduct.Models.Dto.Mall.Coupon;
using EasyProduct.Business.Mall;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyProduct.Web.Controllers.App.Mall;

/// <summary>
/// 用户优惠券控制器（会员端）
/// </summary>
[ApiController]
[Route("api/app/mall/coupon")]
[Authorize(AuthenticationSchemes = "MemberJwt")]
public class UserCouponController : BaseController
{
    private readonly ICouponService _couponService;
    private readonly IUserCouponService _userCouponService;

    public UserCouponController(ICouponService couponService, IUserCouponService userCouponService)
    {
        _couponService = couponService;
        _userCouponService = userCouponService;
    }

    /// <summary>
    /// 获取可领取的优惠券列表
    /// </summary>
    [HttpGet("available")]
    public async Task<ApiResponse<List<CouponDto>>> GetAvailableCoupons()
    {
        var result = await _couponService.GetAvailableCouponsAsync();
        return Success(result);
    }

    /// <summary>
    /// 领取优惠券
    /// </summary>
    [HttpPost("{couponId}/claim")]
    public async Task<ApiResponse<string>> Claim(string couponId)
    {
        var memberId = GetCurrentUserId().ToString();
        var result = await _userCouponService.ClaimCouponAsync(memberId, couponId);
        return ApiResponse<string>.Success(result, "优惠券领取成功");
    }

    /// <summary>
    /// 获取我的优惠券列表
    /// </summary>
    [HttpGet("my")]
    public async Task<ApiResponse<List<UserCouponDto>>> GetMyCoupons([FromQuery] int? status = null)
    {
        var memberId = GetCurrentUserId().ToString();
        var result = await _userCouponService.GetUserCouponListAsync(memberId, status);
        return Success(result);
    }

    /// <summary>
    /// 获取优惠券详情
    /// </summary>
    [HttpGet("user/{id}")]
    public async Task<ApiResponse<UserCouponDto>> GetDetail(string id)
    {
        var result = await _userCouponService.GetUserCouponDetailAsync(id);
        return Success(result);
    }
}