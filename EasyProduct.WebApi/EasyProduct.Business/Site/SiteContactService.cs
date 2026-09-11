using EasyProduct.Common.Base;
using EasyProduct.Common.Error;
using EasyProduct.Models.Dto.Site.Contact;
using EasyProduct.Models.Entitys.Site;
using EasyProduct.Models.Enums.Site;
using Mapster;
using Microsoft.Extensions.Logging;
using SqlSugar;

namespace EasyProduct.Business.Site;

/// <summary>
/// 留言服务实现（官网公开）
/// </summary>
/// <remarks>
/// 提供官网公开的留言提交功能
/// 官网使用，允许匿名访问，但需要限流保护
/// </remarks>
public class SiteContactService : BaseService, ISiteContactService
{
    private readonly ILogger<SiteContactService> _logger;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="logger">日志服务</param>
    public SiteContactService(ILogger<SiteContactService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// 提交留言
    /// </summary>
    /// <param name="dto">创建留言参数</param>
    /// <returns>留言ID</returns>
    public async Task<string> SubmitContactAsync(CreateContactDto dto)
    {
        // 创建留言实体
        var entity = dto.Adapt<site_contact>();
        entity.Id = Guid.NewGuid();
        entity.Status = ContactStatus.Unread;
        entity.CreatedAt = DateTime.UtcNow;

        // 插入留言
        await _db.Insertable(entity).ExecuteCommandAsync();

        _logger.LogInformation("提交留言成功：{Id}", entity.Id);

        return entity.Id.ToString();
    }
}