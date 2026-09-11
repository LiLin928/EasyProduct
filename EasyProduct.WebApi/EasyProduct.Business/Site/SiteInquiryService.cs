using EasyProduct.Common.Base;
using EasyProduct.Common.Error;
using EasyProduct.Models.Dto.Site.Inquiry;
using EasyProduct.Models.Entitys.Site;
using EasyProduct.Models.Enums.Site;
using Mapster;
using Microsoft.Extensions.Logging;
using SqlSugar;

namespace EasyProduct.Business.Site;

/// <summary>
/// 询价单服务实现（官网公开）
/// </summary>
/// <remarks>
/// 提供官网公开的询价单提交功能
/// 官网使用，允许匿名访问，但需要限流保护
/// </remarks>
public class SiteInquiryService : BaseService, ISiteInquiryService
{
    private readonly ILogger<SiteInquiryService> _logger;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="logger">日志服务</param>
    public SiteInquiryService(ILogger<SiteInquiryService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// 提交询价单
    /// </summary>
    /// <param name="dto">创建询价单参数</param>
    /// <returns>询价单ID</returns>
    public async Task<string> SubmitInquiryAsync(CreateInquiryDto dto)
    {
        // 生成询价单号
        var inquiryNo = await GenerateInquiryNoAsync();

        // 创建询价单
        var entity = dto.Adapt<site_inquiry>();
        entity.Id = Guid.NewGuid();
        entity.InquiryNo = inquiryNo;
        entity.Status = InquiryStatus.Pending;
        entity.CreatedAt = DateTime.UtcNow;

        // 使用事务插入询价单和明细
        try
        {
            _db.Ado.BeginTran();

            // 插入询价单
            await _db.Insertable(entity).ExecuteCommandAsync();

            // 插入询价明细
            if (dto.Items != null && dto.Items.Count > 0)
            {
                var items = dto.Items.Adapt<List<site_inquiry_item>>();
                foreach (var item in items)
                {
                    item.Id = Guid.NewGuid();
                    item.InquiryId = entity.Id.ToString();
                    item.InquiryNo = inquiryNo;
                    item.CreatedAt = DateTime.UtcNow;
                }
                await _db.Insertable(items).ExecuteCommandAsync();
            }

            _db.Ado.CommitTran();

            _logger.LogInformation("提交询价单成功：{InquiryNo}", inquiryNo);

            return entity.Id.ToString();
        }
        catch (Exception ex)
        {
            _db.Ado.RollbackTran();
            _logger.LogError(ex, "提交询价单失败");
            throw new BusinessException("提交询价单失败，请稍后重试");
        }
    }

    /// <summary>
    /// 生成询价单号
    /// </summary>
    /// <remarks>
    /// 格式：INQ + yyyyMMdd + 4位序号
    /// 例如：INQ202609110001
    /// </remarks>
    /// <returns>询价单号</returns>
    private async Task<string> GenerateInquiryNoAsync()
    {
        var today = DateTime.UtcNow.ToString("yyyyMMdd");
        var prefix = $"INQ{today}";

        // 查询当天最大的序号
        var maxNo = await _db.Queryable<site_inquiry>()
            .Where(x => x.InquiryNo.StartsWith(prefix))
            .OrderBy(x => x.InquiryNo, OrderByType.Desc)
            .Select(x => x.InquiryNo)
            .FirstAsync();

        var nextSeq = 1;
        if (!string.IsNullOrEmpty(maxNo) && maxNo.Length == 14)
        {
            // 提取序号部分
            var seqStr = maxNo.Substring(10);
            if (int.TryParse(seqStr, out var seq))
            {
                nextSeq = seq + 1;
            }
        }

        // 格式化为 4 位序号
        var inquiryNo = $"{prefix}{nextSeq:D4}";

        return inquiryNo;
    }
}