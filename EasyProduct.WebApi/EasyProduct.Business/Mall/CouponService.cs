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
/// 优惠券服务实现类
/// </summary>
public class CouponService : BaseService, ICouponService
{
    private readonly ILogger<CouponService> _logger;

    public CouponService(ILogger<CouponService> logger)
    {
        _logger = logger;
    }

    #region 创建优惠券

    public async Task<string> CreateCouponAsync(CreateCouponDto dto)
    {
        // 验证时间范围
        if (dto.StartTime >= dto.EndTime)
        {
            throw new BusinessException("开始时间必须早于结束时间", 400);
        }

        var coupon = dto.Adapt<Coupon>();
        coupon.Id = Guid.NewGuid();

        // 处理商品ID列表
        if (dto.ProductIds != null && dto.ProductIds.Count > 0)
        {
            coupon.ProductIds = System.Text.Json.JsonSerializer.Serialize(dto.ProductIds);
        }

        await _db.Insertable(coupon).ExecuteCommandAsync();

        _logger.LogInformation("优惠券创建成功：优惠券名称={Name}，类型={Type}", coupon.Name, coupon.Type);

        return coupon.Id.ToString();
    }

    #endregion

    #region 查询优惠券

    public async Task<PageResponse<CouponDto>> GetCouponListAsync(CouponQuery query)
    {
        var queryable = _db.Queryable<Coupon>()
            .WhereIF(!string.IsNullOrEmpty(query.Name), c => c.Name.Contains(query.Name!))
            .WhereIF(query.Type.HasValue, c => (int)c.Type == query.Type.Value)
            .WhereIF(query.StartTime.HasValue, c => c.StartTime >= query.StartTime)
            .WhereIF(query.EndTime.HasValue, c => c.EndTime <= query.EndTime)
            .Where(c => c.IsDeleted == 0)
            .OrderBy(c => c.CreatedAt, OrderByType.Desc);

        RefAsync<int> totalCount = 0;
        var coupons = await queryable.ToPageListAsync(query.PageIndex, query.PageSize, totalCount);

        var dtos = coupons.Adapt<List<CouponDto>>();

        return new PageResponse<CouponDto>
        {
            List = dtos,
            Total = totalCount.Value,
            PageIndex = query.PageIndex,
            PageSize = query.PageSize
        };
    }

    public async Task<CouponDto> GetCouponDetailAsync(string couponId)
    {
        var coupon = await _db.Queryable<Coupon>()
            .FirstAsync(c => c.Id.ToString() == couponId && c.IsDeleted == 0);

        if (coupon == null)
        {
            throw new BusinessException("优惠券不存在", 404);
        }

        return coupon.Adapt<CouponDto>();
    }

    public async Task<List<CouponDto>> GetAvailableCouponsAsync()
    {
        var now = DateTime.Now;
        var coupons = await _db.Queryable<Coupon>()
            .Where(c => c.StartTime <= now && c.EndTime >= now && c.ClaimedCount < c.TotalCount && c.IsDeleted == 0)
            .OrderBy(c => c.CreatedAt, OrderByType.Desc)
            .ToListAsync();

        return coupons.Adapt<List<CouponDto>>();
    }

    #endregion

    #region 更新优惠券

    public async Task<bool> UpdateCouponAsync(string couponId, CreateCouponDto dto)
    {
        var coupon = await _db.Queryable<Coupon>()
            .FirstAsync(c => c.Id.ToString() == couponId && c.IsDeleted == 0);

        if (coupon == null)
        {
            throw new BusinessException("优惠券不存在", 404);
        }

        // 验证时间范围
        if (dto.StartTime >= dto.EndTime)
        {
            throw new BusinessException("开始时间必须早于结束时间", 400);
        }

        // 更新优惠券信息
        coupon.Name = dto.Name;
        coupon.Type = dto.Type;
        coupon.Value = dto.Value;
        coupon.MinAmount = dto.MinAmount;
        coupon.StartTime = dto.StartTime;
        coupon.EndTime = dto.EndTime;
        coupon.TotalCount = dto.TotalCount;
        coupon.Description = dto.Description;
        coupon.UpdatedAt = DateTime.Now;

        if (dto.ProductIds != null && dto.ProductIds.Count > 0)
        {
            coupon.ProductIds = System.Text.Json.JsonSerializer.Serialize(dto.ProductIds);
        }
        else
        {
            coupon.ProductIds = null;
        }

        var success = await _db.Updateable(coupon).ExecuteCommandAsync() > 0;

        if (success)
        {
            _logger.LogInformation("优惠券更新成功：优惠券ID={CouponId}", couponId);
        }

        return success;
    }

    public async Task<bool> DeleteCouponAsync(string couponId)
    {
        var coupon = await _db.Queryable<Coupon>()
            .FirstAsync(c => c.Id.ToString() == couponId && c.IsDeleted == 0);

        if (coupon == null)
        {
            throw new BusinessException("优惠券不存在", 404);
        }

        // 软删除
        coupon.IsDeleted = 1;
        coupon.UpdatedAt = DateTime.Now;

        var success = await _db.Updateable(coupon).ExecuteCommandAsync() > 0;

        if (success)
        {
            _logger.LogInformation("优惠券删除成功：优惠券ID={CouponId}", couponId);
        }

        return success;
    }

    #endregion

    #region 优惠券统计

    public async Task<CouponStatisticsDto> GetCouponStatisticsAsync(string couponId)
    {
        var coupon = await _db.Queryable<Coupon>()
            .FirstAsync(c => c.Id.ToString() == couponId && c.IsDeleted == 0);

        if (coupon == null)
        {
            throw new BusinessException("优惠券不存在", 404);
        }

        return new CouponStatisticsDto
        {
            CouponId = couponId,
            TotalCount = coupon.TotalCount,
            ClaimedCount = coupon.ClaimedCount,
            UsedCount = coupon.UsedCount
        };
    }

    #endregion
}