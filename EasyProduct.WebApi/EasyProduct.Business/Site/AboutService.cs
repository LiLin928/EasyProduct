using EasyProduct.Common.Base;
using EasyProduct.Common.Error;
using EasyProduct.Models.Dto.Site.About;
using EasyProduct.Models.Entitys.Site;
using Mapster;
using Microsoft.Extensions.Logging;
using SqlSugar;

namespace EasyProduct.Business.Site;

/// <summary>
/// 关于我们服务实现（管理端）
/// </summary>
/// <remarks>
/// 提供关于我们的获取和更新功能，供管理端使用
/// 单页内容，不支持创建和删除
/// </remarks>
public class AboutService : BaseService, IAboutService
{
    private readonly ILogger<AboutService> _logger;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="logger">日志服务</param>
    public AboutService(ILogger<AboutService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// 获取关于我们详情
    /// </summary>
    /// <param name="id">关于我们ID</param>
    /// <returns>关于我们详情</returns>
    public async Task<AboutDto> GetByIdAsync(string id)
    {
        var entity = await _db.Queryable<site_about>()
            .Where(x => x.Id.ToString() == id && x.IsDeleted == 0)
            .FirstAsync();

        if (entity == null)
        {
            throw new BusinessException("关于我们不存在", 404);
        }

        return entity.Adapt<AboutDto>();
    }

    /// <summary>
    /// 更新关于我们
    /// </summary>
    /// <param name="dto">更新参数</param>
    /// <returns>是否成功</returns>
    public async Task<bool> UpdateAsync(UpdateAboutDto dto)
    {
        var entity = await _db.Queryable<site_about>()
            .Where(x => x.IsDeleted == 0)
            .FirstAsync();

        if (entity == null)
        {
            throw new BusinessException("关于我们不存在", 404);
        }

        // 更新字段（只更新非空字段）
        if (!string.IsNullOrEmpty(dto.Title)) entity.Title = dto.Title;
        if (dto.TitleEn != null) entity.TitleEn = dto.TitleEn;
        if (dto.Subtitle != null) entity.Subtitle = dto.Subtitle;
        if (dto.SubtitleEn != null) entity.SubtitleEn = dto.SubtitleEn;
        if (dto.Content != null) entity.Content = dto.Content;
        if (dto.ContentEn != null) entity.ContentEn = dto.ContentEn;
        if (dto.CoverImage != null) entity.CoverImage = dto.CoverImage;
        if (dto.Keywords != null) entity.Keywords = dto.Keywords;
        if (dto.Description != null) entity.Description = dto.Description;
        if (dto.Status.HasValue) entity.Status = (Models.Enums.Status)dto.Status.Value;

        entity.UpdatedAt = DateTime.UtcNow;

        await _db.Updateable(entity).ExecuteCommandAsync();

        _logger.LogInformation("更新关于我们成功：{Title}, ID: {Id}", entity.Title, entity.Id);

        return true;
    }

    /// <summary>
    /// 更新关于我们状态
    /// </summary>
    /// <param name="id">关于我们ID</param>
    /// <param name="status">状态：0=禁用，1=启用</param>
    /// <returns>是否成功</returns>
    public async Task<bool> UpdateStatusAsync(string id, int status)
    {
        var entity = await _db.Queryable<site_about>()
            .Where(x => x.Id.ToString() == id && x.IsDeleted == 0)
            .FirstAsync();

        if (entity == null)
        {
            throw new BusinessException("关于我们不存在", 404);
        }

        entity.Status = (Models.Enums.Status)status;
        entity.UpdatedAt = DateTime.UtcNow;

        await _db.Updateable(entity).ExecuteCommandAsync();

        _logger.LogInformation("更新关于我们状态成功：Status: {Status}, ID: {Id}", status, entity.Id);

        return true;
    }
}