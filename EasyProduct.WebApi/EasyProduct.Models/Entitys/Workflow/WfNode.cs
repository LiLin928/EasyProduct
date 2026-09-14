using EasyProduct.Models.Constants;

namespace EasyProduct.Models.Entitys.Workflow
{
    /// <summary>
    /// 流程节点（存储在 WfDefinition.Nodes 字段中）
    /// </summary>
    public class WfNode
    {
        /// <summary>
        /// 节点ID
        /// </summary>
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// 节点名称
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 节点类型：start-开始节点、end-结束节点、task-任务节点、gateway-网关、subprocess-子流程
        /// </summary>
        public string Type { get; set; } = WorkflowConstants.NodeType.TASK;

        /// <summary>
        /// 分配类型：user-指定用户、role-角色、dept-部门、expression-表达式
        /// </summary>
        public string AssigneeType { get; set; } = string.Empty;

        /// <summary>
        /// 分配对象ID（用户ID/角色ID/部门ID）
        /// </summary>
        public string AssigneeId { get; set; } = string.Empty;

        /// <summary>
        /// 审批类型：or-或签、and-会签、sequential-顺序签
        /// </summary>
        public string ApprovalType { get; set; } = WorkflowConstants.ApprovalType.OR;

        /// <summary>
        /// 超时时间（小时）
        /// </summary>
        public int Timeout { get; set; } = 72;

        /// <summary>
        /// 节点配置（JSON 格式，存储扩展信息）
        /// </summary>
        public string? Config { get; set; }
    }
}