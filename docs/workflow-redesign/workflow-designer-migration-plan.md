# EasyProduct.Admin 工作流设计器升级方案

> 基于 EasyProject PCWeb ant_workflow 架构重构文档

## 📋 文档概述

本文档对比 EasyProduct.Admin（当前原生实现）与 EasyProject PCWeb ant_workflow（AntV X6 专业实现）两个工作流设计器架构，提供从原生实现迁移到 AntV X6 专业图引擎的详细方案。

- **当前实现**: EasyProduct.Admin (原生 SVG + DOM)
- **参考实现**: EasyProject PCWeb (AntV X6 专业引擎)
- **目标**: 升级工作流设计器至企业级标准

---

## 1️⃣ 差异对比表

### 1.1 核心技术栈对比

| 维度 | 当前实现 (EasyProduct) | 参考实现 (Ant Workflow) | 影响评估 |
|------|------------------------|------------------------|----------|
| **引擎** | 原生 SVG + DOM 手动绘制 | AntV X6 专业图引擎 | 🔴 核心差异 |
| **渲染性能** | 节点多时卡顿 | 大规模图流畅渲染 | 🔴 性能差距大 |
| **交互能力** | 基础拖拽/连线 | 端口连接、磁吸吸附、缩放平移 | 🟡 体验差距 |
| **节点类型** | 5种 (start/approval/condition/cc/end) | 11种完整类型 | 🔴 功能缺失严重 |
| **撤销重做** | ❌ 无 | ✅ 历史栈管理 | 🟡 必要功能 |
| **自动布局** | ❌ 无 | ✅ dagre 自动布局 | 🟡 专业需求 |
| **缩放控制** | ❌ 无 | ✅ 0.2x-4x 缩放 | 🟡 大型流程必备 |

### 1.2 节点类型对比

| 节点类型 | 当前实现 | Ant Workflow | 状态 |
|----------|----------|--------------|------|
| 开始节点 | ✅ start | ✅ START | ✅ 可复用 |
| 审批节点 | ✅ approval | ✅ APPROVER | ⚠️ 需增强 |
| 抄送节点 | ✅ cc | ✅ COPYER | ✅ 可复用 |
| 条件分支 | ✅ condition | ✅ CONDITION | ⚠️ 需重构 |
| 结束节点 | ✅ end | ✅ END | ⚠️ 需增强 |
| 并行分支 | ❌ 无 | ✅ PARALLEL | 🔴 缺失 |
| 服务任务 | ❌ 无 | ✅ SERVICE | 🔴 缺失 |
| 通知节点 | ❌ 无 | ✅ NOTIFICATION | 🔴 缺失 |
| Webhook | ❌ 无 | ✅ WEBHOOK | 🔴 缺失 |
| 子流程 | ❌ 无 | ✅ SUBFLOW | 🔴 缺失 |
| 会签节点 | ❌ 无 | ✅ COUNTER_SIGN | 🔴 缺失 |

### 1.3 属性面板对比

| 功能 | 当前实现 | Ant Workflow | 备注 |
|------|----------|--------------|------|
| **基础信息** | 名称、类型、位置 | 名称、类型、描述 | 基础功能对齐 |
| **审批人配置** | 简单 assigneeType + assigneeName | 完整审批策略设置 | 🔴 需大幅增强 |
| **条件规则** | ❌ 无可视化配置 | ✅ 条件规则编辑器 | 🔴 缺失核心功能 |
| **表单字段** | ❌ 无 | ✅ 表单字段配置 | 🟡 增强功能 |
| **超时设置** | ❌ 无 | ✅ 超时时间+动作 | 🟡 增强功能 |
| **通知配置** | ❌ 无 | ✅ 多通道通知 | 🟡 增强功能 |
| **Webhook 配置** | ❌ 无 | ✅ URL/方法/参数 | 🟡 增强功能 |

### 1.4 画布功能对比

