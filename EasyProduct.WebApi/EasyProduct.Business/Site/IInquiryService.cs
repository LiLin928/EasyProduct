using EasyProduct.Common.Base;
using EasyProduct.Models.Dto.Site.Inquiry;

namespace EasyProduct.Business.Site;

/// <summary>
/// 询价单服务接口（管理端）
/// </summary>
/// <remarks>
/// 提供询价单的增删改查、跟进、转客户、关闭等功能
/// 管理端使用，需要 Admin JWT 认证
/// </remarks>
public interface IInquiryService
{
    /// <summary>
    /// 获取询价单分页列表
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>询价单分页结果</returns>
    Task<PageResponse<InquiryDto>> GetInquiryListAsync(InquiryQueryDto query);

    /// <summary>
    /// 获取询价单详情
    /// </summary>
    /// <param name="id">询价单ID</param>
    /// <returns>询价单详情</returns>
    Task<InquiryDto> GetInquiryByIdAsync(string id);

    /// <summary>
    /// 跟进询价单
    /// </summary>
    /// <param name="id">询价单ID</param>
    /// <param name="userId">操作人ID</param>
    /// <returns>是否成功</returns>
    Task<bool> FollowInquiryAsync(string id, string userId);

    /// <summary>
    /// 询价单转客户
    /// </summary>
    /// <param name="id">询价单ID</param>
    /// <param name="userId">操作人ID</param>
    /// <returns>新客户ID</returns>
    Task<string> ConvertToCustomerAsync(string id, string userId);

    /// <summary>
    /// 关闭询价单
    /// </summary>
    /// <param name="id">询价单ID</param>
    /// <param name="reason">关闭原因</param>
    /// <param name="userId">操作人ID</param>
    /// <returns>是否成功</returns>
    Task<bool> CloseInquiryAsync(string id, string reason, string userId);

    /// <summary>
    /// 删除询价单
    /// </summary>
    /// <param name="id">询价单ID</param>
    /// <returns>是否成功</returns>
    Task<bool> DeleteInquiryAsync(string id);
}