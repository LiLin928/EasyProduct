using EasyProduct.Business.Site;
using EasyProduct.Common.Base;
using EasyProduct.Models.Dto.Site.Download;
using EasyProduct.Web.Controllers.Site.Base;
using Microsoft.AspNetCore.Mvc;

namespace EasyProduct.Web.Controllers.Site;

/// <summary>
/// 下载管理控制器（官网公开）
/// </summary>
/// <remarks>
/// 提供下载管理的公开查询功能
/// 官网接口，允许匿名访问
/// 只返回启用状态的下载内容
/// 访问下载详情时会自动增加下载次数
/// </remarks>
public class DownloadController : SiteControllerBase
{
    private readonly ISiteDownloadService _siteDownloadService;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="siteDownloadService">官网下载管理服务</param>
    public DownloadController(ISiteDownloadService siteDownloadService)
    {
        _siteDownloadService = siteDownloadService;
    }

    /// <summary>
    /// 获取下载列表
    /// </summary>
    /// <param name="category">分类，可选筛选</param>
    /// <returns>下载列表</returns>
    /// <remarks>
    /// 获取启用状态的下载列表，可按分类筛选
    /// </remarks>
    [HttpGet("list/{category?}")]
    public async Task<ApiResponse<List<DownloadDto>>> GetList(string? category = null)
    {
        var result = await _siteDownloadService.GetListAsync(category);
        return Success(result);
    }

    /// <summary>
    /// 获取下载详情（并增加下载次数）
    /// </summary>
    /// <param name="id">下载ID</param>
    /// <returns>下载详情</returns>
    /// <remarks>
    /// 获取启用状态的下载详情，并自动增加下载次数
    /// </remarks>
    [HttpGet("{id}")]
    public async Task<ApiResponse<DownloadDto>> GetDownload(string id)
    {
        var result = await _siteDownloadService.GetDownloadAsync(id);
        return Success(result);
    }
}