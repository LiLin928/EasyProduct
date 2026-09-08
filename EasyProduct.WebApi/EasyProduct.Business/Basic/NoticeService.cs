using EasyProduct.Common.Base;
using EasyProduct.Common.Error;
using EasyProduct.Models.Dto.Basic.Notice;
using EasyProduct.Models.Entitys.Basic;
using Mapster;
using Microsoft.Extensions.Logging;
using SqlSugar;

namespace EasyProduct.Business.Basic;

/// <summary>
/// 公告服务实现
/// </summary>
/// <remarks>
/// 提供公告的增删改查、发布/取消发布等功能
/// </remarks>
public class NoticeService : BaseService, INoticeService
{
    private readonly ILogger<NoticeService> _logger;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="logger">日志服务</param>
    public NoticeService(ILogger<NoticeService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// 获取公告列表（分页）
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>公告分页列表</returns>
    public async Task<PageResponse<NoticeDto>> GetNoticeListAsync(NoticeQueryDto query)
    {
        var queryable = _db.Queryable<basic_notice>()
            .Where(x => x.IsDeleted == 0);

        // 标题模糊搜索
        if (!string.IsNullOrEmpty(query.NoticeTitle))
        {
            queryable = queryable.Where(x => x.NoticeTitle.Contains(query.NoticeTitle));
        }

        // 公告类型筛选
        if (query.NoticeType.HasValue)
        {
            queryable = queryable.Where(x => x.NoticeType == query.NoticeType.Value);
        }

        // 置顶标记筛选
        if (query.TopFlag.HasValue)
        {
            queryable = queryable.Where(x => x.TopFlag == query.TopFlag.Value);
        }

        // 状态筛选
        if (query.Status.HasValue)
        {
            queryable = queryable.Where(x => (int)x.Status == query.Status.Value);
        }

        // 统计总数
        var total = await queryable.CountAsync();

        // 排序：置顶优先，然后按发布时间倒序
        var list = await queryable
            .OrderByDescending(x => x.TopFlag)
            .OrderByDescending(x => x.PublishTime)
            .OrderByDescending(x => x.CreatedAt)
            .Skip((query.PageIndex - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();

        // 转换为DTO
        var dtoList = list.Adapt<List<NoticeDto>>();

        // 设置公告类型名称
        foreach (var dto in dtoList)
        {
            dto.NoticeTypeName = dto.NoticeType == 1 ? "通知" : "公告";
        }

        return PageResponse<NoticeDto>.Create(dtoList, total, query.PageIndex, query.PageSize);
    }

    /// <summary>
    /// 获取公告详情
    /// </summary>
    /// <param name="id">公告ID</param>
    /// <returns>公告详情</returns>
    public async Task<NoticeDto> GetNoticeByIdAsync(string id)
    {
        var entity = await _db.Queryable<basic_notice>()
            .Where(x => x.Id.ToString() == id && x.IsDeleted == 0)
            .FirstAsync();

        if (entity == null)
        {
            throw new BusinessException("公告不存在", 404);
        }

        var dto = entity.Adapt<NoticeDto>();
        dto.NoticeTypeName = entity.NoticeType == 1 ? "通知" : "公告";

        return dto;
    }

    /// <summary>
    /// 创建公告
    /// </summary>
    /// <param name="dto">创建参数</param>
    /// <returns>新公告ID</returns>
    public async Task<string> CreateNoticeAsync(CreateNoticeDto dto)
    {
        var entity = dto.Adapt<basic_notice>();
        entity.Id = Guid.NewGuid();
        entity.Status = (Models.Enums.Status)dto.Status;
        entity.CreatedAt = DateTime.UtcNow;

        await _db.Insertable(entity).ExecuteCommandAsync();

        _logger.LogInformation("创建公告成功：{NoticeTitle}, ID: {Id}", entity.NoticeTitle, entity.Id);

        return entity.Id.ToString();
    }

    /// <summary>
    /// 更新公告
    /// </summary>
    /// <param name="dto">更新参数</param>
    /// <returns>是否成功</returns>
    public async Task<bool> UpdateNoticeAsync(UpdateNoticeDto dto)
    {
        var entity = await _db.Queryable<basic_notice>()
            .Where(x => x.Id.ToString() == dto.Id && x.IsDeleted == 0)
            .FirstAsync();

        if (entity == null)
        {
            throw new BusinessException("公告不存在", 404);
        }

        entity.NoticeTitle = dto.NoticeTitle;
        entity.NoticeContent = dto.NoticeContent;
        entity.NoticeType = dto.NoticeType;
        entity.TopFlag = dto.TopFlag;
        entity.Status = (Models.Enums.Status)dto.Status;
        entity.UpdatedAt = DateTime.UtcNow;

        await _db.Updateable(entity).ExecuteCommandAsync();

        _logger.LogInformation("更新公告成功：{NoticeTitle}, ID: {Id}", entity.NoticeTitle, entity.Id);

        return true;
    }

    /// <summary>
    /// 删除公告
    /// </summary>
    /// <param name="id">公告ID</param>
    /// <returns>是否成功</returns>
    public async Task<bool> DeleteNoticeAsync(string id)
    {
        var entity = await _db.Queryable<basic_notice>()
            .Where(x => x.Id.ToString() == id && x.IsDeleted == 0)
            .FirstAsync();

        if (entity == null)
        {
            throw new BusinessException("公告不存在", 404);
        }

        // 软删除
        entity.IsDeleted = 1;
        entity.UpdatedAt = DateTime.UtcNow;

        await _db.Updateable(entity).ExecuteCommandAsync();

        _logger.LogInformation("删除公告成功：{NoticeTitle}, ID: {Id}", entity.NoticeTitle, entity.Id);

        return true;
    }

    /// <summary>
    /// 发布公告
    /// </summary>
    /// <param name="id">公告ID</param>
    /// <returns>是否成功</returns>
    public async Task<bool> PublishNoticeAsync(string id)
    {
        var entity = await _db.Queryable<basic_notice>()
            .Where(x => x.Id.ToString() == id && x.IsDeleted == 0)
            .FirstAsync();

        if (entity == null)
        {
            throw new BusinessException("公告不存在", 404);
        }

        if (entity.PublishTime.HasValue)
        {
            throw new BusinessException("公告已发布，无需重复发布");
        }

        entity.PublishTime = DateTime.UtcNow;
        entity.Status = Models.Enums.Status.Enabled;
        entity.UpdatedAt = DateTime.UtcNow;

        await _db.Updateable(entity).ExecuteCommandAsync();

        _logger.LogInformation("发布公告成功：{NoticeTitle}, ID: {Id}", entity.NoticeTitle, entity.Id);

        return true;
    }

    /// <summary>
    /// 取消发布公告
    /// </summary>
    /// <param name="id">公告ID</param>
    /// <returns>是否成功</returns>
    public async Task<bool> UnpublishNoticeAsync(string id)
    {
        var entity = await _db.Queryable<basic_notice>()
            .Where(x => x.Id.ToString() == id && x.IsDeleted == 0)
            .FirstAsync();

        if (entity == null)
        {
            throw new BusinessException("公告不存在", 404);
        }

        if (!entity.PublishTime.HasValue)
        {
            throw new BusinessException("公告未发布");
        }

        entity.PublishTime = null;
        entity.UpdatedAt = DateTime.UtcNow;

        await _db.Updateable(entity).ExecuteCommandAsync();

        _logger.LogInformation("取消发布公告成功：{NoticeTitle}, ID: {Id}", entity.NoticeTitle, entity.Id);

        return true;
    }
}