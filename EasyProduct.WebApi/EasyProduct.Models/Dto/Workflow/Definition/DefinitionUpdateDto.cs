using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using EasyProduct.Models.Entitys.Workflow;

namespace EasyProduct.Models.Dto.Workflow.Definition
{
    /// <summary>
    /// 流程定义更新参数
    /// </summary>
    public class DefinitionUpdateDto
    {
        /// <summary>
        /// 流程名称
        /// </summary>
        [Required(ErrorMessage = "流程名称不能为空")]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 流程分类
        /// </summary>
        [MaxLength(50)]
        public string Category { get; set; } = string.Empty;

        /// <summary>
        /// 流程描述
        /// </summary>
        [MaxLength(500)]
        public string? Description { get; set; }

        /// <summary>
        /// 流程节点列表
        /// </summary>
        public List<WfNode> Nodes { get; set; } = new();
    }
}