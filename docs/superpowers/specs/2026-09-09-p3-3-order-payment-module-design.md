# P3.3 订单支付模块设计规范

> **日期：** 2026-09-09
> **状态：** 已评审通过
> **范围：** 小程序商城订单（B2C）

---

## 1. 业务背景

### 1.1 项目背景

EasyProduct 是整合了 EasyWebSite（官网）、EasyProject（电商管理）、EasyCRM（ERP/CRM 前端）的模块化单体项目。P3.3 订单支付模块属于 P3 商城线，主要服务于小程序商城的订单业务。

### 1.2 模块范围

**业务范围：**
- ✅ 小程序商城订单（B2C，会员下单）
- ❌ CRM 销售订单（B2B，后台为客户下单，属于 P4 CRM 线）

**核心功能：**
- 订单创建（购物车下单 + 直接购买）
- 订单支付（微信支付 + 余额支付）
- 订单发货与物流管理
- 订单收货与完成
- 订单取消
- 退款服务（仅退款 + 退货退款）

---

## 2. 整体架构设计

### 2.1 架构分层

```
┌─────────────────────────────────────────┐
│  EasyProduct.WebApi (Controllers)       │
│  ├─ App/Mall/OrderController.cs         │ 小程序端
│  ├─ App/Mall/RefundController.cs        │ 小程序端
│  ├─ Admin/Mall/OrderController.cs       │ 管理端
│  └─ Admin/Mall/RefundController.cs      │ 管理端
└─────────────────────────────────────────┘
                    ↓
┌─────────────────────────────────────────┐
│  EasyProduct.Business (Services)        │
│  ├─ IOrderService.cs                    │ 订单服务接口
│  ├─ OrderService.cs                     │ 订单服务实现
│  ├─ IRefundService.cs                   │ 退款服务接口
│  ├─ RefundService.cs                    │ 退款服务实现
│  ├─ IPaymentService.cs                  │ 支付服务接口
│  └─ PaymentService.cs                   │ 支付服务实现
└─────────────────────────────────────────┘
                    ↓
┌─────────────────────────────────────────┐
│  EasyProduct.Models (Entity + DTO)      │
│  ├─ Entitys/Mall/                       │
│  │  ├─ Order.cs                         │ 订单实体
│  │  ├─ OrderItem.cs                     │ 订单项实体
│  │  ├─ Payment.cs                       │ 支付记录实体
│  │  └─ Refund.cs                        │ 退款记录实体
│  ├─ Dto/Mall/Order/                     │ 订单相关 DTO
│  ├─ Dto/Mall/Payment/                   │ 支付相关 DTO
│  └─ Enums/Mall/                         │ 枚举
└─────────────────────────────────────────┘
```

### 2.2 模块职责

**Controller 层：**
- 接收请求、参数验证
- 调用 Service 处理业务
- 返回统一响应格式

**Service 层：**
- 核心业务逻辑
- 事务管理
- 跨模块协调（库存、优惠券、会员）

**Model 层：**
- 数据模型定义
- 数据传输对象
- 枚举和常量

---

## 3. 数据模型设计

### 3.1 数据表设计

#### 3.1.1 mall_order（订单主表）

| 字段名 | 类型 | 说明 |
|--------|------|------|
| id | VARCHAR(36) | 主键GUID |
| order_no | VARCHAR(50) | 订单编号（唯一） |
| member_id | VARCHAR(36) | 会员ID |
| **收货信息** | | |
| receiver_name | VARCHAR(50) | 收货人姓名 |
| receiver_phone | VARCHAR(20) | 收货人电话 |
| receiver_address | VARCHAR(200) | 收货地址 |
| **订单金额** | | |
| total_amount | DECIMAL(18,2) | 订单总金额 |
| pay_amount | DECIMAL(18,2) | 实付金额 |
| freight_amount | DECIMAL(18,2) | 运费 |
| discount_amount | DECIMAL(18,2) | 优惠金额 |
| **状态与时间** | | |
| status | INT | 订单状态（枚举） |
| payment_time | DATETIME | 支付时间 |
| delivery_time | DATETIME | 发货时间 |
| receive_time | DATETIME | 收货时间 |
| complete_time | DATETIME | 完成时间 |
| cancel_time | DATETIME | 取消时间 |
| **物流信息** | | |
| logistics_company | VARCHAR(50) | 物流公司 |
| logistics_no | VARCHAR(50) | 物流单号 |
| **优惠信息** | | |
| coupon_id | VARCHAR(36) | 优惠券ID |
| coupon_name | VARCHAR(100) | 优惠券名称 |
| **其他** | | |
| remark | VARCHAR(500) | 会员备注 |
| admin_remark | VARCHAR(500) | 后台备注 |
| source | VARCHAR(20) | 订单来源（cart/direct） |
| create_time | DATETIME | 创建时间 |
| update_time | DATETIME | 更新时间 |
| create_by | VARCHAR(50) | 创建人 |
| update_by | VARCHAR(50) | 更新人 |
| is_deleted | INT | 软删除标记 |

