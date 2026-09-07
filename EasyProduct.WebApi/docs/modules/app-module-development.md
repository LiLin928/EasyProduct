# App 模块后端开发说明文档

> **模块名称**：小程序会员端（App Module）
> **路由前缀**：`/api/app/**`
> **认证方式**：Member JWT（小程序会员身份）
> **文档版本**：v1.0
> **生成时间**：2026-09-07

---

## 1. 模块概述

### 1.1 定位与职责

App 模块是 EasyProduct 项目的小程序会员端，服务于微信小程序（EasyProduct.MiniApp），为注册会员提供商品浏览、购物车管理、订单管理、支付、收货地址管理等核心电商功能。

**核心职责**：
- 微信小程序登录认证（code → openid → member）
- 会员个人信息管理
- 商品浏览与详情查看
- 购物车管理
- 订单创建、查询、取消
- 微信支付对接
- 收货地址管理

### 1.2 业务边界

**包含**：
- 会员个人中心（资料、积分）
- 商品浏览（只读，商品数据来自 Product 模块）
- 购物车管理
- 订单全流程
- 支付对接
- 收货地址管理

**不包含**：
- 商品管理（由 Product 模块负责）
- 会员等级管理（由 Mall 模块 Admin 端负责）
- 优惠券管理（由 Mall 模块 Admin 端负责）
- 订单发货管理（由 Mall 模块 Admin 端负责）

### 1.3 与其他模块的依赖关系

```text
App 模块依赖：
  ├─ Basic 模块：认证授权（JWT）
  ├─ Product 模块：商品数据（只读）
  └─ Mall 模块：会员、订单、支付、地址等实体共享

依赖方向：
  App Controller → Mall Service / Product Service → Common / Models
```

**重要约束**：
- App 模块与 Mall 模块共享实体（Member, Order, Cart, Address 等）
- App 模块不得直接访问 Product 模块的数据库表，必须通过 Product Service 接口调用
- App 模块与 Admin 端 Mall 模块共用 Service 层，Controller 分开

---

## 2. 技术栈和依赖

### 2.1 核心技术栈

| 技术 | 版本 | 用途 |
|------|------|------|
| .NET | 8.0 (LTS) | 运行时 |
| SqlSugarCore | 5.1.4.x | ORM |
| Microsoft.AspNetCore.Authentication.JwtBearer | 8.x | JWT 认证 |
| Newtonsoft.Json 或 System.Text.Json | 8.x | JSON 序列化 |
| BCrypt.Net-Next | 4.x | 密码哈希（会员可选） |
| Serilog | 8.x | 日志 |

### 2.2 微信集成依赖

**微信登录**：
- **API**：`https://api.weixin.qq.com/sns/jscode2session`
- **所需参数**：appid, secret, js_code, grant_type=authorization_code
- **返回数据**：openid, session_key, unionid（可选）

**微信支付**：
- **统一下单接口**：`https://api.mch.weixin.qq.com/v3/pay/transactions/jsapi`
- **所需证书**：apiclient_key.pem, apiclient_cert.pem
- **签名算法**：RSA-SHA256
- **回调验证**：签名验证 + 证书验证

**NuGet 包**：
- 不引入第三方微信 SDK，使用 HttpClient 直接调用微信 API
- 支付签名和验签可使用 `System.Security.Cryptography` 实现

### 2.3 配置项

**appsettings.json 配置示例**：

```json
{
  "WeChat": {
    "MiniProgram": {
      "AppId": "wx1234567890abcdef",
      "Secret": "your_mini_program_secret",
      "Token": "your_token",
      "EncodingAESKey": "your_encoding_aes_key"
    },
    "Payment": {
      "MchId": "1234567890",
      "ApiKey": "your_api_key_v3",
      "CertPath": "certs/apiclient_cert.p12",
      "NotifyUrl": "https://api.yourdomain.com/api/app/payment/callback"
    }
  },
  "Jwt": {
    "Member": {
      "Issuer": "EasyProduct",
      "Audience": "EasyProduct.Member",
      "SecretKey": "your_jwt_secret_key",
      "AccessTokenExpiresMinutes": 120,
      "RefreshTokenExpiresDays": 30
    }
  }
}
```

---

## 3. 实体类设计

### 3.1 会员相关实体

#### Member - 会员

**文件路径**：`EasyProduct.Models/Entitys/Mall/Member.cs`

```csharp
using SqlSugar;
using System;

namespace EasyProduct.Models.Entitys.Mall
{
    /// <summary>
    /// 会员实体
    /// </summary>
    [SugarTable("mall_member", "会员表")]
    public class Member : BaseEntity
    {
        /// <summary>
        /// 昵称
        /// </summary>
        [SugarColumn(Length = 50, IsNullable = true)]
        public string? Nickname { get; set; }

        /// <summary>
        /// 头像URL
        /// </summary>
        [SugarColumn(Length = 500, IsNullable = true)]
        public string? Avatar { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        [SugarColumn(Length = 20, IsNullable = true)]
        public string? Phone { get; set; }

        /// <summary>
        /// 微信OpenID
        /// </summary>
        [SugarColumn(Length = 100, IsNullable = true, UniqueGroupNameList = new[] { "UQ_OpenId" })]
        public string? OpenId { get; set; }

        /// <summary>
        /// 微信UnionID
        /// </summary>
        [SugarColumn(Length = 100, IsNullable = true)]
        public string? UnionId { get; set; }

        /// <summary>
        /// 会员等级ID
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public Guid? LevelId { get; set; }

        /// <summary>
        /// 当前积分
        /// </summary>
        public int Points { get; set; } = 0;

        /// <summary>
        /// 累计消费金额
        /// </summary>
        [SugarColumn(Length = 12, DecimalDigits = 2)]
        public decimal TotalSpent { get; set; } = 0;

        /// <summary>
        /// 订单数量
        /// </summary>
        public int OrderCount { get; set; } = 0;

        /// <summary>
        /// 状态：1-活跃，0-禁用
        /// </summary>
        public int Status { get; set; } = MemberStatus.Active;
    }
}
```

#### MemberLevel - 会员等级

**文件路径**：`EasyProduct.Models/Entitys/Mall/MemberLevel.cs`

```csharp
using SqlSugar;
using System;

namespace EasyProduct.Models.Entitys.Mall
{
    /// <summary>
    /// 会员等级实体
    /// </summary>
    [SugarTable("mall_level", "会员等级表")]
    public class MemberLevel : BaseEntity
    {
        /// <summary>
        /// 等级名称
        /// </summary>
        [SugarColumn(Length = 50)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 升级所需积分
        /// </summary>
        public int MinPoints { get; set; } = 0;

        /// <summary>
        /// 折扣率（如0.95表示95折）
        /// </summary>
        [SugarColumn(Length = 3, DecimalDigits = 2)]
        public decimal Discount { get; set; } = 1.00m;

        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; } = 0;

        /// <summary>
        /// 状态：1-启用，0-禁用
        /// </summary>
        public int Status { get; set; } = MemberLevelStatus.Enabled;
    }
}
```

#### PointsRecord - 积分记录

**文件路径**：`EasyProduct.Models/Entitys/Mall/PointsRecord.cs`

```csharp
using SqlSugar;
using System;

namespace EasyProduct.Models.Entitys.Mall
{
    /// <summary>
    /// 积分记录实体
    /// </summary>
    [SugarTable("mall_points_record", "积分记录表")]
    public class PointsRecord : BaseEntity
    {
        /// <summary>
        /// 会员ID
        /// </summary>
        public Guid MemberId { get; set; }

        /// <summary>
        /// 会员名称
        /// </summary>
        [SugarColumn(Length = 50, IsNullable = true)]
        public string? MemberName { get; set; }

        /// <summary>
        /// 类型：1-获得，2-消费
        /// </summary>
        public int Type { get; set; } = PointsType.Earn;

        /// <summary>
        /// 积分数量（正数获得，负数消费）
        /// </summary>
        public int Amount { get; set; }

        /// <summary>
        /// 来源：1-订单，2-签到，3-活动，4-退款，5-调整
        /// </summary>
        public int Source { get; set; } = PointsSource.Order;

        /// <summary>
        /// 描述
        /// </summary>
        [SugarColumn(Length = 200, IsNullable = true)]
        public string? Description { get; set; }
    }
}
```

### 3.2 购物车与订单实体

#### Cart - 购物车

**文件路径**：`EasyProduct.Models/Entitys/Mall/Cart.cs`

```csharp
using SqlSugar;
using System;

namespace EasyProduct.Models.Entitys.Mall
{
    /// <summary>
    /// 购物车实体
    /// </summary>
    [SugarTable("mall_cart", "购物车表")]
    public class Cart : BaseEntity
    {
        /// <summary>
        /// 会员ID
        /// </summary>
        public Guid MemberId { get; set; }

        /// <summary>
        /// 商品SPU ID
        /// </summary>
        public Guid SpuId { get; set; }

        /// <summary>
        /// 商品SKU ID
        /// </summary>
        public Guid SkuId { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        [SugarColumn(Length = 200)]
        public string SpuName { get; set; } = string.Empty;

        /// <summary>
        /// 规格值（JSON格式）
        /// </summary>
        [SugarColumn(ColumnDataType = "json", IsNullable = true)]
        public string? SpecValues { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public int Quantity { get; set; } = 1;

        /// <summary>
        /// 单价
        /// </summary>
        [SugarColumn(Length = 10, DecimalDigits = 2)]
        public decimal Price { get; set; }

        /// <summary>
        /// 商品主图
        /// </summary>
        [SugarColumn(Length = 500, IsNullable = true)]
        public string? MainImage { get; set; }
    }
}
```

#### Order - 订单

**文件路径**：`EasyProduct.Models/Entitys/Mall/Order.cs`

```csharp
using SqlSugar;
using System;

namespace EasyProduct.Models.Entitys.Mall
{
    /// <summary>
    /// 订单实体
    /// </summary>
    [SugarTable("mall_order", "订单表")]
    public class Order : BaseEntity
    {
        /// <summary>
        /// 订单编号
        /// </summary>
        [SugarColumn(Length = 50, UniqueGroupNameList = new[] { "UQ_OrderNo" })]
        public string OrderNo { get; set; } = string.Empty;

        /// <summary>
        /// 会员ID
        /// </summary>
        public Guid MemberId { get; set; }

        /// <summary>
        /// 会员名称
        /// </summary>
        [SugarColumn(Length = 50, IsNullable = true)]
        public string? MemberName { get; set; }

        /// <summary>
        /// 会员手机
        /// </summary>
        [SugarColumn(Length = 20, IsNullable = true)]
        public string? MemberPhone { get; set; }

        /// <summary>
        /// 商品总金额
        /// </summary>
        [SugarColumn(Length = 12, DecimalDigits = 2)]
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// 优惠金额
        /// </summary>
        [SugarColumn(Length = 12, DecimalDigits = 2)]
        public decimal DiscountAmount { get; set; } = 0;

        /// <summary>
        /// 积分抵扣金额
        /// </summary>
        [SugarColumn(Length = 12, DecimalDigits = 2)]
        public decimal PointsAmount { get; set; } = 0;

        /// <summary>
        /// 运费
        /// </summary>
        [SugarColumn(Length = 10, DecimalDigits = 2)]
        public decimal ShippingFee { get; set; } = 0;

        /// <summary>
        /// 实付金额
        /// </summary>
        [SugarColumn(Length = 12, DecimalDigits = 2)]
        public decimal PayAmount { get; set; }

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
        /// 订单状态：1-待支付，2-已支付，3-已发货，4-已完成，5-已取消，6-已退款
        /// </summary>
        public int Status { get; set; } = OrderStatus.Pending;

        /// <summary>
        /// 支付方式：wechat-微信支付
        /// </summary>
        [SugarColumn(Length = 20, IsNullable = true)]
        public string? PaymentMethod { get; set; }

        /// <summary>
        /// 收货地址ID
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public Guid? AddressId { get; set; }

        /// <summary>
        /// 收货人姓名
        /// </summary>
        [SugarColumn(Length = 50, IsNullable = true)]
        public string? ReceiverName { get; set; }

        /// <summary>
        /// 收货人电话
        /// </summary>
        [SugarColumn(Length = 20, IsNullable = true)]
        public string? ReceiverPhone { get; set; }

        /// <summary>
        /// 收货地址（省市区详细地址）
        /// </summary>
        [SugarColumn(Length = 500, IsNullable = true)]
        public string? ReceiverAddress { get; set; }

        /// <summary>
        /// 物流公司
        /// </summary>
        [SugarColumn(Length = 50, IsNullable = true)]
        public string? ExpressCompany { get; set; }

        /// <summary>
        /// 物流单号
        /// </summary>
        [SugarColumn(Length = 50, IsNullable = true)]
        public string? ExpressNo { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [SugarColumn(ColumnDataType = "text", IsNullable = true)]
        public string? Remark { get; set; }

        /// <summary>
        /// 支付时间
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public DateTime? PaidAt { get; set; }

        /// <summary>
        /// 发货时间
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public DateTime? ShippedAt { get; set; }

        /// <summary>
        /// 完成时间
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public DateTime? CompletedAt { get; set; }
    }
}
```

