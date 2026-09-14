using SqlSugar;
using System.ComponentModel.DataAnnotations;
using EasyProduct.Models.Entitys.Base;
using EasyProduct.Models.Constants;

namespace EasyProduct.Models.Entitys.Workflow
{
    /// <summary>
    /// 流程实例实体
    /// </summary>
    [SugarTable("wf_instance", "流程实例表")]
    public class WfInstance : BaseEntity
    {
        /// <summary>
        /// 流程定义ID
        /// </summary>
        public Guid DefinitionId { get; set; }

        /// <summary>
        /// 流程定义名称（冗余字段）
        /// </summary>
        [SugarColumn(Length = 100)]
        public string DefinitionName { get; set; } = string.Empty;

        /// <summary>
        /// 业务单据ID
        /// </summary>
        [SugarColumn(Length = 50)]
        [Required(ErrorMessage = "业务单据ID不能为空")]
        public string BusinessKey { get; set; } = string.Empty;

        /// <summary>
        /// 业务类型（如：order、contract、leave）
        /// </summary>
        [SugarColumn(Length = 50)]
        [Required(ErrorMessage = "业务类型不能为空")]
        public string BusinessType { get; set; } = string.Empty;

        /// <summary>
        /// 流程标题
        /// </summary>
        [SugarColumn(Length = 200)]
        [Required(ErrorMessage = "流程标题不能为空")]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// 申请人ID
        /// </summary>
        public Guid ApplicantId { get; set; }

        /// <summary>
        /// 申请人姓名（冗余字段）
        /// </summary>
        [SugarColumn(Length = 50)]
        public string ApplicantName { get; set; } = string.Empty;

        /// <summary>
        /// 当前节点ID
        /// </summary>
        [SugarColumn(Length = 50)]
        public string CurrentNodeId { get; set; } = string.Empty;

        /// <summary>
        /// 当前节点名称（冗余字段）
        /// </summary>
        [SugarColumn(Length = 100)]
        public string CurrentNodeName { get; set; } = string.Empty;

        /// <summary>
        /// 流程状态（0-运行中、1-已完成、2-已取消、3-已拒绝）
        /// </summary>
        public int Status { get; set; } = WorkflowConstants.InstanceStatus.RUNNING;

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime StartTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndTime { get; set; }
    }
}