**索引：**
- `uk_order_no` (order_no) - 唯一索引
- `idx_member_id` (member_id)
- `idx_status` (status)
- `idx_create_time` (create_time)

#### 3.1.2 mall_order_item（订单项表）

| 字段名 | 类型 | 说明 |
|--------|------|------|
| id | VARCHAR(36) | 主键GUID |
| order_id | VARCHAR(36) | 订单ID |
| sku_id | VARCHAR(36) | SKU ID |
| **商品快照** | | |
| product_name | VARCHAR(200) | 商品名称 |
| sku_name | VARCHAR(200) | SKU名称 |
| sku_spec | VARCHAR(500) | SKU规格（JSON） |
| product_image | VARCHAR(500) | 商品图片 |
| **价格与数量** | | |
| price | DECIMAL(18,2) | 单价 |
| quantity | INT | 数量 |
| subtotal | DECIMAL(18,2) | 小计 |
| **状态** | | |
| is_refunded | INT | 是否已退款（0否1是） |
| refund_quantity | INT | 已退款数量 |

**索引：**
- `idx_order_id` (order_id)
- `idx_sku_id` (sku_id)

#### 3.1.3 mall_payment（支付记录表）

| 字段名 | 类型 | 说明 |
|--------|------|------|
| id | VARCHAR(36) | 主键GUID |
| payment_no | VARCHAR(50) | 支付单号（唯一） |
| order_id | VARCHAR(36) | 订单ID |
| member_id | VARCHAR(36) | 会员ID |
| **支付信息** | | |
| amount | DECIMAL(18,2) | 支付金额 |
| payment_method | INT | 支付方式（枚举） |
| payment_channel | VARCHAR(20) | 支付渠道 |
| status | INT | 支付状态（枚举） |
| **第三方信息** | | |
| third_party_no | VARCHAR(100) | 第三方交易号 |
| **时间** | | |
| payment_time | DATETIME | 支付时间 |
| create_time | DATETIME | 创建时间 |

**索引：**
- `uk_payment_no` (payment_no) - 唯一索引
- `idx_order_id` (order_id)

#### 3.1.4 mall_refund（退款记录表）

| 字段名 | 类型 | 说明 |
|--------|------|------|
| id | VARCHAR(36) | 主键GUID |
| refund_no | VARCHAR(50) | 退款单号（唯一） |
| order_id | VARCHAR(36) | 订单ID |
| order_item_id | VARCHAR(36) | 订单项ID |
| member_id | VARCHAR(36) | 会员ID |
| **退款信息** | | |
| refund_type | INT | 退款类型（枚举：仅退款/退货退款） |
| refund_amount | DECIMAL(18,2) | 退款金额 |
| refund_quantity | INT | 退款数量 |
| refund_reason | VARCHAR(500) | 退款原因 |
| status | INT | 退款状态（枚举） |
| **物流信息（退货退款）** | | |
| logistics_company | VARCHAR(50) | 物流公司 |
| logistics_no | VARCHAR(50) | 物流单号 |
| **处理信息** | | |
| audit_status | INT | 审核状态（枚举） |
| audit_remark | VARCHAR(500) | 审核备注 |
| audit_time | DATETIME | 审核时间 |
| refund_time | DATETIME | 退款时间 |
| **时间** | | |
| create_time | DATETIME | 创建时间 |
| update_time | DATETIME | 更新时间 |

**索引：**
- `uk_refund_no` (refund_no) - 唯一索引
- `idx_order_id` (order_id)
- `idx_status` (status)