#### OrderItem - 订单明细

**文件路径**：`EasyProduct.Models/Entitys/Mall/OrderItem.cs`

```csharp
using SqlSugar;
using System;

namespace EasyProduct.Models.Entitys.Mall
{
    /// <summary>
    /// 订单明细实体
    /// </summary>
    [SugarTable("mall_order_item", "订单明细表")]
    public class OrderItem : BaseEntity
    {
        /// <summary>
        /// 订单ID
        /// </summary>
        public Guid OrderId { get; set; }

        /// <summary>
        /// 商品SPU ID
        /// </summary>
        public Guid SpuId { get; set; }

        /// <summary>
        /// 商品SKU ID
        /// </summary>
        public Guid SkuId { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        [SugarColumn(Length = 200)]
        public string SpuName { get; set; } = string.Empty;

        /// <summary>
        /// 规格值（JSON格式）
        /// </summary>
        [SugarColumn(ColumnDataType = "json", IsNullable = true)]
        public string? SpecValues { get; set; }

        /// <summary>
        /// 单价
        /// </summary>
        [SugarColumn(Length = 10, DecimalDigits = 2)]
        public decimal Price { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// 小计
        /// </summary>
        [SugarColumn(Length = 12, DecimalDigits = 2)]
        public decimal Subtotal { get; set; }

        /// <summary>
        /// 商品主图
        /// </summary>
        [SugarColumn(Length = 500, IsNullable = true)]
        public string? MainImage { get; set; }
    }
}
```

### 3.3 支付实体

#### PaymentRecord - 支付记录

**文件路径**：`EasyProduct.Models/Entitys/Mall/PaymentRecord.cs`

```csharp
using SqlSugar;
using System;

namespace EasyProduct.Models.Entitys.Mall
{
    /// <summary>
    /// 支付记录实体
    /// </summary>
    [SugarTable("mall_payment", "支付记录表")]
    public class PaymentRecord : BaseEntity
    {
        /// <summary>
        /// 订单ID
        /// </summary>
        public Guid OrderId { get; set; }

        /// <summary>
        /// 订单编号
        /// </summary>
        [SugarColumn(Length = 50, IsNullable = true)]
        public string? OrderNo { get; set; }

        /// <summary>
        /// 支付金额
        /// </summary>
        [SugarColumn(Length = 12, DecimalDigits = 2)]
        public decimal Amount { get; set; }

        /// <summary>
        /// 支付方式：wechat-微信支付
        /// </summary>
        [SugarColumn(Length = 20)]
        public string Method { get; set; } = string.Empty;

        /// <summary>
        /// 支付状态：1-待支付，2-成功，3-失败，4-已退款
        /// </summary>
        public int Status { get; set; } = PaymentStatus.Pending;

        /// <summary>
        /// 第三方交易号
        /// </summary>
        [SugarColumn(Length = 100, IsNullable = true)]
        public string? TransactionId { get; set; }

        /// <summary>
        /// 预支付交易会话标识（微信支付返回）
        /// </summary>
        [SugarColumn(Length = 100, IsNullable = true)]
        public string? PrepayId { get; set; }

        /// <summary>
        /// 支付时间
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public DateTime? PaidAt { get; set; }

        /// <summary>
        /// 会员ID
        /// </summary>
        public Guid MemberId { get; set; }

        /// <summary>
        /// 会员OpenID
        /// </summary>
        [SugarColumn(Length = 100, IsNullable = true)]
        public string? OpenId { get; set; }
    }
}
```

### 3.4 收货地址实体

#### Address - 收货地址

**文件路径**：`EasyProduct.Models/Entitys/Mall/Address.cs`

```csharp
using SqlSugar;
using System;

namespace EasyProduct.Models.Entitys.Mall
{
    /// <summary>
    /// 收货地址实体
    /// </summary>
    [SugarTable("mall_address", "收货地址表")]
    public class Address : BaseEntity
    {
        /// <summary>
        /// 会员ID
        /// </summary>
        public Guid MemberId { get; set; }

        /// <summary>
        /// 收货人姓名
        /// </summary>
        [SugarColumn(Length = 50)]
        public string ReceiverName { get; set; } = string.Empty;

        /// <summary>
        /// 收货人电话
        /// </summary>
        [SugarColumn(Length = 20)]
        public string Phone { get; set; } = string.Empty;

        /// <summary>
        /// 省
        /// </summary>
        [SugarColumn(Length = 50, IsNullable = true)]
        public string? Province { get; set; }

        /// <summary>
        /// 市
        /// </summary>
        [SugarColumn(Length = 50, IsNullable = true)]
        public string? City { get; set; }

        /// <summary>
        /// 区
        /// </summary>
        [SugarColumn(Length = 50, IsNullable = true)]
        public string? District { get; set; }

        /// <summary>
        /// 详细地址
        /// </summary>
        [SugarColumn(Length = 200)]
        public string DetailAddress { get; set; } = string.Empty;

        /// <summary>
        /// 是否默认地址：0-否，1-是
        /// </summary>
        public int IsDefault { get; set; } = 0;
    }
}
```

### 3.5 常量定义

**文件路径**：`EasyProduct.Models/Constants/MallConstants.cs`

```csharp
namespace EasyProduct.Models.Constants
{
    /// <summary>
    /// 会员状态常量
    /// </summary>
    public static class MemberStatus
    {
        /// <summary>
        /// 活跃
        /// </summary>
        public const int Active = 1;

        /// <summary>
        /// 禁用
        /// </summary>
        public const int Inactive = 0;
    }

    /// <summary>
    /// 会员等级状态常量
    /// </summary>
    public static class MemberLevelStatus
    {
        /// <summary>
        /// 启用
        /// </summary>
        public const int Enabled = 1;

        /// <summary>
        /// 禁用
        /// </summary>
        public const int Disabled = 0;
    }

    /// <summary>
    /// 订单状态常量
    /// </summary>
    public static class OrderStatus
    {
        /// <summary>
        /// 待支付
        /// </summary>
        public const int Pending = 1;

        /// <summary>
        /// 已支付
        /// </summary>
        public const int Paid = 2;

        /// <summary>
        /// 已发货
        /// </summary>
        public const int Shipped = 3;

        /// <summary>
        /// 已完成
        /// </summary>
        public const int Completed = 4;

        /// <summary>
        /// 已取消
        /// </summary>
        public const int Cancelled = 5;

        /// <summary>
        /// 已退款
        /// </summary>
        public const int Refunded = 6;
    }

    /// <summary>
    /// 支付状态常量
    /// </summary>
    public static class PaymentStatus
    {
        /// <summary>
        /// 待支付
        /// </summary>
        public const int Pending = 1;

        /// <summary>
        /// 成功
        /// </summary>
        public const int Success = 2;

        /// <summary>
        /// 失败
        /// </summary>
        public const int Failed = 3;

        /// <summary>
        /// 已退款
        /// </summary>
        public const int Refunded = 4;
    }

    /// <summary>
    /// 积分类型常量
    /// </summary>
    public static class PointsType
    {
        /// <summary>
        /// 获得
        /// </summary>
        public const int Earn = 1;

        /// <summary>
        /// 消费
        /// </summary>
        public const int Spend = 2;
    }

    /// <summary>
    /// 积分来源常量
    /// </summary>
    public static class PointsSource
    {
        /// <summary>
        /// 订单
        /// </summary>
        public const int Order = 1;

        /// <summary>
        /// 签到
        /// </summary>
        public const int Signin = 2;

        /// <summary>
        /// 活动
        /// </summary>
        public const int Activity = 3;

        /// <summary>
        /// 退款
        /// </summary>
        public const int Refund = 4;

        /// <summary>
        /// 调整
        /// </summary>
        public const int Adjust = 5;
    }
}
```

---

## 4. DTO 设计

### 4.1 认证相关 DTO

**文件路径**：`EasyProduct.Models/Dto/App/AuthDto.cs`

```csharp
using System.ComponentModel.DataAnnotations;

namespace EasyProduct.Models.Dto.App
{
    /// <summary>
    /// 微信登录请求
    /// </summary>
    public class WeChatLoginDto
    {
        /// <summary>
        /// 微信登录凭证（code）
        /// </summary>
        [Required(ErrorMessage = "登录凭证不能为空")]
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// 用户信息（可选，用于更新昵称头像）
        /// </summary>
        public UserInfoDto? UserInfo { get; set; }
    }

    /// <summary>
    /// 用户信息DTO
    /// </summary>
    public class UserInfoDto
    {
        /// <summary>
        /// 昵称
        /// </summary>
        public string? NickName { get; set; }

        /// <summary>
        /// 头像URL
        /// </summary>
        public string? AvatarUrl { get; set; }
    }

    /// <summary>
    /// 微信登录响应
    /// </summary>
    public class WeChatLoginResultDto
    {
        /// <summary>
        /// 访问令牌
        /// </summary>
        public string AccessToken { get; set; } = string.Empty;

        /// <summary>
        /// 刷新令牌
        /// </summary>
        public string? RefreshToken { get; set; }

        /// <summary>
        /// 过期时间（秒）
        /// </summary>
        public int ExpiresIn { get; set; }

        /// <summary>
        /// 是否新用户
        /// </summary>
        public bool IsNewMember { get; set; }

        /// <summary>
        /// 会员ID
        /// </summary>
        public string MemberId { get; set; } = string.Empty;
    }

    /// <summary>
    /// 绑定手机号请求
    /// </summary>
    public class BindPhoneDto
    {
        /// <summary>
        /// 手机号
        /// </summary>
        [Required(ErrorMessage = "手机号不能为空")]
        [RegularExpression(@"^1[3-9]\d{9}$", ErrorMessage = "手机号格式不正确")]
        public string Phone { get; set; } = string.Empty;

        /// <summary>
        /// 微信手机号加密数据
        /// </summary>
        public string? EncryptedData { get; set; }

        /// <summary>
        /// 加密算法初始向量
        /// </summary>
        public string? Iv { get; set; }
    }
}
```

### 4.2 会员相关 DTO

**文件路径**：`EasyProduct.Models/Dto/App/MemberDto.cs`

```csharp
namespace EasyProduct.Models.Dto.App
{
    /// <summary>
    /// 会员信息DTO
    /// </summary>
    public class MemberInfoDto
    {
        /// <summary>
        /// 会员ID
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// 昵称
        /// </summary>
        public string? Nickname { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        public string? Avatar { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        public string? Phone { get; set; }

        /// <summary>
        /// 会员等级名称
        /// </summary>
        public string? LevelName { get; set; }

        /// <summary>
        /// 当前积分
        /// </summary>
        public int Points { get; set; }

        /// <summary>
        /// 累计消费金额
        /// </summary>
        public decimal TotalSpent { get; set; }

        /// <summary>
        /// 订单数量
        /// </summary>
        public int OrderCount { get; set; }
    }

    /// <summary>
    /// 会员资料DTO
    /// </summary>
    public class MemberProfileDto
    {
        /// <summary>
        /// 昵称
        /// </summary>
        public string? Nickname { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        public string? Avatar { get; set; }

        /// <summary>
        /// 性别：0-未知，1-男，2-女
        /// </summary>
        public int? Gender { get; set; }

        /// <summary>
        /// 生日
        /// </summary>
        public DateTime? Birthday { get; set; }
    }

    /// <summary>
    /// 更新会员资料请求
    /// </summary>
    public class UpdateMemberProfileDto
    {
        /// <summary>
        /// 昵称
        /// </summary>
        public string? Nickname { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        public string? Avatar { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public int? Gender { get; set; }

        /// <summary>
        /// 生日
        /// </summary>
        public DateTime? Birthday { get; set; }
    }

    /// <summary>
    /// 积分记录查询DTO
    /// </summary>
    public class PointsRecordQueryDto : PageQuery
    {
        /// <summary>
        /// 类型：1-获得，2-消费
        /// </summary>
        public int? Type { get; set; }

        /// <summary>
        /// 来源：1-订单，2-签到，3-活动，4-退款，5-调整
        /// </summary>
        public int? Source { get; set; }
    }

    /// <summary>
    /// 积分记录DTO
    /// </summary>
    public class PointsRecordDto
    {
        /// <summary>
        /// 记录ID
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// 类型：1-获得，2-消费
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// 类型文本
        /// </summary>
        public string TypeText { get; set; } = string.Empty;

        /// <summary>
        /// 积分数量
        /// </summary>
        public int Amount { get; set; }

        /// <summary>
        /// 来源：1-订单，2-签到，3-活动，4-退款，5-调整
        /// </summary>
        public int Source { get; set; }

        /// <summary>
        /// 来源文本
        /// </summary>
        public string SourceText { get; set; } = string.Empty;

        /// <summary>
        /// 描述
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
}
```

