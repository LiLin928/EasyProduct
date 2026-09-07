# Mall 模块后端开发说明

> 商城模块后端实现指南，包含会员、等级、积分、优惠券、订单、支付等功能

## 1. 模块概述

### 1.1 功能范围

Mall 模块是 EasyProduct 的核心电商业务模块，负责管理会员体系、商品交易、支付流程等关键业务。主要功能包括：

- **会员管理**：会员信息管理、等级体系、积分体系
- **优惠券管理**：优惠券创建、发放、使用统计
- **订单管理**：订单创建、状态流转、发货处理
- **支付管理**：支付记录、支付状态跟踪、退款处理
- **收货地址**：会员地址管理

### 1.2 业务流程

```
会员注册 → 等级自动计算 → 下单 → 使用优惠券/积分 → 支付 → 发货 → 完成
                                                              ↓
                                                            退款/取消
```

### 1.3 模块依赖

- **Basic 模块**：用户认证（AdminJwt/MemberJwt）、字典配置
- **Product 模块**：商品信息查询、库存扣减
- **Ops 模块**：订单超时自动取消定时任务

---

## 2. 技术栈和依赖

### 2.1 核心技术栈

| 技术 | 版本 | 用途 |
|------|------|------|
| .NET | 8.0 (LTS) | 运行时 |
| SqlSugarCore | 5.1.4.x | ORM |
| Mapster | 10.x | 对象映射 |
| Serilog | 8.x | 日志 |
| xUnit | 2.x | 单元测试 |

### 2.2 模块依赖

```xml
<!-- EasyProduct.Business/Mall/MallModule.csproj -->
<ItemGroup>
  <ProjectReference Include="..\..\Models\EasyProduct.Models.csproj" />
  <ProjectReference Include="..\..\Common\EasyProduct.Common.csproj" />
</ItemGroup>
```

### 2.3 第三方集成

- **微信支付**：订单支付、退款
- **支付宝支付**：订单支付、退款
- **微信授权**：会员登录（openid）

---

## 3. 实体类设计

### 3.1 实体基类

所有 Mall 模块实体继承 `BaseEntity`：

```csharp
/// <summary>
/// 实体基类，所有业务表继承
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// 主键ID
    /// </summary>
    [SugarColumn(IsPrimaryKey = true)]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// 是否删除（0-未删除，1-已删除）
    /// </summary>
    public int IsDeleted { get; set; } = 0;

    /// <summary>
    /// 状态（通用状态字段，不同实体含义不同）
    /// </summary>
    public int Status { get; set; } = 0;

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    /// <summary>
    /// 创建人
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = true)]
    public string? CreatedBy { get; set; }

    /// <summary>
    /// 更新人
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = true)]
    public string? UpdatedBy { get; set; }
}
```

### 3.2 核心实体类

#### Member - 会员

```csharp
/// <summary>
/// 会员实体
/// </summary>
[SugarTable("mall_member", "会员表")]
public class Member : BaseEntity
{
    [SugarColumn(Length = 50)]
    public string Nickname { get; set; } = string.Empty;

    [SugarColumn(Length = 500)]
    public string Avatar { get; set; } = string.Empty;

    [SugarColumn(Length = 20)]
    public string Phone { get; set; } = string.Empty;

    [SugarColumn(Length = 100, IsNullable = true)]
    public string? Openid { get; set; }

    [SugarColumn(Length = 36, IsNullable = true)]
    public string? LevelId { get; set; }

    [SugarColumn(IsIgnore = true)]
    public string? LevelName { get; set; }

    public int Points { get; set; } = 0;

    [SugarColumn(Length = 18, DecimalDigits = 2)]
    public decimal TotalSpent { get; set; } = 0;

    public int OrderCount { get; set; } = 0;

    // Status 字段已从 BaseEntity 继承，表示会员状态
    // 10-激活，20-禁用
}
```

#### MemberLevel - 会员等级

```csharp
/// <summary>
/// 会员等级实体
/// </summary>
[SugarTable("mall_level", "会员等级表")]
public class MemberLevel : BaseEntity
{
    [SugarColumn(Length = 50)]
    public string Name { get; set; } = string.Empty;

    public int MinPoints { get; set; } = 0;

    [SugarColumn(Length = 3, DecimalDigits = 2)]
    public decimal Discount { get; set; } = 1.00m;

    public int Sort { get; set; } = 0;

    // Status 字段已从 BaseEntity 继承，表示等级状态
    // 10-启用，20-禁用
}
```

#### PointsRecord - 积分记录

```csharp
/// <summary>
/// 积分记录实体
/// </summary>
[SugarTable("mall_points_record", "积分记录表")]
public class PointsRecord : BaseEntity
{
    [SugarColumn(Length = 36)]
    public string MemberId { get; set; } = string.Empty;

    [SugarColumn(Length = 50)]
    public string MemberName { get; set; } = string.Empty;

    /// <summary>
    /// 积分类型：1-获得，2-消费
    /// </summary>
    public int Type { get; set; }

    public int Amount { get; set; }

    /// <summary>
    /// 积分来源：order-订单，signin-签到，activity-活动，refund-退款，adjust-调整
    /// </summary>
    [SugarColumn(Length = 20)]
    public string Source { get; set; } = string.Empty;

    [SugarColumn(Length = 200)]
    public string Description { get; set; } = string.Empty;
}
```

#### Coupon - 优惠券

```csharp
/// <summary>
/// 优惠券实体
/// </summary>
[SugarTable("mall_coupon", "优惠券表")]
public class Coupon : BaseEntity
{
    [SugarColumn(Length = 100)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 优惠券类型：1-固定金额，2-百分比折扣
    /// </summary>
    public int Type { get; set; }

    [SugarColumn(Length = 10, DecimalDigits = 2)]
    public decimal Value { get; set; }

    [SugarColumn(Length = 12, DecimalDigits = 2)]
    public decimal MinSpend { get; set; } = 0;

    public int TotalCount { get; set; }

    public int IssuedCount { get; set; } = 0;

    public int UsedCount { get; set; } = 0;

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    // Status 字段已从 BaseEntity 继承，表示优惠券状态
    // 10-启用，20-禁用
}
```

#### Order - 订单

```csharp
/// <summary>
/// 订单实体
/// </summary>
[SugarTable("mall_order", "订单表")]
public class Order : BaseEntity
{
    [SugarColumn(Length = 50, IsNullable = false, UniqueGroupNameList = new[] { "order_no_unique" })]
    public string OrderNo { get; set; } = string.Empty;

    [SugarColumn(Length = 36)]
    public string MemberId { get; set; } = string.Empty;

    [SugarColumn(Length = 50)]
    public string MemberName { get; set; } = string.Empty;

    [SugarColumn(Length = 20)]
    public string MemberPhone { get; set; } = string.Empty;

    [SugarColumn(Length = 12, DecimalDigits = 2)]
    public decimal TotalAmount { get; set; }

    [SugarColumn(Length = 12, DecimalDigits = 2)]
    public decimal DiscountAmount { get; set; } = 0;

    [SugarColumn(Length = 12, DecimalDigits = 2)]
    public decimal PointsAmount { get; set; } = 0;

    [SugarColumn(Length = 10, DecimalDigits = 2)]
    public decimal ShippingFee { get; set; } = 0;

    [SugarColumn(Length = 12, DecimalDigits = 2)]
    public decimal PayAmount { get; set; }

    [SugarColumn(Length = 36, IsNullable = true)]
    public string? CouponId { get; set; }

    [SugarColumn(Length = 100, IsNullable = true)]
    public string? CouponName { get; set; }

    // Status 字段已从 BaseEntity 继承，表示订单状态
    // 10-待支付，20-已支付，30-已发货，40-已完成，50-已取消，60-已退款

    /// <summary>
    /// 支付方式：1-微信，2-支付宝，3-银行，4-现金
    /// </summary>
    public int? PaymentMethod { get; set; }

    [SugarColumn(ColumnDataType = "text", IsNullable = true)]
    public string? Remark { get; set; }

    [SugarColumn(IsIgnore = true)]
    public List<OrderItem>? Items { get; set; }
}
```