| 功能 | 当前实现 | Ant Workflow | 重要性 |
|------|----------|--------------|--------|
| **网格背景** | ✅ 简单点阵 | ✅ 可配置网格 | 基础 |
| **端口连接** | ❌ 无，直接连节点 | ✅ 精确端口连接 | 🔴 关键 |
| **磁吸吸附** | ❌ 无 | ✅ 节点对齐吸附 | 🟡 体验 |
| **画布平移** | ❌ 无 | ✅ 鼠标拖拽平移 | 🔴 必需 |
| **画布缩放** | ❌ 无 | ✅ Ctrl+滚轮缩放 | 🟡 大型流程 |
| **自动布局** | ❌ 无 | ✅ dagre 算法 | 🟡 专业功能 |
| **连线标签** | ❌ 无 | ✅ 条件标签显示 | 🔴 关键 |

### 1.5 文件架构对比

```
当前 (EasyProduct)                          目标 (Ant Workflow)
─────────────────────────────────           ─────────────────────────────────
src/views/workflow/                        src/components/ant_workflow/
├── designer/                              ├── AntDagDesigner.vue          [主容器]
│   └── index.vue   [单文件 5000+行]       ├── Canvas.vue                   [X6画布]
├── publish/                               ├── NodePalette.vue              [节点库]
│   └── index.vue                          ├── Toolbar.vue                  [工具栏]
├── definition/                            ├── PropertyPanel.vue            [属性面板]
│   └── index.vue                          ├── nodes/                      [节点配置]
├── instance/                              │   ├── StartNodeConfig.vue
│   └── index.vue                          │   ├── ApproverNodeConfig.vue
├── task/                                  │   ├── ConditionNodeConfig.vue
│   └── index.vue                          │   └── ... (11个组件)
└── list/                                  ├── common/                     [公共组件]
    └── index.vue                          │   ├── ConditionRuleEditor.vue
                                           │   └── ...
src/types/workflow.ts                      ├── utils/                      [工具函数]
    [300行]                                │   ├── graphConfig.ts
                                           │   └── nodeRegistry.ts
                                           └── mock/
                                           
src/types/antWorkflow/                     [700+行完整类型定义]
├── nodeTypes.ts
├── workflow.ts
└── ...
```

---

## 2️⃣ 架构升级建议

### 2.1 整体架构图

```
┌─────────────────────────────────────────────────────────────┐
│                    AntDagDesigner.vue                        │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐        │
│  │   Toolbar    │  │    Title     │  │   Actions    │        │
│  │ 撤销|重做|布局 │  │  流程名称    │  │ 保存|发布|预览│       │
│  └──────────────┘  └──────────────┘  └──────────────┘        │
├──────────────┬───────────────────────────┬──────────────────┤
│              │                           │                  │
│  ┌────────┐  │   ┌─────────────────┐     │   ┌───────────┐  │
│  │Node    │  │   │                 │     │   │Property   │  │
│  │Palette │  │   │  X6 Canvas      │     │   │Panel      │  │
│  │        │  │   │                 │     │   │           │  │
│  │ 节点库  │  │   │  专业画布引擎   │◄────┼──►│ 属性面板   │  │
│  │ 可折叠  │  │   │                 │     │   │           │  │
│  └────────┘  │   └─────────────────┘     │   └───────────┘  │
│              │                           │                  │
└──────────────┴───────────────────────────┴──────────────────┘
```

### 2.2 组件职责划分

| 组件 | 职责 | 当前状态 |
|------|------|----------|
| **AntDagDesigner** | 主容器，协调三栏布局，历史记录管理 | 🔴 需新建 |
| **Canvas** | X6 图实例封装，画布交互处理 | 🔴 需新建 |
| **NodePalette** | 左侧节点库，支持搜索和拖拽 | 🔴 需新建 |
| **Toolbar** | 顶部工具栏，撤销重做自动布局 | 🔴 需新建 |
| **PropertyPanel** | 右侧属性面板，动态节点配置 | 🔴 需新建 |
| **Node Configs** | 11种节点类型的独立配置组件 | 🔴 需新建 |
| **Utils** | graphConfig + nodeRegistry | 🔴 需新建 |

