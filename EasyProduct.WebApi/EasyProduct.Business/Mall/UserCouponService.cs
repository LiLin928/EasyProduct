using EasyProduct.Common.Base;
using EasyProduct.Common.Error;
using EasyProduct.Models.Dto.Mall.Coupon;
using EasyProduct.Models.Entitys.Mall;
using EasyProduct.Models.Enums.Mall;
using Mapster;
using SqlSugar;
using Microsoft.Extensions.Logging;

namespace EasyProduct.Business.Mall;

/// <summary>
/// 用户优惠券服务实现类
/// </summary>
public class UserCouponService : BaseService, IUserCouponService
{
    private readonly ILogger<UserCouponService> _logger;

    public UserCouponService(ILogger<UserCouponService> logger)
    {
        _logger = logger;
    }

    #region 领取优惠券

    public async Task<string> ClaimCouponAsync(string memberId, string couponId)
    {
        // 查询优惠券
        var coupon = await _db.Queryable<Coupon>()
            .FirstAsync(c => c.Id.ToString() == couponId && c.IsDeleted == 0);

        if (coupon == null)
        {
            throw new BusinessException("优惠券不存在", 404);
        }

        // 验证优惠券是否有效
        var now = DateTime.Now;
        if (coupon.StartTime > now || coupon.EndTime < now)
        {
            throw new BusinessException("优惠券不在有效期内", 400);
        }

        if (coupon.ClaimedCount >= coupon.TotalCount)
        {
            throw new BusinessException("优惠券已被领完", 400);
        }

        // 检查是否已领取
        var existingUserCoupon = await _db.Queryable<UserCoupon>()
            .FirstAsync(uc => uc.MemberId == memberId && uc.CouponId == couponId && uc.IsDeleted == 0);

        if (existingUserCoupon != null)
        {
            throw new BusinessException("您已领取过该优惠券", 400);
        }

        // 创建用户优惠券
        var userCoupon = new UserCoupon
        {
            Id = Guid.NewGuid(),
            CouponId = couponId,
            MemberId = memberId,
            Status = UserCouponStatus.Unused,
            ClaimTime = DateTime.Now
        };

        // 开启事务
        var result = await _db.Ado.UseTranAsync(async () =>
        {
            // 创建用户优惠券
            await _db.Insertable(userCoupon).ExecuteCommandAsync();

            // 更新优惠券领取数量
            coupon.ClaimedCount++;
            coupon.UpdatedAt = DateTime.Now;
            await _db.Updateable(coupon).ExecuteCommandAsync();
        });

        if (!result.IsSuccess)
        {
            _logger.LogError("优惠券领取失败：{ErrorMessage}", result.ErrorMessage);
            throw new BusinessException($"优惠券领取失败：{result.ErrorMessage}", 400);
        }

        _logger.LogInformation("优惠券领取成功：会员ID={MemberId}，优惠券ID={CouponId}", memberId, couponId);

        return userCoupon.Id.ToString();
    }

    #endregion

    #region 查询用户优惠券

    public async Task<List<UserCouponDto>> GetUserCouponListAsync(string memberId, int? status = null)
    {
        var queryable = _db.Queryable<UserCoupon>()
            .LeftJoin<Coupon>((uc, c) => uc.CouponId == c.Id.ToString())
            .Where((uc, c) => uc.MemberId == memberId && uc.IsDeleted == 0)
            .WhereIF(status.HasValue, (uc, c) => (int)uc.Status == status.Value)
            .OrderBy((uc, c) => uc.ClaimTime, OrderByType.Desc);

        var userCoupons = await queryable
            .Select((uc, c) => new UserCouponDto
            {
                Id = uc.Id.ToString(),
                CouponId = uc.CouponId,
                CouponName = c.Name,
                CouponType = c.Type.ToString(),
                Value = c.Value,
                MinAmount = c.MinAmount,
                StartTime = c.StartTime,
                EndTime = c.EndTime,
                Status = uc.Status.ToString(),
                ClaimTime = uc.ClaimTime,
                UsedTime = uc.UsedTime,
                OrderId = uc.OrderId
            })
            .ToListAsync();

        // 检查并更新过期状态
        var now = DateTime.Now;
        foreach (var userCoupon in userCoupons)
        {
            if (userCoupon.Status == UserCouponStatus.Unused.ToString() && DateTime.Parse(userCoupon.EndTime.ToString()) < now)
            {
                // 更新为已过期
                var uc = await _db.Queryable<UserCoupon>()
                    .FirstAsync(u => u.Id.ToString() == userCoupon.Id);

                if (uc != null)
                {
                    uc.Status = UserCouponStatus.Expired;
                    uc.UpdatedAt = DateTime.Now;
                    await _db.Updateable(uc).ExecuteCommandAsync();
                    userCoupon.Status = UserCouponStatus.Expired.ToString();
                }
            }
        }

        return userCoupons;
    }

