using EasyProduct.Business.Workflow;
using EasyProduct.Common.Base;
using EasyProduct.Models.Dto.Workflow.Definition;
using EasyProduct.Web.Controllers.Admin.Base;
using Microsoft.AspNetCore.Mvc;

namespace EasyProduct.Web.Controllers.Admin.Workflow
{
    /// <summary>
    /// 流程定义控制器
    /// </summary>
    /// <remarks>
    /// 提供流程定义管理的 CRUD 接口，包括流程定义列表、创建、更新、删除、发布等功能
    /// </remarks>
    public class DefinitionController : AdminControllerBase
    {
        /// <summary>
        /// 流程定义服务接口（属性注入）
        /// </summary>
        public IWorkflowDefinitionService _definitionService { get; set; } = null!;

        /// <summary>
        /// 获取流程定义列表（分页）
        /// </summary>
        /// <param name="query">查询参数</param>
        /// <returns>流程定义分页结果</returns>
        [HttpGet]
        public async Task<ApiResponse<PageResponse<DefinitionDto>>> GetList([FromQuery] DefinitionQuery query)
        {
            var result = await _definitionService.GetListAsync(query);
            return Success(result);
        }

        /// <summary>
        /// 获取流程定义详情
        /// </summary>
        /// <param name="id">流程定义ID</param>
        /// <returns>流程定义详情</returns>
        [HttpGet("{id}")]
        public async Task<ApiResponse<DefinitionDto>> GetById(string id)
        {
            var result = await _definitionService.GetByIdAsync(Guid.Parse(id));
            return Success(result);
        }

        /// <summary>
        /// 创建流程定义
        /// </summary>
        /// <param name="dto">创建参数</param>
        /// <returns>创建结果</returns>
        [HttpPost]
        public async Task<ApiResponse<object>> Create([FromBody] DefinitionCreateDto dto)
        {
            var id = await _definitionService.CreateAsync(dto);
            return Success(id);
        }

        /// <summary>
        /// 更新流程定义
        /// </summary>
        /// <param name="id">流程定义ID</param>
        /// <param name="dto">更新参数</param>
        /// <returns>是否成功</returns>
        [HttpPut("{id}")]
        public async Task<ApiResponse<bool>> Update(string id, [FromBody] DefinitionUpdateDto dto)
        {
            var result = await _definitionService.UpdateAsync(Guid.Parse(id), dto);
            return Success(result);
        }

        /// <summary>
        /// 删除流程定义
        /// </summary>
        /// <param name="id">流程定义ID</param>
        /// <returns>是否成功</returns>
        [HttpDelete("{id}")]
        public async Task<ApiResponse<bool>> Delete(string id)
        {
            var result = await _definitionService.DeleteAsync(Guid.Parse(id));
            return Success(result);
        }

        /// <summary>
        /// 发布流程定义
        /// </summary>
        /// <param name="id">流程定义ID</param>
        /// <returns>是否成功</returns>
        [HttpPost("{id}/publish")]
        public async Task<ApiResponse<bool>> Publish(string id)
        {
            var result = await _definitionService.PublishAsync(Guid.Parse(id));
            return Success(result);
        }
    }
}
