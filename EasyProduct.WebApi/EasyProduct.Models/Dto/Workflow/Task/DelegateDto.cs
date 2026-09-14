using System.ComponentModel.DataAnnotations;

namespace EasyProduct.Models.Dto.Workflow.Task
{
    /// <summary>
    /// 委托参数
    /// </summary>
    public class DelegateDto
    {
        /// <summary>
        /// 受委托人ID
        /// </summary>
        [Required(ErrorMessage = "受委托人ID不能为空")]
        public string AssigneeId { get; set; } = string.Empty;

        /// <summary>
        /// 委托说明
        /// </summary>
        [MaxLength(500)]
        public string? Comment { get; set; }
    }
}