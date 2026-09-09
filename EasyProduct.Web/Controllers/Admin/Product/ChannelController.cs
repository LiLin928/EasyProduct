using EasyProduct.Business.Product;
using EasyProduct.Common.Base;
using EasyProduct.Models.Dto.Product.Channel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyProduct.Web.Controllers.Admin.Product;

/// <summary>
/// 渠道发布管理控制器
/// </summary>
/// <remarks>
/// 提供商品渠道发布的创建、管理、上架状态控制等功能
/// 管理端接口，需要 Admin JWT 认证
/// API 路由前缀：/api/admin/product/channel
/// 支持三端渠道：官网（site）、小程序（miniapp）、B2B（b2b）
/// </remarks>
[ApiController]
[Route("api/admin/product/channel")]
[Authorize(AuthenticationSchemes = "AdminJwt")]
public class ChannelController : BaseController
{
    private readonly IChannelService _channelService;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="channelService">渠道发布服务</param>
    public ChannelController(IChannelService channelService)
    {
        _channelService = channelService;
    }

    /// <summary>
    /// 获取渠道发布列表
    /// </summary>
    /// <param name="spuId">商品ID</param>
    /// <param name="channelCode">渠道编码</param>
    /// <param name="status">上架状态</param>
    /// <returns>渠道发布列表</returns>
    /// <remarks>
    /// 获取渠道发布列表，支持按商品、渠道、状态筛选
    /// 结果按排序字段和创建时间排序
    /// </remarks>
    /// <example>
    /// GET /api/admin/product/channel/list?spuId=xxx-xxx-xxx
    /// GET /api/admin/product/channel/list?channelCode=site
    /// GET /api/admin/product/channel/list?status=1
    /// </example>
    [HttpGet("list")]
    public async Task<ApiResponse<List<ChannelDto>>> GetChannelList(
        [FromQuery] string? spuId,
        [FromQuery] string? channelCode,
        [FromQuery] int? status)
    {
        var query = new ChannelQueryDto
        {
            SpuId = spuId,
            ChannelCode = channelCode,
            Status = status
        };

        var result = await _channelService.GetChannelListAsync(query);
        return Success(result);
    }

    /// <summary>
    /// 获取渠道发布详情
    /// </summary>
    /// <param name="id">渠道发布ID</param>
    /// <returns>渠道发布详情</returns>
    /// <remarks>
    /// 根据ID获取渠道发布的详细信息
    /// </remarks>
    /// <example>
    /// GET /api/admin/product/channel/xxx-xxx-xxx
    /// </example>
    [HttpGet("{id}")]
    public async Task<ApiResponse<ChannelDto>> GetChannelById(string id)
    {
        var result = await _channelService.GetChannelByIdAsync(id);
        return Success(result!);
    }

    /// <summary>
    /// 获取商品的渠道发布情况
    /// </summary>
    /// <param name="spuId">商品ID</param>
    /// <returns>渠道发布列表</returns>
    /// <remarks>
    /// 获取指定商品在所有渠道的发布情况
    /// </remarks>
    /// <example>
    /// GET /api/admin/product/channel/spu/xxx-xxx-xxx
    /// </example>
    [HttpGet("spu/{spuId}")]
    public async Task<ApiResponse<List<ChannelDto>>> GetSpuChannels(string spuId)
    {
        var result = await _channelService.GetSpuChannelsAsync(spuId);
        return Success(result);
    }

    /// <summary>
    /// 创建渠道发布
    /// </summary>
    /// <param name="dto">创建参数</param>
    /// <returns>新渠道发布ID</returns>
    /// <remarks>
    /// 创建新的渠道发布
    /// 必填字段：SpuId、ChannelCode
    /// 验证商品是否存在、渠道编码是否有效
    /// 检查是否已发布到该渠道
    /// </remarks>
    /// <example>
    /// POST /api/admin/product/channel
    /// {
    ///     "spuId": "xxx-xxx-xxx",
    ///     "channelCode": "site",
    ///     "status": 1
    /// }
    /// </example>
    [HttpPost]
    public async Task<ApiResponse<string>> CreateChannel([FromBody] CreateChannelDto dto)
    {
        var result = await _channelService.CreateChannelAsync(dto);
        return Success(result, "渠道发布创建成功");
    }

