using EasyProduct.Common.Base;
using EasyProduct.Models.Dto.Product.Spu;

namespace EasyProduct.Business.Product;

/// <summary>
/// 商品主档服务接口
/// </summary>
/// <remarks>
/// 提供商品主档（SPU）的增删改查、分页查询、状态更新等功能
/// </remarks>
public interface ISpuService
{
    /// <summary>
    /// 获取商品主档分页列表
    /// </summary>
    /// <param name="query">查询参数，支持商品名称、商品编码、分类、类型、状态筛选</param>
    /// <returns>商品主档分页列表</returns>
    Task<PageResponse<SpuDto>> GetSpuPageListAsync(SpuQueryDto query);

    /// <summary>
    /// 获取商品主档详情
    /// </summary>
    /// <param name="id">商品ID</param>
    /// <returns>商品主档详情</returns>
    /// <exception cref="Common.Error.BusinessException">商品不存在时抛出</exception>
    Task<SpuDto> GetSpuByIdAsync(string id);

    /// <summary>
    /// 按分类查询商品主档列表
    /// </summary>
    /// <param name="categoryId">分类ID</param>
    /// <returns>商品主档列表</returns>
    /// <remarks>
    /// 查询指定分类下的所有商品（包含子分类的商品需递归查询）
    /// </remarks>
    Task<List<SpuDto>> GetSpuListByCategoryAsync(string categoryId);

    /// <summary>
    /// 创建商品主档
    /// </summary>
    /// <param name="dto">创建商品主档参数</param>
    /// <returns>创建成功返回 true</returns>
    /// <exception cref="Common.Error.BusinessException">商品编码已存在时抛出</exception>
    Task<bool> CreateSpuAsync(CreateSpuDto dto);

    /// <summary>
    /// 更新商品主档
    /// </summary>
    /// <param name="dto">更新商品主档参数</param>
    /// <returns>更新成功返回 true</returns>
    /// <exception cref="Common.Error.BusinessException">商品不存在、商品编码已存在时抛出</exception>
    Task<bool> UpdateSpuAsync(UpdateSpuDto dto);

    /// <summary>
    /// 删除商品主档（软删除）
    /// </summary>
    /// <param name="id">商品ID</param>
    /// <returns>删除成功返回 true</returns>
    /// <exception cref="Common.Error.BusinessException">商品不存在、存在关联SKU时抛出</exception>
    Task<bool> DeleteSpuAsync(string id);

    /// <summary>
    /// 更新商品主档状态
    /// </summary>
    /// <param name="id">商品ID</param>
    /// <param name="status">状态值：0=禁用，1=启用</param>
    /// <returns>更新成功返回 true</returns>
    /// <exception cref="Common.Error.BusinessException">商品不存在时抛出</exception>
    Task<bool> UpdateSpuStatusAsync(string id, int status);
}