### 4.3 购物车相关 DTO

**文件路径**：`EasyProduct.Models/Dto/App/CartDto.cs`

```csharp
using System.ComponentModel.DataAnnotations;

namespace EasyProduct.Models.Dto.App
{
    /// <summary>
    /// 购物车项DTO
    /// </summary>
    public class CartItemDto
    {
        /// <summary>
        /// 购物车ID
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// 商品SPU ID
        /// </summary>
        public string SpuId { get; set; } = string.Empty;

        /// <summary>
        /// 商品SKU ID
        /// </summary>
        public string SkuId { get; set; } = string.Empty;

        /// <summary>
        /// 商品名称
        /// </summary>
        public string SpuName { get; set; } = string.Empty;

        /// <summary>
        /// 规格值
        /// </summary>
        public object? SpecValues { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// 单价
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 小计
        /// </summary>
        public decimal Subtotal { get; set; }

        /// <summary>
        /// 商品主图
        /// </summary>
        public string? MainImage { get; set; }

        /// <summary>
        /// 库存
        /// </summary>
        public int Stock { get; set; }

        /// <summary>
        /// 商品状态
        /// </summary>
        public string? Status { get; set; }
    }

    /// <summary>
    /// 加入购物车请求
    /// </summary>
    public class AddToCartDto
    {
        /// <summary>
        /// 商品SPU ID
        /// </summary>
        [Required(ErrorMessage = "商品ID不能为空")]
        public string SpuId { get; set; } = string.Empty;

        /// <summary>
        /// 商品SKU ID
        /// </summary>
        [Required(ErrorMessage = "SKU ID不能为空")]
        public string SkuId { get; set; } = string.Empty;

        /// <summary>
        /// 数量
        /// </summary>
        [Range(1, 999, ErrorMessage = "数量必须在1-999之间")]
        public int Quantity { get; set; } = 1;
    }

    /// <summary>
    /// 更新购物车数量请求
    /// </summary>
    public class UpdateCartQuantityDto
    {
        /// <summary>
        /// 数量
        /// </summary>
        [Range(1, 999, ErrorMessage = "数量必须在1-999之间")]
        public int Quantity { get; set; }
    }
}
```

### 4.4 订单相关 DTO

**文件路径**：`EasyProduct.Models/Dto/App/OrderDto.cs`

```csharp
using System.ComponentModel.DataAnnotations;

namespace EasyProduct.Models.Dto.App
{
    /// <summary>
    /// 订单列表查询DTO
    /// </summary>
    public class OrderQueryDto : PageQuery
    {
        /// <summary>
        /// 订单状态：1-待支付，2-已支付，3-已发货，4-已完成，5-已取消，6-已退款
        /// </summary>
        public int? Status { get; set; }
    }

    /// <summary>
    /// 订单DTO
    /// </summary>
    public class OrderDto
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
        /// 商品总金额
        /// </summary>
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// 优惠金额
        /// </summary>
        public decimal DiscountAmount { get; set; }

        /// <summary>
        /// 积分抵扣金额
        /// </summary>
        public decimal PointsAmount { get; set; }

        /// <summary>
        /// 运费
        /// </summary>
        public decimal ShippingFee { get; set; }

        /// <summary>
        /// 实付金额
        /// </summary>
        public decimal PayAmount { get; set; }

        /// <summary>
        /// 订单状态：1-待支付，2-已支付，3-已发货，4-已完成，5-已取消，6-已退款
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 订单状态文本
        /// </summary>
        public string StatusText { get; set; } = string.Empty;

        /// <summary>
        /// 订单明细
        /// </summary>
        public List<OrderItemDto> Items { get; set; } = new List<OrderItemDto>();

        /// <summary>
        /// 收货人姓名
        /// </summary>
        public string? ReceiverName { get; set; }

        /// <summary>
        /// 收货人电话
        /// </summary>
        public string? ReceiverPhone { get; set; }

        /// <summary>
        /// 收货地址
        /// </summary>
        public string? ReceiverAddress { get; set; }

        /// <summary>
        /// 物流公司
        /// </summary>
        public string? ExpressCompany { get; set; }

        /// <summary>
        /// 物流单号
        /// </summary>
        public string? ExpressNo { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 支付时间
        /// </summary>
        public DateTime? PaidAt { get; set; }

        /// <summary>
        /// 发货时间
        /// </summary>
        public DateTime? ShippedAt { get; set; }

        /// <summary>
        /// 完成时间
        /// </summary>
        public DateTime? CompletedAt { get; set; }
    }

    /// <summary>
    /// 订单明细DTO
    /// </summary>
    public class OrderItemDto
    {
        /// <summary>
        /// 明细ID
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// 商品SPU ID
        /// </summary>
        public string SpuId { get; set; } = string.Empty;

        /// <summary>
        /// 商品名称
        /// </summary>
        public string SpuName { get; set; } = string.Empty;

        /// <summary>
        /// 规格值
        /// </summary>
        public object? SpecValues { get; set; }

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
        /// 商品主图
        /// </summary>
        public string? MainImage { get; set; }
    }

    /// <summary>
    /// 创建订单请求
    /// </summary>
    public class CreateOrderDto
    {
        /// <summary>
        /// 收货地址ID
        /// </summary>
        [Required(ErrorMessage = "收货地址不能为空")]
        public string AddressId { get; set; } = string.Empty;

        /// <summary>
        /// 购物车ID列表（从购物车创建订单）
        /// </summary>
        public List<string>? CartIds { get; set; }

        /// <summary>
        /// 优惠券ID
        /// </summary>
        public string? CouponId { get; set; }

        /// <summary>
        /// 使用积分
        /// </summary>
        public int UsePoints { get; set; } = 0;

        /// <summary>
        /// 备注
        /// </summary>
        [MaxLength(200, ErrorMessage = "备注不能超过200字")]
        public string? Remark { get; set; }
    }

    /// <summary>
    /// 创建订单结果
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
        /// 实付金额
        /// </summary>
        public decimal PayAmount { get; set; }
    }
}
```

### 4.5 支付相关 DTO

**文件路径**：`EasyProduct.Models/Dto/App/PaymentDto.cs`

```csharp
using System.ComponentModel.DataAnnotations;

namespace EasyProduct.Models.Dto.App
{
    /// <summary>
    /// 创建支付请求
    /// </summary>
    public class CreatePaymentDto
    {
        /// <summary>
        /// 订单ID
        /// </summary>
        [Required(ErrorMessage = "订单ID不能为空")]
        public string OrderId { get; set; } = string.Empty;
    }

    /// <summary>
    /// 创建支付结果
    /// </summary>
    public class CreatePaymentResultDto
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
        public string SignType { get; set; } = string.Empty;

        /// <summary>
        /// 签名
        /// </summary>
        public string PaySign { get; set; } = string.Empty;
    }

    /// <summary>
    /// 支付回调通知
    /// </summary>
    public class PaymentCallbackDto
    {
        /// <summary>
        /// 微信支付回调原始数据
        /// </summary>
        public string RawData { get; set; } = string.Empty;
    }
}
```

### 4.6 收货地址相关 DTO

**文件路径**：`EasyProduct.Models/Dto/App/AddressDto.cs`

```csharp
using System.ComponentModel.DataAnnotations;

namespace EasyProduct.Models.Dto.App
{
    /// <summary>
    /// 收货地址DTO
    /// </summary>
    public class AddressDto
    {
        /// <summary>
        /// 地址ID
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// 收货人姓名
        /// </summary>
        public string ReceiverName { get; set; } = string.Empty;

        /// <summary>
        /// 收货人电话
        /// </summary>
        public string Phone { get; set; } = string.Empty;

        /// <summary>
        /// 省
        /// </summary>
        public string? Province { get; set; }

        /// <summary>
        /// 市
        /// </summary>
        public string? City { get; set; }

        /// <summary>
        /// 区
        /// </summary>
        public string? District { get; set; }

        /// <summary>
        /// 详细地址
        /// </summary>
        public string DetailAddress { get; set; } = string.Empty;

        /// <summary>
        /// 完整地址
        /// </summary>
        public string FullAddress { get; set; } = string.Empty;

        /// <summary>
        /// 是否默认地址
        /// </summary>
        public bool IsDefault { get; set; }
    }

    /// <summary>
    /// 创建收货地址请求
    /// </summary>
    public class CreateAddressDto
    {
        /// <summary>
        /// 收货人姓名
        /// </summary>
        [Required(ErrorMessage = "收货人姓名不能为空")]
        [MaxLength(50, ErrorMessage = "收货人姓名不能超过50字")]
        public string ReceiverName { get; set; } = string.Empty;

        /// <summary>
        /// 收货人电话
        /// </summary>
        [Required(ErrorMessage = "收货人电话不能为空")]
        [RegularExpression(@"^1[3-9]\d{9}$", ErrorMessage = "手机号格式不正确")]
        public string Phone { get; set; } = string.Empty;

        /// <summary>
        /// 省
        /// </summary>
        [MaxLength(50, ErrorMessage = "省份不能超过50字")]
        public string? Province { get; set; }

        /// <summary>
        /// 市
        /// </summary>
        [MaxLength(50, ErrorMessage = "城市不能超过50字")]
        public string? City { get; set; }

        /// <summary>
        /// 区
        /// </summary>
        [MaxLength(50, ErrorMessage = "区县不能超过50字")]
        public string? District { get; set; }

        /// <summary>
        /// 详细地址
        /// </summary>
        [Required(ErrorMessage = "详细地址不能为空")]
        [MaxLength(200, ErrorMessage = "详细地址不能超过200字")]
        public string DetailAddress { get; set; } = string.Empty;

        /// <summary>
        /// 是否默认地址：0-否，1-是
        /// </summary>
        public int IsDefault { get; set; } = 0;
    }

    /// <summary>
    /// 更新收货地址请求
    /// </summary>
    public class UpdateAddressDto
    {
        /// <summary>
        /// 收货人姓名
        /// </summary>
        [Required(ErrorMessage = "收货人姓名不能为空")]
        [MaxLength(50, ErrorMessage = "收货人姓名不能超过50字")]
        public string ReceiverName { get; set; } = string.Empty;

        /// <summary>
        /// 收货人电话
        /// </summary>
        [Required(ErrorMessage = "收货人电话不能为空")]
        [RegularExpression(@"^1[3-9]\d{9}$", ErrorMessage = "手机号格式不正确")]
        public string Phone { get; set; } = string.Empty;

        /// <summary>
        /// 省
        /// </summary>
        [MaxLength(50, ErrorMessage = "省份不能超过50字")]
        public string? Province { get; set; }

        /// <summary>
        /// 市
        /// </summary>
        [MaxLength(50, ErrorMessage = "城市不能超过50字")]
        public string? City { get; set; }

        /// <summary>
        /// 区
        /// </summary>
        [MaxLength(50, ErrorMessage = "区县不能超过50字")]
        public string? District { get; set; }

        /// <summary>
        /// 详细地址
        /// </summary>
        [Required(ErrorMessage = "详细地址不能为空")]
        [MaxLength(200, ErrorMessage = "详细地址不能超过200字")]
        public string DetailAddress { get; set; } = string.Empty;

        /// <summary>
        /// 是否默认地址：0-否，1-是
        /// </summary>
        public int IsDefault { get; set; } = 0;
    }
}
```

---

## 5. Service 层实现指南

### 5.1 Service 接口定义

#### IAppAuthService - 认证服务

**文件路径**：`EasyProduct.Business/App/IAppAuthService.cs`

```csharp
using EasyProduct.Models.Dto.App;

namespace EasyProduct.Business.App
{
    /// <summary>
    /// App认证服务接口
    /// </summary>
    public interface IAppAuthService
    {
        /// <summary>
        /// 微信登录
        /// </summary>
        /// <param name="dto">微信登录请求</param>
        /// <returns>登录结果</returns>
        Task<WeChatLoginResultDto> WeChatLoginAsync(WeChatLoginDto dto);

        /// <summary>
        /// 绑定手机号
        /// </summary>
        /// <param name="memberId">会员ID</param>
        /// <param name="dto">绑定手机号请求</param>
        /// <returns>是否成功</returns>
        Task<bool> BindPhoneAsync(Guid memberId, BindPhoneDto dto);

        /// <summary>
        /// 刷新Token
        /// </summary>
        /// <param name="refreshToken">刷新令牌</param>
        /// <returns>新的登录结果</returns>
        Task<WeChatLoginResultDto> RefreshTokenAsync(string refreshToken);
    }
}
```

