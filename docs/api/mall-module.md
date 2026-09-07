# Mall 模块接口文档

> 商城模块，包含会员、等级、积分、优惠券、订单、支付等功能

## 实体对象

### Member - 会员

```typescript
interface Member {
  id: string                    // GUID 主键
  nickname: string              // 昵称
  avatar: string                // 头像URL
  phone: string                 // 手机号
  openid: string                // 微信OpenID
  levelId: string               // 会员等级ID
  levelName: string             // 会员等级名称
  points: number                // 当前积分
  totalSpent: number            // 累计消费金额
  orderCount: number            // 订单数量
  status: 0 | 1  // 状态：0=不活跃，1=活跃 // 状态
  createdAt: string
  updatedAt: string
}
```

### MemberLevel - 会员等级

```typescript
interface MemberLevel {
  id: string                    // GUID 主键
  name: string                  // 等级名称
  minPoints: number             // 升级所需积分
  discount: number              // 折扣率（如0.95表示95折）
  sort: number                  // 排序
  status: 0 | 1  // 状态：0=禁用，1=启用 // 状态
  createdAt: string
  updatedAt: string
}
```

### PointsRecord - 积分记录

```typescript
interface PointsRecord {
  id: string                    // GUID 主键
  memberId: string              // 会员ID
  memberName: string            // 会员名称
  type: 0 | 1  // 类型：0=消费，1=获得        // 类型：获得/消费
  amount: number                // 积分数量（正数获得，负数消费）
  source: 'order' | 'signin' | 'activity' | 'refund' | 'adjust' // 来源
  description: string           // 描述
  createdAt: string
}
```

### Coupon - 优惠券

```typescript
interface Coupon {
  id: string                    // GUID 主键
  name: string                  // 优惠券名称
  type: 0 | 1  // 类型：0=固定金额，1=百分比折扣     // 类型：固定金额/百分比折扣
  value: number                 // 面值（固定金额或折扣率）
  minSpend: number              // 最低消费金额
  totalCount: number            // 发行总量
  issuedCount: number           // 已发放数量
  usedCount: number             // 已使用数量
  startDate: string             // 开始日期（YYYY-MM-DD）
  endDate: string               // 结束日期（YYYY-MM-DD）
  status: 0 | 1  // 状态：0=禁用，1=启用 // 状态
  createdAt: string
  updatedAt: string
}
```

### Order - 订单

```typescript
interface Order {
  id: string                    // GUID 主键
  orderNo: string               // 订单编号
  memberId: string              // 会员ID
  memberName: string            // 会员名称
  memberPhone: string           // 会员手机
  totalAmount: number           // 商品总金额
  discountAmount: number        // 优惠金额
  pointsAmount: number          // 积分抵扣金额
  shippingFee: number           // 运费
  payAmount: number             // 实付金额
  couponId: string              // 优惠券ID
  couponName: string            // 优惠券名称
  status: 0 | 1 | 2 | 3 | 4 | 5  // 订单状态：0=已取消，1=待支付，2=已支付，3=已发货，4=已完成，5=已退款 // 订单状态
  paymentMethod: string         // 支付方式
  remark: string                // 备注
  items: OrderItem[]            // 订单明细
  createdAt: string
  updatedAt: string
}

interface OrderItem {
  id: string                    // GUID 主键
  orderId: string               // 订单ID
  spuId: string                 // 商品SPU ID
  spuName: string               // 商品名称
  specValues: string            // 规格值（JSON对象）
  price: number                 // 单价
  quantity: number              // 数量
  subtotal: number              // 小计
}
```

### PaymentRecord - 支付记录

```typescript
interface PaymentRecord {
  id: string                    // GUID 主键
  orderId: string               // 订单ID
  orderNo: string               // 订单编号
  amount: number                // 支付金额
  method: string                // 支付方式（wechat/alipay）
  status: 'pending' | 'success' | 'failed' | 'refunded' // 支付状态
  transactionId: string         // 第三方交易号
  paidAt: string                // 支付时间
  createdAt: string
}
```

---

## API 接口

### 1. 会员管理（Member）

#### GET /api/admin/mall/member/list
获取会员列表（分页）

