using EasyProduct.Models.Dto.Product.Channel;

namespace EasyProduct.Business.Product;

/// <summary>
/// 渠道发布服务接口
/// </summary>
/// <remarks>
/// 提供商品渠道发布的创建、管理、上架状态控制等功能
/// 支持三端渠道：官网（site）、小程序（miniapp）、B2B（b2b）
/// </remarks>
public interface IChannelService
{
    /// <summary>
    /// 获取渠道发布列表
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>渠道发布列表</returns>
    /// <remarks>
    /// 根据查询参数获取渠道发布列表
    /// 支持按商品ID、渠道编码、上架状态筛选
    /// 结果按排序字段和创建时间排序
    /// </remarks>
    Task<List<ChannelDto>> GetChannelListAsync(ChannelQueryDto query);

    /// <summary>
    /// 获取渠道发布详情
    /// </summary>
    /// <param name="id">渠道发布ID</param>
    /// <returns>渠道发布详情</returns>
    /// <remarks>
    /// 根据ID获取渠道发布的详细信息
    /// 如果渠道发布不存在，抛出 BusinessException 异常
    /// </remarks>
    Task<ChannelDto> GetChannelByIdAsync(string id);

    /// <summary>
    /// 获取商品的渠道发布情况
    /// </summary>
    /// <param name="spuId">商品ID</param>
    /// <returns>渠道发布列表</returns>
    /// <remarks>
    /// 获取指定商品在所有渠道的发布情况
    /// </remarks>
    Task<List<ChannelDto>> GetSpuChannelsAsync(string spuId);

    /// <summary>
    /// 创建渠道发布
    /// </summary>
    /// <param name="dto">创建参数</param>
    /// <returns>新渠道发布ID</returns>
    /// <remarks>
    /// 创建新的渠道发布
    /// 验证商品是否存在，不存在则抛出 BusinessException 异常
    /// 验证渠道编码是否有效，无效则抛出 BusinessException 异常
    /// 检查是否已发布到该渠道，已发布则抛出 BusinessException 异常
    /// 如果上架，自动设置发布时间
    /// </remarks>
    Task<string> CreateChannelAsync(CreateChannelDto dto);

    /// <summary>
    /// 批量发布到渠道
    /// </summary>
    /// <param name="dto">批量发布参数</param>
    /// <returns>成功发布的数量</returns>
    /// <remarks>
    /// 批量将商品发布到多个渠道
    /// 自动跳过无效的渠道编码和已发布的渠道
    /// 验证商品是否存在，不存在则抛出 BusinessException 异常
    /// </remarks>
    Task<int> BatchPublishAsync(BatchPublishDto dto);

    /// <summary>
    /// 更新渠道发布
    /// </summary>
    /// <param name="dto">更新参数</param>
    /// <returns>是否成功</returns>
    /// <remarks>
    /// 更新已存在的渠道发布信息
    /// 如果从未上架变为上架，自动设置发布时间
    /// 如果从上架变为下架，自动设置下架时间
    /// 如果渠道发布不存在，抛出 BusinessException 异常
    /// </remarks>
    Task<bool> UpdateChannelAsync(UpdateChannelDto dto);

    /// <summary>
    /// 删除渠道发布
    /// </summary>
    /// <param name="id">渠道发布ID</param>
    /// <returns>是否成功</returns>
    /// <remarks>
    /// 删除指定的渠道发布（软删除）
    /// 如果渠道发布不存在，抛出 BusinessException 异常
    /// </remarks>
    Task<bool> DeleteChannelAsync(string id);

    /// <summary>
    /// 更新上架状态
    /// </summary>
    /// <param name="id">渠道发布ID</param>
    /// <param name="status">上架状态：0=下架，1=上架</param>
    /// <returns>是否成功</returns>
    /// <remarks>
    /// 更新渠道发布的上架状态
    /// 如果上架，自动设置发布时间
    /// 如果下架，自动设置下架时间
    /// 如果渠道发布不存在，抛出 BusinessException 异常
    /// </remarks>
    Task<bool> UpdateChannelStatusAsync(string id, int status);

    /// <summary>
    /// 批量更新上架状态
    /// </summary>
    /// <param name="ids">渠道发布ID列表</param>
    /// <param name="status">上架状态：0=下架，1=上架</param>
    /// <returns>成功更新的数量</returns>
    /// <remarks>
    /// 批量更新渠道发布的上架状态
    /// 只更新存在的渠道发布，忽略不存在的ID
    /// </remarks>
    Task<int> BatchUpdateStatusAsync(List<string> ids, int status);
}