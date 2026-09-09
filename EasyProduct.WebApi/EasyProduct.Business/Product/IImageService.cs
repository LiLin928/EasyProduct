using EasyProduct.Models.Dto.Product.Image;

namespace EasyProduct.Business.Product;

/// <summary>
/// 商品图片服务接口
/// </summary>
/// <remarks>
/// 提供商品图片的上传、管理、排序、删除等功能
/// 支持单张和批量操作，支持主图设置和排序管理
/// </remarks>
public interface IImageService
{
    /// <summary>
    /// 获取图片列表
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>图片列表</returns>
    /// <remarks>
    /// 根据查询参数获取图片列表
    /// 支持按商品ID筛选、按是否主图筛选
    /// 结果按排序字段和创建时间排序
    /// </remarks>
    Task<List<ImageDto>> GetImageListAsync(ImageQueryDto query);

    /// <summary>
    /// 获取图片详情
    /// </summary>
    /// <param name="id">图片ID</param>
    /// <returns>图片详情</returns>
    /// <remarks>
    /// 根据ID获取图片的详细信息
    /// 如果图片不存在，抛出 BusinessException 异常
    /// </remarks>
    Task<ImageDto> GetImageByIdAsync(string id);

    /// <summary>
    /// 创建图片
    /// </summary>
    /// <param name="dto">创建参数</param>
    /// <returns>新图片ID</returns>
    /// <remarks>
    /// 创建新的商品图片
    /// 如果设置为主图，会自动清除该商品的其他主图标记
    /// 验证商品是否存在，不存在则抛出 BusinessException 异常
    /// </remarks>
    Task<string> CreateImageAsync(CreateImageDto dto);

    /// <summary>
    /// 批量创建图片
    /// </summary>
    /// <param name="spuId">商品ID</param>
    /// <param name="images">图片列表</param>
    /// <returns>成功创建的数量</returns>
    /// <remarks>
    /// 批量创建商品图片
    /// 自动设置排序值（按列表顺序）
    /// 验证商品是否存在，不存在则抛出 BusinessException 异常
    /// 如果列表中有主图，会自动清除该商品的其他主图标记
    /// </remarks>
    Task<int> BatchCreateImagesAsync(string spuId, List<CreateImageDto> images);

    /// <summary>
    /// 更新图片
    /// </summary>
    /// <param name="dto">更新参数</param>
    /// <returns>是否成功</returns>
    /// <remarks>
    /// 更新已存在的图片信息
    /// 如果设置为主图，会自动清除该商品的其他主图标记
    /// 如果图片不存在，抛出 BusinessException 异常
    /// </remarks>
    Task<bool> UpdateImageAsync(UpdateImageDto dto);

    /// <summary>
    /// 删除图片
    /// </summary>
    /// <param name="id">图片ID</param>
    /// <returns>是否成功</returns>
    /// <remarks>
    /// 删除指定的商品图片（软删除）
    /// 如果图片不存在，抛出 BusinessException 异常
    /// </remarks>
    Task<bool> DeleteImageAsync(string id);

    /// <summary>
    /// 批量删除图片
    /// </summary>
    /// <param name="ids">图片ID列表</param>
    /// <returns>成功删除的数量</returns>
    /// <remarks>
    /// 批量删除商品图片（软删除）
    /// 只删除存在的图片，忽略不存在的ID
    /// </remarks>
    Task<int> BatchDeleteImagesAsync(List<string> ids);

    /// <summary>
    /// 设置主图
    /// </summary>
    /// <param name="id">图片ID</param>
    /// <returns>是否成功</returns>
    /// <remarks>
    /// 将指定图片设置为商品的主图
    /// 自动清除该商品的其他主图标记
    /// 如果图片不存在，抛出 BusinessException 异常
    /// </remarks>
    Task<bool> SetMainImageAsync(string id);

    /// <summary>
    /// 更新图片排序
    /// </summary>
    /// <param name="imageId">图片ID</param>
    /// <param name="sort">排序值</param>
    /// <returns>是否成功</returns>
    /// <remarks>
    /// 更新图片的排序值
    /// 如果图片不存在，抛出 BusinessException 异常
    /// </remarks>
    Task<bool> UpdateImageSortAsync(string imageId, int sort);
}