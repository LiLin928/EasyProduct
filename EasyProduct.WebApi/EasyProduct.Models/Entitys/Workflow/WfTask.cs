using SqlSugar;
using EasyProduct.Models.Entitys.Base;
using EasyProduct.Models.Constants;

namespace EasyProduct.Models.Entitys.Workflow
{
    /// <summary>
    /// 流程任务实体
    /// </summary>
    [SugarTable("wf_task", "流程任务表")]
    public class WfTask : BaseEntity
    {
        /// <summary>
        /// 流程实例ID
        /// </summary>
        public Guid InstanceId { get; set; }

        /// <summary>
        /// 节点ID
        /// </summary>
        [SugarColumn(Length = 50)]
        public string NodeId { get; set; } = string.Empty;

        /// <summary>
        /// 节点名称
        /// </summary>
        [SugarColumn(Length = 100)]
        public string NodeName { get; set; } = string.Empty;

        /// <summary>
        /// 受托人ID
        /// </summary>
        public Guid AssigneeId { get; set; }

        /// <summary>
        /// 受托人姓名（冗余字段）
        /// </summary>
        [SugarColumn(Length = 50)]
        public string AssigneeName { get; set; } = string.Empty;

        /// <summary>
        /// 任务状态（0-待处理、1-已批准、2-已拒绝、3-已委托、4-已取消）
        /// </summary>
        public int Status { get; set; } = WorkflowConstants.TaskStatus.PENDING;

        /// <summary>
        /// 审批意见
        /// </summary>
        [SugarColumn(ColumnDataType = "text", IsNullable = true)]
        public string? Comment { get; set; }

        /// <summary>
        /// 完成时间
        /// </summary>
        public DateTime? CompletedAt { get; set; }

        /// <summary>
        /// 委托人ID（委托任务时记录原受托人）
        /// </summary>
        public Guid? DelegatorId { get; set; }

        /// <summary>
        /// 委托人姓名
        /// </summary>
        [SugarColumn(Length = 50, IsNullable = true)]
        public string? DelegatorName { get; set; }
    }
}