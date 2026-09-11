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
/// 留言服务实现（管理端）
/// </summary>
/// <remarks>
/// 提供留言的查询、标记已读、回复、删除等功能
/// 管理端使用，需要 Admin JWT 认证
/// </remarks>
public class ContactService : BaseService, IContactService
{
    private readonly ILogger<ContactService> _logger;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="logger">日志服务</param>
    public ContactService(ILogger<ContactService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// 获取留言分页列表
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>留言分页结果</returns>
    public async Task<PageResponse<ContactDto>> GetContactListAsync(ContactQueryDto query)
    {
        var queryable = _db.Queryable<site_contact>()
            .Where(x => x.IsDeleted == 0);

        // 姓名模糊搜索
        if (!string.IsNullOrEmpty(query.Name))
        {
            queryable = queryable.Where(x => x.Name.Contains(query.Name) ||
                                              (x.NameEn != null && x.NameEn.Contains(query.Name)));
        }

        // 联系电话模糊搜索
        if (!string.IsNullOrEmpty(query.Phone))
        {
            queryable = queryable.Where(x => x.Phone != null && x.Phone.Contains(query.Phone));
        }

        // 电子邮箱模糊搜索
        if (!string.IsNullOrEmpty(query.Email))
        {
            queryable = queryable.Where(x => x.Email != null && x.Email.Contains(query.Email));
        }

        // 公司名称模糊搜索
        if (!string.IsNullOrEmpty(query.Company))
        {
            queryable = queryable.Where(x => x.Company != null && x.Company.Contains(query.Company) ||
                                              (x.CompanyEn != null && x.CompanyEn.Contains(query.Company)));
        }

        // 主题模糊搜索
        if (!string.IsNullOrEmpty(query.Subject))
        {
            queryable = queryable.Where(x => x.Subject != null && x.Subject.Contains(query.Subject) ||
                                              (x.SubjectEn != null && x.SubjectEn.Contains(query.Subject)));
        }

        // 状态筛选
        if (query.Status.HasValue)
        {
            queryable = queryable.Where(x => (int)x.Status == query.Status.Value);
        }

        // 创建时间范围查询
        if (query.StartTime.HasValue)
        {
            queryable = queryable.Where(x => x.CreatedAt >= query.StartTime.Value);
        }

        if (query.EndTime.HasValue)
        {
            queryable = queryable.Where(x => x.CreatedAt <= query.EndTime.Value);
        }

        // 获取总数
        var total = await queryable.CountAsync();

        // 分页查询
        var list = await queryable
            .OrderBy(x => x.CreatedAt, OrderByType.Desc)
            .Skip((query.PageIndex - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();

        // 转换为 DTO
        var dtoList = list.Adapt<List<ContactDto>>();

        return new PageResponse<ContactDto>
        {
            List = dtoList,
            Total = total,
            PageIndex = query.PageIndex,
            PageSize = query.PageSize
        };
    }

    /// <summary>
    /// 获取留言详情
    /// </summary>
    /// <param name="id">留言ID</param>
    /// <returns>留言详情</returns>
    public async Task<ContactDto> GetContactByIdAsync(string id)
    {
        var entity = await _db.Queryable<site_contact>()
            .Where(x => x.Id.ToString() == id && x.IsDeleted == 0)
            .FirstAsync();

        if (entity == null)
        {
            throw new BusinessException("留言不存在", 404);
        }

        return entity.Adapt<ContactDto>();
    }

    /// <summary>
    /// 标记留言为已读
    /// </summary>
    /// <param name="id">留言ID</param>
    /// <returns>是否成功</returns>
    public async Task<bool> MarkAsReadAsync(string id)
    {
        var entity = await _db.Queryable<site_contact>()
            .Where(x => x.Id.ToString() == id && x.IsDeleted == 0)
            .FirstAsync();

        if (entity == null)
        {
            throw new BusinessException("留言不存在", 404);
        }

        // 只有"未读"状态可以标记为已读
        if (entity.Status != ContactStatus.Unread)
        {
            throw new BusinessException("只有未读状态的留言可以标记为已读");
        }

        entity.Status = ContactStatus.Read;
        entity.UpdatedAt = DateTime.UtcNow;

        await _db.Updateable(entity).ExecuteCommandAsync();

        _logger.LogInformation("标记留言为已读：{Id}", id);

        return true;
    }

    /// <summary>
    /// 回复留言
    /// </summary>
    /// <param name="dto">回复参数</param>
    /// <param name="userId">操作人ID</param>
    /// <returns>是否成功</returns>
    public async Task<bool> ReplyContactAsync(ReplyContactDto dto, string userId)
    {
        var entity = await _db.Queryable<site_contact>()
            .Where(x => x.Id.ToString() == dto.Id && x.IsDeleted == 0)
            .FirstAsync();

        if (entity == null)
        {
            throw new BusinessException("留言不存在", 404);
        }

        // 只有"未读"或"已读"状态可以回复
        if (entity.Status != ContactStatus.Unread && entity.Status != ContactStatus.Read)
        {
            throw new BusinessException("只有未读或已读状态的留言可以回复");
        }

        entity.Status = ContactStatus.Replied;
        entity.Reply = dto.Reply;
        entity.ReplyEn = dto.ReplyEn;
        entity.RepliedAt = DateTime.UtcNow;
        entity.RepliedBy = userId;
        entity.UpdatedAt = DateTime.UtcNow;

        await _db.Updateable(entity).ExecuteCommandAsync();

        _logger.LogInformation("回复留言成功：{Id}, 操作人：{UserId}", dto.Id, userId);

        return true;
    }

    /// <summary>
    /// 删除留言
    /// </summary>
    /// <param name="id">留言ID</param>
    /// <returns>是否成功</returns>
    public async Task<bool> DeleteContactAsync(string id)
    {
        var entity = await _db.Queryable<site_contact>()
            .Where(x => x.Id.ToString() == id && x.IsDeleted == 0)
            .FirstAsync();

        if (entity == null)
        {
            throw new BusinessException("留言不存在", 404);
        }

        // 软删除
        entity.IsDeleted = 1;
        entity.UpdatedAt = DateTime.UtcNow;

        await _db.Updateable(entity).ExecuteCommandAsync();

        _logger.LogInformation("删除留言成功：{Id}", id);

        return true;
    }

    /// <summary>
    /// 获取未读留言数量
    /// </summary>
    /// <returns>未读数量</returns>
    public async Task<int> GetUnreadCountAsync()
    {
        var count = await _db.Queryable<site_contact>()
            .Where(x => x.IsDeleted == 0 && x.Status == ContactStatus.Unread)
            .CountAsync();

        return count;
    }
}