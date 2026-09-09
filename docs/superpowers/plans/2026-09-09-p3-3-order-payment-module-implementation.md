# P3.3 订单支付模块实施计划

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** 实现小程序商城订单支付模块，支持订单创建、支付、发货、退款等完整业务流程。

**Architecture:** 采用简单分层架构（Entity → Service → Controller），符合 EasyProject 现有架构模式。订单服务为核心，协调库存、优惠券、会员等服务。

**Tech Stack:** .NET 8 + SqlSugar 5.1.4 + Mapster 10.x + xUnit

---

## 文件结构

### 数据模型层（Entitys + Enums + DTOs）

**实体类（4个）：**
- `EasyProduct.Models/Entitys/Mall/Order.cs` - 订单主表
- `EasyProduct.Models/Entitys/Mall/OrderItem.cs` - 订单项表
- `EasyProduct.Models/Entitys/Mall/Payment.cs` - 支付记录表
- `EasyProduct.Models/Entitys/Mall/Refund.cs` - 退款记录表

**枚举类（5个）：**
- `EasyProduct.Models/Enums/Mall/OrderStatus.cs` - 订单状态
- `EasyProduct.Models/Enums/Mall/PaymentMethod.cs` - 支付方式
- `EasyProduct.Models/Enums/Mall/PaymentStatus.cs` - 支付状态
- `EasyProduct.Models/Enums/Mall/RefundType.cs` - 退款类型
- `EasyProduct.Models/Enums/Mall/RefundStatus.cs` - 退款状态

**DTOs（约20个）：**
- 订单查询 DTO：`OrderQuery.cs`, `OrderListDto.cs`, `OrderDetailDto.cs`, `OrderStatisticsDto.cs`
- 订单创建 DTO：`CreateOrderFromCartDto.cs`, `CreateOrderDirectDto.cs`, `CreateOrderResultDto.cs`
- 订单操作 DTO：`OrderCancelDto.cs`, `OrderDeliverDto.cs`, `OrderReceiveDto.cs`
- 支付 DTO：`PaymentWechatDto.cs`, `PaymentBalanceDto.cs`, `PaymentResultDto.cs`
- 退款 DTO：`RefundApplyDto.cs`, `RefundQuery.cs`, `RefundListDto.cs`, `RefundDetailDto.cs`, `RefundAuditDto.cs`, `RefundLogisticsDto.cs`, `RefundInspectDto.cs`

### 业务逻辑层（Services）

**接口（3个）：**
- `EasyProduct.Business/Mall/IOrderService.cs`
- `EasyProduct.Business/Mall/IPaymentService.cs`
- `EasyProduct.Business/Mall/IRefundService.cs`

**实现类（3个）：**
- `EasyProduct.Business/Mall/OrderService.cs`
- `EasyProduct.Business/Mall/PaymentService.cs`
- `EasyProduct.Business/Mall/RefundService.cs`

### 控制器层（Controllers）

**小程序端（2个）：**
- `EasyProduct.Web/Controllers/App/Mall/OrderController.cs`
- `EasyProduct.Web/Controllers/App/Mall/RefundController.cs`

**管理端（2个）：**
- `EasyProduct.Web/Controllers/Admin/Mall/OrderController.cs`
- `EasyProduct.Web/Controllers/Admin/Mall/RefundController.cs`

---

## 阶段 1：数据模型层（枚举类）

### Task 1: 创建订单状态枚举

**Files:**
- Create: `EasyProduct.Models/Enums/Mall/OrderStatus.cs`

- [ ] **Step 1: 创建 OrderStatus 枚举类**

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

- [ ] **Step 2: 验证文件创建成功**

Run: `ls -la EasyProduct.WebApi/EasyProduct.Models/Enums/Mall/OrderStatus.cs`
Expected: 文件存在

- [ ] **Step 3: 提交**

```bash
git add EasyProduct.WebApi/EasyProduct.Models/Enums/Mall/OrderStatus.cs
git commit -m "feat(models): 添加订单状态枚举"
```

---

### Task 2: 创建支付相关枚举

**Files:**
- Create: `EasyProduct.Models/Enums/Mall/PaymentMethod.cs`
- Create: `EasyProduct.Models/Enums/Mall/PaymentStatus.cs`

- [ ] **Step 1: 创建 PaymentMethod 枚举类**

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

- [ ] **Step 2: 创建 PaymentStatus 枚举类**

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

- [ ] **Step 3: 验证文件创建成功**

Run: `ls -la EasyProduct.WebApi/EasyProduct.Models/Enums/Mall/`
Expected: PaymentMethod.cs 和 PaymentStatus.cs 存在

- [ ] **Step 4: 提交**

```bash
git add EasyProduct.WebApi/EasyProduct.Models/Enums/Mall/PaymentMethod.cs EasyProduct.WebApi/EasyProduct.Models/Enums/Mall/PaymentStatus.cs
git commit -m "feat(models): 添加支付方式和支付状态枚举"
```

---

### Task 3: 创建退款相关枚举

**Files:**
- Create: `EasyProduct.Models/Enums/Mall/RefundType.cs`
- Create: `EasyProduct.Models/Enums/Mall/RefundStatus.cs`

- [ ] **Step 1: 创建 RefundType 枚举类**

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

- [ ] **Step 2: 创建 RefundStatus 枚举类**

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

- [ ] **Step 3: 验证文件创建成功**

Run: `ls -la EasyProduct.WebApi/EasyProduct.Models/Enums/Mall/`
Expected: RefundType.cs 和 RefundStatus.cs 存在

- [ ] **Step 4: 提交**

