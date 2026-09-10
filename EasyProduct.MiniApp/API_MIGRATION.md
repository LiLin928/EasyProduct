# API 迁移文档

## 迁移时间
2026-09-10

## 迁移内容

### 已完成
- ✅ 环境配置更新（useMock: false, apiBase: http://localhost:5000/api/app）
- ✅ 订单 API 路径修复（/orders → /order）
- ✅ 新增积分 API（/point）
- ✅ 新增优惠券 API（/coupon）
- ✅ 新增支付 API（/payment）
- ✅ 商品 API 路径修复（/products → /product）

### 待完成
- ⏳ 后端商品控制器（编译失败，缺少 IJwtService 和商品 DTO）
- ⏳ 地址 API（需确认后端路由）

### API 接口清单

#### 微信登录
- POST /api/app/auth/wx-login - 微信登录
- GET /api/app/auth/wx-user-info - 获取微信用户信息

#### 购物车
- GET /api/app/mall/cart - 获取购物车列表
- POST /api/app/mall/cart - 添加商品到购物车
- PUT /api/app/mall/cart/{id} - 更新购物车商品数量
- DELETE /api/app/mall/cart/{id} - 删除购物车商品
- DELETE /api/app/mall/cart - 清空购物车

#### 订单
- POST /api/app/mall/order - 创建订单
- GET /api/app/mall/order - 获取订单列表
- GET /api/app/mall/order/{id} - 获取订单详情
- PUT /api/app/mall/order/{id}/cancel - 取消订单
- PUT /api/app/mall/order/{id}/receive - 确认收货

#### 支付
- POST /api/app/mall/payment - 创建支付
- GET /api/app/mall/payment/{id} - 获取支付详情
- GET /api/app/mall/payment/{id}/status - 查询支付状态

#### 积分
- GET /api/app/mall/point/balance - 获取我的积分余额
- GET /api/app/mall/point/record/list - 获取我的积分流水
- GET /api/app/mall/point/exchange/list - 获取我的积分兑换记录
- GET /api/app/mall/point/statistics - 获取我的积分统计
- POST /api/app/mall/point/check-in - 签到赠送积分

#### 优惠券
- GET /api/app/mall/coupon/available - 获取可领取的优惠券列表
- POST /api/app/mall/coupon/{id}/claim - 领取优惠券
- GET /api/app/mall/coupon/my - 获取我的优惠券列表
- GET /api/app/mall/coupon/user/{id} - 获取用户优惠券详情

#### 商品
- GET /api/app/product - 获取商品列表（后端控制器缺失）
- GET /api/app/product/{id} - 获取商品详情（后端控制器缺失）
- GET /api/app/product/categories - 获取商品分类（后端控制器缺失）

## 数据格式
- 主键：GUID（string）
- 时间格式：ISO 8601
- 金额：decimal（数字）
- 命名规范：驼峰命名（camelCase）

## 认证方式
- 微信登录后获取会员 Token
- Token 存储在 Storage 中
- 请求时在 Header 中添加：`Authorization: Bearer {token}`

## 已知问题
1. 后端编译失败：缺少 IJwtService 实现
2. 商品 DTO 不完整：SpuDto、CategoryDto 等类型未定义
3. 会员端商品控制器无法创建