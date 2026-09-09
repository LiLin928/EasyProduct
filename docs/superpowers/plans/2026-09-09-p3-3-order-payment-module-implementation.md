# P3.3 订单支付模块实施计划（精简版）

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** 实现小程序商城订单支付模块的数据模型层，为后续业务逻辑开发打好基础。

**Architecture:** 采用简单分层架构（Entity → Service → Controller），本计划聚焦数据模型层。

**Tech Stack:** .NET 8 + SqlSugar 5.1.4

---

## 说明

**本计划仅包含数据模型层（实体类 + 枚举类 + DTOs）的开发。**

**理由：**
1. 数据模型层是独立的基础设施，可以单独开发和测试
2. 业务逻辑层涉及复杂的业务规则和跨服务协调，需要单独规划
3. 控制器层依赖业务逻辑层，需要后续开发

**后续计划：**
- P3.3.2: 业务逻辑层（订单服务）
- P3.3.3: 业务逻辑层（支付服务）
- P3.3.4: 业务逻辑层（退款服务）
- P3.3.5: 控制器层

---

## 文件清单

**枚举类（5个）：**
- `EasyProduct.Models/Enums/Mall/OrderStatus.cs`
- `EasyProduct.Models/Enums/Mall/PaymentMethod.cs`
- `EasyProduct.Models/Enums/Mall/PaymentStatus.cs`
- `EasyProduct.Models/Enums/Mall/RefundType.cs`
- `EasyProduct.Models/Enums/Mall/RefundStatus.cs`

**实体类（4个）：**
- `EasyProduct.Models/Entitys/Mall/Order.cs`
- `EasyProduct.Models/Entitys/Mall/OrderItem.cs`
- `EasyProduct.Models/Entitys/Mall/Payment.cs`
- `EasyProduct.Models/Entitys/Mall/Refund.cs`

**DTOs（~20个）：**
详见各任务

---

## 阶段 1：枚举类

### Task 1: 创建所有枚举类

**Files:**
- Create: `EasyProduct.Models/Enums/Mall/OrderStatus.cs`
- Create: `EasyProduct.Models/Enums/Mall/PaymentMethod.cs`
- Create: `EasyProduct.Models/Enums/Mall/PaymentStatus.cs`
- Create: `EasyProduct.Models/Enums/Mall/RefundType.cs`
- Create: `EasyProduct.Models/Enums/Mall/RefundStatus.cs`

- [ ] **Step 1: 创建 OrderStatus 枚举**

```csharp
namespace EasyProduct.Models.Enums.Mall;

/// <summary>
/// 订单状态枚举
/// </summary>
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

- [ ] **Step 2: 创建 PaymentMethod 枚举**

```csharp
namespace EasyProduct.Models.Enums.Mall;

/// <summary>
/// 支付方式枚举
/// </summary>
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

- [ ] **Step 3: 创建 PaymentStatus 枚举**

```csharp
namespace EasyProduct.Models.Enums.Mall;

/// <summary>
/// 支付状态枚举
/// </summary>
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

- [ ] **Step 4: 创建 RefundType 枚举**

```csharp
namespace EasyProduct.Models.Enums.Mall;

/// <summary>
/// 退款类型枚举
/// </summary>
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

- [ ] **Step 5: 创建 RefundStatus 枚举**

```csharp
namespace EasyProduct.Models.Enums.Mall;

/// <summary>
/// 退款状态枚举
/// </summary>
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

- [ ] **Step 6: 验证文件创建成功**

Run: `ls -la EasyProduct.WebApi/EasyProduct.Models/Enums/Mall/`
Expected: 所有枚举文件存在

- [ ] **Step 7: 编译检查**

Run: `cd EasyProduct.WebApi && dotnet build`
Expected: Build succeeded

- [ ] **Step 8: 提交**

```bash
git add EasyProduct.WebApi/EasyProduct.Models/Enums/Mall/
git commit -m "feat(models): 添加订单支付退款相关枚举"
```

---

## 阶段 2：实体类

### Task 2: 创建 Order 实体类

**Files:**
- Create: `EasyProduct.Models/Entitys/Mall/Order.cs`

- [ ] **Step 1: 创建 Order 实体类**

```csharp
using SqlSugar;
using EasyProduct.Models.Enums.Mall;
using EasyProduct.Models.Entitys.Base;

