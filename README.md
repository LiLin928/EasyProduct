# EasyProduct

EasyWebSite + EasyProject + EasyCRM 三项目整合后的模块化单体（官网 + 管理后台 + 小程序商城）。

## 目录

| 目录 | 说明 |
|------|------|
| EasyProduct.WebApi/ | 后端（.NET 8，B 系列计划另立，尚未创建） |
| EasyProduct.Admin/ | PC 管理后台（Vue3 + Element Plus），端口 5173 |
| EasyProduct.Site/ | 官网门户（Vue3 响应式），端口 5174 |
| EasyProduct.MiniApp/ | 微信小程序（原生 + TS），微信开发者工具打开 |
| mock-server/ | 独立 Mock 服务（契约先行），端口 7700 |
| docs/ | 设计方案与开发规范（先读 CLAUDE.md） |

## 启动（前端/mock 阶段）

1. `cd mock-server && pnpm install && pnpm dev`（7700）
2. `cd EasyProduct.Admin && pnpm install && pnpm dev`（5173，登录 admin/admin123）
3. `cd EasyProduct.Site && pnpm install && pnpm dev`（5174）
4. 微信开发者工具导入 `EasyProduct.MiniApp`（勾选"不校验合法域名"）

后端联调时：Admin/Site 改 `.env.development` 的 `VITE_PROXY_TARGET=http://localhost:7600`；MiniApp 改 `config/env.ts`。

## 文档

- 整合设计方案：docs/superpowers/specs/2026-08-08-easyproduct-integration-design.md
- 前端规范：docs/frontend-guidelines.md
- 后端规范：docs/backend-guidelines.md
- Mock 规范：docs/mock-guidelines.md
- 前端先行开发计划：docs/superpowers/plans/2026-08-08-frontend-first-development.md
