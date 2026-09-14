using System;
using System.Threading.Tasks;
using EasyProduct.Common.Base;
using EasyProduct.Models.Dto.Workflow.Instance;

namespace EasyProduct.Business.Workflow
{
    /// <summary>
    /// 流程实例服务接口
    /// </summary>
    public interface IWorkflowInstanceService
    {
        /// <summary>
        /// 获取流程实例列表（分页）
        /// </summary>
        /// <param name="query">查询参数</param>
        /// <returns>流程实例分页结果</returns>
        Task<PageResponse<InstanceDto>> GetListAsync(InstanceQuery query);

        /// <summary>
        /// 获取流程实例详情
        /// </summary>
        /// <param name="id">流程实例ID</param>
        /// <returns>流程实例详情</returns>
        Task<InstanceDto> GetByIdAsync(Guid id);

        /// <summary>
        /// 发起流程
        /// </summary>
        /// <param name="dto">发起流程参数</param>
        /// <param name="applicantId">申请人ID</param>
        /// <param name="applicantName">申请人姓名</param>
        /// <returns>流程实例ID</returns>
        Task<string> StartAsync(StartWorkflowDto dto, Guid applicantId, string applicantName);

        /// <summary>
        /// 撤销流程
        /// </summary>
        /// <param name="id">流程实例ID</param>
        /// <param name="applicantId">申请人ID</param>
        /// <returns>是否成功</returns>
        Task<bool> CancelAsync(Guid id, Guid applicantId);
    }
}