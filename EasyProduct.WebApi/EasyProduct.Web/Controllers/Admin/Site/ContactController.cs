using EasyProduct.Business.Site;
using EasyProduct.Common.Base;
using EasyProduct.Models.Dto.Site.Contact;
using EasyProduct.Web.Controllers.Admin.Base;
using Microsoft.AspNetCore.Mvc;

namespace EasyProduct.Web.Controllers.Admin.Site;

/// <summary>
/// 留言管理控制器（管理端）
/// </summary>
/// <remarks>
/// 提供留言的查询、标记已读、回复、删除等功能
/// 管理端接口，需要 Admin JWT 认证
/// </remarks>
public class ContactController : AdminControllerBase
{
    private readonly IContactService _contactService;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="contactService">留言服务</param>
    public ContactController(IContactService contactService)
    {
        _contactService = contactService;
    }

    /// <summary>
    /// 获取留言分页列表
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>留言分页结果</returns>
    /// <remarks>
    /// 获取留言的分页列表，支持按姓名、电话、邮箱、公司名称、主题、状态、时间范围筛选
    /// </remarks>
    [HttpGet("list")]
    public async Task<ApiResponse<PageResponse<ContactDto>>> GetContactList([FromQuery] ContactQueryDto query)
    {
        var result = await _contactService.GetContactListAsync(query);
        return Success(result);
    }

    /// <summary>
    /// 获取留言详情
    /// </summary>
    /// <param name="id">留言ID</param>
    /// <returns>留言详情</returns>
    /// <remarks>
    /// 根据ID获取留言的详细信息
    /// </remarks>
    [HttpGet("{id}")]
    public async Task<ApiResponse<ContactDto>> GetContactById(string id)
    {
        var result = await _contactService.GetContactByIdAsync(id);
        return Success(result!);
    }

    /// <summary>
    /// 标记留言为已读
    /// </summary>
    /// <param name="id">留言ID</param>
    /// <returns>是否成功</returns>
    /// <remarks>
    /// 将留言状态标记为"已读"
    /// 只有"未读"状态的留言可以标记为已读
    /// </remarks>
    [HttpPost("{id}/read")]
    public async Task<ApiResponse<bool>> MarkAsRead(string id)
    {
        var result = await _contactService.MarkAsReadAsync(id);
        return Success(result, "标记成功");
    }

    /// <summary>
    /// 回复留言
    /// </summary>
    /// <param name="dto">回复参数</param>
    /// <returns>是否成功</returns>
    /// <remarks>
    /// 回复留言内容
    /// 只有"未读"或"已读"状态的留言可以回复
    /// 回复后状态自动变为"已回复"
    /// </remarks>
    [HttpPost("reply")]
    public async Task<ApiResponse<bool>> ReplyContact([FromBody] ReplyContactDto dto)
    {
        var userId = GetCurrentUserId().ToString();
        var result = await _contactService.ReplyContactAsync(dto, userId);
        return Success(result, "回复成功");
    }

    /// <summary>
    /// 删除留言
    /// </summary>
    /// <param name="id">留言ID</param>
    /// <returns>是否成功</returns>
    /// <remarks>
    /// 删除留言（软删除）
    /// </remarks>
    [HttpDelete("{id}")]
    public async Task<ApiResponse<bool>> DeleteContact(string id)
    {
        var result = await _contactService.DeleteContactAsync(id);
        return Success(result, "删除成功");
    }

    /// <summary>
    /// 获取未读留言数量
    /// </summary>
    /// <returns>未读数量</returns>
    /// <remarks>
    /// 获取未读状态的留言总数，用于显示未读消息提醒
    /// </remarks>
    [HttpGet("unread-count")]
    public async Task<ApiResponse<int>> GetUnreadCount()
    {
        var result = await _contactService.GetUnreadCountAsync();
        return Success(result);
    }
}