#### OrderItem - 订单明细

```csharp
/// <summary>
/// 订单明细实体
/// </summary>
[SugarTable("mall_order_item", "订单明细表")]
public class OrderItem : BaseEntity
{
    [SugarColumn(Length = 36)]
    public string OrderId { get; set; } = string.Empty;

    [SugarColumn(Length = 36)]
    public string SpuId { get; set; } = string.Empty;

    [SugarColumn(Length = 200)]
    public string SpuName { get; set; } = string.Empty;

    [SugarColumn(ColumnDataType = "json")]
    public string? SpecValues { get; set; }

    [SugarColumn(Length = 10, DecimalDigits = 2)]
    public decimal Price { get; set; }

    public int Quantity { get; set; }

    [SugarColumn(Length = 12, DecimalDigits = 2)]
    public decimal Subtotal { get; set; }
}
```

#### PaymentRecord - 支付记录

```csharp
/// <summary>
/// 支付记录实体
/// </summary>
[SugarTable("mall_payment", "支付记录表")]
public class PaymentRecord : BaseEntity
{
    [SugarColumn(Length = 36)]
    public string OrderId { get; set; } = string.Empty;

    [SugarColumn(Length = 50)]
    public string OrderNo { get; set; } = string.Empty;

    [SugarColumn(Length = 12, DecimalDigits = 2)]
    public decimal Amount { get; set; }

    /// <summary>
    /// 支付方式：1-微信，2-支付宝，3-银行，4-现金
    /// </summary>
    public int Method { get; set; }

    // Status 字段已从 BaseEntity 继承，表示支付状态
    // 10-待支付，20-成功，30-失败，40-已退款

    [SugarColumn(Length = 100, IsNullable = true)]
    public string? TransactionId { get; set; }

    public DateTime? PaidAt { get; set; }
}
```

#### MemberAddress - 会员地址

```csharp
/// <summary>
/// 会员地址实体
/// </summary>
[SugarTable("mall_address", "会员地址表")]
public class MemberAddress : BaseEntity
{
    [SugarColumn(Length = 36)]
    public string MemberId { get; set; } = string.Empty;

    [SugarColumn(Length = 50)]
    public string ReceiverName { get; set; } = string.Empty;

    [SugarColumn(Length = 20)]
    public string Phone { get; set; } = string.Empty;

    [SugarColumn(Length = 50)]
    public string Province { get; set; } = string.Empty;

    [SugarColumn(Length = 50)]
    public string City { get; set; } = string.Empty;

    [SugarColumn(Length = 50)]
    public string District { get; set; } = string.Empty;

    [SugarColumn(Length = 200)]
    public string DetailAddress { get; set; } = string.Empty;

    public bool IsDefault { get; set; } = false;
}
```

### 3.3 常量定义

```csharp
/// <summary>
/// Mall 模块常量定义
/// </summary>
public static class MallConstants
{
    // 会员状态
    public static class MemberStatus
    {
        public const int Active = 10;    // 激活
        public const int Inactive = 20;  // 禁用
    }

    // 会员等级状态
    public static class LevelStatus
    {
        public const int Enabled = 10;   // 启用
        public const int Disabled = 20;  // 禁用
    }

    // 积分类型
    public static class PointsType
    {
        public const int Earn = 1;   // 获得
        public const int Spend = 2;  // 消费
    }

    // 积分来源
    public static class PointsSource
    {
        public const string Order = "order";      // 订单
        public const string Signin = "signin";    // 签到
        public const string Activity = "activity"; // 活动
        public const string Refund = "refund";    // 退款
        public const string Adjust = "adjust";    // 调整
    }

    // 优惠券类型
    public static class CouponType
    {
        public const int Fixed = 1;    // 固定金额
        public const int Percent = 2;  // 百分比折扣
    }

    // 优惠券状态
    public static class CouponStatus
    {
        public const int Enabled = 10;   // 启用
        public const int Disabled = 20;  // 禁用
    }

    // 订单状态
    public static class OrderStatus
    {
        public const int Pending = 10;     // 待支付
        public const int Paid = 20;        // 已支付
        public const int Shipped = 30;     // 已发货
        public const int Completed = 40;   // 已完成
        public const int Cancelled = 50;   // 已取消
        public const int Refunded = 60;    // 已退款
    }

    // 支付状态
    public static class PaymentStatus
    {
        public const int Pending = 10;   // 待支付
        public const int Success = 20;   // 成功
        public const int Failed = 30;    // 失败
        public const int Refunded = 40;  // 已退款
    }

    // 支付方式
    public static class PaymentMethod
    {
        public const int Wechat = 1;  // 微信
        public const int Alipay = 2;  // 支付宝
        public const int Bank = 3;    // 银行
        public const int Cash = 4;    // 现金
    }
}
```

---

## 4. DTO 设计

### 4.1 查询 DTO

#### MemberQuery - 会员查询

```csharp
/// <summary>
/// 会员查询参数
/// </summary>
public class MemberQuery : PageQuery
{
    public string? Nickname { get; set; }
    public string? Phone { get; set; }
    public string? LevelId { get; set; }
    public string? Status { get; set; }
}
```

#### PointsQuery - 积分记录查询

```csharp
/// <summary>
/// 积分记录查询参数
/// </summary>
public class PointsQuery : PageQuery
{
    public string? MemberId { get; set; }
    public string? Type { get; set; }
    public string? Source { get; set; }
}
```

#### OrderQuery - 订单查询

```csharp
/// <summary>
/// 订单查询参数
/// </summary>
public class OrderQuery : PageQuery
{
    public string? OrderNo { get; set; }
    public string? MemberId { get; set; }
    public string? Status { get; set; }

    [DataType(DataType.Date)]
    public DateTime? StartTime { get; set; }

    [DataType(DataType.Date)]
    public DateTime? EndTime { get; set; }
}
```

#### PaymentQuery - 支付记录查询

```csharp
/// <summary>
/// 支付记录查询参数
/// </summary>
public class PaymentQuery : PageQuery
{
    public string? OrderNo { get; set; }
    public string? Status { get; set; }
    public string? Method { get; set; }
}
```

### 4.2 创建/更新 DTO

#### MemberUpdateDto - 更新会员

```csharp
/// <summary>
/// 更新会员参数
/// </summary>
public class MemberUpdateDto
{
    [MaxLength(50, ErrorMessage = "昵称不能超过50个字符")]
    public string? Nickname { get; set; }

    [MaxLength(500, ErrorMessage = "头像URL不能超过500个字符")]
    public string? Avatar { get; set; }

    [MaxLength(20, ErrorMessage = "手机号格式不正确")]
    [RegularExpression(@"^1[3-9]\d{9}$", ErrorMessage = "手机号格式不正确")]
    public string? Phone { get; set; }

    public string? LevelId { get; set; }
}
```

#### MemberStatusUpdateDto - 更新会员状态

```csharp
/// <summary>
/// 更新会员状态参数
/// </summary>
public class MemberStatusUpdateDto
{
    [Required(ErrorMessage = "状态不能为空")]
    public string Status { get; set; } = string.Empty;
}
```

#### LevelCreateDto - 创建会员等级

```csharp
/// <summary>
/// 创建会员等级参数
/// </summary>
public class LevelCreateDto
{
    [Required(ErrorMessage = "等级名称不能为空")]
    [MaxLength(50, ErrorMessage = "等级名称不能超过50个字符")]
    public string Name { get; set; } = string.Empty;

    [Range(0, int.MaxValue, ErrorMessage = "升级所需积分必须大于等于0")]
    public int MinPoints { get; set; }

    [Range(0.01, 1.0, ErrorMessage = "折扣率必须在0.01到1.0之间")]
    public decimal Discount { get; set; }

    public int Sort { get; set; } = 0;
}
```

