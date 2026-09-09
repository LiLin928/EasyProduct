using EasyProduct.Business.Product;
using EasyProduct.Common.Base;
using EasyProduct.Models.Dto.Product.Image;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyProduct.Web.Controllers.Admin.Product;

/// <summary>
/// 商品图片管理控制器
/// </summary>
/// <remarks>
/// 提供商品图片的上传、管理、排序、删除等功能
/// 管理端接口，需要 Admin JWT 认证
/// API 路由前缀：/api/admin/product/image
/// </remarks>
[ApiController]
[Route("api/admin/product/image")]
[Authorize(AuthenticationSchemes = "AdminJwt")]
public class ImageController : BaseController
{
    private readonly IImageService _imageService;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="imageService">图片服务</param>
    public ImageController(IImageService imageService)
    {
        _imageService = imageService;
    }

    /// <summary>
    /// 获取图片列表
    /// </summary>
    /// <param name="spuId">商品ID</param>
    /// <param name="isMain">是否主图</param>
    /// <returns>图片列表</returns>
    /// <remarks>
    /// 获取指定商品的图片列表
    /// 支持按商品ID和是否主图筛选
    /// 结果按排序字段和创建时间排序
    /// </remarks>
    /// <example>
    /// GET /api/admin/product/image/list?spuId=xxx-xxx-xxx
    /// GET /api/admin/product/image/list?spuId=xxx-xxx-xxx&isMain=true
    /// </example>
    [HttpGet("list")]
    public async Task<ApiResponse<List<ImageDto>>> GetImageList(
        [FromQuery] string? spuId,
        [FromQuery] bool? isMain)
    {
        var query = new ImageQueryDto
        {
            SpuId = spuId,
            IsMain = isMain
        };

        var result = await _imageService.GetImageListAsync(query);
        return Success(result);
    }

    /// <summary>
    /// 获取图片详情
    /// </summary>
    /// <param name="id">图片ID</param>
    /// <returns>图片详情</returns>
    /// <remarks>
    /// 根据ID获取图片的详细信息
    /// </remarks>
    /// <example>
    /// GET /api/admin/product/image/xxx-xxx-xxx
    /// </example>
    [HttpGet("{id}")]
    public async Task<ApiResponse<ImageDto>> GetImageById(string id)
    {
        var result = await _imageService.GetImageByIdAsync(id);
        return Success(result!);
    }

    /// <summary>
    /// 创建图片
    /// </summary>
    /// <param name="dto">创建参数</param>
    /// <returns>新图片ID</returns>
    /// <remarks>
    /// 创建新的商品图片
    /// 必填字段：SpuId、ImageUrl
    /// 如果设置为主图，会自动清除该商品的其他主图标记
    /// </remarks>
    /// <example>
    /// POST /api/admin/product/image
    /// {
    ///     "spuId": "xxx-xxx-xxx",
    ///     "imageUrl": "/uploads/images/xxx.jpg",
    ///     "isMain": true
    /// }
    /// </example>
    [HttpPost]
    public async Task<ApiResponse<string>> CreateImage([FromBody] CreateImageDto dto)
    {
        var result = await _imageService.CreateImageAsync(dto);
        return Success(result, "图片创建成功");
    }

    /// <summary>
    /// 批量创建图片
    /// </summary>
    /// <param name="spuId">商品ID</param>
    /// <param name="images">图片列表</param>
    /// <returns>成功创建的数量</returns>
    /// <remarks>
    /// 批量创建商品图片
    /// 自动设置排序值（按列表顺序）
    /// </remarks>
    /// <example>
    /// POST /api/admin/product/image/batch/xxx-xxx-xxx
    /// [
    ///     { "imageUrl": "/uploads/images/1.jpg" },
    ///     { "imageUrl": "/uploads/images/2.jpg", "isMain": true }
    /// ]
    /// </example>
    [HttpPost("batch/{spuId}")]
    public async Task<ApiResponse<int>> BatchCreateImages(string spuId, [FromBody] List<CreateImageDto> images)
    {
        var result = await _imageService.BatchCreateImagesAsync(spuId, images);
        return Success(result, $"成功创建 {result} 张图片");
    }

    /// <summary>
    /// 更新图片
    /// </summary>
    /// <param name="id">图片ID</param>
    /// <param name="dto">更新参数</param>
    /// <returns>是否成功</returns>
    /// <remarks>
    /// 更新图片信息
    /// 如果设置为主图，会自动清除该商品的其他主图标记
    /// </remarks>
    /// <example>
    /// PUT /api/admin/product/image/xxx-xxx-xxx
    /// {
    ///     "imageUrl": "/uploads/images/new.jpg",
    ///     "isMain": true
    /// }
    /// </example>
    [HttpPut("{id}")]
    public async Task<ApiResponse<bool>> UpdateImage(string id, [FromBody] UpdateImageDto dto)
    {
        dto.Id = id;
        var result = await _imageService.UpdateImageAsync(dto);
        return Success(result, "图片更新成功");
    }

    /// <summary>
    /// 删除图片
    /// </summary>
    /// <param name="id">图片ID</param>
    /// <returns>是否成功</returns>
    /// <remarks>
    /// 删除图片（软删除）
    /// </remarks>
    /// <example>
    /// DELETE /api/admin/product/image/xxx-xxx-xxx
    /// </example>
    [HttpDelete("{id}")]
    public async Task<ApiResponse<bool>> DeleteImage(string id)
    {
        var result = await _imageService.DeleteImageAsync(id);
        return Success(result, "图片删除成功");
    }

    /// <summary>
    /// 批量删除图片
    /// </summary>
    /// <param name="ids">图片ID列表</param>
    /// <returns>成功删除的数量</returns>
    /// <remarks>
    /// 批量删除图片（软删除）
    /// </remarks>
    /// <example>
    /// DELETE /api/admin/product/image/batch
    /// ["xxx-xxx-xxx", "yyy-yyy-yyy"]
    /// </example>
    [HttpDelete("batch")]
    public async Task<ApiResponse<int>> BatchDeleteImages([FromBody] List<string> ids)
    {
        var result = await _imageService.BatchDeleteImagesAsync(ids);
        return Success(result, $"成功删除 {result} 张图片");
    }

    /// <summary>
    /// 设置主图
    /// </summary>
    /// <param name="id">图片ID</param>
    /// <returns>是否成功</returns>
    /// <remarks>
    /// 将指定图片设置为主图
    /// 自动清除该商品的其他主图标记
    /// </remarks>
    /// <example>
    /// PUT /api/admin/product/image/xxx-xxx-xxx/main
    /// </example>
    [HttpPut("{id}/main")]
    public async Task<ApiResponse<bool>> SetMainImage(string id)
    {
        var result = await _imageService.SetMainImageAsync(id);
        return Success(result, "主图设置成功");
    }

    /// <summary>
    /// 更新图片排序
    /// </summary>
    /// <param name="id">图片ID</param>
    /// <param name="sort">排序值</param>
    /// <returns>是否成功</returns>
    /// <remarks>
    /// 更新图片的排序值
    /// 数值越小越靠前
    /// </remarks>
    /// <example>
    /// PUT /api/admin/product/image/xxx-xxx-xxx/sort?sort=1
    /// </example>
    [HttpPut("{id}/sort")]
    public async Task<ApiResponse<bool>> UpdateImageSort(string id, [FromQuery] int sort)
    {
        var result = await _imageService.UpdateImageSortAsync(id, sort);
        return Success(result, "图片排序更新成功");
    }
}