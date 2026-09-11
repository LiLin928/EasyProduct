using EasyProduct.Business.Site;
using EasyProduct.Common.Base;
using EasyProduct.Models.Dto.Site.Download;
using EasyProduct.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyProduct.Web.Controllers.Admin.Site;

/// <summary>
/// 下载管理控制器（管理端）
/// </summary>
/// <remarks>
/// 提供下载管理的增删改查功能
/// 管理端接口，需要 Admin JWT 认证
/// 权限标识前缀：site:download:
/// </remarks>
[ApiController]
[Route("api/admin/site/download")]
[Authorize(AuthenticationSchemes = "AdminJwt")]
public class DownloadController : BaseController
{
    private readonly IDownloadService _downloadService;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="downloadService">下载管理服务</param>
    public DownloadController(IDownloadService downloadService)
    {
        _downloadService = downloadService;
    }

    /// <summary>
    /// 获取下载列表（分页）
    /// </summary>
    /// <param name="title">标题（模糊搜索）</param>
    /// <param name="category">分类</param>
    /// <param name="status">状态：Disabled=禁用，Enabled=启用</param>
    /// <param name="fileType">文件类型</param>
    /// <param name="pageIndex">页码，从1开始</param>
    /// <param name="pageSize">每页条数，默认10</param>
    /// <returns>下载列表</returns>
    /// <remarks>
    /// 获取下载的分页列表，支持按标题、分类、状态、文件类型筛选
    /// </remarks>
    [HttpGet("list")]
    public async Task<ApiResponse<List<DownloadDto>>> GetList(
        [FromQuery] string? title,
        [FromQuery] string? category,
        [FromQuery] Status? status,
        [FromQuery] string? fileType,
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new DownloadQueryDto
        {
            Title = title,
            Category = category,
            Status = status,
            FileType = fileType,
            PageIndex = pageIndex,
            PageSize = pageSize
        };

        var result = await _downloadService.GetListAsync(query);
        return Success(result);
    }

    /// <summary>
    /// 获取下载详情
    /// </summary>
    /// <param name="id">下载ID</param>
    /// <returns>下载详情</returns>
    /// <remarks>
    /// 根据ID获取下载的详细信息
    /// </remarks>
    [HttpGet("{id}")]
    public async Task<ApiResponse<DownloadDto>> GetById(string id)
    {
        var result = await _downloadService.GetByIdAsync(id);
        return Success(result!);
    }

    /// <summary>
    /// 创建下载
    /// </summary>
    /// <param name="dto">创建参数</param>
    /// <returns>新下载ID</returns>
    /// <remarks>
    /// 创建新的下载记录
    /// </remarks>
    [HttpPost]
    public async Task<ApiResponse<string>> Create([FromBody] CreateDownloadDto dto)
    {
        var result = await _downloadService.CreateAsync(dto);
        return Success(result, "下载创建成功");
    }

    /// <summary>
    /// 更新下载
    /// </summary>
    /// <param name="id">下载ID</param>
    /// <param name="dto">更新参数</param>
    /// <returns>是否成功</returns>
    /// <remarks>
    /// 更新下载信息
    /// </remarks>
    [HttpPut("{id}")]
    public async Task<ApiResponse<bool>> Update(string id, [FromBody] UpdateDownloadDto dto)
    {
        dto.Id = id;
        var result = await _downloadService.UpdateAsync(dto);
        return Success(result, "下载更新成功");
    }

    /// <summary>
    /// 删除下载
    /// </summary>
    /// <param name="id">下载ID</param>
    /// <returns>是否成功</returns>
    /// <remarks>
    /// 删除下载（软删除）
    /// </remarks>
    [HttpDelete("{id}")]
    public async Task<ApiResponse<bool>> Delete(string id)
    {
        var result = await _downloadService.DeleteAsync(id);
        return Success(result, "下载删除成功");
    }

    /// <summary>
    /// 批量删除下载
    /// </summary>
    /// <param name="ids">下载ID列表</param>
    /// <returns>是否成功</returns>
    /// <remarks>
    /// 批量删除下载（软删除）
    /// </remarks>
    [HttpPost("batch-delete")]
    public async Task<ApiResponse<bool>> BatchDelete([FromBody] List<string> ids)
    {
        var successCount = 0;
        foreach (var id in ids)
        {
            var result = await _downloadService.DeleteAsync(id);
            if (result)
            {
                successCount++;
            }
        }

        return Success(true, $"成功删除 {successCount} 个下载");
    }

    /// <summary>
    /// 更新下载状态
    /// </summary>
    /// <param name="id">下载ID</param>
    /// <param name="status">状态：0=禁用，1=启用</param>
    /// <returns>是否成功</returns>
    /// <remarks>
    /// 更新下载的状态（启用/禁用）
    /// </remarks>
    [HttpPut("{id}/status")]
    public async Task<ApiResponse<bool>> UpdateStatus(string id, [FromQuery] int status)
    {
        var result = await _downloadService.UpdateStatusAsync(id, status);
        return Success(result, status == 1 ? "下载已启用" : "下载已禁用");
    }
}