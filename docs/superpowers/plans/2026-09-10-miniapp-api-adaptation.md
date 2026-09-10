# P3.6 API 适配层完善实施计划

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** 完善 MiniApp 小程序 API 适配层，将所有 API 切换到真实后端，确保商品、购物车、订单、支付、积分、优惠券等模块都能正常连接

**Architecture:**
- 修改小程序 API 文件，确保路径与后端控制器路由一致
- 创建缺失的后端控制器（商品、地址）
- 验证数据格式兼容性
- 使用真实后端地址（http://localhost:5000/api/app）

**Tech Stack:**
- 前端：微信小程序 TypeScript
- 后端：.NET 8 + ASP.NET Core
- API 路由规范：RESTful，单数形式（/order 而非 /orders）

---

## 前置条件

- ✅ 后端已运行在 localhost:5000
- ✅ 小程序环境配置已更新（useMock: false）
- ⚠️ 部分控制器缺失（商品、地址）

---

## 任务分解

### Task 1: 修复订单 API 路径

**Files:**
- Modify: `EasyProduct.MiniApp/api/order.ts:5`

**现状分析：**
- 小程序：`/orders`（复数）
- 后端：`/api/app/mall/order`（单数）
- 需要统一为单数形式

- [ ] **Step 1: 修改订单 API 路径**

修改 `EasyProduct.MiniApp/api/order.ts` 第 5 行：

```typescript
// 修改前
const BASE_URL = '/orders'

// 修改后
const BASE_URL = '/order'
```

- [ ] **Step 2: 验证订单 API 路径正确**

检查所有订单相关方法：
- `createOrder()` - POST `/order`
- `getOrderList()` - GET `/order`
- `getOrderDetail()` - GET `/order/{id}`
- `cancelOrder()` - PUT `/order/{id}/cancel`
- `confirmReceive()` - PUT `/order/{id}/receive`

确保路径正确。

- [ ] **Step 3: 提交订单 API 修复**

```bash
git add EasyProduct.MiniApp/api/order.ts
git commit -m "fix(miniapp): 修复订单 API 路径（复数改单数）"
```

---

### Task 2: 创建积分 API 文件

**Files:**
- Create: `EasyProduct.MiniApp/api/point.ts`

**现状分析：**
- 小程序缺少积分 API 文件
- 后端已有 PointController（`/api/app/mall/point`）
- 需要创建对应的 API 文件

- [ ] **Step 1: 创建积分 API 文件**

创建 `EasyProduct.MiniApp/api/point.ts`：

```typescript
// api/point.ts —— 积分相关 API
import { request } from '../utils/request'
import type { ApiResponse } from '../types/api.types'

const BASE_URL = '/point'

/** 积分余额 */
interface PointBalance {
  memberId: string
  totalPoints: number
  availablePoints: number
  frozenPoints: number
}

/** 积分流水 */
interface PointRecord {
  id: string
  memberId: string
  type: number
  source: number
  points: number
  balance: number
  remark?: string
  relatedId?: string
  createTime: string
}

/** 积分流水查询参数 */
interface PointRecordQuery {
  type?: number
  source?: number
  startTime?: string
  endTime?: string
  pageIndex?: number
  pageSize?: number
}

/** 分页结果 */
interface PageResult<T> {
  list: T[]
  total: number
}

/** 获取我的积分余额 */
export function getPointBalance(): Promise<PointBalance> {
  return request<PointBalance>({
    url: `${BASE_URL}/balance`,
    method: 'GET',
  })
}

/** 获取我的积分流水 */
export function getPointRecords(params: PointRecordQuery): Promise<PageResult<PointRecord>> {
  return request<PageResult<PointRecord>>({
    url: `${BASE_URL}/record/list`,
    method: 'GET',
    data: params,
  })
}

/** 获取我的积分兑换记录 */
export function getPointExchanges(pageIndex: number = 1, pageSize: number = 10): Promise<PageResult<any>> {
  return request<PageResult<any>>({
    url: `${BASE_URL}/exchange/list`,
    method: 'GET',
    data: { pageIndex, pageSize },
  })
}

/** 获取我的积分统计 */
export function getPointStatistics(): Promise<Record<string, any>> {
  return request<Record<string, any>>({
    url: `${BASE_URL}/statistics`,
    method: 'GET',
  })
}

/** 签到赠送积分 */
export function checkIn(): Promise<number> {
  return request<number>({
    url: `${BASE_URL}/check-in`,
    method: 'POST',
  })
}
```

- [ ] **Step 2: 创建积分类型定义**

检查是否需要在 `types/` 目录下创建积分相关的类型定义。如果现有类型文件中没有，创建 `EasyProduct.MiniApp/types/point.types.ts`。

- [ ] **Step 3: 提交积分 API 文件**

```bash
git add EasyProduct.MiniApp/api/point.ts
git commit -m "feat(miniapp): 添加积分 API 文件"
```

---

### Task 3: 创建优惠券 API 文件

**Files:**
- Create: `EasyProduct.MiniApp/api/coupon.ts`

