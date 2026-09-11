using EasyProduct.Models.Dto.Site.About;

namespace EasyProduct.Business.Site;

/// <summary>
/// 关于我们服务接口（管理端）
/// </summary>
/// <remarks>
/// 提供关于我们的获取和更新功能，供管理端使用
/// 单页内容，不支持创建和删除
/// </remarks>
public interface IAboutService
{
    /// <summary>
    /// 获取关于我们详情
    /// </summary>
    /// <param name="id">关于我们ID</param>
    /// <returns>关于我们详情</returns>
    Task<AboutDto> GetByIdAsync(string id);

    /// <summary>
    /// 更新关于我们
    /// </summary>
    /// <param name="dto">更新参数</param>
    /// <returns>是否成功</returns>
    Task<bool> UpdateAsync(UpdateAboutDto dto);

    /// <summary>
    /// 更新关于我们状态
    /// </summary>
    /// <param name="id">关于我们ID</param>
    /// <param name="status">状态：0=禁用，1=启用</param>
    /// <returns>是否成功</returns>
    Task<bool> UpdateStatusAsync(string id, int status);
}