```bash
git add EasyProduct.WebApi/EasyProduct.Models/Enums/Mall/RefundType.cs EasyProduct.WebApi/EasyProduct.Models/Enums/Mall/RefundStatus.cs
git commit -m "feat(models): 添加退款类型和退款状态枚举"
```

---

## 阶段 2：数据模型层（实体类）

### Task 4: 创建订单实体类

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

### Task 5: 创建订单项实体类

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

### Task 6: 创建支付记录实体类

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

### Task 7: 创建退款记录实体类

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

## 阶段 3：数据模型层（DTOs - 订单查询）

### Task 8: 创建订单查询 DTO

**Files:**
- Create: `EasyProduct.Models/Dto/Mall/Order/OrderQuery.cs`
- Create: `EasyProduct.Models/Dto/Mall/Order/OrderListDto.cs`
- Create: `EasyProduct.Models/Dto/Mall/Order/OrderDetailDto.cs`

- [ ] **Step 1: 创建 OrderQuery DTO**

```csharp
using EasyProduct.Common.Base;

namespace EasyProduct.Models.Dto.Mall.Order;

/// <summary>
/// 订单查询参数
/// </summary>
public class OrderQuery : PageQuery
{
    /// <summary>
    /// 订单编号
    /// </summary>
    public string? OrderNo { get; set; }

    /// <summary>
    /// 会员ID
    /// </summary>
    public string? MemberId { get; set; }

    /// <summary>
    /// 订单状态
    /// </summary>
    public int? Status { get; set; }

    /// <summary>
    /// 关键词（会员名称/电话）
    /// </summary>
    public string? Keyword { get; set; }

    /// <summary>
    /// 开始时间
    /// </summary>
    public DateTime? StartTime { get; set; }

    /// <summary>
    /// 结束时间
    /// </summary>
    public DateTime? EndTime { get; set; }
}
```

- [ ] **Step 2: 创建 OrderListDto**

```csharp
using EasyProduct.Models.Enums.Mall;

namespace EasyProduct.Models.Dto.Mall.Order;

/// <summary>
/// 订单列表项 DTO
/// </summary>
public class OrderListDto
{
    /// <summary>
    /// 订单ID
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// 订单编号
    /// </summary>
    public string OrderNo { get; set; } = string.Empty;

    /// <summary>
    /// 会员名称
    /// </summary>
    public string? MemberName { get; set; }

    /// <summary>
    /// 会员电话
    /// </summary>
    public string? MemberPhone { get; set; }

    /// <summary>
    /// 订单状态
    /// </summary>
    public OrderStatus Status { get; set; }

    /// <summary>
    /// 状态文本
    /// </summary>
    public string StatusText { get; set; } = string.Empty;

    /// <summary>
    /// 订单总金额
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// 实付金额
    /// </summary>
    public decimal PayAmount { get; set; }

    /// <summary>
    /// 商品项数量
    /// </summary>
    public int ItemCount { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreateTime { get; set; }
}
```

- [ ] **Step 3: 创建 OrderDetailDto**

```csharp
using EasyProduct.Models.Enums.Mall;

namespace EasyProduct.Models.Dto.Mall.Order;

/// <summary>
/// 订单详情 DTO
/// </summary>
public class OrderDetailDto
{
    /// <summary>
    /// 订单ID
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// 订单编号
    /// </summary>
    public string OrderNo { get; set; } = string.Empty;

    /// <summary>
    /// 会员ID
    /// </summary>
    public string MemberId { get; set; } = string.Empty;

    /// <summary>
    /// 会员名称
    /// </summary>
    public string? MemberName { get; set; }

    /// <summary>
    /// 会员电话
    /// </summary>
    public string? MemberPhone { get; set; }

    /// <summary>
    /// 订单状态
    /// </summary>
    public OrderStatus Status { get; set; }

    /// <summary>
    /// 状态文本
    /// </summary>
    public string StatusText { get; set; } = string.Empty;

    /// <summary>
    /// 收货人姓名
    /// </summary>
    public string ReceiverName { get; set; } = string.Empty;

    /// <summary>
    /// 收货人电话
    /// </summary>
    public string ReceiverPhone { get; set; } = string.Empty;

    /// <summary>
    /// 收货地址
    /// </summary>
    public string ReceiverAddress { get; set; } = string.Empty;

    /// <summary>
    /// 订单总金额
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// 运费
    /// </summary>
    public decimal FreightAmount { get; set; }

    /// <summary>
    /// 优惠金额
    /// </summary>
    public decimal DiscountAmount { get; set; }

    /// <summary>
    /// 实付金额
    /// </summary>
    public decimal PayAmount { get; set; }

    /// <summary>
    /// 优惠券名称
    /// </summary>
    public string? CouponName { get; set; }

    /// <summary>
    /// 物流公司
    /// </summary>
    public string? LogisticsCompany { get; set; }

    /// <summary>
    /// 物流单号
    /// </summary>
    public string? LogisticsNo { get; set; }

    /// <summary>
    /// 会员备注
    /// </summary>
    public string? Remark { get; set; }

    /// <summary>
    /// 后台备注
    /// </summary>
    public string? AdminRemark { get; set; }

    /// <summary>
    /// 订单项列表
    /// </summary>
    public List<OrderItemDto> Items { get; set; } = new();

    /// <summary>
    /// 支付时间
    /// </summary>
    public DateTime? PaymentTime { get; set; }

    /// <summary>
    /// 发货时间
    /// </summary>
    public DateTime? DeliveryTime { get; set; }

    /// <summary>
    /// 收货时间
    /// </summary>
    public DateTime? ReceiveTime { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreateTime { get; set; }
}
```

