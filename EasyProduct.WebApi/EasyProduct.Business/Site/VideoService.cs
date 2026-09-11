using EasyProduct.Common.Base;
using EasyProduct.Common.Error;
using EasyProduct.Models.Dto.Site.Video;
using EasyProduct.Models.Entitys.Site;
using Mapster;
using Microsoft.Extensions.Logging;
using SqlSugar;

namespace EasyProduct.Business.Site;

/// <summary>
/// 视频管理服务实现（管理端）
/// </summary>
/// <remarks>
/// 提供视频管理的增删改查功能，供管理端使用
/// </remarks>
public class VideoService : BaseService, IVideoService
{
    private readonly ILogger<VideoService> _logger;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="logger">日志服务</param>
    public VideoService(ILogger<VideoService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// 获取视频列表（支持分页、筛选）
    /// </summary>
    /// <param name="query">查询参数，包含分页、标题、分类、状态、视频类型筛选</param>
    /// <returns>视频列表</returns>
    public async Task<List<VideoDto>> GetListAsync(VideoQueryDto query)
    {
        var queryable = _db.Queryable<site_video>()
            .Where(x => x.IsDeleted == 0);

        // 按标题模糊搜索
        if (!string.IsNullOrEmpty(query.Title))
        {
            queryable = queryable.Where(x => x.Title.Contains(query.Title));
        }

        // 按分类筛选
        if (!string.IsNullOrEmpty(query.Category))
        {
            queryable = queryable.Where(x => x.Category == query.Category);
        }

        // 按状态筛选
        if (query.Status.HasValue)
        {
            queryable = queryable.Where(x => x.Status == query.Status.Value);
        }

        // 按视频类型筛选
        if (!string.IsNullOrEmpty(query.VideoType))
        {
            queryable = queryable.Where(x => x.VideoType == query.VideoType);
        }

        // 排序：先按排序号，再按创建时间倒序
        var list = await queryable
            .OrderBy(x => x.Sort)
            .OrderBy(x => x.CreatedAt, OrderByType.Desc)
            .Skip((query.PageIndex - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();

        return list.Adapt<List<VideoDto>>();
    }

    /// <summary>
    /// 获取视频详情
    /// </summary>
    /// <param name="id">视频ID</param>
    /// <returns>视频详情</returns>
    public async Task<VideoDto> GetByIdAsync(string id)
    {
        var entity = await _db.Queryable<site_video>()
            .Where(x => x.Id.ToString() == id && x.IsDeleted == 0)
            .FirstAsync();

        if (entity == null)
        {
            throw new BusinessException("视频不存在", 404);
        }

        return entity.Adapt<VideoDto>();
    }

    /// <summary>
    /// 创建视频
    /// </summary>
    /// <param name="dto">创建参数</param>
    /// <returns>新视频ID</returns>
    public async Task<string> CreateAsync(CreateVideoDto dto)
    {
        var entity = dto.Adapt<site_video>();
        entity.Id = Guid.NewGuid();
        entity.Status = Models.Enums.Status.Enabled;
        entity.CreatedAt = DateTime.UtcNow;

        await _db.Insertable(entity).ExecuteCommandAsync();

        _logger.LogInformation("创建视频成功：{Title}, ID: {Id}", entity.Title, entity.Id);

        return entity.Id.ToString();
    }

    /// <summary>
    /// 更新视频
    /// </summary>
    /// <param name="dto">更新参数</param>
    /// <returns>是否成功</returns>
    public async Task<bool> UpdateAsync(UpdateVideoDto dto)
    {
        var entity = await _db.Queryable<site_video>()
            .Where(x => x.Id.ToString() == dto.Id && x.IsDeleted == 0)
            .FirstAsync();

        if (entity == null)
        {
            throw new BusinessException("视频不存在", 404);
        }

        // 更新字段（只更新非空字段）
        if (!string.IsNullOrEmpty(dto.Title)) entity.Title = dto.Title;
        if (dto.TitleEn != null) entity.TitleEn = dto.TitleEn;
        if (dto.Description != null) entity.Description = dto.Description;
        if (dto.DescriptionEn != null) entity.DescriptionEn = dto.DescriptionEn;
        if (dto.CoverImage != null) entity.CoverImage = dto.CoverImage;
        if (!string.IsNullOrEmpty(dto.VideoUrl)) entity.VideoUrl = dto.VideoUrl;
        if (dto.VideoType != null) entity.VideoType = dto.VideoType;
        if (dto.Duration.HasValue) entity.Duration = dto.Duration.Value;
        if (dto.Category != null) entity.Category = dto.Category;
        if (dto.Sort.HasValue) entity.Sort = dto.Sort.Value;
        if (dto.Status.HasValue) entity.Status = (Models.Enums.Status)dto.Status.Value;

        entity.UpdatedAt = DateTime.UtcNow;

        await _db.Updateable(entity).ExecuteCommandAsync();

        _logger.LogInformation("更新视频成功：{Title}, ID: {Id}", entity.Title, entity.Id);

        return true;
    }

    /// <summary>
    /// 删除视频
    /// </summary>
    /// <param name="id">视频ID</param>
    /// <returns>是否成功</returns>
    public async Task<bool> DeleteAsync(string id)
    {
        var entity = await _db.Queryable<site_video>()
            .Where(x => x.Id.ToString() == id && x.IsDeleted == 0)
            .FirstAsync();

        if (entity == null)
        {
            throw new BusinessException("视频不存在", 404);
        }

        // 软删除
        entity.IsDeleted = 1;
        entity.UpdatedAt = DateTime.UtcNow;

        await _db.Updateable(entity).ExecuteCommandAsync();

        _logger.LogInformation("删除视频成功：{Title}, ID: {Id}", entity.Title, entity.Id);

        return true;
    }

    /// <summary>
    /// 更新视频状态
    /// </summary>
    /// <param name="id">视频ID</param>
    /// <param name="status">状态：0=禁用，1=启用</param>
    /// <returns>是否成功</returns>
    public async Task<bool> UpdateStatusAsync(string id, int status)
    {
        var entity = await _db.Queryable<site_video>()
            .Where(x => x.Id.ToString() == id && x.IsDeleted == 0)
            .FirstAsync();

        if (entity == null)
        {
            throw new BusinessException("视频不存在", 404);
        }

        entity.Status = (Models.Enums.Status)status;
        entity.UpdatedAt = DateTime.UtcNow;

        await _db.Updateable(entity).ExecuteCommandAsync();

        _logger.LogInformation("更新视频状态成功：{Title}, Status: {Status}, ID: {Id}", entity.Title, status, entity.Id);

        return true;
    }
}