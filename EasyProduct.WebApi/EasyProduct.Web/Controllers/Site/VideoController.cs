using EasyProduct.Business.Site;
using EasyProduct.Common.Base;
using EasyProduct.Models.Dto.Site.Video;
using EasyProduct.Web.Controllers.Site.Base;
using Microsoft.AspNetCore.Mvc;

namespace EasyProduct.Web.Controllers.Site;

/// <summary>
/// 视频管理控制器（官网公开）
/// </summary>
/// <remarks>
/// 提供视频管理的公开查询功能
/// 官网接口，允许匿名访问
/// 只返回启用状态的视频内容
/// 访问视频详情时会自动增加播放次数
/// </remarks>
public class VideoController : SiteControllerBase
{
    private readonly ISiteVideoService _siteVideoService;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="siteVideoService">官网视频管理服务</param>
    public VideoController(ISiteVideoService siteVideoService)
    {
        _siteVideoService = siteVideoService;
    }

    /// <summary>
    /// 获取视频列表
    /// </summary>
    /// <param name="category">分类，可选筛选</param>
    /// <returns>视频列表</returns>
    /// <remarks>
    /// 获取启用状态的视频列表，可按分类筛选
    /// </remarks>
    [HttpGet("list/{category?}")]
    public async Task<ApiResponse<List<VideoDto>>> GetList(string? category = null)
    {
        var result = await _siteVideoService.GetListAsync(category);
        return Success(result);
    }

    /// <summary>
    /// 获取视频详情（并增加播放次数）
    /// </summary>
    /// <param name="id">视频ID</param>
    /// <returns>视频详情</returns>
    /// <remarks>
    /// 获取启用状态的视频详情，并自动增加播放次数
    /// </remarks>
    [HttpGet("{id}")]
    public async Task<ApiResponse<VideoDto>> GetVideo(string id)
    {
        var result = await _siteVideoService.GetVideoAsync(id);
        return Success(result);
    }
}