**现状分析：**
- 小程序缺少优惠券 API 文件
- 后端已有 UserCouponController（`/api/app/mall/coupon`）
- 需要创建对应的 API 文件

- [ ] **Step 1: 创建优惠券 API 文件**

创建 `EasyProduct.MiniApp/api/coupon.ts`：

```typescript
// api/coupon.ts —— 优惠券相关 API
import { request } from '../utils/request'

const BASE_URL = '/coupon'

/** 优惠券 */
interface Coupon {
  id: string
  name: string
  type: number
  discount: number
  minAmount: number
  startTime: string
  endTime: string
  status: number
}

/** 用户优惠券 */
interface UserCoupon {
  id: string
  couponId: string
  couponName: string
  couponType: number
  discount: number
  minAmount: number
  startTime: string
  endTime: string
  status: number
  useTime?: string
  orderId?: string
}

/** 获取可领取的优惠券列表 */
export function getAvailableCoupons(): Promise<Coupon[]> {
  return request<Coupon[]>({
    url: `${BASE_URL}/available`,
    method: 'GET',
  })
}

/** 领取优惠券 */
export function claimCoupon(couponId: string): Promise<string> {
  return request<string>({
    url: `${BASE_URL}/${couponId}/claim`,
    method: 'POST',
  })
}

/** 获取我的优惠券列表 */
export function getMyCoupons(status?: number): Promise<UserCoupon[]> {
  return request<UserCoupon[]>({
    url: `${BASE_URL}/my`,
    method: 'GET',
    data: { status },
  })
}

/** 获取用户优惠券详情 */
export function getUserCouponDetail(id: string): Promise<UserCoupon> {
  return request<UserCoupon>({
    url: `${BASE_URL}/user/${id}`,
    method: 'GET',
  })
}
```

- [ ] **Step 2: 提交优惠券 API 文件**

```bash
git add EasyProduct.MiniApp/api/coupon.ts
git commit -m "feat(miniapp): 添加优惠券 API 文件"
```

---

### Task 4: 创建支付 API 文件

**Files:**
- Create: `EasyProduct.MiniApp/api/payment.ts`

**现状分析：**
- 小程序缺少支付 API 文件
- 后端已有 PaymentController（`/api/app/mall/payment`）
- 需要创建对应的 API 文件

- [ ] **Step 1: 创建支付 API 文件**

创建 `EasyProduct.MiniApp/api/payment.ts`：

```typescript
// api/payment.ts —— 支付相关 API
import { request } from '../utils/request'

const BASE_URL = '/payment'

/** 支付单 */
interface Payment {
  id: string
  orderId: string
  amount: number
  method: number
  status: number
  createTime: string
  payTime?: string
}

/** 创建支付参数 */
interface CreatePaymentParams {
  orderId: string
  method: number
}

/** 微信支付参数 */
interface WxPayParams {
  timeStamp: string
  nonceStr: string
  package: string
  signType: string
  paySign: string
}

/** 创建支付 */
export function createPayment(params: CreatePaymentParams): Promise<WxPayParams> {
  return request<WxPayParams>({
    url: BASE_URL,
    method: 'POST',
    data: params,
  })
}

/** 获取支付详情 */
export function getPaymentDetail(id: string): Promise<Payment> {
  return request<Payment>({
    url: `${BASE_URL}/${id}`,
    method: 'GET',
  })
}

/** 查询支付状态 */
export function queryPaymentStatus(id: string): Promise<{ status: string }> {
  return request<{ status: string }>({
    url: `${BASE_URL}/${id}/status`,
    method: 'GET',
  })
}
```

- [ ] **Step 2: 提交支付 API 文件**

```bash
git add EasyProduct.MiniApp/api/payment.ts
git commit -m "feat(miniapp): 添加支付 API 文件"
```

---

### Task 5: 修复商品 API 路径

**Files:**
- Modify: `EasyProduct.MiniApp/api/product.ts:14`

**现状分析：**
- 小程序：`/products`（复数）
- 后端：需要确认是否有商品控制器
- 如果没有，需要创建后端商品控制器

- [ ] **Step 1: 检查后端是否有商品控制器**

```bash
find EasyProduct.WebApi/EasyProduct.Web/Controllers/App -name "*Product*"
```

如果没有找到，说明需要创建。

- [ ] **Step 2: 创建后端商品控制器（如果缺失）**

创建 `EasyProduct.WebApi/EasyProduct.Web/Controllers/App/Product/ProductController.cs`：

```csharp
using EasyProduct.Common.Base;
using EasyProduct.Business.Product;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyProduct.Web.Controllers.App.Product;

/// <summary>
/// 商品控制器（会员端）
/// </summary>
[ApiController]
[Route("api/app/product")]
[Authorize(AuthenticationSchemes = "MemberJwt")]
public class ProductController : BaseController
{
    private readonly ISpuService _spuService;

    public ProductController(ISpuService spuService)
    {
        _spuService = spuService;
    }

    /// <summary>
    /// 获取商品列表
    /// </summary>
    [HttpGet]
    public async Task<ApiResponse<PageResponse<ProductDto>>> GetList([FromQuery] ProductQuery query)
    {
        var result = await _spuService.GetProductListAsync(query, "miniapp");
        return Success(result);
    }

    /// <summary>
    /// 获取商品详情
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ApiResponse<ProductDetailDto>> GetDetail(string id)
    {
        var result = await _spuService.GetProductDetailAsync(id);
        return Success(result);
    }

    /// <summary>
    /// 获取商品分类
    /// </summary>
    [HttpGet("categories")]
    [AllowAnonymous]
    public async Task<ApiResponse<List<CategoryDto>>> GetCategories()
    {
        var result = await _spuService.GetCategoriesAsync();
        return Success(result);
    }
}
```