---

## 3️⃣ 具体文件修改清单

### 3.1 目录结构新建

```
src/components/workflow/
├── AntDagDesigner.vue          [NEW] 主设计器组件
├── Canvas.vue                   [NEW] X6画布组件
├── NodePalette.vue              [NEW] 节点库组件
├── Toolbar.vue                  [NEW] 工具栏组件
├── PropertyPanel.vue            [NEW] 属性面板
├── nodes/                       [NEW] 节点配置组件
│   ├── index.ts                 [NEW] 组件导出
│   ├── StartNodeConfig.vue      [NEW] 开始节点配置
│   ├── ApproverNodeConfig.vue   [NEW] 审批节点配置
│   ├── CopyerNodeConfig.vue     [NEW] 抄送节点配置
│   ├── ConditionNodeConfig.vue  [NEW] 条件节点配置
│   ├── ParallelNodeConfig.vue   [NEW] 并行节点配置
│   ├── ServiceNodeConfig.vue    [NEW] 服务任务配置
│   ├── NotificationNodeConfig.vue [NEW] 通知节点配置
│   ├── WebhookNodeConfig.vue    [NEW] Webhook配置
│   ├── SubflowNodeConfig.vue    [NEW] 子流程配置
│   ├── CounterSignNodeConfig.vue [NEW] 会签节点配置
│   └── EndNodeConfig.vue        [NEW] 结束节点配置
├── common/                      [NEW] 公共配置组件
│   ├── ConditionRuleEditor.vue  [NEW] 条件规则编辑器
│   ├── UserRoleSelector.vue     [NEW] 用户角色选择器
│   ├── KeyValueEditor.vue       [NEW] 键值编辑器
│   └── ParamMappingEditor.vue   [NEW] 参数映射编辑器
└── utils/                       [NEW] 工具函数
    ├── graphConfig.ts           [NEW] X6图配置
    └── nodeRegistry.ts          [NEW] 节点注册

src/types/workflow/              [MODIFY] 扩展现有类型
├── index.ts                    [MODIFY] 类型导出
├── nodeTypes.ts                [NEW] 节点类型定义
└── graph.ts                    [NEW] 图数据类型
```

### 3.2 现有文件修改

| 文件 | 修改类型 | 修改内容 |
|------|----------|----------|
| src/views/workflow/designer/index.vue | 🔄 重写 | 替换为新的 AntDagDesigner 使用 |
| src/types/workflow.ts | 🔄 扩展 | 添加 AntNodeType 等类型定义 |
| src/i18n/zh-CN/workflow.json | 🔄 扩展 | 添加新节点类型的翻译 |
| src/i18n/en-US/workflow.json | 🔄 扩展 | 添加英文翻译 |
| src/router/modules/workflow.ts | ✅ 兼容 | 无需修改，路由保持不变 |

---

## 4️⃣ 新功能实现要点

### 4.1 AntV X6 集成要点

```typescript
// graphConfig.ts 核心配置
import { Graph, Shape } from '@' + 'antv/x6'

export function createGraph(options: CreateGraphOptions): Graph {
  const graph = new Graph({
    container: options.container,
    
    // 网格配置
    grid: {
      size: 20,
      visible: true,
      type: 'dot',
      args: { color: '#e0e0e0', thickness: 1 }
    },
    
    // 平移配置
    panning: {
      enabled: true,
      modifiers: [],
      eventTypes: ['leftMouseDown', 'mouseWheel']
    },
    
    // 缩放配置
    mousewheel: {
      enabled: true,
      modifiers: ['ctrl'],
      minScale: 0.2,
      maxScale: 4
    },
    
    // 连线配置
    connecting: {
      allowBlank: false,
      allowLoop: false,
      allowNode: false,      // 只能连接端口
      allowPort: true,
      snap: { radius: 20 },  // 吸附半径
      createEdge() {
        return new Shape.Edge({
          attrs: defaultEdgeAttrs,
          connector: { name: 'rounded', args: { radius: 8 } },
          tools: [{
            name: 'button-remove',
            args: { distance: -40 }
          }]
        })
      }
    }
  })
  
  return graph
}
```

