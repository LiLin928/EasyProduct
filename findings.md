# P3 商城线后续开发发现（P3.5-P3.7）

> **创建时间：** 2026-09-10

---

## 设计文档分析

**来源：** docs/superpowers/specs/2026-08-08-easyproduct-integration-design.md

### 核心发现

1. **模块定位**
   - P3 商城线包含：会员/购物车/订单/支付/优惠券/积分 + MiniApp 接入 + 冲销
   - P3.2-P3.4 已完成（购物车、订单支付、优惠券）
   - P3.5-P3.7 为剩余开发任务

2. **技术架构**
   - 后端：.NET 8 + SqlSugar + Autofac
   - 分层：Controller → Service → Repository
   - 数据库：MySQL 8 单库，表前缀 mall_ / crm_
   - 认证：会员 JWT（小程序端）

3. **关键数据表**
   - 积分系统：mall_member（积分字段）、mall_point_rule、mall_point_record
   - 冲销：crm_reversal、crm_reversal_item、wf_biz_link

---

## P3.5 积分系统详细设计

### 数据模型

**会员积分字段（mall_member）：**
- total_points：累计积分
- available_points：可用积分
- frozen_points：冻结积分

**积分规则表（mall_point_rule）：**
- id：规则 ID（GUID）
- name：规则名称
- type：类型（下单、评价、签到、邀请）
- points：积分数
- multiple：是否倍数（下单金额倍数）
- status：状态（启用/禁用）
- start_time：生效时间
- end_time：失效时间

**积分流水表（mall_point_record）：**
- id：流水 ID（GUID）
- member_id：会员 ID
- type：类型（收入/支出）
- source：来源（下单、评价、兑换、抵扣）
- points：积分数
- balance：余额
- remark：备注
- related_id：关联 ID（订单号、兑换 ID）
- create_time：创建时间

**积分兑换记录表（mall_point_exchange）：**
- id：兑换 ID（GUID）
- member_id：会员 ID
- coupon_id：优惠券 ID（如果兑换优惠券）
- points：消耗积分
- status：状态（成功/失败）
- create_time：创建时间

### 业务规则

**积分发放：**
1. 下单：订单金额 × 积分比例（可配置）
2. 评价：固定积分（如 +10 分）
3. 签到：连续签到天数递增（如 1 天 +5，2 天 +10...）
4. 邀请：被邀请人首单后发放

**积分消费：**
1. 兑换优惠券：积分 → 优惠券
2. 订单抵扣：积分抵扣现金（如 100 积分 = 1 元，最大抵扣 50%）
3. 积分冻结：下单时冻结，取消/退款时解冻

**积分有效期：**
- 建议：积分有效期为 1 年
- 年底清零：每年 12 月 31 日清零上年积分

### API 契约

**管理端：**
- `GET /api/admin/mall/point-rule/list`：积分规则列表
- `POST /api/admin/mall/point-rule`：创建积分规则
- `PUT /api/admin/mall/point-rule/:id`：更新积分规则
- `DELETE /api/admin/mall/point-rule/:id`：删除积分规则

**小程序端：**
- `GET /api/app/mall/point/balance`：积分余额查询
- `GET /api/app/mall/point/record/list`：积分流水查询
- `POST /api/app/mall/point/exchange`：积分兑换优惠券
- `POST /api/app/mall/order/create`：下单时积分抵扣（扩展）

---

## P3.6 MiniApp 小程序接入详细设计

### 微信登录流程

```mermaid
sequenceDiagram
    participant 小程序
    participant 后端
    participant 微信服务器
    
    小程序->>小程序：wx.login() 获取 code
    小程序->>后端：POST /api/app/auth/wx-login {code}
    后端->>微信服务器：code2Session(appid, secret, code)
    微信服务器->>后端：{openid, session_key}
    后端->>后端：查询/创建会员记录
    后端->>后端：生成会员 JWT
    后端->>小程序：{token, member_info}
    小程序->>小程序：存储 token 到 Storage
```

### API 适配层改造

**当前状态：**
- 小程序使用 mock 数据（adapters/）
- 需要切换为真实 API

**改造要点：**
1. API Base URL：从 mock 改为 `/api/app/*`
2. 认证头：添加 `Authorization: Bearer {token}`
3. 数据格式：字段名驼峰化（mock 可能是下划线）
4. 错误处理：统一响应格式 `{code, message, data}`

**关键 API：**
- 商品：`GET /api/app/product/list`（渠道=小程序）
- 商品详情：`GET /api/app/product/:id`
- 购物车：`GET /api/app/cart/list`
- 加入购物车：`POST /api/app/cart`
- 创建订单：`POST /api/app/order`
- 订单列表：`GET /api/app/order/list`
- 支付：`POST /api/app/payment`
- 积分余额：`GET /api/app/point/balance`
- 我的优惠券：`GET /api/app/coupon/my`

### 微信支付集成

