using EasyProduct.Business.Report;
using EasyProduct.Common.Base;
using EasyProduct.Models.Dto.Report.ColumnTemplate;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyProduct.Web.Controllers.Admin.Report;

/// <summary>
/// 报表列模板管理控制器
/// </summary>
/// <remarks>
/// 提供报表列模板的管理接口，包括分页列表、详情查询、创建、更新、删除等功能。
/// 所有接口需要管理员权限（AdminJwt 认证）。
/// </remarks>
[ApiController]
[Route("api/admin/report/column-template")]
[Authorize(AuthenticationSchemes = "AdminJwt")]
public class ColumnTemplateController : BaseController
{
    private readonly IRptColumnTemplateService _columnTemplateService;
    private readonly ILogger<ColumnTemplateController> _logger;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="columnTemplateService">报表列模板服务接口</param>
    /// <param name="logger">日志记录器</param>
    public ColumnTemplateController(
        IRptColumnTemplateService columnTemplateService,
        ILogger<ColumnTemplateController> logger)
    {
        _columnTemplateService = columnTemplateService;
        _logger = logger;
    }

    /// <summary>
    /// 获取列模板分页列表
    /// </summary>
    /// <param name="name">模板名称（可选），支持模糊搜索</param>
    /// <param name="type">字段类型（可选），精确匹配</param>
    /// <param name="pageIndex">页码，从 1 开始，默认 1</param>
    /// <param name="pageSize">每页数量，默认 10</param>
    /// <returns>列模板分页列表</returns>
    /// <remarks>
    /// 1. 支持按模板名称模糊搜索、字段类型精确匹配
    /// 2. 默认按创建时间倒序排列
    /// </remarks>
    [HttpGet("list")]
    public async Task<ApiResponse<PageResponse<RptColumnTemplateDto>>> GetList(
        [FromQuery] string? name = null,
        [FromQuery] string? type = null,
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new RptColumnTemplateQuery
        {
            Name = name,
            Type = type,
            PageIndex = pageIndex,
            PageSize = pageSize
        };

        var result = await _columnTemplateService.GetListAsync(query);
        return Success(result);
    }

    /// <summary>
    /// 获取列模板详情
    /// </summary>
    /// <param name="id">列模板ID</param>
    /// <returns>列模板详情</returns>
    /// <remarks>
    /// 根据列模板ID获取详细信息，包括模板名称、字段名、字段类型、列宽、格式化规则等
    /// </remarks>
    [HttpGet("{id}")]
    public async Task<ApiResponse<RptColumnTemplateDto>> GetById(Guid id)
    {
        var result = await _columnTemplateService.GetByIdAsync(id);
        return Success(result);
    }

    /// <summary>
    /// 创建列模板
    /// </summary>
    /// <param name="dto">创建列模板参数</param>
    /// <returns>创建结果</returns>
    /// <remarks>
    /// 1. 模板名称不能为空，最大长度 100 个字符
    /// 2. 字段名不能为空，最大长度 50 个字符
    /// 3. 字段类型必填，默认为 "string"
    /// 4. 列宽范围 50-500，默认 150
    /// 5. 创建成功返回新模板ID
    /// </remarks>
    [HttpPost]
    public async Task<ApiResponse<object>> Create([FromBody] RptColumnTemplateCreateDto dto)
    {
        var result = await _columnTemplateService.CreateAsync(dto);
        return Success<object>(result.ToString(), "创建成功");
    }

    /// <summary>
    /// 更新列模板
    /// </summary>
    /// <param name="id">列模板ID</param>
    /// <param name="dto">更新列模板参数</param>
    /// <returns>更新结果</returns>
    /// <remarks>
    /// 1. 所有字段均为可选，只更新传入的字段
    /// 2. 更新成功返回 true
    /// </remarks>
    [HttpPut("{id}")]
    public async Task<ApiResponse<object>> Update(Guid id, [FromBody] RptColumnTemplateUpdateDto dto)
    {
        var result = await _columnTemplateService.UpdateAsync(id, dto);
        return result ? Success("更新成功") : Error<object>("更新失败");
    }

    /// <summary>
    /// 删除列模板
    /// </summary>
    /// <param name="id">列模板ID</param>
    /// <returns>删除结果</returns>
    /// <remarks>
    /// 1. 软删除，不会物理删除数据
    /// 2. 删除成功返回 true
    /// </remarks>
    [HttpDelete("{id}")]
    public async Task<ApiResponse<object>> Delete(Guid id)
    {
        var result = await _columnTemplateService.DeleteAsync(id);
        return result ? Success("删除成功") : Error<object>("删除失败");
    }

    /// <summary>
    /// 获取所有列模板列表（不分页）
    /// </summary>
    /// <returns>列模板列表</returns>
    /// <remarks>
    /// 1. 返回所有列模板，不分页
    /// 2. 用于下拉选择等场景
    /// </remarks>
    [HttpGet("all")]
    public async Task<ApiResponse<List<RptColumnTemplateDto>>> GetAll()
    {
        var result = await _columnTemplateService.GetAllAsync();
        return Success(result);
    }
}