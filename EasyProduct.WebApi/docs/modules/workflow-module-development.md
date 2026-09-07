# Workflow 模块后端开发说明文档

> **模块名称**：Workflow（工作流引擎）
> **适用范围**：EasyProduct.WebApi Workflow 模块
> **创建时间**：2026-09-07
> **依据文档**：`docs/api/other-modules.md`、`docs/backend-guidelines.md`

---

## 1. 模块概述

Workflow 模块是 EasyProduct 的核心业务流程引擎，提供灵活的工作流定义、审批流程管理和任务调度能力。

### 1.1 核心功能

- **流程定义管理**：支持可视化流程设计器，定义流程节点、审批策略、流转条件
- **流程实例管理**：跟踪流程执行状态，管理流程生命周期
- **任务管理**：支持待办任务、已办任务、委托代办等功能
- **审批策略**：支持或签、会签、顺序签等多种审批模式
- **任务分配**：支持按用户、角色、部门、表达式等方式分配任务

### 1.2 业务场景

- 订单审批流程
- 请假审批流程
- 报销审批流程
- 合同审批流程
- 采购审批流程

### 1.3 模块边界

Workflow 模块与其他模块的关系：
- 与 Basic 模块：使用用户、角色、部门等基础数据
- 与 Mall 模块：订单审批流程
- 与 Crm 模块：合同审批流程
- 与 Ops 模块：操作日志、定时任务

---

## 2. 技术栈和依赖

### 2.1 核心技术栈

| 类别 | 技术 | 版本 | 用途 |
|------|------|------|------|
| ORM | SqlSugarCore | 5.1.4.x | 数据访问 |
| 对象映射 | Mapster | 10.x | DTO 映射 |
| 日志 | Serilog | 8.x | 日志记录 |
| 认证 | JwtBearer | 8.x | 身份认证 |
| 定时任务 | Quartz | 3.14.x | 超时任务调度 |

### 2.2 模块依赖

```text
Workflow 模块依赖：
├─ Basic 模块：用户、角色、部门服务
├─ Common 模块：基础组件、异常处理
└─ Ops 模块：操作日志、任务日志
```

### 2.3 项目结构

```text
EasyProduct.Business/Workflow/
├─ IWorkflowDefinitionService.cs      # 流程定义服务接口
├─ WorkflowDefinitionService.cs       # 流程定义服务实现
├─ IWorkflowInstanceService.cs        # 流程实例服务接口
├─ WorkflowInstanceService.cs         # 流程实例服务实现
├─ IWorkflowTaskService.cs            # 流程任务服务接口
├─ WorkflowTaskService.cs             # 流程任务服务实现
└─ Helpers/
   └─ WorkflowHelper.cs               # 工作流工具类

EasyProduct.Models/
├─ Entitys/Workflow/
│  ├─ WfDefinition.cs                 # 流程定义实体
│  ├─ WfInstance.cs                   # 流程实例实体
│  ├─ WfTask.cs                       # 流程任务实体
│  └─ WfNode.cs                       # 流程节点实体
├─ Dto/Workflow/
│  ├─ Definition/
│  │  ├─ DefinitionQuery.cs           # 流程定义查询
│  │  ├─ DefinitionCreateDto.cs       # 流程定义创建
│  │  ├─ DefinitionUpdateDto.cs       # 流程定义更新
│  │  └─ DefinitionDto.cs             # 流程定义输出
│  ├─ Instance/
│  │  ├─ InstanceQuery.cs             # 流程实例查询
│  │  ├─ InstanceDto.cs               # 流程实例输出
│  │  └─ StartWorkflowDto.cs          # 发起流程
│  └─ Task/
│     ├─ TaskQuery.cs                 # 流程任务查询
│     ├─ TaskDto.cs                   # 流程任务输出
│     ├─ ApproveDto.cs                # 审批通过
│     ├─ RejectDto.cs                 # 审批拒绝
│     └─ DelegateDto.cs               # 委托
└─ Constants/
   └─ WorkflowConstants.cs            # 工作流常量

EasyProduct.Web/Controllers/Admin/Workflow/
├─ DefinitionController.cs            # 流程定义控制器
├─ InstanceController.cs              # 流程实例控制器
├─ TaskController.cs                  # 流程任务控制器
└─ DesignerController.cs              # 流程设计器控制器
```

---

## 3. 实体类设计

### 3.1 WfDefinition - 流程定义实体

```csharp
using SqlSugar;
using System;
using System.Collections.Generic;
using EasyProduct.Models.Entitys.Common;

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
```

### 3.2 WfNode - 流程节点实体

```csharp
namespace EasyProduct.Models.Entitys.Workflow
{
    /// <summary>
    /// 流程节点（存储在 WfDefinition.Nodes 字段中）
    /// </summary>
    public class WfNode
    {
        /// <summary>
        /// 节点ID
        /// </summary>
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// 节点名称
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 节点类型：start-开始节点、end-结束节点、task-任务节点、gateway-网关、subprocess-子流程
        /// </summary>
        public string Type { get; set; } = WorkflowConstants.NodeType.TASK;

        /// <summary>
        /// 分配类型：user-指定用户、role-角色、dept-部门、expression-表达式
        /// </summary>
        public string AssigneeType { get; set; } = string.Empty;

        /// <summary>
        /// 分配对象ID（用户ID/角色ID/部门ID）
        /// </summary>
        public string AssigneeId { get; set; } = string.Empty;

        /// <summary>
        /// 审批类型：or-或签、and-会签、sequential-顺序签
        /// </summary>
        public string ApprovalType { get; set; } = WorkflowConstants.ApprovalType.OR;

        /// <summary>
        /// 超时时间（小时）
        /// </summary>
        public int Timeout { get; set; } = 72;

        /// <summary>
        /// 节点配置（JSON 格式，存储扩展信息）
        /// </summary>
        public string? Config { get; set; }
    }
}
```

### 3.3 WfInstance - 流程实例实体

```csharp
using SqlSugar;
using System;
using EasyProduct.Models.Entitys.Common;

namespace EasyProduct.Models.Entitys.Workflow
{
    /// <summary>
    /// 流程实例实体
    /// </summary>
    [SugarTable("wf_instance", "流程实例表")]
    public class WfInstance : BaseEntity
    {
        /// <summary>
        /// 流程定义ID
        /// </summary>
        public Guid DefinitionId { get; set; }

        /// <summary>
        /// 流程定义名称（冗余字段）
        /// </summary>
        [SugarColumn(Length = 100)]
        public string DefinitionName { get; set; } = string.Empty;

        /// <summary>
        /// 业务单据ID
        /// </summary>
        [SugarColumn(Length = 50)]
        [Required(ErrorMessage = "业务单据ID不能为空")]
        public string BusinessKey { get; set; } = string.Empty;

        /// <summary>
        /// 业务类型（如：order、contract、leave）
        /// </summary>
        [SugarColumn(Length = 50)]
        [Required(ErrorMessage = "业务类型不能为空")]
        public string BusinessType { get; set; } = string.Empty;

        /// <summary>
        /// 流程标题
        /// </summary>
        [SugarColumn(Length = 200)]
        [Required(ErrorMessage = "流程标题不能为空")]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// 申请人ID
        /// </summary>
        public Guid ApplicantId { get; set; }

        /// <summary>
        /// 申请人姓名（冗余字段）
        /// </summary>
        [SugarColumn(Length = 50)]
        public string ApplicantName { get; set; } = string.Empty;

        /// <summary>
        /// 当前节点ID
        /// </summary>
        [SugarColumn(Length = 50)]
        public string CurrentNodeId { get; set; } = string.Empty;

        /// <summary>
        /// 当前节点名称（冗余字段）
        /// </summary>
        [SugarColumn(Length = 100)]
        public string CurrentNodeName { get; set; } = string.Empty;

        /// <summary>
        /// 流程状态（0-运行中、1-已完成、2-已取消、3-已拒绝）
        /// </summary>
        public int Status { get; set; } = WorkflowConstants.InstanceStatus.RUNNING;

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime StartTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndTime { get; set; }
    }
}
```

### 3.4 WfTask - 流程任务实体

```csharp
using SqlSugar;
using System;
using EasyProduct.Models.Entitys.Common;

namespace EasyProduct.Models.Entitys.Workflow
{
    /// <summary>
    /// 流程任务实体
    /// </summary>
    [SugarTable("wf_task", "流程任务表")]
    public class WfTask : BaseEntity
    {
        /// <summary>
        /// 流程实例ID
        /// </summary>
        public Guid InstanceId { get; set; }

        /// <summary>
        /// 节点ID
        /// </summary>
        [SugarColumn(Length = 50)]
        public string NodeId { get; set; } = string.Empty;

        /// <summary>
        /// 节点名称
        /// </summary>
        [SugarColumn(Length = 100)]
        public string NodeName { get; set; } = string.Empty;

        /// <summary>
        /// 受托人ID
        /// </summary>
        public Guid AssigneeId { get; set; }

        /// <summary>
        /// 受托人姓名（冗余字段）
        /// </summary>
        [SugarColumn(Length = 50)]
        public string AssigneeName { get; set; } = string.Empty;

        /// <summary>
        /// 任务状态（0-待处理、1-已批准、2-已拒绝、3-已委托、4-已取消）
        /// </summary>
        public int Status { get; set; } = WorkflowConstants.TaskStatus.PENDING;

        /// <summary>
        /// 审批意见
        /// </summary>
        [SugarColumn(ColumnDataType = "text", IsNullable = true)]
        public string? Comment { get; set; }

        /// <summary>
        /// 完成时间
        /// </summary>
        public DateTime? CompletedAt { get; set; }

        /// <summary>
        /// 委托人ID（委托任务时记录原受托人）
        /// </summary>
        public Guid? DelegatorId { get; set; }

        /// <summary>
        /// 委托人姓名
        /// </summary>
        [SugarColumn(Length = 50, IsNullable = true)]
        public string? DelegatorName { get; set; }
    }
}
```

