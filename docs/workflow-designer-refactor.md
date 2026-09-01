# EasyProduct 工作流设计器重构文档

## 一、现状分析

### 1.1 当前设计 (EasyProduct.Admin)

**文件位置**: `EasyProduct.Admin/src/views/workflow/designer/index.vue`

**技术栈**: 纯原生实现 (SVG + DOM + Vue3)

**功能特点**:
- ✅ 基础节点拖拽和连线
- ✅ 简单的5种节点类型: 开始、审批、条件、抄送、结束
- ✅ 节点位置编辑
- ⚠️ 无专业画布引擎
- ⚠️ 无历史记录(撤销/重做)
- ⚠️ 无缩放功能
- ⚠️ 无自动布局
- ⚠️ 属性面板功能简单

---

## 二、参考设计 (EasyProject PCWeb ant_workflow)

**文件位置**: `PCWeb/src/components/ant_workflow/AntDagDesigner.vue`

**技术栈**: AntV X6 + Vue3 + Pinia

**功能特点**:
- ✅ 基于 AntV X6 专业图引擎
- ✅ 11种节点类型: 发起人、审批人、抄送人、条件分支、并行分支、服务任务、通知、Webhook、子流程、会签、结束
- ✅ 完整撤销/重做功能
- ✅ 画布缩放控制
- ✅ 自动布局(基于 dagre)
- ✅ 流程配置预览
- ✅ 丰富的属性配置(每个节点类型独立配置组件)
- ✅ 三栏布局(节点库+画布+属性面板)

---

## 三、差异对比表

| 功能项 | 当前设计 | 参考设计 | 优先级 |
|--------|----------|----------|--------|
| **技术架构** | 纯原生 SVG+DOM | AntV X6 专业引擎 | P0 |
| **节点类型** | 5种(start/approval/condition/cc/end) | 11种(含并行/服务/Webhook/子流程/会签) | P1 |
| **撤销/重做** | ❌ 无 | ✅ 完整历史记录 | P1 |
| **画布缩放** | ❌ 无 | ✅ 缩放/适应视图 | P1 |
| **自动布局** | ❌ 无 | ✅ 基于 dagre | P2 |
| **节点库** | ✅ 简单Palette | ✅ 可折叠+搜索+分组 | P1 |
| **属性面板** | ⚠️ 简单表单 | ✅ 动态组件+丰富配置 | P1 |
| **连线样式** | ⚠️ SVG Path | ✅ 智能曲线(normal/smooth) | P2 |
| **端口连接** | ⚠️ 节点级连接 | ✅ 端口级连接 | P2 |
| **流程预览** | ❌ 无 | ✅ JSON预览+复制 | P3 |

---

## 四、重构建议

### 4.1 架构升级方案

#### 方案A: 引入 AntV X6 (推荐)

**优点**:
- 成熟的图编辑引擎，功能完善
- 社区活跃，文档丰富
- 性能好，支持大规模节点
- 内置撤销/重做、对齐、网格等功能

**缺点**:
- 增加依赖体积(~200KB)
- 学习成本

#### 方案B: 继续优化原生实现

**优点**:
- 无额外依赖
- 完全可控

**缺点**:
- 开发工作量大
- 难以达到专业效果

**推荐**: 方案A (AntV X6)

---

## 五、具体修改清单

### 5.1 依赖安装

```bash
cd EasyProduct.Admin
pnpm add @antv/x6 @antv/x6-plugin-snapshot dagre
```

### 5.2 文件结构改造

```
src/views/workflow/designer/
├── index.vue                 # 主入口(简化)
├── components/
│   ├── AntDagDesigner.vue    # 设计器主组件(新增)
│   ├── Canvas.vue            # X6画布组件(新增)
│   ├── NodePalette.vue       # 节点库组件(新增)
│   ├── PropertyPanel.vue     # 属性面板(新增)
│   ├── Toolbar.vue           # 工具栏(新增)
│   ├── nodes/                # 节点配置组件
│   │   ├── StartNodeConfig.vue
│   │   ├── ApproverNodeConfig.vue
│   │   ├── CopyerNodeConfig.vue
│   │   ├── ConditionNodeConfig.vue
│   │   ├── ParallelNodeConfig.vue
│   │   ├── ServiceNodeConfig.vue
│   │   ├── NotificationNodeConfig.vue
│   │   ├── WebhookNodeConfig.vue
│   │   ├── SubflowNodeConfig.vue
│   │   ├── CounterSignNodeConfig.vue
│   │   └── EndNodeConfig.vue
│   └── common/               # 通用组件
│       ├── ConditionRuleEditor.vue
│       ├── KeyValueEditor.vue
│       ├── ParamMappingEditor.vue
│       └── UserRoleSelector.vue
└── utils/
    ├── graphConfig.ts        # X6图配置
    └── nodeRegistry.ts       # 节点注册管理
```

### 5.3 类型定义扩展

**文件**: `src/types/workflow.ts`

新增:
```typescript
// 节点类型扩展
export type AntNodeType = 
  | 'start' 
  | 'approver' 
  | 'copyer'      // 新增
  | 'condition' 
  | 'parallel'    // 新增
  | 'service'     // 新增
  | 'notification' // 新增
  | 'webhook'     // 新增
  | 'subflow'     // 新增
  | 'counter_sign' // 新增
  | 'end'

// DAG配置
export interface DagConfig {
  version: string
  nodes: DagNode[]
  edges: DagEdge[]
  globalConfig?: DagGlobalConfig
}

export interface DagNode {
  id: string
  name: string
  type: AntNodeType
  position: { x: number; y: number }
  config: any
}

export interface DagEdge {
  id: string
  sourceNodeId: string
  targetNodeId: string
  sourcePort?: string
  targetPort?: string
  condition?: EdgeCondition
}
```

