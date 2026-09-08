# P2 批次 3：SKU 管理实施计划

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task.

**Goal:** 实现 SKU（库存量单位）管理功能，支持商品规格、价格、库存管理。

**Architecture:** 基于批次 1-2 已完成的分类和 SPU，实现 SKU 管理。支持规格组合、多价格体系、库存管理。

**Tech Stack:** .NET 8.0 + SqlSugar 5.1.4 + Mapster 10.x

---

## 模块概述

**SKU（Stock Keeping Unit）- 库存量单位**
- 商品的具体规格组合
- 关联 SPU（product_spu）
- 支持条码、零售价、会员价、B2B价、成本价
- 支持库存数量
- 支持规格组合（颜色、尺寸等）

---

## 任务划分

| 任务 | 内容 | 预计工时 |
|------|------|---------|
| **Task 1** | 更新数据库添加 SKU 表 | 0.5 天 |
| **Task 2** | 创建 SKU 实体 | 0.5 天 |
| **Task 3** | 创建 SKU DTO | 1 天 |
| **Task 4** | 创建 SKU 服务 | 1.5 天 |
| **Task 5** | 创建 SKU 控制器 | 1 天 |

---

## Task 1: 更新数据库添加 SKU 表

**Files:**
- Modify: `sql/init-database.sql`

**表结构设计：**
- id: CHAR(36) - 主键ID
- spu_id: CHAR(36) - 关联SPU
- sku_name: VARCHAR(200) - SKU名称
- sku_code: VARCHAR(50) - SKU编码（唯一）
- barcode: VARCHAR(50) - 条码
- spec_json: TEXT - 规格组合JSON
- price: DECIMAL(18,2) - 零售价
- member_price: DECIMAL(18,2) - 会员价
- wholesale_price: DECIMAL(18,2) - B2B价
- cost_price: DECIMAL(18,2) - 成本价
- stock: INT - 库存数量
- status: INT - 状态
- create_time, update_time, create_by, is_deleted - 审计字段

**索引：**
- 主键：id
- 唯一索引：sku_code
- 外键索引：spu_id
- 普通索引：status, is_deleted

---

## Task 2: 创建 SKU 实体

**Files:**
- Create: `EasyProduct.Models\Entitys\Product\product_sku.cs`

**实体字段：**
- 继承 BaseEntity
- 包含所有业务字段
- 使用 [SugarTable] 和 [SugarColumn] 特性
- 所有属性有完整中文注释

---

## Task 3: 创建 SKU DTO

**Files:**
- Create: `EasyProduct.Models\Dto\Product\Sku\SkuQueryDto.cs`
- Create: `EasyProduct.Models\Dto\Product\Sku\CreateSkuDto.cs`
- Create: `EasyProduct.Models\Dto\Product\Sku\UpdateSkuDto.cs`
- Create: `EasyProduct.Models\Dto\Product\Sku\SkuDto.cs`

**DTO 设计：**
- SkuQueryDto: 查询参数（SkuName, SkuCode, SpuId, Status）
- CreateSkuDto: 创建参数（所有业务字段）
- UpdateSkuDto: 更新参数（+ Status）
- SkuDto: 数据传输对象（+ SpuName）

---

## Task 4: 创建 SKU 服务

**Files:**
- Create: `EasyProduct.Business\Product\ISkuService.cs`
- Create: `EasyProduct.Business\Product\SkuService.cs`

**服务方法：**
- GetSkuPageListAsync - 分页查询
- GetSkuByIdAsync - 获取详情
- GetSkuListBySpuAsync - 按SPU查询
- CreateSkuAsync - 创建
- UpdateSkuAsync - 更新
- DeleteSkuAsync - 删除
- UpdateSkuStatusAsync - 更新状态
- UpdateSkuStockAsync - 更新库存

**业务逻辑：**
- 创建时检查编码唯一性
- 更新时检查编码唯一性（排除自己）
- 创建/更新时验证SPU是否存在
- 更新库存时验证库存不能为负

---

## Task 5: 创建 SKU 控制器

**Files:**
- Create: `EasyProduct.Web\Controllers\Admin\Product\SkuController.cs`

**API 接口：**
- `GET /api/admin/product/sku/list` - 分页查询
- `GET /api/admin/product/sku/{id}` - 获取详情
- `GET /api/admin/product/sku/spu/{spuId}` - 按SPU查询
- `POST /api/admin/product/sku` - 创建
- `PUT /api/admin/product/sku/{id}` - 更新
- `DELETE /api/admin/product/sku/{id}` - 删除
- `PUT /api/admin/product/sku/{id}/status` - 更新状态
- `PUT /api/admin/product/sku/{id}/stock` - 更新库存

---

## 验收标准

- ✅ SKU 表已创建
- ✅ SKU 实体、DTO、服务、控制器已实现
- ✅ 支持分页查询、详情查询、创建、更新、删除
- ✅ 支持库存管理
- ✅ 项目构建成功（0 编译错误）
- ✅ 所有方法添加完整中文注释