---

## 4. DTO 设计

### 4.1 流程定义 DTO

#### DefinitionQuery - 流程定义查询

```csharp
using System.ComponentModel.DataAnnotations;

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
```

#### DefinitionCreateDto - 流程定义创建

```csharp
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EasyProduct.Models.Dto.Workflow.Definition
{
    /// <summary>
    /// 流程定义创建参数
    /// </summary>
    public class DefinitionCreateDto
    {
        /// <summary>
        /// 流程编码
        /// </summary>
        [Required(ErrorMessage = "流程编码不能为空")]
        [MaxLength(50)]
        public string Code { get; set; } = string.Empty;

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
```

#### DefinitionDto - 流程定义输出

```csharp
using System;
using System.Collections.Generic;

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
```

### 4.2 流程实例 DTO

#### StartWorkflowDto - 发起流程

```csharp
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
```

#### InstanceDto - 流程实例输出

```csharp
using System;

namespace EasyProduct.Models.Dto.Workflow.Instance
{
    /// <summary>
    /// 流程实例输出
    /// </summary>
    public class InstanceDto
    {
        /// <summary>
        /// 流程实例ID
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// 流程定义ID
        /// </summary>
        public string DefinitionId { get; set; } = string.Empty;

        /// <summary>
        /// 流程定义名称
        /// </summary>
        public string DefinitionName { get; set; } = string.Empty;

        /// <summary>
        /// 业务单据ID
        /// </summary>
        public string BusinessKey { get; set; } = string.Empty;

        /// <summary>
        /// 业务类型
        /// </summary>
        public string BusinessType { get; set; } = string.Empty;

        /// <summary>
        /// 流程标题
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// 申请人ID
        /// </summary>
        public string ApplicantId { get; set; } = string.Empty;

        /// <summary>
        /// 申请人姓名
        /// </summary>
        public string ApplicantName { get; set; } = string.Empty;

        /// <summary>
        /// 当前节点ID
        /// </summary>
        public string CurrentNodeId { get; set; } = string.Empty;

        /// <summary>
        /// 当前节点名称
        /// </summary>
        public string CurrentNodeName { get; set; } = string.Empty;

        /// <summary>
        /// 流程状态（0-运行中、1-已完成、2-已取消、3-已拒绝）
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndTime { get; set; }
    }
}
```

### 4.3 流程任务 DTO

#### ApproveDto - 审批通过

```csharp
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
```

#### RejectDto - 审批拒绝

```csharp
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
```

#### DelegateDto - 委托

```csharp
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
```

#### TaskDto - 流程任务输出

```csharp
using System;

namespace EasyProduct.Models.Dto.Workflow.Task
{
    /// <summary>
    /// 流程任务输出
    /// </summary>
    public class TaskDto
    {
        /// <summary>
        /// 任务ID
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// 流程实例ID
        /// </summary>
        public string InstanceId { get; set; } = string.Empty;

        /// <summary>
        /// 节点ID
        /// </summary>
        public string NodeId { get; set; } = string.Empty;

        /// <summary>
        /// 节点名称
        /// </summary>
        public string NodeName { get; set; } = string.Empty;

        /// <summary>
        /// 受托人ID
        /// </summary>
        public string AssigneeId { get; set; } = string.Empty;

        /// <summary>
        /// 受托人姓名
        /// </summary>
        public string AssigneeName { get; set; } = string.Empty;

        /// <summary>
        /// 任务状态（0-待处理、1-已批准、2-已拒绝、3-已委托、4-已取消）
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 审批意见
        /// </summary>
        public string? Comment { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 完成时间
        /// </summary>
        public DateTime? CompletedAt { get; set; }

        /// <summary>
        /// 流程实例信息
        /// </summary>
        public InstanceDto? Instance { get; set; }
    }
}
```

---

## 5. Service 层实现指南

### 5.1 IWorkflowDefinitionService - 流程定义服务接口

```csharp
using System;
using System.Threading.Tasks;
using EasyProduct.Common.Base;
using EasyProduct.Models.Dto.Workflow.Definition;

namespace EasyProduct.Business.Workflow
{
    /// <summary>
    /// 流程定义服务接口
    /// </summary>
    public interface IWorkflowDefinitionService
    {
        /// <summary>
        /// 获取流程定义列表（分页）
        /// </summary>
        /// <param name="query">查询参数</param>
        /// <returns>流程定义分页结果</returns>
        Task<PageResult<DefinitionDto>> GetListAsync(DefinitionQuery query);

        /// <summary>
        /// 获取流程定义详情
        /// </summary>
        /// <param name="id">流程定义ID</param>
        /// <returns>流程定义详情</returns>
        Task<DefinitionDto> GetByIdAsync(Guid id);

        /// <summary>
        /// 创建流程定义
        /// </summary>
        /// <param name="dto">创建参数</param>
        /// <returns>新流程定义ID</returns>
        Task<string> CreateAsync(DefinitionCreateDto dto);

        /// <summary>
        /// 更新流程定义
        /// </summary>
        /// <param name="id">流程定义ID</param>
        /// <param name="dto">更新参数</param>
        /// <returns>是否成功</returns>
        Task<bool> UpdateAsync(Guid id, DefinitionUpdateDto dto);

        /// <summary>
        /// 删除流程定义
        /// </summary>
        /// <param name="id">流程定义ID</param>
        /// <returns>是否成功</returns>
        Task<bool> DeleteAsync(Guid id);

        /// <summary>
        /// 发布流程定义
        /// </summary>
        /// <param name="id">流程定义ID</param>
        /// <returns>是否成功</returns>
        Task<bool> PublishAsync(Guid id);
    }
}
```

### 5.2 WorkflowDefinitionService - 流程定义服务实现

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using EasyProduct.Business.Basic;
using EasyProduct.Common.Base;
using EasyProduct.Common.Error;
using EasyProduct.Models.Dto.Workflow.Definition;
using EasyProduct.Models.Entitys.Workflow;
using Mapster;
using Microsoft.Extensions.Logging;
using SqlSugar;

namespace EasyProduct.Business.Workflow
{
    /// <summary>
    /// 流程定义服务实现
    /// </summary>
    public class WorkflowDefinitionService : BaseService, IWorkflowDefinitionService
    {
        private readonly ILogger<WorkflowDefinitionService> _logger;

        public WorkflowDefinitionService(ISqlSugarClient db, ILogger<WorkflowDefinitionService> logger) : base(db)
        {
            _logger = logger;
        }

        /// <summary>
        /// 获取流程定义列表（分页）
        /// </summary>
        /// <param name="query">查询参数</param>
        /// <returns>流程定义分页结果</returns>
        public async Task<PageResult<DefinitionDto>> GetListAsync(DefinitionQuery query)
        {
            var queryable = _db.Queryable<WfDefinition>()
                .WhereIF(!string.IsNullOrEmpty(query.Keyword), x => x.Code.Contains(query.Keyword) || x.Name.Contains(query.Keyword))
                .WhereIF(!string.IsNullOrEmpty(query.Category), x => x.Category == query.Category)
                .WhereIF(query.Status.HasValue, x => x.Status == query.Status)
                .OrderBy(x => x.CreateTime, OrderByType.Desc);

            var totalCount = 0;
            var list = await queryable.ToPageListAsync(query.PageIndex, query.PageSize, totalCount);
            totalCount = queryable.Count();

            var dtoList = list.Select(x =>
            {
                var dto = x.Adapt<DefinitionDto>();
                dto.Nodes = JsonSerializer.Deserialize<List<WfNode>>(x.Nodes) ?? new List<WfNode>();
                return dto;
            }).ToList();

            return new PageResult<DefinitionDto>
            {
                List = dtoList,
                Total = totalCount
            };
        }

        /// <summary>
        /// 获取流程定义详情
        /// </summary>
        /// <param name="id">流程定义ID</param>
        /// <returns>流程定义详情</returns>
        public async Task<DefinitionDto> GetByIdAsync(Guid id)
        {
            var entity = await _db.Queryable<WfDefinition>()
                .FirstAsync(x => x.Id == id);

            if (entity == null)
            {
                throw BusinessException.NotFound("流程定义不存在");
            }

            var dto = entity.Adapt<DefinitionDto>();
            dto.Nodes = JsonSerializer.Deserialize<List<WfNode>>(entity.Nodes) ?? new List<WfNode>();
            return dto;
        }

        /// <summary>
        /// 创建流程定义
        /// </summary>
        /// <param name="dto">创建参数</param>
        /// <returns>新流程定义ID</returns>
        public async Task<string> CreateAsync(DefinitionCreateDto dto)
        {
            // 检查编码是否重复
            var exists = await _db.Queryable<WfDefinition>()
                .AnyAsync(x => x.Code == dto.Code && !x.IsDeleted);

            if (exists)
            {
                throw BusinessException.BadRequest($"流程编码 {dto.Code} 已存在");
            }

            // 验证流程节点
            ValidateNodes(dto.Nodes);

            var entity = dto.Adapt<WfDefinition>();
            entity.Nodes = JsonSerializer.Serialize(dto.Nodes);
            entity.Status = WorkflowConstants.DefinitionStatus.DRAFT;

            await _db.Insertable(entity).ExecuteCommandAsync();

            _logger.LogInformation("创建流程定义成功，ID: {Id}, Code: {Code}", entity.Id, entity.Code);

            return entity.Id.ToString();
        }

