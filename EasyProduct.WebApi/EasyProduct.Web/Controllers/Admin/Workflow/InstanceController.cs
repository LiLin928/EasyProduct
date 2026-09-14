using EasyProduct.Business.Workflow;
using EasyProduct.Common.Base;
using EasyProduct.Models.Dto.Workflow.Instance;
using EasyProduct.Web.Controllers.Admin.Base;
using Microsoft.AspNetCore.Mvc;

namespace EasyProduct.Web.Controllers.Admin.Workflow
{
    /// <summary>
    /// 流程实例控制器
    /// </summary>
    /// <remarks>
    /// 提供流程实例管理的接口，包括流程实例列表、详情、撤销等功能
    /// </remarks>
    public class InstanceController : AdminControllerBase
    {
        /// <summary>
        /// 流程实例服务接口（属性注入）
        /// </summary>
        public IWorkflowInstanceService _instanceService { get; set; } = null!;

        /// <summary>
        /// 获取流程实例列表（分页）
        /// </summary>
        /// <param name="query">查询参数</param>
        /// <returns>流程实例分页结果</returns>
        [HttpGet]
        public async Task<ApiResponse<PageResponse<InstanceDto>>> GetList([FromQuery] InstanceQuery query)
        {
            var result = await _instanceService.GetListAsync(query);
            return Success(result);
        }

        /// <summary>
        /// 获取流程实例详情
        /// </summary>
        /// <param name="id">流程实例ID</param>
        /// <returns>流程实例详情</returns>
        [HttpGet("{id}")]
        public async Task<ApiResponse<InstanceDto>> GetById(string id)
        {
            var result = await _instanceService.GetByIdAsync(Guid.Parse(id));
            return Success(result);
        }

        /// <summary>
        /// 撤销流程
        /// </summary>
        /// <param name="id">流程实例ID</param>
        /// <returns>是否成功</returns>
        [HttpPost("{id}/cancel")]
        public async Task<ApiResponse<bool>> Cancel(string id)
        {
            var userId = GetCurrentUserId();
            var result = await _instanceService.CancelAsync(Guid.Parse(id), userId);
            return Success(result);
        }
    }
}