using EasyProduct.Business.Ops;
using EasyProduct.Common.Base;
using EasyProduct.Models.Dto.Ops.OperateLog;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyProduct.Web.Controllers.Admin.Ops;

/// <summary>
/// 操作日志管理控制器
/// </summary>
/// <remarks>
/// 提供操作日志的查询和删除功能
/// 管理端接口，需要 Admin JWT 认证
/// </remarks>
[ApiController]
[Route("api/admin/ops/operate-log")]
[Authorize(AuthenticationSchemes = "AdminJwt")]
public class OperateLogController : BaseController
{
    private readonly IOperateLogService _operateLogService;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="operateLogService">操作日志服务</param>
    public OperateLogController(IOperateLogService operateLogService)
    {
        _operateLogService = operateLogService;
    }

    /// <summary>
    /// 获取操作日志分页列表
    /// </summary>
    /// <param name="query">查询参数，包含分页、筛选条件等</param>
    /// <returns>操作日志分页列表</returns>
    /// <remarks>
    /// 支持按模块、操作类型、用户名、状态、时间范围筛选
    /// 默认按创建时间倒序排列
    /// </remarks>
    [HttpGet("list")]
    public async Task<ApiResponse<PageResponse<OperateLogDto>>> GetPageList([FromQuery] OperateLogQueryDto query)
    {
        var result = await _operateLogService.GetPageListAsync(query);
        return Success(result);
    }

    /// <summary>
    /// 获取操作日志详情
    /// </summary>
    /// <param name="id">日志ID</param>
    /// <returns>操作日志详情</returns>
    /// <remarks>
    /// 根据ID获取操作日志的详细信息，包括请求参数、响应结果等
    /// </remarks>
    [HttpGet("{id}")]
    public async Task<ApiResponse<OperateLogDto>> GetById(string id)
    {
        var result = await _operateLogService.GetByIdAsync(id);
        return Success(result);
    }

    /// <summary>
    /// 删除操作日志
    /// </summary>
    /// <param name="id">日志ID</param>
    /// <returns>是否成功</returns>
    /// <remarks>
    /// 软删除操作日志，不会物理删除数据
    /// </remarks>
    [HttpDelete("{id}")]
    public async Task<ApiResponse<bool>> Delete(string id)
    {
        var result = await _operateLogService.DeleteAsync(id);
        return Success(result, "删除成功");
    }

    /// <summary>
    /// 批量删除操作日志
    /// </summary>
    /// <param name="ids">日志ID列表</param>
    /// <returns>是否成功</returns>
    /// <remarks>
    /// 批量软删除操作日志，不会物理删除数据
    /// </remarks>
    [HttpDelete("batch")]
    public async Task<ApiResponse<bool>> DeleteBatch([FromBody] List<string> ids)
    {
        var result = await _operateLogService.DeleteBatchAsync(ids);
        return Success(result, "批量删除成功");
    }
}