        /// <summary>
        /// 更新流程定义
        /// </summary>
        /// <param name="id">流程定义ID</param>
        /// <param name="dto">更新参数</param>
        /// <returns>是否成功</returns>
        public async Task<bool> UpdateAsync(Guid id, DefinitionUpdateDto dto)
        {
            var entity = await _db.Queryable<WfDefinition>()
                .FirstAsync(x => x.Id == id);

            if (entity == null)
            {
                throw BusinessException.NotFound("流程定义不存在");
            }

            // 已发布的流程不允许修改
            if (entity.Status == WorkflowConstants.DefinitionStatus.PUBLISHED)
            {
                throw BusinessException.BadRequest("已发布的流程不允许修改，请新建版本");
            }

            // 验证流程节点
            ValidateNodes(dto.Nodes);

            entity.Name = dto.Name;
            entity.Category = dto.Category;
            entity.Description = dto.Description;
            entity.Nodes = JsonSerializer.Serialize(dto.Nodes);

            await _db.Updateable(entity).ExecuteCommandAsync();

            _logger.LogInformation("更新流程定义成功，ID: {Id}", id);

            return true;
        }

        /// <summary>
        /// 删除流程定义
        /// </summary>
        /// <param name="id">流程定义ID</param>
        /// <returns>是否成功</returns>
        public async Task<bool> DeleteAsync(Guid id)
        {
            var entity = await _db.Queryable<WfDefinition>()
                .FirstAsync(x => x.Id == id);

            if (entity == null)
            {
                throw BusinessException.NotFound("流程定义不存在");
            }

            // 检查是否有正在运行的流程实例
            var hasRunningInstance = await _db.Queryable<WfInstance>()
                .AnyAsync(x => x.DefinitionId == id && x.Status == WorkflowConstants.InstanceStatus.RUNNING);

            if (hasRunningInstance)
            {
                throw BusinessException.BadRequest("该流程定义下存在正在运行的流程实例，无法删除");
            }

            entity.IsDeleted = true;
            await _db.Updateable(entity).ExecuteCommandAsync();

            _logger.LogInformation("删除流程定义成功，ID: {Id}", id);

            return true;
        }

        /// <summary>
        /// 发布流程定义
        /// </summary>
        /// <param name="id">流程定义ID</param>
        /// <returns>是否成功</returns>
        public async Task<bool> PublishAsync(Guid id)
        {
            var entity = await _db.Queryable<WfDefinition>()
                .FirstAsync(x => x.Id == id);

            if (entity == null)
            {
                throw BusinessException.NotFound("流程定义不存在");
            }

            // 验证流程节点
            var nodes = JsonSerializer.Deserialize<List<WfNode>>(entity.Nodes) ?? new List<WfNode>();
            ValidateNodes(nodes);

            entity.Status = WorkflowConstants.DefinitionStatus.PUBLISHED;
            await _db.Updateable(entity).ExecuteCommandAsync();

            _logger.LogInformation("发布流程定义成功，ID: {Id}", id);

            return true;
        }

        /// <summary>
        /// 验证流程节点
        /// </summary>
        /// <param name="nodes">节点列表</param>
        private void ValidateNodes(List<WfNode> nodes)
        {
            if (nodes == null || nodes.Count == 0)
            {
                throw BusinessException.BadRequest("流程节点不能为空");
            }

            // 检查是否有开始节点
            var hasStart = nodes.Any(x => x.Type == WorkflowConstants.NodeType.START);
            if (!hasStart)
            {
                throw BusinessException.BadRequest("流程必须包含开始节点");
            }

            // 检查是否有结束节点
            var hasEnd = nodes.Any(x => x.Type == WorkflowConstants.NodeType.END);
            if (!hasEnd)
            {
                throw BusinessException.BadRequest("流程必须包含结束节点");
            }

            // 检查任务节点是否有审批人
            foreach (var node in nodes.Where(x => x.Type == WorkflowConstants.NodeType.TASK))
            {
                if (string.IsNullOrEmpty(node.AssigneeType) || string.IsNullOrEmpty(node.AssigneeId))
                {
                    throw BusinessException.BadRequest($"任务节点【{node.Name}】未设置审批人");
                }
            }
        }
    }
}
```

### 5.3 IWorkflowInstanceService - 流程实例服务接口

```csharp
using System;
using System.Threading.Tasks;
using EasyProduct.Common.Base;
using EasyProduct.Models.Dto.Workflow.Instance;

namespace EasyProduct.Business.Workflow
{
    /// <summary>
    /// 流程实例服务接口
    /// </summary>
    public interface IWorkflowInstanceService
    {
        /// <summary>
        /// 获取流程实例列表（分页）
        /// </summary>
        /// <param name="query">查询参数</param>
        /// <returns>流程实例分页结果</returns>
        Task<PageResult<InstanceDto>> GetListAsync(InstanceQuery query);

        /// <summary>
        /// 获取流程实例详情
        /// </summary>
        /// <param name="id">流程实例ID</param>
        /// <returns>流程实例详情</returns>
        Task<InstanceDto> GetByIdAsync(Guid id);

        /// <summary>
        /// 发起流程
        /// </summary>
        /// <param name="dto">发起流程参数</param>
        /// <param name="applicantId">申请人ID</param>
        /// <param name="applicantName">申请人姓名</param>
        /// <returns>流程实例ID</returns>
        Task<string> StartAsync(StartWorkflowDto dto, Guid applicantId, string applicantName);

