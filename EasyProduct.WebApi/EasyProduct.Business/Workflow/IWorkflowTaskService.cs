using System;
using System.Threading.Tasks;
using EasyProduct.Common.Base;
using EasyProduct.Models.Dto.Workflow.Task;

namespace EasyProduct.Business.Workflow
{
    /// <summary>
    /// 流程任务服务接口
    /// </summary>
    public interface IWorkflowTaskService
    {
        /// <summary>
        /// 获取待办任务列表（分页）
        /// </summary>
        /// <param name="query">查询参数</param>
        /// <param name="assigneeId">受托人ID</param>
        /// <returns>任务分页结果</returns>
        Task<PageResponse<TaskDto>> GetTodoListAsync(TaskQuery query, Guid assigneeId);

        /// <summary>
        /// 获取已办任务列表（分页）
        /// </summary>
        /// <param name="query">查询参数</param>
        /// <param name="assigneeId">受托人ID</param>
        /// <returns>任务分页结果</returns>
        Task<PageResponse<TaskDto>> GetDoneListAsync(TaskQuery query, Guid assigneeId);

        /// <summary>
        /// 获取任务详情
        /// </summary>
        /// <param name="id">任务ID</param>
        /// <returns>任务详情</returns>
        Task<TaskDto> GetByIdAsync(Guid id);

        /// <summary>
        /// 审批通过
        /// </summary>
        /// <param name="id">任务ID</param>
        /// <param name="dto">审批参数</param>
        /// <param name="assigneeId">受托人ID</param>
        /// <returns>是否成功</returns>
        Task<bool> ApproveAsync(Guid id, ApproveDto dto, Guid assigneeId);

        /// <summary>
        /// 审批拒绝
        /// </summary>
        /// <param name="id">任务ID</param>
        /// <param name="dto">拒绝参数</param>
        /// <param name="assigneeId">受托人ID</param>
        /// <returns>是否成功</returns>
        Task<bool> RejectAsync(Guid id, RejectDto dto, Guid assigneeId);

        /// <summary>
        /// 委托任务
        /// </summary>
        /// <param name="id">任务ID</param>
        /// <param name="dto">委托参数</param>
        /// <param name="delegatorId">委托人ID</param>
        /// <returns>是否成功</returns>
        Task<bool> DelegateAsync(Guid id, DelegateDto dto, Guid delegatorId);
    }
}
