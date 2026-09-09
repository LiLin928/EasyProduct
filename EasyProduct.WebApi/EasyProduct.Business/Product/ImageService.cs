using EasyProduct.Common.Base;
using EasyProduct.Common.Error;
using EasyProduct.Models.Dto.Product.Image;
using EasyProduct.Models.Entitys.Product;
using Mapster;
using Microsoft.Extensions.Logging;
using SqlSugar;

namespace EasyProduct.Business.Product;

/// <summary>
/// 商品图片服务实现
/// </summary>
/// <remarks>
/// 提供商品图片的上传、管理、排序、删除等功能
/// 支持单张和批量操作，支持主图设置和排序管理
/// </remarks>
public class ImageService : BaseService, IImageService
{
    private readonly ILogger<ImageService> _logger;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="logger">日志服务</param>
    public ImageService(ILogger<ImageService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// 获取图片列表
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>图片列表</returns>
    public async Task<List<ImageDto>> GetImageListAsync(ImageQueryDto query)
    {
        var queryable = _db.Queryable<product_image>()
            .Where(x => x.IsDeleted == 0);

        if (!string.IsNullOrEmpty(query.SpuId))
        {
            queryable = queryable.Where(x => x.SpuId == query.SpuId);
        }

        if (query.IsMain.HasValue)
        {
            queryable = queryable.Where(x => x.IsMain == query.IsMain.Value);
        }

        var images = await queryable
            .OrderBy(x => x.Sort)
            .OrderBy(x => x.CreatedAt)
            .ToListAsync();

        return images.Adapt<List<ImageDto>>();
    }

    /// <summary>
    /// 获取图片详情
    /// </summary>
    /// <param name="id">图片ID</param>
    /// <returns>图片详情</returns>
    public async Task<ImageDto> GetImageByIdAsync(string id)
    {
        var image = await _db.Queryable<product_image>()
            .Where(x => x.Id.ToString() == id && x.IsDeleted == 0)
            .FirstAsync();

        if (image == null)
        {
            throw new BusinessException("图片不存在", 404);
        }

        return image.Adapt<ImageDto>();
    }

    /// <summary>
    /// 创建图片
    /// </summary>
    /// <param name="dto">创建参数</param>
    /// <returns>新图片ID</returns>
    public async Task<string> CreateImageAsync(CreateImageDto dto)
    {
        // 验证商品是否存在
        var spu = await _db.Queryable<product_spu>()
            .Where(x => x.Id.ToString() == dto.SpuId && x.IsDeleted == 0)
            .FirstAsync();

        if (spu == null)
        {
            throw new BusinessException("商品不存在", 404);
        }

        // 如果设置为主图，先清除其他主图
        if (dto.IsMain)
        {
            await ClearMainImage(dto.SpuId);
        }

        // 创建图片实体
        var image = dto.Adapt<product_image>();
        image.Id = Guid.NewGuid();
        image.CreatedAt = DateTime.UtcNow;

        // 插入数据库
        await _db.Insertable(image).ExecuteCommandAsync();

        _logger.LogInformation("创建商品图片成功：商品ID: {SpuId}, 图片: {ImageUrl}", dto.SpuId, dto.ImageUrl);

        return image.Id.ToString();
    }

    /// <summary>
    /// 批量创建图片
    /// </summary>
    /// <param name="spuId">商品ID</param>
    /// <param name="images">图片列表</param>
    /// <returns>成功创建的数量</returns>
    public async Task<int> BatchCreateImagesAsync(string spuId, List<CreateImageDto> images)
    {
        // 验证商品是否存在
        var spu = await _db.Queryable<product_spu>()
            .Where(x => x.Id.ToString() == spuId && x.IsDeleted == 0)
            .FirstAsync();

        if (spu == null)
        {
            throw new BusinessException("商品不存在", 404);
        }

        var imageEntities = new List<product_image>();
        for (int i = 0; i < images.Count; i++)
        {
            var image = images[i].Adapt<product_image>();
            image.Id = Guid.NewGuid();
            image.SpuId = spuId;
            image.Sort = i;
            image.CreatedAt = DateTime.UtcNow;

            // 如果设置为主图，先清除其他主图
            if (image.IsMain)
            {
                await ClearMainImage(spuId);
            }

            imageEntities.Add(image);
        }

        // 批量插入
        var count = await _db.Insertable(imageEntities).ExecuteCommandAsync();

        _logger.LogInformation("批量创建商品图片成功：商品ID: {SpuId}, 数量: {Count}", spuId, count);

        return count;
    }

    /// <summary>
    /// 更新图片
    /// </summary>
    /// <param name="dto">更新参数</param>
    /// <returns>是否成功</returns>
    public async Task<bool> UpdateImageAsync(UpdateImageDto dto)
    {
        // 检查图片是否存在
        var image = await _db.Queryable<product_image>()
            .Where(x => x.Id.ToString() == dto.Id && x.IsDeleted == 0)
            .FirstAsync();

        if (image == null)
        {
            throw new BusinessException("图片不存在", 404);
        }

        // 如果设置为主图，先清除其他主图
        if (dto.IsMain)
        {
            await ClearMainImage(image.SpuId);
        }

        // 更新图片信息
        image.ImageUrl = dto.ImageUrl;
        image.ThumbnailUrl = dto.ThumbnailUrl;
        image.ImageName = dto.ImageName;
        image.Sort = dto.Sort;
        image.IsMain = dto.IsMain;
        image.UpdatedAt = DateTime.UtcNow;

        // 更新数据库
        await _db.Updateable(image).ExecuteCommandAsync();

        _logger.LogInformation("更新商品图片成功：ID: {Id}", dto.Id);

        return true;
    }

    /// <summary>
    /// 删除图片
    /// </summary>
    /// <param name="id">图片ID</param>
    /// <returns>是否成功</returns>
    public async Task<bool> DeleteImageAsync(string id)
    {
        // 检查图片是否存在
        var image = await _db.Queryable<product_image>()
            .Where(x => x.Id.ToString() == id && x.IsDeleted == 0)
            .FirstAsync();

        if (image == null)
        {
            throw new BusinessException("图片不存在", 404);
        }

        // 软删除
        image.IsDeleted = 1;
        image.UpdatedAt = DateTime.UtcNow;

        await _db.Updateable(image).ExecuteCommandAsync();

        _logger.LogInformation("删除商品图片成功：ID: {Id}", id);

        return true;
    }

    /// <summary>
    /// 批量删除图片
    /// </summary>
    /// <param name="ids">图片ID列表</param>
    /// <returns>成功删除的数量</returns>
    public async Task<int> BatchDeleteImagesAsync(List<string> ids)
    {
        var images = await _db.Queryable<product_image>()
            .Where(x => ids.Contains(x.Id.ToString()) && x.IsDeleted == 0)
            .ToListAsync();

        if (images.Count == 0)
        {
            return 0;
        }

        // 批量软删除
        foreach (var image in images)
        {
            image.IsDeleted = 1;
            image.UpdatedAt = DateTime.UtcNow;
        }

        var count = await _db.Updateable(images).ExecuteCommandAsync();

        _logger.LogInformation("批量删除商品图片成功：数量: {Count}", count);

        return count;
    }

    /// <summary>
    /// 设置主图
    /// </summary>
    /// <param name="id">图片ID</param>
    /// <returns>是否成功</returns>
    public async Task<bool> SetMainImageAsync(string id)
    {
        // 检查图片是否存在
        var image = await _db.Queryable<product_image>()
            .Where(x => x.Id.ToString() == id && x.IsDeleted == 0)
            .FirstAsync();

        if (image == null)
        {
            throw new BusinessException("图片不存在", 404);
        }

        // 清除该商品的其他主图
        await ClearMainImage(image.SpuId);

        // 设置为主图
        image.IsMain = true;
        image.UpdatedAt = DateTime.UtcNow;

        await _db.Updateable(image).ExecuteCommandAsync();

        _logger.LogInformation("设置主图成功：商品ID: {SpuId}, 图片ID: {Id}", image.SpuId, id);

        return true;
    }

    /// <summary>
    /// 更新图片排序
    /// </summary>
    /// <param name="imageId">图片ID</param>
    /// <param name="sort">排序值</param>
    /// <returns>是否成功</returns>
    public async Task<bool> UpdateImageSortAsync(string imageId, int sort)
    {
        var image = await _db.Queryable<product_image>()
            .Where(x => x.Id.ToString() == imageId && x.IsDeleted == 0)
            .FirstAsync();

        if (image == null)
        {
            throw new BusinessException("图片不存在", 404);
        }

        image.Sort = sort;
        image.UpdatedAt = DateTime.UtcNow;

        await _db.Updateable(image).ExecuteCommandAsync();

        _logger.LogInformation("更新图片排序成功：图片ID: {Id}, 排序: {Sort}", imageId, sort);

        return true;
    }

    /// <summary>
    /// 清除主图标记
    /// </summary>
    /// <param name="spuId">商品ID</param>
    /// <remarks>
    /// 将指定商品的所有图片的主图标记设置为 false
    /// </remarks>
    private async Task ClearMainImage(string spuId)
    {
        await _db.Updateable<product_image>()
            .SetColumns(x => x.IsMain == false)
            .SetColumns(x => x.UpdatedAt == DateTime.UtcNow)
            .Where(x => x.SpuId == spuId && x.IsDeleted == 0)
            .ExecuteCommandAsync();
    }
}