### 3.2 枚举定义

#### 3.2.1 OrderStatus（订单状态）

```csharp
public enum OrderStatus
{
    /// <summary>
    /// 待付款
    /// </summary>
    PendingPayment = 0,

    /// <summary>
    /// 待发货
    /// </summary>
    PendingDelivery = 1,

    /// <summary>
    /// 待收货
    /// </summary>
    PendingReceive = 2,

    /// <summary>
    /// 已完成
    /// </summary>
    Completed = 3,

    /// <summary>
    /// 已取消
    /// </summary>
    Cancelled = 4,

    /// <summary>
    /// 退款中
    /// </summary>
    Refunding = 5,

    /// <summary>
    /// 已退款
    /// </summary>
    Refunded = 6
}
```

#### 3.2.2 PaymentMethod（支付方式）

```csharp
public enum PaymentMethod
{
    /// <summary>
    /// 微信支付
    /// </summary>
    WechatPay = 1,

    /// <summary>
    /// 余额支付
    /// </summary>
    Balance = 2
}
```

#### 3.2.3 PaymentStatus（支付状态）

```csharp
public enum PaymentStatus
{
    /// <summary>
    /// 待支付
    /// </summary>
    Pending = 0,

    /// <summary>
    /// 支付成功
    /// </summary>
    Success = 1,

    /// <summary>
    /// 支付失败
    /// </summary>
    Failed = 2,

    /// <summary>
    /// 已退款
    /// </summary>
    Refunded = 3
}
```

#### 3.2.4 RefundType（退款类型）

```csharp
public enum RefundType
{
    /// <summary>
    /// 仅退款
    /// </summary>
    RefundOnly = 1,

    /// <summary>
    /// 退货退款
    /// </summary>
    ReturnAndRefund = 2
}
```

#### 3.2.5 RefundStatus（退款状态）

```csharp
public enum RefundStatus
{
    /// <summary>
    /// 待审核
    /// </summary>
    Pending = 0,

    /// <summary>
    /// 已通过
    /// </summary>
    Approved = 1,

    /// <summary>
    /// 已拒绝
    /// </summary>
    Rejected = 2,

    /// <summary>
    /// 退货中
    /// </summary>
    Returning = 3,

    /// <summary>
    /// 退款中
    /// </summary>
    Refunding = 4,

    /// <summary>
    /// 已完成
    /// </summary>
    Completed = 5,

    /// <summary>
    /// 已取消
    /// </summary>
    Cancelled = 6
}
```

---

## 4. 业务流程设计

### 4.1 订单创建流程

```
会员下单（购物车/直接购买）
    ↓
验证商品信息（SKU是否存在、是否启用）
    ↓
验证库存是否充足
    ↓
验证优惠券是否可用（如果有）
    ↓
计算订单金额（商品金额 + 运费 - 优惠金额）
    ↓
创建订单主表 + 订单项表
    ↓
扣减库存（预扣库存）
    ↓
使用优惠券（标记为已使用）
    ↓
返回订单信息（订单号、金额）
```

**关键点：**
- 下单即扣减库存，避免超卖
- 订单创建后状态为 `PendingPayment`（待付款）
- 未支付订单超时自动取消（建议 30 分钟）
- 库存使用预扣机制，订单取消后释放

### 4.2 支付流程

#### 4.2.1 微信支付流程

```
会员选择微信支付
    ↓
创建支付记录（status=Pending）
    ↓
调用微信支付 API（统一下单）
    ↓
返回支付参数给小程序
    ↓
小程序调用 wx.requestPayment
    ↓
支付成功回调
    ↓
更新支付记录（status=Success）
    ↓
更新订单状态（PendingDelivery）
    ↓
记录支付时间
```

#### 4.2.2 余额支付流程

```
会员选择余额支付
    ↓
验证会员余额是否充足
    ↓
创建支付记录（status=Pending）
    ↓
扣减会员余额
    ↓
更新支付记录（status=Success）
    ↓
更新订单状态（PendingDelivery）
    ↓
记录支付时间
```

**关键点：**
- 支付成功后触发库存正式扣减（预扣 → 正式扣减）
- 支付记录与订单一对一关联
- 支付失败不改变订单状态

### 4.3 发货流程

