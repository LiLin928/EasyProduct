# VueFlow 工作流设计器迁移方案总结

> 基于 EasyRag 项目参考，为 EasyProduct 设计 VueFlow 迁移完整方案

---

## 📋 文档清单

| 文档 | 路径 | 说明 |
|-----|------|------|
| 迁移计划 | `docs/workflow-vueflow-migration-plan.md` | 完整的设计方案和技术规范 |
| 实施指南 | `docs/workflow-vueflow-implementation-guide.md` | 分步骤实施指南 |
| 方案对比 | `docs/workflow-designer-vueflow-comparison.md` | 与现有设计对比分析 |

---

## 🎯 方案核心改进

### 当前设计 vs VueFlow 设计

| 特性 | 当前 (SVG+DOM) | VueFlow 方案 |
|-----|---------------|-------------|
| 渲染引擎 | 原生 SVG + DOM | VueFlow (@vue-flow/core) |
| 节点拖拽 | 自定义实现 | VueFlow 内置 |
| 连线绘制 | SVG Path 手动计算 | VueFlow 内置贝塞尔曲线 |
| 缩放平移 | 未实现 | 内置支持 |
| 撤销重做 | 未实现 | Pinia Store + 快照 |
| 边插入节点 | 无 | 边点击+菜单 |
| 节点类型 | 5种 | 10种 (扩展) |
| 调试执行 | 无 | 完整面板 |

---

## 📦 安装依赖

```bash
cd EasyProduct.Admin
pnpm add @vue-flow/core @vue-flow/background @vue-flow/controls @vue-flow/minimap
```

---

## 📁 创建的文件清单

### 类型定义
- `src/types/workflow.ts` - 工作流类型定义 (NodeType, WfNode, WfEdge, Workflow 等)

### Store
- `src/stores/workflowEditor.ts` - 编辑器状态管理 (撤销/重做, 节点操作, 保存发布)
- `src/stores/workflowExecution.ts` - 执行状态管理 (调试, 节点状态跟踪)

### API
- `src/api/workflow.ts` - 工作流 API (列表, 详情, 创建, 更新, 发布, 执行等)

### VueFlow 组件
- `src/views/workflow/designer/components/WorkflowCanvas.vue` - VueFlow 画布主组件
- `src/views/workflow/designer/components/BaseNodeCard.vue` - 节点卡片 (10种类型样式)
- `src/views/workflow/designer/components/AddButtonEdge.vue` - 可插入节点的边组件
- `src/views/workflow/designer/components/NodePalette.vue` - 左侧节点面板

### Mock 数据
- `mock-server/src/data/workflow.ts` - 工作流 Mock 数据 (列表, 详情, 执行历史)
- `mock-server/src/routes/workflow.ts` - Mock 路由配置

### 主页面 (参考)
- `src/views/workflow/designer/index-vueflow.vue` - 重构后的主页面参考

---

## 🔑 核心功能实现

### 1. 撤销/重做 (Undo/Redo)
```typescript
const undoStack = ref<string[]>([])

function saveUndo() {
  const snapshot = JSON.stringify({ nodes: nodes.value, edges: edges.value })
  undoStack.value.push(snapshot)
  if (undoStack.value.length > 50) undoStack.value.shift()
}

function undo() {
  if (undoStack.value.length === 0) return
  const snapshot = JSON.parse(undoStack.value.pop()!)
  nodes.value = snapshot.nodes
  edges.value = snapshot.edges
  dirty.value = true
}
```

### 2. 边中间插入节点
```typescript
function insertNodeBetween(edgeId: string, nodeType: string) {
  const edge = edges.value.find(e => e.id === edgeId)
  if (!edge) return

  const sourceNode = nodes.value.find(n => n.id === edge.source)
  const targetNode = nodes.value.find(n => n.id === edge.target)
  if (!sourceNode || !targetNode) return

  const midPos = {
    x: (sourceNode.position.x + targetNode.position.x) / 2,
    y: (sourceNode.position.y + targetNode.position.y) / 2
  }

  // 创建新节点，删除原边，添加两条新边
  const newNode = addNode(nodeType, midPos)
  edges.value = edges.value.filter(e => e.id !== edgeId)
  edges.value.push(
    { id: 'e-' + Date.now() + '-1', source: edge.source, target: newNode.id },
    { id: 'e-' + Date.now() + '-2', source: newNode.id, target: edge.target }
  )
}
```

### 3. 节点类型定义 (10种)
```typescript
export const NODE_TYPES: NodeTypeInfo[] = [
  { type: 'start', name: '开始', group: 'basic', color: '#334155', icon: 'VideoPlay' },
  { type: 'end', name: '结束', group: 'basic', color: '#334155', icon: 'VideoPause' },
  { type: 'approval', name: '审批', group: 'basic', color: '#409EFF', icon: 'User' },
  { type: 'condition', name: '条件', group: 'basic', color: '#CA8A04', icon: 'Share' },
  { type: 'cc', name: '抄送', group: 'basic', color: '#909399', icon: 'Message' },
  { type: 'copy', name: '复制', group: 'advanced', color: '#67C23A', icon: 'CopyDocument' },
  { type: 'parallel', name: '并行', group: 'advanced', color: '#E6A23C', icon: 'Grid' },
  { type: 'delay', name: '延迟', group: 'advanced', color: '#F56C6C', icon: 'Timer' },
  { type: 'subprocess', name: '子流程', group: 'advanced', color: '#8E44AD', icon: 'SetUp' },
  { type: 'service', name: '服务', group: 'advanced', color: '#17A2B8', icon: 'Service' }
]
```

---

## 📊 Mock 数据结构

### 工作流列表
- 5个示例工作流
- 包含草稿和已发布状态
- 支持分页和关键词搜索

### 工作流详情
- 完整的节点和边数据
- 条件分支 (yes/no)
- 节点配置预览

### 执行历史
- 执行状态跟踪
- 节点进度显示
- 执行时长统计

---

## 🚀 实施步骤

### Step 1: 安装依赖
```bash
cd EasyProduct.Admin
pnpm add @vue-flow/core @vue-flow/background @vue-flow/controls @vue-flow/minimap
```

### Step 2: 复制文件
将创建的文件复制到项目对应目录

### Step 3: 更新主页面
备份原文件，使用新文件

### Step 4: 配置 Mock
在 mock-server/src/app.ts 中注册 workflow 路由

### Step 5: 启动测试
启动 Mock 服务器和前端进行测试

---

## ✅ 测试清单

- [ ] VueFlow 画布正常渲染
- [ ] 节点拖拽添加功能正常
- [ ] 节点之间连线功能正常
- [ ] 边中间插入节点功能正常
- [ ] 节点双击打开配置
- [ ] 撤销/重做功能正常
- [ ] 自动布局功能正常
- [ ] 保存/发布流程功能正常
- [ ] 执行/调试功能正常

---

## 📞 参考

- VueFlow 官方文档: https://vueflow.dev/
- EasyRag 参考: D:\4-MyProject\EasyRag\frontend\src\views\workflow
