# P3.3 订单支付模块开发计划

> **创建时间：** 2026-09-10
> **依据文档：** docs/superpowers/specs/2026-08-08-easyproduct-integration-design.md
> **开发策略：** 4 个子任务交付，逐个验收

---

## 阶段规划

### P3.3.1：数据模型层
- **状态：** ✅ 已完成
- **目标：** 实体类、枚举类、DTOs 定义
- **内容：** 枚举类(6个) + 实体类(4个) + DTOs(~20个)
- **完成时间：** 2026-09-10
- **决策：** GUID 主键，状态字段使用字符串常量

### P3.3.2：业务逻辑层（订单服务）
- **状态：** ✅ 已完成
- **目标：** 订单创建、查询、状态流转、取消
- **完成时间：** 2026-09-10
- **决策：** 使用事务保证库存扣减和订单创建的原子性

### P3.3.3：业务逻辑层（支付服务）
- **状态：** 🔵 待开发
- **目标：** 支付渠道集成、支付状态管理、支付回调
- **预计时间：** 3-4 小时
- **决策：** _

### P3.3.4：业务逻辑层（退款服务）
- **状态：** 🔵 待开发
- **目标：** 退款申请、退款审核、退款状态管理
- **预计时间：** 3-4 小时
- **决策：** _

### P3.3.5：控制器层
- **状态：** 🔵 待开发
- **目标：** 暴露 REST API，对接业务逻辑层
- **预计时间：** 2-3 小时
- **决策：** _

---

## 当前焦点

**正在处理：** P3.3.2 订单服务已完成

**下一步：** P3.3.3 业务逻辑层（支付服务）

---

## P3.3.2 详细任务清单：订单服务

### 开发顺序

```
订单服务接口定义 → 订单服务实现 → 订单状态流转 → 单元测试
```

### 阶段 2.1：订单服务接口定义

#### 2.1.1 订单服务接口
- **状态：** 🔵 未开始
- **文件：** `EasyProduct.WebApi/Services/Mall/IOrderService.cs`
- **任务：**
  - [ ] 定义 IOrderService 接口
  - [ ] 定义 CreateOrderAsync 方法（创建订单）
  - [ ] 定义 GetOrderListAsync 方法（订单列表查询）
  - [ ] 定义 GetOrderDetailAsync 方法（订单详情）
  - [ ] 定义 CancelOrderAsync 方法（取消订单）
  - [ ] 定义 UpdateOrderStatusAsync 方法（更新订单状态）
- **依赖：** P3.3.1
- **预计时间：** 30 分钟

### 阶段 2.2：订单服务实现

#### 2.2.1 订单创建
- **状态：** 🔵 未开始
- **文件：** `EasyProduct.WebApi/Services/Mall/OrderService.cs`
- **任务：**
  - [ ] 实现 CreateOrderAsync 方法
  - [ ] 验证购物车商品
  - [ ] 计算订单金额
  - [ ] 创建订单主表和订单明细表
  - [ ] 扣减库存（调用库存服务）
  - [ ] 清空购物车
- **依赖：** 2.1.1
- **预计时间：** 1.5 小时

#### 2.2.2 订单查询
- **状态：** 🔵 未开始
- **文件：** `EasyProduct.WebApi/Services/Mall/OrderService.cs`
- **任务：**
  - [ ] 实现 GetOrderListAsync 方法
  - [ ] 支持分页查询
  - [ ] 支持多条件筛选（状态、时间范围、会员等）
  - [ ] 实现 GetOrderDetailAsync 方法
  - [ ] 关联查询订单明细、商品信息
- **依赖：** 2.1.1
- **预计时间：** 1 小时

#### 2.2.3 订单状态流转
- **状态：** 🔵 未开始
- **文件：** `EasyProduct.WebApi/Services/Mall/OrderService.cs`
- **任务：**
  - [ ] 实现 UpdateOrderStatusAsync 方法
  - [ ] 定义订单状态流转规则
  - [ ] 实现状态流转验证
  - [ ] 记录订单状态变更日志
- **依赖：** 2.1.1
- **预计时间：** 1 小时

#### 2.2.4 订单取消
- **状态：** 🔵 未开始
- **文件：** `EasyProduct.WebApi/Services/Mall/OrderService.cs`
- **任务：**
  - [ ] 实现 CancelOrderAsync 方法
  - [ ] 验证订单状态是否可取消
  - [ ] 恢复库存
  - [ ] 更新订单状态为"已取消"
  - [ ] 记录取消原因
- **依赖：** 2.2.3
- **预计时间：** 1 小时

### 阶段 2.3：单元测试

#### 2.3.1 订单创建测试
- **状态：** 🔵 未开始
- **文件：** `EasyProduct.WebApi.Tests/Services/Mall/OrderServiceTests.cs`
- **任务：**
  - [ ] 测试正常订单创建
  - [ ] 测试库存不足场景
  - [ ] 测试购物车为空场景
  - [ ] 测试金额计算正确性
- **依赖：** 2.2.1
- **预计时间：** 1 小时

