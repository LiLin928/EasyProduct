using System;
using System.Collections.Generic;
using EasyProduct.Models.Entitys.Workflow;

namespace EasyProduct.Models.Dto.Workflow.Definition
{
    /// <summary>
    /// 流程定义输出
    /// </summary>
    public class DefinitionDto
    {
        /// <summary>
        /// 流程定义ID
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// 流程编码
        /// </summary>
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// 流程名称
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 版本号
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// 流程分类
        /// </summary>
        public string Category { get; set; } = string.Empty;

        /// <summary>
        /// 流程描述
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// 流程节点列表
        /// </summary>
        public List<WfNode> Nodes { get; set; } = new();

        /// <summary>
        /// 流程状态（0-草稿、1-已发布、2-已归档）
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateTime { get; set; }
    }
}