#### LevelUpdateDto - 更新会员等级

```csharp
/// <summary>
/// 更新会员等级参数
/// </summary>
public class LevelUpdateDto
{
    [Required(ErrorMessage = "等级名称不能为空")]
    [MaxLength(50, ErrorMessage = "等级名称不能超过50个字符")]
    public string Name { get; set; } = string.Empty;

    [Range(0, int.MaxValue, ErrorMessage = "升级所需积分必须大于等于0")]
    public int MinPoints { get; set; }

    [Range(0.01, 1.0, ErrorMessage = "折扣率必须在0.01到1.0之间")]
    public decimal Discount { get; set; }

    public int Sort { get; set; } = 0;
}
```

#### PointsAdjustDto - 积分调整

```csharp
/// <summary>
/// 积分调整参数
/// </summary>
public class PointsAdjustDto
{
    [Required(ErrorMessage = "会员ID不能为空")]
    public string MemberId { get; set; } = string.Empty;

    [Required(ErrorMessage = "积分数量不能为空")]
    [Range(-10000, 10000, ErrorMessage = "单次调整积分不能超过10000")]
    public int Amount { get; set; }

    [Required(ErrorMessage = "描述不能为空")]
    [MaxLength(200, ErrorMessage = "描述不能超过200个字符")]
    public string Description { get; set; } = string.Empty;
}
```

#### CouponCreateDto - 创建优惠券

```csharp
/// <summary>
/// 创建优惠券参数
/// </summary>
public class CouponCreateDto
{
    [Required(ErrorMessage = "优惠券名称不能为空")]
    [MaxLength(100, ErrorMessage = "优惠券名称不能超过100个字符")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "优惠券类型不能为空")]
    public string Type { get; set; } = string.Empty;

    [Required(ErrorMessage = "面值不能为空")]
    [Range(0.01, double.MaxValue, ErrorMessage = "面值必须大于0")]
    public decimal Value { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "最低消费金额不能为负数")]
    public decimal MinSpend { get; set; }

    [Required(ErrorMessage = "发行总量不能为空")]
    [Range(1, int.MaxValue, ErrorMessage = "发行总量必须大于0")]
    public int TotalCount { get; set; }

    [Required(ErrorMessage = "开始日期不能为空")]
    public DateTime StartDate { get; set; }

    [Required(ErrorMessage = "结束日期不能为空")]
    public DateTime EndDate { get; set; }
}
```

#### CouponUpdateDto - 更新优惠券

```csharp
/// <summary>
/// 更新优惠券参数
/// </summary>
public class CouponUpdateDto
{
    [Required(ErrorMessage = "优惠券名称不能为空")]
    [MaxLength(100, ErrorMessage = "优惠券名称不能超过100个字符")]
    public string Name { get; set; } = string.Empty;

    [Range(0, double.MaxValue, ErrorMessage = "最低消费金额不能为负数")]
    public decimal MinSpend { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "发行总量必须大于0")]
    public int TotalCount { get; set; }

    [Required(ErrorMessage = "开始日期不能为空")]
    public DateTime StartDate { get; set; }

    [Required(ErrorMessage = "结束日期不能为空")]
    public DateTime EndDate { get; set; }
}
```

#### OrderShipDto - 订单发货

```csharp
/// <summary>
/// 订单发货参数
/// </summary>
public class OrderShipDto
{
    [Required(ErrorMessage = "快递公司不能为空")]
    [MaxLength(50, ErrorMessage = "快递公司不能超过50个字符")]
    public string ExpressCompany { get; set; } = string.Empty;

    [Required(ErrorMessage = "快递单号不能为空")]
    [MaxLength(50, ErrorMessage = "快递单号不能超过50个字符")]
    public string ExpressNo { get; set; } = string.Empty;
}
```

#### AddressCreateDto - 创建地址

```csharp
/// <summary>
/// 创建地址参数
/// </summary>
public class AddressCreateDto
{
    [Required(ErrorMessage = "会员ID不能为空")]
    public string MemberId { get; set; } = string.Empty;

    [Required(ErrorMessage = "收货人姓名不能为空")]
    [MaxLength(50, ErrorMessage = "收货人姓名不能超过50个字符")]
    public string ReceiverName { get; set; } = string.Empty;

    [Required(ErrorMessage = "手机号不能为空")]
    [MaxLength(20, ErrorMessage = "手机号不能超过20个字符")]
    public string Phone { get; set; } = string.Empty;

    [Required(ErrorMessage = "省份不能为空")]
    [MaxLength(50, ErrorMessage = "省份不能超过50个字符")]
    public string Province { get; set; } = string.Empty;

    [Required(ErrorMessage = "城市不能为空")]
    [MaxLength(50, ErrorMessage = "城市不能超过50个字符")]
    public string City { get; set; } = string.Empty;

    [Required(ErrorMessage = "区县不能为空")]
    [MaxLength(50, ErrorMessage = "区县不能超过50个字符")]
    public string District { get; set; } = string.Empty;

    [Required(ErrorMessage = "详细地址不能为空")]
    [MaxLength(200, ErrorMessage = "详细地址不能超过200个字符")]
    public string DetailAddress { get; set; } = string.Empty;

    public bool IsDefault { get; set; } = false;
}
```

### 4.3 输出 DTO

#### MemberDto - 会员输出

```csharp
/// <summary>
/// 会员输出
/// </summary>
public class MemberDto
{
    public string Id { get; set; } = string.Empty;
    public string Nickname { get; set; } = string.Empty;
    public string Avatar { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Openid { get; set; }
    public string? LevelId { get; set; }
    public string? LevelName { get; set; }
    public int Points { get; set; }
    public decimal TotalSpent { get; set; }
    public int OrderCount { get; set; }
    public int Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
```

#### OrderDto - 订单输出

```csharp
/// <summary>
/// 订单输出
/// </summary>
public class OrderDto
{
    public string Id { get; set; } = string.Empty;
    public string OrderNo { get; set; } = string.Empty;
    public string MemberId { get; set; } = string.Empty;
    public string MemberName { get; set; } = string.Empty;
    public string MemberPhone { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal PointsAmount { get; set; }
    public decimal ShippingFee { get; set; }
    public decimal PayAmount { get; set; }
    public string? CouponId { get; set; }
    public string? CouponName { get; set; }
    public int Status { get; set; }
    public int? PaymentMethod { get; set; }
    public string? Remark { get; set; }
    public List<OrderItemDto>? Items { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 订单明细输出
/// </summary>
public class OrderItemDto
{
    public string Id { get; set; } = string.Empty;
    public string OrderId { get; set; } = string.Empty;
    public string SpuId { get; set; } = string.Empty;
    public string SpuName { get; set; } = string.Empty;
    public string? SpecValues { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public decimal Subtotal { get; set; }
}
```

---

## 5. Service 层实现指南

### 5.1 Service 接口定义

#### IMemberService - 会员服务接口

```csharp
/// <summary>
/// 会员服务接口
/// </summary>
public interface IMemberService
{
    /// <summary>
    /// 获取会员列表（分页）
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>会员分页结果</returns>
    Task<PageResult<MemberDto>> GetListAsync(MemberQuery query);

    /// <summary>
    /// 获取会员详情
    /// </summary>
    /// <param name="id">会员ID</param>
    /// <returns>会员详情</returns>
    Task<MemberDto> GetByIdAsync(string id);

    /// <summary>
    /// 更新会员信息
    /// </summary>
    /// <param name="id">会员ID</param>
    /// <param name="dto">更新参数</param>
    /// <returns>是否成功</returns>
    Task<bool> UpdateAsync(string id, MemberUpdateDto dto);

    /// <summary>
    /// 更新会员状态
    /// </summary>
    /// <param name="id">会员ID</param>
    /// <param name="status">新状态</param>
    /// <returns>是否成功</returns>
    Task<bool> UpdateStatusAsync(string id, string status);
}
```

#### IOrderService - 订单服务接口

