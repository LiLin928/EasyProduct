using EasyProduct.Common.Base;
using EasyProduct.Common.Error;
using EasyProduct.Models.Dto.Site.Download;
using EasyProduct.Models.Entitys.Site;
using Mapster;
using Microsoft.Extensions.Logging;
using SqlSugar;

namespace EasyProduct.Business.Site;

/// <summary>
/// 下载管理服务实现（管理端）
/// </summary>
/// <remarks>
/// 提供下载管理的增删改查功能，供管理端使用
/// </remarks>
public class DownloadService : BaseService, IDownloadService
{
    private readonly ILogger<DownloadService> _logger;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="logger">日志服务</param>
    public DownloadService(ILogger<DownloadService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// 获取下载列表（支持分页、筛选）
    /// </summary>
    /// <param name="query">查询参数，包含分页、标题、分类、状态、文件类型筛选</param>
    /// <returns>下载列表</returns>
    public async Task<List<DownloadDto>> GetListAsync(DownloadQueryDto query)
    {
        var queryable = _db.Queryable<site_download>()
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

        // 按文件类型筛选
        if (!string.IsNullOrEmpty(query.FileType))
        {
            queryable = queryable.Where(x => x.FileType == query.FileType);
        }

        // 排序：先按排序号，再按创建时间倒序
        var list = await queryable
            .OrderBy(x => x.Sort)
            .OrderBy(x => x.CreatedAt, OrderByType.Desc)
            .Skip((query.PageIndex - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();

        return list.Adapt<List<DownloadDto>>();
    }

    /// <summary>
    /// 获取下载详情
    /// </summary>
    /// <param name="id">下载ID</param>
    /// <returns>下载详情</returns>
    public async Task<DownloadDto> GetByIdAsync(string id)
    {
        var entity = await _db.Queryable<site_download>()
            .Where(x => x.Id.ToString() == id && x.IsDeleted == 0)
            .FirstAsync();

        if (entity == null)
        {
            throw new BusinessException("下载不存在", 404);
        }

        return entity.Adapt<DownloadDto>();
    }

    /// <summary>
    /// 创建下载
    /// </summary>
    /// <param name="dto">创建参数</param>
    /// <returns>新下载ID</returns>
    public async Task<string> CreateAsync(CreateDownloadDto dto)
    {
        var entity = dto.Adapt<site_download>();
        entity.Id = Guid.NewGuid();
        entity.Status = Models.Enums.Status.Enabled;
        entity.CreatedAt = DateTime.UtcNow;

        await _db.Insertable(entity).ExecuteCommandAsync();

        _logger.LogInformation("创建下载成功：{Title}, ID: {Id}", entity.Title, entity.Id);

        return entity.Id.ToString();
    }

    /// <summary>
    /// 更新下载
    /// </summary>
    /// <param name="dto">更新参数</param>
    /// <returns>是否成功</returns>
    public async Task<bool> UpdateAsync(UpdateDownloadDto dto)
    {
        var entity = await _db.Queryable<site_download>()
            .Where(x => x.Id.ToString() == dto.Id && x.IsDeleted == 0)
            .FirstAsync();

        if (entity == null)
        {
            throw new BusinessException("下载不存在", 404);
        }

        // 更新字段（只更新非空字段）
        if (!string.IsNullOrEmpty(dto.Title)) entity.Title = dto.Title;
        if (dto.TitleEn != null) entity.TitleEn = dto.TitleEn;
        if (dto.Description != null) entity.Description = dto.Description;
        if (dto.DescriptionEn != null) entity.DescriptionEn = dto.DescriptionEn;
        if (!string.IsNullOrEmpty(dto.FileUrl)) entity.FileUrl = dto.FileUrl;
        if (dto.FileName != null) entity.FileName = dto.FileName;
        if (dto.FileSize.HasValue) entity.FileSize = dto.FileSize.Value;
        if (dto.FileType != null) entity.FileType = dto.FileType;
        if (dto.Category != null) entity.Category = dto.Category;
        if (dto.Sort.HasValue) entity.Sort = dto.Sort.Value;
        if (dto.Status.HasValue) entity.Status = (Models.Enums.Status)dto.Status.Value;

        entity.UpdatedAt = DateTime.UtcNow;

        await _db.Updateable(entity).ExecuteCommandAsync();

        _logger.LogInformation("更新下载成功：{Title}, ID: {Id}", entity.Title, entity.Id);

        return true;
    }

    /// <summary>
    /// 删除下载
    /// </summary>
    /// <param name="id">下载ID</param>
    /// <returns>是否成功</returns>
    public async Task<bool> DeleteAsync(string id)
    {
        var entity = await _db.Queryable<site_download>()
            .Where(x => x.Id.ToString() == id && x.IsDeleted == 0)
            .FirstAsync();

        if (entity == null)
        {
            throw new BusinessException("下载不存在", 404);
        }

        // 软删除
        entity.IsDeleted = 1;
        entity.UpdatedAt = DateTime.UtcNow;

        await _db.Updateable(entity).ExecuteCommandAsync();

        _logger.LogInformation("删除下载成功：{Title}, ID: {Id}", entity.Title, entity.Id);

        return true;
    }

    /// <summary>
    /// 更新下载状态
    /// </summary>
    /// <param name="id">下载ID</param>
    /// <param name="status">状态：0=禁用，1=启用</param>
    /// <returns>是否成功</returns>
    public async Task<bool> UpdateStatusAsync(string id, int status)
    {
        var entity = await _db.Queryable<site_download>()
            .Where(x => x.Id.ToString() == id && x.IsDeleted == 0)
            .FirstAsync();

        if (entity == null)
        {
            throw new BusinessException("下载不存在", 404);
        }

        entity.Status = (Models.Enums.Status)status;
        entity.UpdatedAt = DateTime.UtcNow;

        await _db.Updateable(entity).ExecuteCommandAsync();

        _logger.LogInformation("更新下载状态成功：{Title}, Status: {Status}, ID: {Id}", entity.Title, status, entity.Id);

        return true;
    }
}