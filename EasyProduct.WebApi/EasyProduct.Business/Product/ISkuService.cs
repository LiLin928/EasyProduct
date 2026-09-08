using EasyProduct.Common.Base;
using EasyProduct.Models.Dto.Product.Sku;

namespace EasyProduct.Business.Product;

/// <summary>
/// 商品SKU服务接口
/// </summary>
/// <remarks>
/// 提供商品SKU（库存量单位）的增删改查、分页查询、状态更新、库存更新等功能
/// </remarks>
public interface ISkuService
{
    /// <summary>
    /// 获取商品SKU分页列表
    /// </summary>
    /// <param name="query">查询参数，支持SKU名称、SKU编码、SPU ID、状态筛选</param>
    /// <returns>商品SKU分页列表</returns>
    Task<PageResponse<SkuDto>> GetSkuPageListAsync(SkuQueryDto query);

    /// <summary>
    /// 获取商品SKU详情
    /// </summary>
    /// <param name="id">SKU ID</param>
    /// <returns>商品SKU详情</returns>
    /// <exception cref="Common.Error.BusinessException">SKU不存在时抛出</exception>
    Task<SkuDto> GetSkuByIdAsync(string id);

    /// <summary>
    /// 按SPU查询商品SKU列表
    /// </summary>
    /// <param name="spuId">商品SPU ID</param>
    /// <returns>商品SKU列表</returns>
    /// <remarks>
    /// 查询指定SPU下的所有SKU
    /// </remarks>
    Task<List<SkuDto>> GetSkuListBySpuAsync(string spuId);

    /// <summary>
    /// 创建商品SKU
    /// </summary>
    /// <param name="dto">创建商品SKU参数</param>
    /// <returns>创建成功返回 true</returns>
    /// <exception cref="Common.Error.BusinessException">SKU编码已存在、SPU不存在时抛出</exception>
    Task<bool> CreateSkuAsync(CreateSkuDto dto);

    /// <summary>
    /// 更新商品SKU
    /// </summary>
    /// <param name="dto">更新商品SKU参数</param>
    /// <returns>更新成功返回 true</returns>
    /// <exception cref="Common.Error.BusinessException">SKU不存在、SKU编码已存在、SPU不存在时抛出</exception>
    Task<bool> UpdateSkuAsync(UpdateSkuDto dto);

    /// <summary>
    /// 删除商品SKU（软删除）
    /// </summary>
    /// <param name="id">SKU ID</param>
    /// <returns>删除成功返回 true</returns>
    /// <exception cref="Common.Error.BusinessException">SKU不存在时抛出</exception>
    Task<bool> DeleteSkuAsync(string id);

    /// <summary>
    /// 更新商品SKU状态
    /// </summary>
    /// <param name="id">SKU ID</param>
    /// <param name="status">状态值：0=禁用，1=启用</param>
    /// <returns>更新成功返回 true</returns>
    /// <exception cref="Common.Error.BusinessException">SKU不存在时抛出</exception>
    Task<bool> UpdateSkuStatusAsync(string id, int status);

    /// <summary>
    /// 更新商品SKU库存
    /// </summary>
    /// <param name="id">SKU ID</param>
    /// <param name="stock">库存数量</param>
    /// <returns>更新成功返回 true</returns>
    /// <exception cref="Common.Error.BusinessException">SKU不存在、库存不能为负时抛出</exception>
    /// <remarks>
    /// 更新SKU的库存数量，库存数量必须大于或等于0
    /// </remarks>
    Task<bool> UpdateSkuStockAsync(string id, int stock);
}