```csharp
/// <summary>
/// 订单服务接口
/// </summary>
public interface IOrderService
{
    /// <summary>
    /// 获取订单列表（分页）
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>订单分页结果</returns>
    Task<PageResult<OrderDto>> GetListAsync(OrderQuery query);

    /// <summary>
    /// 获取订单详情
    /// </summary>
    /// <param name="id">订单ID</param>
    /// <returns>订单详情</returns>
    Task<OrderDto> GetByIdAsync(string id);

    /// <summary>
    /// 更新订单状态
    /// </summary>
    /// <param name="id">订单ID</param>
    /// <param name="status">新状态</param>
    /// <returns>是否成功</returns>
    Task<bool> UpdateStatusAsync(string id, string status);

    /// <summary>
    /// 订单发货
    /// </summary>
    /// <param name="id">订单ID</param>
    /// <param name="dto">发货参数</param>
    /// <returns>是否成功</returns>
    Task<bool> ShipAsync(string id, OrderShipDto dto);
}
```

#### IPointsService - 积分服务接口

```csharp
/// <summary>
/// 积分服务接口
/// </summary>
public interface IPointsService
{
    /// <summary>
    /// 获取积分记录列表（分页）
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>积分记录分页结果</returns>
    Task<PageResult<PointsRecordDto>> GetListAsync(PointsQuery query);

    /// <summary>
    /// 积分调整
    /// </summary>
    /// <param name="dto">调整参数</param>
    /// <returns>是否成功</returns>
    Task<bool> AdjustAsync(PointsAdjustDto dto);

    /// <summary>
    /// 增加积分
    /// </summary>
    /// <param name="memberId">会员ID</param>
    /// <param name="amount">积分数量</param>
    /// <param name="source">来源</param>
    /// <param name="description">描述</param>
    /// <returns>是否成功</returns>
    Task<bool> EarnAsync(string memberId, int amount, string source, string description);
}
```

### 5.2 Service 实现示例

#### MemberService - 会员服务实现

```csharp
/// <summary>
/// 会员服务实现
/// </summary>
public class MemberService : BaseService, IMemberService
{
    private readonly ILogger<MemberService> _logger;

    /// <summary>
    /// 构造函数
    /// </summary>
    public MemberService(ISqlSugarClient db, ILogger<MemberService> logger) : base(db)
    {
        _logger = logger;
    }

    /// <summary>
    /// 获取会员列表（分页）
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>会员分页结果</returns>
    /// <remarks>
    /// 支持按昵称、手机号、等级、状态筛选
    /// 默认按创建时间倒序排列
    /// </remarks>
    public async Task<PageResult<MemberDto>> GetListAsync(MemberQuery query)
    {
        var queryable = _db.Queryable<Member>()
            .LeftJoin<MemberLevel>((m, l) => m.LevelId == l.Id.ToString())
            .WhereIF(!string.IsNullOrEmpty(query.Nickname), (m, l) => m.Nickname.Contains(query.Nickname!))
            .WhereIF(!string.IsNullOrEmpty(query.Phone), (m, l) => m.Phone == query.Phone)
            .WhereIF(!string.IsNullOrEmpty(query.LevelId), (m, l) => m.LevelId == query.LevelId)
            .WhereIF(query.Status.HasValue, (m, l) => m.Status == query.Status)
            .OrderByDescending((m, l) => m.CreatedAt)
            .Select((m, l) => new MemberDto
            {
                Id = m.Id.ToString(),
                Nickname = m.Nickname,
                Avatar = m.Avatar,
                Phone = m.Phone,
                Openid = m.Openid,
                LevelId = m.LevelId,
                LevelName = l.Name,
                Points = m.Points,
                TotalSpent = m.TotalSpent,
                OrderCount = m.OrderCount,
                Status = m.Status,
                CreatedAt = m.CreatedAt,
                UpdatedAt = m.UpdatedAt
            });

        var total = await queryable.CountAsync();
        var list = await queryable
            .Skip((query.PageIndex - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();

        return new PageResult<MemberDto>
        {
            List = list,
            Total = total
        };
    }

    /// <summary>
    /// 获取会员详情
    /// </summary>
    /// <param name="id">会员ID</param>
    /// <returns>会员详情</returns>
    /// <exception cref="BusinessException">会员不存在时抛出异常</exception>
    public async Task<MemberDto> GetByIdAsync(string id)
    {
        var member = await _db.Queryable<Member>()
            .LeftJoin<MemberLevel>((m, l) => m.LevelId == l.Id.ToString())
            .Where((m, l) => m.Id.ToString() == id)
            .Select((m, l) => new MemberDto
            {
                Id = m.Id.ToString(),
                Nickname = m.Nickname,
                Avatar = m.Avatar,
                Phone = m.Phone,
                Openid = m.Openid,
                LevelId = m.LevelId,
                LevelName = l.Name,
                Points = m.Points,
                TotalSpent = m.TotalSpent,
                OrderCount = m.OrderCount,
                Status = m.Status,
                CreatedAt = m.CreatedAt,
                UpdatedAt = m.UpdatedAt
            })
            .FirstAsync();

        if (member == null)
        {
            throw BusinessException.NotFound("会员不存在");
        }

        return member;
    }

    /// <summary>
    /// 更新会员信息
    /// </summary>
    /// <param name="id">会员ID</param>
    /// <param name="dto">更新参数</param>
    /// <returns>是否成功</returns>
    /// <exception cref="BusinessException">会员不存在时抛出异常</exception>
    public async Task<bool> UpdateAsync(string id, MemberUpdateDto dto)
    {
        var member = await _db.Queryable<Member>()
            .Where(m => m.Id.ToString() == id)
            .FirstAsync();

        if (member == null)
        {
            throw BusinessException.NotFound("会员不存在");
        }

        if (dto.Nickname != null) member.Nickname = dto.Nickname;
        if (dto.Avatar != null) member.Avatar = dto.Avatar;
        if (dto.Phone != null) member.Phone = dto.Phone;
        if (dto.LevelId != null) member.LevelId = dto.LevelId;

        member.UpdatedAt = DateTime.Now;

        var result = await _db.Updateable(member).ExecuteCommandAsync();
        return result > 0;
    }

    /// <summary>
    /// 更新会员状态
    /// </summary>
    /// <param name="id">会员ID</param>
    /// <param name="status">新状态</param>
    /// <returns>是否成功</returns>
    /// <exception cref="BusinessException">会员不存在时抛出异常</exception>
    public async Task<bool> UpdateStatusAsync(string id, string status)
    {
        var member = await _db.Queryable<Member>()
            .Where(m => m.Id.ToString() == id)
            .FirstAsync();

        if (member == null)
        {
            throw BusinessException.NotFound("会员不存在");
        }

        member.Status = status;
        member.UpdatedAt = DateTime.Now;

        var result = await _db.Updateable(member).ExecuteCommandAsync();
        return result > 0;
    }
}
```

#### OrderService - 订单服务实现

