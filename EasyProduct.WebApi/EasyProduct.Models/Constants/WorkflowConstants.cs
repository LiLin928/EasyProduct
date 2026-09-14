namespace EasyProduct.Models.Constants
{
    /// <summary>
    /// 工作流常量
    /// </summary>
    public static class WorkflowConstants
    {
        /// <summary>
        /// 流程定义状态
        /// </summary>
        public static class DefinitionStatus
        {
            /// <summary>草稿</summary>
            public const int DRAFT = 0;

            /// <summary>已发布</summary>
            public const int PUBLISHED = 1;

            /// <summary>已归档</summary>
            public const int ARCHIVED = 2;
        }

        /// <summary>
        /// 流程实例状态
        /// </summary>
        public static class InstanceStatus
        {
            /// <summary>运行中</summary>
            public const int RUNNING = 0;

            /// <summary>已完成</summary>
            public const int COMPLETED = 1;

            /// <summary>已取消</summary>
            public const int CANCELLED = 2;

            /// <summary>已拒绝</summary>
            public const int REJECTED = 3;
        }

        /// <summary>
        /// 任务状态
        /// </summary>
        public static class TaskStatus
        {
            /// <summary>待处理</summary>
            public const int PENDING = 0;

            /// <summary>已批准</summary>
            public const int APPROVED = 1;

            /// <summary>已拒绝</summary>
            public const int REJECTED = 2;

            /// <summary>已委托</summary>
            public const int DELEGATED = 3;

            /// <summary>已取消</summary>
            public const int CANCELLED = 4;
        }

        /// <summary>
        /// 节点类型
        /// </summary>
        public static class NodeType
        {
            /// <summary>开始节点</summary>
            public const string START = "start";

            /// <summary>结束节点</summary>
            public const string END = "end";

            /// <summary>任务节点</summary>
            public const string TASK = "task";

            /// <summary>网关节点</summary>
            public const string GATEWAY = "gateway";

            /// <summary>子流程节点</summary>
            public const string SUBPROCESS = "subprocess";
        }

        /// <summary>
        /// 审批类型
        /// </summary>
        public static class ApprovalType
        {
            /// <summary>或签</summary>
            public const string OR = "or";

            /// <summary>会签</summary>
            public const string AND = "and";

            /// <summary>顺序签</summary>
            public const string SEQUENTIAL = "sequential";
        }

        /// <summary>
        /// 分配类型
        /// </summary>
        public static class AssigneeType
        {
            /// <summary>用户</summary>
            public const string USER = "user";

            /// <summary>角色</summary>
            public const string ROLE = "role";

            /// <summary>部门</summary>
            public const string DEPT = "dept";

            /// <summary>表达式</summary>
            public const string EXPRESSION = "expression";
        }
    }
}