```
管理员填写物流信息（公司 + 单号）
    ↓
更新订单物流信息
    ↓
更新订单状态（PendingReceive）
    ↓
记录发货时间
    ↓
通知会员（可选）
```

**关键点：**
- 发货前需验证订单状态是否为 `PendingDelivery`
- 发货后状态变为 `PendingReceive`（待收货）
- 可选：集成短信/微信消息通知

### 4.4 会员收货流程

```
会员确认收货
    ↓
更新订单状态（Completed）
    ↓
记录收货时间
    ↓
更新订单完成时间
```

**关键点：**
- 收货前需验证订单状态是否为 `PendingReceive`
- 支持自动确认收货（发货后 7 天自动确认，可选）

### 4.5 订单取消流程

```
会员/管理员取消订单
    ↓
验证订单状态是否允许取消
    ↓
释放库存（预扣库存返还）
    ↓
退还优惠券（如果已使用）
    ↓
如果已支付，创建退款记录
    ↓
更新订单状态（Cancelled）
    ↓
记录取消时间
```

**关键点：**
- 仅 `PendingPayment` 状态会员可取消
- `PendingDelivery` 状态需管理员取消
- 已发货订单不可取消，需走退款流程

### 4.6 退款流程

#### 4.6.1 仅退款流程

```
会员申请退款（选择仅退款）
    ↓
创建退款记录（status=Pending, type=RefundOnly）
    ↓
更新订单状态（Refunding）
    ↓
管理员审核
    ↓
审核通过 → 调用支付平台退款接口
    ↓
退款成功 → 更新退款记录（status=Completed）
    ↓
更新订单状态（Refunded）
    ↓
退还库存（退款数量返还）
```

#### 4.6.2 退货退款流程

```
会员申请退款（选择退货退款）
    ↓
创建退款记录（status=Pending, type=ReturnAndRefund）
    ↓
更新订单状态（Refunding）
    ↓
管理员审核
    ↓
审核通过 → 会员填写物流信息
    ↓
更新退款记录（status=Returning）
    ↓
管理员验收商品
    ↓
验收通过 → 调用支付平台退款接口
    ↓
退款成功 → 更新退款记录（status=Completed）
    ↓
更新订单状态（Refunded）
    ↓
退还库存
```

**关键点：**
- 仅 `PendingDelivery`/`PendingReceive`/`Completed` 状态可申请退款
- 退款金额 ≤ 订单项实付金额
- 部分退款支持（可退部分数量）
- 微信支付退款需调用微信退款 API
- 余额支付退款直接返还余额

### 4.7 订单状态流转图

```
PendingPayment (待付款)
    ├─ 支付成功 → PendingDelivery (待发货)
    ├─ 取消订单 → Cancelled (已取消)
    └─ 超时未付 → Cancelled (已取消)

PendingDelivery (待发货)
    ├─ 发货 → PendingReceive (待收货)
    └─ 取消订单 → Cancelled (已取消)

PendingReceive (待收货)
    ├─ 确认收货 → Completed (已完成)
    └─ 申请退款 → Refunding (退款中)

Completed (已完成)
    └─ 申请退款 → Refunding (退款中)

Refunding (退款中)
    └─ 退款完成 → Refunded (已退款)
```

---

## 5. API 设计

### 5.1 小程序端 API（会员端）

**路由前缀：** `/api/app/mall/order`

#### 5.1.1 订单创建

**从购物车下单：**
```
POST /api/app/mall/order/cart

Request:
{
  "cartIds": ["guid1", "guid2"],        // 购物车项ID列表
  "receiverName": "张三",
  "receiverPhone": "13800138000",
  "receiverAddress": "北京市朝阳区xxx",
  "couponId": "guid",                  // 可选
  "remark": "备注信息"                  // 可选
}

Response:
{
  "code": 200,
  "message": "下单成功",
  "data": {
    "orderId": "guid",
    "orderNo": "2026090912345678",
    "totalAmount": 199.00,
    "payAmount": 179.00
  }
}
```