**支付流程：**
```mermaid
sequenceDiagram
    participant 小程序
    participant 后端
    participant 微信支付
    
    小程序->>后端：POST /api/app/payment {order_id, pay_method}
    后端->>后端：创建支付单（pending）
    后端->>微信支付：JSAPI 统一下单
    微信支付->>后端：prepay_id
    后端->>后端：签名计算
    后端->>小程序：{timeStamp, nonceStr, package, signType, paySign}
    小程序->>小程序：wx.requestPayment()
    微信支付->>小程序：支付结果
    微信支付->>后端：支付回调（异步）
    后端->>后端：更新支付单状态（success）
    后端->>后端：更新订单状态（paid）
    后端->>微信支付：返回 success
```

**关键配置：**
- AppID：小程序 AppID
- MchID：商户号
- API Key：商户 API 密钥
- 支付证书：apiclient_cert.p12

---

## P3.7 冲销功能详细设计

### 数据模型

**冲销主表（crm_reversal）：**
- id：冲销 ID（GUID）
- type：冲销类型（商城退款、销售退货、采购退货、单据作废）
- status：状态（待审核、已批准、已执行、已拒绝）
- source_type：来源类型（mall_order、sales_order、purchase_order）
- source_id：来源单号
- amount：冲销金额
- reason：冲销原因
- audit_user：审核人
- audit_time：审核时间
- execute_time：执行时间
- create_time：创建时间
- create_user：创建人

**冲销明细表（crm_reversal_item）：**
- id：明细 ID（GUID）
- reversal_id：冲销 ID
- sku_id：SKU ID
- quantity：数量
- amount：金额
- warehouse_id：仓库 ID

**工作流关联（wf_biz_link）：**
- biz_type：业务类型（reversal）
- biz_id：业务 ID（冲销 ID）
- workflow_instance_id：工作流实例 ID

### 业务规则

**商城退款冲销：**
1. 触发：会员申请退款
2. 动作：创建冲销单（类型=商城退款）
3. 流程：退款审核通过 → 冲销执行 → 库存回滚 → 财务记录

**销售退货冲销：**
1. 触发：B2B 客户退货
2. 动作：创建冲销单（类型=销售退货）
3. 流程：退货审核 → 冲销执行 → 库存回滚 → 应收调整

**采购退货冲销：**
1. 触发：向供应商退货
2. 动作：创建冲销单（类型=采购退货）
3. 流程：退货审核 → 冲销执行 → 库存减少 → 应付调整

**单据作废冲销：**
1. 触发：管理员作废单据
2. 动作：创建冲销单（类型=单据作废）
3. 流程：作废审核 → 冲销执行 → 全部回滚

### 库存回滚逻辑

```
商城退款：
- 库存增加（可用 + 数量）
- 记录流水（来源=冲销退回）

销售退货：
- 库存增加（可用 + 数量）
- 记录流水（来源=冲销退回）

采购退货：
- 库存减少（可用 - 数量）
- 记录流水（来源=冲销出库）
```

### API 契约

**管理端：**
- `GET /api/admin/crm/reversal/list`：冲销列表
- `GET /api/admin/crm/reversal/:id`：冲销详情
- `POST /api/admin/crm/reversal`：创建冲销
- `POST /api/admin/crm/reversal/:id/audit`：审核冲销
- `POST /api/admin/crm/reversal/:id/execute`：执行冲销
- `GET /api/admin/crm/reversal/:id/audit-history`：审核历史

---

## 技术依赖关系

```
P3.5 积分系统
    ├── 依赖：会员模块（已完成）
    ├── 依赖：优惠券模块（P3.4 已完成）
    └── 依赖：订单模块（P3.3 已完成）

P3.6 MiniApp 接入
    ├── 依赖：所有商城 API（已完成）
    ├── 依赖：微信支付资质（需落实）
    └── 依赖：小程序配置（AppID、密钥）

P3.7 冲销功能
    ├── 依赖：库存模块（P4 CRM 线）
    ├── 依赖：工作流模块（P5）
    └── 可选：审批流程（暂缓）
```

---

## 风险提示

1. **微信支付资质**
   - 需要商户号和小程序类目资质
   - 建议：P3.6 前落实，否则使用模拟支付

2. **工作流依赖**
   - P3.7 冲销需要工作流模块支持审批
   - 建议：P3.7 暂不挂审批，简单审核即可

3. **库存并发**
   - 冲销涉及库存回滚，需考虑并发安全
   - 建议：使用数据库事务 + 乐观锁

4. **积分并发**
   - 积分发放/消费可能并发
   - 建议：使用数据库事务 + 会员积分字段乐观锁

---

## 后续优化方向

1. **积分规则引擎**
   - 可视化配置积分规则
   - 支持复杂条件（满额、指定商品、首单）

2. **小程序性能优化**
   - 商品列表懒加载
   - 图片懒加载
   - 分包加载

3. **冲销审计日志**
   - 记录所有冲销操作
   - 支持回溯查询

---

## 参考资料

- EasyProduct 集成设计文档：`docs/superpowers/specs/2026-08-08-easyproduct-integration-design.md`
- 后端开发规范：`docs/backend-guidelines.md`
- 前端开发规范：`docs/frontend-guidelines.md`
- 微信支付文档：https://pay.weixin.qq.com/wiki/doc/apiv3/apis/index.shtml