    public async Task<UserCouponDto> GetUserCouponDetailAsync(string userCouponId)
    {
        var userCoupon = await _db.Queryable<UserCoupon>()
            .LeftJoin<Coupon>((uc, c) => uc.CouponId == c.Id.ToString())
            .Where((uc, c) => uc.Id.ToString() == userCouponId && uc.IsDeleted == 0)
            .Select((uc, c) => new UserCouponDto
            {
                Id = uc.Id.ToString(),
                CouponId = uc.CouponId,
                CouponName = c.Name,
                CouponType = c.Type.ToString(),
                Value = c.Value,
                MinAmount = c.MinAmount,
                StartTime = c.StartTime,
                EndTime = c.EndTime,
                Status = uc.Status.ToString(),
                ClaimTime = uc.ClaimTime,
                UsedTime = uc.UsedTime,
                OrderId = uc.OrderId
            })
            .FirstAsync();

        if (userCoupon == null)
        {
            throw new BusinessException("用户优惠券不存在", 404);
        }

        return userCoupon;
    }

    public async Task<List<UserCouponDto>> GetAvailableCouponsForOrderAsync(string memberId, decimal orderAmount)
    {
        var now = DateTime.Now;
        var userCoupons = await _db.Queryable<UserCoupon>()
            .LeftJoin<Coupon>((uc, c) => uc.CouponId == c.Id.ToString())
            .Where((uc, c) => uc.MemberId == memberId && uc.Status == UserCouponStatus.Unused && uc.IsDeleted == 0)
            .Where((uc, c) => c.StartTime <= now && c.EndTime >= now)
            .Where((uc, c) => c.MinAmount <= orderAmount)
            .OrderBy((uc, c) => c.Value, OrderByType.Desc)
            .Select((uc, c) => new UserCouponDto
            {
                Id = uc.Id.ToString(),
                CouponId = uc.CouponId,
                CouponName = c.Name,
                CouponType = c.Type.ToString(),
                Value = c.Value,
                MinAmount = c.MinAmount,
                StartTime = c.StartTime,
                EndTime = c.EndTime,
                Status = uc.Status.ToString(),
                ClaimTime = uc.ClaimTime,
                UsedTime = uc.UsedTime,
                OrderId = uc.OrderId
            })
            .ToListAsync();

        return userCoupons;
    }

    #endregion

    #region 使用优惠券

    public async Task<bool> UseCouponAsync(string userCouponId, string orderId)
    {
        var userCoupon = await _db.Queryable<UserCoupon>()
            .FirstAsync(uc => uc.Id.ToString() == userCouponId && uc.IsDeleted == 0);

        if (userCoupon == null)
        {
            throw new BusinessException("用户优惠券不存在", 404);
        }

        if (userCoupon.Status != UserCouponStatus.Unused)
        {
            throw new BusinessException("优惠券已使用或已过期", 400);
        }

        // 更新优惠券状态
        userCoupon.Status = UserCouponStatus.Used;
        userCoupon.UsedTime = DateTime.Now;
        userCoupon.OrderId = orderId;
        userCoupon.UpdatedAt = DateTime.Now;

        var success = await _db.Updateable(userCoupon).ExecuteCommandAsync() > 0;

        if (success)
        {
            // 更新优惠券使用数量
            var coupon = await _db.Queryable<Coupon>()
                .FirstAsync(c => c.Id.ToString() == userCoupon.CouponId && c.IsDeleted == 0);

            if (coupon != null)
            {
                coupon.UsedCount++;
                coupon.UpdatedAt = DateTime.Now;
                await _db.Updateable(coupon).ExecuteCommandAsync();
            }

            _logger.LogInformation("优惠券使用成功：用户优惠券ID={UserCouponId}，订单ID={OrderId}", userCouponId, orderId);
        }

        return success;
    }

    #endregion
}