#### IAppMemberService - 会员服务

**文件路径**：`EasyProduct.Business/App/IAppMemberService.cs`

```csharp
using EasyProduct.Models.Dto.App;
using EasyProduct.Models.Common;

namespace EasyProduct.Business.App
{
    /// <summary>
    /// App会员服务接口
    /// </summary>
    public interface IAppMemberService
    {
        /// <summary>
        /// 获取当前会员信息
        /// </summary>
        /// <param name="memberId">会员ID</param>
        /// <returns>会员信息</returns>
        Task<MemberInfoDto> GetMemberInfoAsync(Guid memberId);

        /// <summary>
        /// 获取会员资料
        /// </summary>
        /// <param name="memberId">会员ID</param>
        /// <returns>会员资料</returns>
        Task<MemberProfileDto> GetMemberProfileAsync(Guid memberId);

        /// <summary>
        /// 更新会员资料
        /// </summary>
        /// <param name="memberId">会员ID</param>
        /// <param name="dto">更新资料请求</param>
        /// <returns>是否成功</returns>
        Task<bool> UpdateMemberProfileAsync(Guid memberId, UpdateMemberProfileDto dto);

        /// <summary>
        /// 获取积分记录
        /// </summary>
        /// <param name="memberId">会员ID</param>
        /// <param name="query">查询参数</param>
        /// <returns>积分记录分页结果</returns>
        Task<PageResult<PointsRecordDto>> GetPointsRecordsAsync(Guid memberId, PointsRecordQueryDto query);
    }
}
```

#### IAppCartService - 购物车服务

**文件路径**：`EasyProduct.Business/App/IAppCartService.cs`

```csharp
using EasyProduct.Models.Dto.App;

namespace EasyProduct.Business.App
{
    /// <summary>
    /// App购物车服务接口
    /// </summary>
    public interface IAppCartService
    {
        /// <summary>
        /// 获取购物车列表
        /// </summary>
        /// <param name="memberId">会员ID</param>
        /// <returns>购物车列表</returns>
        Task<List<CartItemDto>> GetCartListAsync(Guid memberId);

        /// <summary>
        /// 加入购物车
        /// </summary>
        /// <param name="memberId">会员ID</param>
        /// <param name="dto">加入购物车请求</param>
        /// <returns>购物车项ID</returns>
        Task<string> AddToCartAsync(Guid memberId, AddToCartDto dto);

        /// <summary>
        /// 更新购物车数量
        /// </summary>
        /// <param name="memberId">会员ID</param>
        /// <param name="cartId">购物车ID</param>
        /// <param name="dto">更新数量请求</param>
        /// <returns>是否成功</returns>
        Task<bool> UpdateCartQuantityAsync(Guid memberId, Guid cartId, UpdateCartQuantityDto dto);

        /// <summary>
        /// 删除购物车商品
        /// </summary>
        /// <param name="memberId">会员ID</param>
        /// <param name="cartId">购物车ID</param>
        /// <returns>是否成功</returns>
        Task<bool> DeleteCartAsync(Guid memberId, Guid cartId);

        /// <summary>
        /// 清空购物车
        /// </summary>
        /// <param name="memberId">会员ID</param>
        /// <returns>是否成功</returns>
        Task<bool> ClearCartAsync(Guid memberId);
    }
}
```

#### IAppOrderService - 订单服务

**文件路径**：`EasyProduct.Business/App/IAppOrderService.cs`

```csharp
using EasyProduct.Models.Dto.App;
using EasyProduct.Models.Common;

namespace EasyProduct.Business.App
{
    /// <summary>
    /// App订单服务接口
    /// </summary>
    public interface IAppOrderService
    {
        /// <summary>
        /// 获取订单列表
        /// </summary>
        /// <param name="memberId">会员ID</param>
        /// <param name="query">查询参数</param>
        /// <returns>订单分页结果</returns>
        Task<PageResult<OrderDto>> GetOrderListAsync(Guid memberId, OrderQueryDto query);

        /// <summary>
        /// 获取订单详情
        /// </summary>
        /// <param name="memberId">会员ID</param>
        /// <param name="orderId">订单ID</param>
        /// <returns>订单详情</returns>
        Task<OrderDto> GetOrderDetailAsync(Guid memberId, Guid orderId);

        /// <summary>
        /// 创建订单
        /// </summary>
        /// <param name="memberId">会员ID</param>
        /// <param name="dto">创建订单请求</param>
        /// <returns>创建订单结果</returns>
        Task<CreateOrderResultDto> CreateOrderAsync(Guid memberId, CreateOrderDto dto);

        /// <summary>
        /// 取消订单
        /// </summary>
        /// <param name="memberId">会员ID</param>
        /// <param name="orderId">订单ID</param>
        /// <returns>是否成功</returns>
        Task<bool> CancelOrderAsync(Guid memberId, Guid orderId);
    }
}
```

#### IAppPaymentService - 支付服务

**文件路径**：`EasyProduct.Business/App/IAppPaymentService.cs`

```csharp
using EasyProduct.Models.Dto.App;

namespace EasyProduct.Business.App
{
    /// <summary>
    /// App支付服务接口
    /// </summary>
    public interface IAppPaymentService
    {
        /// <summary>
        /// 创建支付
        /// </summary>
        /// <param name="memberId">会员ID</param>
        /// <param name="dto">创建支付请求</param>
        /// <returns>微信支付参数</returns>
        Task<CreatePaymentResultDto> CreatePaymentAsync(Guid memberId, CreatePaymentDto dto);

        /// <summary>
        /// 处理支付回调
        /// </summary>
        /// <param name="callbackData">回调数据</param>
        /// <returns>是否成功</returns>
        Task<bool> HandlePaymentCallbackAsync(string callbackData);
    }
}
```

#### IAppAddressService - 收货地址服务

**文件路径**：`EasyProduct.Business/App/IAppAddressService.cs`

```csharp
using EasyProduct.Models.Dto.App;

namespace EasyProduct.Business.App
{
    /// <summary>
    /// App收货地址服务接口
    /// </summary>
    public interface IAppAddressService
    {
        /// <summary>
        /// 获取地址列表
        /// </summary>
        /// <param name="memberId">会员ID</param>
        /// <returns>地址列表</returns>
        Task<List<AddressDto>> GetAddressListAsync(Guid memberId);

        /// <summary>
        /// 获取地址详情
        /// </summary>
        /// <param name="memberId">会员ID</param>
        /// <param name="addressId">地址ID</param>
        /// <returns>地址详情</returns>
        Task<AddressDto> GetAddressDetailAsync(Guid memberId, Guid addressId);

        /// <summary>
        /// 创建地址
        /// </summary>
        /// <param name="memberId">会员ID</param>
        /// <param name="dto">创建地址请求</param>
        /// <returns>地址ID</returns>
        Task<string> CreateAddressAsync(Guid memberId, CreateAddressDto dto);

        /// <summary>
        /// 更新地址
        /// </summary>
        /// <param name="memberId">会员ID</param>
        /// <param name="addressId">地址ID</param>
        /// <param name="dto">更新地址请求</param>
        /// <returns>是否成功</returns>
        Task<bool> UpdateAddressAsync(Guid memberId, Guid addressId, UpdateAddressDto dto);

        /// <summary>
        /// 删除地址
        /// </summary>
        /// <param name="memberId">会员ID</param>
        /// <param name="addressId">地址ID</param>
        /// <returns>是否成功</returns>
        Task<bool> DeleteAddressAsync(Guid memberId, Guid addressId);
    }
}
```

### 5.2 Service 实现要点

#### AppAuthService 实现要点

```csharp
/// <summary>
/// 微信登录
/// </summary>
/// <remarks>
/// 1. 调用微信 API：code2Session 获取 openid 和 session_key
/// 2. 根据 openid 查询会员表，存在则更新登录时间，不存在则创建新会员
/// 3. 生成 JWT Token（Member JWT Scheme）
/// 4. 返回 Token 和会员信息
/// </remarks>
public async Task<WeChatLoginResultDto> WeChatLoginAsync(WeChatLoginDto dto)
{
    // 实现逻辑：
    // 1. 调用微信 API
    var weChatResult = await CallWeChatCode2SessionAsync(dto.Code);

    // 2. 查询或创建会员
    var member = await _db.Queryable<Member>()
        .FirstAsync(m => m.OpenId == weChatResult.OpenId);

    bool isNewMember = member == null;
    if (isNewMember)
    {
        member = new Member
        {
            OpenId = weChatResult.OpenId,
            Nickname = dto.UserInfo?.NickName,
            Avatar = dto.UserInfo?.AvatarUrl,
            Status = MemberStatus.Active
        };
        await _db.Insertable(member).ExecuteCommandAsync();
    }
    else
    {
        // 更新用户信息（如果有传入）
        if (dto.UserInfo != null)
        {
            member.Nickname = dto.UserInfo.NickName;
            member.Avatar = dto.UserInfo.AvatarUrl;
            await _db.Updateable(member).ExecuteCommandAsync();
        }
    }

    // 3. 生成 JWT
    var token = GenerateJwtToken(member);

    return new WeChatLoginResultDto
    {
        AccessToken = token.AccessToken,
        RefreshToken = token.RefreshToken,
        ExpiresIn = token.ExpiresIn,
        IsNewMember = isNewMember,
        MemberId = member.Id.ToString()
    };
}
```

#### AppOrderService 实现要点

```csharp
/// <summary>
/// 创建订单
/// </summary>
/// <remarks>
/// 业务逻辑：
/// 1. 验证购物车商品（库存、状态）
/// 2. 计算价格（商品总价、优惠金额、积分抵扣、运费）
/// 3. 创建订单主表和明细表（事务）
/// 4. 扣减库存
/// 5. 清除购物车
/// 6. 返回订单ID和编号
/// </remarks>
public async Task<CreateOrderResultDto> CreateOrderAsync(Guid memberId, CreateOrderDto dto)
{
    // 使用事务确保数据一致性
    var result = await _db.Ado.UseTranAsync(async () =>
    {
        // 1. 获取购物车商品
        var cartItems = await GetCartItemsAsync(memberId, dto.CartIds);

        // 2. 验证商品库存
        await ValidateStockAsync(cartItems);

        // 3. 计算价格
        var priceResult = CalculateOrderPrice(cartItems, dto);

        // 4. 获取收货地址
        var address = await GetAddressAsync(dto.AddressId);

        // 5. 生成订单编号
        var orderNo = GenerateOrderNo();

        // 6. 创建订单主表
        var order = new Order
        {
            OrderNo = orderNo,
            MemberId = memberId,
            TotalAmount = priceResult.TotalAmount,
            DiscountAmount = priceResult.DiscountAmount,
            PointsAmount = priceResult.PointsAmount,
            ShippingFee = priceResult.ShippingFee,
            PayAmount = priceResult.PayAmount,
            Status = OrderStatus.Pending,
            ReceiverName = address.ReceiverName,
            ReceiverPhone = address.Phone,
            ReceiverAddress = address.FullAddress,
            Remark = dto.Remark
        };
        await _db.Insertable(order).ExecuteCommandAsync();

        // 7. 创建订单明细
        var orderItems = cartItems.Select(c => new OrderItem
        {
            OrderId = order.Id,
            SpuId = c.SpuId,
            SkuId = c.SkuId,
            SpuName = c.SpuName,
            SpecValues = c.SpecValues,
            Price = c.Price,
            Quantity = c.Quantity,
            Subtotal = c.Price * c.Quantity,
            MainImage = c.MainImage
        }).ToList();
        await _db.Insertable(orderItems).ExecuteCommandAsync();

        // 8. 扣减库存
        await DeductStockAsync(cartItems);

        // 9. 清除购物车
        await ClearCartItemsAsync(memberId, dto.CartIds);

        // 10. 扣减积分（如果使用）
        if (dto.UsePoints > 0)
        {
            await DeductPointsAsync(memberId, dto.UsePoints);
        }

        return new CreateOrderResultDto
        {
            OrderId = order.Id.ToString(),
            OrderNo = orderNo,
            PayAmount = priceResult.PayAmount
        };
    });

    return result;
}
```

---

## 6. Controller 层实现指南

### 6.1 Controller 设计规范

**路由前缀**：`/api/app/<模块>/<资源>`

**认证要求**：所有 App Controller 必须标注 `[Authorize(AuthenticationSchemes = "MemberJwt")]`

**基类**：继承 `BaseController`

