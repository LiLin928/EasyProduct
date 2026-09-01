# VueFlow 工作流设计器实施指南

> 完整的分步实施指南，用于将 EasyProduct 工作流设计器从原生 SVG 迁移到 VueFlow

---

## 📋 前置条件

- 已完成 `workflow-vueflow-migration-plan.md` 文档阅读
- 了解 EasyRag VueFlow 参考实现
- 当前代码状态已备份

---

## 🚀 实施步骤

### 第 1 步：安装依赖

```bash
cd D:\4-MyProject\EasyProduct\EasyProduct.Admin
pnpm add @vue-flow/core @vue-flow/background @vue-flow/controls @vue-flow/minimap
```

### 第 2 步：创建类型定义

**文件**: `src/types/workflow.ts`

已创建，包含：
- NodeType 枚举（10种节点类型）
- WfNode 和 WfEdge 接口
- Workflow 完整定义
- NODE_TYPES 配置数组

### 第 3 步：创建 Store

**文件**: 
- `src/stores/workflowEditor.ts` - 编辑器状态管理
- `src/stores/workflowExecution.ts` - 执行状态管理

核心功能：
- ✅ 撤销/重做 (undo/redo)
- ✅ 边中间插入节点 (insertNodeBetween)
- ✅ 自动布局 (autoLayout)

### 第 4 步：创建 API 层

**文件**: `src/api/workflow.ts`

包含接口：
- getWorkflowList
- getWorkflowDetail
- createWorkflow
- updateWorkflow
- publishWorkflow
- deleteWorkflow
- validateWorkflow
- getExecutionList
- executeWorkflow

### 第 5 步：创建 VueFlow 组件

#### 5.1 BaseNodeCard.vue
- 节点卡片组件
- 支持10种节点类型
- 动态颜色、图标
- Handle 连接点配置

#### 5.2 AddButtonEdge.vue
- 可插入节点的边
- 点击+号打开节点菜单
- 支持基础/高级节点分类

#### 5.3 WorkflowCanvas.vue
- VueFlow 画布主组件
- 节点拖拽放置
- 事件绑定
- 样式配置

#### 5.4 NodePalette.vue
- 左侧节点面板
- 拖拽添加节点
- 分类展示

### 第 6 步：更新主页面

**文件**: `src/views/workflow/designer/index.vue`

使用已创建的 `index-vueflow.vue` 作为参考：

```bash
# 备份原文件
copy src\views\workflow\designer\index.vue src\views\workflow\designer\index-backup.vue

# 使用新文件
rename src\views\workflow\designer\index-vueflow.vue src\views\workflow\designer\index.vue
```

### 第 7 步：配置 Mock 数据

**文件**:
- `mock-server/src/data/workflow.ts` - Mock 数据
- `mock-server/src/routes/workflow.ts` - Mock 路由

在 `mock-server/src/app.ts` 中注册路由：

```typescript
import workflowRouter from './routes/workflow'
app.use('/api/admin/workflow', workflowRouter)
```

### 第 8 步：添加 i18n 翻译

**文件**: `src/i18n/locales/zh-CN/workflow.json`

```json
{
  "designer": {
    "title": "工作流设计器",
    "palette": "节点面板",
    "properties": "属性配置",
    "back": "返回",
    "save": "保存",
    "publish": "发布",
    "validate": "验证",
    "execute": "执行",
    "debug": "调试",
    "undo": "撤销",
    "autoLayout": "自动布局",
    "untitled": "未命名流程"
  },
  "node": {
    "start": "开始",
    "end": "结束",
    "approval": "审批",
    "condition": "条件",
    "cc": "抄送",
    "copy": "复制",
    "parallel": "并行",
    "delay": "延迟",
    "subprocess": "子流程",
    "service": "服务"
  }
}
```

### 第 9 步：更新路由

**文件**: `src/router/modules/workflow.ts`

确保路由配置正确：