```csharp
/// <summary>
/// 订单服务实现
/// </summary>
public class OrderService : BaseService, IOrderService
{
    private readonly ILogger<OrderService> _logger;
    private readonly IProductService _productService;

    /// <summary>
    /// 构造函数
    /// </summary>
    public OrderService(
        ISqlSugarClient db,
        ILogger<OrderService> logger,
        IProductService productService) : base(db)
    {
        _logger = logger;
        _productService = productService;
    }

    /// <summary>
    /// 获取订单列表（分页）
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>订单分页结果</returns>
    /// <remarks>
    /// 支持按订单编号、会员ID、状态、时间范围筛选
    /// 默认按创建时间倒序排列
    /// </remarks>
    public async Task<PageResult<OrderDto>> GetListAsync(OrderQuery query)
    {
        var queryable = _db.Queryable<Order>()
            .WhereIF(!string.IsNullOrEmpty(query.OrderNo), o => o.OrderNo.Contains(query.OrderNo!))
            .WhereIF(!string.IsNullOrEmpty(query.MemberId), o => o.MemberId == query.MemberId)
            .WhereIF(!string.IsNullOrEmpty(query.Status), o => o.Status == query.Status)
            .WhereIF(query.StartTime.HasValue, o => o.CreateTime >= query.StartTime)
            .WhereIF(query.EndTime.HasValue, o => o.CreateTime <= query.EndTime!.Value.AddDays(1))
            .OrderByDescending(o => o.CreateTime);

        var total = await queryable.CountAsync();
        var orders = await queryable
            .Skip((query.PageIndex - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();

        var orderIds = orders.Select(o => o.Id.ToString()).ToList();

        // 查询订单明细
        var items = await _db.Queryable<OrderItem>()
            .Where(i => orderIds.Contains(i.OrderId))
            .ToListAsync();

        var orderDtos = orders.Select(o => new OrderDto
        {
            Id = o.Id.ToString(),
            OrderNo = o.OrderNo,
            MemberId = o.MemberId,
            MemberName = o.MemberName,
            MemberPhone = o.MemberPhone,
            TotalAmount = o.TotalAmount,
            DiscountAmount = o.DiscountAmount,
            PointsAmount = o.PointsAmount,
            ShippingFee = o.ShippingFee,
            PayAmount = o.PayAmount,
            CouponId = o.CouponId,
            CouponName = o.CouponName,
            Status = o.Status,
            PaymentMethod = o.PaymentMethod,
            Remark = o.Remark,
            Items = items.Where(i => i.OrderId == o.Id.ToString())
                .Select(i => new OrderItemDto
                {
                    Id = i.Id.ToString(),
                    OrderId = i.OrderId,
                    SpuId = i.SpuId,
                    SpuName = i.SpuName,
                    SpecValues = i.SpecValues,
                    Price = i.Price,
                    Quantity = i.Quantity,
                    Subtotal = i.Subtotal
                })
                .ToList(),
            CreatedAt = o.CreatedAt,
            UpdatedAt = o.UpdatedAt
        })
        .ToList();

        return new PageResult<OrderDto>
        {
            List = orderDtos,
            Total = total
        };
    }

    /// <summary>
    /// 获取订单详情
    /// </summary>
    /// <param name="id">订单ID</param>
    /// <returns>订单详情</returns>
    /// <exception cref="BusinessException">订单不存在时抛出异常</exception>
    public async Task<OrderDto> GetByIdAsync(string id)
    {
        var order = await _db.Queryable<Order>()
            .Where(o => o.Id.ToString() == id)
            .FirstAsync();

        if (order == null)
        {
            throw BusinessException.NotFound("订单不存在");
        }

        var items = await _db.Queryable<OrderItem>()
            .Where(i => i.OrderId == id)
            .ToListAsync();

        return new OrderDto
        {
            Id = order.Id.ToString(),
            OrderNo = order.OrderNo,
            MemberId = order.MemberId,
            MemberName = order.MemberName,
            MemberPhone = order.MemberPhone,
            TotalAmount = order.TotalAmount,
            DiscountAmount = order.DiscountAmount,
            PointsAmount = order.PointsAmount,
            ShippingFee = order.ShippingFee,
            PayAmount = order.PayAmount,
            CouponId = order.CouponId,
            CouponName = order.CouponName,
            Status = order.Status,
            PaymentMethod = order.PaymentMethod,
            Remark = order.Remark,
            Items = items.Select(i => new OrderItemDto
            {
                Id = i.Id.ToString(),
                OrderId = i.OrderId,
                SpuId = i.SpuId,
                SpuName = i.SpuName,
                SpecValues = i.SpecValues,
                Price = i.Price,
                Quantity = i.Quantity,
                Subtotal = i.Subtotal
            })
            .ToList(),
            CreateTime = order.CreateTime,
            UpdateTime = order.UpdateTime
        };
    }

    /// <summary>
    /// 更新订单状态
    /// </summary>
    /// <param name="id">订单ID</param>
    /// <param name="status">新状态</param>
    /// <returns>是否成功</returns>
    /// <exception cref="BusinessException">订单不存在或状态流转不合法时抛出异常</exception>
    public async Task<bool> UpdateStatusAsync(string id, string status)
    {
        var order = await _db.Queryable<Order>()
            .Where(o => o.Id.ToString() == id)
            .FirstAsync();

        if (order == null)
        {
            throw BusinessException.NotFound("订单不存在");
        }

        // 验证状态流转是否合法
        if (!IsValidStatusTransition(order.Status, status))
        {
            throw BusinessException.BadRequest($"订单状态不能从 {order.Status} 变更为 {status}");
        }

        order.Status = status;
        order.UpdateTime = DateTime.Now;

        var result = await _db.Updateable(order).ExecuteCommandAsync();
        return result > 0;
    }

    /// <summary>
    /// 订单发货
    /// </summary>
    /// <param name="id">订单ID</param>
    /// <param name="dto">发货参数</param>
    /// <returns>是否成功</returns>
    /// <exception cref="BusinessException">订单不存在或状态不是已付款时抛出异常</exception>
    public async Task<bool> ShipAsync(string id, OrderShipDto dto)
    {
        var order = await _db.Queryable<Order>()
            .Where(o => o.Id.ToString() == id)
            .FirstAsync();

        if (order == null)
        {
            throw BusinessException.NotFound("订单不存在");
        }

        if (order.Status != OrderStatus.Paid)
        {
            throw BusinessException.BadRequest("只有已付款的订单才能发货");
        }

        // TODO: 保存物流信息（需增加 mall_order_logistics 表）

        order.Status = OrderStatus.Shipped;
        order.UpdateTime = DateTime.Now;

        var result = await _db.Updateable(order).ExecuteCommandAsync();
        return result > 0;
    }

    /// <summary>
    /// 验证订单状态流转是否合法
    /// </summary>
    /// <param name="currentStatus">当前状态</param>
    /// <param name="newStatus">新状态</param>
    /// <returns>是否合法</returns>
    private bool IsValidStatusTransition(string currentStatus, string newStatus)
    {
        var validTransitions = new Dictionary<string, List<string>>
        {
            [OrderStatus.Pending] = new List<string> { OrderStatus.Paid, OrderStatus.Cancelled },
            [OrderStatus.Paid] = new List<string> { OrderStatus.Shipped, OrderStatus.Refunded },
            [OrderStatus.Shipped] = new List<string> { OrderStatus.Completed, OrderStatus.Refunded },
            [OrderStatus.Completed] = new List<string>(),
            [OrderStatus.Cancelled] = new List<string>(),
            [OrderStatus.Refunded] = new List<string>()
        };

        return validTransitions.TryGetValue(currentStatus, out var allowed) && allowed.Contains(newStatus);
    }
}
```

---

## 6. Controller 层实现指南

### 6.1 Controller 基类

```csharp
/// <summary>
/// Controller 基类
/// </summary>
public abstract class BaseController : ControllerBase
{
    /// <summary>
    /// 获取当前用户ID
    /// </summary>
    protected string GetCurrentUserId()
    {
        return User.FindFirst("UserId")?.Value ?? string.Empty;
    }

    /// <summary>
    /// 获取当前用户名
    /// </summary>
    protected string GetCurrentUserName()
    {
        return User.FindFirst("UserName")?.Value ?? string.Empty;
    }

    /// <summary>
    /// 获取当前用户真实姓名
    /// </summary>
    protected string GetCurrentRealName()
    {
        return User.FindFirst("RealName")?.Value ?? string.Empty;
    }

    /// <summary>
    /// 判断是否为管理员
    /// </summary>
    protected bool IsAdmin()
    {
        return User.IsInRole("admin");
    }
}
```

### 6.2 Controller 实现示例

#### MemberController - 会员管理控制器

