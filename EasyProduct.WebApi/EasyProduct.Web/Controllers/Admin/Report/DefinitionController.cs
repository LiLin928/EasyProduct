using EasyProduct.Business.Report;
using EasyProduct.Common.Base;
using EasyProduct.Models.Dto.Report.Definition;
using EasyProduct.Web.Controllers.Admin.Base;
using Microsoft.AspNetCore.Mvc;

namespace EasyProduct.Web.Controllers.Admin.Report
{
    /// <summary>
    /// 报表定义控制器
    /// </summary>
    /// <remarks>
    /// 提供报表定义管理的 CRUD 接口，包括报表定义列表、创建、更新、删除、执行、导出、发布、归档等功能
    /// </remarks>
    public class DefinitionController : AdminControllerBase
    {
        /// <summary>
        /// 报表定义服务接口（属性注入）
        /// </summary>
        public IRptDefinitionService _definitionService { get; set; } = null!;

        /// <summary>
        /// 获取报表定义列表（分页）
        /// </summary>
        /// <param name="query">查询参数，包含分页、名称、编码、分类、数据源、状态等筛选条件</param>
        /// <returns>报表定义分页结果</returns>
        /// <remarks>
        /// 支持以下筛选条件：
        /// 1. Name - 报表名称模糊搜索
        /// 2. Code - 报表编码精确匹配
        /// 3. Category - 报表分类
        /// 4. DatasourceId - 数据源ID
        /// 5. Status - 状态筛选
        /// </remarks>
        [HttpGet]
        public async Task<ApiResponse<PageResponse<RptDefinitionDto>>> GetList([FromQuery] RptDefinitionQuery query)
        {
            var result = await _definitionService.GetListAsync(query);
            return Success(result);
        }

        /// <summary>
        /// 获取报表定义详情
        /// </summary>
        /// <param name="id">报表定义ID</param>
        /// <returns>报表定义详情，包含列配置等信息</returns>
        [HttpGet("{id}")]
        public async Task<ApiResponse<RptDefinitionDto>> GetById(string id)
        {
            var result = await _definitionService.GetByIdAsync(Guid.Parse(id));
            return Success(result);
        }

        /// <summary>
        /// 创建报表定义
        /// </summary>
        /// <param name="dto">创建参数，包含报表名称、编码、分类、数据源、SQL模板、图表类型、列配置等</param>
        /// <returns>新报表定义ID</returns>
        /// <remarks>
        /// 创建报表定义时需注意：
        /// 1. 报表编码必须唯一
        /// 2. SQL模板需要符合规范
        /// 3. 列配置需要与 SQL 查询结果匹配
        /// </remarks>
        [HttpPost]
        public async Task<ApiResponse<object>> Create([FromBody] RptDefinitionCreateDto dto)
        {
            var id = await _definitionService.CreateAsync(dto);
            return Success<object>(id.ToString(), "报表定义创建成功");
        }

        /// <summary>
        /// 更新报表定义
        /// </summary>
        /// <param name="id">报表定义ID</param>
        /// <param name="dto">更新参数，包含报表名称、编码、分类、数据源、SQL模板、图表类型、列配置、状态等</param>
        /// <returns>是否成功</returns>
        /// <remarks>
        /// 更新报表定义时需注意：
        /// 1. 已发布的报表可能限制修改
        /// 2. 修改 SQL 模板会影响查询结果
        /// </remarks>
        [HttpPut("{id}")]
        public async Task<ApiResponse<bool>> Update(string id, [FromBody] RptDefinitionUpdateDto dto)
        {
            var result = await _definitionService.UpdateAsync(Guid.Parse(id), dto);
            return Success(result, "报表定义更新成功");
        }

        /// <summary>
        /// 删除报表定义
        /// </summary>
        /// <param name="id">报表定义ID</param>
        /// <returns>是否成功</returns>
        /// <remarks>
        /// 删除报表定义时需注意：
        /// 1. 已发布的报表不允许删除
        /// 2. 删除操作不可恢复
        /// </remarks>
        [HttpDelete("{id}")]
        public async Task<ApiResponse<bool>> Delete(string id)
        {
            var result = await _definitionService.DeleteAsync(Guid.Parse(id));
            return Success(result, "报表定义删除成功");
        }

        /// <summary>
        /// 执行报表查询
        /// </summary>
        /// <param name="id">报表定义ID</param>
        /// <param name="pageIndex">页码，默认为 1</param>
        /// <param name="pageSize">每页条数，默认为 100</param>
        /// <returns>查询结果，包含列配置和数据行</returns>
        /// <remarks>
        /// 执行报表查询时：
        /// 1. 会根据报表定义的 SQL 模板执行查询
        /// 2. 返回结果包含列配置和数据行
        /// 3. 支持分页参数
        /// </remarks>
        [HttpPost("{id}/execute")]
        public async Task<ApiResponse<RptExecuteResultDto>> Execute(string id, [FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 100)
        {
            var result = await _definitionService.ExecuteAsync(Guid.Parse(id), pageIndex, pageSize);
            return Success(result);
        }

        /// <summary>
        /// 导出报表数据到 Excel
        /// </summary>
        /// <param name="id">报表定义ID</param>
        /// <returns>Excel 文件</returns>
        /// <remarks>
        /// 导出功能说明：
        /// 1. 根据报表定义的 SQL 模板执行查询
        /// 2. 生成 Excel 文件
        /// 3. 返回文件流供下载
        /// </remarks>
        [HttpGet("{id}/export")]
        public async Task<IActionResult> Export(string id)
        {
            var bytes = await _definitionService.ExportToExcelAsync(Guid.Parse(id));
            var fileName = $"report_{id}_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        /// <summary>
        /// 发布报表
        /// </summary>
        /// <param name="id">报表定义ID</param>
        /// <returns>是否成功</returns>
        /// <remarks>
        /// 发布报表时：
        /// 1. 会验证报表定义的完整性
        /// 2. 发布后报表可供用户使用
        /// 3. 发布后部分字段可能限制修改
        /// </remarks>
        [HttpPost("{id}/publish")]
        public async Task<ApiResponse<bool>> Publish(string id)
        {
            var result = await _definitionService.PublishAsync(Guid.Parse(id));
            return Success(result, "报表发布成功");
        }

        /// <summary>
        /// 归档报表
        /// </summary>
        /// <param name="id">报表定义ID</param>
        /// <returns>是否成功</returns>
        /// <remarks>
        /// 归档报表时：
        /// 1. 报表状态变为归档状态
        /// 2. 归档后报表不再显示在列表中
        /// 3. 可以通过取消归档恢复报表
        /// </remarks>
        [HttpPost("{id}/archive")]
        public async Task<ApiResponse<bool>> Archive(string id)
        {
            var result = await _definitionService.ArchiveAsync(Guid.Parse(id));
            return Success(result, "报表归档成功");
        }
    }
}