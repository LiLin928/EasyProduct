using EasyProduct.Common.Base;
using EasyProduct.Models.Dto.Mall.Coupon;
using EasyProduct.Business.Mall;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyProduct.Web.Controllers.Admin.Mall;

/// <summary>
/// 优惠券管理控制器（管理端）
/// </summary>
[ApiController]
[Route("api/admin/mall/coupon")]
[Authorize(AuthenticationSchemes = "AdminJwt")]
public class CouponAdminController : BaseController
{
    private readonly ICouponService _couponService;

    public CouponAdminController(ICouponService couponService)
    {
        _couponService = couponService;
    }

    /// <summary>
    /// 创建优惠券
    /// </summary>
    [HttpPost]
    public async Task<ApiResponse<string>> Create([FromBody] CreateCouponDto dto)
    {
        var result = await _couponService.CreateCouponAsync(dto);
        return ApiResponse<string>.Success(result, "优惠券创建成功");
    }

    /// <summary>
    /// 分页查询优惠券列表
    /// </summary>
    [HttpGet("list")]
    public async Task<ApiResponse<PageResponse<CouponDto>>> GetList([FromQuery] CouponQuery query)
    {
        var result = await _couponService.GetCouponListAsync(query);
        return Success(result);
    }

    /// <summary>
    /// 获取优惠券详情
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ApiResponse<CouponDto>> GetDetail(string id)
    {
        var result = await _couponService.GetCouponDetailAsync(id);
        return Success(result);
    }

    /// <summary>
    /// 更新优惠券
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ApiResponse<bool>> Update(string id, [FromBody] CreateCouponDto dto)
    {
        var result = await _couponService.UpdateCouponAsync(id, dto);
        return Success(result);
    }

    /// <summary>
    /// 删除优惠券
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ApiResponse<bool>> Delete(string id)
    {
        var result = await _couponService.DeleteCouponAsync(id);
        return Success(result);
    }

    /// <summary>
    /// 获取优惠券统计数据
    /// </summary>
    [HttpGet("{id}/statistics")]
    public async Task<ApiResponse<CouponStatisticsDto>> GetStatistics(string id)
    {
        var result = await _couponService.GetCouponStatisticsAsync(id);
        return Success(result);
    }
}