using EasyProduct.Common.Base;
using EasyProduct.Common.Error;
using EasyProduct.Models.Dto.Site.Inquiry;
using EasyProduct.Models.Entitys.Site;
using EasyProduct.Models.Enums.Site;
using Mapster;
using Microsoft.Extensions.Logging;
using SqlSugar;
using System.Transactions;

namespace EasyProduct.Business.Site;

/// <summary>
/// 询价单服务实现（管理端）
/// </summary>
/// <remarks>
/// 提供询价单的增删改查、跟进、转客户、关闭等功能
/// 管理端使用，需要 Admin JWT 认证
/// </remarks>
public class InquiryService : BaseService, IInquiryService
{
    private readonly ILogger<InquiryService> _logger;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="logger">日志服务</param>
    public InquiryService(ILogger<InquiryService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// 获取询价单分页列表
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>询价单分页结果</returns>
    public async Task<PageResponse<InquiryDto>> GetInquiryListAsync(InquiryQueryDto query)
    {
        var queryable = _db.Queryable<site_inquiry>()
            .Where(x => x.IsDeleted == 0);

        // 询价单号查询
        if (!string.IsNullOrEmpty(query.InquiryNo))
        {
            queryable = queryable.Where(x => x.InquiryNo == query.InquiryNo);
        }

        // 公司名称模糊搜索
        if (!string.IsNullOrEmpty(query.CompanyName))
        {
            queryable = queryable.Where(x => x.CompanyName.Contains(query.CompanyName) ||
                                              (x.CompanyNameEn != null && x.CompanyNameEn.Contains(query.CompanyName)));
        }

        // 联系人姓名模糊搜索
        if (!string.IsNullOrEmpty(query.ContactName))
        {
            queryable = queryable.Where(x => x.ContactName.Contains(query.ContactName) ||
                                              (x.ContactNameEn != null && x.ContactNameEn.Contains(query.ContactName)));
        }

        // 联系电话模糊搜索
        if (!string.IsNullOrEmpty(query.Phone))
        {
            queryable = queryable.Where(x => x.Phone.Contains(query.Phone));
        }

        // 电子邮箱模糊搜索
        if (!string.IsNullOrEmpty(query.Email))
        {
            queryable = queryable.Where(x => x.Email != null && x.Email.Contains(query.Email));
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
        var dtoList = list.Adapt<List<InquiryDto>>();

        // 查询每个询价单的明细
        foreach (var dto in dtoList)
        {
            var items = await _db.Queryable<site_inquiry_item>()
                .Where(x => x.InquiryId == dto.Id && x.IsDeleted == 0)
                .ToListAsync();
            dto.Items = items.Adapt<List<InquiryItemDto>>();
        }

        return new PageResponse<InquiryDto>
        {
            List = dtoList,
            Total = total,
            PageIndex = query.PageIndex,
            PageSize = query.PageSize
        };
    }

    /// <summary>
    /// 获取询价单详情
    /// </summary>
    /// <param name="id">询价单ID</param>
    /// <returns>询价单详情</returns>
    public async Task<InquiryDto> GetInquiryByIdAsync(string id)
    {
        var entity = await _db.Queryable<site_inquiry>()
            .Where(x => x.Id.ToString() == id && x.IsDeleted == 0)
            .FirstAsync();

        if (entity == null)
        {
            throw new BusinessException("询价单不存在", 404);
        }

        var dto = entity.Adapt<InquiryDto>();

        // 查询询价明细
        var items = await _db.Queryable<site_inquiry_item>()
            .Where(x => x.InquiryId == id && x.IsDeleted == 0)
            .ToListAsync();
        dto.Items = items.Adapt<List<InquiryItemDto>>();

        return dto;
    }

    /// <summary>
    /// 跟进询价单
    /// </summary>
    /// <param name="id">询价单ID</param>
    /// <param name="userId">操作人ID</param>
    /// <returns>是否成功</returns>
    public async Task<bool> FollowInquiryAsync(string id, string userId)
    {
        var entity = await _db.Queryable<site_inquiry>()
            .Where(x => x.Id.ToString() == id && x.IsDeleted == 0)
            .FirstAsync();

        if (entity == null)
        {
            throw new BusinessException("询价单不存在", 404);
        }

        // 只有"待处理"状态可以跟进
        if (entity.Status != InquiryStatus.Pending)
        {
            throw new BusinessException("只有待处理状态的询价单可以跟进");
        }

        entity.Status = InquiryStatus.Followed;
        entity.FollowedAt = DateTime.UtcNow;
        entity.FollowedBy = userId;
        entity.UpdatedAt = DateTime.UtcNow;
        entity.UpdatedBy = userId;

        await _db.Updateable(entity).ExecuteCommandAsync();

        _logger.LogInformation("跟进询价单成功：{InquiryNo}, 操作人：{UserId}", entity.InquiryNo, userId);

        return true;
    }

    /// <summary>
    /// 询价单转客户
    /// </summary>
    /// <param name="id">询价单ID</param>
    /// <param name="userId">操作人ID</param>
    /// <returns>新客户ID</returns>
    public async Task<string> ConvertToCustomerAsync(string id, string userId)
    {
        var entity = await _db.Queryable<site_inquiry>()
            .Where(x => x.Id.ToString() == id && x.IsDeleted == 0)
            .FirstAsync();

        if (entity == null)
        {
            throw new BusinessException("询价单不存在", 404);
        }

        // 只有"已跟进"状态可以转客户
        if (entity.Status != InquiryStatus.Followed)
        {
            throw new BusinessException("只有已跟进状态的询价单可以转客户");
        }

        // 检查是否已经转为客户
        if (!string.IsNullOrEmpty(entity.CustomerId))
        {
            throw new BusinessException("该询价单已转为客户，不能重复操作");
        }

        // 使用事务处理
        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        try
        {
            // TODO: 调用 CRM 模块的 ICustomerService 创建客户档案
            // 暂时生成一个模拟的客户ID
            var customerId = Guid.NewGuid().ToString();

            // 更新询价单状态
            entity.Status = InquiryStatus.Converted;
            entity.CustomerId = customerId;
            entity.ConvertedAt = DateTime.UtcNow;
            entity.ConvertedBy = userId;
            entity.UpdatedAt = DateTime.UtcNow;
            entity.UpdatedBy = userId;

            await _db.Updateable(entity).ExecuteCommandAsync();

            scope.Complete();

            _logger.LogInformation("询价单转客户成功：{InquiryNo}, 客户ID：{CustomerId}, 操作人：{UserId}",
                entity.InquiryNo, customerId, userId);

            return customerId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "询价单转客户失败：{InquiryNo}", entity.InquiryNo);
            throw new BusinessException("询价单转客户失败，请稍后重试");
        }
    }

    /// <summary>
    /// 关闭询价单
    /// </summary>
    /// <param name="id">询价单ID</param>
    /// <param name="reason">关闭原因</param>
    /// <param name="userId">操作人ID</param>
    /// <returns>是否成功</returns>
    public async Task<bool> CloseInquiryAsync(string id, string reason, string userId)
    {
        var entity = await _db.Queryable<site_inquiry>()
            .Where(x => x.Id.ToString() == id && x.IsDeleted == 0)
            .FirstAsync();

        if (entity == null)
        {
            throw new BusinessException("询价单不存在", 404);
        }

        // 只有"待处理"或"已跟进"状态可以关闭
        if (entity.Status != InquiryStatus.Pending && entity.Status != InquiryStatus.Followed)
        {
            throw new BusinessException("只有待处理或已跟进状态的询价单可以关闭");
        }

        entity.Status = InquiryStatus.Closed;
        entity.CloseReason = reason;
        entity.ClosedAt = DateTime.UtcNow;
        entity.ClosedBy = userId;
        entity.UpdatedAt = DateTime.UtcNow;
        entity.UpdatedBy = userId;

        await _db.Updateable(entity).ExecuteCommandAsync();

        _logger.LogInformation("关闭询价单成功：{InquiryNo}, 原因：{Reason}, 操作人：{UserId}",
            entity.InquiryNo, reason, userId);

        return true;
    }

    /// <summary>
    /// 删除询价单
    /// </summary>
    /// <param name="id">询价单ID</param>
    /// <returns>是否成功</returns>
    public async Task<bool> DeleteInquiryAsync(string id)
    {
        var entity = await _db.Queryable<site_inquiry>()
            .Where(x => x.Id.ToString() == id && x.IsDeleted == 0)
            .FirstAsync();

        if (entity == null)
        {
            throw new BusinessException("询价单不存在", 404);
        }

        // 软删除询价单
        entity.IsDeleted = 1;
        entity.UpdatedAt = DateTime.UtcNow;

        await _db.Updateable(entity).ExecuteCommandAsync();

        // 软删除询价明细
        await _db.Updateable<site_inquiry_item>()
            .Where(x => x.InquiryId == id)
            .SetColumns(x => x.IsDeleted == 1)
            .SetColumns(x => x.UpdatedAt == DateTime.UtcNow)
            .ExecuteCommandAsync();

        _logger.LogInformation("删除询价单成功：{InquiryNo}", entity.InquiryNo);

        return true;
    }
}