- [ ] **Step 4: 创建 OrderItemDto**

```csharp
namespace EasyProduct.Models.Dto.Mall.Order;

/// <summary>
/// 订单项 DTO
/// </summary>
public class OrderItemDto
{
    /// <summary>
    /// 订单项ID
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// SKU ID
    /// </summary>
    public string SkuId { get; set; } = string.Empty;

    /// <summary>
    /// 商品名称
    /// </summary>
    public string ProductName { get; set; } = string.Empty;

    /// <summary>
    /// SKU名称
    /// </summary>
    public string SkuName { get; set; } = string.Empty;

    /// <summary>
    /// SKU规格
    /// </summary>
    public string? SkuSpec { get; set; }

    /// <summary>
    /// 商品图片
    /// </summary>
    public string ProductImage { get; set; } = string.Empty;

    /// <summary>
    /// 单价
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// 数量
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// 小计
    /// </summary>
    public decimal Subtotal { get; set; }

    /// <summary>
    /// 是否已退款
    /// </summary>
    public bool IsRefunded { get; set; }

    /// <summary>
    /// 已退款数量
    /// </summary>
    public int RefundQuantity { get; set; }
}
```

- [ ] **Step 5: 验证文件创建成功**

Run: `ls -la EasyProduct.WebApi/EasyProduct.Models/Dto/Mall/Order/`
Expected: 所有文件存在

- [ ] **Step 6: 编译检查**

Run: `cd EasyProduct.WebApi && dotnet build`
Expected: Build succeeded

- [ ] **Step 7: 提交**

```bash
git add EasyProduct.WebApi/EasyProduct.Models/Dto/Mall/Order/
git commit -m "feat(models): 添加订单查询 DTO"
```

---

## 阶段 4：数据模型层（DTOs - 订单创建与操作）

### Task 9: 创建订单创建 DTO

**Files:**
- Create: `EasyProduct.Models/Dto/Mall/Order/CreateOrderFromCartDto.cs`
- Create: `EasyProduct.Models/Dto/Mall/Order/CreateOrderDirectDto.cs`
- Create: `EasyProduct.Models/Dto/Mall/Order/CreateOrderResultDto.cs`

- [ ] **Step 1: 创建 CreateOrderFromCartDto**

```csharp
using System.ComponentModel.DataAnnotations;

namespace EasyProduct.Models.Dto.Mall.Order;

/// <summary>
/// 从购物车创建订单 DTO
/// </summary>
public class CreateOrderFromCartDto
{
    /// <summary>
    /// 购物车项ID列表
    /// </summary>
    [Required(ErrorMessage = "购物车项ID不能为空")]
    [MinLength(1, ErrorMessage = "至少选择一个商品")]
    public List<string> CartIds { get; set; } = new();

    /// <summary>
    /// 收货人姓名
    /// </summary>
    [Required(ErrorMessage = "收货人姓名不能为空")]
    [StringLength(50, ErrorMessage = "收货人姓名长度不能超过50")]
    public string ReceiverName { get; set; } = string.Empty;

    /// <summary>
    /// 收货人电话
    /// </summary>
    [Required(ErrorMessage = "收货人电话不能为空")]
    [StringLength(20, ErrorMessage = "收货人电话长度不能超过20")]
    public string ReceiverPhone { get; set; } = string.Empty;

    /// <summary>
    /// 收货地址
    /// </summary>
    [Required(ErrorMessage = "收货地址不能为空")]
    [StringLength(200, ErrorMessage = "收货地址长度不能超过200")]
    public string ReceiverAddress { get; set; } = string.Empty;

    /// <summary>
    /// 优惠券ID
    /// </summary>
    public string? CouponId { get; set; }

    /// <summary>
    /// 订单备注
    /// </summary>
    [StringLength(500, ErrorMessage = "备注长度不能超过500")]
    public string? Remark { get; set; }
}
```

- [ ] **Step 2: 创建 CreateOrderDirectDto**

```csharp
using System.ComponentModel.DataAnnotations;

namespace EasyProduct.Models.Dto.Mall.Order;

/// <summary>
/// 直接购买创建订单 DTO
/// </summary>
public class CreateOrderDirectDto
{
    /// <summary>
    /// SKU ID
    /// </summary>
    [Required(ErrorMessage = "SKU ID不能为空")]
    public string SkuId { get; set; } = string.Empty;

    /// <summary>
    /// 数量
    /// </summary>
    [Required(ErrorMessage = "数量不能为空")]
    [Range(1, int.MaxValue, ErrorMessage = "数量必须大于0")]
    public int Quantity { get; set; }

    /// <summary>
    /// 收货人姓名
    /// </summary>
    [Required(ErrorMessage = "收货人姓名不能为空")]
    [StringLength(50, ErrorMessage = "收货人姓名长度不能超过50")]
    public string ReceiverName { get; set; } = string.Empty;

    /// <summary>
    /// 收货人电话
    /// </summary>
    [Required(ErrorMessage = "收货人电话不能为空")]
    [StringLength(20, ErrorMessage = "收货人电话长度不能超过20")]
    public string ReceiverPhone { get; set; } = string.Empty;

    /// <summary>
    /// 收货地址
    /// </summary>
    [Required(ErrorMessage = "收货地址不能为空")]
    [StringLength(200, ErrorMessage = "收货地址长度不能超过200")]
    public string ReceiverAddress { get; set; } = string.Empty;

    /// <summary>
    /// 优惠券ID
    /// </summary>
    public string? CouponId { get; set; }

    /// <summary>
    /// 订单备注
    /// </summary>
    [StringLength(500, ErrorMessage = "备注长度不能超过500")]
    public string? Remark { get; set; }
}
```

