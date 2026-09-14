using SqlSugar;
using System.ComponentModel.DataAnnotations;
using EasyProduct.Models.Entitys.Base;
using EasyProduct.Models.Constants;

namespace EasyProduct.Models.Entitys.Workflow
{
    /// <summary>
    /// 流程定义实体
    /// </summary>
    [SugarTable("wf_definition", "流程定义表")]
    public class WfDefinition : BaseEntity
    {
        /// <summary>
        /// 流程编码
        /// </summary>
        [SugarColumn(Length = 50)]
        [Required(ErrorMessage = "流程编码不能为空")]
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// 流程名称
        /// </summary>
        [SugarColumn(Length = 100)]
        [Required(ErrorMessage = "流程名称不能为空")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 版本号
        /// </summary>
        public int Version { get; set; } = 1;

        /// <summary>
        /// 流程分类
        /// </summary>
        [SugarColumn(Length = 50)]
        public string Category { get; set; } = string.Empty;

        /// <summary>
        /// 流程描述
        /// </summary>
        [SugarColumn(ColumnDataType = "text", IsNullable = true)]
        public string? Description { get; set; }

        /// <summary>
        /// 流程节点（JSON 存储）
        /// </summary>
        [SugarColumn(ColumnDataType = "text")]
        public string Nodes { get; set; } = "[]";

        /// <summary>
        /// 流程状态（0-草稿、1-已发布、2-已归档）
        /// </summary>
        public int Status { get; set; } = WorkflowConstants.DefinitionStatus.DRAFT;
    }
}