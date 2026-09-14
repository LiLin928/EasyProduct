using System.ComponentModel.DataAnnotations;
using EasyProduct.Models.Dto.Common;

namespace EasyProduct.Models.Dto.Workflow.Definition
{
    /// <summary>
    /// 流程定义查询参数
    /// </summary>
    public class DefinitionQuery : PageQuery
    {
        /// <summary>
        /// 流程编码/名称关键词
        /// </summary>
        [MaxLength(100)]
        public string? Keyword { get; set; }

        /// <summary>
        /// 流程分类
        /// </summary>
        [MaxLength(50)]
        public string? Category { get; set; }

        /// <summary>
        /// 流程状态（0-草稿、1-已发布、2-已归档）
        /// </summary>
        public int? Status { get; set; }
    }
}