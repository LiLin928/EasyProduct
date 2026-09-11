using EasyProduct.Common.Base;
using EasyProduct.Models.Dto.Site.Contact;

namespace EasyProduct.Business.Site;

/// <summary>
/// 留言服务接口（官网公开）
/// </summary>
/// <remarks>
/// 提供官网公开的留言提交功能
/// 官网使用，允许匿名访问，但需要限流保护
/// </remarks>
public interface ISiteContactService
{
    /// <summary>
    /// 提交留言
    /// </summary>
    /// <param name="dto">创建留言参数</param>
    /// <returns>留言ID</returns>
    Task<string> SubmitContactAsync(CreateContactDto dto);
}