        /// <summary>
        /// 撤销流程
        /// </summary>
        /// <param name="id">流程实例ID</param>
        /// <param name="applicantId">申请人ID</param>
        /// <returns>是否成功</returns>
        Task<bool> CancelAsync(Guid id, Guid applicantId);
    }
}
```

### 5.4 WorkflowInstanceService - 流程实例服务实现

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using EasyProduct.Business.Basic;
using EasyProduct.Common.Base;
using EasyProduct.Common.Error;
using EasyProduct.Models.Dto.Workflow.Instance;
using EasyProduct.Models.Entitys.Workflow;
using Mapster;
using Microsoft.Extensions.Logging;
using SqlSugar;

namespace EasyProduct.Business.Workflow
{
    /// <summary>
    /// 流程实例服务实现
    /// </summary>
    public class WorkflowInstanceService : BaseService, IWorkflowInstanceService
    {
        private readonly ILogger<WorkflowInstanceService> _logger;
        private readonly IWorkflowTaskService _taskService;

        public WorkflowInstanceService(
            ISqlSugarClient db,
            ILogger<WorkflowInstanceService> logger,
            IWorkflowTaskService taskService) : base(db)
        {
            _logger = logger;
            _taskService = taskService;
        }

        /// <summary>
        /// 获取流程实例列表（分页）
        /// </summary>
        /// <param name="query">查询参数</param>
        /// <returns>流程实例分页结果</returns>
        public async Task<PageResult<InstanceDto>> GetListAsync(InstanceQuery query)
        {
            var queryable = _db.Queryable<WfInstance>()
                .WhereIF(!string.IsNullOrEmpty(query.Keyword), x => x.Title.Contains(query.Keyword))
                .WhereIF(!string.IsNullOrEmpty(query.BusinessType), x => x.BusinessType == query.BusinessType)
                .WhereIF(query.Status.HasValue, x => x.Status == query.Status)
                .WhereIF(query.ApplicantId.HasValue, x => x.ApplicantId == query.ApplicantId)
                .OrderBy(x => x.StartTime, OrderByType.Desc);

            var totalCount = 0;
            var list = await queryable.ToPageListAsync(query.PageIndex, query.PageSize, totalCount);
            totalCount = queryable.Count();

            return new PageResult<InstanceDto>
            {
                List = list.Adapt<List<InstanceDto>>(),
                Total = totalCount
            };
        }

        /// <summary>
        /// 获取流程实例详情
        /// </summary>
        /// <param name="id">流程实例ID</param>
        /// <returns>流程实例详情</returns>
        public async Task<InstanceDto> GetByIdAsync(Guid id)
        {
            var entity = await _db.Queryable<WfInstance>()
                .FirstAsync(x => x.Id == id);

            if (entity == null)
            {
                throw BusinessException.NotFound("流程实例不存在");
            }

            return entity.Adapt<InstanceDto>();
        }

        /// <summary>
        /// 发起流程
        /// </summary>
        /// <param name="dto">发起流程参数</param>
        /// <param name="applicantId">申请人ID</param>
        /// <param name="applicantName">申请人姓名</param>
        /// <returns>流程实例ID</returns>
        public async Task<string> StartAsync(StartWorkflowDto dto, Guid applicantId, string applicantName)
        {
            // 获取流程定义
            var definition = await _db.Queryable<WfDefinition>()
                .FirstAsync(x => x.Id == Guid.Parse(dto.DefinitionId) && x.Status == WorkflowConstants.DefinitionStatus.PUBLISHED);

            if (definition == null)
            {
                throw BusinessException.NotFound("流程定义不存在或未发布");
            }

            // 检查业务单据是否已发起流程
            var exists = await _db.Queryable<WfInstance>()
                .AnyAsync(x => x.BusinessKey == dto.BusinessKey && x.Status == WorkflowConstants.InstanceStatus.RUNNING);

            if (exists)
            {
                throw BusinessException.BadRequest($"业务单据 {dto.BusinessKey} 已存在正在运行的流程");
            }

            // 获取流程节点
            var nodes = JsonSerializer.Deserialize<List<WfNode>>(definition.Nodes) ?? new List<WfNode>();
            var startNode = nodes.FirstOrDefault(x => x.Type == WorkflowConstants.NodeType.START);
            var firstTaskNode = nodes.FirstOrDefault(x => x.Type == WorkflowConstants.NodeType.TASK);

            if (startNode == null || firstTaskNode == null)
            {
                throw BusinessException.BadRequest("流程定义异常，缺少开始节点或任务节点");
            }

            // 创建流程实例
            var instance = new WfInstance
            {
                DefinitionId = definition.Id,
                DefinitionName = definition.Name,
                BusinessKey = dto.BusinessKey,
                BusinessType = dto.BusinessType,
                Title = dto.Title,
                ApplicantId = applicantId,
                ApplicantName = applicantName,
                CurrentNodeId = firstTaskNode.Id,
                CurrentNodeName = firstTaskNode.Name,
                Status = WorkflowConstants.InstanceStatus.RUNNING,
                StartTime = DateTime.Now
            };

            // 创建第一个任务
            var task = new WfTask
            {
                InstanceId = instance.Id,
                NodeId = firstTaskNode.Id,
                NodeName = firstTaskNode.Name,
                AssigneeId = await GetAssigneeId(firstTaskNode),
                AssigneeName = await GetAssigneeName(firstTaskNode),
                Status = WorkflowConstants.TaskStatus.PENDING
            };

            using var tran = _db.Ado.UseTranAsync();
            try
            {
                await _db.Insertable(instance).ExecuteCommandAsync();
                await _db.Insertable(task).ExecuteCommandAsync();
                tran.Commit();

                _logger.LogInformation("发起流程成功，实例ID: {InstanceId}, 流程: {DefinitionName}", instance.Id, definition.Name);

                return instance.Id.ToString();
            }
            catch (Exception ex)
            {
                tran.Rollback();
                _logger.LogError(ex, "发起流程失败，流程定义ID: {DefinitionId}", definition.Id);
                throw;
            }
        }

        /// <summary>
        /// 撤销流程
        /// </summary>
        /// <param name="id">流程实例ID</param>
        /// <param name="applicantId">申请人ID</param>
        /// <returns>是否成功</returns>
        public async Task<bool> CancelAsync(Guid id, Guid applicantId)
        {
            var instance = await _db.Queryable<WfInstance>()
                .FirstAsync(x => x.Id == id);

            if (instance == null)
            {
                throw BusinessException.NotFound("流程实例不存在");
            }

            // 只有申请人可以撤销
            if (instance.ApplicantId != applicantId)
            {
                throw BusinessException.Forbidden("只有申请人可以撤销流程");
            }

            // 只有运行中的流程可以撤销
            if (instance.Status != WorkflowConstants.InstanceStatus.RUNNING)
            {
                throw BusinessException.BadRequest("只有运行中的流程可以撤销");
            }

            instance.Status = WorkflowConstants.InstanceStatus.CANCELLED;
            instance.EndTime = DateTime.Now;

            // 取消所有待办任务
            var tasks = await _db.Queryable<WfTask>()
                .Where(x => x.InstanceId == id && x.Status == WorkflowConstants.TaskStatus.PENDING)
                .ToListAsync();

            foreach (var task in tasks)
            {
                task.Status = WorkflowConstants.TaskStatus.CANCELLED;
                task.CompletedAt = DateTime.Now;
            }

            using var tran = _db.Ado.UseTranAsync();
            try
            {
                await _db.Updateable(instance).ExecuteCommandAsync();
                await _db.Updateable(tasks).ExecuteCommandAsync();
                tran.Commit();

                _logger.LogInformation("撤销流程成功，实例ID: {InstanceId}", id);

                return true;
            }
            catch (Exception ex)
            {
                tran.Rollback();
                _logger.LogError(ex, "撤销流程失败，实例ID: {InstanceId}", id);
                throw;
            }
        }

        /// <summary>
        /// 获取审批人ID
        /// </summary>
        /// <param name="node">流程节点</param>
        /// <returns>审批人ID</returns>
        private async Task<Guid> GetAssigneeId(WfNode node)
        {
            // TODO: 根据分配类型获取审批人
            // user: 直接返回用户ID
            // role: 根据角色ID查询用户
            // dept: 根据部门ID查询部门负责人
            // expression: 解析表达式获取用户
            return Guid.Parse(node.AssigneeId);
        }

        /// <summary>
        /// 获取审批人姓名
        /// </summary>
        /// <param name="node">流程节点</param>
        /// <returns>审批人姓名</returns>
        private async Task<string> GetAssigneeName(WfNode node)
        {
            // TODO: 根据审批人ID查询用户姓名
            return node.Name;
        }
    }
}
```

### 5.5 IWorkflowTaskService - 流程任务服务接口

```csharp
using System;
using System.Threading.Tasks;
using EasyProduct.Common.Base;
using EasyProduct.Models.Dto.Workflow.Task;

namespace EasyProduct.Business.Workflow
{
    /// <summary>
    /// 流程任务服务接口
    /// </summary>
    public interface IWorkflowTaskService
    {
        /// <summary>
        /// 获取待办任务列表（分页）
        /// </summary>
        /// <param name="query">查询参数</param>
        /// <param name="assigneeId">受托人ID</param>
        /// <returns>任务分页结果</returns>
        Task<PageResult<TaskDto>> GetTodoListAsync(TaskQuery query, Guid assigneeId);

        /// <summary>
        /// 获取已办任务列表（分页）
        /// </summary>
        /// <param name="query">查询参数</param>
        /// <param name="assigneeId">受托人ID</param>
        /// <returns>任务分页结果</returns>
        Task<PageResult<TaskDto>> GetDoneListAsync(TaskQuery query, Guid assigneeId);

        /// <summary>
        /// 获取任务详情
        /// </summary>
        /// <param name="id">任务ID</param>
        /// <returns>任务详情</returns>
        Task<TaskDto> GetByIdAsync(Guid id);

        /// <summary>
        /// 审批通过
        /// </summary>
        /// <param name="id">任务ID</param>
        /// <param name="dto">审批参数</param>
        /// <param name="assigneeId">受托人ID</param>
        /// <returns>是否成功</returns>
        Task<bool> ApproveAsync(Guid id, ApproveDto dto, Guid assigneeId);

        /// <summary>
        /// 审批拒绝
        /// </summary>
        /// <param name="id">任务ID</param>
        /// <param name="dto">拒绝参数</param>
        /// <param name="assigneeId">受托人ID</param>
        /// <returns>是否成功</returns>
        Task<bool> RejectAsync(Guid id, RejectDto dto, Guid assigneeId);

        /// <summary>
        /// 委托任务
        /// </summary>
        /// <param name="id">任务ID</param>
        /// <param name="dto">委托参数</param>
        /// <param name="delegatorId">委托人ID</param>
        /// <returns>是否成功</returns>
        Task<bool> DelegateAsync(Guid id, DelegateDto dto, Guid delegatorId);
    }
}
```

---

## 6. Controller 层实现指南

### 6.1 DefinitionController - 流程定义控制器

```csharp
using System;
using System.Threading.Tasks;
using EasyProduct.Business.Workflow;
using EasyProduct.Common.Attributes;
using EasyProduct.Common.Base;
using EasyProduct.Models.Dto.Workflow.Definition;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyProduct.Web.Controllers.Admin.Workflow
{
    /// <summary>
    /// 流程定义控制器
    /// </summary>
    [ApiController]
    [Route("api/admin/workflow/definition")]
    [Authorize(AuthenticationSchemes = "AdminJwt")]
    public class DefinitionController : BaseController
    {
        private readonly IWorkflowDefinitionService _definitionService;

        public DefinitionController(IWorkflowDefinitionService definitionService)
        {
            _definitionService = definitionService;
        }

        /// <summary>
        /// 获取流程定义列表（分页）
        /// </summary>
        /// <param name="query">查询参数</param>
        /// <returns>流程定义分页结果</returns>
        [HttpGet("list")]
        [Permission("workflow:definition:view")]
        [ProducesResponseType(typeof(ApiResponse<PageResult<DefinitionDto>>), 200)]
        public async Task<IActionResult> GetList([FromQuery] DefinitionQuery query)
        {
            var result = await _definitionService.GetListAsync(query);
            return Success(result);
        }

        /// <summary>
        /// 获取流程定义详情
        /// </summary>
        /// <param name="id">流程定义ID</param>
        /// <returns>流程定义详情</returns>
        [HttpGet("{id}")]
        [Permission("workflow:definition:view")]
        [ProducesResponseType(typeof(ApiResponse<DefinitionDto>), 200)]
        public async Task<IActionResult> GetById(string id)
        {
            var result = await _definitionService.GetByIdAsync(Guid.Parse(id));
            return Success(result);
        }

        /// <summary>
        /// 创建流程定义
        /// </summary>
        /// <param name="dto">创建参数</param>
        /// <returns>新流程定义ID</returns>
        [HttpPost]
        [Permission("workflow:definition:create")]
        [OperateLog(Module = "workflow", Target = "definition", Action = "create")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        public async Task<IActionResult> Create([FromBody] DefinitionCreateDto dto)
        {
            var id = await _definitionService.CreateAsync(dto);
            return Success(id);
        }

        /// <summary>
        /// 更新流程定义
        /// </summary>
        /// <param name="id">流程定义ID</param>
        /// <param name="dto">更新参数</param>
        /// <returns>是否成功</returns>
        [HttpPut("{id}")]
        [Permission("workflow:definition:edit")]
        [OperateLog(Module = "workflow", Target = "definition", Action = "edit")]
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        public async Task<IActionResult> Update(string id, [FromBody] DefinitionUpdateDto dto)
        {
            var result = await _definitionService.UpdateAsync(Guid.Parse(id), dto);
            return Success(result);
        }

        /// <summary>
        /// 删除流程定义
        /// </summary>
        /// <param name="id">流程定义ID</param>
        /// <returns>是否成功</returns>
        [HttpDelete("{id}")]
        [Permission("workflow:definition:delete")]
        [OperateLog(Module = "workflow", Target = "definition", Action = "delete")]
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _definitionService.DeleteAsync(Guid.Parse(id));
            return Success(result);
        }

        /// <summary>
        /// 发布流程定义
        /// </summary>
        /// <param name="id">流程定义ID</param>
        /// <returns>是否成功</returns>
        [HttpPost("{id}/publish")]
        [Permission("workflow:definition:publish")]
        [OperateLog(Module = "workflow", Target = "definition", Action = "publish")]
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        public async Task<IActionResult> Publish(string id)
        {
            var result = await _definitionService.PublishAsync(Guid.Parse(id));
            return Success(result);
        }
    }
}
```