- [ ] **Step 3: 创建 CreateOrderResultDto**

```csharp
namespace EasyProduct.Models.Dto.Mall.Order;

/// <summary>
/// 创建订单结果 DTO
/// </summary>
public class CreateOrderResultDto
{
    /// <summary>
    /// 订单ID
    /// </summary>
    public string OrderId { get; set; } = string.Empty;

    /// <summary>
    /// 订单编号
    /// </summary>
    public string OrderNo { get; set; } = string.Empty;

    /// <summary>
    /// 订单总金额
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// 实付金额
    /// </summary>
    public decimal PayAmount { get; set; }
}
```

- [ ] **Step 4: 验证文件创建成功**

Run: `ls -la EasyProduct.WebApi/EasyProduct.Models/Dto/Mall/Order/`
Expected: 所有文件存在

- [ ] **Step 5: 编译检查**

Run: `cd EasyProduct.WebApi && dotnet build`
Expected: Build succeeded

- [ ] **Step 6: 提交**

```bash
git add EasyProduct.WebApi/EasyProduct.Models/Dto/Mall/Order/
git commit -m "feat(models): 添加订单创建 DTO"
```

---

### Task 10: 创建订单操作 DTO

**Files:**
- Create: `EasyProduct.Models/Dto/Mall/Order/OrderCancelDto.cs`
- Create: `EasyProduct.Models/Dto/Mall/Order/OrderDeliverDto.cs`
- Create: `EasyProduct.Models/Dto/Mall/Order/OrderStatisticsDto.cs`

- [ ] **Step 1: 创建 OrderCancelDto**

```csharp
using System.ComponentModel.DataAnnotations;

namespace EasyProduct.Models.Dto.Mall.Order;

/// <summary>
/// 订单取消 DTO
/// </summary>
public class OrderCancelDto
{
    /// <summary>
    /// 取消原因
    /// </summary>
    [StringLength(500, ErrorMessage = "取消原因长度不能超过500")]
    public string? CancelReason { get; set; }
}
```

- [ ] **Step 2: 创建 OrderDeliverDto**

```csharp
using System.ComponentModel.DataAnnotations;

namespace EasyProduct.Models.Dto.Mall.Order;

/// <summary>
/// 订单发货 DTO
/// </summary>
public class OrderDeliverDto
{
    /// <summary>
    /// 物流公司
    /// </summary>
    [Required(ErrorMessage = "物流公司不能为空")]
    [StringLength(50, ErrorMessage = "物流公司长度不能超过50")]
    public string LogisticsCompany { get; set; } = string.Empty;

    /// <summary>
    /// 物流单号
    /// </summary>
    [Required(ErrorMessage = "物流单号不能为空")]
    [StringLength(50, ErrorMessage = "物流单号长度不能超过50")]
    public string LogisticsNo { get; set; } = string.Empty;
}
```

- [ ] **Step 3: 创建 OrderStatisticsDto**

```csharp
namespace EasyProduct.Models.Dto.Mall.Order;

/// <summary>
/// 订单统计 DTO
/// </summary>
public class OrderStatisticsDto
{
    /// <summary>
    /// 总订单数
    /// </summary>
    public int TotalOrders { get; set; }

    /// <summary>
    /// 待付款订单数
    /// </summary>
    public int PendingPayment { get; set; }

    /// <summary>
    /// 待发货订单数
    /// </summary>
    public int PendingDelivery { get; set; }

    /// <summary>
    /// 待收货订单数
    /// </summary>
    public int PendingReceive { get; set; }

    /// <summary>
    /// 已完成订单数
    /// </summary>
    public int Completed { get; set; }

    /// <summary>
    /// 已取消订单数
    /// </summary>
    public int Cancelled { get; set; }

    /// <summary>
    /// 退款中订单数
    /// </summary>
    public int Refunding { get; set; }

    /// <summary>
    /// 总金额
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// 今日订单数
    /// </summary>
    public int TodayOrders { get; set; }

    /// <summary>
    /// 今日金额
    /// </summary>
    public decimal TodayAmount { get; set; }
}
```

- [ ] **Step 4: 验证文件创建成功**

Run: `ls -la EasyProduct.WebApi/EasyProduct.Models/Dto/Mall/Order/`
Expected: 所有文件存在

- [ ] **Step 5: 编译检查**

Run: `cd EasyProduct.WebApi && dotnet build`
Expected: Build succeeded

- [ ] **Step 6: 提交**

```bash
git add EasyProduct.WebApi/EasyProduct.Models/Dto/Mall/Order/
git commit -m "feat(models): 添加订单操作 DTO"
```

---

## 阶段 5：数据模型层（DTOs - 支付与退款）

### Task 11: 创建支付 DTO

**Files:**
- Create: `EasyProduct.Models/Dto/Mall/Payment/PaymentWechatResultDto.cs`
- Create: `EasyProduct.Models/Dto/Mall/Payment/PaymentBalanceDto.cs`

- [ ] **Step 1: 创建 PaymentWechatResultDto**

```csharp
namespace EasyProduct.Models.Dto.Mall.Payment;

/// <summary>
/// 微信支付结果 DTO
/// </summary>
public class PaymentWechatResultDto
{
    /// <summary>
    /// 时间戳
    /// </summary>
    public string TimeStamp { get; set; } = string.Empty;

    /// <summary>
    /// 随机字符串
    /// </summary>
    public string NonceStr { get; set; } = string.Empty;

    /// <summary>
    /// 订单详情扩展字符串
    /// </summary>
    public string Package { get; set; } = string.Empty;

    /// <summary>
    /// 签名方式
    /// </summary>
    public string SignType { get; set; } = "RSA";

    /// <summary>
    /// 签名
    /// </summary>
    public string PaySign { get; set; } = string.Empty;
}
```

