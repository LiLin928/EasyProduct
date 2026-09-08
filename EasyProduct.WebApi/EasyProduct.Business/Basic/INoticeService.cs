using EasyProduct.Common.Base;
using EasyProduct.Models.Dto.Basic.Notice;

namespace EasyProduct.Business.Basic;

/// <summary>
/// 公告服务接口
/// </summary>
/// <remarks>
/// 提供公告的增删改查、发布/取消发布等功能
/// </remarks>
public interface INoticeService
{
    /// <summary>
    /// 获取公告列表（分页）
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>公告分页列表</returns>
    Task<PageResponse<NoticeDto>> GetNoticeListAsync(NoticeQueryDto query);

    /// <summary>
    /// 获取公告详情
    /// </summary>
    /// <param name="id">公告ID</param>
    /// <returns>公告详情</returns>
    Task<NoticeDto> GetNoticeByIdAsync(string id);

    /// <summary>
    /// 创建公告
    /// </summary>
    /// <param name="dto">创建参数</param>
    /// <returns>新公告ID</returns>
    Task<string> CreateNoticeAsync(CreateNoticeDto dto);

    /// <summary>
    /// 更新公告
    /// </summary>
    /// <param name="dto">更新参数</param>
    /// <returns>是否成功</returns>
    Task<bool> UpdateNoticeAsync(UpdateNoticeDto dto);

    /// <summary>
    /// 删除公告
    /// </summary>
    /// <param name="id">公告ID</param>
    /// <returns>是否成功</returns>
    Task<bool> DeleteNoticeAsync(string id);

    /// <summary>
    /// 发布公告
    /// </summary>
    /// <param name="id">公告ID</param>
    /// <returns>是否成功</returns>
    Task<bool> PublishNoticeAsync(string id);

    /// <summary>
    /// 取消发布公告
    /// </summary>
    /// <param name="id">公告ID</param>
    /// <returns>是否成功</returns>
    Task<bool> UnpublishNoticeAsync(string id);
}