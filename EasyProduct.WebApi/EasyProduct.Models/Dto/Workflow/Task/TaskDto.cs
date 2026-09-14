using System;
using EasyProduct.Models.Dto.Workflow.Instance;

namespace EasyProduct.Models.Dto.Workflow.Task
{
    /// <summary>
    /// 流程任务输出
    /// </summary>
    public class TaskDto
    {
        /// <summary>
        /// 任务ID
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// 流程实例ID
        /// </summary>
        public string InstanceId { get; set; } = string.Empty;

        /// <summary>
        /// 节点ID
        /// </summary>
        public string NodeId { get; set; } = string.Empty;

        /// <summary>
        /// 节点名称
        /// </summary>
        public string NodeName { get; set; } = string.Empty;

        /// <summary>
        /// 受托人ID
        /// </summary>
        public string AssigneeId { get; set; } = string.Empty;

        /// <summary>
        /// 受托人姓名
        /// </summary>
        public string AssigneeName { get; set; } = string.Empty;

        /// <summary>
        /// 任务状态（0-待处理、1-已批准、2-已拒绝、3-已委托、4-已取消）
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 审批意见
        /// </summary>
        public string? Comment { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 完成时间
        /// </summary>
        public DateTime? CompletedAt { get; set; }

        /// <summary>
        /// 流程实例信息
        /// </summary>
        public InstanceDto? Instance { get; set; }
    }
}