**直接购买：**
```
POST /api/app/mall/order/direct

Request:
{
  "skuId": "guid",                     // SKU ID
  "quantity": 2,
  "receiverName": "张三",
  "receiverPhone": "13800138000",
  "receiverAddress": "北京市朝阳区xxx",
  "couponId": "guid",                  // 可选
  "remark": "备注信息"                  // 可选
}

Response:
{
  "code": 200,
  "message": "下单成功",
  "data": {
    "orderId": "guid",
    "orderNo": "2026090912345678",
    "totalAmount": 199.00,
    "payAmount": 179.00
  }
}
```

#### 5.1.2 订单列表

```
GET /api/app/mall/order/list?status=1&pageIndex=1&pageSize=10

Response:
{
  "code": 200,
  "message": "查询成功",
  "data": {
    "list": [
      {
        "id": "guid",
        "orderNo": "2026090912345678",
        "status": 1,
        "statusText": "待发货",
        "totalAmount": 199.00,
        "payAmount": 179.00,
        "itemCount": 2,
        "items": [
          {
            "productName": "商品名称",
            "skuSpec": "规格",
            "productImage": "url",
            "price": 99.50,
            "quantity": 2
          }
        ],
        "createTime": "2026-09-09 10:00:00"
      }
    ],
    "total": 15
  }
}
```

#### 5.1.3 订单详情

```
GET /api/app/mall/order/{id}

Response:
{
  "code": 200,
  "message": "查询成功",
  "data": {
    "id": "guid",
    "orderNo": "2026090912345678",
    "status": 1,
    "statusText": "待发货",
    "receiverName": "张三",
    "receiverPhone": "13800138000",
    "receiverAddress": "北京市朝阳区xxx",
    "totalAmount": 199.00,
    "freightAmount": 0.00,
    "discountAmount": 20.00,
    "payAmount": 179.00,
    "couponName": "满100减20",
    "logisticsCompany": null,
    "logisticsNo": null,
    "remark": "备注",
    "items": [...],
    "paymentTime": "2026-09-09 10:05:00",
    "createTime": "2026-09-09 10:00:00"
  }
}
```

#### 5.1.4 订单取消

```
POST /api/app/mall/order/{id}/cancel

Request:
{
  "cancelReason": "不想买了"             // 可选
}

Response:
{
  "code": 200,
  "message": "订单已取消"
}
```

#### 5.1.5 确认收货

```
POST /api/app/mall/order/{id}/receive

Response:
{
  "code": 200,
  "message": "已确认收货"
}
```

#### 5.1.6 申请退款

```
POST /api/app/mall/order/refund

Request:
{
  "orderId": "guid",
  "orderItemId": "guid",               // 订单项ID
  "refundType": 1,                      // 1=仅退款，2=退货退款
  "refundQuantity": 1,                  // 退款数量
  "refundAmount": 89.50,
  "refundReason": "商品有质量问题"
}

Response:
{
  "code": 200,
  "message": "退款申请已提交",
  "data": {
    "refundId": "guid",
    "refundNo": "RF2026090912345678"
  }
}
```

#### 5.1.7 退款列表

```
GET /api/app/mall/order/refund/list?pageIndex=1&pageSize=10

Response:
{
  "code": 200,
  "message": "查询成功",
  "data": {
    "list": [
      {
        "id": "guid",
        "refundNo": "RF2026090912345678",
        "orderNo": "2026090912345678",
        "refundType": 1,
        "refundTypeText": "仅退款",
        "refundAmount": 89.50,
        "status": 0,
        "statusText": "待审核",
        "createTime": "2026-09-09 10:00:00"
      }
    ],
    "total": 5
  }
}
```

#### 5.1.8 退款详情

```
GET /api/app/mall/order/refund/{id}

Response:
{
  "code": 200,
  "message": "查询成功",
  "data": {
    "id": "guid",
    "refundNo": "RF2026090912345678",
    "orderNo": "2026090912345678",
    "refundType": 1,
    "refundTypeText": "仅退款",
    "refundQuantity": 1,
    "refundAmount": 89.50,
    "refundReason": "商品有质量问题",
    "status": 0,
    "statusText": "待审核",
    "auditRemark": null,
    "logisticsCompany": null,
    "logisticsNo": null,
    "orderItem": {...},
    "createTime": "2026-09-09 10:00:00"
  }
}
```

#### 5.1.9 填写退货物流信息

```
POST /api/app/mall/order/refund/{id}/logistics

Request:
{
  "logisticsCompany": "顺丰速运",
  "logisticsNo": "SF1234567890"
}

Response:
{
  "code": 200,
  "message": "物流信息已提交"
}
```