**查询参数：**
- `pageIndex` - 页码
- `pageSize` - 每页条数
- `nickname` - 昵称（模糊搜索）
- `phone` - 手机号
- `levelId` - 会员等级ID
- `status` - 状态

---

#### GET /api/admin/mall/member/:id
获取会员详情

---

#### PUT /api/admin/mall/member/:id
更新会员信息

---

#### PUT /api/admin/mall/member/:id/status
更新会员状态

---

### 2. 会员等级（Level）

#### GET /api/admin/mall/level/list
获取会员等级列表

---

#### POST /api/admin/mall/level
创建会员等级

---

#### PUT /api/admin/mall/level/:id
更新会员等级

---

#### DELETE /api/admin/mall/level/:id
删除会员等级

---

### 3. 积分管理（Points）

#### GET /api/admin/mall/points/list
获取积分记录列表（分页）

**查询参数：**
- `pageIndex` - 页码
- `pageSize` - 每页条数
- `memberId` - 会员ID
- `type` - 类型（earn/spend）
- `source` - 来源

---

#### POST /api/admin/mall/points/adjust
积分调整（手工增减积分）

**请求参数：**
```json
{
  "memberId": "string",
  "amount": 100,
  "description": "手工调整"
}
```

---

### 4. 优惠券管理（Coupon）

#### GET /api/admin/mall/coupon/list
获取优惠券列表（分页）

---

#### GET /api/admin/mall/coupon/:id
获取优惠券详情

---

#### POST /api/admin/mall/coupon
创建优惠券

---

#### PUT /api/admin/mall/coupon/:id
更新优惠券

---

#### DELETE /api/admin/mall/coupon/:id
删除优惠券

---

#### PUT /api/admin/mall/coupon/:id/status
更新优惠券状态

---

### 5. 订单管理（Order）

#### GET /api/admin/mall/order/list
获取订单列表（分页）

**查询参数：**
- `pageIndex` - 页码
- `pageSize` - 每页条数
- `orderNo` - 订单编号
- `memberId` - 会员ID
- `status` - 订单状态
- `startTime` / `endTime` - 下单时间范围

---

#### GET /api/admin/mall/order/:id
获取订单详情

---

#### PUT /api/admin/mall/order/:id/status
更新订单状态

---

#### POST /api/admin/mall/order/:id/ship
订单发货

**请求参数：**
```json
{
  "expressCompany": "string",
  "expressNo": "string"
}
```

---

### 6. 支付记录（Payment）

#### GET /api/admin/mall/payment/list
获取支付记录列表（分页）

**查询参数：**
- `pageIndex` - 页码
- `pageSize` - 每页条数
- `orderNo` - 订单编号
- `status` - 支付状态
- `method` - 支付方式

---

### 7. 收货地址（Address）

#### GET /api/admin/mall/address/list
获取会员地址列表

**查询参数：**
- `memberId` - 会员ID

---

#### GET /api/admin/mall/address/:id
获取地址详情

---

#### POST /api/admin/mall/address
创建地址

---

#### PUT /api/admin/mall/address/:id
更新地址

---

#### DELETE /api/admin/mall/address/:id
删除地址

---

## 数据库表设计

### mall_member
```sql
CREATE TABLE mall_member (
  id VARCHAR(36) PRIMARY KEY,
  nickname VARCHAR(50),
  avatar VARCHAR(500),
  phone VARCHAR(20),
  openid VARCHAR(100) UNIQUE,
  level_id VARCHAR(36),
  points INT DEFAULT 0,
  total_spent DECIMAL(12,2) DEFAULT 0,
  order_count INT DEFAULT 0,
  status VARCHAR(20) DEFAULT 'active',
  created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
  updated_at DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  INDEX idx_phone (phone),
  INDEX idx_openid (openid),
  INDEX idx_level_id (level_id)
);
```

### mall_level
```sql
CREATE TABLE mall_level (
  id VARCHAR(36) PRIMARY KEY,
  name VARCHAR(50) NOT NULL,
  min_points INT DEFAULT 0,
  discount DECIMAL(3,2) DEFAULT 1.00,
  sort INT DEFAULT 0,
  status VARCHAR(20) DEFAULT 'enabled',
  created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
  updated_at DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
);
```