```csharp
/// <summary>
/// 会员管理控制器
/// </summary>
[ApiController]
[Route("api/admin/mall/member")]
[Authorize(AuthenticationSchemes = "AdminJwt")]
public class MemberController : BaseController
{
    private readonly IMemberService _memberService;

    /// <summary>
    /// 构造函数
    /// </summary>
    public MemberController(IMemberService memberService)
    {
        _memberService = memberService;
    }

    /// <summary>
    /// 获取会员列表（分页）
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>会员分页结果</returns>
    [HttpGet("list")]
    [ProducesResponseType(typeof(ApiResponse<PageResult<MemberDto>>), 200)]
    public async Task<IActionResult> GetList([FromQuery] MemberQuery query)
    {
        var result = await _memberService.GetListAsync(query);
        return Success(result);
    }

    /// <summary>
    /// 获取会员详情
    /// </summary>
    /// <param name="id">会员ID</param>
    /// <returns>会员详情</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<MemberDto>), 200)]
    public async Task<IActionResult> GetById(string id)
    {
        var result = await _memberService.GetByIdAsync(id);
        return Success(result);
    }

    /// <summary>
    /// 更新会员信息
    /// </summary>
    /// <param name="id">会员ID</param>
    /// <param name="dto">更新参数</param>
    /// <returns>是否成功</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
    public async Task<IActionResult> Update(string id, [FromBody] MemberUpdateDto dto)
    {
        var result = await _memberService.UpdateAsync(id, dto);
        return Success(result);
    }

    /// <summary>
    /// 更新会员状态
    /// </summary>
    /// <param name="id">会员ID</param>
    /// <param name="dto">状态参数</param>
    /// <returns>是否成功</returns>
    [HttpPut("{id}/status")]
    [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
    public async Task<IActionResult> UpdateStatus(string id, [FromBody] MemberStatusUpdateDto dto)
    {
        var result = await _memberService.UpdateStatusAsync(id, dto.Status);
        return Success(result);
    }
}
```

#### OrderController - 订单管理控制器

```csharp
/// <summary>
/// 订单管理控制器
/// </summary>
[ApiController]
[Route("api/admin/mall/order")]
[Authorize(AuthenticationSchemes = "AdminJwt")]
public class OrderController : BaseController
{
    private readonly IOrderService _orderService;

    /// <summary>
    /// 构造函数
    /// </summary>
    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    /// <summary>
    /// 获取订单列表（分页）
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>订单分页结果</returns>
    [HttpGet("list")]
    [ProducesResponseType(typeof(ApiResponse<PageResult<OrderDto>>), 200)]
    public async Task<IActionResult> GetList([FromQuery] OrderQuery query)
    {
        var result = await _orderService.GetListAsync(query);
        return Success(result);
    }

    /// <summary>
    /// 获取订单详情
    /// </summary>
    /// <param name="id">订单ID</param>
    /// <returns>订单详情</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<OrderDto>), 200)]
    public async Task<IActionResult> GetById(string id)
    {
        var result = await _orderService.GetByIdAsync(id);
        return Success(result);
    }

    /// <summary>
    /// 更新订单状态
    /// </summary>
    /// <param name="id">订单ID</param>
    /// <param name="dto">状态参数</param>
    /// <returns>是否成功</returns>
    [HttpPut("{id}/status")]
    [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
    public async Task<IActionResult> UpdateStatus(string id, [FromBody] OrderStatusUpdateDto dto)
    {
        var result = await _orderService.UpdateStatusAsync(id, dto.Status);
        return Success(result);
    }

    /// <summary>
    /// 订单发货
    /// </summary>
    /// <param name="id">订单ID</param>
    /// <param name="dto">发货参数</param>
    /// <returns>是否成功</returns>
    [HttpPost("{id}/ship")]
    [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
    public async Task<IActionResult> Ship(string id, [FromBody] OrderShipDto dto)
    {
        var result = await _orderService.ShipAsync(id, dto);
        return Success(result);
    }
}
```

---

## 7. 数据库表设计

### 7.1 表结构 SQL

#### mall_member - 会员表

```sql
CREATE TABLE mall_member (
  id VARCHAR(36) PRIMARY KEY COMMENT '会员ID',
  nickname VARCHAR(50) COMMENT '昵称',
  avatar VARCHAR(500) COMMENT '头像URL',
  phone VARCHAR(20) COMMENT '手机号',
  openid VARCHAR(100) UNIQUE COMMENT '微信OpenID',
  level_id VARCHAR(36) COMMENT '会员等级ID',
  points INT DEFAULT 0 COMMENT '当前积分',
  total_spent DECIMAL(12,2) DEFAULT 0 COMMENT '累计消费金额',
  order_count INT DEFAULT 0 COMMENT '订单数量',
  is_deleted INT DEFAULT 0 COMMENT '是否删除：0-未删除，1-已删除',
  status INT DEFAULT 10 COMMENT '状态：10-激活，20-禁用',
  created_at DATETIME DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  updated_at DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
  created_by VARCHAR(50) COMMENT '创建人',
  updated_by VARCHAR(50) COMMENT '更新人',
  INDEX idx_phone (phone),
  INDEX idx_openid (openid),
  INDEX idx_level_id (level_id),
  INDEX idx_status (status)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='会员表';
```

#### mall_level - 会员等级表

```sql
CREATE TABLE mall_level (
  id VARCHAR(36) PRIMARY KEY COMMENT '等级ID',
  name VARCHAR(50) NOT NULL COMMENT '等级名称',
  min_points INT DEFAULT 0 COMMENT '升级所需积分',
  discount DECIMAL(3,2) DEFAULT 1.00 COMMENT '折扣率（如0.95表示95折）',
  sort INT DEFAULT 0 COMMENT '排序',
  is_deleted INT DEFAULT 0 COMMENT '是否删除：0-未删除，1-已删除',
  status INT DEFAULT 10 COMMENT '状态：10-启用，20-禁用',
  created_at DATETIME DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  updated_at DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
  created_by VARCHAR(50) COMMENT '创建人',
  updated_by VARCHAR(50) COMMENT '更新人',
  INDEX idx_status (status)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='会员等级表';
```

#### mall_points_record - 积分记录表

```sql
CREATE TABLE mall_points_record (
  id VARCHAR(36) PRIMARY KEY COMMENT '记录ID',
  member_id VARCHAR(36) NOT NULL COMMENT '会员ID',
  member_name VARCHAR(50) COMMENT '会员名称',
  type INT NOT NULL COMMENT '类型：1-获得，2-消费',
  amount INT NOT NULL COMMENT '积分数量（正数获得，负数消费）',
  source VARCHAR(20) NOT NULL COMMENT '来源：order-订单 signin-签到 activity-活动 refund-退款 adjust-调整',
  description VARCHAR(200) COMMENT '描述',
  is_deleted INT DEFAULT 0 COMMENT '是否删除：0-未删除，1-已删除',
  status INT DEFAULT 10 COMMENT '状态（预留字段）',
  created_at DATETIME DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  updated_at DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
  created_by VARCHAR(50) COMMENT '创建人',
  updated_by VARCHAR(50) COMMENT '更新人',
  INDEX idx_member_id (member_id),
  INDEX idx_type (type),
  INDEX idx_source (source)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='积分记录表';
```

#### mall_coupon - 优惠券表

```sql
CREATE TABLE mall_coupon (
  id VARCHAR(36) PRIMARY KEY COMMENT '优惠券ID',
  name VARCHAR(100) NOT NULL COMMENT '优惠券名称',
  type INT NOT NULL COMMENT '类型：1-固定金额，2-百分比折扣',
  value DECIMAL(10,2) NOT NULL COMMENT '面值（固定金额或折扣率）',
  min_spend DECIMAL(12,2) DEFAULT 0 COMMENT '最低消费金额',
  total_count INT NOT NULL COMMENT '发行总量',
  issued_count INT DEFAULT 0 COMMENT '已发放数量',
  used_count INT DEFAULT 0 COMMENT '已使用数量',
  start_date DATE NOT NULL COMMENT '开始日期',
  end_date DATE NOT NULL COMMENT '结束日期',
  is_deleted INT DEFAULT 0 COMMENT '是否删除：0-未删除，1-已删除',
  status INT DEFAULT 10 COMMENT '状态：10-启用，20-禁用',
  created_at DATETIME DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  updated_at DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
  created_by VARCHAR(50) COMMENT '创建人',
  updated_by VARCHAR(50) COMMENT '更新人',
  INDEX idx_status (status),
  INDEX idx_dates (start_date, end_date)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='优惠券表';
```

