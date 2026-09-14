# P4.4 库存管理模块开发计划

> **创建时间：** 2026-09-14
> **依据文档：** mock-server/src/data/crm-inventory.ts
> **开发策略：** 分批次开发，核心功能优先
> **前置依赖：** P4.1 主数据管理已完成、P4.2 销售订单已完成、P4.3 采购订单已完成

---

## 模块概览

### 核心功能
1. **仓库管理** - 仓库 CRUD
2. **库存账面** - 库存查询、调整
3. **出入库流水** - 流水记录
4. **盘点管理** - 盘点单管理
5. **库存预警** - 自动预警

### 数据结构

**5 个主要实体：**
- Warehouse（仓库）
- Stock（库存账面）
- StockRecord（出入库流水）
- StockCheck（盘点单）
- StockAlert（库存预警）

**1 个明细实体：**
- StockCheckItem（盘点明细）

---

## Mockjs 数据结构

### 1. 仓库（Warehouse）
- id, code, name, address, manager, phone
- status: 'active' | 'inactive'
- remark, createdAt, updatedAt

### 2. 库存账面（Stock）
- id, warehouseId, warehouseName, skuCode, skuName, spec, unit
- available, locked, total
- minLimit, maxLimit
- updatedAt

### 3. 出入库流水（StockRecord）
- id, warehouseId, warehouseName, skuCode, skuName, spec, unit
- type: 'in' | 'out'
- sourceType: 'purchase_in' | 'sales_out' | 'check_adjust' | 'reversal_return'
- sourceOrderNo, quantity, operator, remark
- createdAt

### 4. 盘点单（StockCheck）
- id, checkNo, warehouseId, warehouseName, checker, checkDate
- status: 'draft' | 'counting' | 'completed'
- remark, items[], createdAt, updatedAt

### 5. 盘点明细（StockCheckItem）
- id, checkId, skuCode, skuName, spec, unit
- systemQty, countedQty, diff

### 6. 库存预警（StockAlert）
- id, warehouseId, warehouseName, skuCode, skuName, spec
- available, minLimit, maxLimit
- alertType: 'low' | 'high'
- status: 'pending' | 'resolved'
- createdAt, resolvedAt, remark

---

## 开发批次

### 批次 1：仓库管理（优先级最高）
- **预计时间：** 2 小时
- **功能：** 仓库 CRUD、下拉选项
- **API 接口：** 5 个

### 批次 2：库存账面
- **预计时间：** 2 小时
- **功能：** 库存查询、调整
- **API 接口：** 4 个

### 批次 3：出入库流水
- **预计时间：** 2 小时
- **功能：** 流水查询、记录创建
- **API 接口：** 3 个

### 批次 4：盘点管理
- **预计时间：** 3 小时
- **功能：** 盘点单 CRUD、完成盘点
- **API 接口：** 6 个

### 批次 5：库存预警
- **预计时间：** 2 小时
- **功能：** 预警查询、解决预警
- **API 接口：** 4 个

**总计：** 11 小时，22 个 API 接口

---

## 进度总览

| 批次 | 功能 | 状态 | 预计时间 |
|------|------|------|---------|
| 批次 1 | 仓库管理 | ✅ 已完成 | 2 小时 |
| 批次 2 | 库存账面 | 🔵 待开发 | 2 小时 |
| 批次 3 | 出入库流水 | 🔵 待开发 | 2 小时 |
| 批次 4 | 盘点管理 | 🔵 待开发 | 3 小时 |
| 批次 5 | 库存预警 | 🔵 待开发 | 2 小时 |
| **总计** | **库存管理** | 🔄 进行中 | **11 小时** |

---

## 下一步行动

**当前任务：** 批次 2 - 库存账面

**开发顺序：**
1. 创建 Stock 实体类
2. 创建库存管理 DTOs
3. 创建库存服务接口和实现
4. 创建库存控制器
5. 创建数据库脚本
6. 编译验证