#### 2.3.2 订单状态流转测试
- **状态：** 🔵 未开始
- **文件：** `EasyProduct.WebApi.Tests/Services/Mall/OrderServiceTests.cs`
- **任务：**
  - [ ] 测试正常状态流转
  - [ ] 测试非法状态流转
  - [ ] 测试状态流转日志记录
- **依赖：** 2.2.3
- **预计时间：** 30 分钟

---

## P3.3.3 详细任务清单：支付服务

### 开发顺序

```
支付服务接口定义 → 支付服务实现 → 支付回调 → 单元测试
```

### 阶段 3.1：支付服务接口定义

#### 3.1.1 支付服务接口
- **状态：** 🔵 未开始
- **文件：** `EasyProduct.WebApi/Services/Mall/IPaymentService.cs`
- **任务：**
  - [ ] 定义 IPaymentService 接口
  - [ ] 定义 CreatePaymentAsync 方法（创建支付单）
  - [ ] 定义 GetPaymentListAsync 方法（支付单列表）
  - [ ] 定义 HandlePaymentCallbackAsync 方法（支付回调）
  - [ ] 定义 QueryPaymentStatusAsync 方法（查询支付状态）
- **依赖：** P3.3.1
- **预计时间：** 20 分钟

### 阶段 3.2：支付服务实现

#### 3.2.1 支付单创建
- **状态：** 🔵 未开始
- **文件：** `EasyProduct.WebApi/Services/Mall/PaymentService.cs`
- **任务：**
  - [ ] 实现 CreatePaymentAsync 方法
  - [ ] 验证订单状态
  - [ ] 创建支付单
  - [ ] 调用支付渠道 API（模拟）
  - [ ] 更新订单状态为"待支付"
- **依赖：** 3.1.1
- **预计时间：** 1 小时

#### 3.2.2 支付回调处理
- **状态：** 🔵 未开始
- **文件：** `EasyProduct.WebApi/Services/Mall/PaymentService.cs`
- **任务：**
  - [ ] 实现 HandlePaymentCallbackAsync 方法
  - [ ] 验证回调签名
  - [ ] 更新支付单状态
  - [ ] 更新订单状态为"已支付"
  - [ ] 记录支付流水
- **依赖：** 3.1.1
- **预计时间：** 1.5 小时

#### 3.2.3 支付状态查询
- **状态：** 🔵 未开始
- **文件：** `EasyProduct.WebApi/Services/Mall/PaymentService.cs`
- **任务：**
  - [ ] 实现 QueryPaymentStatusAsync 方法
  - [ ] 调用支付渠道查询接口
  - [ ] 同步支付状态
- **依赖：** 3.1.1
- **预计时间：** 30 分钟

### 阶段 3.3：单元测试

#### 3.3.1 支付流程测试
- **状态：** 🔵 未开始
- **文件：** `EasyProduct.WebApi.Tests/Services/Mall/PaymentServiceTests.cs`
- **任务：**
  - [ ] 测试支付单创建
  - [ ] 测试支付回调处理
  - [ ] 测试支付状态查询
- **依赖：** 3.2.1, 3.2.2, 3.2.3
- **预计时间：** 1 小时

---

## P3.3.4 详细任务清单：退款服务

### 开发顺序

```
退款服务接口定义 → 退款服务实现 → 退款审核 → 单元测试
```

### 阶段 4.1：退款服务接口定义

#### 4.1.1 退款服务接口
- **状态：** 🔵 未开始
- **文件：** `EasyProduct.WebApi/Services/Mall/IRefundService.cs`
- **任务：**
  - [ ] 定义 IRefundService 接口
  - [ ] 定义 CreateRefundAsync 方法（创建退款申请）
  - [ ] 定义 GetRefundListAsync 方法（退款列表查询）
  - [ ] 定义 AuditRefundAsync 方法（退款审核）
  - [ ] 定义 ProcessRefundAsync 方法（执行退款）
- **依赖：** P3.3.1
- **预计时间：** 20 分钟

### 阶段 4.2：退款服务实现

#### 4.2.1 退款申请
- **状态：** 🔵 未开始
- **文件：** `EasyProduct.WebApi/Services/Mall/RefundService.cs`
- **任务：**
  - [ ] 实现 CreateRefundAsync 方法
  - [ ] 验证订单状态
  - [ ] 验证退款金额
  - [ ] 创建退款单
  - [ ] 更新订单状态为"退款中"
- **依赖：** 4.1.1
- **预计时间：** 1 小时

#### 4.2.2 退款审核
- **状态：** 🔵 未开始
- **文件：** `EasyProduct.WebApi/Services/Mall/RefundService.cs`
- **任务：**
  - [ ] 实现 AuditRefundAsync 方法
  - [ ] 验证退款单状态
  - [ ] 审核通过/拒绝逻辑
  - [ ] 记录审核意见
- **依赖：** 4.1.1
- **预计时间：** 1 小时

#### 4.2.3 退款执行
- **状态：** 🔵 未开始
- **文件：** `EasyProduct.WebApi/Services/Mall/RefundService.cs`
- **任务：**
  - [ ] 实现 ProcessRefundAsync 方法
  - [ ] 调用支付渠道退款接口
  - [ ] 更新退款单状态
  - [ ] 更新订单状态为"已退款"
  - [ ] 恢复库存（如果需要）
  - [ ] 记录退款流水
