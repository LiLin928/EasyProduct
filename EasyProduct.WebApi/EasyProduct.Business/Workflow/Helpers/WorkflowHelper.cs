using EasyProduct.Models.Entitys.Workflow;
using EasyProduct.Models.Constants;

namespace EasyProduct.Business.Workflow.Helpers
{
    /// <summary>
    /// 工作流工具类
    /// </summary>
    public static class WorkflowHelper
    {
        /// <summary>
        /// 验证流程节点是否有效
        /// </summary>
        /// <param name="nodes">节点列表</param>
        /// <returns>是否有效</returns>
        public static bool ValidateNodes(List<WfNode> nodes)
        {
            if (nodes == null || nodes.Count == 0)
            {
                return false;
            }

            // 检查是否有开始节点
            var hasStart = nodes.Any(x => x.Type == WorkflowConstants.NodeType.START);
            if (!hasStart)
            {
                return false;
            }

            // 检查是否有结束节点
            var hasEnd = nodes.Any(x => x.Type == WorkflowConstants.NodeType.END);
            if (!hasEnd)
            {
                return false;
            }

            // 检查任务节点是否有审批人
            foreach (var node in nodes.Where(x => x.Type == WorkflowConstants.NodeType.TASK))
            {
                if (string.IsNullOrEmpty(node.AssigneeType) || string.IsNullOrEmpty(node.AssigneeId))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 获取第一个任务节点
        /// </summary>
        /// <param name="nodes">节点列表</param>
        /// <returns>第一个任务节点</returns>
        public static WfNode? GetFirstTaskNode(List<WfNode> nodes)
        {
            return nodes?.FirstOrDefault(x => x.Type == WorkflowConstants.NodeType.TASK);
        }

        /// <summary>
        /// 获取下一个节点
        /// </summary>
        /// <param name="nodes">节点列表</param>
        /// <param name="currentNodeId">当前节点ID</param>
        /// <returns>下一个节点</returns>
        public static WfNode? GetNextNode(List<WfNode> nodes, string currentNodeId)
        {
            if (nodes == null || string.IsNullOrEmpty(currentNodeId))
            {
                return null;
            }

            var currentIndex = nodes.FindIndex(x => x.Id == currentNodeId);
            if (currentIndex < 0 || currentIndex >= nodes.Count - 1)
            {
                return null;
            }

            return nodes[currentIndex + 1];
        }
    }
}