# Mock Server 接口梳理总结

> 本文档总结了基于 mock-server 分析的所有实体和接口

## 📊 梳理成果

已创建以下文档：

1. **总览文档**: `docs/api/README.md`
   - API 统一响应格式
   - 模块划分概览
   - 认证方式说明

2. **Basic 模块**: `docs/api/basic-module.md`
   - 8 个核心实体：User, Role, Menu, Dept, DictType, DictData, SystemConfig, Announcement
   - 40+ 个管理接口
   - 数据库表设计

3. **Mall 模块**: `docs/api/mall-module.md`
   - 6 个核心实体：Member, Level, PointsRecord, Coupon, Order, Payment
   - 30+ 个管理接口
   - 数据库表设计

4. **CRM 模块**: `docs/api/crm-module.md`
   - 20+ 个核心实体（客户、供应商、进销存、财务）
   - 50+ 个管理接口
   - 模块间关联关系

5. **其他模块**: `docs/api/other-modules.md`
   - Site 模块（官网）
   - Product 模块（商品）
   - App 模块（小程序）
   - Workflow 模块（工作流）
   - Report 模块（报表）
   - Ops 模块（运维）

---

## 🎯 核心发现

### 1. 实体统计

| 模块 | 实体数量 | 主要实体 |
|------|---------|---------|
| Basic | 8 | User, Role, Menu, Dept, Dict, Config |
| Site | 8 | News, Category, Banner, Video, Download |
| Product | 3 | Category, SPU, SKU |
| Mall | 6 | Member, Level, Points, Coupon, Order, Payment |
| CRM | 20+ | Customer, Supplier, Order, Stock, Invoice, Payment |
| Workflow | 3 | Definition, Instance, Task |
| Report | 3 | Datasource, Definition, Template |
| Ops | 4 | OperateLog, LoginLog, Task, TaskLog |
| **总计** | **50+** | - |

### 2. 接口统计

| 模块 | 接口数量 | 路由前缀 |
|------|---------|---------|
| Basic | 40+ | `/api/admin/basic/*` |
| Site | 20+ | `/api/site/*` + `/api/admin/site-*` |
| Product | 15+ | `/api/admin/product/*` |
| Mall | 30+ | `/api/admin/mall/*` + `/api/app/*` |
| CRM | 50+ | `/api/admin/crm/*` |
| Workflow | 20+ | `/api/admin/workflow/*` |
| Report | 10+ | `/api/admin/report/*` |
| Ops | 15+ | `/api/admin/ops/*` |
| **总计** | **200+** | - |

### 3. 关键设计模式

#### 统一响应格式
```typescript
interface ApiResponse<T> {
  code: number       // 200 成功
  message: string
  data: T
  timestamp: number
}

interface PageResult<T> {
  list: T[]
  total: number
  pageIndex: number
  pageSize: number
  totalPages: number
  hasNextPage: boolean
  hasPrevPage: boolean
}
```

#### 认证方式
- **Admin JWT**: 管理端接口，`/api/admin/*`
- **Member JWT**: 小程序会员接口，`/api/app/*`
- **匿名访问**: 官网接口，`/api/site/*`

#### 数据规范
- **主键**: GUID 字符串
- **命名**: JSON camelCase，数据库 snake_case
- **分页**: pageIndex 从 1 开始
- **状态**: 小写字符串（`'enabled'`, `'active'` 等）

---

## 🔧 技术栈

### Mock Server
- **框架**: Express + TypeScript
- **Mock 数据**: MockJS
- **端口**: 7700
- **路由前缀**: `/api`

### 后端（.NET 8）
- **ORM**: SqlSugar
- **IoC**: Autofac
- **日志**: Serilog
- **任务调度**: Quartz
- **测试**: xUnit

### 前端（Vue 3）
- **UI**: Element Plus
- **状态管理**: Pinia
- **路由**: Vue Router
- **HTTP**: Axios
- **国际化**: vue-i18n

---

## 📝 下一步行动

### 1. 后端开发优先级

根据 mock 状态跟踪：

- **P1 阶段**: Basic 模块（认证、用户、角色、菜单、字典）
- **P2 阶段**: Site 模块（官网内容管理）
- **P3 阶段**: Mall 模块（会员、订单、支付）
- **P3-ext**: App 模块（小程序会员端）
- **P4 阶段**: Product + CRM（商品、进销存、财务）
- **P5 阶段**: Workflow + Report + Ops（工作流、报表、运维）

### 2. 开发规范

#### 后端开发流程
1. 阅读对应模块的接口文档
2. 创建实体类（Entity）
3. 创建 DTO（Request/Response）
4. 实现 Service 层（添加中文注释）
5. 实现 Controller 层（添加中文注释）
6. 编写单元测试（涉钱逻辑必须）
7. 更新 Swagger 文档

#### 前端开发流程
1. 阅读前端开发规范（`docs/frontend-guidelines.md`）
2. 创建 API 接口文件
3. 实现页面组件
4. 添加 i18n 翻译
5. 提交前检查：vue-tsc、ESLint、check:i18n

---

## 🚀 Mock Server 使用指南

### 启动 Mock Server

```bash
cd mock-server
pnpm install
pnpm dev
```

访问地址：http://localhost:7700

### Mock 接口测试

```bash
# 登录
curl -X POST http://localhost:7700/api/admin/auth/login \
  -H "Content-Type: application/json" \
  -d '{"userName":"admin","password":"admin123"}'

# 获取用户列表
curl http://localhost:7700/api/admin/basic/user/list?pageIndex=1&pageSize=10

# 查看 mock 状态
curl http://localhost:7700/__mock/status

# 重置 mock 数据
curl -X POST http://localhost:7700/__mock/reset
```

---

## 📚 参考文档

- **项目规范**: `CLAUDE.md`
- **前端规范**: `docs/frontend-guidelines.md`
- **后端规范**: `docs/backend-guidelines.md`
- **Mock 规范**: `docs/mock-guidelines.md`
- **整体设计**: `docs/superpowers/specs/2026-08-08-easyproduct-integration-design.md`

---

生成时间: 2026-09-07
生成工具: Claude Code + Superpowers
数据来源: mock-server (D:\4-MyProject\EasyProduct\mock-server)