- **依赖：** 4.2.2
- **预计时间：** 1.5 小时

### 阶段 4.3：单元测试

#### 4.3.1 退款流程测试
- **状态：** 🔵 未开始
- **文件：** `EasyProduct.WebApi.Tests/Services/Mall/RefundServiceTests.cs`
- **任务：**
  - [ ] 测试退款申请
  - [ ] 测试退款审核
  - [ ] 测试退款执行
  - [ ] 测试退款金额验证
- **依赖：** 4.2.1, 4.2.2, 4.2.3
- **预计时间：** 1 小时

---

## P3.3.5 详细任务清单：控制器层

### 开发顺序

```
订单控制器 → 支付控制器 → 退款控制器 → API 测试
```

### 阶段 5.1：订单控制器

#### 5.1.1 订单控制器实现
- **状态：** 🔵 未开始
- **文件：** `EasyProduct.WebApi/Controllers/Mall/OrderController.cs`
- **任务：**
  - [ ] 创建 OrderController
  - [ ] 实现 POST /api/app/mall/order 接口（创建订单）
  - [ ] 实现 GET /api/app/mall/order/list 接口（订单列表）
  - [ ] 实现 GET /api/app/mall/order/:id 接口（订单详情）
  - [ ] 实现 POST /api/app/mall/order/:id/cancel 接口（取消订单）
  - [ ] 实现 PUT /api/app/mall/order/:id/status 接口（更新状态）
  - [ ] 添加权限控制特性
- **依赖：** P3.3.2
- **预计时间：** 1 小时

### 阶段 5.2：支付控制器

#### 5.2.1 支付控制器实现
- **状态：** 🔵 未开始
- **文件：** `EasyProduct.WebApi/Controllers/Mall/PaymentController.cs`
- **任务：**
  - [ ] 创建 PaymentController
  - [ ] 实现 POST /api/app/mall/payment 接口（创建支付）
  - [ ] 实现 GET /api/app/mall/payment/list 接口（支付列表）
  - [ ] 实现 POST /api/app/mall/payment/callback 接口（支付回调）
  - [ ] 实现 GET /api/app/mall/payment/:id/status 接口（查询状态）
  - [ ] 添加权限控制特性
- **依赖：** P3.3.3
- **预计时间：** 45 分钟

### 阶段 5.3：退款控制器

#### 5.3.1 退款控制器实现
- **状态：** 🔵 未开始
- **文件：** `EasyProduct.WebApi/Controllers/Mall/RefundController.cs`
- **任务：**
  - [ ] 创建 RefundController
  - [ ] 实现 POST /api/app/mall/refund 接口（申请退款）
  - [ ] 实现 GET /api/app/mall/refund/list 接口（退款列表）
  - [ ] 实现 POST /api/app/mall/refund/:id/audit 接口（审核退款）
  - [ ] 实现 POST /api/app/mall/refund/:id/process 接口（执行退款）
  - [ ] 添加权限控制特性
- **依赖：** P3.3.4
- **预计时间：** 45 分钟

### 阶段 5.4：API 测试

#### 5.4.1 接口联调测试
- **状态：** 🔵 未开始
- **任务：**
  - [ ] 测试订单创建流程
  - [ ] 测试支付流程
  - [ ] 测试退款流程
  - [ ] 测试异常场景
- **依赖：** 5.1.1, 5.2.1, 5.3.1
- **预计时间：** 1 小时

---

## 进度总览

| 阶段 | 状态 | 任务数 | 预计时间 | 完成度 | 最后更新 |
|------|------|--------|---------|--------|---------|
| P3.3.1 数据模型层 | ✅ 已完成 | ~30 个 | 3 小时 | 100% | 2026-09-10 |
| P3.3.2 订单服务 | ✅ 已完成 | 17 个 | 5.5 小时 | 100% | 2026-09-10 |
| P3.3.3 支付服务 | 🔵 待开发 | 12 个 | 4 小时 | 0% | - |
| P3.3.4 退款服务 | 🔵 待开发 | 12 个 | 4.5 小时 | 0% | - |
| P3.3.5 控制器层 | 🔵 待开发 | 13 个 | 3.5 小时 | 0% | - |
| **总计** | 🔄 进行中 | **~84 个** | **20.5 小时** | **33%** | 2026-09-10 |

---

## 关键决策

### P3.3.1 数据模型层

1. **GUID 主键**：所有表使用 GUID 作为主键，类型为 string
2. **状态字段**：使用字符串常量（如 "pending", "paid"），便于扩展和阅读
3. **时间格式**：使用 ISO 8601 格式
4. **金额字段**：使用 decimal 类型，精度为 18,2

---

## 风险与阻塞

_暂无_

---

## 下一步行动

**当前任务：** P3.3.2 业务逻辑层（订单服务）

**建议开发顺序：**
1. 定义订单服务接口（IOrderService）
2. 实现订单创建逻辑
3. 实现订单查询逻辑
4. 实现订单状态流转
5. 实现订单取消
6. 编写单元测试