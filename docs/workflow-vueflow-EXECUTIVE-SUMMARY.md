# VueFlow 工作流设计器 - 执行摘要

## 🎯 一句话目标
将 EasyProduct 工作流设计器从原生 SVG 迁移到 VueFlow，新增撤销重做、边插入节点、自动布局等高级功能。

---

## 📊 关键指标

| 指标 | 数值 |
|-----|------|
| **项目周期** | 8个工作日 |
| **总工时** | 58.5h + 5.5h缓冲 |
| **新增文件** | 15个 |
| **节点类型** | 5种 → 10种 |
| **参考项目** | EasyRag |

---

## 📅 时间线

```
Day 1-2    [基础设施]    类型定义 | Store | API | 基础组件
Day 3-4    [核心画布]    WorkflowCanvas | AddButtonEdge | 交互
Day 5-6    [主页面]      重构index | 工具栏 | 撤销重做 | 自动布局
Day 7-8    [Mock+测试]   Mock数据 | 功能测试 | i18n
```

---

## 📁 核心交付物

### 前端代码 (EasyProduct.Admin)
```
src/
├── types/workflow.ts                    # 10种节点类型定义
├── stores/
│   ├── workflowEditor.ts                # 编辑器状态(撤销/重做)
│   └── workflowExecution.ts             # 执行状态
├── api/workflow.ts                      # 9个API接口
└── views/workflow/designer/components/
    ├── WorkflowCanvas.vue               # VueFlow画布
    ├── BaseNodeCard.vue                 # 节点卡片
    ├── AddButtonEdge.vue                # 可插入节点的边
    └── NodePalette.vue                  # 节点面板
```

### Mock数据 (mock-server)
```
src/
├── data/workflow.ts                     # Mock数据
└── routes/workflow.ts                   # Mock路由
```

### 设计文档 (docs)
```
workflow-vueflow-migration-plan.md       # 完整设计方案
workflow-vueflow-implementation-guide.md # 实施指南
workflow-vueflow-dev-plan.md             # 开发计划
```

---

## 🚀 快速启动

```bash
# 1. 安装依赖 (5分钟)
cd EasyProduct.Admin
pnpm add @vue-flow/core @vue-flow/background @vue-flow/controls @vue-flow/minimap

# 2. 复制代码文件 (10分钟)
# 复制所有已创建的.ts和.vue文件到对应目录

# 3. 配置Mock (5分钟)
# 在 mock-server/src/app.ts 中添加:
# import workflowRouter from './routes/workflow'
# app.use('/api/admin/workflow', workflowRouter)

# 4. 启动测试
pnpm dev
```

---

## ✅ 功能清单

### 已实现功能
- [x] VueFlow画布集成
- [x] 10种节点类型 (开始/结束/审批/条件/抄送/复制/并行/延迟/子流程/服务)
- [x] 撤销/重做 (50步历史)
- [x] 边中间插入节点
- [x] 自动布局
- [x] 缩放/平移/小地图
- [x] Mock数据完整

### 待实现
- [ ] 节点配置弹窗
- [ ] 执行调试面板
- [ ] i18n翻译

---

## ⚠️ 风险点

| 风险 | 可能性 | 影响 | 对策 |
|-----|-------|-----|------|
| VueFlow版本兼容 | 中 | 高 | 参考EasyRag版本 |
| 样式冲突 | 中 | 中 | 使用:deep()覆盖 |
| Store数据同步 | 低 | 高 | 充分测试watch |

---

## 📞 参考

- VueFlow文档: https://vueflow.dev/
- EasyRag参考: D:\4-MyProject\EasyRag\frontend\src\views\workflow

---

## 📝 下一步行动

1. 审阅设计文档
2. 安装VueFlow依赖
3. 按Phase 1任务开始开发
4. 每日进度同步

---

**文档创建时间**: 2026-08-31
**负责人**: [待指定]
**状态**: 待开始