**方法命名**：
- 列表：`GetList`
- 详情：`GetById`
- 创建：`Create`
- 更新：`Update`
- 删除：`Delete`

### 6.2 Controller 示例

#### AppAuthController - 认证控制器

**文件路径**：`EasyProduct.Web/Controllers/App/AuthController.cs`

```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using EasyProduct.Business.App;
using EasyProduct.Models.Dto.App;
using EasyProduct.Web.Controllers.Base;

namespace EasyProduct.Web.Controllers.App
{
    /// <summary>
    /// 小程序认证控制器
    /// </summary>
    [ApiController]
    [Route("api/app/auth")]
    public class AuthController : BaseController
    {
        private readonly IAppAuthService _authService;

        public AuthController(IAppAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// 微信登录
        /// </summary>
        /// <param name="dto">微信登录请求</param>
        /// <returns>登录结果</returns>
        /// <remarks>
        /// 1. 小程序调用 wx.login() 获取 code
        /// 2. 将 code 传给此接口
        /// 3. 后端调用微信 API 获取 openid
        /// 4. 根据 openid 创建或更新会员
        /// 5. 返回 JWT Token
        /// </remarks>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] WeChatLoginDto dto)
        {
            var result = await _authService.WeChatLoginAsync(dto);
            return Success(result);
        }

        /// <summary>
        /// 绑定手机号
        /// </summary>
        /// <param name="dto">绑定手机号请求</param>
        /// <returns>是否成功</returns>
        [HttpPost("phone")]
        [Authorize(AuthenticationSchemes = "MemberJwt")]
        public async Task<IActionResult> BindPhone([FromBody] BindPhoneDto dto)
        {
            var memberId = GetCurrentUserId();
            await _authService.BindPhoneAsync(memberId, dto);
            return Success(true);
        }
    }
}
```

#### AppMemberController - 会员控制器

**文件路径**：`EasyProduct.Web/Controllers/App/MemberController.cs`

```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using EasyProduct.Business.App;
using EasyProduct.Models.Dto.App;
using EasyProduct.Web.Controllers.Base;

namespace EasyProduct.Web.Controllers.App
{
    /// <summary>
    /// 小程序会员控制器
    /// </summary>
    [ApiController]
    [Route("api/app/member")]
    [Authorize(AuthenticationSchemes = "MemberJwt")]
    public class MemberController : BaseController
    {
        private readonly IAppMemberService _memberService;

        public MemberController(IAppMemberService memberService)
        {
            _memberService = memberService;
        }

        /// <summary>
        /// 获取当前会员信息
        /// </summary>
        /// <returns>会员信息</returns>
        [HttpGet("info")]
        public async Task<IActionResult> GetInfo()
        {
            var memberId = GetCurrentUserId();
            var result = await _memberService.GetMemberInfoAsync(memberId);
            return Success(result);
        }

        /// <summary>
        /// 获取会员资料
        /// </summary>
        /// <returns>会员资料</returns>
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var memberId = GetCurrentUserId();
            var result = await _memberService.GetMemberProfileAsync(memberId);
            return Success(result);
        }

        /// <summary>
        /// 更新会员资料
        /// </summary>
        /// <param name="dto">更新资料请求</param>
        /// <returns>是否成功</returns>
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateMemberProfileDto dto)
        {
            var memberId = GetCurrentUserId();
            await _memberService.UpdateMemberProfileAsync(memberId, dto);
            return Success(true);
        }

        /// <summary>
        /// 获取积分记录
        /// </summary>
        /// <param name="query">查询参数</param>
        /// <returns>积分记录分页结果</returns>
        [HttpGet("points")]
        public async Task<IActionResult> GetPointsRecords([FromQuery] PointsRecordQueryDto query)
        {
            var memberId = GetCurrentUserId();
            var result = await _memberService.GetPointsRecordsAsync(memberId, query);
            return Success(result);
        }
    }
}
```

#### AppCartController - 购物车控制器

**文件路径**：`EasyProduct.Web/Controllers/App/CartController.cs`

```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using EasyProduct.Business.App;
using EasyProduct.Models.Dto.App;
using EasyProduct.Web.Controllers.Base;

namespace EasyProduct.Web.Controllers.App
{
    /// <summary>
    /// 小程序购物车控制器
    /// </summary>
    [ApiController]
    [Route("api/app/cart")]
    [Authorize(AuthenticationSchemes = "MemberJwt")]
    public class CartController : BaseController
    {
        private readonly IAppCartService _cartService;

        public CartController(IAppCartService cartService)
        {
            _cartService = cartService;
        }

        /// <summary>
        /// 获取购物车列表
        /// </summary>
        /// <returns>购物车列表</returns>
        [HttpGet("list")]
        public async Task<IActionResult> GetList()
        {
            var memberId = GetCurrentUserId();
            var result = await _cartService.GetCartListAsync(memberId);
            return Success(result);
        }

        /// <summary>
        /// 加入购物车
        /// </summary>
        /// <param name="dto">加入购物车请求</param>
        /// <returns>购物车项ID</returns>
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] AddToCartDto dto)
        {
            var memberId = GetCurrentUserId();
            var result = await _cartService.AddToCartAsync(memberId, dto);
            return Success(result);
        }

        /// <summary>
        /// 更新购物车数量
        /// </summary>
        /// <param name="id">购物车ID</param>
        /// <param name="dto">更新数量请求</param>
        /// <returns>是否成功</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCartQuantityDto dto)
        {
            var memberId = GetCurrentUserId();
            await _cartService.UpdateCartQuantityAsync(memberId, id, dto);
            return Success(true);
        }

        /// <summary>
        /// 删除购物车商品
        /// </summary>
        /// <param name="id">购物车ID</param>
        /// <returns>是否成功</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var memberId = GetCurrentUserId();
            await _cartService.DeleteCartAsync(memberId, id);
            return Success(true);
        }
    }
}
```

#### AppOrderController - 订单控制器

**文件路径**：`EasyProduct.Web/Controllers/App/OrderController.cs`

```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using EasyProduct.Business.App;
using EasyProduct.Models.Dto.App;
using EasyProduct.Web.Controllers.Base;

namespace EasyProduct.Web.Controllers.App
{
    /// <summary>
    /// 小程序订单控制器
    /// </summary>
    [ApiController]
    [Route("api/app/orders")]
    [Authorize(AuthenticationSchemes = "MemberJwt")]
    public class OrderController : BaseController
    {
        private readonly IAppOrderService _orderService;

        public OrderController(IAppOrderService orderService)
        {
            _orderService = orderService;
        }

        /// <summary>
        /// 获取订单列表
        /// </summary>
        /// <param name="query">查询参数</param>
        /// <returns>订单分页结果</returns>
        [HttpGet("list")]
        public async Task<IActionResult> GetList([FromQuery] OrderQueryDto query)
        {
            var memberId = GetCurrentUserId();
            var result = await _orderService.GetOrderListAsync(memberId, query);
            return Success(result);
        }

        /// <summary>
        /// 获取订单详情
        /// </summary>
        /// <param name="id">订单ID</param>
        /// <returns>订单详情</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var memberId = GetCurrentUserId();
            var result = await _orderService.GetOrderDetailAsync(memberId, id);
            return Success(result);
        }

        /// <summary>
        /// 创建订单
        /// </summary>
        /// <param name="dto">创建订单请求</param>
        /// <returns>创建订单结果</returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateOrderDto dto)
        {
            var memberId = GetCurrentUserId();
            var result = await _orderService.CreateOrderAsync(memberId, dto);
            return Success(result);
        }

        /// <summary>
        /// 取消订单
        /// </summary>
        /// <param name="id">订单ID</param>
        /// <returns>是否成功</returns>
        [HttpPost("{id}/cancel")]
        public async Task<IActionResult> Cancel(Guid id)
        {
            var memberId = GetCurrentUserId();
            await _orderService.CancelOrderAsync(memberId, id);
            return Success(true);
        }
    }
}
```

#### AppPaymentController - 支付控制器

**文件路径**：`EasyProduct.Web/Controllers/App/PaymentController.cs`

```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using EasyProduct.Business.App;
using EasyProduct.Models.Dto.App;
using EasyProduct.Web.Controllers.Base;

namespace EasyProduct.Web.Controllers.App
{
    /// <summary>
    /// 小程序支付控制器
    /// </summary>
    [ApiController]
    [Route("api/app/payment")]
    [Authorize(AuthenticationSchemes = "MemberJwt")]
    public class PaymentController : BaseController
    {
        private readonly IAppPaymentService _paymentService;

        public PaymentController(IAppPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        /// <summary>
        /// 创建支付
        /// </summary>
        /// <param name="dto">创建支付请求</param>
        /// <returns>微信支付参数</returns>
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreatePaymentDto dto)
        {
            var memberId = GetCurrentUserId();
            var result = await _paymentService.CreatePaymentAsync(memberId, dto);
            return Success(result);
        }

        /// <summary>
        /// 支付回调
        /// </summary>
        /// <returns>是否成功</returns>
        /// <remarks>
        /// 微信支付成功后会调用此接口
        /// 需要验证签名和处理业务逻辑
        /// </remarks>
        [HttpPost("callback")]
        [AllowAnonymous]
        public async Task<IActionResult> Callback()
        {
            var callbackData = await GetRequestBodyAsStringAsync();
            var result = await _paymentService.HandlePaymentCallbackAsync(callbackData);

            // 返回微信要求的格式
            return Content(result ? "{\"code\":\"SUCCESS\",\"message\":\"成功\"}" : "{\"code\":\"FAIL\",\"message\":\"失败\"}", "application/json");
        }
    }
}
```

#### AppAddressController - 收货地址控制器

**文件路径**：`EasyProduct.Web/Controllers/App/AddressController.cs`

```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using EasyProduct.Business.App;
using EasyProduct.Models.Dto.App;
using EasyProduct.Web.Controllers.Base;

namespace EasyProduct.Web.Controllers.App
{
    /// <summary>
    /// 小程序收货地址控制器
    /// </summary>
    [ApiController]
    [Route("api/app/addresses")]
    [Authorize(AuthenticationSchemes = "MemberJwt")]
    public class AddressController : BaseController
    {
        private readonly IAppAddressService _addressService;

        public AddressController(IAppAddressService addressService)
        {
            _addressService = addressService;
        }

        /// <summary>
        /// 获取地址列表
        /// </summary>
        /// <returns>地址列表</returns>
        [HttpGet("list")]
        public async Task<IActionResult> GetList()
        {
            var memberId = GetCurrentUserId();
            var result = await _addressService.GetAddressListAsync(memberId);
            return Success(result);
        }

        /// <summary>
        /// 获取地址详情
        /// </summary>
        /// <param name="id">地址ID</param>
        /// <returns>地址详情</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var memberId = GetCurrentUserId();
            var result = await _addressService.GetAddressDetailAsync(memberId, id);
            return Success(result);
        }

        /// <summary>
        /// 创建地址
        /// </summary>
        /// <param name="dto">创建地址请求</param>
        /// <returns>地址ID</returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAddressDto dto)
        {
            var memberId = GetCurrentUserId();
            var result = await _addressService.CreateAddressAsync(memberId, dto);
            return Success(result);
        }

        /// <summary>
        /// 更新地址
        /// </summary>
        /// <param name="id">地址ID</param>
        /// <param name="dto">更新地址请求</param>
        /// <returns>是否成功</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAddressDto dto)
        {
            var memberId = GetCurrentUserId();
            await _addressService.UpdateAddressAsync(memberId, id, dto);
            return Success(true);
        }

        /// <summary>
        /// 删除地址
        /// </summary>
        /// <param name="id">地址ID</param>
        /// <returns>是否成功</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var memberId = GetCurrentUserId();
            await _addressService.DeleteAddressAsync(memberId, id);
            return Success(true);
        }
    }
}
```

---

## 7. 数据库表设计

### 7.1 表结构

#### mall_member - 会员表

```sql
CREATE TABLE `mall_member` (
  `id` VARCHAR(36) PRIMARY KEY COMMENT '会员ID（GUID）',
  `nickname` VARCHAR(50) COMMENT '昵称',
  `avatar` VARCHAR(500) COMMENT '头像URL',
  `phone` VARCHAR(20) COMMENT '手机号',
  `openid` VARCHAR(100) UNIQUE COMMENT '微信OpenID',
  `unionid` VARCHAR(100) COMMENT '微信UnionID',
  `level_id` VARCHAR(36) COMMENT '会员等级ID',
  `points` INT DEFAULT 0 COMMENT '当前积分',
  `total_spent` DECIMAL(12,2) DEFAULT 0.00 COMMENT '累计消费金额',
  `order_count` INT DEFAULT 0 COMMENT '订单数量',
  `status` INT DEFAULT 1 COMMENT '状态：1-活跃，0-禁用',
  `create_time` DATETIME DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `update_time` DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
  `create_by` VARCHAR(50) COMMENT '创建人',
  `is_deleted` INT DEFAULT 0 COMMENT '是否删除：0-未删除，1-已删除',
  INDEX `idx_phone` (`phone`),
  INDEX `idx_openid` (`openid`),
  INDEX `idx_level_id` (`level_id`),
  INDEX `idx_status` (`status`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='会员表';
```

