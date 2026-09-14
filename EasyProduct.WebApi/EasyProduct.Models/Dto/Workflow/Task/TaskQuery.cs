using EasyProduct.Models.Dto.Common;

namespace EasyProduct.Models.Dto.Workflow.Task
{
    /// <summary>
    /// 流程任务查询参数
    /// </summary>
    public class TaskQuery : PageQuery
    {
        /// <summary>
        /// 任务状态（0-待处理、1-已批准、2-已拒绝、3-已委托、4-已取消）
        /// </summary>
        public int? Status { get; set; }
    }
}