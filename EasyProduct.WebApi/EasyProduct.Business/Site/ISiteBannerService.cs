using EasyProduct.Models.Dto.Site.Banner;

namespace EasyProduct.Business.Site;

/// <summary>
/// Banner服务接口（官网公开）
/// </summary>
/// <remarks>
/// 提供Banner的公开查询功能，供官网使用
/// 只返回启用状态且在有效期内的Banner
/// </remarks>
public interface ISiteBannerService
{
    /// <summary>
    /// 获取Banner列表（官网公开，只返回启用且在有效期内的Banner）
    /// </summary>
    /// <param name="position">显示位置，如：home、product_list、detail</param>
    /// <returns>Banner列表</returns>
    Task<List<BannerDto>> GetListAsync(string? position = null);

    /// <summary>
    /// 获取首页Banner列表（官网公开，只返回启用且在有效期内的Banner）
    /// </summary>
    /// <returns>Banner列表</returns>
    Task<List<BannerDto>> GetHomeListAsync();
}