- [ ] **Step 2: 验证文件创建成功**

Run: `ls -la EasyProduct.WebApi/EasyProduct.Models/Dto/Mall/Payment/`
Expected: 文件存在

- [ ] **Step 3: 编译检查**

Run: `cd EasyProduct.WebApi && dotnet build`
Expected: Build succeeded

- [ ] **Step 4: 提交**

```bash
git add EasyProduct.WebApi/EasyProduct.Models/Dto/Mall/Payment/
git commit -m "feat(models): 添加支付 DTO"
```

---

### Task 12: 创建退款 DTO

**Files:**
- Create: `EasyProduct.Models/Dto/Mall/Refund/RefundApplyDto.cs`
- Create: `EasyProduct.Models/Dto/Mall/Refund/RefundQuery.cs`
- Create: `EasyProduct.Models/Dto/Mall/Refund/RefundListDto.cs`
- Create: `EasyProduct.Models/Dto/Mall/Refund/RefundDetailDto.cs`
- Create: `EasyProduct.Models/Dto/Mall/Refund/RefundAuditDto.cs`
- Create: `EasyProduct.Models/Dto/Mall/Refund/RefundLogisticsDto.cs`
- Create: `EasyProduct.Models/Dto/Mall/Refund/RefundInspectDto.cs`

- [ ] **Step 1: 创建所有退款相关 DTO**

由于文件较多，我将分步创建。首先创建 RefundApplyDto：

```csharp
using System.ComponentModel.DataAnnotations;
using EasyProduct.Models.Enums.Mall;

namespace EasyProduct.Models.Dto.Mall.Refund;

/// <summary>
/// 申请退款 DTO
/// </summary>
public class RefundApplyDto
{
    /// <summary>
    /// 订单ID
    /// </summary>
    [Required(ErrorMessage = "订单ID不能为空")]
    public string OrderId { get; set; } = string.Empty;

    /// <summary>
    /// 订单项ID
    /// </summary>
    [Required(ErrorMessage = "订单项ID不能为空")]
    public string OrderItemId { get; set; } = string.Empty;

    /// <summary>
    /// 退款类型
    /// </summary>
    [Required(ErrorMessage = "退款类型不能为空")]
    public RefundType RefundType { get; set; }

    /// <summary>
    /// 退款数量
    /// </summary>
    [Required(ErrorMessage = "退款数量不能为空")]
    [Range(1, int.MaxValue, ErrorMessage = "退款数量必须大于0")]
    public int RefundQuantity { get; set; }

    /// <summary>
    /// 退款金额
    /// </summary>
    [Required(ErrorMessage = "退款金额不能为空")]
    [Range(0.01, double.MaxValue, ErrorMessage = "退款金额必须大于0")]
    public decimal RefundAmount { get; set; }

    /// <summary>
    /// 退款原因
    /// </summary>
    [Required(ErrorMessage = "退款原因不能为空")]
    [StringLength(500, ErrorMessage = "退款原因长度不能超过500")]
    public string RefundReason { get; set; } = string.Empty;
}
```

创建 RefundQuery：

```csharp
using EasyProduct.Common.Base;

namespace EasyProduct.Models.Dto.Mall.Refund;

/// <summary>
/// 退款查询参数
/// </summary>
public class RefundQuery : PageQuery
{
    /// <summary>
    /// 退款单号
    /// </summary>
    public string? RefundNo { get; set; }

    /// <summary>
    /// 订单编号
    /// </summary>
    public string? OrderNo { get; set; }

    /// <summary>
    /// 退款状态
    /// </summary>
    public int? Status { get; set; }

    /// <summary>
    /// 开始时间
    /// </summary>
    public DateTime? StartTime { get; set; }

    /// <summary>
    /// 结束时间
    /// </summary>
    public DateTime? EndTime { get; set; }
}
```

创建 RefundListDto：

```csharp
using EasyProduct.Models.Enums.Mall;

namespace EasyProduct.Models.Dto.Mall.Refund;

/// <summary>
/// 退款列表项 DTO
/// </summary>
public class RefundListDto
{
    /// <summary>
    /// 退款ID
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// 退款单号
    /// </summary>
    public string RefundNo { get; set; } = string.Empty;

    /// <summary>
    /// 订单编号
    /// </summary>
    public string OrderNo { get; set; } = string.Empty;

    /// <summary>
    /// 退款类型
    /// </summary>
    public RefundType RefundType { get; set; }

    /// <summary>
    /// 退款类型文本
    /// </summary>
    public string RefundTypeText { get; set; } = string.Empty;

    /// <summary>
    /// 退款金额
    /// </summary>
    public decimal RefundAmount { get; set; }

    /// <summary>
    /// 退款状态
    /// </summary>
    public RefundStatus Status { get; set; }

    /// <summary>
    /// 状态文本
    /// </summary>
    public string StatusText { get; set; } = string.Empty;

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreateTime { get; set; }
}
```

创建 RefundDetailDto：