### 5.4 Store 扩展

**文件**: `src/stores/workflow.ts`

新增:
```typescript
// DAG历史记录
const history = ref<string[]>([])
const historyIndex = ref(-1)

const canUndo = computed(() => historyIndex.value > 0)
const canRedo = computed(() => historyIndex.value < history.value.length - 1)

// Actions
function pushHistory(config: DagConfig)
function undo(): DagConfig | null
function redo(): DagConfig | null
```

---

## 六、关键功能实现要点

### 6.1 X6 图初始化

```typescript
// utils/graphConfig.ts
import { Graph } from '@antv/x6'

export const createGraph = (options: Graph.Options) => {
  return new Graph({
    container: options.container,
    grid: { size: 10, visible: true, type: 'dot' },
    panning: { enabled: true },
    mousewheel: { enabled: true, zoomAtMousePosition: true },
    connecting: {
      router: 'manhattan',
      connector: { name: 'smooth' },
      allowBlank: false,
      allowLoop: false,
      highlight: true,
    },
    history: { enabled: true },
    selecting: { enabled: true, rubberband: true },
    snapping: { enabled: true, radius: 10 },
    ...options,
  })
}
```

### 6.2 节点注册

```typescript
// utils/nodeRegistry.ts
import { Graph, Node } from '@antv/x6'

export const registerAntNodes = (graph: Graph) => {
  // 审批节点
  Node.registry.register('approver-node', {
    inherit: 'rect',
    width: 200,
    height: 80,
    attrs: {
      body: {
        fill: '#fff7e6',
        stroke: '#ffd591',
        strokeWidth: 2,
        rx: 4,
      },
      label: {
        fill: '#333',
        fontSize: 14,
      },
    },
    ports: {
      groups: {
        in: { position: 'left', attrs: { circle: { r: 6 } } },
        out: { position: 'right', attrs: { circle: { r: 6 } } },
      },
      items: [
        { id: 'in', group: 'in' },
        { id: 'out', group: 'out' },
      ],
    },
  })
  // ...其他节点类型
}
```

### 6.3 撤销/重做实现

```typescript
// 监听X6内置历史事件
graph.on('history:change', () => {
  canUndo.value = graph.canUndo()
  canRedo.value = graph.canRedo()
})

// 撤销
const handleUndo = () => {
  graph.undo()
}

// 重做
const handleRedo = () => {
  graph.redo()
}
```

### 6.4 自动布局

```typescript
import Dagre from 'dagre'

const handleAutoLayout = () => {
  const dagreGraph = new Dagre.graphlib.Graph()
  dagreGraph.setDefaultEdgeLabel(() => ({}))
  dagreGraph.setGraph({ rankdir: 'LR', ranksep: 50, nodesep: 30 })
  
  // 添加节点和边...
  
  Dagre.layout(dagreGraph)
  
  // 应用布局结果到X6
  dagreGraph.nodes().forEach((id) => {
    const node = graph.getCellById(id)
    if (node) {
      const pos = dagreGraph.node(id)
      node.position(pos.x, pos.y)
    }
  })
}
```

---

## 七、迁移步骤

### Step 1: 依赖安装
```bash
pnpm add @antv/x6 dagre
```

### Step 2: 类型定义更新
- 更新 `src/types/workflow.ts`
- 添加 DAG 相关类型

### Step 3: Store 扩展
- 添加历史记录管理
- 添加 DAG 配置管理

### Step 4: 组件开发
- 开发 Canvas.vue (X6画布)
- 开发 NodePalette.vue (节点库)
- 开发 PropertyPanel.vue (属性面板)
- 开发各节点配置组件

### Step 5: 主页面重构
- 重写 `designer/index.vue`
- 整合各子组件

### Step 6: API 对接
- 更新保存接口
- 更新加载接口

---

## 八、兼容性说明

### 数据格式兼容

**当前格式**:
```json
{
  "definitionId": "",
  "definitionName": "",
  "nodes": [],
  "edges": []
}
```

**新格式**:
```json
{
  "version": "1.0",
  "nodes": [],
  "edges": [],
  "globalConfig": {}
}
```

**迁移方案**: 编写数据迁移脚本

---

## 九、预估工作量

| 任务 | 工时 | 负责人 |
|------|------|--------|
| 依赖安装+环境配置 | 0.5d | |
| 类型定义+Store扩展 | 1d | |
| Canvas组件(X6封装) | 2d | |
| NodePalette组件 | 1d | |
| PropertyPanel组件 | 1d | |
| 节点配置组件(11个) | 3d | |
| Toolbar组件 | 0.5d | |
| 主页面整合 | 1d | |
| API对接+测试 | 1d | |
| **总计** | **~11d** | |

---

## 十、参考资源

- [AntV X6 文档](https://x6.antv.vision/)
- [PCWeb ant_workflow 源码](../PCWeb/src/components/ant_workflow/)
- [Dagre 布局引擎](https://github.com/dagrejs/dagre)
