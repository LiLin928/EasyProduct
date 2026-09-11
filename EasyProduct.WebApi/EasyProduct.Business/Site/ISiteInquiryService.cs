using EasyProduct.Common.Base;
using EasyProduct.Models.Dto.Site.Inquiry;

namespace EasyProduct.Business.Site;

/// <summary>
/// 询价单服务接口（官网公开）
/// </summary>
/// <remarks>
/// 提供官网公开的询价单提交功能
/// 官网使用，允许匿名访问，但需要限流保护
/// </remarks>
public interface ISiteInquiryService
{
    /// <summary>
    /// 提交询价单
    /// </summary>
    /// <param name="dto">创建询价单参数</param>
    /// <returns>询价单ID</returns>
    Task<string> SubmitInquiryAsync(CreateInquiryDto dto);
}