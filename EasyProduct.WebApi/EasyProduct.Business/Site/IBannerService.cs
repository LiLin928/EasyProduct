using EasyProduct.Models.Dto.Site.Banner;

namespace EasyProduct.Business.Site;

/// <summary>
/// Banner服务接口（管理端）
/// </summary>
/// <remarks>
/// 提供Banner的增删改查等功能，供管理端使用
/// </remarks>
public interface IBannerService
{
    /// <summary>
    /// 获取Banner列表（支持分页、筛选）
    /// </summary>
    /// <param name="query">查询参数，包含分页、标题、显示位置、状态、时间范围筛选</param>
    /// <returns>Banner列表</returns>
    Task<List<BannerDto>> GetListAsync(BannerQueryDto query);

    /// <summary>
    /// 获取Banner详情
    /// </summary>
    /// <param name="id">BannerID</param>
    /// <returns>Banner详情</returns>
    Task<BannerDto> GetByIdAsync(string id);

    /// <summary>
    /// 创建Banner
    /// </summary>
    /// <param name="dto">创建参数</param>
    /// <returns>新BannerID</returns>
    Task<string> CreateAsync(CreateBannerDto dto);

    /// <summary>
    /// 更新Banner
    /// </summary>
    /// <param name="dto">更新参数</param>
    /// <returns>是否成功</returns>
    Task<bool> UpdateAsync(UpdateBannerDto dto);

    /// <summary>
    /// 删除Banner
    /// </summary>
    /// <param name="id">BannerID</param>
    /// <returns>是否成功</returns>
    Task<bool> DeleteAsync(string id);

    /// <summary>
    /// 更新Banner状态
    /// </summary>
    /// <param name="id">BannerID</param>
    /// <param name="status">状态：0=禁用，1=启用</param>
    /// <returns>是否成功</returns>
    Task<bool> UpdateStatusAsync(string id, int status);
}