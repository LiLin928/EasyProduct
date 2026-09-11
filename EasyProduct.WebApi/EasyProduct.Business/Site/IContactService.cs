using EasyProduct.Common.Base;
using EasyProduct.Models.Dto.Site.Contact;

namespace EasyProduct.Business.Site;

/// <summary>
/// 留言服务接口（管理端）
/// </summary>
/// <remarks>
/// 提供留言的查询、标记已读、回复、删除等功能
/// 管理端使用，需要 Admin JWT 认证
/// </remarks>
public interface IContactService
{
    /// <summary>
    /// 获取留言分页列表
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>留言分页结果</returns>
    Task<PageResponse<ContactDto>> GetContactListAsync(ContactQueryDto query);

    /// <summary>
    /// 获取留言详情
    /// </summary>
    /// <param name="id">留言ID</param>
    /// <returns>留言详情</returns>
    Task<ContactDto> GetContactByIdAsync(string id);

    /// <summary>
    /// 标记留言为已读
    /// </summary>
    /// <param name="id">留言ID</param>
    /// <returns>是否成功</returns>
    Task<bool> MarkAsReadAsync(string id);

    /// <summary>
    /// 回复留言
    /// </summary>
    /// <param name="dto">回复参数</param>
    /// <param name="userId">操作人ID</param>
    /// <returns>是否成功</returns>
    Task<bool> ReplyContactAsync(ReplyContactDto dto, string userId);

    /// <summary>
    /// 删除留言
    /// </summary>
    /// <param name="id">留言ID</param>
    /// <returns>是否成功</returns>
    Task<bool> DeleteContactAsync(string id);

    /// <summary>
    /// 获取未读留言数量
    /// </summary>
    /// <returns>未读数量</returns>
    Task<int> GetUnreadCountAsync();
}