namespace EasyProduct.Models.Entitys.Mall;

/// <summary>
/// 订单实体类
/// </summary>
[SugarTable("mall_order", "订单表")]
public class Order : BaseEntity
{
    /// <summary>
    /// 订单编号
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = false)]
    public string OrderNo { get; set; } = string.Empty;

    /// <summary>
    /// 会员ID
    /// </summary>
    [SugarColumn(Length = 36, IsNullable = false)]
    public string MemberId { get; set; } = string.Empty;

    /// <summary>
    /// 收货人姓名
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = false)]
    public string ReceiverName { get; set; } = string.Empty;

    /// <summary>
    /// 收货人电话
    /// </summary>
    [SugarColumn(Length = 20, IsNullable = false)]
    public string ReceiverPhone { get; set; } = string.Empty;

    /// <summary>
    /// 收货地址
    /// </summary>
    [SugarColumn(Length = 200, IsNullable = false)]
    public string ReceiverAddress { get; set; } = string.Empty;

    /// <summary>
    /// 订单总金额
    /// </summary>
    [SugarColumn(Length = 18, DecimalDigits = 2, IsNullable = false)]
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// 实付金额
    /// </summary>
    [SugarColumn(Length = 18, DecimalDigits = 2, IsNullable = false)]
    public decimal PayAmount { get; set; }

    /// <summary>
    /// 运费
    /// </summary>
    [SugarColumn(Length = 18, DecimalDigits = 2, IsNullable = false)]
    public decimal FreightAmount { get; set; }

    /// <summary>
    /// 优惠金额
    /// </summary>
    [SugarColumn(Length = 18, DecimalDigits = 2, IsNullable = false)]
    public decimal DiscountAmount { get; set; }

    /// <summary>
    /// 订单状态
    /// </summary>
    public OrderStatus Status { get; set; } = OrderStatus.PendingPayment;

    /// <summary>
    /// 支付时间
    /// </summary>
    [SugarColumn(IsNullable = true)]
    public DateTime? PaymentTime { get; set; }

    /// <summary>
    /// 发货时间
    /// </summary>
    [SugarColumn(IsNullable = true)]
    public DateTime? DeliveryTime { get; set; }

    /// <summary>
    /// 收货时间
    /// </summary>
    [SugarColumn(IsNullable = true)]
    public DateTime? ReceiveTime { get; set; }

    /// <summary>
    /// 完成时间
    /// </summary>
    [SugarColumn(IsNullable = true)]
    public DateTime? CompleteTime { get; set; }

    /// <summary>
    /// 取消时间
    /// </summary>
    [SugarColumn(IsNullable = true)]
    public DateTime? CancelTime { get; set; }

    /// <summary>
    /// 物流公司
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = true)]
    public string? LogisticsCompany { get; set; }

    /// <summary>
    /// 物流单号
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = true)]
    public string? LogisticsNo { get; set; }

    /// <summary>
    /// 优惠券ID
    /// </summary>
    [SugarColumn(IsNullable = true)]
    public Guid? CouponId { get; set; }

    /// <summary>
    /// 优惠券名称
    /// </summary>
    [SugarColumn(Length = 100, IsNullable = true)]
    public string? CouponName { get; set; }

    /// <summary>
    /// 会员备注
    /// </summary>
    [SugarColumn(Length = 500, IsNullable = true)]
    public string? Remark { get; set; }

    /// <summary>
    /// 后台备注
    /// </summary>
    [SugarColumn(Length = 500, IsNullable = true)]
    public string? AdminRemark { get; set; }

    /// <summary>
    /// 订单来源（cart/direct）
    /// </summary>
    [SugarColumn(Length = 20, IsNullable = false)]
    public string Source { get; set; } = "cart";
}
```

- [ ] **Step 2: 验证文件创建成功**

Run: `ls -la EasyProduct.WebApi/EasyProduct.Models/Entitys/Mall/Order.cs`
Expected: 文件存在

- [ ] **Step 3: 编译检查**

Run: `cd EasyProduct.WebApi && dotnet build`
Expected: Build succeeded

- [ ] **Step 4: 提交**

```bash
git add EasyProduct.WebApi/EasyProduct.Models/Entitys/Mall/Order.cs
git commit -m "feat(models): 添加订单实体类"
```

---

### Task 3: 创建 OrderItem 实体类

**Files:**
- Create: `EasyProduct.Models/Entitys/Mall/OrderItem.cs`

- [ ] **Step 1: 创建 OrderItem 实体类**

```csharp
using SqlSugar;
using EasyProduct.Models.Entitys.Base;