#### mall_level - 会员等级表

```sql
CREATE TABLE `mall_level` (
  `id` VARCHAR(36) PRIMARY KEY COMMENT '等级ID（GUID）',
  `name` VARCHAR(50) NOT NULL COMMENT '等级名称',
  `min_points` INT DEFAULT 0 COMMENT '升级所需积分',
  `discount` DECIMAL(3,2) DEFAULT 1.00 COMMENT '折扣率（如0.95表示95折）',
  `sort` INT DEFAULT 0 COMMENT '排序',
  `status` INT DEFAULT 1 COMMENT '状态：1-启用，0-禁用',
  `create_time` DATETIME DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `update_time` DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
  `create_by` VARCHAR(50) COMMENT '创建人',
  `is_deleted` INT DEFAULT 0 COMMENT '是否删除：0-未删除，1-已删除'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='会员等级表';
```

#### mall_points_record - 积分记录表

```sql
CREATE TABLE `mall_points_record` (
  `id` VARCHAR(36) PRIMARY KEY COMMENT '记录ID（GUID）',
  `member_id` VARCHAR(36) NOT NULL COMMENT '会员ID',
  `member_name` VARCHAR(50) COMMENT '会员名称',
  `type` INT NOT NULL COMMENT '类型：1-获得，2-消费',
  `amount` INT NOT NULL COMMENT '积分数量（正数获得，负数消费）',
  `source` INT NOT NULL COMMENT '来源：1-订单，2-签到，3-活动，4-退款，5-调整',
  `description` VARCHAR(200) COMMENT '描述',
  `create_time` DATETIME DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `update_time` DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
  `create_by` VARCHAR(50) COMMENT '创建人',
  `is_deleted` INT DEFAULT 0 COMMENT '是否删除：0-未删除，1-已删除',
  INDEX `idx_member_id` (`member_id`),
  INDEX `idx_type` (`type`),
  INDEX `idx_source` (`source`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='积分记录表';
```

#### mall_cart - 购物车表

```sql
CREATE TABLE `mall_cart` (
  `id` VARCHAR(36) PRIMARY KEY COMMENT '购物车ID（GUID）',
  `member_id` VARCHAR(36) NOT NULL COMMENT '会员ID',
  `spu_id` VARCHAR(36) NOT NULL COMMENT '商品SPU ID',
  `sku_id` VARCHAR(36) NOT NULL COMMENT '商品SKU ID',
  `spu_name` VARCHAR(200) COMMENT '商品名称',
  `spec_values` JSON COMMENT '规格值（JSON格式）',
  `quantity` INT DEFAULT 1 COMMENT '数量',
  `price` DECIMAL(10,2) COMMENT '单价',
  `main_image` VARCHAR(500) COMMENT '商品主图',
  `create_time` DATETIME DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `update_time` DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
  `create_by` VARCHAR(50) COMMENT '创建人',
  `is_deleted` INT DEFAULT 0 COMMENT '是否删除：0-未删除，1-已删除',
  INDEX `idx_member_id` (`member_id`),
  INDEX `idx_spu_id` (`spu_id`),
  INDEX `idx_sku_id` (`sku_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='购物车表';
