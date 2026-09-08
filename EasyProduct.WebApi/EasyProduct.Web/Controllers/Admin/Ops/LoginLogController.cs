using EasyProduct.Business.Ops;
using EasyProduct.Common.Base;
using EasyProduct.Models.Dto.Ops.LoginLog;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyProduct.Web.Controllers.Admin.Ops;

/// <summary>
/// 登录日志管理控制器
/// </summary>
/// <remarks>
/// 提供登录日志的查询和删除功能
/// 管理端接口，需要 Admin JWT 认证
/// </remarks>
[ApiController]
[Route("api/admin/ops/login-log")]
[Authorize(AuthenticationSchemes = "AdminJwt")]
public class LoginLogController : BaseController
{
    private readonly ILoginLogService _loginLogService;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="loginLogService">登录日志服务</param>
    public LoginLogController(ILoginLogService loginLogService)
    {
        _loginLogService = loginLogService;
    }

    /// <summary>
    /// 获取登录日志分页列表
    /// </summary>
    /// <param name="query">查询参数，包含分页、筛选条件等</param>
    /// <returns>登录日志分页列表</returns>
    /// <remarks>
    /// 支持按用户名、登录IP、状态、时间范围筛选
    /// 默认按登录时间倒序排列
    /// </remarks>
    [HttpGet("list")]
    public async Task<ApiResponse<PageResponse<LoginLogDto>>> GetPageList([FromQuery] LoginLogQueryDto query)
    {
        var result = await _loginLogService.GetPageListAsync(query);
        return Success(result);
    }

    /// <summary>
    /// 获取登录日志详情
    /// </summary>
    /// <param name="id">日志ID</param>
    /// <returns>登录日志详情</returns>
    /// <remarks>
    /// 根据ID获取登录日志的详细信息，包括登录设备、浏览器、操作系统等
    /// </remarks>
    [HttpGet("{id}")]
    public async Task<ApiResponse<LoginLogDto>> GetById(string id)
    {
        var result = await _loginLogService.GetByIdAsync(id);
        return Success(result);
    }

    /// <summary>
    /// 删除登录日志
    /// </summary>
    /// <param name="id">日志ID</param>
    /// <returns>是否成功</returns>
    /// <remarks>
    /// 软删除登录日志，不会物理删除数据
    /// </remarks>
    [HttpDelete("{id}")]
    public async Task<ApiResponse<bool>> Delete(string id)
    {
        var result = await _loginLogService.DeleteAsync(id);
        return Success(result, "删除成功");
    }

    /// <summary>
    /// 批量删除登录日志
    /// </summary>
    /// <param name="ids">日志ID列表</param>
    /// <returns>是否成功</returns>
    /// <remarks>
    /// 批量软删除登录日志，不会物理删除数据
    /// </remarks>
    [HttpDelete("batch")]
    public async Task<ApiResponse<bool>> DeleteBatch([FromBody] List<string> ids)
    {
        var result = await _loginLogService.DeleteBatchAsync(ids);
        return Success(result, "批量删除成功");
    }
}