using EasyProduct.Business.Site;
using EasyProduct.Common.Base;
using EasyProduct.Models.Dto.Site.Banner;
using EasyProduct.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyProduct.Web.Controllers.Admin.Site;

/// <summary>
/// Banner管理控制器（管理端）
/// </summary>
/// <remarks>
/// 提供Banner的增删改查功能
/// 管理端接口，需要 Admin JWT 认证
/// 权限标识前缀：site:banner:
/// </remarks>
[ApiController]
[Route("api/admin/site/banner")]
[Authorize(AuthenticationSchemes = "AdminJwt")]
public class BannerController : BaseController
{
    private readonly IBannerService _bannerService;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="bannerService">Banner服务</param>
    public BannerController(IBannerService bannerService)
    {
        _bannerService = bannerService;
    }

    /// <summary>
    /// 获取Banner列表（分页）
    /// </summary>
    /// <param name="title">标题（模糊搜索）</param>
    /// <param name="position">显示位置</param>
    /// <param name="status">状态：Disabled=禁用，Enabled=启用</param>
    /// <param name="startTimeBegin">开始时间-起</param>
    /// <param name="startTimeEnd">开始时间-止</param>
    /// <param name="endTimeBegin">结束时间-起</param>
    /// <param name="endTimeEnd">结束时间-止</param>
    /// <param name="pageIndex">页码，从1开始</param>
    /// <param name="pageSize">每页条数，默认10</param>
    /// <returns>Banner列表</returns>
    /// <remarks>
    /// 获取Banner的分页列表，支持按标题、位置、状态、时间范围筛选
    /// </remarks>
    [HttpGet("list")]
    public async Task<ApiResponse<List<BannerDto>>> GetList(
        [FromQuery] string? title,
        [FromQuery] string? position,
        [FromQuery] Status? status,
        [FromQuery] DateTime? startTimeBegin,
        [FromQuery] DateTime? startTimeEnd,
        [FromQuery] DateTime? endTimeBegin,
        [FromQuery] DateTime? endTimeEnd,
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new BannerQueryDto
        {
            Title = title,
            Position = position,
            Status = status,
            StartTimeBegin = startTimeBegin,
            StartTimeEnd = startTimeEnd,
            EndTimeBegin = endTimeBegin,
            EndTimeEnd = endTimeEnd,
            PageIndex = pageIndex,
            PageSize = pageSize
        };

        var result = await _bannerService.GetListAsync(query);
        return Success(result);
    }

    /// <summary>
    /// 获取Banner详情
    /// </summary>
    /// <param name="id">BannerID</param>
    /// <returns>Banner详情</returns>
    /// <remarks>
    /// 根据ID获取Banner的详细信息
    /// </remarks>
    [HttpGet("{id}")]
    public async Task<ApiResponse<BannerDto>> GetById(string id)
    {
        var result = await _bannerService.GetByIdAsync(id);
        return Success(result!);
    }

    /// <summary>
    /// 创建Banner
    /// </summary>
    /// <param name="dto">创建参数</param>
    /// <returns>新BannerID</returns>
    /// <remarks>
    /// 创建新的Banner
    /// </remarks>
    [HttpPost]
    public async Task<ApiResponse<string>> Create([FromBody] CreateBannerDto dto)
    {
        var result = await _bannerService.CreateAsync(dto);
        return Success(result, "Banner创建成功");
    }

    /// <summary>
    /// 更新Banner
    /// </summary>
    /// <param name="id">BannerID</param>
    /// <param name="dto">更新参数</param>
    /// <returns>是否成功</returns>
    /// <remarks>
    /// 更新Banner信息
    /// </remarks>
    [HttpPut("{id}")]
    public async Task<ApiResponse<bool>> Update(string id, [FromBody] UpdateBannerDto dto)
    {
        dto.Id = id;
        var result = await _bannerService.UpdateAsync(dto);
        return Success(result, "Banner更新成功");
    }

    /// <summary>
    /// 删除Banner
    /// </summary>
    /// <param name="id">BannerID</param>
    /// <returns>是否成功</returns>
    /// <remarks>
    /// 删除Banner（软删除）
    /// </remarks>
    [HttpDelete("{id}")]
    public async Task<ApiResponse<bool>> Delete(string id)
    {
        var result = await _bannerService.DeleteAsync(id);
        return Success(result, "Banner删除成功");
    }

    /// <summary>
    /// 批量删除Banner
    /// </summary>
    /// <param name="ids">BannerID列表</param>
    /// <returns>是否成功</returns>
    /// <remarks>
    /// 批量删除Banner（软删除）
    /// </remarks>
    [HttpPost("batch-delete")]
    public async Task<ApiResponse<bool>> BatchDelete([FromBody] List<string> ids)
    {
        var successCount = 0;
        foreach (var id in ids)
        {
            var result = await _bannerService.DeleteAsync(id);
            if (result)
            {
                successCount++;
            }
        }

        return Success(true, $"成功删除 {successCount} 个Banner");
    }

    /// <summary>
    /// 更新Banner状态
    /// </summary>
    /// <param name="id">BannerID</param>
    /// <param name="status">状态：0=禁用，1=启用</param>
    /// <returns>是否成功</returns>
    /// <remarks>
    /// 更新Banner的状态（启用/禁用）
    /// </remarks>
    [HttpPut("{id}/status")]
    public async Task<ApiResponse<bool>> UpdateStatus(string id, [FromQuery] int status)
    {
        var result = await _bannerService.UpdateStatusAsync(id, status);
        return Success(result, status == 1 ? "Banner已启用" : "Banner已禁用");
    }
}