```

#### mall_order - 订单表

```sql
CREATE TABLE `mall_order` (
  `id` VARCHAR(36) PRIMARY KEY COMMENT '订单ID（GUID）',
  `order_no` VARCHAR(50) NOT NULL UNIQUE COMMENT '订单编号',
  `member_id` VARCHAR(36) NOT NULL COMMENT '会员ID',
  `member_name` VARCHAR(50) COMMENT '会员名称',
  `member_phone` VARCHAR(20) COMMENT '会员手机',
  `total_amount` DECIMAL(12,2) NOT NULL COMMENT '商品总金额',
  `discount_amount` DECIMAL(12,2) DEFAULT 0.00 COMMENT '优惠金额',
  `points_amount` DECIMAL(12,2) DEFAULT 0.00 COMMENT '积分抵扣金额',
  `shipping_fee` DECIMAL(10,2) DEFAULT 0.00 COMMENT '运费',
  `pay_amount` DECIMAL(12,2) NOT NULL COMMENT '实付金额',
  `coupon_id` VARCHAR(36) COMMENT '优惠券ID',
  `coupon_name` VARCHAR(100) COMMENT '优惠券名称',
  `status` INT NOT NULL COMMENT '订单状态：1-待支付，2-已支付，3-已发货，4-已完成，5-已取消，6-已退款',
  `payment_method` VARCHAR(20) COMMENT '支付方式',
  `address_id` VARCHAR(36) COMMENT '收货地址ID',
  `receiver_name` VARCHAR(50) COMMENT '收货人姓名',
  `receiver_phone` VARCHAR(20) COMMENT '收货人电话',
  `receiver_address` VARCHAR(500) COMMENT '收货地址（省市区详细地址）',
  `express_company` VARCHAR(50) COMMENT '物流公司',
  `express_no` VARCHAR(50) COMMENT '物流单号',
  `remark` TEXT COMMENT '备注',
  `paid_at` DATETIME COMMENT '支付时间',
  `shipped_at` DATETIME COMMENT '发货时间',
  `completed_at` DATETIME COMMENT '完成时间',
  `create_time` DATETIME DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `update_time` DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
  `create_by` VARCHAR(50) COMMENT '创建人',
  `is_deleted` INT DEFAULT 0 COMMENT '是否删除：0-未删除，1-已删除',
  INDEX `idx_member_id` (`member_id`),
  INDEX `idx_order_no` (`order_no`),
  INDEX `idx_status` (`status`),
  INDEX `idx_create_time` (`create_time`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='订单表';
```

#### mall_order_item - 订单明细表

```sql
CREATE TABLE `mall_order_item` (
  `id` VARCHAR(36) PRIMARY KEY COMMENT '明细ID（GUID）',
  `order_id` VARCHAR(36) NOT NULL COMMENT '订单ID',
  `spu_id` VARCHAR(36) NOT NULL COMMENT '商品SPU ID',
  `sku_id` VARCHAR(36) NOT NULL COMMENT '商品SKU ID',
  `spu_name` VARCHAR(200) COMMENT '商品名称',
  `spec_values` JSON COMMENT '规格值（JSON格式）',
  `price` DECIMAL(10,2) NOT NULL COMMENT '单价',
  `quantity` INT NOT NULL COMMENT '数量',
  `subtotal` DECIMAL(12,2) NOT NULL COMMENT '小计',
  `main_image` VARCHAR(500) COMMENT '商品主图',
  `create_time` DATETIME DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `update_time` DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
  `create_by` VARCHAR(50) COMMENT '创建人',
  `is_deleted` INT DEFAULT 0 COMMENT '是否删除：0-未删除，1-已删除',
  INDEX `idx_order_id` (`order_id`),
  INDEX `idx_spu_id` (`spu_id`),
  INDEX `idx_sku_id` (`sku_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='订单明细表';
```

#### mall_payment - 支付记录表

```sql
CREATE TABLE `mall_payment` (
  `id` VARCHAR(36) PRIMARY KEY COMMENT '支付ID（GUID）',
  `order_id` VARCHAR(36) NOT NULL COMMENT '订单ID',
  `order_no` VARCHAR(50) COMMENT '订单编号',
  `amount` DECIMAL(12,2) NOT NULL COMMENT '支付金额',
  `method` VARCHAR(20) NOT NULL COMMENT '支付方式：wechat-微信支付',
  `status` INT NOT NULL COMMENT '支付状态：1-待支付，2-成功，3-失败，4-已退款',
  `transaction_id` VARCHAR(100) COMMENT '第三方交易号',
  `prepay_id` VARCHAR(100) COMMENT '预支付交易会话标识',
  `member_id` VARCHAR(36) NOT NULL COMMENT '会员ID',
  `openid` VARCHAR(100) COMMENT '会员OpenID',
  `paid_at` DATETIME COMMENT '支付时间',
  `create_time` DATETIME DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `update_time` DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
  `create_by` VARCHAR(50) COMMENT '创建人',
  `is_deleted` INT DEFAULT 0 COMMENT '是否删除：0-未删除，1-已删除',
  INDEX `idx_order_id` (`order_id`),
  INDEX `idx_status` (`status`),
  INDEX `idx_transaction_id` (`transaction_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='支付记录表';
```

#### mall_address - 收货地址表

```sql
CREATE TABLE `mall_address` (
  `id` VARCHAR(36) PRIMARY KEY COMMENT '地址ID（GUID）',
  `member_id` VARCHAR(36) NOT NULL COMMENT '会员ID',
  `receiver_name` VARCHAR(50) NOT NULL COMMENT '收货人姓名',
  `phone` VARCHAR(20) NOT NULL COMMENT '收货人电话',
  `province` VARCHAR(50) COMMENT '省',
  `city` VARCHAR(50) COMMENT '市',
  `district` VARCHAR(50) COMMENT '区',
  `detail_address` VARCHAR(200) NOT NULL COMMENT '详细地址',
  `is_default` INT DEFAULT 0 COMMENT '是否默认地址：0-否，1-是',
  `create_time` DATETIME DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `update_time` DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
  `create_by` VARCHAR(50) COMMENT '创建人',
  `is_deleted` INT DEFAULT 0 COMMENT '是否删除：0-未删除，1-已删除',
  INDEX `idx_member_id` (`member_id`),
  INDEX `idx_is_default` (`is_default`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='收货地址表';
```

---

## 8. 业务逻辑要点

### 8.1 微信登录流程

```text
┌─────────────┐                  ┌──────────────┐
│   小程序     │                  │   后端API     │
└──────┬──────┘                  └──────┬───────┘
       │                                │
       │ 1. wx.login()                  │
       │   获取 code                    │
       │                                │
       │ 2. POST /api/app/auth/login    │
       │   { code, userInfo? }          │
       ├───────────────────────────────>│
       │                                │
       │                                │ 3. 调用微信 API
       │                                │    code2Session
       │                                │    获取 openid
       │                                │
       │                                │ 4. 查询会员表
       │                                │    openid → member
       │                                │
       │                                │ 5. 不存在则创建新会员
       │                                │    存在则更新登录时间
       │                                │
       │                                │ 6. 生成 JWT Token
       │                                │
       │ 7. 返回 Token + 会员信息        │
       │<───────────────────────────────┤
       │ {                              │
       │   accessToken,                 │
       │   refreshToken,                │
       │   isNewMember,                 │
       │   memberId                     │
       │ }                              │
       │                                │
       │ 8. 存储 Token                  │
       │   设置 Authorization           │
       │                                │
```

**实现要点**：

1. **code2Session 接口**：
   ```csharp
   private async Task<WeChatSessionResult> CallWeChatCode2SessionAsync(string code)
   {
       var url = $"https://api.weixin.qq.com/sns/jscode2session?" +
                 $"appid={_weChatConfig.AppId}&" +
                 $"secret={_weChatConfig.Secret}&" +
                 $"js_code={code}&" +
                 $"grant_type=authorization_code";

       var httpClient = _httpClientFactory.CreateClient();
       var response = await httpClient.GetAsync(url);
       var json = await response.Content.ReadAsStringAsync();

       var result = JsonSerializer.Deserialize<WeChatSessionResult>(json);
       if (result.ErrCode != 0)
       {
           throw BusinessException.BadRequest($"微信登录失败：{result.ErrMsg}");
       }

       return result;
   }
   ```

2. **会员创建/更新**：
   ```csharp
   var member = await _db.Queryable<Member>()
       .FirstAsync(m => m.OpenId == openid);

   if (member == null)
   {
       // 新用户，创建会员
       member = new Member
       {
           OpenId = openid,
           Nickname = userInfo?.NickName,
           Avatar = userInfo?.AvatarUrl,
           Status = MemberStatus.Active
       };
       await _db.Insertable(member).ExecuteCommandAsync();
   }
   else
   {
       // 老用户，更新信息
       if (userInfo != null)
       {
           member.Nickname = userInfo.NickName;
           member.Avatar = userInfo.AvatarUrl;
           await _db.Updateable(member).ExecuteCommandAsync();
       }
   }
   ```

3. **JWT Token 生成**：
   ```csharp
   private string GenerateJwtToken(Member member)
   {
       var claims = new[]
       {
           new Claim("UserId", member.Id.ToString()),
           new Claim("OpenId", member.OpenId ?? ""),
           new Claim("identity_type", "member"),
           new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
       };

       var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.SecretKey));
       var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

       var token = new JwtSecurityToken(
           issuer: _jwtConfig.Issuer,
           audience: _jwtConfig.Audience,
           claims: claims,
           expires: DateTime.UtcNow.AddMinutes(_jwtConfig.AccessTokenExpiresMinutes),
           signingCredentials: credentials
       );

       return new JwtSecurityTokenHandler().WriteToken(token);
   }
   ```

### 8.2 支付对接流程

```text
┌─────────────┐                  ┌──────────────┐                  ┌──────────────┐
│   小程序     │                  │   后端API     │                  │   微信支付     │
└──────┬──────┘                  └──────┬───────┘                  └──────┬───────┘
       │                                │                                │
       │ 1. 选择订单                     │                                │
       │    确认支付                     │                                │
       │                                │                                │
       │ 2. POST /api/app/payment/create│                                │
       │    { orderId }                 │                                │
       ├───────────────────────────────>│                                │
       │                                │                                │
       │                                │ 3. 创建支付记录                  │
       │                                │    状态：pending                 │
       │                                │                                │
       │                                │ 4. 调用微信统一下单接口           │
       │                                ├───────────────────────────────>│
       │                                │                                │
       │                                │ 5. 返回 prepay_id               │
       │                                │<───────────────────────────────┤
       │                                │                                │
       │                                │ 6. 生成小程序支付参数             │
       │                                │    (timeStamp, nonceStr, etc.)  │
       │                                │                                │
       │ 7. 返回支付参数                  │                                │
       │<───────────────────────────────┤                                │
       │ {                              │                                │
       │   timeStamp,                   │                                │
       │   nonceStr,                    │                                │
       │   package,                     │                                │
       │   signType,                    │                                │
       │   paySign                      │                                │
       │ }                              │                                │
       │                                │                                │
       │ 8. wx.requestPayment()         │                                │
       ├───────────────────────────────────────────────────────────────>│
       │                                │                                │
       │                                │ 9. 支付成功                     │
       │                                │    回调通知                      │
       │                                │<───────────────────────────────┤
       │                                │                                │
       │                                │ 10. 验证签名                     │
       │                                │     更新订单状态                  │
       │                                │     更新支付记录                  │
       │                                │     扣减库存                     │
       │                                │                                │
       │ 11. 支付结果                    │                                │
       │<───────────────────────────────┤                                │
       │                                │                                │
```

**实现要点**：

1. **创建支付**：
   ```csharp
   public async Task<CreatePaymentResultDto> CreatePaymentAsync(Guid memberId, CreatePaymentDto dto)
   {
       // 1. 查询订单
       var order = await _db.Queryable<Order>()
           .FirstAsync(o => o.Id == Guid.Parse(dto.OrderId) && o.MemberId == memberId);

       if (order == null)
           throw BusinessException.NotFound("订单不存在");

       if (order.Status != OrderStatus.Pending)
           throw BusinessException.BadRequest("订单状态不允许支付");

       // 2. 获取会员 OpenId
       var member = await _db.Queryable<Member>()
           .FirstAsync(m => m.Id == memberId);

       // 3. 创建支付记录
       var payment = new PaymentRecord
       {
           OrderId = order.Id,
           OrderNo = order.OrderNo,
           Amount = order.PayAmount,
           Method = "wechat",
           Status = PaymentStatus.Pending,
           MemberId = memberId,
           OpenId = member.OpenId
       };
       await _db.Insertable(payment).ExecuteCommandAsync();

       // 4. 调用微信统一下单接口
       var weChatPayResult = await CallWeChatUnifiedOrderAsync(order, payment, member.OpenId);

       // 5. 更新支付记录
       payment.PrepayId = weChatPayResult.PrepayId;
       await _db.Updateable(payment).ExecuteCommandAsync();

       // 6. 生成小程序支付参数
       return GenerateMiniProgramPayParams(weChatPayResult.PrepayId);
   }
   ```

2. **调用微信统一下单接口**：
   ```csharp
   private async Task<WeChatUnifiedOrderResult> CallWeChatUnifiedOrderAsync(Order order, PaymentRecord payment, string openId)
   {
       var requestBody = new
       {
           appid = _weChatConfig.AppId,
           mchid = _weChatPayConfig.MchId,
           description = $"订单：{order.OrderNo}",
           out_trade_no = payment.Id.ToString(),
           notify_url = _weChatPayConfig.NotifyUrl,
           amount = new
           {
               total = (int)(order.PayAmount * 100) // 单位：分
           },
           payer = new
           {
               openid = openId
           }
       };

       var json = JsonSerializer.Serialize(requestBody);
       var client = _httpClientFactory.CreateClient();

       // 添加签名头
       var request = new HttpRequestMessage(HttpMethod.Post, "https://api.mch.weixin.qq.com/v3/pay/transactions/jsapi");
       request.Content = new StringContent(json, Encoding.UTF8, "application/json");
       AddWeChatPaySignature(request, json);

       var response = await client.SendAsync(request);
       var responseJson = await response.Content.ReadAsStringAsync();

       var result = JsonSerializer.Deserialize<WeChatUnifiedOrderResult>(responseJson);
       return result;
   }
   ```

3. **生成小程序支付参数**：
   ```csharp
   private CreatePaymentResultDto GenerateMiniProgramPayParams(string prepayId)
   {
       var timeStamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
       var nonceStr = Guid.NewGuid().ToString("N");
       var package = $"prepay_id={prepayId}";

       // 生成签名
       var signStr = $"{_weChatConfig.AppId}\n{timeStamp}\n{nonceStr}\n{package}\n";
       var paySign = SignWithRSA(signStr);

       return new CreatePaymentResultDto
       {
           TimeStamp = timeStamp,
           NonceStr = nonceStr,
           Package = package,
           SignType = "RSA",
           PaySign = paySign
       };
   }
   ```

4. **处理支付回调**：
   ```csharp
   public async Task<bool> HandlePaymentCallbackAsync(string callbackData)
   {
       // 1. 验证签名
       if (!VerifyWeChatPaySignature(callbackData))
           return false;

       // 2. 解析回调数据
       var callback = JsonSerializer.Deserialize<WeChatPayCallback>(callbackData);
       var paymentId = callback.OutTradeNo;
       var transactionId = callback.TransactionId;

       // 3. 使用事务更新订单和支付记录
       await _db.Ado.UseTranAsync(async () =>
       {
           var payment = await _db.Queryable<PaymentRecord>()
               .FirstAsync(p => p.Id.ToString() == paymentId);

           if (payment.Status == PaymentStatus.Success)
               return; // 幂等处理

           // 更新支付记录
           payment.Status = PaymentStatus.Success;
           payment.TransactionId = transactionId;
           payment.PaidAt = DateTime.Now;
           await _db.Updateable(payment).ExecuteCommandAsync();

           // 更新订单状态
           var order = await _db.Queryable<Order>()
               .FirstAsync(o => o.Id == payment.OrderId);
           order.Status = OrderStatus.Paid;
           order.PaymentMethod = payment.Method;
           order.PaidAt = DateTime.Now;
           await _db.Updateable(order).ExecuteCommandAsync();

           // 更新会员统计
           var member = await _db.Queryable<Member>()
               .FirstAsync(m => m.Id == order.MemberId);
           member.TotalSpent += order.PayAmount;
           member.OrderCount += 1;
           await _db.Updateable(member).ExecuteCommandAsync();

           // 赠送积分
           var points = (int)(order.PayAmount * 1); // 1元1积分
           await AddPointsAsync(member.Id, points, PointsSource.Order, "订单赠送积分");
       });

       return true;
   }
   ```

### 8.3 订单状态机

```text
┌─────────────────────────────────────────────────────────────────┐
│                         订单状态流转                              │
└─────────────────────────────────────────────────────────────────┘

    pending（待支付）
         │
         ├── 支付成功 ────────────────> paid（已支付）
         │                                 │
         │                                 ├── 发货 ──────────────> shipped（已发货）
         │                                 │                           │
         │                                 │                           └── 确认收货 ──> completed（已完成）
         │                                 │
         │                                 └── 退款 ──────────────> refunded（已退款）
         │
         └── 取消订单 ──────────────> cancelled（已取消）

    状态转换规则：
    ┌──────────────┬────────────────────────────────────────┐
    │ 当前状态      │ 允许转换到的状态                         │
    ├──────────────┼────────────────────────────────────────┤
    │ pending      │ paid, cancelled                         │
    │ paid         │ shipped, refunded                       │
    │ shipped      │ completed                               │
    │ completed    │ -                                       │
    │ cancelled    │ -                                       │
    │ refunded     │ -                                       │
    └──────────────┴────────────────────────────────────────┘
```

**实现要点**：

```csharp
/// <summary>
/// 取消订单
/// </summary>
public async Task<bool> CancelOrderAsync(Guid memberId, Guid orderId)
{
    var order = await _db.Queryable<Order>()
        .FirstAsync(o => o.Id == orderId && o.MemberId == memberId);

    if (order == null)
        throw BusinessException.NotFound("订单不存在");

    // 验证订单状态
    if (order.Status != OrderStatus.Pending)
        throw BusinessException.BadRequest("订单状态不允许取消");

    await _db.Ado.UseTranAsync(async () =>
    {
        // 1. 更新订单状态
        order.Status = OrderStatus.Cancelled;
        await _db.Updateable(order).ExecuteCommandAsync();

        // 2. 回滚库存
        var orderItems = await _db.Queryable<OrderItem>()
            .Where(i => i.OrderId == orderId)
            .ToListAsync();
        await RollbackStockAsync(orderItems);

        // 3. 回滚积分
        if (order.PointsAmount > 0)
        {
            var points = (int)(order.PointsAmount * 100); // 1元=100积分
            await AddPointsAsync(memberId, points, PointsSource.Refund, "取消订单退还积分");
        }
    });

    return true;
}

/// <summary>
/// 验证订单状态转换
/// </summary>
private void ValidateOrderStatusTransition(int currentStatus, int targetStatus)
{
    var allowedTransitions = new Dictionary<int, List<int>>
    {
        { OrderStatus.Pending, new List<int> { OrderStatus.Paid, OrderStatus.Cancelled } },
        { OrderStatus.Paid, new List<int> { OrderStatus.Shipped, OrderStatus.Refunded } },
        { OrderStatus.Shipped, new List<int> { OrderStatus.Completed } }
    };

    if (!allowedTransitions.ContainsKey(currentStatus) ||
        !allowedTransitions[currentStatus].Contains(targetStatus))
    {
        throw BusinessException.BadRequest($"订单状态不允许从 {currentStatus} 转换到 {targetStatus}");
    }
}
```

---

## 9. 单元测试要求

### 9.1 测试覆盖范围

**强制测试**（涉钱逻辑，必须先写测试）：

1. **订单创建**：
   - 正常创建订单
   - 库存不足时创建失败
   - 重复提交订单（幂等性）

2. **支付流程**：
   - 正常支付成功
   - 支付回调处理
   - 重复回调处理（幂等性）

3. **库存管理**：
   - 库存扣减
   - 库存回滚
   - 并发库存扣减

### 9.2 测试示例

#### 订单创建测试

**文件路径**：`EasyProduct.Tests/Business/App/OrderServiceTests.cs`

```csharp
using Xunit;
using Moq;
using EasyProduct.Business.App;
using EasyProduct.Models.Dto.App;
using SqlSugar;

namespace EasyProduct.Tests.Business.App
{
    /// <summary>
    /// 订单服务测试
    /// </summary>
    public class OrderServiceTests
    {
        private readonly IAppOrderService _orderService;
        private readonly ISqlSugarClient _db;

        public OrderServiceTests()
        {
            // 初始化测试数据库
            _db = CreateTestDatabase();
            _orderService = new AppOrderService(_db);
        }

        /// <summary>
        /// 测试创建订单_正常场景
        /// </summary>
        [Fact]
        public async Task CreateOrder_Normal_Success()
        {
            // Arrange
            var memberId = Guid.NewGuid();
            var addressId = Guid.NewGuid();
            var cartIds = new List<string> { Guid.NewGuid().ToString() };

            var dto = new CreateOrderDto
            {
                AddressId = addressId.ToString(),
                CartIds = cartIds,
                UsePoints = 0,
                Remark = "测试订单"
            };

            // 准备测试数据
            await PrepareTestDataAsync(memberId, addressId, cartIds);

            // Act
            var result = await _orderService.CreateOrderAsync(memberId, dto);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result.OrderId);
            Assert.NotEmpty(result.OrderNo);
            Assert.True(result.PayAmount > 0);
        }

        /// <summary>
        /// 测试创建订单_库存不足_抛出异常
        /// </summary>
        [Fact]
        public async Task CreateOrder_InsufficientStock_ThrowException()
        {
            // Arrange
            var memberId = Guid.NewGuid();
            var addressId = Guid.NewGuid();
            var cartIds = new List<string> { Guid.NewGuid().ToString() };

            var dto = new CreateOrderDto
            {
                AddressId = addressId.ToString(),
                CartIds = cartIds
            };

            // 准备测试数据（库存为0）
            await PrepareTestDataAsync(memberId, addressId, cartIds, stock: 0);

            // Act & Assert
            await Assert.ThrowsAsync<BusinessException>(
                () => _orderService.CreateOrderAsync(memberId, dto));
        }

        /// <summary>
        /// 测试创建订单_并发提交_幂等性
        /// </summary>
        [Fact]
        public async Task CreateOrder_ConcurrentSubmit_Idempotent()
        {
            // Arrange
            var memberId = Guid.NewGuid();
            var addressId = Guid.NewGuid();
            var cartIds = new List<string> { Guid.NewGuid().ToString() };

            var dto = new CreateOrderDto
            {
                AddressId = addressId.ToString(),
                CartIds = cartIds
            };

            await PrepareTestDataAsync(memberId, addressId, cartIds, stock: 10);

            // Act - 并发提交
            var tasks = Enumerable.Range(0, 5)
                .Select(_ => _orderService.CreateOrderAsync(memberId, dto))
                .ToList();

            var results = await Task.WhenAll(tasks);

            // Assert - 只有一个成功
            var successCount = results.Count(r => r != null);
            Assert.Equal(1, successCount);
        }

        private async Task PrepareTestDataAsync(
            Guid memberId,
            Guid addressId,
            List<string> cartIds,
            int stock = 100)
        {
            // 准备会员数据
            await _db.Insertable(new Member
            {
                Id = memberId,
                OpenId = "test_openid",
                Status = MemberStatus.Active
            }).ExecuteCommandAsync();

            // 准备地址数据
            await _db.Insertable(new Address
            {
                Id = addressId,
                MemberId = memberId,
                ReceiverName = "测试用户",
                Phone = "13800138000",
                DetailAddress = "测试地址"
            }).ExecuteCommandAsync();

            // 准备商品数据
            var spuId = Guid.NewGuid();
            var skuId = Guid.NewGuid();
            await _db.Insertable(new ProductSku
            {
                Id = skuId,
                SpuId = spuId,
                Stock = stock,
                Status = "active"
            }).ExecuteCommandAsync();

            // 准备购物车数据
            var cartId = Guid.Parse(cartIds[0]);
            await _db.Insertable(new Cart
            {
                Id = cartId,
                MemberId = memberId,
                SpuId = spuId,
                SkuId = skuId,
                SpuName = "测试商品",
                Quantity = 1,
                Price = 100
            }).ExecuteCommandAsync();
        }

        private ISqlSugarClient CreateTestDatabase()
        {
            // 创建测试数据库连接
            var db = new SqlSugarClient(new ConnectionConfig
            {
                ConnectionString = "Server=localhost;Database=easyproduct_test;Uid=root;Pwd=test;",
                DbType = DbType.MySql,
                IsAutoCloseConnection = true
            });

            return db;
        }
    }
}
```

### 9.3 测试运行

```bash
# 运行所有测试
dotnet test

# 运行特定测试
dotnet test --filter "FullyQualifiedName~OrderServiceTests"

# 运行测试并生成覆盖率报告
dotnet test --collect:"XPlat Code Coverage"
```

---

## 10. 开发检查清单

### 10.1 代码开发检查

**实体类**：
- [ ] 实体类继承 `BaseEntity`
- [ ] 所有属性添加 XML 注释
- [ ] 金额字段使用 `decimal` 类型
- [ ] 状态字段使用 `int` 类型 + 常量
- [ ] 表名和注释标注正确

**DTO**：
- [ ] DTO 放在 `EasyProduct.Models/Dto/App/` 目录
- [ ] 入参 DTO 添加数据验证特性
- [ ] 分页查询 DTO 继承 `PageQuery`
- [ ] 命名符合规范（XxxDto, XxxQueryDto, XxxCreateDto, XxxUpdateDto）

**Service**：
- [ ] 接口定义在 `EasyProduct.Business/App/`
- [ ] 所有方法添加 XML 注释（summary + remarks）
- [ ] 涉钱操作使用事务
- [ ] 业务规则校验抛 `BusinessException`
- [ ] 不捕获异常，由全局异常中间件处理

**Controller**：
- [ ] 路由前缀正确（`/api/app/<资源>`）
- [ ] 标注 `[Authorize(AuthenticationSchemes = "MemberJwt")]`
- [ ] 所有方法添加 XML 注释
- [ ] 使用 `GetCurrentUserId()` 获取当前会员ID
- [ ] 返回统一信封 `Success(result)`

### 10.2 业务逻辑检查

**微信登录**：
- [ ] 调用微信 API 获取 openid
- [ ] 自动创建/更新会员
- [ ] 生成正确的 JWT Token（identity_type=member）
- [ ] 返回 isNewMember 标识

**购物车**：
- [ ] 加入购物车时验证商品状态和库存
- [ ] 更新数量时验证库存
- [ ] 删除商品时验证会员归属

**订单**：
- [ ] 创建订单前验证库存
- [ ] 创建订单时使用事务（订单+明细+库存扣减+购物车清除）
- [ ] 取消订单时回滚库存
- [ ] 状态流转符合状态机规则

**支付**：
- [ ] 创建支付前验证订单状态
- [ ] 调用微信统一下单接口
- [ ] 生成正确的小程序支付参数
- [ ] 支付回调验证签名
- [ ] 支付回调幂等处理

**收货地址**：
- [ ] 设置默认地址时取消其他默认地址
- [ ] 验证会员归属

### 10.3 单元测试检查

**强制测试**：
- [ ] 订单创建测试（正常 + 库存不足 + 并发幂等）
- [ ] 支付回调测试（正常 + 重复回调）
- [ ] 库存扣减/回滚测试

**测试质量**：
- [ ] 测试方法命名：`<方法>_<场景>_<期望>`
- [ ] 使用 given-when-then 分段注释
- [ ] 测试数据库独立（不使用生产库）
- [ ] 测试数据自建自清

### 10.4 提交前检查

**代码质量**：
- [ ] `dotnet build` 0 错误 0 警告
- [ ] `dotnet test` 全部通过
- [ ] 所有方法添加中文注释
- [ ] 代码格式符合规范

**文档更新**：
- [ ] Swagger 注释完整
- [ ] 接口文档更新（如有变更）
- [ ] 数据库脚本归档到 `sql/`

**Git 提交**：
- [ ] Commit 遵循 Conventional Commits
- [ ] Scope 使用 `api`（后端）
- [ ] 示例：`feat(api): App 模块订单创建接口`

---

## 附录 A：常用工具方法

### A.1 订单编号生成

```csharp
/// <summary>
/// 生成订单编号
/// </summary>
/// <returns>订单编号（格式：yyyyMMddHHmmss + 6位随机数）</returns>
private string GenerateOrderNo()
{
    var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
    var random = new Random().Next(100000, 999999);
    return $"{timestamp}{random}";
}
```

### A.2 积分计算

```csharp
/// <summary>
/// 计算订单赠送积分
/// </summary>
private int CalculateOrderPoints(decimal payAmount)
{
    // 1元赠送1积分
    return (int)Math.Floor(payAmount);
}

/// <summary>
/// 添加积分
/// </summary>
private async Task AddPointsAsync(Guid memberId, int amount, string source, string description)
{
    // 1. 查询会员
    var member = await _db.Queryable<Member>()
        .FirstAsync(m => m.Id == memberId);

    // 2. 更新会员积分
    member.Points += amount;
    await _db.Updateable(member).ExecuteCommandAsync();

    // 3. 记录积分流水
    var record = new PointsRecord
    {
        MemberId = memberId,
        MemberName = member.Nickname,
        Type = amount > 0 ? PointsType.Earn : PointsType.Spend,
        Amount = Math.Abs(amount),
        Source = source,
        Description = description
    };
    await _db.Insertable(record).ExecuteCommandAsync();
}
```

### A.3 库存管理

```csharp
/// <summary>
/// 扣减库存
/// </summary>
private async Task DeductStockAsync(List<CartItemDto> cartItems)
{
    foreach (var item in cartItems)
    {
        var sku = await _db.Queryable<ProductSku>()
            .FirstAsync(s => s.Id == item.SkuId);

        if (sku.Stock < item.Quantity)
            throw BusinessException.BadRequest($"商品 {item.SpuName} 库存不足");

        sku.Stock -= item.Quantity;
        await _db.Updateable(sku).ExecuteCommandAsync();
    }
}

/// <summary>
/// 回滚库存
/// </summary>
private async Task RollbackStockAsync(List<OrderItem> orderItems)
{
    foreach (var item in orderItems)
    {
        var sku = await _db.Queryable<ProductSku>()
            .FirstAsync(s => s.Id == item.SkuId);

        sku.Stock += item.Quantity;
        await _db.Updateable(sku).ExecuteCommandAsync();
    }
}
```

### A.4 价格计算

```csharp
/// <summary>
/// 计算订单价格
/// </summary>
private OrderPriceResult CalculateOrderPrice(List<CartItemDto> cartItems, CreateOrderDto dto)
{
    // 1. 计算商品总价
    var totalAmount = cartItems.Sum(c => c.Price * c.Quantity);

    // 2. 计算优惠金额（优惠券）
    var discountAmount = 0m;
    if (!string.IsNullOrEmpty(dto.CouponId))
    {
        var coupon = _db.Queryable<Coupon>()
            .FirstAsync(c => c.Id == Guid.Parse(dto.CouponId));
        discountAmount = CalculateCouponDiscount(coupon, totalAmount);
    }

    // 3. 计算积分抵扣
    var pointsAmount = 0m;
    if (dto.UsePoints > 0)
    {
        // 100积分抵扣1元
        pointsAmount = dto.UsePoints / 100m;
    }

    // 4. 计算运费
    var shippingFee = CalculateShippingFee(cartItems);

    // 5. 计算实付金额
    var payAmount = totalAmount - discountAmount - pointsAmount + shippingFee;

    return new OrderPriceResult
    {
        TotalAmount = totalAmount,
        DiscountAmount = discountAmount,
        PointsAmount = pointsAmount,
        ShippingFee = shippingFee,
        PayAmount = payAmount
    };
}
```

---

## 附录 B：错误码定义

| 错误码 | 说明 | 示例场景 |
|--------|------|----------|
| 200 | 成功 | 正常返回 |
| 400 | 参数错误 | 必填字段为空、格式不正确 |
| 401 | 未登录 | Token 失效、未携带 Token |
| 403 | 无权限 | 会员 Token 调管理接口 |
| 404 | 资源不存在 | 订单不存在、商品不存在 |
| 500 | 服务器错误 | 未捕获异常 |

**业务异常示例**：

```csharp
// 参数错误
throw BusinessException.BadRequest("手机号格式不正确");
throw BusinessException.BadRequest("商品库存不足");

// 资源不存在
throw BusinessException.NotFound("订单不存在");
throw BusinessException.NotFound("商品不存在");

// 业务规则
throw new BusinessException("订单状态不允许取消", 400);
```

---

## 附录 C：Swagger 注释示例

```csharp
/// <summary>
/// 创建订单
/// </summary>
/// <param name="dto">创建订单请求</param>
/// <returns>创建订单结果</returns>
/// <remarks>
/// 1. 验证购物车商品（库存、状态）
/// 2. 计算价格（商品总价、优惠金额、积分抵扣、运费）
/// 3. 创建订单主表和明细表
/// 4. 扣减库存
/// 5. 清除购物车
/// 6. 返回订单ID和编号
/// </remarks>
/// <response code="200">创建成功</response>
/// <response code="400">参数错误或业务规则不满足</response>
/// <response code="401">未登录</response>
[HttpPost]
[ProducesResponseType(typeof(ApiResponse<CreateOrderResultDto>), 200)]
[ProducesResponseType(typeof(ApiResponse<object>), 400)]
public async Task<IActionResult> Create([FromBody] CreateOrderDto dto)
{
    // ...
}
```

---

**文档版本历史**：

| 版本 | 日期 | 作者 | 说明 |
|------|------|------|------|
| v1.0 | 2026-09-07 | Claude | 初始版本 |

---

> 本文档遵循 EasyProduct 后端开发规范，与 `docs/backend-guidelines.md` 保持一致。