### 6.2 InstanceController - 流程实例控制器

```csharp
using System;
using System.Threading.Tasks;
using EasyProduct.Business.Workflow;
using EasyProduct.Common.Attributes;
using EasyProduct.Common.Base;
using EasyProduct.Models.Dto.Workflow.Instance;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyProduct.Web.Controllers.Admin.Workflow
{
    /// <summary>
    /// 流程实例控制器
    /// </summary>
    [ApiController]
    [Route("api/admin/workflow/instance")]
    [Authorize(AuthenticationSchemes = "AdminJwt")]
    public class InstanceController : BaseController
    {
        private readonly IWorkflowInstanceService _instanceService;

        public InstanceController(IWorkflowInstanceService instanceService)
        {
            _instanceService = instanceService;
        }

        /// <summary>
        /// 获取流程实例列表（分页）
        /// </summary>
        /// <param name="query">查询参数</param>
        /// <returns>流程实例分页结果</returns>
        [HttpGet("list")]
        [Permission("workflow:instance:view")]
        [ProducesResponseType(typeof(ApiResponse<PageResult<InstanceDto>>), 200)]
        public async Task<IActionResult> GetList([FromQuery] InstanceQuery query)
        {
            var result = await _instanceService.GetListAsync(query);
            return Success(result);
        }

        /// <summary>
        /// 获取流程实例详情
        /// </summary>
        /// <param name="id">流程实例ID</param>
        /// <returns>流程实例详情</returns>
        [HttpGet("{id}")]
        [Permission("workflow:instance:view")]
        [ProducesResponseType(typeof(ApiResponse<InstanceDto>), 200)]
        public async Task<IActionResult> GetById(string id)
        {
            var result = await _instanceService.GetByIdAsync(Guid.Parse(id));
            return Success(result);
        }

        /// <summary>
        /// 撤销流程
        /// </summary>
        /// <param name="id">流程实例ID</param>
        /// <returns>是否成功</returns>
        [HttpPost("{id}/cancel")]
        [Permission("workflow:instance:cancel")]
        [OperateLog(Module = "workflow", Target = "instance", Action = "cancel")]
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        public async Task<IActionResult> Cancel(string id)
        {
            var userId = GetCurrentUserId();
            var result = await _instanceService.CancelAsync(Guid.Parse(id), userId);
            return Success(result);
        }
    }
}
```

### 6.3 TaskController - 流程任务控制器

```csharp
using System;
using System.Threading.Tasks;
using EasyProduct.Business.Workflow;
using EasyProduct.Common.Attributes;
using EasyProduct.Common.Base;
using EasyProduct.Models.Dto.Workflow.Task;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyProduct.Web.Controllers.Admin.Workflow
{
    /// <summary>
    /// 流程任务控制器
    /// </summary>
    [ApiController]
    [Route("api/admin/workflow")]
    [Authorize(AuthenticationSchemes = "AdminJwt")]
    public class TaskController : BaseController
    {
        private readonly IWorkflowTaskService _taskService;
        private readonly IWorkflowInstanceService _instanceService;

        public TaskController(
            IWorkflowTaskService taskService,
            IWorkflowInstanceService instanceService)
        {
            _taskService = taskService;
            _instanceService = instanceService;
        }

        /// <summary>
        /// 获取我的申请列表（分页）
        /// </summary>
        /// <param name="query">查询参数</param>
        /// <returns>流程实例分页结果</returns>
        [HttpGet("my-apply/list")]
        [ProducesResponseType(typeof(ApiResponse<PageResult<InstanceDto>>), 200)]
        public async Task<IActionResult> GetMyApplyList([FromQuery] InstanceQuery query)
        {
            query.ApplicantId = GetCurrentUserId();
            var result = await _instanceService.GetListAsync(query);
            return Success(result);
        }

        /// <summary>
        /// 发起流程
        /// </summary>
        /// <param name="dto">发起流程参数</param>
        /// <returns>流程实例ID</returns>
        [HttpPost("my-apply/start")]
        [OperateLog(Module = "workflow", Target = "instance", Action = "start")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        public async Task<IActionResult> Start([FromBody] StartWorkflowDto dto)
        {
            var userId = GetCurrentUserId();
            var userName = GetCurrentRealName();
            var id = await _instanceService.StartAsync(dto, userId, userName);
            return Success(id);
        }

        /// <summary>
        /// 获取待办任务列表（分页）
        /// </summary>
        /// <param name="query">查询参数</param>
        /// <returns>任务分页结果</returns>
        [HttpGet("todo/list")]
        [ProducesResponseType(typeof(ApiResponse<PageResult<TaskDto>>), 200)]
        public async Task<IActionResult> GetTodoList([FromQuery] TaskQuery query)
        {
            var userId = GetCurrentUserId();
            var result = await _taskService.GetTodoListAsync(query, userId);
            return Success(result);
        }

        /// <summary>
        /// 获取待办任务详情
        /// </summary>
        /// <param name="id">任务ID</param>
        /// <returns>任务详情</returns>
        [HttpGet("todo/{id}")]
        [ProducesResponseType(typeof(ApiResponse<TaskDto>), 200)]
        public async Task<IActionResult> GetTodoById(string id)
        {
            var result = await _taskService.GetByIdAsync(Guid.Parse(id));
            return Success(result);
        }

        /// <summary>
        /// 审批通过
        /// </summary>
        /// <param name="id">任务ID</param>
        /// <param name="dto">审批参数</param>
        /// <returns>是否成功</returns>
        [HttpPost("todo/{id}/approve")]
        [OperateLog(Module = "workflow", Target = "task", Action = "approve")]
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        public async Task<IActionResult> Approve(string id, [FromBody] ApproveDto dto)
        {
            var userId = GetCurrentUserId();
            var result = await _taskService.ApproveAsync(Guid.Parse(id), dto, userId);
            return Success(result);
        }

        /// <summary>
        /// 审批拒绝
        /// </summary>
        /// <param name="id">任务ID</param>
        /// <param name="dto">拒绝参数</param>
        /// <returns>是否成功</returns>
        [HttpPost("todo/{id}/reject")]
        [OperateLog(Module = "workflow", Target = "task", Action = "reject")]
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        public async Task<IActionResult> Reject(string id, [FromBody] RejectDto dto)
        {
            var userId = GetCurrentUserId();
            var result = await _taskService.RejectAsync(Guid.Parse(id), dto, userId);
            return Success(result);
        }

        /// <summary>
        /// 委托任务
        /// </summary>
        /// <param name="id">任务ID</param>
        /// <param name="dto">委托参数</param>
        /// <returns>是否成功</returns>
        [HttpPost("todo/{id}/delegate")]
        [OperateLog(Module = "workflow", Target = "task", Action = "delegate")]
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        public async Task<IActionResult> Delegate(string id, [FromBody] DelegateDto dto)
        {
            var userId = GetCurrentUserId();
            var result = await _taskService.DelegateAsync(Guid.Parse(id), dto, userId);
            return Success(result);
        }

        /// <summary>
        /// 获取已办任务列表（分页）
        /// </summary>
        /// <param name="query">查询参数</param>
        /// <returns>任务分页结果</returns>
        [HttpGet("done/list")]
        [ProducesResponseType(typeof(ApiResponse<PageResult<TaskDto>>), 200)]
        public async Task<IActionResult> GetDoneList([FromQuery] TaskQuery query)
        {
            var userId = GetCurrentUserId();
            var result = await _taskService.GetDoneListAsync(query, userId);
            return Success(result);
        }
    }
}
```

---

## 7. 数据库表设计（SQL）

### 7.1 wf_definition - 流程定义表

```sql
CREATE TABLE `wf_definition` (
  `id` CHAR(36) NOT NULL COMMENT '主键ID',
  `code` VARCHAR(50) NOT NULL COMMENT '流程编码',
  `name` VARCHAR(100) NOT NULL COMMENT '流程名称',
  `version` INT NOT NULL DEFAULT 1 COMMENT '版本号',
  `category` VARCHAR(50) DEFAULT '' COMMENT '流程分类',
  `description` TEXT COMMENT '流程描述',
  `nodes` TEXT NOT NULL COMMENT '流程节点（JSON）',
  `status` INT NOT NULL DEFAULT 0 COMMENT '流程状态：0-草稿、1-已发布、2-已归档',
  `create_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `update_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
  `create_by` VARCHAR(50) COMMENT '创建人',
  `is_deleted` TINYINT(1) NOT NULL DEFAULT 0 COMMENT '是否删除',
  PRIMARY KEY (`id`),
  KEY `idx_code` (`code`),
  KEY `idx_status` (`status`),
  KEY `idx_category` (`category`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='流程定义表';
```