#### mall_order - 订单表

```sql
CREATE TABLE mall_order (
  id VARCHAR(36) PRIMARY KEY COMMENT '订单ID',
  order_no VARCHAR(50) NOT NULL UNIQUE COMMENT '订单编号',
  member_id VARCHAR(36) NOT NULL COMMENT '会员ID',
  member_name VARCHAR(50) COMMENT '会员名称',
  member_phone VARCHAR(20) COMMENT '会员手机',
  total_amount DECIMAL(12,2) NOT NULL COMMENT '商品总金额',
  discount_amount DECIMAL(12,2) DEFAULT 0 COMMENT '优惠金额',
  points_amount DECIMAL(12,2) DEFAULT 0 COMMENT '积分抵扣金额',
  shipping_fee DECIMAL(10,2) DEFAULT 0 COMMENT '运费',
  pay_amount DECIMAL(12,2) NOT NULL COMMENT '实付金额',
  coupon_id VARCHAR(36) COMMENT '优惠券ID',
  coupon_name VARCHAR(100) COMMENT '优惠券名称',
  is_deleted INT DEFAULT 0 COMMENT '是否删除：0-未删除，1-已删除',
  status INT NOT NULL COMMENT '订单状态：10-待支付，20-已支付，30-已发货，40-已完成，50-已取消，60-已退款',
  payment_method INT COMMENT '支付方式：1-微信，2-支付宝，3-银行，4-现金',
  remark TEXT COMMENT '备注',
  created_at DATETIME DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  updated_at DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
  created_by VARCHAR(50) COMMENT '创建人',
  updated_by VARCHAR(50) COMMENT '更新人',
  INDEX idx_member_id (member_id),
  INDEX idx_status (status),
  INDEX idx_created_at (created_at),
  INDEX idx_order_no (order_no)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='订单表';
```

#### mall_order_item - 订单明细表

```sql
CREATE TABLE mall_order_item (
  id VARCHAR(36) PRIMARY KEY COMMENT '明细ID',
  order_id VARCHAR(36) NOT NULL COMMENT '订单ID',
  spu_id VARCHAR(36) NOT NULL COMMENT '商品SPU ID',
  spu_name VARCHAR(200) COMMENT '商品名称',
  spec_values JSON COMMENT '规格值（JSON对象）',
  price DECIMAL(10,2) NOT NULL COMMENT '单价',
  quantity INT NOT NULL COMMENT '数量',
  subtotal DECIMAL(12,2) NOT NULL COMMENT '小计',
  is_deleted INT DEFAULT 0 COMMENT '是否删除：0-未删除，1-已删除',
  status INT DEFAULT 10 COMMENT '状态（预留字段）',
  created_at DATETIME DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  updated_at DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
  created_by VARCHAR(50) COMMENT '创建人',
  updated_by VARCHAR(50) COMMENT '更新人',
  INDEX idx_order_id (order_id),
  INDEX idx_spu_id (spu_id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='订单明细表';
```

#### mall_payment - 支付记录表

```sql
CREATE TABLE mall_payment (
  id VARCHAR(36) PRIMARY KEY COMMENT '支付ID',
  order_id VARCHAR(36) NOT NULL COMMENT '订单ID',
  order_no VARCHAR(50) COMMENT '订单编号',
  amount DECIMAL(12,2) NOT NULL COMMENT '支付金额',
  method INT NOT NULL COMMENT '支付方式：1-微信，2-支付宝，3-银行，4-现金',
  is_deleted INT DEFAULT 0 COMMENT '是否删除：0-未删除，1-已删除',
  status INT NOT NULL COMMENT '支付状态：10-待支付，20-成功，30-失败，40-已退款',
  transaction_id VARCHAR(100) COMMENT '第三方交易号',
  paid_at DATETIME COMMENT '支付时间',
  created_at DATETIME DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  updated_at DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
  created_by VARCHAR(50) COMMENT '创建人',
  updated_by VARCHAR(50) COMMENT '更新人',
  INDEX idx_order_id (order_id),
  INDEX idx_status (status),
  INDEX idx_transaction_id (transaction_id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='支付记录表';
```

#### mall_address - 会员地址表

```sql
CREATE TABLE mall_address (
  id VARCHAR(36) PRIMARY KEY COMMENT '地址ID',
  member_id VARCHAR(36) NOT NULL COMMENT '会员ID',
  receiver_name VARCHAR(50) NOT NULL COMMENT '收货人姓名',
  phone VARCHAR(20) NOT NULL COMMENT '手机号',
  province VARCHAR(50) COMMENT '省份',
  city VARCHAR(50) COMMENT '城市',
  district VARCHAR(50) COMMENT '区县',
  detail_address VARCHAR(200) COMMENT '详细地址',
  is_default BOOLEAN DEFAULT FALSE COMMENT '是否默认地址',
  is_deleted INT DEFAULT 0 COMMENT '是否删除：0-未删除，1-已删除',
  status INT DEFAULT 10 COMMENT '状态（预留字段）',
  created_at DATETIME DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  updated_at DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
  created_by VARCHAR(50) COMMENT '创建人',
  updated_by VARCHAR(50) COMMENT '更新人',
  INDEX idx_member_id (member_id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='会员地址表';
```

### 7.2 初始数据

#### 会员等级初始数据

```sql
INSERT INTO mall_level (id, name, min_points, discount, sort, is_deleted, status, created_at, updated_at) VALUES
('guid-1', '普通会员', 0, 1.00, 1, 0, 10, NOW(), NOW()),
('guid-2', '银卡会员', 1000, 0.98, 2, 0, 10, NOW(), NOW()),
('guid-3', '金卡会员', 5000, 0.95, 3, 0, 10, NOW(), NOW()),
('guid-4', '钻石会员', 10000, 0.90, 4, 0, 10, NOW(), NOW());
```

---

## 8. 业务逻辑要点

### 8.1 订单状态机

```
10-待支付 (Pending)
    ├─ 支付成功 → 20-已支付 (Paid)
    │              ├─ 发货 → 30-已发货 (Shipped)
    │              │         ├─ 确认收货 → 40-已完成 (Completed)
    │              │         └─ 退款 → 60-已退款 (Refunded)
    │              └─ 退款 → 60-已退款 (Refunded)
    └─ 超时/取消 → 50-已取消 (Cancelled)
```

**状态流转规则：**

| 当前状态 | 状态值 | 可转换状态 | 状态值 | 触发条件 |
|---------|--------|-----------|--------|---------|
| Pending | 10 | Paid | 20 | 支付成功 |
| Pending | 10 | Cancelled | 50 | 超时未支付/用户取消 |
| Paid | 20 | Shipped | 30 | 商家发货 |
| Paid | 20 | Refunded | 60 | 申请退款成功 |
| Shipped | 30 | Completed | 40 | 用户确认收货 |
| Shipped | 30 | Refunded | 60 | 申请退款成功 |
| Completed | 40 | - | - | 终态，不可转换 |
| Cancelled | 50 | - | - | 终态，不可转换 |
| Refunded | 60 | - | - | 终态，不可转换 |

### 8.2 支付流程

```
1. 创建订单 → 订单状态 10-待支付
2. 创建支付记录 → 支付状态 10-待支付
3. 调用第三方支付（微信/支付宝）
4. 支付成功回调
   ├─ 验证签名
   ├─ 更新支付记录状态 20-成功
   ├─ 更新订单状态 20-已支付
   └─ 扣减库存
5. 支付失败回调
   └─ 更新支付记录状态 30-失败
```

### 8.3 积分规则

**获得积分场景：**

