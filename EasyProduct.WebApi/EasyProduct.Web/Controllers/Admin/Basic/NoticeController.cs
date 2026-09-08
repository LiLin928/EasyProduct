using EasyProduct.Business.Basic;
using EasyProduct.Common.Base;
using EasyProduct.Models.Dto.Basic.Notice;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyProduct.Web.Controllers.Admin.Basic;

/// <summary>
/// 公告管理控制器
/// </summary>
/// <remarks>
/// 提供公告的增删改查、发布/取消发布功能
/// 管理端接口，需要 Admin JWT 认证
/// </remarks>
[ApiController]
[Route("api/admin/basic/notice")]
[Authorize(AuthenticationSchemes = "AdminJwt")]
public class NoticeController : BaseController
{
    private readonly INoticeService _noticeService;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="noticeService">公告服务</param>
    public NoticeController(INoticeService noticeService)
    {
        _noticeService = noticeService;
    }

    /// <summary>
    /// 获取公告列表（分页）
    /// </summary>
    /// <param name="noticeTitle">公告标题（模糊搜索）</param>
    /// <param name="noticeType">公告类型：1=通知，2=公告</param>
    /// <param name="topFlag">是否置顶：0=否，1=是</param>
    /// <param name="status">状态：0=禁用，1=启用</param>
    /// <param name="pageIndex">页码（从1开始）</param>
    /// <param name="pageSize">每页条数</param>
    /// <returns>公告分页列表</returns>
    /// <remarks>
    /// 获取公告的列表，支持分页和按标题、类型、置顶标记、状态筛选
    /// </remarks>
    [HttpGet("list")]
    public async Task<ApiResponse<PageResponse<NoticeDto>>> GetNoticeList(
        [FromQuery] string? noticeTitle,
        [FromQuery] int? noticeType,
        [FromQuery] int? topFlag,
        [FromQuery] int? status,
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new NoticeQueryDto
        {
            NoticeTitle = noticeTitle,
            NoticeType = noticeType,
            TopFlag = topFlag,
            Status = status,
            PageIndex = pageIndex,
            PageSize = pageSize
        };

        var result = await _noticeService.GetNoticeListAsync(query);
        return Success(result);
    }

    /// <summary>
    /// 获取公告详情
    /// </summary>
    /// <param name="id">公告ID</param>
    /// <returns>公告详情</returns>
    /// <remarks>
    /// 根据ID获取公告的详细信息
    /// </remarks>
    [HttpGet("{id}")]
    public async Task<ApiResponse<NoticeDto>> GetNoticeById(string id)
    {
        var result = await _noticeService.GetNoticeByIdAsync(id);
        return Success(result!);
    }

    /// <summary>
    /// 创建公告
    /// </summary>
    /// <param name="dto">创建参数</param>
    /// <returns>新公告ID</returns>
    /// <remarks>
    /// 创建新的公告
    /// </remarks>
    [HttpPost]
    public async Task<ApiResponse<string>> CreateNotice([FromBody] CreateNoticeDto dto)
    {
        var result = await _noticeService.CreateNoticeAsync(dto);
        return Success(result, "公告创建成功");
    }

    /// <summary>
    /// 更新公告
    /// </summary>
    /// <param name="id">公告ID</param>
    /// <param name="dto">更新参数</param>
    /// <returns>是否成功</returns>
    /// <remarks>
    /// 更新公告信息
    /// </remarks>
    [HttpPut("{id}")]
    public async Task<ApiResponse<bool>> UpdateNotice(string id, [FromBody] UpdateNoticeDto dto)
    {
        dto.Id = id;
        var result = await _noticeService.UpdateNoticeAsync(dto);
        return Success(result, "公告更新成功");
    }

    /// <summary>
    /// 删除公告
    /// </summary>
    /// <param name="id">公告ID</param>
    /// <returns>是否成功</returns>
    /// <remarks>
    /// 删除公告（软删除）
    /// </remarks>
    [HttpDelete("{id}")]
    public async Task<ApiResponse<bool>> DeleteNotice(string id)
    {
        var result = await _noticeService.DeleteNoticeAsync(id);
        return Success(result, "公告删除成功");
    }

    /// <summary>
    /// 发布公告
    /// </summary>
    /// <param name="id">公告ID</param>
    /// <returns>是否成功</returns>
    /// <remarks>
    /// 发布公告，设置发布时间
    /// </remarks>
    [HttpPost("{id}/publish")]
    public async Task<ApiResponse<bool>> PublishNotice(string id)
    {
        var result = await _noticeService.PublishNoticeAsync(id);
        return Success(result, "公告发布成功");
    }

    /// <summary>
    /// 取消发布公告
    /// </summary>
    /// <param name="id">公告ID</param>
    /// <returns>是否成功</returns>
    /// <remarks>
    /// 取消发布公告，清除发布时间
    /// </remarks>
    [HttpPost("{id}/unpublish")]
    public async Task<ApiResponse<bool>> UnpublishNotice(string id)
    {
        var result = await _noticeService.UnpublishNoticeAsync(id);
        return Success(result, "取消发布公告成功");
    }
}