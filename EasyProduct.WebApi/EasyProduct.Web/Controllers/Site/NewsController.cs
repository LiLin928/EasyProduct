using EasyProduct.Business.Site;
using EasyProduct.Common.Base;
using EasyProduct.Models.Dto.Site.News;
using EasyProduct.Web.Controllers.Site.Base;
using Microsoft.AspNetCore.Mvc;

namespace EasyProduct.Web.Controllers.Site;

/// <summary>
/// 新闻控制器（官网公开）
/// </summary>
/// <remarks>
/// 提供新闻的公开查询功能
/// 官网接口，允许匿名访问
/// 只返回启用状态的新闻
/// </remarks>
public class NewsController : SiteControllerBase
{
    private readonly ISiteNewsService _siteNewsService;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="siteNewsService">官网新闻服务</param>
    public NewsController(ISiteNewsService siteNewsService)
    {
        _siteNewsService = siteNewsService;
    }

    /// <summary>
    /// 获取新闻列表（分页）
    /// </summary>
    /// <param name="categoryId">分类ID（可选）</param>
    /// <param name="pageIndex">页码，从1开始</param>
    /// <param name="pageSize">每页条数，默认10</param>
    /// <returns>新闻列表（不包含内容字段）</returns>
    /// <remarks>
    /// 获取新闻的分页列表，只返回启用状态的新闻
    /// 不包含新闻内容字段，用于列表页展示
    /// </remarks>
    [HttpGet("list")]
    public async Task<ApiResponse<List<NewsListDto>>> GetList(
        [FromQuery] string? categoryId,
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new NewsQueryDto
        {
            CategoryId = categoryId,
            PageIndex = pageIndex,
            PageSize = pageSize
        };

        var result = await _siteNewsService.GetListAsync(query);
        return Success(result);
    }

    /// <summary>
    /// 获取新闻详情
    /// </summary>
    /// <param name="id">新闻ID</param>
    /// <returns>新闻详情</returns>
    /// <remarks>
    /// 根据ID获取新闻的详细信息，包含新闻内容
    /// 只返回启用状态的新闻
    /// 自动增加浏览次数
    /// </remarks>
    [HttpGet("{id}")]
    public async Task<ApiResponse<NewsDto>> GetById(string id)
    {
        var result = await _siteNewsService.GetByIdAsync(id);
        return Success(result!);
    }

    /// <summary>
    /// 获取置顶新闻列表
    /// </summary>
    /// <param name="count">返回数量，默认5条</param>
    /// <returns>置顶新闻列表</returns>
    /// <remarks>
    /// 获取置顶新闻列表，只返回启用状态的置顶新闻
    /// 用于首页展示
    /// </remarks>
    [HttpGet("recommended")]
    public async Task<ApiResponse<List<NewsListDto>>> GetRecommended([FromQuery] int count = 5)
    {
        var result = await _siteNewsService.GetTopListAsync(count);
        return Success(result);
    }
}