using System.ComponentModel.DataAnnotations;

namespace EasyProduct.Models.Dto.Workflow.Instance
{
    /// <summary>
    /// 发起流程参数
    /// </summary>
    public class StartWorkflowDto
    {
        /// <summary>
        /// 流程定义ID
        /// </summary>
        [Required(ErrorMessage = "流程定义ID不能为空")]
        public string DefinitionId { get; set; } = string.Empty;

        /// <summary>
        /// 业务单据ID
        /// </summary>
        [Required(ErrorMessage = "业务单据ID不能为空")]
        [MaxLength(50)]
        public string BusinessKey { get; set; } = string.Empty;

        /// <summary>
        /// 业务类型
        /// </summary>
        [Required(ErrorMessage = "业务类型不能为空")]
        [MaxLength(50)]
        public string BusinessType { get; set; } = string.Empty;

        /// <summary>
        /// 流程标题
        /// </summary>
        [Required(ErrorMessage = "流程标题不能为空")]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;
    }
}