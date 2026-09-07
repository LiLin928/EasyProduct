# App 端（小程序）模块 API 文档

> 来源: `D:\4-MyProject\EasyProduct\mock-server\src\routes\app`
> 基础路径: `/api/app`

---

## 目录

1. [实体定义](#实体定义)
2. [API 接口](#api-接口)
   - [地址管理](#地址管理)
   - [公告管理](#公告管理)
   - [认证授权](#认证授权)
   - [购物车](#购物车)
   - [会员中心](#会员中心)
   - [订单管理](#订单管理)
   - [支付管理](#支付管理)
   - [商品管理](#商品管理)

---

## 实体定义

### Address (收货地址)

| 字段名 | 类型 | 说明 | 备注 |
|--------|------|------|------|
| id | string | 地址ID | GUID |
| memberId | string | 会员ID | GUID |
| name | string | 收货人姓名 | - |
| phone | string | 收货人手机号 | 格式: 1[3-9]XXXXXXXXX |
| province | string | 省份 | - |
| city | string | 城市 | - |
| district | string | 区县 | - |
| detail | string | 详细地址 | - |
| isDefault | boolean | 是否默认地址 | 同会员仅一条默认地址 |
| fullAddress | string | 完整地址 | 服务端拼接: {province} {city} {district} {detail} |
| createdAt | string | 创建时间 | ISO 8601 |
| updatedAt | string | 更新时间 | ISO 8601 |

### Announcement (公告)

| 字段名 | 类型 | 说明 | 备注 |
|--------|------|------|------|
| id | string | 公告ID | GUID |
| title | string | 标题 | - |
| type | enum | 类型 | `'system' | 'activity' | 'update'` |
| summary | string | 摘要 | - |
| content | string | 内容 | HTML 格式 |
| publishTime | string | 发布时间 | ISO 8601 |
| targetType | enum | 目标类型 | `'global' \| 'member'` |
| targetIds | string[] | 目标会员IDs | 定向公告时有效 |
| attachments | object[] | 附件列表 | 见下表 |
| isRead | boolean | 是否已读 | 会员视角 |
| readTime | string | 阅读时间 | ISO 8601，可选 |

**Attachment 附件结构:**

| 字段名 | 类型 | 说明 | 备注 |
|--------|------|------|------|
| id | string | 附件ID | - |
| name | string | 文件名 | - |
| url | string | 文件URL | - |
| size | number | 文件大小 | 字节 |

### Member (会员)

| 字段名 | 类型 | 说明 | 备注 |
|--------|------|------|------|
| id | string | 会员ID | GUID |
| nickname | string | 昵称 | - |
| avatar | string | 头像URL | - |
| phone | string | 手机号 | - |
| openid | string | 微信OpenID | - |
| levelId | string | 等级ID | - |
| levelName | string | 等级名称 | - |
| points | number | 积分 | - |
| totalSpent | number | 累计消费金额 | - |
| orderCount | number | 订单数 | - |
| status | enum | 状态 | `'active' \| 'inactive'` |
| createdAt | string | 创建时间 | ISO 8601 |
| updatedAt | string | 更新时间 | ISO 8601 |

### CartItem (购物车条目)

| 字段名 | 类型 | 说明 | 备注 |
|--------|------|------|------|
| id | string | 条目ID | GUID |
| memberId | string | 会员ID | GUID |
| spuId | string | SPU ID | 商品ID |
| spuName | string | 商品名称 | - |
| mainImage | string | 商品主图 | - |
| skuId | string | SKU ID | 规格ID |
| specValues | object | 规格值 | `{\"颜色\": \"红色\", \"规格\": \"标准\"}` |
| price | number | 会员价 | - |
| quantity | number | 数量 | - |
| stock | number | 库存 | - |
| selected | boolean | 是否选中 | - |

### Order (订单)

| 字段名 | 类型 | 说明 | 备注 |
|--------|------|------|------|
| id | string | 订单ID | GUID |
| orderNo | string | 订单编号 | 格式: ORD-XXXXXX |
| memberId | string | 会员ID | GUID |
| memberName | string | 会员昵称 | - |
| memberPhone | string | 会员手机号 | - |
| totalAmount | number | 商品总金额 | - |
| discountAmount | number | 优惠金额 | - |
| pointsAmount | number | 积分抵扣金额 | - |
| shippingFee | number | 运费 | 满500免运费 |
| payAmount | number | 应付金额 | - |
| couponId | string | 优惠券ID | 可选 |
| couponName | string | 优惠券名称 | 可选 |
| status | enum | 订单状态 | `'pending' \| 'paid' \| 'shipped' \| 'completed' \| 'cancelled' \| 'refunded'` |
| paymentMethod | string | 支付方式 | `'wechat' \| 'alipay'` |
| remark | string | 订单备注 | - |
| items | OrderItem[] | 订单条目 | 见下表 |
| createdAt | string | 创建时间 | ISO 8601 |
| updatedAt | string | 更新时间 | ISO 8601 |

**OrderItem 订单条目结构:**

| 字段名 | 类型 | 说明 | 备注 |
|--------|------|------|------|
| id | string | 条目ID | GUID |
| orderId | string | 订单ID | - |
| spuId | string | SPU ID | - |
| spuName | string | 商品名称 | - |
| specValues | string | 规格值 | 格式: \"颜色: 红色; 规格: 标准\" |
| price | number | 单价 | - |
| quantity | number | 数量 | - |
| subtotal | number | 小计金额 | - |

### PaymentRecord (支付记录)

| 字段名 | 类型 | 说明 | 备注 |
|--------|------|------|------|
| id | string | 记录ID | GUID |
| orderId | string | 订单ID | - |
| orderNo | string | 订单编号 | - |
| amount | number | 支付金额 | - |
| method | string | 支付方式 | `'wechat' \| 'alipay'` |
| status | enum | 状态 | `'pending' \| 'success' \| 'failed' \| 'refunded'` |
| transactionId | string | 交易流水号 | - |
| paidAt | string | 支付时间 | ISO 8601 |
| createdAt | string | 创建时间 | ISO 8601 |

### PointsRecord (积分记录)

| 字段名 | 类型 | 说明 | 备注 |
|--------|------|------|------|
| id | string | 记录ID | GUID |
| memberId | string | 会员ID | - |
| memberName | string | 会员昵称 | - |
| type | enum | 类型 | `'earn' \| 'spend'` |
| amount | number | 变动金额 | 正数收入，负数支出 |
| source | enum | 来源 | `'order' \| 'signin' \| 'activity' \| 'refund' \| 'adjust'` |
| description | string | 描述 | - |
| createdAt | string | 创建时间 | ISO 8601 |

### ProductCategory (商品分类)

| 字段名 | 类型 | 说明 | 备注 |
|--------|------|------|------|
| id | string | 分类ID | GUID |
| name | string | 分类名称(中文) | - |
| nameEn | string | 分类名称(英文) | - |
| sort | number | 排序号 | - |
| productCount | number | 商品数量 | 小程序渠道商品数 |

### ProductSpu (商品SPU)

| 字段名 | 类型 | 说明 | 备注 |
|--------|------|------|------|
| id | string | SPU ID | GUID |
| code | string | 商品编码 | - |
| name | string | 商品名称(中文) | - |
| nameEn | string | 商品名称(英文) | - |
| categoryId | string | 分类ID | - |
| categoryName | string | 分类名称 | - |
| categoryNameEn | string | 分类名称(英文) | - |
| type | enum | 类型 | `'ticket' \| 'material'` |
| mainImage | string | 主图URL | - |
| images | string[] | 图片列表 | - |
| description | string | 商品描述 | HTML 格式 |
| unit | string | 单位 | - |
| brand | string | 品牌 | - |
| specs | object | 规格组 | 见下表 |
| status | enum | 状态 | `'active' \| 'inactive'` |
| skuCount | number | SKU数量 | - |
| minPrice | number | 最低售价 | - |
| createdAt | string | 创建时间 | ISO 8601 |

**SpecGroup 规格组结构:**

| 字段名 | 类型 | 说明 | 备注 |
|--------|------|------|------|
| name | string | 规格名称 | 如: \"颜色\"、\"规格\" |
| values | string[] | 可选值 | 如: `[\"红色\", \"蓝色\", \"黑色\"]` |

### ProductSku (商品SKU)

| 字段名 | 类型 | 说明 | 备注 |
|--------|------|------|------|
| id | string | SKU ID | GUID |
| spuId | string | SPU ID | - |
| specValues | object | 规格值 | `{\"颜色\": \"红色\", \"规格\": \"标准\"}` |
| barcode | string | 条形码 | - |
| retailPrice | number | 零售价 | - |
| memberPrice | number | 会员价 | - |
| stock | number | 库存 | - |

---

## API 接口

### 地址管理

#### 获取会员地址列表

- **URL**: `/api/app/addresses`
- **Method**: `GET`
- **描述**: 获取当前会员的全部收货地址，默认地址排首位
- **请求参数**: 无
- **响应结构**:
```json
{
  "code": 200,
  "message": "操作成功",
  "data": [Address],
  "timestamp": 1234567890
}
```

#### 获取单个地址详情

- **URL**: `/api/app/addresses/:id`
- **Method**: `GET`
- **描述**: 获取单个收货地址详情
- **请求参数**:
  - `id` (path): 地址ID
- **响应结构**:
```json
{
  "code": 200,
  "message": "操作成功",
  "data": Address,
  "timestamp": 1234567890
}
```

#### 新增收货地址

- **URL**: `/api/app/addresses`
- **Method**: `POST`
- **描述**: 新增收货地址
- **请求参数**:
```json
{
  "name": "string",      // 收货人姓名
  "phone": "string",     // 手机号
  "province": "string",  // 省份
  "city": "string",      // 城市
  "district": "string",  // 区县
  "detail": "string",    // 详细地址
  "isDefault": boolean   // 是否设为默认
}
```
- **响应结构**:
```json
{
  "code": 200,
  "message": "地址已新增",
  "data": Address,
  "timestamp": 1234567890
}
```

#### 更新收货地址

- **URL**: `/api/app/addresses/:id`
- **Method**: `PUT`
- **描述**: 更新收货地址
- **请求参数**:
  - `id` (path): 地址ID
  - Body: 同新增地址
- **响应结构**:
```json
{
  "code": 200,
  "message": "地址已更新",
  "data": Address,
  "timestamp": 1234567890
}
```

#### 删除收货地址

- **URL**: `/api/app/addresses/:id`
- **Method**: `DELETE`
- **描述**: 删除收货地址，若删除默认地址则自动提升最新地址为默认
- **请求参数**:
  - `id` (path): 地址ID
- **响应结构**:
```json
{
  "code": 200,
  "message": "地址已删除",
  "data": null,
  "timestamp": 1234567890
}
```

#### 设置默认地址

- **URL**: `/api/app/addresses/:id/default`
- **Method**: `PUT`
- **描述**: 设置指定地址为默认地址
- **请求参数**:
  - `id` (path): 地址ID
- **响应结构**:
```json
{
  "code": 200,
  "message": "已设为默认地址",
  "data": Address,
  "timestamp": 1234567890
}
```

---

### 公告管理

#### 获取未读公告数量

- **URL**: `/api/app/announcements/unread-count`
- **Method**: `GET`
- **描述**: 获取当前会员的未读公告数量
- **请求参数**: 无
- **响应结构**:
```json
{
  "code": 200,
  "message": "操作成功",
  "data": { "count": 5 },
  "timestamp": 1234567890
}
```

#### 获取公告列表

- **URL**: `/api/app/announcements`
- **Method**: `GET`
- **描述**: 获取用户公告列表（含阅读状态）
- **请求参数**:
  - `pageIndex` (query): 页码，默认1
  - `pageSize` (query): 每页条数，默认10
  - `type` (query): 类型过滤 (`'system' | 'activity' | 'update'`)
  - `isRead` (query): 阅读状态过滤 (`'true' | 'false'`)
- **响应结构**:
```json
{
  "code": 200,
  "message": "操作成功",
  "data": {
    "list": [Announcement],
    "total": 50
  },
  "timestamp": 1234567890
}
```

#### 获取公告详情

- **URL**: `/api/app/announcements/:id`
- **Method**: `GET`
- **描述**: 获取公告详情
- **请求参数**:
  - `id` (path): 公告ID
- **响应结构**:
```json
{
  "code": 200,
  "message": "操作成功",
  "data": Announcement,
  "timestamp": 1234567890
}
```

#### 标记公告为已读

- **URL**: `/api/app/announcements/:id/read`
- **Method**: `POST`
- **描述**: 标记指定公告为已读
- **请求参数**:
  - `id` (path): 公告ID
- **响应结构**:
```json
{
  "code": 200,
  "message": "已标记为已读",
  "data": null,
  "timestamp": 1234567890
}
```

---

### 认证授权

#### 手机号密码登录

- **URL**: `/api/app/auth/login`
- **Method**: `POST`
- **描述**: 手机号+密码登录
- **请求参数**:
```json
{
  "phone": "string",     // 手机号，格式1[3-9]XXXXXXXXX
  "password": "string"   // 密码，至少6位
}
```
- **响应结构**:
```json
{
  "code": 200,
  "message": "操作成功",
  "data": {
    "memberToken": "string",
    "member": {
      "id": "string",
      "nickName": "string",
      "avatar": "string",
      "level": "string",
      "points": 0
    }
  },
  "timestamp": 1234567890
}
```

#### 微信一键登录

- **URL**: `/api/app/auth/wx-login`
- **Method**: `POST`
- **描述**: 微信小程序一键登录（mock阶段任意code返回固定会员）
- **请求参数**:
```json
{
  "code": "string"  // 微信登录code
}
```
- **响应结构**: 同手机号登录

---

### 购物车

#### 获取购物车列表

- **URL**: `/api/app/cart`
- **Method**: `GET`
- **描述**: 获取当前会员购物车列表及合计信息
- **请求参数**: 无
- **响应结构**:
```json
{
  "code": 200,
  "message": "操作成功",
  "data": {
    "list": [CartItem],
    "totalQuantity": 5,
    "totalAmount": 999.99
  },
  "timestamp": 1234567890
}
```

#### 添加商品到购物车

- **URL**: `/api/app/cart`
- **Method**: `POST`
- **描述**: 添加商品到购物车，同SKU合并数量
- **请求参数**:
```json
{
  "skuId": "string",   // SKU ID
  "quantity": 1        // 数量
}
```
- **响应结构**: 同获取购物车列表

#### 更新购物车条目

- **URL**: `/api/app/cart/:id`
- **Method**: `PUT`
- **描述**: 更新购物车条目数量或选中状态
- **请求参数**:
  - `id` (path): 条目ID
  - Body:
```json
{
  "quantity": 1,   // 数量（可选）
  "selected": true  // 是否选中（可选）
}
```
- **响应结构**: 同获取购物车列表

#### 清空购物车

- **URL**: `/api/app/cart/clear`
- **Method**: `DELETE`
- **描述**: 清空当前会员购物车
- **请求参数**: 无
- **响应结构**:
```json
{
  "code": 200,
  "message": "购物车已清空",
  "data": null,
  "timestamp": 1234567890
}
```

#### 删除购物车条目

- **URL**: `/api/app/cart/:id`
- **Method**: `DELETE`
- **描述**: 删除指定购物车条目
- **请求参数**:
  - `id` (path): 条目ID
- **响应结构**: 同获取购物车列表

---

### 会员中心

#### 获取会员资料

- **URL**: `/api/app/member/info`
- **Method**: `GET`
- **描述**: 获取当前会员资料（含等级与订单统计）
- **请求参数**: 无
- **响应结构**:
```json
{
  "code": 200,
  "message": "操作成功",
  "data": {
    "id": "string",
    "nickname": "string",
    "avatar": "string",
    "phone": "string",
    "level": "string",
    "points": 0,
    "totalSpent": 0,
    "orderCount": 0,
    "pendingCount": 0,
    "paidCount": 0,
    "completedCount": 0
  },
  "timestamp": 1234567890
}
```

#### 获取会员资料(兼容版)

- **URL**: `/api/app/member/profile`
- **Method**: `GET`
- **描述**: 同 `/api/app/member/info`

#### 获取积分记录

- **URL**: `/api/app/member/points`
- **Method**: `GET`
- **描述**: 获取当前会员积分记录
- **请求参数**:
  - `pageIndex` (query): 页码，默认1
  - `pageSize` (query): 每页条数，默认10
- **响应结构**:
```json
{
  "code": 200,
  "message": "操作成功",
  "data": {
    "list": [PointsRecord],
    "total": 20
  },
  "timestamp": 1234567890
}
```

---

### 订单管理

#### 创建订单

- **URL**: `/api/app/orders`
- **Method**: `POST`
- **描述**: 创建订单（从购物车或直接购买，成功后自动从购物车移除已下单条目）
- **请求参数**:
```json
{
  "items": [
    {
      "skuId": "string",
      "quantity": 1
    }
  ],
  "remark": "string"  // 订单备注（可选）
}
```
- **响应结构**:
```json
{
  "code": 200,
  "message": "下单成功",
  "data": Order,
  "timestamp": 1234567890
}
```

#### 获取订单列表

- **URL**: `/api/app/orders`
- **Method**: `GET`
- **描述**: 获取当前会员订单列表
- **请求参数**:
  - `pageIndex` (query): 页码，默认1
  - `pageSize` (query): 每页条数，默认10
  - `status` (query): 订单状态过滤
- **响应结构**:
```json
{
  "code": 200,
  "message": "操作成功",
  "data": {
    "list": [Order],
    "total": 50,
    "pageIndex": 1,
    "pageSize": 10,
    "totalPages": 5,
    "hasNextPage": true,
    "hasPrevPage": false
  },
  "timestamp": 1234567890
}
```

#### 获取订单详情

- **URL**: `/api/app/orders/:id`
- **Method**: `GET`
- **描述**: 获取订单详情
- **请求参数**:
  - `id` (path): 订单ID
- **响应结构**:
```json
{
  "code": 200,
  "message": "操作成功",
  "data": Order,
  "timestamp": 1234567890
}
```

#### 取消订单

- **URL**: `/api/app/orders/:id/cancel`
- **Method**: `POST`
- **描述**: 取消待支付订单
- **请求参数**:
  - `id` (path): 订单ID
- **响应结构**:
```json
{
  "code": 200,
  "message": "订单已取消",
  "data": Order,
  "timestamp": 1234567890
}
```

---

### 支付管理

#### 模拟支付

- **URL**: `/api/app/payment/pay`
- **Method**: `POST`
- **描述**: 模拟微信支付（mock阶段仅支持成功/失败两种结果）
- **请求参数**:
```json
{
  "orderId": "string",
  "result": "success"  // 'success' | 'failed'，默认success
}
```
- **响应结构**:
```json
// 成功
{
  "code": 200,
  "message": "支付成功",
  "data": {
    "status": "success",
    "orderId": "string",
    "orderNo": "string",
    "transactionId": "string",
    "paidAt": "string"
  },
  "timestamp": 1234567890
}

// 失败
{
  "code": 200,
  "message": "支付失败（mock）",
  "data": {
    "status": "failed",
    "orderId": "string",
    "orderNo": "string"
  },
  "timestamp": 1234567890
}
```

---

### 商品管理

#### 获取分类列表

- **URL**: `/api/app/categories`
- **Method**: `GET`
- **描述**: 获取商品分类列表（含小程序渠道商品数量）
- **请求参数**: 无
- **响应结构**:
```json
{
  "code": 200,
  "message": "操作成功",
  "data": [ProductCategory],
  "timestamp": 1234567890
}
```

#### 获取分类列表(兼容版)

- **URL**: `/api/app/products/categories`
- **Method**: `GET`
- **描述**: 同 `/api/app/categories`

#### 获取新品列表

- **URL**: `/api/app/products/new`
- **Method**: `GET`
- **描述**: 获取新品列表
- **请求参数**:
  - `limit` (query): 返回数量，默认6，最大20
- **响应结构**:
```json
{
  "code": 200,
  "message": "操作成功",
  "data": [ProductSpu],
  "timestamp": 1234567890
}
```

#### 获取热销列表

- **URL**: `/api/app/products/hot`
- **Method**: `GET`
- **描述**: 获取热销商品列表
- **请求参数**:
  - `limit` (query): 返回数量，默认6，最大20
- **响应结构**: 同新品列表

#### 获取商品列表

- **URL**: `/api/app/products`
- **Method**: `GET`
- **描述**: 获取商品列表（分页+分类/关键词筛选）
- **请求参数**:
  - `pageIndex` (query): 页码，默认1
  - `pageSize` (query): 每页条数，默认10
  - `categoryId` (query): 分类ID过滤（可选）
  - `keyword` (query): 关键词搜索（可选，匹配名称/编码）
- **响应结构**:
```json
{
  "code": 200,
  "message": "操作成功",
  "data": {
    "list": [ProductSpu],
    "total": 100
  },
  "timestamp": 1234567890
}
```

#### 获取商品详情

- **URL**: `/api/app/products/:id`
- **Method**: `GET`
- **描述**: 获取商品详情（SPU信息+可售SKU列表+分类信息）
- **请求参数**:
  - `id` (path): SPU ID
- **响应结构**:
```json
{
  "code": 200,
  "message": "操作成功",
  "data": {
    ...ProductSpu,
    "skus": [{
      "id": "string",
      "specValues": {},
      "retailPrice": 0,
      "memberPrice": 0,
      "stock": 0
    }]
  },
  "timestamp": 1234567890
}
```

---

## 通用响应格式

所有接口统一返回格式:

```json
{
  "code": 200,           // 状态码: 200成功; 400参数错误; 401未授权; 403无权限; 404不存在; 500服务错误
  "message": "操作成功", // 提示信息
  "data": {},            // 业务数据
  "timestamp": 1234567890 // 时间戳
}
```

## 分页数据结构

分页列表响应:

```json
{
  "code": 200,
  "message": "操作成功",
  "data": {
    "list": [],        // 数据列表
    "total": 50,       // 总条数
    "pageIndex": 1,    // 当前页码
    "pageSize": 10,    // 每页条数
    "totalPages": 5,   // 总页数
    "hasNextPage": true,
    "hasPrevPage": false
  },
  "timestamp": 1234567890
}
```

---

*文档生成时间: 2026-09-07*
*来源: mock-server/src/routes/app*