### 7.2 wf_instance - 流程实例表

```sql
CREATE TABLE `wf_instance` (
  `id` CHAR(36) NOT NULL COMMENT '主键ID',
  `definition_id` CHAR(36) NOT NULL COMMENT '流程定义ID',
  `definition_name` VARCHAR(100) NOT NULL COMMENT '流程定义名称',
  `business_key` VARCHAR(50) NOT NULL COMMENT '业务单据ID',
  `business_type` VARCHAR(50) NOT NULL COMMENT '业务类型',
  `title` VARCHAR(200) NOT NULL COMMENT '流程标题',
  `applicant_id` CHAR(36) NOT NULL COMMENT '申请人ID',
  `applicant_name` VARCHAR(50) NOT NULL COMMENT '申请人姓名',
  `current_node_id` VARCHAR(50) NOT NULL COMMENT '当前节点ID',
  `current_node_name` VARCHAR(100) NOT NULL COMMENT '当前节点名称',
  `status` INT NOT NULL DEFAULT 0 COMMENT '流程状态：0-运行中、1-已完成、2-已取消、3-已拒绝',
  `start_time` DATETIME NOT NULL COMMENT '开始时间',
  `end_time` DATETIME COMMENT '结束时间',
  `create_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `update_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
  `create_by` VARCHAR(50) COMMENT '创建人',
  `is_deleted` TINYINT(1) NOT NULL DEFAULT 0 COMMENT '是否删除',
  PRIMARY KEY (`id`),
  KEY `idx_definition_id` (`definition_id`),
  KEY `idx_business_key` (`business_key`),
  KEY `idx_applicant_id` (`applicant_id`),
  KEY `idx_status` (`status`),
  KEY `idx_start_time` (`start_time`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='流程实例表';
```

### 7.3 wf_task - 流程任务表

```sql
CREATE TABLE `wf_task` (
  `id` CHAR(36) NOT NULL COMMENT '主键ID',
  `instance_id` CHAR(36) NOT NULL COMMENT '流程实例ID',
  `node_id` VARCHAR(50) NOT NULL COMMENT '节点ID',
  `node_name` VARCHAR(100) NOT NULL COMMENT '节点名称',
  `assignee_id` CHAR(36) NOT NULL COMMENT '受托人ID',
  `assignee_name` VARCHAR(50) NOT NULL COMMENT '受托人姓名',
  `status` INT NOT NULL DEFAULT 0 COMMENT '任务状态：0-待办、1-已通过、2-已拒绝、3-已委托、4-已取消',
  `comment` TEXT COMMENT '审批意见',
  `create_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `update_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
  `completed_at` DATETIME COMMENT '完成时间',
  `delegator_id` CHAR(36) COMMENT '委托人ID',
  `delegator_name` VARCHAR(50) COMMENT '委托人姓名',
  `create_by` VARCHAR(50) COMMENT '创建人',
  `is_deleted` TINYINT(1) NOT NULL DEFAULT 0 COMMENT '是否删除',
  PRIMARY KEY (`id`),
  KEY `idx_instance_id` (`instance_id`),
  KEY `idx_assignee_id` (`assignee_id`),
  KEY `idx_status` (`status`),
  KEY `idx_create_time` (`create_time`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='流程任务表';
```

---

## 8. 业务逻辑要点

### 8.1 流程定义

#### 8.1.1 流程节点类型

| 节点类型 | 说明 | 必需 |
|---------|------|------|
| start | 开始节点 | 是 |
| end | 结束节点 | 是 |
| task | 任务节点（审批节点） | 可选 |
| gateway | 网关节点（条件分支） | 可选 |
| subprocess | 子流程节点 | 可选 |

#### 8.1.2 流程状态

| 状态值 | 常量 | 说明 |
|--------|------|------|
| 0 | DRAFT | 草稿状态，可以编辑 |
| 1 | PUBLISHED | 已发布，可以发起流程 |
| 2 | ARCHIVED | 已归档，不可发起流程 |

#### 8.1.3 流程实例状态

| 状态值 | 常量 | 说明 |
|--------|------|------|
| 0 | RUNNING | 运行中 |
| 1 | COMPLETED | 已完成 |
| 2 | CANCELLED | 已取消 |
| 3 | REJECTED | 已拒绝 |

#### 8.1.4 任务状态

| 状态值 | 常量 | 说明 |
|--------|------|------|
| 0 | PENDING | 待处理 |
| 1 | APPROVED | 已批准 |
| 2 | REJECTED | 已拒绝 |
| 3 | DELEGATED | 已委托 |
| 4 | CANCELLED | 已取消 |

#### 8.1.5 流程定义规则

1. 每个流程必须有且仅有一个开始节点和一个结束节点
2. 任务节点必须设置审批人
3. 已发布的流程不允许修改，需要新建版本
4. 删除流程定义前需要检查是否有正在运行的流程实例

### 8.2 审批策略

#### 8.2.1 审批类型

| 审批类型 | 说明 | 使用场景 |
|---------|------|---------|
| or | 或签 | 任意一人审批通过即可 |
| and | 会签 | 所有人都必须审批通过 |
| sequential | 顺序签 | 按顺序依次审批 |

#### 8.2.2 或签逻辑

```csharp
/// <summary>
/// 或签：任意一人审批通过即可
/// </summary>
public async Task<bool> OrApproveAsync(WfTask task, WfInstance instance)
{
    // 1. 更新当前任务状态为已通过
    task.Status = WorkflowConstants.TaskStatus.APPROVED;
    task.CompletedAt = DateTime.Now;
    await _db.Updateable(task).ExecuteCommandAsync();

    // 2. 取消其他待办任务
    var otherTasks = await _db.Queryable<WfTask>()
        .Where(x => x.InstanceId == instance.Id
            && x.NodeId == task.NodeId
            && x.Status == WorkflowConstants.TaskStatus.PENDING)
        .ToListAsync();

    foreach (var otherTask in otherTasks)
    {
        otherTask.Status = WorkflowConstants.TaskStatus.CANCELLED;
        otherTask.CompletedAt = DateTime.Now;
        await _db.Updateable(otherTask).ExecuteCommandAsync();
    }

    // 3. 流转到下一节点
    await MoveToNextNodeAsync(instance);

    return true;
}
```

#### 8.2.3 会签逻辑

```csharp
/// <summary>
/// 会签：所有人都必须审批通过
/// </summary>
public async Task<bool> AndApproveAsync(WfTask task, WfInstance instance)
{
    // 1. 更新当前任务状态为已通过
    task.Status = WorkflowConstants.TaskStatus.APPROVED;
    task.CompletedAt = DateTime.Now;
    await _db.Updateable(task).ExecuteCommandAsync();

    // 2. 检查是否所有任务都已通过
    var allTasks = await _db.Queryable<WfTask>()
        .Where(x => x.InstanceId == instance.Id && x.NodeId == task.NodeId)
        .ToListAsync();

    var allApproved = allTasks.All(x => x.Status == WorkflowConstants.TaskStatus.APPROVED);

    if (allApproved)
    {
        // 所有人都已通过，流转到下一节点
        await MoveToNextNodeAsync(instance);
    }

    return true;
}
```

#### 8.2.4 顺序签逻辑

```csharp
/// <summary>
/// 顺序签：按顺序依次审批
/// </summary>
public async Task<bool> SequentialApproveAsync(WfTask task, WfInstance instance)
{
    // 1. 更新当前任务状态为已通过
    task.Status = WorkflowConstants.TaskStatus.APPROVED;
    task.CompletedAt = DateTime.Now;
    await _db.Updateable(task).ExecuteCommandAsync();

    // 2. 查找下一个审批人
    var nextAssignee = await GetNextSequentialAssigneeAsync(instance, task.NodeId);

    if (nextAssignee != null)
    {
        // 创建下一个任务
        var nextTask = new WfTask
        {
            InstanceId = instance.Id,
            NodeId = task.NodeId,
            NodeName = task.NodeName,
            AssigneeId = nextAssignee.Id,
            AssigneeName = nextAssignee.Name,
            Status = WorkflowConstants.TaskStatus.PENDING
        };
        await _db.Insertable(nextTask).ExecuteCommandAsync();
    }
    else
    {
        // 所有审批人都已审批，流转到下一节点
        await MoveToNextNodeAsync(instance);
    }

    return true;
}
```

### 8.3 任务分配

#### 8.3.1 分配类型

| 分配类型 | 说明 | 使用场景 |
|---------|------|---------|
| user | 指定用户 | 直接指定审批人 |
| role | 角色 | 由指定角色的用户审批 |
| dept | 部门 | 由部门负责人审批 |
| expression | 表达式 | 根据表达式动态计算审批人 |

#### 8.3.2 用户分配

```csharp
/// <summary>
/// 用户分配：直接返回指定的用户ID
/// </summary>
public async Task<Guid> GetUserAssigneeIdAsync(string assigneeId)
{
    // 验证用户是否存在
    var user = await _userService.GetByIdAsync(Guid.Parse(assigneeId));
    if (user == null)
    {
        throw BusinessException.NotFound("指定的审批用户不存在");
    }
    return Guid.Parse(assigneeId);
}
```

#### 8.3.3 角色分配