```csharp
using EasyProduct.Models.Dto.Mall.Order;
using EasyProduct.Models.Enums.Mall;

namespace EasyProduct.Models.Dto.Mall.Refund;

/// <summary>
/// 退款详情 DTO
/// </summary>
public class RefundDetailDto
{
    /// <summary>
    /// 退款ID
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// 退款单号
    /// </summary>
    public string RefundNo { get; set; } = string.Empty;

    /// <summary>
    /// 订单编号
    /// </summary>
    public string OrderNo { get; set; } = string.Empty;

    /// <summary>
    /// 退款类型
    /// </summary>
    public RefundType RefundType { get; set; }

    /// <summary>
    /// 退款类型文本
    /// </summary>
    public string RefundTypeText { get; set; } = string.Empty;

    /// <summary>
    /// 退款数量
    /// </summary>
    public int RefundQuantity { get; set; }

    /// <summary>
    /// 退款金额
    /// </summary>
    public decimal RefundAmount { get; set; }

    /// <summary>
    /// 退款原因
    /// </summary>
    public string RefundReason { get; set; } = string.Empty;

    /// <summary>
    /// 退款状态
    /// </summary>
    public RefundStatus Status { get; set; }

    /// <summary>
    /// 状态文本
    /// </summary>
    public string StatusText { get; set; } = string.Empty;

    /// <summary>
    /// 审核备注
    /// </summary>
    public string? AuditRemark { get; set; }

    /// <summary>
    /// 物流公司
    /// </summary>
    public string? LogisticsCompany { get; set; }

    /// <summary>
    /// 物流单号
    /// </summary>
    public string? LogisticsNo { get; set; }

    /// <summary>
    /// 订单项信息
    /// </summary>
    public OrderItemDto? OrderItem { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreateTime { get; set; }
}
```

创建 RefundAuditDto：

```csharp
using System.ComponentModel.DataAnnotations;

namespace EasyProduct.Models.Dto.Mall.Refund;

/// <summary>
/// 退款审核 DTO
/// </summary>
public class RefundAuditDto
{
    /// <summary>
    /// 是否通过
    /// </summary>
    [Required(ErrorMessage = "审核结果不能为空")]
    public bool Approved { get; set; }

    /// <summary>
    /// 审核备注
    /// </summary>
    [StringLength(500, ErrorMessage = "审核备注长度不能超过500")]
    public string? AuditRemark { get; set; }
}
```

创建 RefundLogisticsDto：

```csharp
using System.ComponentModel.DataAnnotations;

namespace EasyProduct.Models.Dto.Mall.Refund;

/// <summary>
/// 退款物流 DTO
/// </summary>
public class RefundLogisticsDto
{
    /// <summary>
    /// 物流公司
    /// </summary>
    [Required(ErrorMessage = "物流公司不能为空")]
    [StringLength(50, ErrorMessage = "物流公司长度不能超过50")]
    public string LogisticsCompany { get; set; } = string.Empty;

    /// <summary>
    /// 物流单号
    /// </summary>
    [Required(ErrorMessage = "物流单号不能为空")]
    [StringLength(50, ErrorMessage = "物流单号长度不能超过50")]
    public string LogisticsNo { get; set; } = string.Empty;
}
```

创建 RefundInspectDto：

```csharp
using System.ComponentModel.DataAnnotations;

namespace EasyProduct.Models.Dto.Mall.Refund;

/// <summary>
/// 退款验货 DTO
/// </summary>
public class RefundInspectDto
{
    /// <summary>
    /// 是否验收通过
    /// </summary>
    [Required(ErrorMessage = "验收结果不能为空")]
    public bool Passed { get; set; }

    /// <summary>
    /// 验收备注
    /// </summary>
    [StringLength(500, ErrorMessage = "验收备注长度不能超过500")]
    public string? InspectRemark { get; set; }
}
```

- [ ] **Step 2: 验证文件创建成功**

Run: `ls -la EasyProduct.WebApi/EasyProduct.Models/Dto/Mall/Refund/`
Expected: 所有文件存在

- [ ] **Step 3: 编译检查**

Run: `cd EasyProduct.WebApi && dotnet build`
Expected: Build succeeded

- [ ] **Step 4: 提交**

```bash
git add EasyProduct.WebApi/EasyProduct.Models/Dto/Mall/Refund/
git commit -m "feat(models): 添加退款 DTO"
```

---

## 阶段 6：业务逻辑层（订单服务接口）

### Task 13: 创建订单服务接口

**Files:**
- Create: `EasyProduct.Business/Mall/IOrderService.cs`

- [ ] **Step 1: 创建 IOrderService 接口**

