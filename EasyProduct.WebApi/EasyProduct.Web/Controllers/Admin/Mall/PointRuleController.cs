using EasyProduct.Common.Base;
using EasyProduct.Models.Dto.Mall.Point;
using EasyProduct.Business.Mall;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyProduct.Web.Controllers.Admin.Mall;

/// <summary>
/// 积分规则管理控制器（管理端）
/// </summary>
/// <remarks>
/// 提供积分规则的创建、查询、更新、删除等管理功能。
/// 需要管理员权限。
/// </remarks>
[ApiController]
[Route("api/admin/mall/point-rule")]
[Authorize(AuthenticationSchemes = "AdminJwt")]
public class PointRuleController : BaseController
{
    private readonly IPointService _pointService;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="pointService">积分服务</param>
    public PointRuleController(IPointService pointService)
    {
        _pointService = pointService;
    }

    /// <summary>
    /// 创建积分规则
    /// </summary>
    /// <param name="dto">创建积分规则参数</param>
    /// <returns>规则ID</returns>
    /// <remarks>
    /// 创建新的积分规则。
    /// 规则类型包括：下单赠送、评价赠送、签到赠送、邀请赠送。
    /// 支持固定积分或按金额倍数计算。
    /// </remarks>
    [HttpPost]
    public async Task<ApiResponse<string>> Create([FromBody] CreatePointRuleDto dto)
    {
        var result = await _pointService.CreateRuleAsync(dto);
        return ApiResponse<string>.Success(result, "积分规则创建成功");
    }

    /// <summary>
    /// 分页查询积分规则列表
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>积分规则分页列表</returns>
    /// <remarks>
    /// 支持按规则名称、类型、状态筛选。
    /// 支持分页查询。
    /// </remarks>
    [HttpGet("list")]
    public async Task<ApiResponse<PageResponse<PointRuleDto>>> GetList([FromQuery] PointRuleQueryDto query)
    {
        var result = await _pointService.GetRuleListAsync(query);
        return Success(result);
    }

    /// <summary>
    /// 获取积分规则详情
    /// </summary>
    /// <param name="id">规则ID</param>
    /// <returns>积分规则详情</returns>
    [HttpGet("{id}")]
    public async Task<ApiResponse<PointRuleDto>> GetDetail(string id)
    {
        var result = await _pointService.GetRuleDetailAsync(id);
        return Success(result);
    }

    /// <summary>
    /// 更新积分规则
    /// </summary>
    /// <param name="id">规则ID</param>
    /// <param name="dto">更新积分规则参数</param>
    /// <returns>是否成功</returns>
    [HttpPut("{id}")]
    public async Task<ApiResponse<bool>> Update(string id, [FromBody] UpdatePointRuleDto dto)
    {
        // 确保 ID 一致
        dto.Id = id;
        var result = await _pointService.UpdateRuleAsync(dto);
        return Success(result);
    }

    /// <summary>
    /// 删除积分规则
    /// </summary>
    /// <param name="id">规则ID</param>
    /// <returns>是否成功</returns>
    /// <remarks>
    /// 软删除积分规则。
    /// </remarks>
    [HttpDelete("{id}")]
    public async Task<ApiResponse<bool>> Delete(string id)
    {
        var result = await _pointService.DeleteRuleAsync(id);
        return Success(result);
    }

    /// <summary>
    /// 获取启用的积分规则列表
    /// </summary>
    /// <param name="type">规则类型（可选）</param>
    /// <returns>启用的积分规则列表</returns>
    /// <remarks>
    /// 获取当前生效的积分规则。
    /// 可按类型筛选。
    /// </remarks>
    [HttpGet("active")]
    public async Task<ApiResponse<List<PointRuleDto>>> GetActiveRules([FromQuery] int? type = null)
    {
        var result = await _pointService.GetActiveRulesAsync(type.HasValue ? (Models.Enums.Mall.PointRuleType?)type.Value : null);
        return Success(result);
    }
}