using EasyProduct.Business.Site;
using EasyProduct.Common.Base;
using EasyProduct.Models.Dto.Site.Contact;
using EasyProduct.Web.Controllers.Site.Base;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace EasyProduct.Web.Controllers.Site;

/// <summary>
/// 留言控制器（官网公开）
/// </summary>
/// <remarks>
/// 提供官网公开的留言提交功能
/// 官网接口，允许匿名访问
/// POST 接口使用限流保护：每 IP 每分钟最多 10 次
/// </remarks>
public class ContactController : SiteControllerBase
{
    private readonly ISiteContactService _siteContactService;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="siteContactService">官网留言服务</param>
    public ContactController(ISiteContactService siteContactService)
    {
        _siteContactService = siteContactService;
    }

    /// <summary>
    /// 提交留言
    /// </summary>
    /// <param name="dto">创建留言参数</param>
    /// <returns>留言ID</returns>
    /// <remarks>
    /// 提交新的留言
    /// 需要提供姓名、联系方式和留言内容
    /// 限流保护：每 IP 每分钟最多 10 次
    /// </remarks>
    [HttpPost]
    [EnableRateLimiting("SiteSubmitPolicy")]
    public async Task<ApiResponse<string>> SubmitContact([FromBody] CreateContactDto dto)
    {
        var result = await _siteContactService.SubmitContactAsync(dto);
        return Success(result, "留言提交成功");
    }
}