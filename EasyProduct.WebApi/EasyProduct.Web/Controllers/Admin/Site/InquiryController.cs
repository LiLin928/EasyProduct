using EasyProduct.Business.Site;
using EasyProduct.Common.Base;
using EasyProduct.Models.Dto.Site.Inquiry;
using EasyProduct.Web.Controllers.Admin.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyProduct.Web.Controllers.Admin.Site;

/// <summary>
/// 询价单管理控制器（管理端）
/// </summary>
/// <remarks>
/// 提供询价单的查询、跟进、转客户、关闭、删除等功能
/// 管理端接口，需要 Admin JWT 认证
/// </remarks>
public class InquiryController : AdminControllerBase
{
    private readonly IInquiryService _inquiryService;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="inquiryService">询价单服务</param>
    public InquiryController(IInquiryService inquiryService)
    {
        _inquiryService = inquiryService;
    }

    /// <summary>
    /// 获取询价单分页列表
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>询价单分页结果</returns>
    /// <remarks>
    /// 获取询价单的分页列表，支持按询价单号、公司名称、联系人、电话、邮箱、状态、时间范围筛选
    /// </remarks>
    [HttpGet("list")]
    public async Task<ApiResponse<PageResponse<InquiryDto>>> GetInquiryList([FromQuery] InquiryQueryDto query)
    {
        var result = await _inquiryService.GetInquiryListAsync(query);
        return Success(result);
    }

    /// <summary>
    /// 获取询价单详情
    /// </summary>
    /// <param name="id">询价单ID</param>
    /// <returns>询价单详情</returns>
    /// <remarks>
    /// 根据ID获取询价单的详细信息，包含询价明细列表
    /// </remarks>
    [HttpGet("{id}")]
    public async Task<ApiResponse<InquiryDto>> GetInquiryById(string id)
    {
        var result = await _inquiryService.GetInquiryByIdAsync(id);
        return Success(result!);
    }

    /// <summary>
    /// 跟进询价单
    /// </summary>
    /// <param name="id">询价单ID</param>
    /// <returns>是否成功</returns>
    /// <remarks>
    /// 将询价单状态标记为"已跟进"
    /// 只有"待处理"状态的询价单可以跟进
    /// </remarks>
    [HttpPost("{id}/follow")]
    public async Task<ApiResponse<bool>> FollowInquiry(string id)
    {
        var userId = GetCurrentUserId().ToString();
        var result = await _inquiryService.FollowInquiryAsync(id, userId);
        return Success(result, "跟进成功");
    }

    /// <summary>
    /// 询价单转客户
    /// </summary>
    /// <param name="id">询价单ID</param>
    /// <returns>新客户ID</returns>
    /// <remarks>
    /// 将询价单转换为客户档案
    /// 只有"已跟进"状态的询价单可以转客户
    /// 会自动创建客户档案并关联
    /// </remarks>
    [HttpPost("{id}/convert")]
    public async Task<ApiResponse<string>> ConvertToCustomer(string id)
    {
        var userId = GetCurrentUserId().ToString();
        var result = await _inquiryService.ConvertToCustomerAsync(id, userId);
        return Success(result, "转客户成功");
    }

    /// <summary>
    /// 关闭询价单
    /// </summary>
    /// <param name="id">询价单ID</param>
    /// <param name="reason">关闭原因</param>
    /// <returns>是否成功</returns>
    /// <remarks>
    /// 关闭询价单，需要提供关闭原因
    /// 只有"待处理"或"已跟进"状态的询价单可以关闭
    /// </remarks>
    [HttpPost("{id}/close")]
    public async Task<ApiResponse<bool>> CloseInquiry(string id, [FromBody] string reason)
    {
        var userId = GetCurrentUserId().ToString();
        var result = await _inquiryService.CloseInquiryAsync(id, reason, userId);
        return Success(result, "关闭成功");
    }

    /// <summary>
    /// 删除询价单
    /// </summary>
    /// <param name="id">询价单ID</param>
    /// <returns>是否成功</returns>
    /// <remarks>
    /// 删除询价单（软删除），同时删除关联的询价明细
    /// </remarks>
    [HttpDelete("{id}")]
    public async Task<ApiResponse<bool>> DeleteInquiry(string id)
    {
        var result = await _inquiryService.DeleteInquiryAsync(id);
        return Success(result, "删除成功");
    }
}