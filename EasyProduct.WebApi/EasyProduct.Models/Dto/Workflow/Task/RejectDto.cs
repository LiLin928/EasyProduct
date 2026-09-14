using System.ComponentModel.DataAnnotations;

namespace EasyProduct.Models.Dto.Workflow.Task
{
    /// <summary>
    /// 审批拒绝参数
    /// </summary>
    public class RejectDto
    {
        /// <summary>
        /// 拒绝原因
        /// </summary>
        [Required(ErrorMessage = "拒绝原因不能为空")]
        [MaxLength(500)]
        public string Comment { get; set; } = string.Empty;
    }
}