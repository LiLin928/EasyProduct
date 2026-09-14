using System.ComponentModel.DataAnnotations;
using EasyProduct.Models.Dto.Common;

namespace EasyProduct.Models.Dto.Workflow.Instance
{
    /// <summary>
    /// 流程实例查询参数
    /// </summary>
    public class InstanceQuery : PageQuery
    {
        /// <summary>
        /// 流程标题关键词
        /// </summary>
        [MaxLength(200)]
        public string? Keyword { get; set; }

        /// <summary>
        /// 业务类型
        /// </summary>
        [MaxLength(50)]
        public string? BusinessType { get; set; }

        /// <summary>
        /// 流程状态（0-运行中、1-已完成、2-已取消、3-已拒绝）
        /// </summary>
        public int? Status { get; set; }

        /// <summary>
        /// 申请人ID
        /// </summary>
        public Guid? ApplicantId { get; set; }
    }
}