#### 5.1.10 支付

**微信支付：**
```
POST /api/app/mall/order/{id}/pay/wechat

Response:
{
  "code": 200,
  "message": "支付参数获取成功",
  "data": {
    "timeStamp": "1234567890",
    "nonceStr": "xxx",
    "package": "prepay_id=xxx",
    "signType": "RSA",
    "paySign": "xxx"
  }
}
```

**余额支付：**
```
POST /api/app/mall/order/{id}/pay/balance

Response:
{
  "code": 200,
  "message": "支付成功"
}
```

### 5.2 管理端 API

**路由前缀：** `/api/admin/mall/order`

#### 5.2.1 订单列表

```
GET /api/admin/mall/order/list?orderNo=xxx&status=1&memberId=guid&pageIndex=1&pageSize=10

Response:
{
  "code": 200,
  "message": "查询成功",
  "data": {
    "list": [
      {
        "id": "guid",
        "orderNo": "2026090912345678",
        "memberName": "张三",
        "memberPhone": "13800138000",
        "status": 1,
        "statusText": "待发货",
        "totalAmount": 199.00,
        "payAmount": 179.00,
        "itemCount": 2,
        "createTime": "2026-09-09 10:00:00"
      }
    ],
    "total": 100
  }
}
```

#### 5.2.2 订单详情

```
GET /api/admin/mall/order/{id}

Response: 同小程序端订单详情
```

#### 5.2.3 订单发货

```
POST /api/admin/mall/order/{id}/deliver

Request:
{
  "logisticsCompany": "顺丰速运",
  "logisticsNo": "SF1234567890"
}

Response:
{
  "code": 200,
  "message": "发货成功"
}
```

#### 5.2.4 订单备注

```
POST /api/admin/mall/order/{id}/remark

Request:
{
  "adminRemark": "备注内容"
}

Response:
{
  "code": 200,
  "message": "备注成功"
}
```

#### 5.2.5 订单取消

```
POST /api/admin/mall/order/{id}/cancel

Request:
{
  "cancelReason": "缺货"
}

Response:
{
  "code": 200,
  "message": "订单已取消"
}
```

#### 5.2.6 退款列表

```
GET /api/admin/mall/order/refund/list?orderNo=xxx&status=0&pageIndex=1&pageSize=10

Response: 同小程序端退款列表
```

#### 5.2.7 退款详情

```
GET /api/admin/mall/order/refund/{id}

Response: 同小程序端退款详情
```

#### 5.2.8 退款审核

```
POST /api/admin/mall/order/refund/{id}/audit

Request:
{
  "approved": true,                     // true=通过，false=拒绝
  "auditRemark": "审核备注"
}

Response:
{
  "code": 200,
  "message": "审核成功"
}
```

#### 5.2.9 验收商品（退货退款）

```
POST /api/admin/mall/order/refund/{id}/inspect

Request:
{
  "passed": true,                       // true=验收通过，false=验收不通过
  "inspectRemark": "验收备注"
}

Response:
{
  "code": 200,
  "message": "验收完成"
}
```

#### 5.2.10 订单统计

```
GET /api/admin/mall/order/statistics

Response:
{
  "code": 200,
  "message": "查询成功",
  "data": {
    "totalOrders": 1000,
    "pendingPayment": 50,
    "pendingDelivery": 30,
    "pendingReceive": 20,
    "completed": 850,
    "cancelled": 30,
    "refunding": 20,
    "totalAmount": 100000.00,
    "todayOrders": 15,
    "todayAmount": 3000.00
  }
}
```

---

## 6. 错误处理与测试

### 6.1 错误处理

#### 6.1.1 业务异常定义

```csharp
// 订单相关异常
public class OrderException : BusinessException
{
    public OrderException(string message) : base(message) { }
}

// 支付相关异常
public class PaymentException : BusinessException
{
    public PaymentException(string message) : base(message) { }
}

// 退款相关异常
public class RefundException : BusinessException
{
    public RefundException(string message) : base(message) { }
}
```

#### 6.1.2 业务规则验证