```typescript
{
  path: '/workflow/designer/:id',
  name: 'WorkflowDesigner',
  component: () => import('@/views/workflow/designer/index.vue'),
  meta: { title: '工作流设计' }
}
```

---

## 🧪 测试验证

### 功能测试清单

- [ ] VueFlow 画布正常渲染
- [ ] 节点拖拽添加功能
- [ ] 节点之间连线功能
- [ ] 边中间插入节点功能
- [ ] 节点双击编辑功能
- [ ] 撤销/重做功能
- [ ] 自动布局功能
- [ ] 保存/发布流程
- [ ] 流程执行功能

### 兼容性测试

- [ ] 浏览器兼容性 (Chrome/Firefox/Edge)
- [ ] 响应式布局
- [ ] 缩放/平移功能

---

## 📚 关键代码片段

### 撤销功能实现

```typescript
// stores/workflowEditor.ts
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

### 边中间插入节点

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

  const newNode = addNode(nodeType, midPos)

  // 删除原边，添加两条新边
  edges.value = edges.value.filter(e => e.id !== edgeId)
  edges.value.push(
    { id: 'e-' + Date.now() + '-1', source: edge.source, target: newNode.id },
    { id: 'e-' + Date.now() + '-2', source: newNode.id, target: edge.target }
  )
}
```

---

## 🔧 常见问题

### Q1: VueFlow 样式不生效？

确保导入样式文件：

```typescript
import '@vue-flow/core/dist/style.css'
import '@vue-flow/core/dist/theme-default.css'
```

### Q2: 节点无法拖拽？

检查 `VueFlow` 组件配置：

```vue
<VueFlow
  v-model:nodes="nodes"
  v-model:edges="edges"
  :default-zoom="1"
  :min-zoom="0.2"
  :max-zoom="4"
  fit-view-on-init
>
```

### Q3: Handle 连接点位置不对？

使用 `Position` 枚举指定位置：

```typescript
import { Position } from '@vue-flow/core'

<Handle type="target" :position="Position.Left" />
<Handle type="source" :position="Position.Right" />
```

---

## 📁 文件清单

### 新增文件

| 文件路径 | 说明 |
|---------|------|
| `src/types/workflow.ts` | 工作流类型定义 |
| `src/stores/workflowEditor.ts` | 编辑器 Store |
| `src/stores/workflowExecution.ts` | 执行 Store |
| `src/api/workflow.ts` | 工作流 API |
| `src/views/workflow/designer/components/WorkflowCanvas.vue` | VueFlow 画布 |
| `src/views/workflow/designer/components/BaseNodeCard.vue` | 节点卡片 |
| `src/views/workflow/designer/components/AddButtonEdge.vue` | 可插入边的组件 |
| `src/views/workflow/designer/components/NodePalette.vue` | 节点面板 |
| `mock-server/src/data/workflow.ts` | Mock 数据 |
| `mock-server/src/routes/workflow.ts` | Mock 路由 |

### 修改文件

| 文件路径 | 修改内容 |
|---------|---------|
| `src/views/workflow/designer/index.vue` | 替换为 VueFlow 版本 |
| `mock-server/src/app.ts` | 注册 workflow 路由 |
| `src/i18n/locales/zh-CN/workflow.json` | 添加翻译 |

---

## ✅ 完成标准

1. ✅ VueFlow 依赖安装成功
2. ✅ 所有类型定义完整
3. ✅ Store 功能测试通过
4. ✅ Mock 数据正常返回
5. ✅ 前端页面正常渲染
6. ✅ 节点拖拽/连线功能正常
7. ✅ 撤销/重做功能正常
8. ✅ 保存/发布功能正常

---

## 📞 参考资源

- [VueFlow 官方文档](https://vueflow.dev/)
- [EasyRag 参考实现](D:\4-MyProject\EasyRag\frontend\src\views\workflow)
- [VueFlow GitHub](https://github.com/bcakmakoglu/vue-flow)