### 4.2 节点注册实现

```typescript
// nodeRegistry.ts 节点注册
import { Graph } from '@' + 'antv/x6'
import { AntNodeType, nodeStyleMap } from '@/types/workflow/nodeTypes'

export function registerAntWorkflowNodes() {
  Object.entries(nodeStyleMap).forEach(([type, config]) => {
    const isBranchNode = type === AntNodeType.CONDITION || 
                         type === AntNodeType.PARALLEL
    const size = isBranchNode ? { w: 160, h: 70 } : { w: 160, h: 50 }

    Graph.registerNode('ant-' + type, {
      inherit: 'rect',
      width: size.w,
      height: size.h,
      attrs: {
        body: {
          fill: config.bgColor,
          stroke: config.borderColor,
          strokeWidth: 2,
          rx: 8,
          ry: 8
        },
        label: {
          text: config.typeName,
          fill: config.color,
          fontSize: 14,
          fontWeight: 500
        }
      },
      // 端口配置
      ports: {
        groups: {
          in: {
            position: 'left',
            attrs: {
              circle: {
                r: 6,
                magnet: true,
                stroke: '#31d0c6',
                strokeWidth: 2,
                fill: '#fff'
              }
            }
          },
          out: {
            position: 'right',
            attrs: {
              circle: {
                r: 6,
                magnet: true,
                stroke: '#31d0c6',
                strokeWidth: 2,
                fill: '#fff'
              }
            }
          }
        }
      }
    })
  })
}
```

### 4.3 历史记录实现

```typescript
// useHistory.ts 历史记录钩子
import { ref, computed } from 'vue'

export function useHistory(graph: Graph | null) {
  const history = ref<string[]>([])
  const historyIndex = ref(-1)
  
  const canUndo = computed(() => historyIndex.value > 0)
  const canRedo = computed(() => historyIndex.value < history.value.length - 1)

  const pushHistory = () => {
    if (!graph) return
    
    const dagConfig = exportDagFromGraph(graph)
    const jsonStr = JSON.stringify(dagConfig)
    
    // 剪枝
    if (historyIndex.value < history.value.length - 1) {
      history.value = history.value.slice(0, historyIndex.value + 1)
    }
    
    history.value.push(jsonStr)
    historyIndex.value++
    
    // 限制历史长度
    if (history.value.length > 50) {
      history.value.shift()
      historyIndex.value--
    }
  }

  const undo = () => {
    if (!canUndo.value || !graph) return
    historyIndex.value--
    restoreFromHistory()
  }

  const redo = () => {
    if (!canRedo.value || !graph) return
    historyIndex.value++
    restoreFromHistory()
  }

  return { history, historyIndex, canUndo, canRedo, pushHistory, undo, redo }
}
```

---

## 5️⃣ 依赖安装建议

### 5.1 核心依赖

```bash
# 进入项目目录
cd EasyProduct.Admin

# 安装 AntV X6
pnpm add @antv/x6

# 安装 dagre 用于自动布局（可选）
pnpm add dagre
```

### 5.2 package.json 变更

```json
{
  "dependencies": {
    "@antv/x6": "^2.x",
    "dagre": "^0.8.5"
  }
}
```

### 5.3 可选依赖

| 包名 | 用途 | 优先级 |
|------|------|--------|
| @antv/x6-plugin-minimap | MiniMap 小地图插件 | 🟢 可选 |
| @antv/x6-plugin-snapline | 对齐线插件 | 🟡 推荐 |
| @antv/x6-plugin-transform | 节点变换插件 | 🟢 可选 |

