using EasyProduct.Models.Dto.Site.About;

namespace EasyProduct.Business.Site;

/// <summary>
/// 关于我们服务接口（官网公开）
/// </summary>
/// <remarks>
/// 提供关于我们的公开查询功能，供官网使用
/// 只返回启用状态的关于我们内容
/// </remarks>
public interface ISiteAboutService
{
    /// <summary>
    /// 获取关于我们详情（官网公开，只返回启用状态的内容）
    /// </summary>
    /// <returns>关于我们详情</returns>
    Task<AboutDto> GetAboutAsync();
}