### mall_points_record
```sql
CREATE TABLE mall_points_record (
  id VARCHAR(36) PRIMARY KEY,
  member_id VARCHAR(36) NOT NULL,
  member_name VARCHAR(50),
  type VARCHAR(20) NOT NULL,
  amount INT NOT NULL,
  source VARCHAR(20) NOT NULL,
  description VARCHAR(200),
  created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
  INDEX idx_member_id (member_id)
);
```

### mall_coupon
```sql
CREATE TABLE mall_coupon (
  id VARCHAR(36) PRIMARY KEY,
  name VARCHAR(100) NOT NULL,
  type VARCHAR(20) NOT NULL,
  value DECIMAL(10,2) NOT NULL,
  min_spend DECIMAL(10,2) DEFAULT 0,
  total_count INT NOT NULL,
  issued_count INT DEFAULT 0,
  used_count INT DEFAULT 0,
  start_date DATE NOT NULL,
  end_date DATE NOT NULL,
  status VARCHAR(20) DEFAULT 'enabled',
  created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
  updated_at DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
);
```

### mall_order
```sql
CREATE TABLE mall_order (
  id VARCHAR(36) PRIMARY KEY,
  order_no VARCHAR(50) NOT NULL UNIQUE,
  member_id VARCHAR(36) NOT NULL,
  member_name VARCHAR(50),
  member_phone VARCHAR(20),
  total_amount DECIMAL(12,2) NOT NULL,
  discount_amount DECIMAL(12,2) DEFAULT 0,
  points_amount DECIMAL(12,2) DEFAULT 0,
  shipping_fee DECIMAL(10,2) DEFAULT 0,
  pay_amount DECIMAL(12,2) NOT NULL,
  coupon_id VARCHAR(36),
  coupon_name VARCHAR(100),
  status VARCHAR(20) NOT NULL,
  payment_method VARCHAR(20),
  remark TEXT,
  created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
  updated_at DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  INDEX idx_member_id (member_id),
  INDEX idx_status (status),
  INDEX idx_created_at (created_at)
);
```

### mall_order_item
```sql
CREATE TABLE mall_order_item (
  id VARCHAR(36) PRIMARY KEY,
  order_id VARCHAR(36) NOT NULL,
  spu_id VARCHAR(36) NOT NULL,
  spu_name VARCHAR(200),
  spec_values JSON,
  price DECIMAL(10,2) NOT NULL,
  quantity INT NOT NULL,
  subtotal DECIMAL(12,2) NOT NULL,
  INDEX idx_order_id (order_id)
);
```

### mall_payment
```sql
CREATE TABLE mall_payment (
  id VARCHAR(36) PRIMARY KEY,
  order_id VARCHAR(36) NOT NULL,
  order_no VARCHAR(50),
  amount DECIMAL(12,2) NOT NULL,
  method VARCHAR(20) NOT NULL,
  status VARCHAR(20) NOT NULL,
  transaction_id VARCHAR(100),
  paid_at DATETIME,
  created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
  INDEX idx_order_id (order_id),
  INDEX idx_status (status)
);
```

### mall_address
```sql
CREATE TABLE mall_address (
  id VARCHAR(36) PRIMARY KEY,
  member_id VARCHAR(36) NOT NULL,
  receiver_name VARCHAR(50) NOT NULL,
  phone VARCHAR(20) NOT NULL,
  province VARCHAR(50),
  city VARCHAR(50),
  district VARCHAR(50),
  detail_address VARCHAR(200),
  is_default INT DEFAULT 0 COMMENT '是否默认：0=否，1=是',
  created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
  updated_at DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  INDEX idx_member_id (member_id)
);
```

---

## 开发注意事项

1. **会员认证**: App端使用 Member JWT，Admin端使用 Admin JWT
2. **积分规则**: 订单完成自动赠送积分，可配置积分比例
3. **优惠券**: 支持固定金额和百分比折扣两种类型
4. **订单流程**: pending → paid → shipped → completed / cancelled / refunded
5. **库存扣减**: 订单支付成功后扣减库存，取消/退款后回滚
6. **支付对接**: 需对接微信支付和支付宝支付

---

生成时间: 2026-09-07