```csharp
using EasyProduct.Common.Base;
using EasyProduct.Models.Dto.Mall.Order;

namespace EasyProduct.Business.Mall;

/// <summary>
/// 订单服务接口
/// </summary>
/// <remarks>
/// 提供订单的创建、查询、修改、取消、发货、收货等功能
/// </remarks>
public interface IOrderService
{
    #region 订单创建

    /// <summary>
    /// 从购物车创建订单
    /// </summary>
    /// <param name="memberId">会员ID</param>
    /// <param name="dto">创建参数</param>
    /// <returns>创建结果</returns>
    Task<CreateOrderResultDto> CreateFromCartAsync(string memberId, CreateOrderFromCartDto dto);

    /// <summary>
    /// 直接购买创建订单
    /// </summary>
    /// <param name="memberId">会员ID</param>
    /// <param name="dto">创建参数</param>
    /// <returns>创建结果</returns>
    Task<CreateOrderResultDto> CreateDirectAsync(string memberId, CreateOrderDirectDto dto);

    #endregion

    #region 订单查询

    /// <summary>
    /// 获取会员订单列表（分页）
    /// </summary>
    /// <param name="memberId">会员ID</param>
    /// <param name="status">订单状态（可选）</param>
    /// <param name="pageIndex">页码</param>
    /// <param name="pageSize">每页数量</param>
    /// <returns>订单列表</returns>
    Task<PageResponse<OrderListDto>> GetMemberOrderListAsync(string memberId, int? status, int pageIndex, int pageSize);

    /// <summary>
    /// 获取订单列表（管理端，分页）
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>订单列表</returns>
    Task<PageResponse<OrderListDto>> GetListAsync(OrderQuery query);

    /// <summary>
    /// 获取订单详情
    /// </summary>
    /// <param name="id">订单ID</param>
    /// <returns>订单详情</returns>
    Task<OrderDetailDto> GetByIdAsync(string id);

    /// <summary>
    /// 获取订单统计
    /// </summary>
    /// <returns>订单统计</returns>
    Task<OrderStatisticsDto> GetStatisticsAsync();

    #endregion

    #region 订单操作

    /// <summary>
    /// 取消订单
    /// </summary>
    /// <param name="memberId">会员ID</param>
    /// <param name="id">订单ID</param>
    /// <param name="dto">取消参数</param>
    /// <returns>是否成功</returns>
    Task<bool> CancelAsync(string memberId, string id, OrderCancelDto dto);

    /// <summary>
    /// 取消订单（管理端）
    /// </summary>
    /// <param name="operatorId">操作人ID</param>
    /// <param name="id">订单ID</param>
    /// <param name="dto">取消参数</param>
    /// <returns>是否成功</returns>
    Task<bool> CancelByAdminAsync(string operatorId, string id, OrderCancelDto dto);

    /// <summary>
    /// 发货
    /// </summary>
    /// <param name="operatorId">操作人ID</param>
    /// <param name="id">订单ID</param>
    /// <param name="dto">发货参数</param>
    /// <returns>是否成功</returns>
    Task<bool> DeliverAsync(string operatorId, string id, OrderDeliverDto dto);

    /// <summary>
    /// 确认收货
    /// </summary>
    /// <param name="memberId">会员ID</param>
    /// <param name="id">订单ID</param>
    /// <returns>是否成功</returns>
    Task<bool> ReceiveAsync(string memberId, string id);

    /// <summary>
    /// 更新后台备注
    /// </summary>
    /// <param name="operatorId">操作人ID</param>
    /// <param name="id">订单ID</param>
    /// <param name="remark">备注内容</param>
    /// <returns>是否成功</returns>
    Task<bool> UpdateAdminRemarkAsync(string operatorId, string id, string remark);

    #endregion
}
```

- [ ] **Step 2: 验证文件创建成功**

Run: `ls -la EasyProduct.WebApi/EasyProduct.Business/Mall/IOrderService.cs`
Expected: 文件存在

- [ ] **Step 3: 编译检查**

Run: `cd EasyProduct.WebApi && dotnet build`
Expected: Build succeeded

- [ ] **Step 4: 提交**

```bash
git add EasyProduct.WebApi/EasyProduct.Business/Mall/IOrderService.cs
git commit -m "feat(api): 添加订单服务接口"
```

---

## 阶段 7：业务逻辑层（支付与退款服务接口）

### Task 14: 创建支付服务接口

**Files:**
- Create: `EasyProduct.Business/Mall/IPaymentService.cs`

- [ ] **Step 1: 创建 IPaymentService 接口**

```csharp
using EasyProduct.Models.Dto.Mall.Payment;

namespace EasyProduct.Business.Mall;

/// <summary>
/// 支付服务接口
/// </summary>
/// <remarks>
/// 提供微信支付、余额支付等功能
/// </remarks>
public interface IPaymentService
{
    /// <summary>
    /// 微信支付
    /// </summary>
    /// <param name="memberId">会员ID</param>
    /// <param name="orderId">订单ID</param>
    /// <returns>支付参数</returns>
    Task<PaymentWechatResultDto> WechatPayAsync(string memberId, string orderId);

    /// <summary>
    /// 余额支付
    /// </summary>
    /// <param name="memberId">会员ID</param>
    /// <param name="orderId">订单ID</param>
    /// <returns>是否成功</returns>
    Task<bool> BalancePayAsync(string memberId, string orderId);

    /// <summary>
    /// 微信支付回调处理
    /// </summary>
    /// <param name="orderId">订单ID</param>
    /// <param name="transactionId">第三方交易号</param>
    /// <returns>是否成功</returns>
    Task<bool> WechatPayCallbackAsync(string orderId, string transactionId);

    /// <summary>
    /// 微信退款
    /// </summary>
    /// <param name="orderId">订单ID</param>
    /// <param name="refundAmount">退款金额</param>
    /// <returns>是否成功</returns>
    Task<bool> WechatRefundAsync(string orderId, decimal refundAmount);
}
```

- [ ] **Step 2: 验证文件创建成功**

Run: `ls -la EasyProduct.WebApi/EasyProduct.Business/Mall/IPaymentService.cs`
Expected: 文件存在

- [ ] **Step 3: 编译检查**

Run: `cd EasyProduct.WebApi && dotnet build`
Expected: Build succeeded

- [ ] **Step 4: 提交**

```bash
git add EasyProduct.WebApi/EasyProduct.Business/Mall/IPaymentService.cs
git commit -m "feat(api): 添加支付服务接口"
```

---

### Task 15: 创建退款服务接口

**Files:**
- Create: `EasyProduct.Business/Mall/IRefundService.cs`

- [ ] **Step 1: 创建 IRefundService 接口**

