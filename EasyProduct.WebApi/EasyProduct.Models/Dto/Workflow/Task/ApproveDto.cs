using System.ComponentModel.DataAnnotations;

namespace EasyProduct.Models.Dto.Workflow.Task
{
    /// <summary>
    /// 审批通过参数
    /// </summary>
    public class ApproveDto
    {
        /// <summary>
        /// 审批意见
        /// </summary>
        [MaxLength(500)]
        public string? Comment { get; set; }
    }
}