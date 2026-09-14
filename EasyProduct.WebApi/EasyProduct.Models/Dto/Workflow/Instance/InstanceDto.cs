using System;

namespace EasyProduct.Models.Dto.Workflow.Instance
{
    /// <summary>
    /// 流程实例输出
    /// </summary>
    public class InstanceDto
    {
        /// <summary>
        /// 流程实例ID
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// 流程定义ID
        /// </summary>
        public string DefinitionId { get; set; } = string.Empty;

        /// <summary>
        /// 流程定义名称
        /// </summary>
        public string DefinitionName { get; set; } = string.Empty;

        /// <summary>
        /// 业务单据ID
        /// </summary>
        public string BusinessKey { get; set; } = string.Empty;

        /// <summary>
        /// 业务类型
        /// </summary>
        public string BusinessType { get; set; } = string.Empty;

        /// <summary>
        /// 流程标题
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// 申请人ID
        /// </summary>
        public string ApplicantId { get; set; } = string.Empty;

        /// <summary>
        /// 申请人姓名
        /// </summary>
        public string ApplicantName { get; set; } = string.Empty;

        /// <summary>
        /// 当前节点ID
        /// </summary>
        public string CurrentNodeId { get; set; } = string.Empty;

        /// <summary>
        /// 当前节点名称
        /// </summary>
        public string CurrentNodeName { get; set; } = string.Empty;

        /// <summary>
        /// 流程状态（0-运行中、1-已完成、2-已取消、3-已拒绝）
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndTime { get; set; }
    }
}