```csharp
/// <summary>
/// 角色分配：从指定角色的用户中选择一个
/// </summary>
public async Task<Guid> GetRoleAssigneeIdAsync(string roleId)
{
    // 获取该角色的所有用户
    var users = await _userService.GetUsersByRoleIdAsync(Guid.Parse(roleId));
    if (users == null || users.Count == 0)
    {
        throw BusinessException.NotFound("该角色下没有用户");
    }

    // 简单策略：选择第一个用户
    // TODO: 可以根据负载均衡、轮询等策略选择用户
    return users.First().Id;
}
```

#### 8.3.4 部门分配

```csharp
/// <summary>
/// 部门分配：获取部门负责人
/// </summary>
public async Task<Guid> GetDeptAssigneeIdAsync(string deptId)
{
    // 获取部门信息
    var dept = await _deptService.GetByIdAsync(Guid.Parse(deptId));
    if (dept == null)
    {
        throw BusinessException.NotFound("指定的部门不存在");
    }

    // 获取部门负责人
    if (dept.ManagerId.HasValue)
    {
        return dept.ManagerId.Value;
    }

    throw BusinessException.NotFound("该部门没有设置负责人");
}
```

#### 8.3.5 表达式分配

```csharp
/// <summary>
/// 表达式分配：根据表达式动态计算审批人
/// 支持的表达式：
/// - ${applicant.manager} - 申请人的直属领导
/// - ${applicant.dept.manager} - 申请人部门的负责人
/// - ${business.owner} - 业务单据的负责人
/// </summary>
public async Task<Guid> GetExpressionAssigneeIdAsync(string expression, WfInstance instance)
{
    if (expression == "${applicant.manager}")
    {
        // 获取申请人的直属领导
        var applicant = await _userService.GetByIdAsync(instance.ApplicantId);
        if (applicant?.ManagerId != null)
        {
            return applicant.ManagerId.Value;
        }
    }
    else if (expression == "${applicant.dept.manager}")
    {
        // 获取申请人部门的负责人
        var applicant = await _userService.GetByIdAsync(instance.ApplicantId);
        if (applicant?.DeptId != null)
        {
            var dept = await _deptService.GetByIdAsync(applicant.DeptId.Value);
            if (dept?.ManagerId != null)
            {
                return dept.ManagerId.Value;
            }
        }
    }
    else if (expression == "${business.owner}")
    {
        // 获取业务单据的负责人（需要根据业务类型实现）
        // TODO: 根据业务类型获取业务单据负责人
    }

    throw BusinessException.BadRequest($"无法解析表达式: {expression}");
}
```

### 8.4 流程流转

#### 8.4.1 流转到下一节点

```csharp
/// <summary>
/// 流转到下一节点
/// </summary>
public async Task MoveToNextNodeAsync(WfInstance instance)
{
    // 获取流程定义
    var definition = await _db.Queryable<WfDefinition>()
        .FirstAsync(x => x.Id == instance.DefinitionId);

    var nodes = JsonSerializer.Deserialize<List<WfNode>>(definition.Nodes) ?? new List<WfNode>();

    // 查找当前节点的下一个节点
    var currentNodeIndex = nodes.FindIndex(x => x.Id == instance.CurrentNodeId);
    var nextNode = nodes.ElementAtOrDefault(currentNodeIndex + 1);

    if (nextNode == null)
    {
        throw BusinessException.BadRequest("流程定义异常，找不到下一节点");
    }

    // 如果是结束节点，结束流程
    if (nextNode.Type == WorkflowConstants.NodeType.END)
    {
        instance.Status = WorkflowConstants.InstanceStatus.COMPLETED;
        instance.EndTime = DateTime.Now;
        instance.CurrentNodeId = nextNode.Id;
        instance.CurrentNodeName = nextNode.Name;
        await _db.Updateable(instance).ExecuteCommandAsync();

        _logger.LogInformation("流程结束，实例ID: {InstanceId}", instance.Id);
        return;
    }

    // 如果是任务节点，创建任务
    if (nextNode.Type == WorkflowConstants.NodeType.TASK)
    {
        var task = new WfTask
        {
            InstanceId = instance.Id,
            NodeId = nextNode.Id,
            NodeName = nextNode.Name,
            AssigneeId = await GetAssigneeId(nextNode),
            AssigneeName = await GetAssigneeName(nextNode),
            Status = WorkflowConstants.TaskStatus.PENDING
        };

        instance.CurrentNodeId = nextNode.Id;
        instance.CurrentNodeName = nextNode.Name;

        using var tran = _db.Ado.UseTranAsync();
        try
        {
            await _db.Insertable(task).ExecuteCommandAsync();
            await _db.Updateable(instance).ExecuteCommandAsync();
            tran.Commit();

            _logger.LogInformation("流转到下一节点，实例ID: {InstanceId}, 节点: {NodeName}", instance.Id, nextNode.Name);
        }
        catch (Exception ex)
        {
            tran.Rollback();
            _logger.LogError(ex, "流转失败，实例ID: {InstanceId}", instance.Id);
            throw;
        }
    }
}
```

### 8.5 超时处理

#### 8.5.1 超时任务检查

```csharp
using System;
using System.Threading.Tasks;
using EasyProduct.Business.Workflow;
using Microsoft.Extensions.Logging;
using Quartz;

namespace EasyProduct.Business.Ops.Jobs
{
    /// <summary>
    /// 工作流超时任务检查定时任务
    /// </summary>
    public class WorkflowTimeoutCheckJob : IJob
    {
        private readonly ILogger<WorkflowTimeoutCheckJob> _logger;
        private readonly IWorkflowTaskService _taskService;

        public WorkflowTimeoutCheckJob(
            ILogger<WorkflowTimeoutCheckJob> logger,
            IWorkflowTaskService taskService)
        {
            _logger = logger;
            _taskService = taskService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("开始执行工作流超时检查任务");

            try
            {
                // 检查所有超时的任务
                var timeoutTasks = await _taskService.GetTimeoutTasksAsync();

                foreach (var task in timeoutTasks)
                {
                    // 发送通知
                    await SendTimeoutNotificationAsync(task);

                    // 记录日志
                    _logger.LogWarning("任务超时，任务ID: {TaskId}, 节点: {NodeName}", task.Id, task.NodeName);
                }

                _logger.LogInformation("工作流超时检查任务执行完成");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "工作流超时检查任务执行失败");
            }
        }

        /// <summary>
        /// 发送超时通知
        /// </summary>
        private async Task SendTimeoutNotificationAsync(WfTask task)
        {
            // TODO: 实现通知逻辑（邮件、短信、站内信）
            await Task.CompletedTask;
        }
    }
}
```

---

## 9. 单元测试要求

### 9.1 测试范围

**强制测试范围**：
- 流程定义创建、更新、删除
- 流程实例创建、撤销
- 任务审批通过、拒绝、委托
- 审批策略（或签、会签、顺序签）
- 任务分配（用户、角色、部门、表达式）

### 9.2 测试用例

#### 9.2.1 流程定义测试

```csharp
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EasyProduct.Business.Workflow;
using EasyProduct.Models.Dto.Workflow.Definition;
using EasyProduct.Models.Entitys.Workflow;
using Xunit;

namespace EasyProduct.Tests.Workflow
{
    /// <summary>
    /// 流程定义服务测试
    /// </summary>
    public class WorkflowDefinitionServiceTests
    {
        private readonly IWorkflowDefinitionService _service;

        public WorkflowDefinitionServiceTests()
        {
            // TODO: 初始化测试环境和依赖
        }

        /// <summary>
        /// 测试创建流程定义
        /// </summary>
        [Fact]
        public async Task Create_ValidDefinition_ReturnsId()
        {
            // Arrange
            var dto = new DefinitionCreateDto
            {
                Code = "TEST_001",
                Name = "测试流程",
                Category = "test",
                Description = "测试流程描述",
                Nodes = new List<WfNode>
                {
                    new WfNode { Id = "node_1", Name = "开始", Type = "start" },
                    new WfNode { Id = "node_2", Name = "审批", Type = "task", AssigneeType = "user", AssigneeId = "user_001" },
                    new WfNode { Id = "node_3", Name = "结束", Type = "end" }
                }
            };

            // Act
            var id = await _service.CreateAsync(dto);

            // Assert
            Assert.NotNull(id);
            Assert.NotEmpty(id);
        }

        /// <summary>
        /// 测试创建流程定义 - 缺少开始节点
        /// </summary>
        [Fact]
        public async Task Create_MissingStartNode_ThrowsException()
        {
            // Arrange
            var dto = new DefinitionCreateDto
            {
                Code = "TEST_002",
                Name = "测试流程",
                Nodes = new List<WfNode>
                {
                    new WfNode { Id = "node_1", Name = "审批", Type = "task", AssigneeType = "user", AssigneeId = "user_001" },
                    new WfNode { Id = "node_2", Name = "结束", Type = "end" }
                }
            };

            // Act & Assert
            await Assert.ThrowsAsync<BusinessException>(() => _service.CreateAsync(dto));
        }

        /// <summary>
        /// 测试发布流程定义
        /// </summary>
        [Fact]
        public async Task Publish_ValidDefinition_ReturnsTrue()
        {
            // Arrange
            var dto = new DefinitionCreateDto
            {
                Code = "TEST_003",
                Name = "测试流程",
                Nodes = new List<WfNode>
                {
                    new WfNode { Id = "node_1", Name = "开始", Type = "start" },
                    new WfNode { Id = "node_2", Name = "审批", Type = "task", AssigneeType = "user", AssigneeId = "user_001" },
                    new WfNode { Id = "node_3", Name = "结束", Type = "end" }
                }
            };
            var id = Guid.Parse(await _service.CreateAsync(dto));

            // Act
            var result = await _service.PublishAsync(id);

            // Assert
            Assert.True(result);
        }
    }
}
```

