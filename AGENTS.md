# AGENTS.md - EasyProduct 项目开发指引

> EasyProduct = EasyWebSite（官网）+ EasyProject（电商管理）+ EasyCRM（CRM/ERP 前端）三项目整合后的模块化单体项目。

## 必读文档（按任务类型）

| 任务 | 先读 |
|------|------|
| 理解整体架构、模块划分、数据模型、阶段规划 | `docs/superpowers/specs/2026-08-08-easyproduct-integration-design.md` |
| **任何前端开发**（Admin/Site/MiniApp） | `docs/frontend-guidelines.md`（强制，含冲突裁决；与源项目旧规范冲突时以它为准） |
| **任何后端开发**（EasyProduct.WebApi） | `docs/backend-guidelines.md`（强制，含三源差异裁决附录；与 EasyWechatWeb 旧写法冲突时以它为准） |
| **Mock 数据开发 / 前后端联调切换** | `docs/mock-guidelines.md`（契约先行；mock 与真后端切换流程见其第 8、9 节） |

## 核心事实速查

- 后端：单一 .NET 8 模块化单体（EasyProduct.WebApi），SqlSugar + Autofac（构造器注入）+ Serilog + Quartz；API 三分区 `/api/admin/**`、`/api/site/**`、`/api/app/**`
- Mock：独立 `mock-server/`（Node + Express + mockjs，端口 7700），三端共享，随后端交付逐模块退役
- 前端：EasyProduct.Admin（Vue3+EP 管理后台）、EasyProduct.Site（官网门户）、EasyProduct.MiniApp（微信原生小程序）
- 数据库：MySQL 8 单库 `easyproduct`，表前缀 basic_/site_/product_/mall_/crm_/wf_/rpt_/ops_，snake_case
- 统一响应：`{code, message, data, timestamp}`，HTTP 一律 200，code===200 成功；GUID 主键；JSON camelCase；分页 pageIndex/pageSize + list/total；状态字段用小写字符串常量
- 部署：单租户，Docker Compose 单机（nginx + api + mysql + redis 可选）

## 开发纪律

- 修改前先读对应规范文档；规范冲突以 docs/ 下文档为准
- 前端提交前：vue-tsc 零错误、ESLint 通过、check:i18n 无硬编码中文
- 后端提交前：dotnet build 0 错误 0 新警告；涉钱逻辑（库存流水/冲销/收付款核销）先写 xUnit
- mock 新增/修改必须与后端规范第 5 节路由和信封逐字一致，禁止自创接口
- Commit 遵循 Conventional Commits，scope 用端名（admin/site/miniapp/api/mock）