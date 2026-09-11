using EasyProduct.Business.Site;
using EasyProduct.Common.Base;
using EasyProduct.Models.Dto.Site.Video;
using EasyProduct.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyProduct.Web.Controllers.Admin.Site;

/// <summary>
/// 视频管理控制器（管理端）
/// </summary>
/// <remarks>
/// 提供视频管理的增删改查功能
/// 管理端接口，需要 Admin JWT 认证
/// 权限标识前缀：site:video:
/// </remarks>
[ApiController]
[Route("api/admin/site/video")]
[Authorize(AuthenticationSchemes = "AdminJwt")]
public class VideoController : BaseController
{
    private readonly IVideoService _videoService;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="videoService">视频管理服务</param>
    public VideoController(IVideoService videoService)
    {
        _videoService = videoService;
    }

    /// <summary>
    /// 获取视频列表（分页）
    /// </summary>
    /// <param name="title">标题（模糊搜索）</param>
    /// <param name="category">分类</param>
    /// <param name="status">状态：Disabled=禁用，Enabled=启用</param>
    /// <param name="videoType">视频类型</param>
    /// <param name="pageIndex">页码，从1开始</param>
    /// <param name="pageSize">每页条数，默认10</param>
    /// <returns>视频列表</returns>
    /// <remarks>
    /// 获取视频的分页列表，支持按标题、分类、状态、视频类型筛选
    /// </remarks>
    [HttpGet("list")]
    public async Task<ApiResponse<List<VideoDto>>> GetList(
        [FromQuery] string? title,
        [FromQuery] string? category,
        [FromQuery] Status? status,
        [FromQuery] string? videoType,
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new VideoQueryDto
        {
            Title = title,
            Category = category,
            Status = status,
            VideoType = videoType,
            PageIndex = pageIndex,
            PageSize = pageSize
        };

        var result = await _videoService.GetListAsync(query);
        return Success(result);
    }

    /// <summary>
    /// 获取视频详情
    /// </summary>
    /// <param name="id">视频ID</param>
    /// <returns>视频详情</returns>
    /// <remarks>
    /// 根据ID获取视频的详细信息
    /// </remarks>
    [HttpGet("{id}")]
    public async Task<ApiResponse<VideoDto>> GetById(string id)
    {
        var result = await _videoService.GetByIdAsync(id);
        return Success(result!);
    }

    /// <summary>
    /// 创建视频
    /// </summary>
    /// <param name="dto">创建参数</param>
    /// <returns>新视频ID</returns>
    /// <remarks>
    /// 创建新的视频记录
    /// </remarks>
    [HttpPost]
    public async Task<ApiResponse<string>> Create([FromBody] CreateVideoDto dto)
    {
        var result = await _videoService.CreateAsync(dto);
        return Success(result, "视频创建成功");
    }

    /// <summary>
    /// 更新视频
    /// </summary>
    /// <param name="id">视频ID</param>
    /// <param name="dto">更新参数</param>
    /// <returns>是否成功</returns>
    /// <remarks>
    /// 更新视频信息
    /// </remarks>
    [HttpPut("{id}")]
    public async Task<ApiResponse<bool>> Update(string id, [FromBody] UpdateVideoDto dto)
    {
        dto.Id = id;
        var result = await _videoService.UpdateAsync(dto);
        return Success(result, "视频更新成功");
    }

    /// <summary>
    /// 删除视频
    /// </summary>
    /// <param name="id">视频ID</param>
    /// <returns>是否成功</returns>
    /// <remarks>
    /// 删除视频（软删除）
    /// </remarks>
    [HttpDelete("{id}")]
    public async Task<ApiResponse<bool>> Delete(string id)
    {
        var result = await _videoService.DeleteAsync(id);
        return Success(result, "视频删除成功");
    }

    /// <summary>
    /// 批量删除视频
    /// </summary>
    /// <param name="ids">视频ID列表</param>
    /// <returns>是否成功</returns>
    /// <remarks>
    /// 批量删除视频（软删除）
    /// </remarks>
    [HttpPost("batch-delete")]
    public async Task<ApiResponse<bool>> BatchDelete([FromBody] List<string> ids)
    {
        var successCount = 0;
        foreach (var id in ids)
        {
            var result = await _videoService.DeleteAsync(id);
            if (result)
            {
                successCount++;
            }
        }

        return Success(true, $"成功删除 {successCount} 个视频");
    }

    /// <summary>
    /// 更新视频状态
    /// </summary>
    /// <param name="id">视频ID</param>
    /// <param name="status">状态：0=禁用，1=启用</param>
    /// <returns>是否成功</returns>
    /// <remarks>
    /// 更新视频的状态（启用/禁用）
    /// </remarks>
    [HttpPut("{id}/status")]
    public async Task<ApiResponse<bool>> UpdateStatus(string id, [FromQuery] int status)
    {
        var result = await _videoService.UpdateStatusAsync(id, status);
        return Success(result, status == 1 ? "视频已启用" : "视频已禁用");
    }
}