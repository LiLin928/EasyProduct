# EasyProduct Mock Server

独立 Mock 服务：按后端契约提前实现三分区接口，三端共享。契约依据 docs/mock-guidelines.md。

## 启动

pnpm install
pnpm dev   # http://localhost:7700

管理端点：GET /__mock/status（状态表）、POST /__mock/reset（重置内存数据）

## 模块 Mock 状态表

| 分区 | 模块 | mock 状态 | 后端交付 | 切换日期 | 经手 |
|------|------|----------|---------|---------|------|
| admin | Basic（认证/菜单/字典骨架） | pending | P1 | — | — |
| site | 官网内容（首页聚合骨架） | pending | P2 | — | — |
| app | 商城（会员登录骨架） | pending | P3 | — | — |

状态值：pending（mock 生效中）→ deprecated（后端已交付、已切换）→ removed（P6 已清理）
