# P4.5 发票与财务管理模块开发计划

> **创建时间：** 2026-09-14
> **依据文档：** mock-server/src/data/crm-finance.ts
> **开发策略：** 分批次开发，核心功能优先
> **前置依赖：** P4.1 主数据管理、P4.2 销售订单、P4.3 采购订单已完成

---

## 模块概览

### 核心功能
1. **发票管理** - 销售开票、采购进项、发票作废
2. **收付款管理** - 收款、付款登记、核销
3. **应收应付台账** - 自动台账、账龄分析、结算状态
4. **固定资产管理** - 资产登记、折旧计算、资产报废

### 数据结构

**4 个主要实体：**
- Invoice（发票）
- Payment（收付款）
- Arap（应收应付台账）
- FixedAsset（固定资产）

**1 个明细实体：**
- AssetDepreciation（资产折旧记录）

---

## Mockjs 数据结构

### 1. 发票（Invoice）
- id, invoiceNo, type: 'output' | 'input'
- orderType: 'sales' | 'purchase', orderNo, partyName
- amount, taxRate, taxAmount, total
- issueDate, status: 'draft' | 'issued' | 'voided'
- remark, createdAt, updatedAt

### 2. 收付款（Payment）
- id, paymentNo, type: 'receipt' | 'payment'
- orderType: 'sales' | 'purchase', orderNo, partyName
- amount, method: 'cash' | 'bank' | 'wechat'
- status: 'draft' | 'confirmed' | 'voided'
- remark, createdAt, updatedAt

### 3. 应收应付台账（Arap）
- id, orderType: 'sales' | 'purchase', orderNo, partyName
- receivable, received, balance
- aging: '0-30' | '31-60' | '61-90' | '90+'
- status: 'settled' | 'unsettled', createdAt

### 4. 固定资产（FixedAsset）
- id, assetNo, name, category
- originalValue, purchaseDate
- depreciationMethod: 'straight-line'
- salvageValue, usefulYears, currentValue
- status: 'active' | 'scrapped'
- remark, createdAt, updatedAt

### 5. 资产折旧记录（AssetDepreciation）
- id, assetId, period
- depreciationAmount, accumulatedDepreciation, currentValue
- createdAt

---

## 开发批次

### 批次 1：发票管理（优先级最高）
- **预计时间：** 2 小时
- **功能：** 发票 CRUD、开票、作废
- **API 接口：** 6 个

### 批次 2：收付款管理
- **预计时间：** 2 小时
- **功能：** 收款、付款登记、核销
- **API 接口：** 6 个

### 批次 3：应收应付台账
- **预计时间：** 2 小时
- **功能：** 台账查询、账龄分析
- **API 接口：** 3 个

### 批次 4：固定资产管理
- **预计时间：** 3 小时
- **功能：** 资产登记、折旧计算、报废
- **API 接口：** 6 个

**总计：** 9 小时，21 个 API 接口

---

## 进度总览

| 批次 | 功能 | 状态 | 预计时间 |
|------|------|------|---------|
| 批次 1 | 发票管理 | 🔵 待开发 | 2 小时 |
| 批次 2 | 收付款管理 | 🔵 待开发 | 2 小时 |
| 批次 3 | 应收应付台账 | 🔵 待开发 | 2 小时 |
| 批次 4 | 固定资产管理 | 🔵 待开发 | 3 小时 |
| **总计** | **财务管理** | 🔵 待开发 | **9 小时** |

---

## 下一步行动

**当前任务：** 批次 1 - 发票管理

**开发顺序：**
1. 创建 InvoiceStatus、InvoiceType、InvoiceOrderType 枚举
2. 创建 Invoice 实体类
3. 创建发票管理 DTOs
4. 创建发票服务接口和实现
5. 创建发票控制器
6. 创建数据库脚本
7. 编译验证