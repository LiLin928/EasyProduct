using EasyProduct.Common.Base;
using EasyProduct.Models.Dto.Mall.Point;
using EasyProduct.Business.Mall;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyProduct.Web.Controllers.App.Mall;

/// <summary>
/// 积分控制器（会员端）
/// </summary>
/// <remarks>
/// 提供会员积分查询、积分流水查询、积分兑换等功能。
/// 需要会员登录。
/// </remarks>
[ApiController]
[Route("api/app/mall/point")]
[Authorize(AuthenticationSchemes = "MemberJwt")]
public class PointController : BaseController
{
    private readonly IPointService _pointService;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="pointService">积分服务</param>
    public PointController(IPointService pointService)
    {
        _pointService = pointService;
    }

    /// <summary>
    /// 获取我的积分余额
    /// </summary>
    /// <returns>积分余额信息</returns>
    /// <remarks>
    /// 返回累计积分、可用积分、冻结积分。
    /// </remarks>
    [HttpGet("balance")]
    public async Task<ApiResponse<PointBalanceDto>> GetMyBalance()
    {
        var memberId = GetCurrentUserId().ToString();
        var result = await _pointService.GetMemberBalanceAsync(memberId);
        return Success(result);
    }

    /// <summary>
    /// 获取我的积分流水
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>积分流水分页列表</returns>
    /// <remarks>
    /// 支持按积分类型、积分来源、时间范围筛选。
    /// 支持分页查询。
    /// </remarks>
    [HttpGet("record/list")]
    public async Task<ApiResponse<PageResponse<PointRecordDto>>> GetMyRecords([FromQuery] PointRecordQueryDto query)
    {
        var memberId = GetCurrentUserId().ToString();
        query.MemberId = memberId;
        var result = await _pointService.GetRecordListAsync(query);
        return Success(result);
    }

    /// <summary>
    /// 获取我的积分兑换记录
    /// </summary>
    /// <param name="pageIndex">页码</param>
    /// <param name="pageSize">每页条数</param>
    /// <returns>兑换记录分页列表</returns>
    [HttpGet("exchange/list")]
    public async Task<ApiResponse<PageResponse<PointExchangeDto>>> GetMyExchanges([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
    {
        var memberId = GetCurrentUserId().ToString();
        var result = await _pointService.GetExchangeListAsync(memberId, pageIndex, pageSize);
        return Success(result);
    }

    /// <summary>
    /// 获取我的积分统计
    /// </summary>
    /// <returns>积分统计信息</returns>
    /// <remarks>
    /// 返回累计积分、可用积分、冻结积分、收入总计、支出总计。
    /// </remarks>
    [HttpGet("statistics")]
    public async Task<ApiResponse<Dictionary<string, object>>> GetMyStatistics()
    {
        var memberId = GetCurrentUserId().ToString();
        var result = await _pointService.GetMemberPointsStatisticsAsync(memberId);
        return Success(result);
    }

    /// <summary>
    /// 兑换优惠券
    /// </summary>
    /// <param name="dto">兑换参数</param>
    /// <returns>兑换记录ID</returns>
    /// <remarks>
    /// 使用积分兑换优惠券。
    /// 需要确保积分余额充足。
    /// </remarks>
    [HttpPost("exchange")]
    public async Task<ApiResponse<string>> ExchangeCoupon([FromBody] ExchangePointDto dto)
    {
        var memberId = GetCurrentUserId().ToString();
        var result = await _pointService.ExchangeCouponAsync(memberId, dto.CouponId!);
        return ApiResponse<string>.Success(result, "优惠券兑换成功");
    }

    /// <summary>
    /// 签到赠送积分
    /// </summary>
    /// <returns>赠送积分数</returns>
    /// <remarks>
    /// 每日签到可获赠积分。
    /// </remarks>
    [HttpPost("check-in")]
    public async Task<ApiResponse<int>> CheckIn()
    {
        var memberId = GetCurrentUserId().ToString();
        var result = await _pointService.GrantCheckInPointsAsync(memberId);
        return ApiResponse<int>.Success(result, "签到成功");
    }
}