    /// <summary>
    /// 批量发布到渠道
    /// </summary>
    /// <param name="dto">批量发布参数</param>
    /// <returns>成功发布的数量</returns>
    /// <remarks>
    /// 批量将商品发布到多个渠道
    /// 必填字段：SpuId、ChannelCodes
    /// 自动跳过无效的渠道编码和已发布的渠道
    /// </remarks>
    /// <example>
    /// POST /api/admin/product/channel/batch
    /// {
    ///     "spuId": "xxx-xxx-xxx",
    ///     "channelCodes": ["site", "miniapp", "b2b"],
    ///     "status": 1
    /// }
    /// </example>
    [HttpPost("batch")]
    public async Task<ApiResponse<int>> BatchPublish([FromBody] BatchPublishDto dto)
    {
        var result = await _channelService.BatchPublishAsync(dto);
        return Success(result, $"成功发布到 {result} 个渠道");
    }

    /// <summary>
    /// 更新渠道发布
    /// </summary>
    /// <param name="id">渠道发布ID</param>
    /// <param name="dto">更新参数</param>
    /// <returns>是否成功</returns>
    /// <remarks>
    /// 更新渠道发布信息
    /// 如果从未上架变为上架，自动设置发布时间
    /// 如果从上架变为下架，自动设置下架时间
    /// </remarks>
    /// <example>
    /// PUT /api/admin/product/channel/xxx-xxx-xxx
    /// {
    ///     "status": 0,
    ///     "showPrice": false
    /// }
    /// </example>
    [HttpPut("{id}")]
    public async Task<ApiResponse<bool>> UpdateChannel(string id, [FromBody] UpdateChannelDto dto)
    {
        dto.Id = id;
        var result = await _channelService.UpdateChannelAsync(dto);
        return Success(result, "渠道发布更新成功");
    }

    /// <summary>
    /// 删除渠道发布
    /// </summary>
    /// <param name="id">渠道发布ID</param>
    /// <returns>是否成功</returns>
    /// <remarks>
    /// 删除渠道发布（软删除）
    /// </remarks>
    /// <example>
    /// DELETE /api/admin/product/channel/xxx-xxx-xxx
    /// </example>
    [HttpDelete("{id}")]
    public async Task<ApiResponse<bool>> DeleteChannel(string id)
    {
        var result = await _channelService.DeleteChannelAsync(id);
        return Success(result, "渠道发布删除成功");
    }

    /// <summary>
    /// 更新上架状态
    /// </summary>
    /// <param name="id">渠道发布ID</param>
    /// <param name="status">上架状态：0=下架，1=上架</param>
    /// <returns>是否成功</returns>
    /// <remarks>
    /// 启用或禁用渠道发布
    /// 如果上架，自动设置发布时间
    /// 如果下架，自动设置下架时间
    /// </remarks>
    /// <example>
    /// PUT /api/admin/product/channel/xxx-xxx-xxx/status?status=1
    /// </example>
    [HttpPut("{id}/status")]
    public async Task<ApiResponse<bool>> UpdateChannelStatus(string id, [FromQuery] int status)
    {
        var result = await _channelService.UpdateChannelStatusAsync(id, status);
        return Success(result, "上架状态更新成功");
    }

    /// <summary>
    /// 批量更新上架状态
    /// </summary>
    /// <param name="ids">渠道发布ID列表</param>
    /// <param name="status">上架状态：0=下架，1=上架</param>
    /// <returns>成功更新的数量</returns>
    /// <remarks>
    /// 批量启用或禁用渠道发布
    /// 只更新存在的渠道发布，忽略不存在的ID
    /// </remarks>
    /// <example>
    /// PUT /api/admin/product/channel/batch/status?status=1
    /// ["xxx-xxx-xxx", "yyy-yyy-yyy"]
    /// </example>
    [HttpPut("batch/status")]
    public async Task<ApiResponse<int>> BatchUpdateStatus([FromBody] List<string> ids, [FromQuery] int status)
    {
        var result = await _channelService.BatchUpdateStatusAsync(ids, status);
        return Success(result, $"成功更新 {result} 个渠道发布状态");
    }
}