namespace EasyProduct.Models.Entitys.Mall;

/// <summary>
/// 订单项实体类
/// </summary>
[SugarTable("mall_order_item", "订单项表")]
public class OrderItem : BaseEntity
{
    /// <summary>
    /// 订单ID
    /// </summary>
    [SugarColumn(Length = 36, IsNullable = false)]
    public string OrderId { get; set; } = string.Empty;

    /// <summary>
    /// SKU ID
    /// </summary>
    [SugarColumn(Length = 36, IsNullable = false)]
    public string SkuId { get; set; } = string.Empty;

    /// <summary>
    /// 商品名称
    /// </summary>
    [SugarColumn(Length = 200, IsNullable = false)]
    public string ProductName { get; set; } = string.Empty;

    /// <summary>
    /// SKU名称
    /// </summary>
    [SugarColumn(Length = 200, IsNullable = false)]
    public string SkuName { get; set; } = string.Empty;

    /// <summary>
    /// SKU规格（JSON）
    /// </summary>
    [SugarColumn(Length = 500, IsNullable = true)]
    public string? SkuSpec { get; set; }

    /// <summary>
    /// 商品图片
    /// </summary>
    [SugarColumn(Length = 500, IsNullable = false)]
    public string ProductImage { get; set; } = string.Empty;

    /// <summary>
    /// 单价
    /// </summary>
    [SugarColumn(Length = 18, DecimalDigits = 2, IsNullable = false)]
    public decimal Price { get; set; }

    /// <summary>
    /// 数量
    /// </summary>
    [SugarColumn(IsNullable = false)]
    public int Quantity { get; set; }

    /// <summary>
    /// 小计
    /// </summary>
    [SugarColumn(Length = 18, DecimalDigits = 2, IsNullable = false)]
    public decimal Subtotal { get; set; }

    /// <summary>
    /// 是否已退款（0否1是）
    /// </summary>
    [SugarColumn(IsNullable = false)]
    public int IsRefunded { get; set; } = 0;

    /// <summary>
    /// 已退款数量
    /// </summary>
    [SugarColumn(IsNullable = false)]
    public int RefundQuantity { get; set; } = 0;
}
```

- [ ] **Step 2: 验证文件创建成功**

Run: `ls -la EasyProduct.WebApi/EasyProduct.Models/Entitys/Mall/OrderItem.cs`
Expected: 文件存在

- [ ] **Step 3: 编译检查**

Run: `cd EasyProduct.WebApi && dotnet build`
Expected: Build succeeded

- [ ] **Step 4: 提交**

```bash
git add EasyProduct.WebApi/EasyProduct.Models/Entitys/Mall/OrderItem.cs
git commit -m "feat(models): 添加订单项实体类"
```

---

### Task 4: 创建 Payment 实体类

**Files:**
- Create: `EasyProduct.Models/Entitys/Mall/Payment.cs`

- [ ] **Step 1: 创建 Payment 实体类**

```csharp
using SqlSugar;
using EasyProduct.Models.Enums.Mall;
using EasyProduct.Models.Entitys.Base;

namespace EasyProduct.Models.Entitys.Mall;

