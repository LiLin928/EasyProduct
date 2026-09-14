using EasyProduct.Business.Workflow;
using EasyProduct.Common.Base;
using EasyProduct.Models.Dto.Workflow.Instance;
using EasyProduct.Models.Dto.Workflow.Task;
using EasyProduct.Web.Controllers.Admin.Base;
using Microsoft.AspNetCore.Mvc;

namespace EasyProduct.Web.Controllers.Admin.Workflow
{
    /// <summary>
    /// 流程任务控制器
    /// </summary>
    /// <remarks>
    /// 提供流程任务管理的接口，包括待办任务、已办任务、审批、委托等功能
    /// </remarks>
    public class TaskController : AdminControllerBase
    {
        /// <summary>
        /// 流程任务服务接口（属性注入）
        /// </summary>
        public IWorkflowTaskService _taskService { get; set; } = null!;

        /// <summary>
        /// 流程实例服务接口（属性注入）
        /// </summary>
        public IWorkflowInstanceService _instanceService { get; set; } = null!;

        /// <summary>
        /// 获取我的申请列表（分页）
        /// </summary>
        [HttpGet("my-apply/list")]
        public async Task<ApiResponse<PageResponse<InstanceDto>>> GetMyApplyList([FromQuery] InstanceQuery query)
        {
            query.ApplicantId = GetCurrentUserId();
            var result = await _instanceService.GetListAsync(query);
            return Success(result);
        }

        /// <summary>
        /// 发起流程
        /// </summary>
        [HttpPost("my-apply/start")]
        public async Task<ApiResponse<object>> Start([FromBody] StartWorkflowDto dto)
        {
            var userId = GetCurrentUserId();
            var userName = GetCurrentRealName();
            var id = await _instanceService.StartAsync(dto, userId, userName);
            return Success(id);
        }

        /// <summary>
        /// 获取待办任务列表（分页）
        /// </summary>
        [HttpGet("todo/list")]
        public async Task<ApiResponse<PageResponse<TaskDto>>> GetTodoList([FromQuery] TaskQuery query)
        {
            var userId = GetCurrentUserId();
            var result = await _taskService.GetTodoListAsync(query, userId);
            return Success(result);
        }

        /// <summary>
        /// 获取待办任务详情
        /// </summary>
        [HttpGet("todo/{id}")]
        public async Task<ApiResponse<TaskDto>> GetTodoById(string id)
        {
            var result = await _taskService.GetByIdAsync(Guid.Parse(id));
            return Success(result);
        }

        /// <summary>
        /// 审批通过
        /// </summary>
        [HttpPost("todo/{id}/approve")]
        public async Task<ApiResponse<bool>> Approve(string id, [FromBody] ApproveDto dto)
        {
            var userId = GetCurrentUserId();
            var result = await _taskService.ApproveAsync(Guid.Parse(id), dto, userId);
            return Success(result);
        }

        /// <summary>
        /// 审批拒绝
        /// </summary>
        [HttpPost("todo/{id}/reject")]
        public async Task<ApiResponse<bool>> Reject(string id, [FromBody] RejectDto dto)
        {
            var userId = GetCurrentUserId();
            var result = await _taskService.RejectAsync(Guid.Parse(id), dto, userId);
            return Success(result);
        }

        /// <summary>
        /// 委托任务
        /// </summary>
        [HttpPost("todo/{id}/delegate")]
        public async Task<ApiResponse<bool>> Delegate(string id, [FromBody] DelegateDto dto)
        {
            var userId = GetCurrentUserId();
            var result = await _taskService.DelegateAsync(Guid.Parse(id), dto, userId);
            return Success(result);
        }

        /// <summary>
        /// 获取已办任务列表（分页）
        /// </summary>
        [HttpGet("done/list")]
        public async Task<ApiResponse<PageResponse<TaskDto>>> GetDoneList([FromQuery] TaskQuery query)
        {
            var userId = GetCurrentUserId();
            var result = await _taskService.GetDoneListAsync(query, userId);
            return Success(result);
        }
    }
}
