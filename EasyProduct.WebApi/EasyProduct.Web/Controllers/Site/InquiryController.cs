using EasyProduct.Business.Site;
using EasyProduct.Common.Base;
using EasyProduct.Models.Dto.Site.Inquiry;
using EasyProduct.Web.Controllers.Site.Base;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace EasyProduct.Web.Controllers.Site;

/// <summary>
/// 询价单控制器（官网公开）
/// </summary>
/// <remarks>
/// 提供官网公开的询价单提交功能
/// 官网接口，允许匿名访问
/// POST 接口使用限流保护：每 IP 每分钟最多 10 次
/// </remarks>
public class InquiryController : SiteControllerBase
{
    private readonly ISiteInquiryService _siteInquiryService;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="siteInquiryService">官网询价单服务</param>
    public InquiryController(ISiteInquiryService siteInquiryService)
    {
        _siteInquiryService = siteInquiryService;
    }

    /// <summary>
    /// 提交询价单
    /// </summary>
    /// <param name="dto">创建询价单参数</param>
    /// <returns>询价单ID</returns>
    /// <remarks>
    /// 提交新的询价单
    /// 需要提供公司信息、联系人信息和询价明细
    /// 自动生成询价单号
    /// 限流保护：每 IP 每分钟最多 10 次
    /// </remarks>
    [HttpPost]
    [EnableRateLimiting("SiteSubmitPolicy")]
    public async Task<ApiResponse<string>> SubmitInquiry([FromBody] CreateInquiryDto dto)
    {
        var result = await _siteInquiryService.SubmitInquiryAsync(dto);
        return Success(result, "询价单提交成功");
    }
}