/// <summary>
/// 支付记录实体类
/// </summary>
[SugarTable("mall_payment", "支付记录表")]
public class Payment : BaseEntity
{
    /// <summary>
    /// 支付单号
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = false)]
    public string PaymentNo { get; set; } = string.Empty;

    /// <summary>
    /// 订单ID
    /// </summary>
    [SugarColumn(Length = 36, IsNullable = false)]
    public string OrderId { get; set; } = string.Empty;

    /// <summary>
    /// 会员ID
    /// </summary>
    [SugarColumn(Length = 36, IsNullable = false)]
    public string MemberId { get; set; } = string.Empty;

    /// <summary>
    /// 支付金额
    /// </summary>
    [SugarColumn(Length = 18, DecimalDigits = 2, IsNullable = false)]
    public decimal Amount { get; set; }

    /// <summary>
    /// 支付方式
    /// </summary>
    public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.WechatPay;

    /// <summary>
    /// 支付渠道
    /// </summary>
    [SugarColumn(Length = 20, IsNullable = false)]
    public string PaymentChannel { get; set; } = string.Empty;

    /// <summary>
    /// 支付状态
    /// </summary>
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

    /// <summary>
    /// 第三方交易号
    /// </summary>
    [SugarColumn(Length = 100, IsNullable = true)]
    public string? ThirdPartyNo { get; set; }

    /// <summary>
    /// 支付时间
    /// </summary>
    [SugarColumn(IsNullable = true)]
    public DateTime? PaymentTime { get; set; }
}
```

- [ ] **Step 2: 验证文件创建成功**

Run: `ls -la EasyProduct.WebApi/EasyProduct.Models/Entitys/Mall/Payment.cs`
Expected: 文件存在

- [ ] **Step 3: 编译检查**

Run: `cd EasyProduct.WebApi && dotnet build`
Expected: Build succeeded

- [ ] **Step 4: 提交**

```bash
git add EasyProduct.WebApi/EasyProduct.Models/Entitys/Mall/Payment.cs
git commit -m "feat(models): 添加支付记录实体类"
```

---

### Task 5: 创建 Refund 实体类

**Files:**
- Create: `EasyProduct.Models/Entitys/Mall/Refund.cs`

- [ ] **Step 1: 创建 Refund 实体类**

```csharp
using SqlSugar;
using EasyProduct.Models.Enums.Mall;
using EasyProduct.Models.Entitys.Base;

namespace EasyProduct.Models.Entitys.Mall;