注意：这个控制器需要依赖 ISpuService 和相关的 DTO，可能需要先确认这些服务是否存在。

- [ ] **Step 3: 修改小程序商品 API 路径**

修改 `EasyProduct.MiniApp/api/product.ts` 第 14 行：

```typescript
// 修改前
const BASE_URL = '/products'

// 修改后
const BASE_URL = '/product'
```

- [ ] **Step 4: 提交商品 API 修复**

```bash
git add EasyProduct.MiniApp/api/product.ts
git add EasyProduct.WebApi/EasyProduct.Web/Controllers/App/Product/ProductController.cs
git commit -m "fix(miniapp): 修复商品 API 路径，添加后端商品控制器"
```

---

### Task 6: 验证 API 连接

**Files:**
- None（验证性任务）

- [ ] **Step 1: 启动后端服务**

```bash
cd EasyProduct.WebApi
dotnet run
```

确保后端运行在 http://localhost:5000

- [ ] **Step 2: 检查后端 Swagger**

访问 http://localhost:5000/swagger，确认所有 API 接口都已暴露：
- `/api/app/mall/cart`
- `/api/app/mall/order`
- `/api/app/mall/payment`
- `/api/app/mall/point`
- `/api/app/mall/coupon`
- `/api/app/product`（如果已创建）

- [ ] **Step 3: 在小程序中测试关键 API**

在小程序开发工具中，测试以下关键功能：
1. 微信登录（`/api/app/auth/wx-login`）
2. 获取购物车（`/api/app/mall/cart`）
3. 创建订单（`/api/app/mall/order`）
4. 获取积分余额（`/api/app/mall/point/balance`）

---

### Task 7: 数据格式适配（可选）

**Files:**
- Modify: 小程序类型定义文件（根据实际情况）

**现状分析：**
- 后端使用驼峰命名（camelCase）
- 小程序类型定义可能需要调整

- [ ] **Step 1: 检查数据格式兼容性**

对比后端 DTO 和小程序类型定义，确认字段名是否一致：
- 后端：`createTime`
- 小程序：`createTime` 或 `created_at`？

如果格式不一致，需要在小程序端做适配。

- [ ] **Step 2: 更新类型定义（如果需要）**

如果发现格式不一致，更新小程序的类型定义文件。

---

### Task 8: 文档和提交

**Files:**
- Create: `EasyProduct.MiniApp/API_MIGRATION.md`

- [ ] **Step 1: 创建 API 迁移文档**

创建 `EasyProduct.MiniApp/API_MIGRATION.md`：

```markdown
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
- GET /api/app/product - 获取商品列表
- GET /api/app/product/{id} - 获取商品详情
- GET /api/app/product/categories - 获取商品分类

## 数据格式
- 主键：GUID（string）
- 时间格式：ISO 8601
- 金额：decimal（数字）
- 命名规范：驼峰命名（camelCase）

## 认证方式
- 微信登录后获取会员 Token
- Token 存储在 Storage 中
- 请求时在 Header 中添加：`Authorization: Bearer {token}`
```

- [ ] **Step 2: 最终提交**

```bash
git add .
git commit -m "docs(miniapp): 添加 API 迁移文档"
git push origin main
```

---

## 自我检查清单

**1. Spec 覆盖率：**
- ✅ 订单 API - Task 1
- ✅ 积分 API - Task 2
- ✅ 优惠券 API - Task 3
- ✅ 支付 API - Task 4
- ✅ 商品 API - Task 5
- ✅ 购物车 API - 已存在且正确
- ⚠️ 地址 API - 已存在但未修改（可能需要后端支持）
- ✅ 验证连接 - Task 6
- ✅ 文档 - Task 8

**2. 占位符扫描：**
- ✅ 无 "TBD"、"TODO"、"implement later"
- ✅ 所有代码步骤都有实际代码
- ✅ 所有路径都是确切的

**3. 类型一致性：**
- ✅ API 路径统一使用单数形式（/order、/product）
- ✅ 类型定义使用驼峰命名
- ✅ 接口方法名与后端控制器方法对应

---

## 执行选项

计划已完成并保存到 `docs/superpowers/plans/2026-09-10-miniapp-api-adaptation.md`。

两种执行选项：

**1. Subagent-Driven（推荐）** - 每个任务派发一个新 subagent，任务间审查，快速迭代

**2. Inline Execution** - 在本会话中使用 executing-plans 执行，批量执行并在检查点审查

选择哪种方式？