**订单创建验证：**
- SKU 不存在或已禁用 → 抛出 `OrderException("商品不存在或已下架")`
- 库存不足 → 抛出 `OrderException("商品库存不足")`
- 优惠券不可用 → 抛出 `OrderException("优惠券不可用")`
- 购物车为空 → 抛出 `OrderException("购物车为空")`

**支付验证：**
- 订单状态不正确 → 抛出 `PaymentException("订单状态不允许支付")`
- 余额不足 → 抛出 `PaymentException("会员余额不足")`
- 支付超时 → 抛出 `PaymentException("订单已超时")`

**退款验证：**
- 订单状态不允许退款 → 抛出 `RefundException("订单状态不允许退款")`
- 退款金额超过订单金额 → 抛出 `RefundException("退款金额超出限制")`
- 已存在退款记录 → 抛出 `RefundException("该商品已申请退款")`

#### 6.1.3 第三方接口异常

**微信支付异常：**
- 统一下单失败 → 记录日志，返回友好错误信息
- 退款接口失败 → 记录日志，标记退款状态为失败
- 网络超时 → 重试机制（最多 3 次）

### 6.2 事务管理

#### 6.2.1 关键事务点

**订单创建事务：**
```csharp
using var tran = _db.BeginTran();
try
{
    // 1. 创建订单
    await _db.Insertable(order).ExecuteCommandAsync();
    // 2. 创建订单项
    await _db.Insertable(orderItems).ExecuteCommandAsync();
    // 3. 扣减库存
    await _stockService.DeductStockAsync(skuId, quantity);
    // 4. 使用优惠券
    if (couponId != null)
        await _couponService.UseCouponAsync(couponId);

    tran.Commit();
}
catch
{
    tran.Rollback();
    throw;
}
```

**支付成功事务：**
```csharp
using var tran = _db.BeginTran();
try
{
    // 1. 更新支付记录
    await _db.Updateable(payment).ExecuteCommandAsync();
    // 2. 更新订单状态
    await _db.Updateable(order).ExecuteCommandAsync();
    // 3. 扣减会员余额（余额支付）
    if (paymentMethod == PaymentMethod.Balance)
        await _memberService.DeductBalanceAsync(memberId, amount);

    tran.Commit();
}
catch
{
    tran.Rollback();
    throw;
}
```

**退款成功事务：**
```csharp
using var tran = _db.BeginTran();
try
{
    // 1. 更新退款记录
    await _db.Updateable(refund).ExecuteCommandAsync();
    // 2. 更新订单状态
    await _db.Updateable(order).ExecuteCommandAsync();
    // 3. 退还库存
    await _stockService.ReturnStockAsync(skuId, quantity);
    // 4. 退还优惠券（如果适用）
    if (order.CouponId != null)
        await _couponService.ReturnCouponAsync(order.CouponId);
    // 5. 退还余额或调用微信退款
    if (paymentMethod == PaymentMethod.Balance)
        await _memberService.ReturnBalanceAsync(memberId, amount);
    else
        await WechatRefundAsync(orderId, amount);

    tran.Commit();
}
catch
{
    tran.Rollback();
    throw;
}
```

### 6.3 测试策略

#### 6.3.1 单元测试（xUnit）

**必须测试的场景：**

**订单服务测试：**
- ✅ 创建订单成功（购物车下单）
- ✅ 创建订单成功（直接购买）
- ✅ 创建订单失败（库存不足）
- ✅ 创建订单失败（优惠券不可用）
- ✅ 订单取消成功（未支付）
- ✅ 订单取消成功（已支付，自动退款）
- ✅ 确认收货成功

**支付服务测试：**
- ✅ 余额支付成功（余额充足）
- ✅ 余额支付失败（余额不足）
- ✅ 微信支付参数生成成功

**退款服务测试：**
- ✅ 申请退款成功（仅退款）
- ✅ 申请退款成功（退货退款）
- ✅ 退款审核通过
- ✅ 退款审核拒绝
- ✅ 退款成功（余额支付）
- ✅ 退款成功（微信支付，Mock 测试）

**库存扣减测试：**
- ✅ 库存扣减成功
- ✅ 库存扣减失败（库存不足）
- ✅ 库存释放成功

#### 6.3.2 集成测试