/// <summary>
/// 退款记录实体类
/// </summary>
[SugarTable("mall_refund", "退款记录表")]
public class Refund : BaseEntity
{
    /// <summary>
    /// 退款单号
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = false)]
    public string RefundNo { get; set; } = string.Empty;

    /// <summary>
    /// 订单ID
    /// </summary>
    [SugarColumn(Length = 36, IsNullable = false)]
    public string OrderId { get; set; } = string.Empty;

    /// <summary>
    /// 订单项ID
    /// </summary>
    [SugarColumn(Length = 36, IsNullable = false)]
    public string OrderItemId { get; set; } = string.Empty;

    /// <summary>
    /// 会员ID
    /// </summary>
    [SugarColumn(Length = 36, IsNullable = false)]
    public string MemberId { get; set; } = string.Empty;

    /// <summary>
    /// 退款类型
    /// </summary>
    public RefundType RefundType { get; set; } = RefundType.RefundOnly;

    /// <summary>
    /// 退款金额
    /// </summary>
    [SugarColumn(Length = 18, DecimalDigits = 2, IsNullable = false)]
    public decimal RefundAmount { get; set; }

    /// <summary>
    /// 退款数量
    /// </summary>
    [SugarColumn(IsNullable = false)]
    public int RefundQuantity { get; set; }

    /// <summary>
    /// 退款原因
    /// </summary>
    [SugarColumn(Length = 500, IsNullable = false)]
    public string RefundReason { get; set; } = string.Empty;

    /// <summary>
    /// 退款状态
    /// </summary>
    public RefundStatus Status { get; set; } = RefundStatus.Pending;

    /// <summary>
    /// 物流公司
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = true)]
    public string? LogisticsCompany { get; set; }

    /// <summary>
    /// 物流单号
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = true)]
    public string? LogisticsNo { get; set; }

    /// <summary>
    /// 审核备注
    /// </summary>
    [SugarColumn(Length = 500, IsNullable = true)]
    public string? AuditRemark { get; set; }

    /// <summary>
    /// 审核时间
    /// </summary>
    [SugarColumn(IsNullable = true)]
    public DateTime? AuditTime { get; set; }

    /// <summary>
    /// 退款时间
    /// </summary>
    [SugarColumn(IsNullable = true)]
    public DateTime? RefundTime { get; set; }
}
```

- [ ] **Step 2: 验证文件创建成功**

Run: `ls -la EasyProduct.WebApi/EasyProduct.Models/Entitys/Mall/Refund.cs`
Expected: 文件存在

- [ ] **Step 3: 编译检查**

Run: `cd EasyProduct.WebApi && dotnet build`
Expected: Build succeeded

- [ ] **Step 4: 提交**

```bash
git add EasyProduct.WebApi/EasyProduct.Models/Entitys/Mall/Refund.cs
git commit -m "feat(models): 添加退款记录实体类"
```

---

## 阶段 3：DTOs

**说明：** 由于 DTOs 数量较多（~20个），建议使用子代理批量创建。此处仅列出文件清单，具体实现代码详见设计规范。

### Task 6: 创建订单查询 DTOs（批量任务）

**Files:**
- Create: `EasyProduct.Models/Dto/Mall/Order/OrderQuery.cs`
- Create: `EasyProduct.Models/Dto/Mall/Order/OrderListDto.cs`
- Create: `EasyProduct.Models/Dto/Mall/Order/OrderDetailDto.cs`
- Create: `EasyProduct.Models/Dto/Mall/Order/OrderItemDto.cs`

**建议：** 使用子代理创建这些文件，代码详见设计规范第 3.1 节。

---

### Task 7: 创建订单操作 DTOs（批量任务）

**Files:**
- Create: `EasyProduct.Models/Dto/Mall/Order/CreateOrderFromCartDto.cs`
- Create: `EasyProduct.Models/Dto/Mall/Order/CreateOrderDirectDto.cs`
- Create: `EasyProduct.Models/Dto/Mall/Order/CreateOrderResultDto.cs`
- Create: `EasyProduct.Models/Dto/Mall/Order/OrderCancelDto.cs`
- Create: `EasyProduct.Models/Dto/Mall/Order/OrderDeliverDto.cs`
- Create: `EasyProduct.Models/Dto/Mall/Order/OrderStatisticsDto.cs`

**建议：** 使用子代理创建这些文件，代码详见设计规范第 5 节。

---

### Task 8: 创建支付 DTOs（批量任务）

**Files:**
- Create: `EasyProduct.Models/Dto/Mall/Payment/PaymentWechatResultDto.cs`

**建议：** 使用子代理创建这些文件，代码详见设计规范第 5 节。

---

### Task 9: 创建退款 DTOs（批量任务）

**Files:**
- Create: `EasyProduct.Models/Dto/Mall/Refund/RefundApplyDto.cs`
- Create: `EasyProduct.Models/Dto/Mall/Refund/RefundQuery.cs`
- Create: `EasyProduct.Models/Dto/Mall/Refund/RefundListDto.cs`
- Create: `EasyProduct.Models/Dto/Mall/Refund/RefundDetailDto.cs`
- Create: `EasyProduct.Models/Dto/Mall/Refund/RefundAuditDto.cs`
- Create: `EasyProduct.Models/Dto/Mall/Refund/RefundLogisticsDto.cs`
- Create: `EasyProduct.Models/Dto/Mall/Refund/RefundInspectDto.cs`

**建议：** 使用子代理创建这些文件，代码详见设计规范第 5 节。

---

### Task 10: 编译验证

- [ ] **编译整个解决方案**

Run: `cd EasyProduct.WebApi && dotnet build`
Expected: 0 个错误，0 个新增警告

---

## 总结

**已完成：**
- ✅ 5 个枚举类
- ✅ 4 个实体类
- ✅ ~20 个 DTOs（建议使用子代理批量创建）

**预计工作量：**
- 枚举类：30 分钟
- 实体类：1 小时
- DTOs：1 小时
- **总计：** 2.5 小时

**下一步：**
完成数据模型层后，继续开发：
1. P3.3.2: 业务逻辑层（订单服务）
2. P3.3.3: 业务逻辑层（支付服务）
3. P3.3.4: 业务逻辑层（退款服务）
4. P3.3.5: 控制器层

---

## 执行方式选择

**Plan complete and saved to `docs/superpowers/plans/2026-09-09-p3-3-order-payment-module-implementation.md`.**

**Two execution options:**

**1. Subagent-Driven (recommended)** - I dispatch a fresh subagent per task, review between tasks, fast iteration

**2. Inline Execution** - Execute tasks in this session using executing-plans, batch execution with checkpoints

**Which approach?**