#### 9.2.2 流程实例测试

```csharp
using System;
using System.Threading.Tasks;
using EasyProduct.Business.Workflow;
using EasyProduct.Models.Dto.Workflow.Instance;
using Xunit;

namespace EasyProduct.Tests.Workflow
{
    /// <summary>
    /// 流程实例服务测试
    /// </summary>
    public class WorkflowInstanceServiceTests
    {
        private readonly IWorkflowInstanceService _service;
        private readonly IWorkflowDefinitionService _definitionService;

        public WorkflowInstanceServiceTests()
        {
            // TODO: 初始化测试环境和依赖
        }

        /// <summary>
        /// 测试发起流程
        /// </summary>
        [Fact]
        public async Task Start_ValidRequest_ReturnsId()
        {
            // Arrange
            var definitionId = await CreateTestDefinitionAsync();
            var dto = new StartWorkflowDto
            {
                DefinitionId = definitionId,
                BusinessKey = "BIZ_001",
                BusinessType = "test",
                Title = "测试流程申请"
            };
            var applicantId = Guid.NewGuid();
            var applicantName = "测试用户";

            // Act
            var id = await _service.StartAsync(dto, applicantId, applicantName);

            // Assert
            Assert.NotNull(id);
            Assert.NotEmpty(id);
        }

        /// <summary>
        /// 测试撤销流程
        /// </summary>
        [Fact]
        public async Task Cancel_RunningInstance_ReturnsTrue()
        {
            // Arrange
            var definitionId = await CreateTestDefinitionAsync();
            var applicantId = Guid.NewGuid();
            var dto = new StartWorkflowDto
            {
                DefinitionId = definitionId,
                BusinessKey = "BIZ_002",
                BusinessType = "test",
                Title = "测试流程申请"
            };
            var instanceId = Guid.Parse(await _service.StartAsync(dto, applicantId, "测试用户"));

            // Act
            var result = await _service.CancelAsync(instanceId, applicantId);

            // Assert
            Assert.True(result);
        }

        private async Task<string> CreateTestDefinitionAsync()
        {
            // TODO: 创建测试流程定义
            return Guid.NewGuid().ToString();
        }
    }
}
```

#### 9.2.3 审批策略测试

```csharp
using System;
using System.Threading.Tasks;
using Xunit;

namespace EasyProduct.Tests.Workflow
{
    /// <summary>
    /// 审批策略测试
    /// </summary>
    public class ApprovalStrategyTests
    {
        /// <summary>
        /// 测试或签 - 任意一人通过
        /// </summary>
        [Fact]
        public async Task OrApproval_AnyOneApprove_MovesToNext()
        {
            // Arrange: 创建3个审批人的或签任务

            // Act: 第一个审批人通过

            // Assert: 流程流转到下一节点，其他任务被取消
        }

        /// <summary>
        /// 测试会签 - 所有人都通过
        /// </summary>
        [Fact]
        public async Task AndApproval_AllApprove_MovesToNext()
        {
            // Arrange: 创建3个审批人的会签任务

            // Act: 第一个审批人通过

            // Assert: 流程不流转

            // Act: 所有审批人都通过

            // Assert: 流程流转到下一节点
        }

        /// <summary>
        /// 测试顺序签 - 按顺序审批
        /// </summary>
        [Fact]
        public async Task SequentialApproval_InOrder_MovesToNext()
        {
            // Arrange: 创建顺序签任务，审批人A、B、C

            // Act: 审批人A通过

            // Assert: 创建审批人B的任务

            // Act: 审批人B通过

            // Assert: 创建审批人C的任务

            // Act: 审批人C通过

            // Assert: 流程流转到下一节点
        }
    }
}
```

### 9.3 测试覆盖率要求

- 代码覆盖率：不低于 80%
- 分支覆盖率：不低于 75%
- 方法覆盖率：不低于 90%

---

## 10. 开发检查清单

### 10.1 实体类检查

- [ ] 所有实体类继承 `BaseEntity`
- [ ] 使用 `[SugarTable]` 标注表名和中文描述
- [ ] 所有字段添加中文注释
- [ ] 状态字段使用 int 类型常量
- [ ] 时间字段使用 `DateTime` 类型

### 10.2 DTO 检查

- [ ] 入参 DTO 使用 DataAnnotations 校验
- [ ] 出参 DTO 不包含敏感字段
- [ ] 分页 DTO 继承 `PageQuery`
- [ ] 创建/更新 DTO 分离

### 10.3 Service 层检查

- [ ] 所有方法添加中文注释
- [ ] 使用构造器注入依赖
- [ ] 业务异常使用 `BusinessException`
- [ ] 涉及多表操作使用事务
- [ ] 记录关键操作日志

### 10.4 Controller 层检查

- [ ] 所有方法添加 XML 注释
- [ ] 使用 `[Permission]` 标注权限
- [ ] 使用 `[OperateLog]` 标注操作日志
- [ ] 使用 `[ProducesResponseType]` 标注响应类型
- [ ] 不包含业务逻辑

### 10.5 数据库检查

- [ ] 表名使用 `wf_` 前缀
- [ ] 列名使用 snake_case
- [ ] 添加必要的索引
- [ ] 外键约束正确

### 10.6 测试检查

- [ ] 涉钱逻辑强制测试
- [ ] 审批策略完整测试
- [ ] 异常场景测试
- [ ] 测试覆盖率达标

### 10.7 文档检查

- [ ] Swagger 注释完整
- [ ] 接口文档归档
- [ ] 数据库脚本归档
- [ ] 业务逻辑文档完整

### 10.8 代码质量检查

- [ ] `dotnet build` 无错误无警告
- [ ] 代码符合规范
- [ ] 无硬编码中文
- [ ] 无魔法值

---

## 附录

### A. WorkflowConstants - 工作流常量

```csharp
namespace EasyProduct.Models.Constants
{
    /// <summary>
    /// 工作流常量
    /// </summary>
    public static class WorkflowConstants
    {
        /// <summary>
        /// 流程定义状态
        /// </summary>
        public static class DefinitionStatus
        {
            /// <summary>
            /// 草稿
            /// </summary>
            public const int DRAFT = 0;

            /// <summary>
            /// 已发布
            /// </summary>
            public const int PUBLISHED = 1;

            /// <summary>
            /// 已归档
            /// </summary>
            public const int ARCHIVED = 2;
        }

        /// <summary>
        /// 流程实例状态
        /// </summary>
        public static class InstanceStatus
        {
            /// <summary>
            /// 运行中
            /// </summary>
            public const int RUNNING = 0;

            /// <summary>
            /// 已完成
            /// </summary>
            public const int COMPLETED = 1;

            /// <summary>
            /// 已取消
            /// </summary>
            public const int CANCELLED = 2;

            /// <summary>
            /// 已拒绝
            /// </summary>
            public const int REJECTED = 3;
        }

        /// <summary>
        /// 任务状态
        /// </summary>
        public static class TaskStatus
        {
            /// <summary>
            /// 待处理
            /// </summary>
            public const int PENDING = 0;

            /// <summary>
            /// 已批准
            /// </summary>
            public const int APPROVED = 1;

            /// <summary>
            /// 已拒绝
            /// </summary>
            public const int REJECTED = 2;

            /// <summary>
            /// 已委托
            /// </summary>
            public const int DELEGATED = 3;

            /// <summary>
            /// 已取消
            /// </summary>
            public const int CANCELLED = 4;
        }

        /// <summary>
        /// 节点类型
        /// </summary>
        public static class NodeType
        {
            /// <summary>
            /// 开始节点
            /// </summary>
            public const string START = "start";

            /// <summary>
            /// 结束节点
            /// </summary>
            public const string END = "end";

            /// <summary>
            /// 任务节点
            /// </summary>
            public const string TASK = "task";

            /// <summary>
            /// 网关节点
            /// </summary>
            public const string GATEWAY = "gateway";

            /// <summary>
            /// 子流程节点
            /// </summary>
            public const string SUBPROCESS = "subprocess";
        }

        /// <summary>
        /// 审批类型
        /// </summary>
        public static class ApprovalType
        {
            /// <summary>
            /// 或签
            /// </summary>
            public const string OR = "or";

            /// <summary>
            /// 会签
            /// </summary>
            public const string AND = "and";

            /// <summary>
            /// 顺序签
            /// </summary>
            public const string SEQUENTIAL = "sequential";
        }

        /// <summary>
        /// 分配类型
        /// </summary>
        public static class AssigneeType
        {
            /// <summary>
            /// 用户
            /// </summary>
            public const string USER = "user";

            /// <summary>
            /// 角色
            /// </summary>
            public const string ROLE = "role";

            /// <summary>
            /// 部门
            /// </summary>
            public const string DEPT = "dept";

            /// <summary>
            /// 表达式
            /// </summary>
            public const string EXPRESSION = "expression";
        }
    }
}
```

### B. 参考文档

- 后端开发规范：`docs/backend-guidelines.md`
- API 接口定义：`docs/api/other-modules.md`
- 整合设计方案：`docs/superpowers/specs/2026-08-08-easyproduct-integration-design.md`

---

> 文档版本：v1.1
> 最后更新：2026-09-07
> 维护者：EasyProduct 开发团队
> 更新说明：应用新的设计规范（实体继承 BaseEntity，状态字段使用 int 类型）