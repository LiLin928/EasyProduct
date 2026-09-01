# VueFlow 工作流设计器 - 实施开发计划

> 基于 EasyRag 参考实现，详细的任务分解和时间规划

---

## 📊 项目概览

| 项目 | 内容 |
|-----|------|
| **目标** | 将 EasyProduct 工作流设计器从原生 SVG 迁移到 VueFlow |
| **参考** | EasyRag 项目 |
| **周期** | 8 个工作日 |
| **里程碑** | 4 个阶段 |

---

## 🎯 核心功能改进

| 特性 | 当前 (SVG) | VueFlow 方案 |
|-----|-----------|-------------|
| 渲染引擎 | 原生 SVG+DOM | VueFlow |
| 节点拖拽 | 自定义实现 | 内置支持 |
| 连线绘制 | 手动计算 | 贝塞尔曲线 |
| 缩放平移 | 未实现 | 内置支持 |
| 撤销重做 | 未实现 | Store+快照 |
| 边插入节点 | 未实现 | AddButtonEdge |
| 节点类型 | 5种 | 10种 |

---

## 📅 开发阶段 (8天)

### Phase 1: 基础设施 (Day 1-2)

#### Day 1: 依赖和类型
| 任务 | 文件 | 工时 |
|-----|------|-----|
| 安装 VueFlow 依赖 | package.json | 0.5h |
| 创建工作流类型定义 | src/types/workflow.ts | 2h |
| 创建编辑器 Store | src/stores/workflowEditor.ts | 3h |
| 创建执行 Store | src/stores/workflowExecution.ts | 2h |
| **小计** | | **7.5h** |

#### Day 2: API和基础组件
| 任务 | 文件 | 工时 |
|-----|------|-----|
| 创建工作流 API | src/api/workflow.ts | 2h |
| 创建 BaseNodeCard | components/BaseNodeCard.vue | 3h |
| 创建 NodePalette | components/NodePalette.vue | 2h |
| **小计** | | **7h** |

**产出:** 类型定义 ✓ | Store ✓ | API ✓ | 基础组件 ✓

---

### Phase 2: 核心画布 (Day 3-4)

#### Day 3: VueFlow画布
| 任务 | 文件 | 工时 |
|-----|------|-----|
| WorkflowCanvas 组件 | components/WorkflowCanvas.vue | 4h |
| AddButtonEdge 组件 | components/AddButtonEdge.vue | 3h |
| 样式优化 | scss | 1h |
| **小计** | | **8h** |

#### Day 4: 画布功能
| 任务 | 工时 |
|-----|-----|
| 节点拖拽放置 | 2h |
| 连线功能 | 2h |
| 节点双击编辑 | 2h |
| Delete键删除 | 1h |
| **小计** | **7h** |

**产出:** VueFlow画布 ✓ | 拖拽 ✓ | 连线 ✓ | 边插入节点 ✓

---

### Phase 3: 主页面和功能 (Day 5-6)

#### Day 5: 主页面
| 任务 | 文件 | 工时 |
|-----|------|-----|
| 重构 designer/index.vue | index.vue | 5h |
| 工具栏实现 | index.vue | 2h |
| **小计** | | **7h** |

#### Day 6: 高级功能
| 任务 | 工时 |
|-----|-----|
| 撤销/重做功能 | 2h |
| 自动布局功能 | 2h |
| 节点配置面板 | 2h |
| 属性表单实现 | 2h |
| **小计** | **8h** |

**产出:** 主页面 ✓ | 撤销重做 ✓ | 自动布局 ✓ | 节点配置 ✓

---

### Phase 4: Mock和联调 (Day 7-8)

#### Day 7: Mock数据
| 任务 | 文件 | 工时 |
|-----|------|-----|
| 工作流 Mock数据 | mock-server/data/workflow.ts | 3h |
| Mock路由 | mock-server/routes/workflow.ts | 2h |
| 注册路由 | mock-server/app.ts | 1h |
| **小计** | | **6h** |

#### Day 8: 测试优化
| 任务 | 工时 |
|-----|-----|
| 功能测试 | 3h |
| Bug修复 | 3h |
| i18n翻译 | 1h |
| 文档更新 | 1h |
| **小计** | **8h** |

**产出:** Mock数据 ✓ | 功能测试 ✓ | i18n ✓

---

## 📋 详细任务清单

### Phase 1 任务

#### Task 1.1: 安装依赖 (0.5h)
```bash
cd EasyProduct.Admin
pnpm add @vue-flow/core @vue-flow/background @vue-flow/controls @vue-flow/minimap
```
**验收:** package.json 包含依赖

#### Task 1.2: 类型定义 (2h)
**文件:** src/types/workflow.ts
**内容:**
- NodeType 枚举 (10种)
- WfNode/WfEdge/Workflow 接口
- NODE_TYPES 配置