**端到端流程测试：**
1. 完整购物流程：下单 → 支付 → 发货 → 收货
2. 退款流程：下单 → 支付 → 申请退款 → 审核 → 退款成功
3. 退货退款流程：下单 → 支付 → 申请退货 → 审核 → 填写物流 → 验收 → 退款成功
4. 订单取消流程：下单 → 取消 → 库存释放 → 优惠券退还

#### 6.3.3 Swagger 接口测试

**测试环境配置：**
- 数据库：使用独立的测试数据库
- 微信支付：使用微信支付沙箱环境
- 小程序：使用微信开发者工具测试

**测试要点：**
- 所有 API 接口在 Swagger 中可测试
- 请求参数验证正确
- 响应格式符合统一规范
- 错误信息清晰友好

### 6.4 性能优化建议

#### 6.4.1 数据库优化

- ✅ 订单表索引：`order_no`（唯一）、`member_id`、`status`、`create_time`
- ✅ 支付表索引：`payment_no`（唯一）、`order_id`
- ✅ 退款表索引：`refund_no`（唯一）、`order_id`、`status`

#### 6.4.2 缓存策略

- ✅ SKU 信息缓存（下单时读取频繁）
- ✅ 会员信息缓存（余额、积分）
- ✅ 订单统计信息缓存（5 分钟过期）

#### 6.4.3 异步处理

- ✅ 支付成功通知：异步处理，避免阻塞用户
- ✅ 库存扣减：使用事务确保一致性
- ✅ 微信支付回调：异步处理，快速响应

### 6.5 监控与日志

#### 6.5.1 关键操作日志

**记录以下操作：**
- 订单创建（订单号、会员ID、金额）
- 支付成功（支付单号、订单号、金额、支付方式）
- 订单发货（订单号、物流信息）
- 退款申请（退款单号、订单号、金额）
- 退款审核（退款单号、审核结果）

#### 6.5.2 异常日志

**记录以下异常：**
- 库存扣减失败
- 支付接口调用失败
- 退款接口调用失败
- 事务回滚

#### 6.5.3 性能监控

**监控指标：**
- 订单创建响应时间
- 支付接口响应时间
- 数据库查询耗时
- 微信支付接口耗时

---

## 7. 技术栈与依赖

### 7.1 后端技术栈

- **框架**：.NET 8.0 (LTS)
- **ORM**：SqlSugarCore 5.1.4.x
- **依赖注入**：Autofac 8.x/9.x
- **日志**：Serilog 8.x
- **对象映射**：Mapster 10.x
- **支付**：微信支付 SDK
- **测试**：xUnit

### 7.2 外部依赖

- **会员服务**：会员信息、余额管理
- **商品服务**：SKU 信息、库存管理
- **优惠券服务**：优惠券验证、使用
- **微信支付**：统一下单、支付回调、退款接口

---

## 8. 开发工作量预估

### 8.1 文件清单

**数据模型层：**
- 4 个实体类（Order, OrderItem, Payment, Refund）
- 5 个枚举类
- ~15 个 DTO 类

**业务逻辑层：**
- 3 个服务接口（IOrderService, IPaymentService, IRefundService）
- 3 个服务实现类

**控制器层：**
- 2 个控制器（小程序端 + 管理端）

**测试：**
- ~20 个单元测试用例

### 8.2 工时预估

| 模块 | 预估工时 |
|------|----------|
| 数据模型层 | 0.5 天 |
| 业务逻辑层 | 1.5 天 |
| 控制器层 | 0.5 天 |
| 单元测试 | 0.5 天 |
| 集成测试 | 0.5 天 |
| **总计** | **3.5 天** |

---

## 9. 后续迭代计划

### 9.1 P3.3 后续功能（可选）

- 订单超时自动取消（定时任务）
- 自动确认收货（发货后 7 天）
- 物流信息查询（对接快递100/快递鸟）
- 订单评价功能

### 9.2 P4 CRM 线相关

- CRM 销售订单（B2B）
- 合同管理
- 报价单
- 收款核销

---

## 10. 参考文档

- EasyProduct 整体设计：`docs/superpowers/specs/2026-08-08-easyproduct-integration-design.md`
- 后端开发规范：`docs/backend-guidelines.md`
- 前端开发规范：`docs/frontend-guidelines.md`
- 购物车模块设计：`docs/superpowers/specs/2026-09-09-p3-2-cart-subsystem-design.md`