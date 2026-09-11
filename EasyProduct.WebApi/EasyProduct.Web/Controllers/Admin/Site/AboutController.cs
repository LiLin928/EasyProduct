using EasyProduct.Business.Site;
using EasyProduct.Common.Base;
using EasyProduct.Models.Dto.Site.About;
using EasyProduct.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyProduct.Web.Controllers.Admin.Site;

/// <summary>
/// 关于我们管理控制器（管理端）
/// </summary>
/// <remarks>
/// 提供关于我们的获取和更新功能
/// 管理端接口，需要 Admin JWT 认证
/// 单页内容，不支持创建和删除
/// 权限标识前缀：site:about:
/// </remarks>
[ApiController]
[Route("api/admin/site/about")]
[Authorize(AuthenticationSchemes = "AdminJwt")]
public class AboutController : BaseController
{
    private readonly IAboutService _aboutService;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="aboutService">关于我们服务</param>
    public AboutController(IAboutService aboutService)
    {
        _aboutService = aboutService;
    }

    /// <summary>
    /// 获取关于我们详情
    /// </summary>
    /// <param name="id">关于我们ID</param>
    /// <returns>关于我们详情</returns>
    /// <remarks>
    /// 根据ID获取关于我们的详细信息
    /// </remarks>
    [HttpGet("{id}")]
    public async Task<ApiResponse<AboutDto>> GetById(string id)
    {
        var result = await _aboutService.GetByIdAsync(id);
        return Success(result!);
    }

    /// <summary>
    /// 更新关于我们
    /// </summary>
    /// <param name="dto">更新参数</param>
    /// <returns>是否成功</returns>
    /// <remarks>
    /// 更新关于我们信息
    /// </remarks>
    [HttpPut]
    public async Task<ApiResponse<bool>> Update([FromBody] UpdateAboutDto dto)
    {
        var result = await _aboutService.UpdateAsync(dto);
        return Success(result, "关于我们更新成功");
    }

    /// <summary>
    /// 更新关于我们状态
    /// </summary>
    /// <param name="id">关于我们ID</param>
    /// <param name="status">状态：0=禁用，1=启用</param>
    /// <returns>是否成功</returns>
    /// <remarks>
    /// 更新关于我们的状态（启用/禁用）
    /// </remarks>
    [HttpPut("{id}/status")]
    public async Task<ApiResponse<bool>> UpdateStatus(string id, [FromQuery] int status)
    {
        var result = await _aboutService.UpdateStatusAsync(id, status);
        return Success(result, status == 1 ? "关于我们已启用" : "关于我们已禁用");
    }
}