| 场景 | 积分规则 | 来源 |
|------|---------|------|
| 订单完成 | 实付金额 × 积分比例（可配置） | order |
| 每日签到 | 固定积分（可配置） | signin |
| 活动 | 按活动规则 | activity |

**消费积分场景：**

| 场景 | 积分规则 | 来源 |
|------|---------|------|
| 订单抵扣 | 按配置比例（如100积分=1元） | order |
| 退款返还 | 返还已使用积分 | refund |

### 8.4 库存扣减策略

**扣减时机：** 订单支付成功后扣减

**扣减流程：**

```
1. 查询商品库存
2. 判断库存是否充足
   ├─ 充足 → 扣减库存 → 记录库存流水
   └─ 不足 → 抛出异常，回滚事务
```

**回滚场景：**

- 订单取消：回滚库存
- 订单退款：回滚库存

### 8.5 优惠券使用规则

**使用条件：**

1. 订单金额 ≥ 最低消费金额
2. 优惠券状态为 enabled
3. 当前日期在有效期内
4. 已发放数量 < 发行总量

**使用流程：**

```
1. 验证优惠券有效性
2. 计算优惠金额
   ├─ 固定金额：面值
   └─ 百分比：订单金额 × 面值
3. 更新已使用数量
4. 记录优惠券使用记录
```

---

## 9. 单元测试要求

### 9.1 强制测试场景

#### 库存扣减测试

```csharp
/// <summary>
/// 库存扣减测试
/// </summary>
public class StockTests
{
    [Fact]
    public async Task DeductStock_WhenSufficient_ShouldSucceed()
    {
        // Given: 库存充足
        // When: 扣减库存
        // Then: 库存减少，流水记录生成
    }

    [Fact]
    public async Task DeductStock_WhenInsufficient_ShouldFail()
    {
        // Given: 库存不足
        // When: 扣减库存
        // Then: 抛出异常，库存不变
    }

    [Fact]
    public async Task DeductStock_WhenConcurrent_ShouldBeThreadSafe()
    {
        // Given: 并发请求
        // When: 同时扣减库存
        // Then: 库存准确，无超卖
    }
}
```

#### 支付记录测试

```csharp
/// <summary>
/// 支付记录测试
/// </summary>
public class PaymentTests
{
    [Fact]
    public async Task CreatePayment_WhenOrderExists_ShouldSucceed()
    {
        // Given: 订单存在
        // When: 创建支付记录
        // Then: 支付记录生成，状态为 pending
    }

    [Fact]
    public async Task UpdatePaymentStatus_WhenSuccess_ShouldUpdateOrder()
    {
        // Given: 支付记录存在
        // When: 支付成功
        // Then: 支付状态更新，订单状态更新
    }

    [Fact]
    public async Task RefundPayment_WhenPaid_ShouldSucceed()
    {
        // Given: 支付成功
        // When: 申请退款
        // Then: 支付状态更新为 refunded
    }
}
```

### 9.2 测试覆盖率要求

- **强制覆盖**：库存扣减、支付记录、积分调整
- **覆盖正例 + 边界**：至少 2 个边界场景
- **并发测试**：库存扣减必须测试并发场景

---

## 10. 开发检查清单

### 10.1 开发前检查

- [ ] 已阅读 API 定义文档（`docs/api/mall-module.md`）
- [ ] 已阅读后端开发规范（`docs/backend-guidelines.md`）
- [ ] 已创建数据库表
- [ ] 已创建实体类（继承 `BaseEntity`）
- [ ] 已创建 DTO 类
- [ ] 已定义常量类

### 10.2 开发中检查

#### Controller 检查

- [ ] Controller 继承 `BaseController`
- [ ] 路由前缀正确（`api/admin/mall/<资源>`）
- [ ] 标注 `[Authorize(AuthenticationSchemes = "AdminJwt")]`
- [ ] 所有方法有中文注释（`summary` + `remarks`）
- [ ] 标注 `[ProducesResponseType]`
- [ ] 使用统一响应格式（`Success()` 方法）
- [ ] **禁止手写 try/catch**

#### Service 检查

- [ ] Service 继承 `BaseService`
- [ ] 构造器注入依赖
- [ ] 所有方法有中文注释
- [ ] 业务异常使用 `BusinessException`
- [ ] 涉钱操作使用事务
- [ ] 状态字段使用字符串常量

#### 实体类检查

- [ ] 继承 `BaseEntity`
- [ ] 标注 `[SugarTable]` 和中文描述
- [ ] 金额字段 `decimal(18,2)`
- [ ] 时间字段 `DateTime`
- [ ] 状态字段使用字符串

#### DTO 检查

- [ ] 入参 DTO 命名正确（`XxxQuery`/`XxxCreateDto`/`XxxUpdateDto`）
- [ ] 出参 DTO 命名正确（`XxxDto`）
- [ ] 使用 DataAnnotations 校验
- [ ] 禁止实体作为入参或出参

### 10.3 开发后检查

#### 构建检查

- [ ] `dotnet build` 0 错误
- [ ] `dotnet build` 0 新警告
- [ ] 所有方法有中文注释

#### 测试检查

- [ ] 强制测试场景已覆盖（库存/支付/积分）
- [ ] 测试通过：`dotnet test`
- [ ] 测试覆盖边界场景

#### 文档检查

- [ ] Swagger 注释完整
- [ ] 导出 `swagger.json` 到 `docs/api/`

#### 提交检查

- [ ] Commit 遵循 Conventional Commits
- [ ] Scope 使用 `api`
- [ ] 示例：`feat(api): Mall 模块会员管理 CRUD`

### 10.4 集成检查

- [ ] Mock 数据与接口定义一致
- [ ] 前端联调通过
- [ ] 数据库表已创建
- [ ] 索引已创建
- [ ] 初始数据已导入

---

## 附录：快速参考

### 常用状态值

```csharp
// 会员状态
MemberStatus.Active      // 10 - 激活
MemberStatus.Inactive    // 20 - 禁用

// 订单状态
OrderStatus.Pending      // 10 - 待支付
OrderStatus.Paid         // 20 - 已支付
OrderStatus.Shipped      // 30 - 已发货
OrderStatus.Completed    // 40 - 已完成
OrderStatus.Cancelled    // 50 - 已取消
OrderStatus.Refunded     // 60 - 已退款

// 支付状态
PaymentStatus.Pending    // 10 - 待支付
PaymentStatus.Success    // 20 - 成功
PaymentStatus.Failed     // 30 - 失败
PaymentStatus.Refunded   // 40 - 已退款

// 支付方式
PaymentMethod.Wechat     // 1 - 微信
PaymentMethod.Alipay     // 2 - 支付宝
PaymentMethod.Bank       // 3 - 银行
PaymentMethod.Cash       // 4 - 现金

// 积分类型
PointsType.Earn          // 1 - 获得
PointsType.Spend         // 2 - 消费

// 优惠券类型
CouponType.Fixed         // 1 - 固定金额
CouponType.Percent       // 2 - 百分比折扣
```

### 常用查询示例

```csharp
// 分页查询
var result = await _db.Queryable<Member>()
    .WhereIF(!string.IsNullOrEmpty(query.Nickname), m => m.Nickname.Contains(query.Nickname!))
    .OrderByDescending(m => m.CreatedAt)
    .ToPageListAsync(query.PageIndex, query.PageSize, total);

// 关联查询
var result = await _db.Queryable<Member>()
    .LeftJoin<MemberLevel>((m, l) => m.LevelId == l.Id.ToString())
    .Select((m, l) => new MemberDto { ... })
    .ToListAsync();

// 事务操作
using var tran = _db.Ado.UseTranAsync();
try
{
    await _db.Insertable(order).ExecuteCommandAsync();
    await _db.Insertable(orderItems).ExecuteCommandAsync();
    tran.CommitTran();
}
catch
{
    tran.RollbackTran();
    throw;
}
```

---

> 文档版本：1.0
> 更新时间：2026-09-07
> 维护者：EasyProduct 后端团队