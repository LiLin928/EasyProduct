# P3.3 订单支付模块开发发现

> **创建时间：** 2026-09-10

---

## 设计文档分析

**来源：** docs/superpowers/specs/2026-08-08-easyproduct-integration-design.md

### 核心发现

1. **模块定位**
   - P3.3 订单支付模块是 Mall（商城）模块的核心
   - 包含：订单、支付、退款三大子系统
   - 数据模型层已完成（P3.3.1）

2. **技术架构**
   - 后端：.NET 8 + SqlSugar + Autofac
   - 分层：Controller → Service → Repository
   - 数据库：MySQL 8 单库，表前缀 mall_

3. **业务流程**
   - 订单流程：创建 → 支付 → 发货 → 完成
   - 支付流程：创建支付单 → 调用支付渠道 → 回调处理
   - 退款流程：申请 → 审核 → 执行退款

4. **数据模型**
   - 枚举类：OrderStatus, PaymentMethod, PaymentStatus, RefundStatus, RefundType, AuditStatus
   - 实体类：Order, OrderItem, Payment, Refund
   - DTOs：查询、创建、更新相关 DTO

---

## API 契约摘要

### 订单相关 API

**创建订单：**
- `POST /api/app/mall/order`
- 请求体：CreateOrderDto
- 响应：OrderDto

**订单列表：**
- `GET /api/app/mall/order/list`
- 查询参数：OrderQueryDto
- 响应：PageResult<OrderDto>

**订单详情：**
- `GET /api/app/mall/order/:id`
- 响应：OrderDetailDto

**取消订单：**
- `POST /api/app/mall/order/:id/cancel`
- 请求体：CancelOrderDto
- 响应：OrderDto

---

### 支付相关 API

**创建支付：**
- `POST /api/app/mall/payment`
- 请求体：CreatePaymentDto
- 响应：PaymentDto

**支付列表：**
- `GET /api/app/mall/payment/list`
- 查询参数：PaymentQueryDto
- 响应：PageResult<PaymentDto>

**支付回调：**
- `POST /api/app/mall/payment/callback`
- 请求体：PaymentCallbackDto
- 响应：PaymentDto

**查询支付状态：**
- `GET /api/app/mall/payment/:id/status`
- 响应：PaymentStatusDto

---

### 退款相关 API

**申请退款：**
- `POST /api/app/mall/refund`
- 请求体：CreateRefundDto
- 响应：RefundDto

**退款列表：**
- `GET /api/app/mall/refund/list`
- 查询参数：RefundQueryDto
- 响应：PageResult<RefundDto>

**审核退款：**
- `POST /api/app/mall/refund/:id/audit`
- 请求体：AuditRefundDto
- 响应：RefundDto

**执行退款：**
- `POST /api/app/mall/refund/:id/process`
- 响应：RefundDto

---

## 技术依赖关系

```
P3.3.1 数据模型层（已完成）
    ↓
P3.3.2 订单服务
    ├── 依赖：购物车服务（P3.2 已完成）
    ├── 依赖：库存服务
    └── 依赖：订单数据模型
    ↓
P3.3.3 支付服务
    ├── 依赖：订单服务
    └── 依赖：支付数据模型
    ↓
P3.3.4 退款服务
    ├── 依赖：订单服务
    ├── 依赖：支付服务
    └── 依赖：退款数据模型
    ↓
P3.3.5 控制器层
    ├── 依赖：订单服务
    ├── 依赖：支付服务
    └── 依赖：退款服务
```

---

## 业务规则发现

### 订单状态流转

```
pending（待支付）
    ↓ 支付成功
paid（已支付）
    ↓ 发货
shipped（已发货）
    ↓ 确认收货
completed（已完成）

任意状态 → canceled（已取消）
paid → refunding（退款中）→ refunded（已退款）
```

### 支付状态流转

```
pending（待支付）
    ↓ 支付成功
success（支付成功）
    ↓ 退款
refunded（已退款）

pending → failed（支付失败）
```

### 退款状态流转

```
pending（待审核）
    ↓ 审核通过
approved（审核通过）
    ↓ 执行退款
processing（退款中）
    ↓ 退款成功
success（退款成功）

pending → rejected（审核拒绝）
processing → failed（退款失败）
```

---

## 关键业务逻辑

### 订单创建流程

1. 验证购物车商品（存在性、库存、价格）
2. 计算订单金额（商品金额 + 运费 - 优惠）
3. 创建订单主表（状态：pending）
4. 创建订单明细表
5. 扣减库存
6. 清空购物车
7. 返回订单信息

### 支付流程

1. 验证订单状态（必须是 pending）
2. 创建支付单（状态：pending）
3. 调用支付渠道 API（模拟）
4. 返回支付参数
5. 接收支付回调
6. 验证签名
7. 更新支付单状态（success）
8. 更新订单状态（paid）
9. 记录支付流水

### 退款流程

1. 验证订单状态（必须是 paid）
2. 验证退款金额（≤ 订单已支付金额）
3. 创建退款单（状态：pending）
4. 更新订单状态（refunding）
5. 管理员审核
6. 审核通过后调用支付渠道退款接口
7. 更新退款单状态（success）
8. 更新订单状态（refunded）
9. 恢复库存（如果需要）
10. 记录退款流水

---

## 测试要点

### 订单服务测试

1. **正常流程**
   - 创建订单成功
   - 订单金额计算正确
   - 库存扣减正确

2. **异常场景**
   - 购物车为空
   - 商品不存在
   - 库存不足
   - 价格不匹配

3. **并发场景**
   - 多用户同时下单
   - 库存并发扣减

### 支付服务测试

1. **正常流程**
   - 创建支付单成功
   - 支付回调处理成功
   - 订单状态更新正确

2. **异常场景**
   - 订单状态不正确
   - 支付渠道异常
   - 重复回调

### 退款服务测试

1. **正常流程**
   - 退款申请成功
   - 退款审核通过
   - 退款执行成功

2. **异常场景**
   - 退款金额超限
   - 订单状态不正确
   - 退款渠道异常

---

## 技术债务

_暂无_

---

## 风险提示

1. **库存并发**
   - 多用户同时下单可能导致库存超卖
   - 建议：使用数据库事务 + 乐观锁

2. **支付回调幂等**
   - 支付渠道可能重复发送回调
   - 建议：使用支付单号作为幂等键

3. **退款金额计算**
   - 退款金额可能超过订单已支付金额
   - 建议：在申请退款时验证金额上限

---

## 后续优化方向

1. **订单超时自动取消**
   - 使用定时任务（Quartz）
   - 超过 30 分钟未支付自动取消

2. **支付渠道扩展**
   - 当前模拟支付
   - 后续接入真实支付渠道（微信、支付宝）

3. **订单日志记录**
   - 记录订单状态变更历史
   - 便于问题排查和审计

---

## 参考资料

- EasyProduct 集成设计文档：`docs/superpowers/specs/2026-08-08-easyproduct-integration-design.md`
- 后端开发规范：`docs/backend-guidelines.md`
- API 路由规范：`CLAUDE.md`