using EasyProduct.Business.Site;
using EasyProduct.Common.Base;
using EasyProduct.Models.Dto.Site.News;
using EasyProduct.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyProduct.Web.Controllers.Admin.Site;

/// <summary>
/// 新闻管理控制器（管理端）
/// </summary>
/// <remarks>
/// 提供新闻的增删改查功能
/// 管理端接口，需要 Admin JWT 认证
/// 权限标识前缀：site:news:
/// </remarks>
[ApiController]
[Route("api/admin/site/news")]
[Authorize(AuthenticationSchemes = "AdminJwt")]
public class NewsController : BaseController
{
    private readonly INewsService _newsService;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="newsService">新闻服务</param>
    public NewsController(INewsService newsService)
    {
        _newsService = newsService;
    }

    /// <summary>
    /// 获取新闻列表（分页）
    /// </summary>
    /// <param name="keyword">关键词（模糊搜索标题）</param>
    /// <param name="categoryId">分类ID</param>
    /// <param name="status">状态：Disabled=禁用，Enabled=启用</param>
    /// <param name="isTop">是否置顶：0=不置顶，1=置顶</param>
    /// <param name="publishTimeStart">发布时间开始</param>
    /// <param name="publishTimeEnd">发布时间结束</param>
    /// <param name="pageIndex">页码，从1开始</param>
    /// <param name="pageSize">每页条数，默认10</param>
    /// <returns>新闻列表</returns>
    /// <remarks>
    /// 获取新闻的分页列表，支持按关键词、分类、状态、置顶、发布时间筛选
    /// </remarks>
    [HttpGet("list")]
    public async Task<ApiResponse<List<NewsDto>>> GetList(
        [FromQuery] string? keyword,
        [FromQuery] string? categoryId,
        [FromQuery] Status? status,
        [FromQuery] int? isTop,
        [FromQuery] DateTime? publishTimeStart,
        [FromQuery] DateTime? publishTimeEnd,
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new NewsQueryDto
        {
            Keyword = keyword,
            CategoryId = categoryId,
            Status = status,
            IsTop = isTop,
            PublishTimeStart = publishTimeStart,
            PublishTimeEnd = publishTimeEnd,
            PageIndex = pageIndex,
            PageSize = pageSize
        };

        var result = await _newsService.GetListAsync(query);
        return Success(result);
    }

    /// <summary>
    /// 获取新闻详情
    /// </summary>
    /// <param name="id">新闻ID</param>
    /// <returns>新闻详情</returns>
    /// <remarks>
    /// 根据ID获取新闻的详细信息，包含新闻内容
    /// </remarks>
    [HttpGet("{id}")]
    public async Task<ApiResponse<NewsDto>> GetById(string id)
    {
        var result = await _newsService.GetByIdAsync(id);
        return Success(result!);
    }

    /// <summary>
    /// 创建新闻
    /// </summary>
    /// <param name="dto">创建参数</param>
    /// <returns>新新闻ID</returns>
    /// <remarks>
    /// 创建新的新闻
    /// </remarks>
    [HttpPost]
    public async Task<ApiResponse<string>> Create([FromBody] CreateNewsDto dto)
    {
        var result = await _newsService.CreateAsync(dto);
        return Success(result, "新闻创建成功");
    }

    /// <summary>
    /// 更新新闻
    /// </summary>
    /// <param name="id">新闻ID</param>
    /// <param name="dto">更新参数</param>
    /// <returns>是否成功</returns>
    /// <remarks>
    /// 更新新闻信息
    /// </remarks>
    [HttpPut("{id}")]
    public async Task<ApiResponse<bool>> Update(string id, [FromBody] UpdateNewsDto dto)
    {
        dto.Id = id;
        var result = await _newsService.UpdateAsync(dto);
        return Success(result, "新闻更新成功");
    }

    /// <summary>
    /// 删除新闻
    /// </summary>
    /// <param name="id">新闻ID</param>
    /// <returns>是否成功</returns>
    /// <remarks>
    /// 删除新闻（软删除）
    /// </remarks>
    [HttpDelete("{id}")]
    public async Task<ApiResponse<bool>> Delete(string id)
    {
        var result = await _newsService.DeleteAsync(id);
        return Success(result, "新闻删除成功");
    }

    /// <summary>
    /// 批量删除新闻
    /// </summary>
    /// <param name="ids">新闻ID列表</param>
    /// <returns>是否成功</returns>
    /// <remarks>
    /// 批量删除新闻（软删除）
    /// </remarks>
    [HttpPost("batch-delete")]
    public async Task<ApiResponse<bool>> BatchDelete([FromBody] List<string> ids)
    {
        var successCount = 0;
        foreach (var id in ids)
        {
            var result = await _newsService.DeleteAsync(id);
            if (result)
            {
                successCount++;
            }
        }

        return Success(true, $"成功删除 {successCount} 条新闻");
    }

    /// <summary>
    /// 更新新闻状态
    /// </summary>
    /// <param name="id">新闻ID</param>
    /// <param name="status">状态：0=禁用，1=启用</param>
    /// <returns>是否成功</returns>
    /// <remarks>
    /// 更新新闻的状态（启用/禁用）
    /// </remarks>
    [HttpPut("{id}/status")]
    public async Task<ApiResponse<bool>> UpdateStatus(string id, [FromQuery] int status)
    {
        var result = await _newsService.UpdateStatusAsync(id, status);
        return Success(result, status == 1 ? "新闻已启用" : "新闻已禁用");
    }

    /// <summary>
    /// 设置新闻置顶
    /// </summary>
    /// <param name="id">新闻ID</param>
    /// <param name="isTop">是否置顶：0=不置顶，1=置顶</param>
    /// <returns>是否成功</returns>
    /// <remarks>
    /// 设置或取消新闻的置顶状态
    /// </remarks>
    [HttpPut("{id}/top")]
    public async Task<ApiResponse<bool>> SetTop(string id, [FromQuery] int isTop)
    {
        var result = await _newsService.SetTopAsync(id, isTop);
        return Success(result, isTop == 1 ? "新闻已置顶" : "新闻已取消置顶");
    }
}