```csharp
using EasyProduct.Common.Base;
using EasyProduct.Models.Dto.Mall.Refund;

namespace EasyProduct.Business.Mall;

/// <summary>
/// 退款服务接口
/// </summary>
/// <remarks>
/// 提供退款申请、审核、退货、退款等功能
/// </remarks>
public interface IRefundService
{
    #region 退款申请

    /// <summary>
    /// 申请退款
    /// </summary>
    /// <param name="memberId">会员ID</param>
    /// <param name="dto">申请参数</param>
    /// <returns>退款ID</returns>
    Task<string> ApplyAsync(string memberId, RefundApplyDto dto);

    #endregion

    #region 退款查询

    /// <summary>
    /// 获取会员退款列表（分页）
    /// </summary>
    /// <param name="memberId">会员ID</param>
    /// <param name="pageIndex">页码</param>
    /// <param name="pageSize">每页数量</param>
    /// <returns>退款列表</returns>
    Task<PageResponse<RefundListDto>> GetMemberRefundListAsync(string memberId, int pageIndex, int pageSize);

    /// <summary>
    /// 获取退款列表（管理端，分页）
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>退款列表</returns>
    Task<PageResponse<RefundListDto>> GetListAsync(RefundQuery query);

    /// <summary>
    /// 获取退款详情
    /// </summary>
    /// <param name="id">退款ID</param>
    /// <returns>退款详情</returns>
    Task<RefundDetailDto> GetByIdAsync(string id);

    #endregion

    #region 退款操作

    /// <summary>
    /// 审核退款
    /// </summary>
    /// <param name="operatorId">操作人ID</param>
    /// <param name="id">退款ID</param>
    /// <param name="dto">审核参数</param>
    /// <returns>是否成功</returns>
    Task<bool> AuditAsync(string operatorId, string id, RefundAuditDto dto);

    /// <summary>
    /// 填写退货物流信息
    /// </summary>
    /// <param name="memberId">会员ID</param>
    /// <param name="id">退款ID</param>
    /// <param name="dto">物流参数</param>
    /// <returns>是否成功</returns>
    Task<bool> FillLogisticsAsync(string memberId, string id, RefundLogisticsDto dto);

    /// <summary>
    /// 验收商品
    /// </summary>
    /// <param name="operatorId">操作人ID</param>
    /// <param name="id">退款ID</param>
    /// <param name="dto">验货参数</param>
    /// <returns>是否成功</returns>
    Task<bool> InspectAsync(string operatorId, string id, RefundInspectDto dto);

    #endregion
}
```

- [ ] **Step 2: 验证文件创建成功**

Run: `ls -la EasyProduct.WebApi/EasyProduct.Business/Mall/IRefundService.cs`
Expected: 文件存在

- [ ] **Step 3: 编译检查**

Run: `cd EasyProduct.WebApi && dotnet build`
Expected: Build succeeded

- [ ] **Step 4: 提交**

```bash
git add EasyProduct.WebApi/EasyProduct.Business/Mall/IRefundService.cs
git commit -m "feat(api): 添加退款服务接口"
```

---

## 阶段 8：业务逻辑层（服务实现）

### Task 16: 实现订单服务（核心业务逻辑）

**注意：** 由于 OrderService 实现较为复杂，涉及库存、优惠券、会员等多个服务的协调，需要分步实现。此任务仅创建服务类的框架和基础方法。

**Files:**
- Create: `EasyProduct.Business/Mall/OrderService.cs`

**此任务需要详细实现以下内容：**
1. 订单创建逻辑（购物车下单 + 直接购买）
2. 订单查询逻辑
3. 订单操作逻辑（取消、发货、收货）
4. 库存扣减/释放
5. 优惠券使用/退还
6. 事务管理

**预估代码量：** 800+ 行

**建议拆分为多个子任务或使用子代理并行开发。**

---

## 阶段 9：控制器层

### Task 17: 创建小程序端订单控制器

**Files:**
- Create: `EasyProduct.Web/Controllers/App/Mall/OrderController.cs`

**需要实现 10 个接口：**
1. 从购物车下单
2. 直接购买
3. 订单列表
4. 订单详情
5. 订单取消
6. 确认收货
7. 申请退款
8. 退款列表
9. 退款详情
10. 微信支付 / 余额支付

---

### Task 18: 创建管理端订单控制器

**Files:**
- Create: `EasyProduct.Web/Controllers/Admin/Mall/OrderController.cs`

**需要实现 10 个接口：**
1. 订单列表
2. 订单详情
3. 订单发货
4. 订单备注
5. 订单取消
6. 退款列表
7. 退款详情
8. 退款审核
9. 验收商品
10. 订单统计

---

## 阶段 10：测试与验证

### Task 19: 编译检查

- [ ] **编译整个解决方案**

Run: `cd EasyProduct.WebApi && dotnet build`
Expected: 0 个错误，0 个新增警告

---

### Task 20: Swagger 接口验证

- [ ] **启动项目**

Run: `cd EasyProduct.WebApi && dotnet run`

- [ ] **访问 Swagger**

访问：`https://localhost:5001/swagger`
验证：所有接口都在 Swagger 中显示

---

## 总结

**预计文件数量：**
- 实体类：4 个
- 枚举类：5 个
- DTO 类：~20 个
- 服务接口：3 个
- 服务实现：3 个（建议后续拆分为多个任务）
- 控制器：4 个

**预计代码量：**
- 数据模型层：~2000 行
- 业务逻辑层：~2000+ 行
- 控制器层：~1000 行
- **总计：** ~5000+ 行

**建议：**
由于订单支付模块涉及的业务逻辑复杂，建议：
1. 使用 subagent-driven-development 方式执行
2. 每个阶段使用独立的子代理
3. 在关键节点进行审查
4. 优先实现核心流程（订单创建 → 支付 → 发货 → 收货）
5. 后续再实现退款功能

**下一步：**
选择执行方式：
1. **Subagent-Driven**（推荐）- 使用子代理逐任务执行，每任务完成后审查
2. **Inline Execution** - 在当前会话中批量执行