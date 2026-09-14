using EasyProduct.Business.Report;
using EasyProduct.Common.Base;
using EasyProduct.Models.Dto.Report.Datasource;
using EasyProduct.Web.Controllers.Admin.Base;
using Microsoft.AspNetCore.Mvc;

namespace EasyProduct.Web.Controllers.Admin.Report;

/// <summary>
/// 数据源管理控制器
/// </summary>
/// <remarks>
/// 提供数据源管理的 CRUD 接口，包括数据源列表、创建、更新、删除、测试连接等功能
/// 支持多种数据库类型：MySQL、PostgreSQL、SQL Server、Oracle
/// </remarks>
public class DatasourceController : AdminControllerBase
{
    /// <summary>
    /// 数据源服务接口（属性注入）
    /// </summary>
    public IRptDatasourceService _datasourceService { get; set; } = null!;

    /// <summary>
    /// 获取数据源分页列表
    /// </summary>
    /// <param name="query">查询参数，包含分页、名称、类型、状态等筛选条件</param>
    /// <returns>数据源分页列表结果</returns>
    /// <remarks>
    /// 支持按名称模糊搜索、数据源类型筛选、连接状态筛选
    /// 默认按创建时间倒序排列
    /// 权限要求：report:datasource:list
    /// </remarks>
    [HttpGet]
    public async Task<ApiResponse<PageResponse<RptDatasourceDto>>> GetList([FromQuery] RptDatasourceQuery query)
    {
        var result = await _datasourceService.GetListAsync(query);
        return Success(result);
    }

    /// <summary>
    /// 根据ID获取数据源详情
    /// </summary>
    /// <param name="id">数据源ID</param>
    /// <returns>数据源详情信息，如果不存在则返回错误</returns>
    /// <remarks>
    /// 返回数据源的基本信息，包括连接参数、连接状态等
    /// 密码字段会脱敏显示为 ******
    /// 权限要求：report:datasource:detail
    /// </remarks>
    [HttpGet("{id}")]
    public async Task<ApiResponse<RptDatasourceDto>> GetById(Guid id)
    {
        var result = await _datasourceService.GetByIdAsync(id);
        if (result == null)
        {
            return Error<RptDatasourceDto>("数据源不存在", 404);
        }
        return Success(result);
    }

    /// <summary>
    /// 创建数据源
    /// </summary>
    /// <param name="dto">创建数据源参数</param>
    /// <returns>创建成功返回新记录的ID</returns>
    /// <remarks>
    /// 1. 验证数据源名称唯一性
    /// 2. 验证连接参数的完整性
    /// 3. 密码进行加密存储
    /// 权限要求：report:datasource:create
    /// </remarks>
    [HttpPost]
    public async Task<ApiResponse<object>> Create([FromBody] RptDatasourceCreateDto dto)
    {
        var id = await _datasourceService.CreateAsync(dto);
        return Success<object>(id.ToString(), "创建成功");
    }

    /// <summary>
    /// 更新数据源
    /// </summary>
    /// <param name="id">数据源ID</param>
    /// <param name="dto">更新数据源参数</param>
    /// <returns>更新成功返回 true，失败返回错误信息</returns>
    /// <remarks>
    /// 1. 验证数据源是否存在
    /// 2. 如果修改密码，需要加密存储
    /// 3. 如果修改连接参数，需要重置连接状态
    /// 权限要求：report:datasource:update
    /// </remarks>
    [HttpPut("{id}")]
    public async Task<ApiResponse<bool>> Update(Guid id, [FromBody] RptDatasourceUpdateDto dto)
    {
        var result = await _datasourceService.UpdateAsync(id, dto);
        if (!result)
        {
            return Error<bool>("更新失败，数据源不存在", 404);
        }
        return Success(result, "更新成功");
    }

    /// <summary>
    /// 删除数据源
    /// </summary>
    /// <param name="id">数据源ID</param>
    /// <returns>删除成功返回 true，失败返回错误信息</returns>
    /// <remarks>
    /// 删除前需要检查该数据源是否被报表引用，如果被引用则不允许删除
    /// 权限要求：report:datasource:delete
    /// </remarks>
    [HttpDelete("{id}")]
    public async Task<ApiResponse<bool>> Delete(Guid id)
    {
        var result = await _datasourceService.DeleteAsync(id);
        if (!result)
        {
            return Error<bool>("删除失败，数据源不存在或被报表引用", 400);
        }
        return Success(result, "删除成功");
    }

    /// <summary>
    /// 测试数据源连接
    /// </summary>
    /// <param name="id">数据源ID</param>
    /// <returns>连接测试结果，包含是否成功、错误信息等</returns>
    /// <remarks>
    /// 1. 根据数据源类型使用对应的连接器进行连接测试
    /// 2. 更新数据源的连接状态和最后测试时间
    /// 3. 记录测试结果和错误信息
    /// 权限要求：report:datasource:test
    /// </remarks>
    [HttpPost("{id}/test")]
    public async Task<ApiResponse<RptConnectionTestResultDto>> TestConnection(Guid id)
    {
        var result = await _datasourceService.TestConnectionAsync(id);
        if (result.Success)
        {
            return Success(result, "连接测试成功");
        }
        return Success(result, "连接测试失败");
    }

    /// <summary>
    /// 获取所有数据源列表（不分页）
    /// </summary>
    /// <returns>所有数据源列表</returns>
    /// <remarks>
    /// 用于下拉选择框，只返回 ID、Name、Type 等基本字段
    /// 无权限要求，供内部调用使用
    /// </remarks>
    [HttpGet("all")]
    public async Task<ApiResponse<List<RptDatasourceDto>>> GetAll()
    {
        var result = await _datasourceService.GetAllAsync();
        return Success(result);
    }
}