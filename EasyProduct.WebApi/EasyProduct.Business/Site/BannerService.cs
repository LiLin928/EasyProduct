using EasyProduct.Common.Base;
using EasyProduct.Common.Error;
using EasyProduct.Models.Dto.Site.Banner;
using EasyProduct.Models.Entitys.Site;
using Mapster;
using Microsoft.Extensions.Logging;
using SqlSugar;

namespace EasyProduct.Business.Site;

/// <summary>
/// Banner服务实现（管理端）
/// </summary>
/// <remarks>
/// 提供Banner的增删改查等功能，供管理端使用
/// </remarks>
public class BannerService : BaseService, IBannerService
{
    private readonly ILogger<BannerService> _logger;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="logger">日志服务</param>
    public BannerService(ILogger<BannerService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// 获取Banner列表（支持分页、筛选）
    /// </summary>
    /// <param name="query">查询参数，包含分页、标题、显示位置、状态、时间范围筛选</param>
    /// <returns>Banner列表</returns>
    public async Task<List<BannerDto>> GetListAsync(BannerQueryDto query)
    {
        var queryable = _db.Queryable<site_banner>()
            .Where(x => x.IsDeleted == 0);

        // 按标题模糊搜索
        if (!string.IsNullOrEmpty(query.Title))
        {
            queryable = queryable.Where(x => x.Title.Contains(query.Title));
        }

        // 按显示位置筛选
        if (!string.IsNullOrEmpty(query.Position))
        {
            queryable = queryable.Where(x => x.Position == query.Position);
        }

        // 按状态筛选
        if (query.Status.HasValue)
        {
            queryable = queryable.Where(x => x.Status == query.Status.Value);
        }

        // 按开始时间范围筛选
        if (query.StartTimeBegin.HasValue)
        {
            queryable = queryable.Where(x => x.StartTime >= query.StartTimeBegin.Value);
        }

        if (query.StartTimeEnd.HasValue)
        {
            queryable = queryable.Where(x => x.StartTime <= query.StartTimeEnd.Value);
        }

        // 按结束时间范围筛选
        if (query.EndTimeBegin.HasValue)
        {
            queryable = queryable.Where(x => x.EndTime >= query.EndTimeBegin.Value);
        }

        if (query.EndTimeEnd.HasValue)
        {
            queryable = queryable.Where(x => x.EndTime <= query.EndTimeEnd.Value);
        }

        // 排序：先按排序号，再按创建时间倒序
        var list = await queryable
            .OrderBy(x => x.Sort)
            .OrderBy(x => x.CreatedAt, OrderByType.Desc)
            .Skip((query.PageIndex - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();

        return list.Adapt<List<BannerDto>>();
    }

    /// <summary>
    /// 获取Banner详情
    /// </summary>
    /// <param name="id">BannerID</param>
    /// <returns>Banner详情</returns>
    public async Task<BannerDto> GetByIdAsync(string id)
    {
        var entity = await _db.Queryable<site_banner>()
            .Where(x => x.Id.ToString() == id && x.IsDeleted == 0)
            .FirstAsync();

        if (entity == null)
        {
            throw new BusinessException("Banner不存在", 404);
        }

        return entity.Adapt<BannerDto>();
    }

    /// <summary>
    /// 创建Banner
    /// </summary>
    /// <param name="dto">创建参数</param>
    /// <returns>新BannerID</returns>
    public async Task<string> CreateAsync(CreateBannerDto dto)
    {
        var entity = dto.Adapt<site_banner>();
        entity.Id = Guid.NewGuid();
        entity.Status = Models.Enums.Status.Enabled;
        entity.CreatedAt = DateTime.UtcNow;

        await _db.Insertable(entity).ExecuteCommandAsync();

        _logger.LogInformation("创建Banner成功：{Title}, ID: {Id}", entity.Title, entity.Id);

        return entity.Id.ToString();
    }

    /// <summary>
    /// 更新Banner
    /// </summary>
    /// <param name="dto">更新参数</param>
    /// <returns>是否成功</returns>
    public async Task<bool> UpdateAsync(UpdateBannerDto dto)
    {
        var entity = await _db.Queryable<site_banner>()
            .Where(x => x.Id.ToString() == dto.Id && x.IsDeleted == 0)
            .FirstAsync();

        if (entity == null)
        {
            throw new BusinessException("Banner不存在", 404);
        }

        // 更新字段（只更新非空字段）
        if (!string.IsNullOrEmpty(dto.Title)) entity.Title = dto.Title;
        if (!string.IsNullOrEmpty(dto.ImageUrl)) entity.ImageUrl = dto.ImageUrl;
        if (dto.LinkUrl != null) entity.LinkUrl = dto.LinkUrl;
        if (dto.LinkType != null) entity.LinkType = dto.LinkType;
        if (dto.LinkParam != null) entity.LinkParam = dto.LinkParam;
        if (dto.Position != null) entity.Position = dto.Position;
        if (dto.StartTime.HasValue) entity.StartTime = dto.StartTime;
        if (dto.EndTime.HasValue) entity.EndTime = dto.EndTime;
        if (dto.Sort.HasValue) entity.Sort = dto.Sort.Value;
        if (dto.Description != null) entity.Description = dto.Description;
        if (dto.Status.HasValue) entity.Status = (Models.Enums.Status)dto.Status.Value;

        entity.UpdatedAt = DateTime.UtcNow;

        await _db.Updateable(entity).ExecuteCommandAsync();

        _logger.LogInformation("更新Banner成功：{Title}, ID: {Id}", entity.Title, entity.Id);

        return true;
    }

    /// <summary>
    /// 删除Banner
    /// </summary>
    /// <param name="id">BannerID</param>
    /// <returns>是否成功</returns>
    public async Task<bool> DeleteAsync(string id)
    {
        var entity = await _db.Queryable<site_banner>()
            .Where(x => x.Id.ToString() == id && x.IsDeleted == 0)
            .FirstAsync();

        if (entity == null)
        {
            throw new BusinessException("Banner不存在", 404);
        }

        // 软删除
        entity.IsDeleted = 1;
        entity.UpdatedAt = DateTime.UtcNow;

        await _db.Updateable(entity).ExecuteCommandAsync();

        _logger.LogInformation("删除Banner成功：{Title}, ID: {Id}", entity.Title, entity.Id);

        return true;
    }

    /// <summary>
    /// 更新Banner状态
    /// </summary>
    /// <param name="id">BannerID</param>
    /// <param name="status">状态：0=禁用，1=启用</param>
    /// <returns>是否成功</returns>
    public async Task<bool> UpdateStatusAsync(string id, int status)
    {
        var entity = await _db.Queryable<site_banner>()
            .Where(x => x.Id.ToString() == id && x.IsDeleted == 0)
            .FirstAsync();

        if (entity == null)
        {
            throw new BusinessException("Banner不存在", 404);
        }

        entity.Status = (Models.Enums.Status)status;
        entity.UpdatedAt = DateTime.UtcNow;

        await _db.Updateable(entity).ExecuteCommandAsync();

        _logger.LogInformation("更新Banner状态成功：{Title}, Status: {Status}, ID: {Id}", entity.Title, status, entity.Id);

        return true;
    }
}