---

## 6️⃣ 迁移路线图

### 阶段一：基础建设 (Week 1)
- [ ] 安装 AntV X6 依赖
- [ ] 创建新的类型定义文件
- [ ] 实现 utils/graphConfig.ts 基础配置
- [ ] 实现 utils/nodeRegistry.ts 节点注册

### 阶段二：核心组件 (Week 2)
- [ ] 开发 Canvas.vue 组件
- [ ] 开发 NodePalette.vue 组件
- [ ] 开发 Toolbar.vue 组件
- [ ] 实现历史记录功能
- [ ] 实现缩放平移功能

### 阶段三：属性面板 (Week 3)
- [ ] 开发 PropertyPanel.vue 框架
- [ ] 实现基础节点配置
- [ ] 实现审批节点配置
- [ ] 实现抄送节点配置

### 阶段四：高级功能 (Week 4)
- [ ] 实现并行分支、服务任务
- [ ] 实现通知节点、Webhook
- [ ] 实现子流程、会签节点

### 阶段五：集成测试 (Week 5)
- [ ] 集成到现有路由
- [ ] 数据导入导出兼容性测试
- [ ] 性能测试（100+节点场景）
- [ ] 用户验收测试

---

## 7️⃣ 数据兼容性

### 7.1 旧数据结构 (当前 EasyProduct)

```typescript
interface FlowNode {
  id: string
  type: 'start' | 'approval' | 'condition' | 'cc' | 'end'
  name: string
  x: number
  y: number
  assigneeType?: 'user' | 'role' | 'dept' | 'self'
  assigneeId?: string
  assigneeName?: string
}

interface FlowEdge {
  id: string
  source: string
  target: string
  label?: string
  condition?: string
}
```

### 7.2 新数据结构 (Ant Workflow)

```typescript
interface DagNode {
  id: string
  name: string
  type: AntNodeType        // 11种类型
  position: { x: number; y: number }
  config: Record<string, any>
}

interface DagEdge {
  id: string
  sourceNodeId: string
  targetNodeId: string
  sourcePort?: string
  targetPort?: string
  condition?: EdgeCondition
}
```

---

## 8️⃣ 参考资源

### 8.1 官方文档
- AntV X6 官方文档: https://x6.antv.vision/
- X6 API 参考: https://x6.antv.vision/api

### 8.2 参考代码位置

```
D:\4-MyProject\EasyProject\PCWeb\src\components\ant_workflow\
├── AntDagDesigner.vue          # 主设计器
├── Canvas.vue                 # X6 画布
├── NodePalette.vue            # 节点库
├── Toolbar.vue                # 工具栏
├── PropertyPanel.vue          # 属性面板
├── nodes/                     # 节点配置组件
├── common/                    # 公共组件
└── utils/                     # 工具函数

D:\4-MyProject\EasyProject\PCWeb\src\types\antWorkflow\
├── nodeTypes.ts               # 节点类型定义
├── workflow.ts                # 流程定义
└── index.ts                   # 类型导出
```

---

## 9️⃣ 总结

本次升级将 EasyProduct.Admin 的工作流设计器从原生 SVG/DOM 实现迁移至 AntV X6 专业图引擎，核心收益：

1. **功能增强**：从 5 种节点扩展至 11 种完整工作流节点类型
2. **性能提升**：X6 引擎支持大规模图流畅渲染
3. **体验升级**：专业交互（端口连接、撤销重做、自动布局）
4. **可维护性**：组件化架构，职责清晰
5. **扩展能力**：基于 X6 生态，可轻松扩展新功能

预计开发周期：**5周**

建议优先实现基础功能（阶段1-2），确保核心可用后再逐步添加高级节点类型。

---

**文档版本**: v1.0  
**创建日期**: 2026-08-31  
**文档作者**: AI Assistant  