**验收:** TypeScript编译无错

#### Task 1.3: 编辑器Store (3h)
**文件:** src/stores/workflowEditor.ts
**功能:**
- 节点操作 (add/update/remove)
- 撤销/重做 (saveUndo/undo)
- 边插入节点 (insertNodeBetween)
- 自动布局 (autoLayout)
- 保存/发布

**验收:** Store可导入，功能正常

#### Task 1.4: 执行Store (2h)
**文件:** src/stores/workflowExecution.ts
**功能:** 执行状态、节点状态、日志

#### Task 1.5: 工作流API (2h)
**文件:** src/api/workflow.ts
**接口:** list/detail/create/update/publish/delete/validate/executions/execute

#### Task 1.6: BaseNodeCard (3h)
**文件:** components/BaseNodeCard.vue
**功能:** 10种节点样式、Handle连接点

#### Task 1.7: NodePalette (2h)
**文件:** components/NodePalette.vue
**功能:** 节点分类、拖拽支持

---

### Phase 2 任务

#### Task 2.1: WorkflowCanvas (4h)
**文件:** components/WorkflowCanvas.vue
**功能:**
- VueFlow画布集成
- Background/Controls/MiniMap
- 节点模板注册
- 事件监听

#### Task 2.2: AddButtonEdge (3h)
**文件:** components/AddButtonEdge.vue
**功能:** 边上+按钮、插入节点菜单

#### Task 2.3: 节点交互 (5h)
- 拖拽添加节点
- 连线功能
- 双击编辑
- Delete键删除

---

### Phase 3 任务

#### Task 3.1: 主页面重构 (5h)
**文件:** designer/index.vue
**内容:**
- VueFlow画布集成
- 节点面板
- 属性面板

#### Task 3.2: 工具栏 (2h)
**功能:** 返回、撤销、自动布局、验证、执行、保存、发布

#### Task 3.3: 撤销/重做 (2h)
**功能:** Ctrl+Z撤销、50步历史

#### Task 3.4: 自动布局 (2h)
**功能:** 节点重新排列

#### Task 3.5: 节点配置 (4h)
**功能:** 审批/条件/延迟等节点配置

---

### Phase 4 任务

#### Task 4.1: Mock数据 (3h)
**文件:** mock-server/data/workflow.ts
**内容:** 列表、详情、执行历史

#### Task 4.2: Mock路由 (2h)
**文件:** mock-server/routes/workflow.ts
**内容:** 所有API路由

#### Task 4.3: 功能测试 (3h)
**清单:**
- [ ] 画布渲染
- [ ] 节点拖拽
- [ ] 节点连线
- [ ] 边插入节点
- [ ] 撤销/重做
- [ ] 自动布局
- [ ] 保存/发布

#### Task 4.4: i18n (1h)
**文件:** locales/zh-CN/workflow.json

---

## ⚠️ 风险点和对策

| 风险 | 可能性 | 影响 | 对策 |
|-----|-------|-----|------|
| VueFlow版本兼容 | 中 | 高 | 参考EasyRag版本 |
| 样式冲突 | 中 | 中 | 使用:deep()覆盖 |
| Store同步问题 | 低 | 高 | 充分测试watch |
| Mock数据格式 | 中 | 中 | 严格遵循API规范 |

---

## 📊 时间汇总

| 阶段 | 天数 | 工时 | 缓冲 |
|-----|------|-----|------|
| Phase 1: 基础设施 | 2 | 14.5h | 1.5h |
| Phase 2: 核心画布 | 2 | 15h | 1h |
| Phase 3: 主页面 | 2 | 15h | 1h |
| Phase 4: Mock测试 | 2 | 14h | 2h |
| **总计** | **8** | **58.5h** | **5.5h** |

---

## 🚀 快速开始

```bash
# Day 1
pnpm add @vue-flow/core @vue-flow/background @vue-flow/controls @vue-flow/minimap

# Day 2-7: 按任务清单开发

# Day 8: 测试
pnpm type-check
pnpm lint

# 启动验证
cd mock-server && pnpm dev
cd EasyProduct.Admin && pnpm dev
```

---

## ✅ Phase完成标准

### Phase 1
- [ ] VueFlow依赖安装
- [ ] types/workflow.ts编译通过
- [ ] Store功能测试
- [ ] API导入测试
- [ ] 基础组件渲染

### Phase 2
- [ ] WorkflowCanvas渲染
- [ ] AddButtonEdge渲染
- [ ] 拖拽功能正常
- [ ] 连线功能正常
- [ ] 边插入节点正常

### Phase 3
- [ ] 主页面加载
- [ ] 工具栏功能
- [ ] 撤销/重做
- [ ] 自动布局
- [ ] 节点配置

### Phase 4
- [ ] Mock API可用
- [ ] 功能测试通过
- [ ] i18n翻译
- [ ] 文档更新
