# CRM 模块后端开发指南

> 客户关系管理模块后端开发规范与实施指南

---

## 一、模块概述

### 1.1 功能范围

CRM 模块是 EasyProduct 的核心业务模块，涵盖企业进销存与财务管理全流程：

| 子模块 | 功能说明 | 核心实体 |
|--------|---------|---------|
| 基础数据 | 客户、供应商、币种、税率管理 | Customer, Supplier, Currency, TaxRate |
| 采购管理 | 采购订单、入库、退货 | PurchaseOrder, PurchaseOrderItem |
| 销售管理 | 销售订单、出库、退货 | SalesOrder, SalesOrderItem |
| 库存管理 | 仓库、库存、盘点、预警 | Warehouse, Stock, StockRecord, StockCheck |
| 财务管理 | 发票、收付款、应收应付、固定资产 | Invoice, Payment, Arap, FixedAsset |

### 1.2 业务流程

```
采购流程：供应商 → 采购订单 → 审批 → 入库 → 应付 → 付款
销售流程：客户 → 销售订单 → 审批 → 出库 → 应收 → 收款
库存管理：入库/出库 → 库存流水 → 成本核算 → 盘点/预警
财务核算：发票管理 → 收付款核销 → 应收应付 → 冲销处理
```

---

## 二、技术栈和依赖

### 2.1 核心技术栈

| 技术 | 版本 | 用途 |
|------|------|------|
| .NET | 8.0 LTS | 运行时框架 |
| SqlSugarCore | 5.1.4.x | ORM 框架 |
| Autofac | 8.x/9.x | 依赖注入容器 |
| Mapster | 10.x | 对象映射 |
| Serilog | 8.x | 日志框架 |
| xUnit | 2.x | 单元测试框架 |
| FluentValidation | 11.x | 参数验证 |

### 2.2 NuGet 包引用

```xml
<!-- CRM 模块依赖 -->
<PackageReference Include="SqlSugarCore" Version="5.1.4.*" />
<PackageReference Include="Mapster" Version="10.*" />
<PackageReference Include="FluentValidation" Version="11.*" />
<PackageReference Include="FluentValidation.DependencyInjectionExtensions" Version="11.*" />

<!-- 单元测试 -->
<PackageReference Include="xunit" Version="2.*" />
<PackageReference Include="xunit.runner.visualstudio" Version="2.*" />
<PackageReference Include="Moq" Version="4.*" />
```

### 2.3 模块依赖关系

```
CRM 模块
├── 依赖 Product 模块（商品数据）
├── 依赖 Basic 模块（用户、字典）
└── 依赖 WF 模块（审批流程，可选）
```

---

## 三、实体类设计

### 3.1 设计原则

1. **实体基类**：所有实体类必须继承 `BaseEntity` 基类
2. **主键**：统一使用 `Guid` 类型（自动生成，无需手动赋值）
3. **状态字段**：使用 `int` 类型常量，禁止使用枚举和字符串（便于数据库存储和前端展示）
4. **软删除**：使用 `int` 类型（0-未删除，1-已删除），使用 `DeleteStatus` 常量
5. **审计字段**：由 `BaseEntity` 统一提供（CreatedAt, UpdatedAt, CreatedBy, UpdatedBy）

### 3.2 实体基类

所有 CRM 模块实体继承自 `BaseEntity`：

```csharp
namespace EasyProduct.Models.Entitys.Base;

/// <summary>
/// 实体基类，所有业务实体类继承此类
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// 主键ID（GUID）
    /// </summary>
    [SugarColumn(IsPrimaryKey = true)]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// 是否删除（软删除标记）
    /// 0-未删除，1-已删除
    /// </summary>
    public int IsDeleted { get; set; } = DeleteStatus.NotDeleted;

    /// <summary>
    /// 状态（通用状态字段）
    /// 1-启用/正常，0-禁用/停用
    /// </summary>
    public int Status { get; set; } = CommonStatus.Enabled;

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// 创建人ID
    /// </summary>
    [SugarColumn(ColumnDataType = "varchar(36)", IsNullable = true)]
    public string? CreatedBy { get; set; }

    /// <summary>
    /// 更新人ID
    /// </summary>
    [SugarColumn(ColumnDataType = "varchar(36)", IsNullable = true)]
    public string? UpdatedBy { get; set; }
}
```

### 3.3 基础数据实体

#### Customer - 客户

```csharp
using EasyProduct.Models.Entitys.Base;
using EasyProduct.Models.Constants;

namespace EasyProduct.Crm.Entities;

/// <summary>
/// 客户实体
/// </summary>
[SugarTable("crm_customer")]
public class Customer : BaseEntity
{
    /// <summary>
    /// 客户编码
    /// </summary>
    [SugarColumn(Length = 50)]
    public string Code { get; set; }

    /// <summary>
    /// 客户名称
    /// </summary>
    [SugarColumn(Length = 200)]
    public string Name { get; set; }

    /// <summary>
    /// 客户类型：1-企业客户，2-零售客户
    /// </summary>
    public int Type { get; set; }

    /// <summary>
    /// 客户来源：1-询盘，2-注册，3-手动录入
    /// </summary>
    public int Source { get; set; }

    /// <summary>
    /// 联系人
    /// </summary>
    [SugarColumn(Length = 50)]
    public string ContactPerson { get; set; }

    /// <summary>
    /// 电话
    /// </summary>
    [SugarColumn(Length = 50)]
    public string Phone { get; set; }

    /// <summary>
    /// 邮箱
    /// </summary>
    [SugarColumn(Length = 100)]
    public string Email { get; set; }

    /// <summary>
    /// 地址
    /// </summary>
    [SugarColumn(Length = 500)]
    public string Address { get; set; }

    /// <summary>
    /// 销售员ID
    /// </summary>
    public Guid? SalesPersonId { get; set; }

    /// <summary>
    /// 销售员姓名
    /// </summary>
    [SugarColumn(Length = 50)]
    public string SalesPersonName { get; set; }

    /// <summary>
    /// 信用额度
    /// </summary>
    [SugarColumn(DecimalDigits = 2)]
    public decimal CreditLimit { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    [SugarColumn(ColumnDataType = "text")]
    public string Remark { get; set; }
}
```

#### Supplier - 供应商

```csharp
using EasyProduct.Models.Entitys.Base;

namespace EasyProduct.Crm.Entities;

/// <summary>
/// 供应商实体
/// </summary>
[SugarTable("crm_supplier")]
public class Supplier : BaseEntity
{
    /// <summary>
    /// 供应商编码
    /// </summary>
    [SugarColumn(Length = 50)]
    public string Code { get; set; }

    /// <summary>
    /// 供应商名称
    /// </summary>
    [SugarColumn(Length = 200)]
    public string Name { get; set; }

    /// <summary>
    /// 联系人
    /// </summary>
    [SugarColumn(Length = 50)]
    public string ContactPerson { get; set; }

    /// <summary>
    /// 电话
    /// </summary>
    [SugarColumn(Length = 50)]
    public string Phone { get; set; }

    /// <summary>
    /// 邮箱
    /// </summary>
    [SugarColumn(Length = 100)]
    public string Email { get; set; }

    /// <summary>
    /// 地址
    /// </summary>
    [SugarColumn(Length = 500)]
    public string Address { get; set; }

    /// <summary>
    /// 开户银行
    /// </summary>
    [SugarColumn(Length = 100)]
    public string BankName { get; set; }

    /// <summary>
    /// 银行账号
    /// </summary>
    [SugarColumn(Length = 50)]
    public string BankAccount { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    [SugarColumn(ColumnDataType = "text")]
    public string Remark { get; set; }
}
```

#### Currency - 币种

```csharp
using EasyProduct.Models.Entitys.Base;

namespace EasyProduct.Crm.Entities;

/// <summary>
/// 币种实体
/// </summary>
[SugarTable("crm_currency")]
public class Currency : BaseEntity
{
    /// <summary>
    /// 币种编码（如 CNY, USD）
    /// </summary>
    [SugarColumn(Length = 10)]
    public string Code { get; set; }

    /// <summary>
    /// 币种名称
    /// </summary>
    [SugarColumn(Length = 50)]
    public string Name { get; set; }

    /// <summary>
    /// 符号
    /// </summary>
    [SugarColumn(Length = 10)]
    public string Symbol { get; set; }

    /// <summary>
    /// 汇率（相对于本位币）
    /// </summary>
    [SugarColumn(DecimalDigits = 6)]
    public decimal ExchangeRate { get; set; }

    /// <summary>
    /// 是否本位币
    /// </summary>
    public bool IsDefault { get; set; }
}
```

#### TaxRate - 税率

```csharp
using EasyProduct.Models.Entitys.Base;

namespace EasyProduct.Crm.Entities;

/// <summary>
/// 税率实体
/// </summary>
[SugarTable("crm_tax_rate")]
public class TaxRate : BaseEntity
{
    /// <summary>
    /// 税率编码
    /// </summary>
    [SugarColumn(Length = 50)]
    public string Code { get; set; }

    /// <summary>
    /// 税率名称
    /// </summary>
    [SugarColumn(Length = 100)]
    public string Name { get; set; }

    /// <summary>
    /// 税率百分比（如 13 表示 13%）
    /// </summary>
    [SugarColumn(DecimalDigits = 2)]
    public decimal Rate { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    [SugarColumn(Length = 500)]
    public string Remark { get; set; }
}
```

### 3.4 采购管理实体

#### PurchaseOrder - 采购订单

```csharp
using EasyProduct.Models.Entitys.Base;
using EasyProduct.Models.Constants;

namespace EasyProduct.Crm.Entities;

/// <summary>
/// 采购订单实体
/// </summary>
[SugarTable("crm_purchase_order")]
public class PurchaseOrder : BaseEntity
{
    /// <summary>
    /// 订单编号
    /// </summary>
    [SugarColumn(Length = 50)]
    public string OrderNo { get; set; }

    /// <summary>
    /// 供应商ID
    /// </summary>
    public Guid SupplierId { get; set; }

    /// <summary>
    /// 供应商名称（冗余字段）
    /// </summary>
    [SugarColumn(Length = 200)]
    public string SupplierName { get; set; }

    /// <summary>
    /// 入库仓库ID
    /// </summary>
    public Guid WarehouseId { get; set; }

    /// <summary>
    /// 仓库名称（冗余字段）
    /// </summary>
    [SugarColumn(Length = 100)]
    public string WarehouseName { get; set; }

    /// <summary>
    /// 币种ID
    /// </summary>
    public Guid CurrencyId { get; set; }

    /// <summary>
    /// 币种编码（冗余字段）
    /// </summary>
    [SugarColumn(Length = 10)]
    public string CurrencyCode { get; set; }

    /// <summary>
    /// 税率ID
    /// </summary>
    public Guid? TaxRateId { get; set; }

    /// <summary>
    /// 税率百分比（冗余字段）
    /// </summary>
    [SugarColumn(DecimalDigits = 2)]
    public decimal TaxRate { get; set; }

    /// <summary>
    /// 订单总额（不含税）
    /// </summary>
    [SugarColumn(DecimalDigits = 2)]
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// 税额
    /// </summary>
    [SugarColumn(DecimalDigits = 2)]
    public decimal TaxAmount { get; set; }

    /// <summary>
    /// 价税合计
    /// </summary>
    [SugarColumn(DecimalDigits = 2)]
    public decimal TotalWithTax { get; set; }

    /// <summary>
    /// 订单状态：10-草稿，20-已提交，30-已审批，40-已入库，50-已取消
    /// 使用 CrmPurchaseOrderStatus 常量
    /// </summary>
    public int OrderStatus { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    [SugarColumn(ColumnDataType = "text")]
    public string Remark { get; set; }

    /// <summary>
    /// 订单明细（导航属性，不映射数据库）
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public List<PurchaseOrderItem> Items { get; set; }
}
```

#### PurchaseOrderItem - 采购订单明细

```csharp
using EasyProduct.Models.Entitys.Base;

namespace EasyProduct.Crm.Entities;

/// <summary>
/// 采购订单明细实体
/// </summary>
[SugarTable("crm_purchase_order_item")]
public class PurchaseOrderItem : BaseEntity
{
    /// <summary>
    /// 订单ID
    /// </summary>
    public Guid OrderId { get; set; }

    /// <summary>
    /// 商品ID
    /// </summary>
    public Guid SpuId { get; set; }

    /// <summary>
    /// 商品名称（冗余字段）
    /// </summary>
    [SugarColumn(Length = 200)]
    public string SpuName { get; set; }

    /// <summary>
    /// 规格值
    /// </summary>
    [SugarColumn(Length = 500)]
    public string SpecValues { get; set; }

    /// <summary>
    /// 数量
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// 单价（不含税）
    /// </summary>
    [SugarColumn(DecimalDigits = 2)]
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// 税率百分比
    /// </summary>
    [SugarColumn(DecimalDigits = 2)]
    public decimal TaxRate { get; set; }

    /// <summary>
    /// 税额
    /// </summary>
    [SugarColumn(DecimalDigits = 2)]
    public decimal TaxAmount { get; set; }

    /// <summary>
    /// 金额（不含税）
    /// </summary>
    [SugarColumn(DecimalDigits = 2)]
    public decimal Amount { get; set; }

    /// <summary>
    /// 已入库数量
    /// </summary>
    public int ReceivedQty { get; set; }
}
```

### 3.5 销售管理实体

#### SalesOrder - 销售订单

```csharp
using EasyProduct.Models.Entitys.Base;
using EasyProduct.Models.Constants;

namespace EasyProduct.Crm.Entities;

/// <summary>
/// 销售订单实体
/// </summary>
[SugarTable("crm_sales_order")]
public class SalesOrder : BaseEntity
{
    /// <summary>
    /// 订单编号
    /// </summary>
    [SugarColumn(Length = 50)]
    public string OrderNo { get; set; }

    /// <summary>
    /// 客户ID
    /// </summary>
    public Guid CustomerId { get; set; }

    /// <summary>
    /// 客户名称（冗余字段）
    /// </summary>
    [SugarColumn(Length = 200)]
    public string CustomerName { get; set; }

    /// <summary>
    /// 出库仓库ID
    /// </summary>
    public Guid WarehouseId { get; set; }

    /// <summary>
    /// 仓库名称（冗余字段）
    /// </summary>
    [SugarColumn(Length = 100)]
    public string WarehouseName { get; set; }

    /// <summary>
    /// 币种ID
    /// </summary>
    public Guid CurrencyId { get; set; }

    /// <summary>
    /// 币种编码（冗余字段）
    /// </summary>
    [SugarColumn(Length = 10)]
    public string CurrencyCode { get; set; }

    /// <summary>
    /// 税率ID
    /// </summary>
    public Guid? TaxRateId { get; set; }

    /// <summary>
    /// 税率百分比（冗余字段）
    /// </summary>
    [SugarColumn(DecimalDigits = 2)]
    public decimal TaxRate { get; set; }

    /// <summary>
    /// 订单总额（不含税）
    /// </summary>
    [SugarColumn(DecimalDigits = 2)]
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// 税额
    /// </summary>
    [SugarColumn(DecimalDigits = 2)]
    public decimal TaxAmount { get; set; }

    /// <summary>
    /// 折扣金额
    /// </summary>
    [SugarColumn(DecimalDigits = 2)]
    public decimal DiscountAmount { get; set; }

    /// <summary>
    /// 应收金额（价税合计 - 折扣）
    /// </summary>
    [SugarColumn(DecimalDigits = 2)]
    public decimal PayAmount { get; set; }

    /// <summary>
    /// 订单状态：10-草稿，20-已提交，30-已审批，40-已发货，50-已完成，60-已取消
    /// 使用 CrmSalesOrderStatus 常量
    /// </summary>
    public int OrderStatus { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    [SugarColumn(ColumnDataType = "text")]
    public string Remark { get; set; }

    /// <summary>
    /// 订单明细（导航属性，不映射数据库）
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public List<SalesOrderItem> Items { get; set; }
}
```

#### SalesOrderItem - 销售订单明细

```csharp
using EasyProduct.Models.Entitys.Base;

namespace EasyProduct.Crm.Entities;

/// <summary>
/// 销售订单明细实体
/// </summary>
[SugarTable("crm_sales_order_item")]
public class SalesOrderItem : BaseEntity
{
    /// <summary>
    /// 订单ID
    /// </summary>
    public Guid OrderId { get; set; }

    /// <summary>
    /// 商品ID
    /// </summary>
    public Guid SpuId { get; set; }

    /// <summary>
    /// 商品名称（冗余字段）
    /// </summary>
    [SugarColumn(Length = 200)]
    public string SpuName { get; set; }

    /// <summary>
    /// 规格值
    /// </summary>
    [SugarColumn(Length = 500)]
    public string SpecValues { get; set; }

    /// <summary>
    /// 数量
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// 单价（不含税）
    /// </summary>
    [SugarColumn(DecimalDigits = 2)]
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// 税率百分比
    /// </summary>
    [SugarColumn(DecimalDigits = 2)]
    public decimal TaxRate { get; set; }

    /// <summary>
    /// 税额
    /// </summary>
    [SugarColumn(DecimalDigits = 2)]
    public decimal TaxAmount { get; set; }

    /// <summary>
    /// 折扣金额
    /// </summary>
    [SugarColumn(DecimalDigits = 2)]
    public decimal DiscountAmount { get; set; }

    /// <summary>
    /// 金额（不含税）
    /// </summary>
    [SugarColumn(DecimalDigits = 2)]
    public decimal Amount { get; set; }

    /// <summary>
    /// 已出库数量
    /// </summary>
    public int ShippedQty { get; set; }
}
```

### 3.6 库存管理实体

#### Warehouse - 仓库

```csharp
using EasyProduct.Models.Entitys.Base;

namespace EasyProduct.Crm.Entities;

/// <summary>
/// 仓库实体
/// </summary>
[SugarTable("crm_warehouse")]
public class Warehouse : BaseEntity
{
    /// <summary>
    /// 仓库编码
    /// </summary>
    [SugarColumn(Length = 50)]
    public string Code { get; set; }

    /// <summary>
    /// 仓库名称
    /// </summary>
    [SugarColumn(Length = 100)]
    public string Name { get; set; }

    /// <summary>
    /// 类型：1-普通仓库，2-虚拟仓库，3-残次品仓库
    /// </summary>
    public int Type { get; set; }

    /// <summary>
    /// 地址
    /// </summary>
    [SugarColumn(Length = 500)]
    public string Address { get; set; }

    /// <summary>
    /// 负责人姓名
    /// </summary>
    [SugarColumn(Length = 50)]
    public string ManagerName { get; set; }

    /// <summary>
    /// 电话
    /// </summary>
    [SugarColumn(Length = 50)]
    public string Phone { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    [SugarColumn(ColumnDataType = "text")]
    public string Remark { get; set; }
}
```

#### Stock - 库存

```csharp
using EasyProduct.Models.Entitys.Base;

namespace EasyProduct.Crm.Entities;

/// <summary>
/// 库存实体
/// </summary>
[SugarTable("crm_stock")]
public class Stock : BaseEntity
{
    /// <summary>
    /// 仓库ID
    /// </summary>
    public Guid WarehouseId { get; set; }

    /// <summary>
    /// 仓库名称（冗余字段）
    /// </summary>
    [SugarColumn(Length = 100)]
    public string WarehouseName { get; set; }

    /// <summary>
    /// 商品ID
    /// </summary>
    public Guid SpuId { get; set; }

    /// <summary>
    /// 商品名称（冗余字段）
    /// </summary>
    [SugarColumn(Length = 200)]
    public string SpuName { get; set; }

    /// <summary>
    /// 规格值
    /// </summary>
    [SugarColumn(Length = 500)]
    public string SpecValues { get; set; }

    /// <summary>
    /// 库存数量
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// 锁定数量
    /// </summary>
    public int LockedQty { get; set; }

    /// <summary>
    /// 可用数量
    /// </summary>
    public int AvailableQty { get; set; }

    /// <summary>
    /// 成本价（加权平均）
    /// </summary>
    [SugarColumn(DecimalDigits = 2)]
    public decimal CostPrice { get; set; }
}
```

#### StockRecord - 库存流水

```csharp
using EasyProduct.Models.Entitys.Base;
using EasyProduct.Models.Constants;

namespace EasyProduct.Crm.Entities;

/// <summary>
/// 库存流水实体
/// </summary>
[SugarTable("crm_stock_record")]
public class StockRecord : BaseEntity
{
    /// <summary>
    /// 仓库ID
    /// </summary>
    public Guid WarehouseId { get; set; }

    /// <summary>
    /// 商品ID
    /// </summary>
    public Guid SpuId { get; set; }

    /// <summary>
    /// 规格值
    /// </summary>
    [SugarColumn(Length = 500)]
    public string SpecValues { get; set; }

    /// <summary>
    /// 类型：1-入库，2-出库，3-盘点，4-调整
    /// 使用 CrmStockRecordType 常量
    /// </summary>
    public int RecordType { get; set; }

    /// <summary>
    /// 业务类型：1-采购，2-销售，3-退货，4-调拨，5-盘点，6-调整
    /// 使用 CrmStockBizType 常量
    /// </summary>
    public int BizType { get; set; }

    /// <summary>
    /// 业务单据ID
    /// </summary>
    public Guid? BizId { get; set; }

    /// <summary>
    /// 业务单据编号
    /// </summary>
    [SugarColumn(Length = 50)]
    public string BizNo { get; set; }

    /// <summary>
    /// 数量（正数入库，负数出库）
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// 变动前数量
    /// </summary>
    public int BeforeQty { get; set; }

    /// <summary>
    /// 变动后数量
    /// </summary>
    public int AfterQty { get; set; }

    /// <summary>
    /// 成本价
    /// </summary>
    [SugarColumn(DecimalDigits = 2)]
    public decimal CostPrice { get; set; }

    /// <summary>
    /// 金额
    /// </summary>
    [SugarColumn(DecimalDigits = 2)]
    public decimal Amount { get; set; }
}
```

### 3.7 财务管理实体

#### Invoice - 发票

```csharp
using EasyProduct.Models.Entitys.Base;
using EasyProduct.Models.Constants;

namespace EasyProduct.Crm.Entities;

/// <summary>
/// 发票实体
/// </summary>
[SugarTable("crm_invoice")]
public class Invoice : BaseEntity
{
    /// <summary>
    /// 发票号码
    /// </summary>
    [SugarColumn(Length = 50)]
    public string InvoiceNo { get; set; }

    /// <summary>
    /// 类型：1-销售发票，2-采购发票
    /// 使用 CrmInvoiceType 常量
    /// </summary>
    public int InvoiceType { get; set; }

    /// <summary>
    /// 业务单据ID
    /// </summary>
    public Guid? BizId { get; set; }

    /// <summary>
    /// 业务单据编号
    /// </summary>
    [SugarColumn(Length = 50)]
    public string BizNo { get; set; }

    /// <summary>
    /// 客户ID（销售发票）
    /// </summary>
    public Guid? CustomerId { get; set; }

    /// <summary>
    /// 供应商ID（采购发票）
    /// </summary>
    public Guid? SupplierId { get; set; }

    /// <summary>
    /// 开票金额（不含税）
    /// </summary>
    [SugarColumn(DecimalDigits = 2)]
    public decimal Amount { get; set; }

    /// <summary>
    /// 税额
    /// </summary>
    [SugarColumn(DecimalDigits = 2)]
    public decimal TaxAmount { get; set; }

    /// <summary>
    /// 价税合计
    /// </summary>
    [SugarColumn(DecimalDigits = 2)]
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// 发票状态：10-草稿，20-已开具，30-已作废
    /// 使用 CrmInvoiceStatus 常量
    /// </summary>
    public int InvoiceStatus { get; set; }

    /// <summary>
    /// 开票日期
    /// </summary>
    public DateTime? IssuedAt { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    [SugarColumn(ColumnDataType = "text")]
    public string Remark { get; set; }
}
```

#### Payment - 收付款

```csharp
using EasyProduct.Models.Entitys.Base;
using EasyProduct.Models.Constants;

namespace EasyProduct.Crm.Entities;

/// <summary>
/// 收付款实体
/// </summary>
[SugarTable("crm_payment")]
public class Payment : BaseEntity
{
    /// <summary>
    /// 收付款单号
    /// </summary>
    [SugarColumn(Length = 50)]
    public string PaymentNo { get; set; }

    /// <summary>
    /// 类型：1-收款，2-付款
    /// 使用 CrmPaymentType 常量
    /// </summary>
    public int PaymentType { get; set; }

    /// <summary>
    /// 业务单据ID
    /// </summary>
    public Guid? BizId { get; set; }

    /// <summary>
    /// 业务单据编号
    /// </summary>
    [SugarColumn(Length = 50)]
    public string BizNo { get; set; }

    /// <summary>
    /// 客户ID（收款）
    /// </summary>
    public Guid? CustomerId { get; set; }

    /// <summary>
    /// 供应商ID（付款）
    /// </summary>
    public Guid? SupplierId { get; set; }

    /// <summary>
    /// 金额
    /// </summary>
    [SugarColumn(DecimalDigits = 2)]
    public decimal Amount { get; set; }

    /// <summary>
    /// 银行账户ID
    /// </summary>
    public Guid? BankAccountId { get; set; }

    /// <summary>
    /// 银行账户名称（冗余字段）
    /// </summary>
    [SugarColumn(Length = 100)]
    public string BankAccountName { get; set; }

    /// <summary>
    /// 支付方式
    /// </summary>
    public int PaymentMethod { get; set; }

    /// <summary>
    /// 支付状态：10-草稿，20-已完成，30-已取消
    /// 使用 CrmPaymentStatus 常量
    /// </summary>
    public int PaymentStatus { get; set; }

    /// <summary>
    /// 支付日期
    /// </summary>
    public DateTime? PaidAt { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    [SugarColumn(ColumnDataType = "text")]
    public string Remark { get; set; }
}
```

#### Arap - 应收应付

```csharp
using EasyProduct.Models.Entitys.Base;
using EasyProduct.Models.Constants;

namespace EasyProduct.Crm.Entities;

/// <summary>
/// 应收应付实体
/// </summary>
[SugarTable("crm_arap")]
public class Arap : BaseEntity
{
    /// <summary>
    /// 类型：1-应收，2-应付
    /// 使用 CrmArapType 常量
    /// </summary>
    public int ArapType { get; set; }

    /// <summary>
    /// 业务单据ID
    /// </summary>
    public Guid? BizId { get; set; }

    /// <summary>
    /// 业务单据编号
    /// </summary>
    [SugarColumn(Length = 50)]
    public string BizNo { get; set; }

    /// <summary>
    /// 业务类型：1-销售，2-采购，3-预收预付，4-其他
    /// 使用 CrmArapBizType 常量
    /// </summary>
    public int BizType { get; set; }

    /// <summary>
    /// 客户ID（应收）
    /// </summary>
    public Guid? CustomerId { get; set; }

    /// <summary>
    /// 供应商ID（应付）
    /// </summary>
    public Guid? SupplierId { get; set; }

    /// <summary>
    /// 总金额
    /// </summary>
    [SugarColumn(DecimalDigits = 2)]
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// 已结算金额
    /// </summary>
    [SugarColumn(DecimalDigits = 2)]
    public decimal PaidAmount { get; set; }

    /// <summary>
    /// 未结算金额
    /// </summary>
    [SugarColumn(DecimalDigits = 2)]
    public decimal UnpaidAmount { get; set; }

    /// <summary>
    /// 状态：10-未结算，20-部分结算，30-已结算
    /// 使用 CrmArapStatus 常量
    /// </summary>
    public int ArapStatus { get; set; }

    /// <summary>
    /// 到期日
    /// </summary>
    public DateTime? DueDate { get; set; }
}
```

---

## 四、DTO 设计

### 4.1 DTO 设计原则

1. **Request DTO**：用于接收前端请求参数
2. **Response DTO**：用于返回数据给前端
3. **Query DTO**：用于查询条件封装
4. **命名规范**：
   - Request DTO：`XxxCreateDto`, `XxxUpdateDto`, `XxxQueryDto`
   - Response DTO：`XxxDto`, `XxxListDto`, `XxxDetailDto`

### 4.2 客户 DTO

```csharp
namespace EasyProduct.Crm.Dtos;

/// <summary>
/// 客户创建请求DTO
/// </summary>
public class CustomerCreateDto
{
    /// <summary>
    /// 客户名称
    /// </summary>
    [Required(ErrorMessage = "客户名称不能为空")]
    [MaxLength(200)]
    public string Name { get; set; }

    /// <summary>
    /// 客户类型：1-企业客户，2-零售客户
    /// </summary>
    [Required]
    public int Type { get; set; }

    /// <summary>
    /// 客户来源：1-询盘，2-注册，3-手动录入
    /// </summary>
    public int? Source { get; set; }

    /// <summary>
    /// 联系人
    /// </summary>
    [MaxLength(50)]
    public string ContactPerson { get; set; }

    /// <summary>
    /// 电话
    /// </summary>
    [MaxLength(50)]
    public string Phone { get; set; }

    /// <summary>
    /// 邮箱
    /// </summary>
    [MaxLength(100)]
    [EmailAddress]
    public string Email { get; set; }

    /// <summary>
    /// 地址
    /// </summary>
    [MaxLength(500)]
    public string Address { get; set; }

    /// <summary>
    /// 销售员ID（GUID字符串）
    /// </summary>
    public string SalesPersonId { get; set; }

    /// <summary>
    /// 信用额度
    /// </summary>
    public decimal? CreditLimit { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    public string Remark { get; set; }
}

/// <summary>
/// 客户更新请求DTO
/// </summary>
public class CustomerUpdateDto
{
    /// <summary>
    /// 客户名称
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string Name { get; set; }

    /// <summary>
    /// 联系人
    /// </summary>
    [MaxLength(50)]
    public string ContactPerson { get; set; }

    /// <summary>
    /// 电话
    /// </summary>
    [MaxLength(50)]
    public string Phone { get; set; }

    /// <summary>
    /// 邮箱
    /// </summary>
    [MaxLength(100)]
    [EmailAddress]
    public string Email { get; set; }

    /// <summary>
    /// 地址
    /// </summary>
    [MaxLength(500)]
    public string Address { get; set; }

    /// <summary>
    /// 销售员ID（GUID字符串）
    /// </summary>
    public string SalesPersonId { get; set; }

    /// <summary>
    /// 信用额度
    /// </summary>
    public decimal? CreditLimit { get; set; }

    /// <summary>
    /// 状态：1-启用，0-停用
    /// </summary>
    public int? Status { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    public string Remark { get; set; }
}

/// <summary>
/// 客户查询DTO
/// </summary>
public class CustomerQueryDto
{
    /// <summary>
    /// 页码（从1开始）
    /// </summary>
    public int PageIndex { get; set; } = 1;

    /// <summary>
    /// 每页数量
    /// </summary>
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// 关键词（名称、编码、联系人）
    /// </summary>
    public string Keyword { get; set; }

    /// <summary>
    /// 客户类型
    /// </summary>
    public int? Type { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    public int? Status { get; set; }

    /// <summary>
    /// 销售员ID（GUID字符串）
    /// </summary>
    public string SalesPersonId { get; set; }
}

/// <summary>
/// 客户响应DTO
/// </summary>
public class CustomerDto
{
    /// <summary>
    /// 主键ID（GUID字符串）
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// 客户编码
    /// </summary>
    public string Code { get; set; }

    /// <summary>
    /// 客户名称
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 客户类型
    /// </summary>
    public int Type { get; set; }

    /// <summary>
    /// 客户来源
    /// </summary>
    public int Source { get; set; }

    /// <summary>
    /// 联系人
    /// </summary>
    public string ContactPerson { get; set; }

    /// <summary>
    /// 电话
    /// </summary>
    public string Phone { get; set; }

    /// <summary>
    /// 邮箱
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// 地址
    /// </summary>
    public string Address { get; set; }

    /// <summary>
    /// 销售员ID
    /// </summary>
    public string SalesPersonId { get; set; }

    /// <summary>
    /// 销售员姓名
    /// </summary>
    public string SalesPersonName { get; set; }

    /// <summary>
    /// 信用额度
    /// </summary>
    public decimal CreditLimit { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    public string Remark { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}
```

### 4.3 采购订单 DTO

```csharp
namespace EasyProduct.Crm.Dtos;

/// <summary>
/// 采购订单创建请求DTO
/// </summary>
public class PurchaseOrderCreateDto
{
    /// <summary>
    /// 供应商ID（GUID字符串）
    /// </summary>
    [Required]
    public string SupplierId { get; set; }

    /// <summary>
    /// 入库仓库ID（GUID字符串）
    /// </summary>
    [Required]
    public string WarehouseId { get; set; }

    /// <summary>
    /// 币种ID（GUID字符串）
    /// </summary>
    [Required]
    public string CurrencyId { get; set; }

    /// <summary>
    /// 税率ID（GUID字符串）
    /// </summary>
    public string TaxRateId { get; set; }

    /// <summary>
    /// 订单明细
    /// </summary>
    [Required]
    [MinLength(1, ErrorMessage = "订单明细不能为空")]
    public List<PurchaseOrderItemCreateDto> Items { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    public string Remark { get; set; }
}

/// <summary>
/// 采购订单明细创建DTO
/// </summary>
public class PurchaseOrderItemCreateDto
{
    /// <summary>
    /// 商品ID（GUID字符串）
    /// </summary>
    [Required]
    public string SpuId { get; set; }

    /// <summary>
    /// 数量
    /// </summary>
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "数量必须大于0")]
    public int Quantity { get; set; }

    /// <summary>
    /// 单价（不含税）
    /// </summary>
    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "单价必须大于0")]
    public decimal UnitPrice { get; set; }
}

/// <summary>
/// 采购订单查询DTO
/// </summary>
public class PurchaseOrderQueryDto
{
    /// <summary>
    /// 页码（从1开始）
    /// </summary>
    public int PageIndex { get; set; } = 1;

    /// <summary>
    /// 每页数量
    /// </summary>
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// 订单编号
    /// </summary>
    public string OrderNo { get; set; }

    /// <summary>
    /// 供应商ID（GUID字符串）
    /// </summary>
    public string SupplierId { get; set; }

    /// <summary>
    /// 订单状态
    /// </summary>
    public int? Status { get; set; }

    /// <summary>
    /// 开始日期
    /// </summary>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// 结束日期
    /// </summary>
    public DateTime? EndDate { get; set; }
}

/// <summary>
/// 采购订单响应DTO
/// </summary>
public class PurchaseOrderDto
{
    /// <summary>
    /// 主键ID（GUID字符串）
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// 订单编号
    /// </summary>
    public string OrderNo { get; set; }

    /// <summary>
    /// 供应商ID
    /// </summary>
    public string SupplierId { get; set; }

    /// <summary>
    /// 供应商名称
    /// </summary>
    public string SupplierName { get; set; }

    /// <summary>
    /// 仓库ID
    /// </summary>
    public string WarehouseId { get; set; }

    /// <summary>
    /// 仓库名称
    /// </summary>
    public string WarehouseName { get; set; }

    /// <summary>
    /// 币种编码
    /// </summary>
    public string CurrencyCode { get; set; }

    /// <summary>
    /// 税率
    /// </summary>
    public decimal TaxRate { get; set; }

    /// <summary>
    /// 订单总额
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// 税额
    /// </summary>
    public decimal TaxAmount { get; set; }

    /// <summary>
    /// 价税合计
    /// </summary>
    public decimal TotalWithTax { get; set; }

    /// <summary>
    /// 订单状态
    /// </summary>
    public int OrderStatus { get; set; }

    /// <summary>
    /// 订单明细
    /// </summary>
    public List<PurchaseOrderItemDto> Items { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    public string Remark { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// 采购订单明细响应DTO
/// </summary>
public class PurchaseOrderItemDto
{
    /// <summary>
    /// 主键ID（GUID字符串）
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// 商品ID
    /// </summary>
    public string SpuId { get; set; }

    /// <summary>
    /// 商品名称
    /// </summary>
    public string SpuName { get; set; }

    /// <summary>
    /// 规格值
    /// </summary>
    public string SpecValues { get; set; }

    /// <summary>
    /// 数量
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// 单价
    /// </summary>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// 税率
    /// </summary>
    public decimal TaxRate { get; set; }

    /// <summary>
    /// 税额
    /// </summary>
    public decimal TaxAmount { get; set; }

    /// <summary>
    /// 金额
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// 已入库数量
    /// </summary>
    public int ReceivedQty { get; set; }
}
```

---

## 五、Service 层实现指南

### 5.1 Service 层规范

1. **命名规范**：接口 `IXxxService`，实现类 `XxxService`
2. **依赖注入**：构造器注入，禁止属性注入
3. **事务处理**：涉钱逻辑必须使用事务
4. **异常处理**：抛出业务异常，由全局异常中间件统一处理
5. **注释规范**：所有公开方法必须添加中文注释

### 5.2 客户服务实现示例

```csharp
using EasyProduct.Models.Constants;

namespace EasyProduct.Crm.Services;

/// <summary>
/// 客户服务接口
/// </summary>
public interface ICustomerService
{
    /// <summary>
    /// 获取客户列表
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>客户分页列表</returns>
    Task<PageResult<CustomerDto>> GetListAsync(CustomerQueryDto query);

    /// <summary>
    /// 获取客户详情
    /// </summary>
    /// <param name="id">客户ID</param>
    /// <returns>客户详情</returns>
    Task<CustomerDto> GetByIdAsync(Guid id);

    /// <summary>
    /// 创建客户
    /// </summary>
    /// <param name="dto">创建参数</param>
    /// <returns>客户ID</returns>
    Task<Guid> CreateAsync(CustomerCreateDto dto);

    /// <summary>
    /// 更新客户
    /// </summary>
    /// <param name="id">客户ID</param>
    /// <param name="dto">更新参数</param>
    Task UpdateAsync(Guid id, CustomerUpdateDto dto);

    /// <summary>
    /// 删除客户
    /// </summary>
    /// <param name="id">客户ID</param>
    Task DeleteAsync(Guid id);
}

/// <summary>
/// 客户服务实现
/// </summary>
public class CustomerService : ICustomerService
{
    private readonly ISqlSugarClient _db;
    private readonly ICurrentUser _currentUser;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="db">数据库上下文</param>
    /// <param name="currentUser">当前用户</param>
    public CustomerService(ISqlSugarClient db, ICurrentUser currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    /// <summary>
    /// 获取客户列表（支持分页、筛选）
    /// </summary>
    /// <param name="query">查询参数，包含分页、类型、状态等</param>
    /// <returns>客户列表分页结果</returns>
    /// <remarks>
    /// 1. 支持按类型、状态、销售员筛选
    /// 2. 支持关键词模糊搜索（名称、编码、联系人）
    /// 3. 默认按创建时间倒序排列
    /// </remarks>
    public async Task<PageResult<CustomerDto>> GetListAsync(CustomerQueryDto query)
    {
        var queryable = _db.Queryable<Customer>()
            .Where(c => c.IsDeleted == DeleteStatus.NotDeleted);

        // 关键词搜索
        if (!string.IsNullOrWhiteSpace(query.Keyword))
        {
            queryable = queryable.Where(c =>
                c.Name.Contains(query.Keyword) ||
                c.Code.Contains(query.Keyword) ||
                c.ContactPerson.Contains(query.Keyword));
        }

        // 类型筛选
        if (query.Type.HasValue)
        {
            queryable = queryable.Where(c => c.Type == query.Type.Value);
        }

        // 状态筛选
        if (query.Status.HasValue)
        {
            queryable = queryable.Where(c => c.Status == query.Status.Value);
        }

        // 销售员筛选
        if (!string.IsNullOrWhiteSpace(query.SalesPersonId))
        {
            var salesPersonId = Guid.Parse(query.SalesPersonId);
            queryable = queryable.Where(c => c.SalesPersonId == salesPersonId);
        }

        // 分页查询
        var total = 0;
        var list = await queryable
            .OrderByDescending(c => c.CreatedAt)
            .ToPageListAsync(query.PageIndex, query.PageSize, total);

        // 映射为 DTO
        var dtoList = list.Adapt<List<CustomerDto>>();

        return new PageResult<CustomerDto>
        {
            List = dtoList,
            Total = total
        };
    }

    /// <summary>
    /// 获取客户详情
    /// </summary>
    /// <param name="id">客户ID</param>
    /// <returns>客户详情</returns>
    /// <exception cref="BusinessException">客户不存在时抛出异常</exception>
    public async Task<CustomerDto> GetByIdAsync(Guid id)
    {
        var entity = await _db.Queryable<Customer>()
            .Where(c => c.Id == id && c.IsDeleted == DeleteStatus.NotDeleted)
            .FirstAsync();

        if (entity == null)
        {
            throw new BusinessException("客户不存在");
        }

        return entity.Adapt<CustomerDto>();
    }

    /// <summary>
    /// 创建客户
    /// </summary>
    /// <param name="dto">创建参数</param>
    /// <returns>客户ID</returns>
    /// <remarks>
    /// 1. 自动生成客户编码（格式：CU + 时间戳 + 随机数）
    /// 2. 默认状态为启用
    /// 3. 记录创建人和创建时间
    /// </remarks>
    public async Task<Guid> CreateAsync(CustomerCreateDto dto)
    {
        // 生成客户编码
        var code = await GenerateCustomerCodeAsync();

        var entity = new Customer
        {
            Code = code,
            Name = dto.Name,
            Type = dto.Type,
            Source = dto.Source ?? CrmCustomerSource.Manual,
            ContactPerson = dto.ContactPerson,
            Phone = dto.Phone,
            Email = dto.Email,
            Address = dto.Address,
            CreditLimit = dto.CreditLimit ?? 0,
            Status = CommonStatus.Enabled,
            Remark = dto.Remark,
            CreatedBy = _currentUser.UserId
        };

        // 处理销售员ID
        if (!string.IsNullOrWhiteSpace(dto.SalesPersonId))
        {
            entity.SalesPersonId = Guid.Parse(dto.SalesPersonId);
        }

        await _db.Insertable(entity).ExecuteCommandAsync();

        return entity.Id;
    }

    /// <summary>
    /// 更新客户
    /// </summary>
    /// <param name="id">客户ID</param>
    /// <param name="dto">更新参数</param>
    /// <exception cref="BusinessException">客户不存在时抛出异常</exception>
    public async Task UpdateAsync(Guid id, CustomerUpdateDto dto)
    {
        var entity = await _db.Queryable<Customer>()
            .Where(c => c.Id == id && c.IsDeleted == DeleteStatus.NotDeleted)
            .FirstAsync();

        if (entity == null)
        {
            throw new BusinessException("客户不存在");
        }

        // 更新字段
        entity.Name = dto.Name;
        entity.ContactPerson = dto.ContactPerson;
        entity.Phone = dto.Phone;
        entity.Email = dto.Email;
        entity.Address = dto.Address;
        entity.CreditLimit = dto.CreditLimit ?? entity.CreditLimit;
        entity.Status = dto.Status ?? entity.Status;
        entity.Remark = dto.Remark;
        entity.UpdatedAt = DateTime.UtcNow;
        entity.UpdatedBy = _currentUser.UserId;

        // 处理销售员ID
        if (!string.IsNullOrWhiteSpace(dto.SalesPersonId))
        {
            entity.SalesPersonId = Guid.Parse(dto.SalesPersonId);
        }

        await _db.Updateable(entity).ExecuteCommandAsync();
    }

    /// <summary>
    /// 删除客户（软删除）
    /// </summary>
    /// <param name="id">客户ID</param>
    /// <exception cref="BusinessException">客户不存在或有关联订单时抛出异常</exception>
    public async Task DeleteAsync(Guid id)
    {
        var entity = await _db.Queryable<Customer>()
            .Where(c => c.Id == id && c.IsDeleted == DeleteStatus.NotDeleted)
            .FirstAsync();

        if (entity == null)
        {
            throw new BusinessException("客户不存在");
        }

        // 检查是否有关联订单
        var hasOrders = await _db.Queryable<SalesOrder>()
            .Where(o => o.CustomerId == id)
            .AnyAsync();

        if (hasOrders)
        {
            throw new BusinessException("客户存在关联订单，无法删除");
        }

        // 软删除
        entity.IsDeleted = DeleteStatus.Deleted;
        entity.UpdatedAt = DateTime.UtcNow;
        entity.UpdatedBy = _currentUser.UserId;

        await _db.Updateable(entity).ExecuteCommandAsync();
    }

    /// <summary>
    /// 生成客户编码
    /// </summary>
    /// <returns>客户编码</returns>
    private async Task<string> GenerateCustomerCodeAsync()
    {
        var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
        var random = new Random().Next(1000, 9999);
        var code = $"CU{timestamp}{random}";

        // 确保编码唯一
        var exists = await _db.Queryable<Customer>()
            .Where(c => c.Code == code)
            .AnyAsync();

        if (exists)
        {
            return await GenerateCustomerCodeAsync();
        }

        return code;
    }
}
```

### 5.3 库存服务实现要点

```csharp
using EasyProduct.Models.Constants;

namespace EasyProduct.Crm.Services;

/// <summary>
/// 库存服务接口
/// </summary>
public interface IStockService
{
    /// <summary>
    /// 入库操作
    /// </summary>
    /// <param name="warehouseId">仓库ID</param>
    /// <param name="spuId">商品ID</param>
    /// <param name="specValues">规格值</param>
    /// <param name="quantity">数量</param>
    /// <param name="costPrice">成本价</param>
    /// <param name="bizType">业务类型</param>
    /// <param name="bizId">业务单据ID</param>
    /// <param name="bizNo">业务单据编号</param>
    Task StockInAsync(Guid warehouseId, Guid spuId, string specValues, 
        int quantity, decimal costPrice, int bizType, Guid bizId, string bizNo);

    /// <summary>
    /// 出库操作
    /// </summary>
    /// <param name="warehouseId">仓库ID</param>
    /// <param name="spuId">商品ID</param>
    /// <param name="specValues">规格值</param>
    /// <param name="quantity">数量</param>
    /// <param name="bizType">业务类型</param>
    /// <param name="bizId">业务单据ID</param>
    /// <param name="bizNo">业务单据编号</param>
    Task StockOutAsync(Guid warehouseId, Guid spuId, string specValues,
        int quantity, int bizType, Guid bizId, string bizNo);

    /// <summary>
    /// 锁定库存
    /// </summary>
    /// <param name="warehouseId">仓库ID</param>
    /// <param name="spuId">商品ID</param>
    /// <param name="specValues">规格值</param>
    /// <param name="quantity">数量</param>
    Task LockStockAsync(Guid warehouseId, Guid spuId, string specValues, int quantity);

    /// <summary>
    /// 解锁库存
    /// </summary>
    /// <param name="warehouseId">仓库ID</param>
    /// <param name="spuId">商品ID</param>
    /// <param name="specValues">规格值</param>
    /// <param name="quantity">数量</param>
    Task UnlockStockAsync(Guid warehouseId, Guid spuId, string specValues, int quantity);
}

/// <summary>
/// 库存服务实现
/// </summary>
public class StockService : IStockService
{
    private readonly ISqlSugarClient _db;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="db">数据库上下文</param>
    public StockService(ISqlSugarClient db)
    {
        _db = db;
    }

    /// <summary>
    /// 入库操作（加权平均法计算成本）
    /// </summary>
    /// <param name="warehouseId">仓库ID</param>
    /// <param name="spuId">商品ID</param>
    /// <param name="specValues">规格值</param>
    /// <param name="quantity">入库数量</param>
    /// <param name="costPrice">成本价</param>
    /// <param name="bizType">业务类型</param>
    /// <param name="bizId">业务单据ID</param>
    /// <param name="bizNo">业务单据编号</param>
    /// <remarks>
    /// 1. 使用事务确保数据一致性
    /// 2. 加权平均法计算成本价：新成本 = (原库存 × 原成本 + 入库数量 × 入库成本) / (原库存 + 入库数量)
    /// 3. 记录库存流水
    /// </remarks>
    [UnitOfWork]
    public async Task StockInAsync(Guid warehouseId, Guid spuId, string specValues,
        int quantity, decimal costPrice, int bizType, Guid bizId, string bizNo)
    {
        // 查询库存记录
        var stock = await _db.Queryable<Stock>()
            .Where(s => s.WarehouseId == warehouseId && s.SpuId == spuId && s.SpecValues == specValues)
            .FirstAsync();

        if (stock == null)
        {
            // 新建库存记录
            stock = new Stock
            {
                WarehouseId = warehouseId,
                SpuId = spuId,
                SpecValues = specValues,
                Quantity = quantity,
                LockedQty = 0,
                AvailableQty = quantity,
                CostPrice = costPrice
            };

            await _db.Insertable(stock).ExecuteCommandAsync();
        }
        else
        {
            // 计算加权平均成本
            var totalQty = stock.Quantity + quantity;
            var totalAmount = stock.Quantity * stock.CostPrice + quantity * costPrice;
            var newCostPrice = Math.Round(totalAmount / totalQty, 2);

            // 更新库存
            stock.Quantity += quantity;
            stock.AvailableQty += quantity;
            stock.CostPrice = newCostPrice;
            stock.UpdatedAt = DateTime.UtcNow;

            await _db.Updateable(stock).ExecuteCommandAsync();
        }

        // 记录库存流水
        var record = new StockRecord
        {
            WarehouseId = warehouseId,
            SpuId = spuId,
            SpecValues = specValues,
            RecordType = CrmStockRecordType.In,
            BizType = bizType,
            BizId = bizId,
            BizNo = bizNo,
            Quantity = quantity,
            BeforeQty = stock.Quantity - quantity,
            AfterQty = stock.Quantity,
            CostPrice = costPrice,
            Amount = quantity * costPrice
        };

        await _db.Insertable(record).ExecuteCommandAsync();
    }

    /// <summary>
    /// 出库操作
    /// </summary>
    /// <param name="warehouseId">仓库ID</param>
    /// <param name="spuId">商品ID</param>
    /// <param name="specValues">规格值</param>
    /// <param name="quantity">出库数量</param>
    /// <param name="bizType">业务类型</param>
    /// <param name="bizId">业务单据ID</param>
    /// <param name="bizNo">业务单据编号</param>
    /// <exception cref="BusinessException">库存不足时抛出异常</exception>
    [UnitOfWork]
    public async Task StockOutAsync(Guid warehouseId, Guid spuId, string specValues,
        int quantity, int bizType, Guid bizId, string bizNo)
    {
        // 查询库存记录
        var stock = await _db.Queryable<Stock>()
            .Where(s => s.WarehouseId == warehouseId && s.SpuId == spuId && s.SpecValues == specValues)
            .FirstAsync();

        if (stock == null || stock.AvailableQty < quantity)
        {
            throw new BusinessException($"库存不足，当前可用库存：{stock?.AvailableQty ?? 0}");
        }

        var beforeQty = stock.Quantity;

        // 更新库存
        stock.Quantity -= quantity;
        stock.AvailableQty -= quantity;
        stock.UpdatedAt = DateTime.UtcNow;

        await _db.Updateable(stock).ExecuteCommandAsync();

        // 记录库存流水
        var record = new StockRecord
        {
            WarehouseId = warehouseId,
            SpuId = spuId,
            SpecValues = specValues,
            RecordType = CrmStockRecordType.Out,
            BizType = bizType,
            BizId = bizId,
            BizNo = bizNo,
            Quantity = -quantity, // 出库为负数
            BeforeQty = beforeQty,
            AfterQty = stock.Quantity,
            CostPrice = stock.CostPrice,
            Amount = quantity * stock.CostPrice
        };

        await _db.Insertable(record).ExecuteCommandAsync();
    }

    /// <summary>
    /// 锁定库存
    /// </summary>
    /// <param name="warehouseId">仓库ID</param>
    /// <param name="spuId">商品ID</param>
    /// <param name="specValues">规格值</param>
    /// <param name="quantity">锁定数量</param>
    /// <exception cref="BusinessException">库存不足时抛出异常</exception>
    public async Task LockStockAsync(Guid warehouseId, Guid spuId, string specValues, int quantity)
    {
        var stock = await _db.Queryable<Stock>()
            .Where(s => s.WarehouseId == warehouseId && s.SpuId == spuId && s.SpecValues == specValues)
            .FirstAsync();

        if (stock == null || stock.AvailableQty < quantity)
        {
            throw new BusinessException($"库存不足，当前可用库存：{stock?.AvailableQty ?? 0}");
        }

        stock.LockedQty += quantity;
        stock.AvailableQty -= quantity;
        stock.UpdatedAt = DateTime.UtcNow;

        await _db.Updateable(stock).ExecuteCommandAsync();
    }

    /// <summary>
    /// 解锁库存
    /// </summary>
    /// <param name="warehouseId">仓库ID</param>
    /// <param name="spuId">商品ID</param>
    /// <param name="specValues">规格值</param>
    /// <param name="quantity">解锁数量</param>
    public async Task UnlockStockAsync(Guid warehouseId, Guid spuId, string specValues, int quantity)
    {
        var stock = await _db.Queryable<Stock>()
            .Where(s => s.WarehouseId == warehouseId && s.SpuId == spuId && s.SpecValues == specValues)
            .FirstAsync();

        if (stock != null)
        {
            stock.LockedQty = Math.Max(0, stock.LockedQty - quantity);
            stock.AvailableQty += quantity;
            stock.UpdatedAt = DateTime.UtcNow;

            await _db.Updateable(stock).ExecuteCommandAsync();
        }
    }
}
```

---

## 六、Controller 层实现指南

### 6.1 Controller 层规范

1. **路由规范**：`/api/admin/crm/xxx`
2. **响应格式**：统一使用 `ApiResponse<T>`
3. **参数验证**：使用 FluentValidation
4. **权限控制**：使用 `[Authorize]` 特性
5. **异常处理**：不捕获异常，由全局异常中间件处理
6. **注释规范**：所有公开方法必须添加中文注释

### 6.2 客户 Controller 示例

```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace EasyProduct.Crm.Controllers;

/// <summary>
/// 客户管理控制器
/// </summary>
[ApiController]
[Route("api/admin/crm/customer")]
[Authorize]
public class CustomerController : ControllerBase
{
    private readonly ICustomerService _customerService;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="customerService">客户服务</param>
    public CustomerController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    /// <summary>
    /// 获取客户列表（支持分页、筛选）
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>客户列表</returns>
    /// <remarks>
    /// 支持按类型、状态、销售员筛选，支持关键词模糊搜索
    /// </remarks>
    [HttpGet("list")]
    public async Task<ApiResponse<PageResult<CustomerDto>>> GetList([FromQuery] CustomerQueryDto query)
    {
        var result = await _customerService.GetListAsync(query);
        return ApiResponse.Success(result);
    }

    /// <summary>
    /// 获取客户详情
    /// </summary>
    /// <param name="id">客户ID（GUID字符串）</param>
    /// <returns>客户详情</returns>
    [HttpGet("{id}")]
    public async Task<ApiResponse<CustomerDto>> GetById(string id)
    {
        var guid = Guid.Parse(id);
        var result = await _customerService.GetByIdAsync(guid);
        return ApiResponse.Success(result);
    }

    /// <summary>
    /// 创建客户
    /// </summary>
    /// <param name="dto">创建参数</param>
    /// <returns>客户ID</returns>
    [HttpPost]
    public async Task<ApiResponse<string>> Create([FromBody] CustomerCreateDto dto)
    {
        var id = await _customerService.CreateAsync(dto);
        return ApiResponse.Success(id.ToString(), "创建成功");
    }

    /// <summary>
    /// 更新客户
    /// </summary>
    /// <param name="id">客户ID（GUID字符串）</param>
    /// <param name="dto">更新参数</param>
    /// <returns>操作结果</returns>
    [HttpPut("{id}")]
    public async Task<ApiResponse> Update(string id, [FromBody] CustomerUpdateDto dto)
    {
        var guid = Guid.Parse(id);
        await _customerService.UpdateAsync(guid, dto);
        return ApiResponse.Success("更新成功");
    }

    /// <summary>
    /// 删除客户
    /// </summary>
    /// <param name="id">客户ID（GUID字符串）</param>
    /// <returns>操作结果</returns>
    [HttpDelete("{id}")]
    public async Task<ApiResponse> Delete(string id)
    {
        var guid = Guid.Parse(id);
        await _customerService.DeleteAsync(guid);
        return ApiResponse.Success("删除成功");
    }
}
```

### 6.3 采购订单 Controller 示例

```csharp
namespace EasyProduct.Crm.Controllers;

/// <summary>
/// 采购订单控制器
/// </summary>
[ApiController]
[Route("api/admin/crm/purchase-order")]
[Authorize]
public class PurchaseOrderController : ControllerBase
{
    private readonly IPurchaseOrderService _purchaseOrderService;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="purchaseOrderService">采购订单服务</param>
    public PurchaseOrderController(IPurchaseOrderService purchaseOrderService)
    {
        _purchaseOrderService = purchaseOrderService;
    }

    /// <summary>
    /// 获取采购订单列表
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>采购订单列表</returns>
    [HttpGet("list")]
    public async Task<ApiResponse<PageResult<PurchaseOrderDto>>> GetList([FromQuery] PurchaseOrderQueryDto query)
    {
        var result = await _purchaseOrderService.GetListAsync(query);
        return ApiResponse.Success(result);
    }

    /// <summary>
    /// 获取采购订单详情
    /// </summary>
    /// <param name="id">订单ID</param>
    /// <returns>订单详情</returns>
    [HttpGet("{id}")]
    public async Task<ApiResponse<PurchaseOrderDto>> GetById(string id)
    {
        var result = await _purchaseOrderService.GetByIdAsync(id);
        return ApiResponse.Success(result);
    }

    /// <summary>
    /// 创建采购订单
    /// </summary>
    /// <param name="dto">创建参数</param>
    /// <returns>订单ID</returns>
    [HttpPost]
    public async Task<ApiResponse<string>> Create([FromBody] PurchaseOrderCreateDto dto)
    {
        var id = await _purchaseOrderService.CreateAsync(dto);
        return ApiResponse.Success(id, "创建成功");
    }

    /// <summary>
    /// 更新采购订单
    /// </summary>
    /// <param name="id">订单ID</param>
    /// <param name="dto">更新参数</param>
    /// <returns>操作结果</returns>
    [HttpPut("{id}")]
    public async Task<ApiResponse> Update(string id, [FromBody] PurchaseOrderUpdateDto dto)
    {
        await _purchaseOrderService.UpdateAsync(id, dto);
        return ApiResponse.Success("更新成功");
    }

    /// <summary>
    /// 提交采购订单
    /// </summary>
    /// <param name="id">订单ID</param>
    /// <returns>操作结果</returns>
    [HttpPost("{id}/submit")]
    public async Task<ApiResponse> Submit(string id)
    {
        await _purchaseOrderService.SubmitAsync(id);
        return ApiResponse.Success("提交成功");
    }

    /// <summary>
    /// 审批采购订单
    /// </summary>
    /// <param name="id">订单ID</param>
    /// <param name="dto">审批参数</param>
    /// <returns>操作结果</returns>
    [HttpPost("{id}/approve")]
    public async Task<ApiResponse> Approve(string id, [FromBody] ApproveDto dto)
    {
        await _purchaseOrderService.ApproveAsync(id, dto);
        return ApiResponse.Success("审批成功");
    }

    /// <summary>
    /// 采购入库
    /// </summary>
    /// <param name="id">订单ID</param>
    /// <param name="dto">入库参数</param>
    /// <returns>操作结果</returns>
    [HttpPost("{id}/receive")]
    public async Task<ApiResponse> Receive(string id, [FromBody] ReceiveDto dto)
    {
        await _purchaseOrderService.ReceiveAsync(id, dto);
        return ApiResponse.Success("入库成功");
    }

    /// <summary>
    /// 取消采购订单
    /// </summary>
    /// <param name="id">订单ID</param>
    /// <returns>操作结果</returns>
    [HttpPost("{id}/cancel")]
    public async Task<ApiResponse> Cancel(string id)
    {
        await _purchaseOrderService.CancelAsync(id);
        return ApiResponse.Success("取消成功");
    }
}
```

---

## 七、数据库表设计（SQL）

### 7.1 基础数据表

#### 客户表

```sql
CREATE TABLE `crm_customer` (
  `id` char(36) NOT NULL COMMENT '主键ID（GUID）',
  `code` varchar(50) NOT NULL COMMENT '客户编码',
  `name` varchar(200) NOT NULL COMMENT '客户名称',
  `type` int NOT NULL COMMENT '类型：1-企业客户，2-零售客户',
  `source` int DEFAULT NULL COMMENT '来源：1-询盘，2-注册，3-手动录入',
  `contact_person` varchar(50) DEFAULT NULL COMMENT '联系人',
  `phone` varchar(50) DEFAULT NULL COMMENT '电话',
  `email` varchar(100) DEFAULT NULL COMMENT '邮箱',
  `address` varchar(500) DEFAULT NULL COMMENT '地址',
  `sales_person_id` char(36) DEFAULT NULL COMMENT '销售员ID',
  `sales_person_name` varchar(50) DEFAULT NULL COMMENT '销售员姓名',
  `credit_limit` decimal(12,2) DEFAULT '0.00' COMMENT '信用额度',
  `status` int DEFAULT '1' COMMENT '状态：1-启用，0-停用',
  `remark` text COMMENT '备注',
  `is_deleted` int DEFAULT '0' COMMENT '是否删除：0-未删除，1-已删除',
  `created_at` datetime NOT NULL COMMENT '创建时间',
  `updated_at` datetime DEFAULT NULL COMMENT '更新时间',
  `created_by` char(36) DEFAULT NULL COMMENT '创建人ID',
  `updated_by` char(36) DEFAULT NULL COMMENT '更新人ID',
  PRIMARY KEY (`id`),
  UNIQUE KEY `uk_code` (`code`),
  KEY `idx_name` (`name`),
  KEY `idx_type` (`type`),
  KEY `idx_status` (`status`),
  KEY `idx_sales_person_id` (`sales_person_id`),
  KEY `idx_created_at` (`created_at`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='客户表';
```

#### 供应商表

```sql
CREATE TABLE `crm_supplier` (
  `id` char(36) NOT NULL COMMENT '主键ID（GUID）',
  `code` varchar(50) NOT NULL COMMENT '供应商编码',
  `name` varchar(200) NOT NULL COMMENT '供应商名称',
  `contact_person` varchar(50) DEFAULT NULL COMMENT '联系人',
  `phone` varchar(50) DEFAULT NULL COMMENT '电话',
  `email` varchar(100) DEFAULT NULL COMMENT '邮箱',
  `address` varchar(500) DEFAULT NULL COMMENT '地址',
  `bank_name` varchar(100) DEFAULT NULL COMMENT '开户银行',
  `bank_account` varchar(50) DEFAULT NULL COMMENT '银行账号',
  `status` int DEFAULT '1' COMMENT '状态：1-启用，0-停用',
  `remark` text COMMENT '备注',
  `is_deleted` int DEFAULT '0' COMMENT '是否删除：0-未删除，1-已删除',
  `created_at` datetime NOT NULL COMMENT '创建时间',
  `updated_at` datetime DEFAULT NULL COMMENT '更新时间',
  `created_by` char(36) DEFAULT NULL COMMENT '创建人ID',
  `updated_by` char(36) DEFAULT NULL COMMENT '更新人ID',
  PRIMARY KEY (`id`),
  UNIQUE KEY `uk_code` (`code`),
  KEY `idx_name` (`name`),
  KEY `idx_status` (`status`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='供应商表';
```

#### 币种表

```sql
CREATE TABLE `crm_currency` (
  `id` char(36) NOT NULL COMMENT '主键ID（GUID）',
  `code` varchar(10) NOT NULL COMMENT '币种编码',
  `name` varchar(50) NOT NULL COMMENT '币种名称',
  `symbol` varchar(10) DEFAULT NULL COMMENT '符号',
  `exchange_rate` decimal(12,6) DEFAULT '1.000000' COMMENT '汇率',
  `is_default` tinyint(1) DEFAULT '0' COMMENT '是否本位币',
  `status` int DEFAULT '1' COMMENT '状态：1-启用，0-停用',
  `is_deleted` int DEFAULT '0' COMMENT '是否删除：0-未删除，1-已删除',
  `created_at` datetime NOT NULL COMMENT '创建时间',
  `updated_at` datetime DEFAULT NULL COMMENT '更新时间',
  `created_by` char(36) DEFAULT NULL COMMENT '创建人ID',
  `updated_by` char(36) DEFAULT NULL COMMENT '更新人ID',
  PRIMARY KEY (`id`),
  UNIQUE KEY `uk_code` (`code`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='币种表';
```

#### 税率表

```sql
CREATE TABLE `crm_tax_rate` (
  `id` char(36) NOT NULL COMMENT '主键ID（GUID）',
  `code` varchar(50) NOT NULL COMMENT '税率编码',
  `name` varchar(100) NOT NULL COMMENT '税率名称',
  `rate` decimal(5,2) NOT NULL COMMENT '税率百分比',
  `status` int DEFAULT '1' COMMENT '状态：1-启用，0-停用',
  `remark` varchar(500) DEFAULT NULL COMMENT '备注',
  `is_deleted` int DEFAULT '0' COMMENT '是否删除：0-未删除，1-已删除',
  `created_at` datetime NOT NULL COMMENT '创建时间',
  `updated_at` datetime DEFAULT NULL COMMENT '更新时间',
  `created_by` char(36) DEFAULT NULL COMMENT '创建人ID',
  `updated_by` char(36) DEFAULT NULL COMMENT '更新人ID',
  PRIMARY KEY (`id`),
  UNIQUE KEY `uk_code` (`code`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='税率表';
```

### 7.2 采购管理表

#### 采购订单表

```sql
CREATE TABLE `crm_purchase_order` (
  `id` char(36) NOT NULL COMMENT '主键ID（GUID）',
  `order_no` varchar(50) NOT NULL COMMENT '订单编号',
  `supplier_id` char(36) NOT NULL COMMENT '供应商ID',
  `supplier_name` varchar(200) DEFAULT NULL COMMENT '供应商名称',
  `warehouse_id` char(36) NOT NULL COMMENT '入库仓库ID',
  `warehouse_name` varchar(100) DEFAULT NULL COMMENT '仓库名称',
  `currency_id` char(36) NOT NULL COMMENT '币种ID',
  `currency_code` varchar(10) DEFAULT NULL COMMENT '币种编码',
  `tax_rate_id` char(36) DEFAULT NULL COMMENT '税率ID',
  `tax_rate` decimal(5,2) DEFAULT '0.00' COMMENT '税率',
  `total_amount` decimal(12,2) DEFAULT '0.00' COMMENT '订单总额',
  `tax_amount` decimal(12,2) DEFAULT '0.00' COMMENT '税额',
  `total_with_tax` decimal(12,2) DEFAULT '0.00' COMMENT '价税合计',
  `order_status` int DEFAULT '10' COMMENT '订单状态：10-草稿，20-已提交，30-已审批，40-已入库，50-已取消',
  `status` int DEFAULT '1' COMMENT '通用状态：1-正常，0-禁用',
  `remark` text COMMENT '备注',
  `is_deleted` int DEFAULT '0' COMMENT '是否删除：0-未删除，1-已删除',
  `created_at` datetime NOT NULL COMMENT '创建时间',
  `updated_at` datetime DEFAULT NULL COMMENT '更新时间',
  `created_by` char(36) DEFAULT NULL COMMENT '创建人ID',
  `updated_by` char(36) DEFAULT NULL COMMENT '更新人ID',
  PRIMARY KEY (`id`),
  UNIQUE KEY `uk_order_no` (`order_no`),
  KEY `idx_supplier_id` (`supplier_id`),
  KEY `idx_order_status` (`order_status`),
  KEY `idx_created_at` (`created_at`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='采购订单表';
```

#### 采购订单明细表

```sql
CREATE TABLE `crm_purchase_order_item` (
  `id` char(36) NOT NULL COMMENT '主键ID（GUID）',
  `order_id` char(36) NOT NULL COMMENT '订单ID',
  `spu_id` char(36) NOT NULL COMMENT '商品ID',
  `spu_name` varchar(200) DEFAULT NULL COMMENT '商品名称',
  `spec_values` varchar(500) DEFAULT NULL COMMENT '规格值',
  `quantity` int NOT NULL COMMENT '数量',
  `unit_price` decimal(12,2) NOT NULL COMMENT '单价',
  `tax_rate` decimal(5,2) DEFAULT '0.00' COMMENT '税率',
  `tax_amount` decimal(12,2) DEFAULT '0.00' COMMENT '税额',
  `amount` decimal(12,2) DEFAULT '0.00' COMMENT '金额',
  `received_qty` int DEFAULT '0' COMMENT '已入库数量',
  `status` int DEFAULT '1' COMMENT '状态：1-正常，0-禁用',
  `is_deleted` int DEFAULT '0' COMMENT '是否删除：0-未删除，1-已删除',
  `created_at` datetime NOT NULL COMMENT '创建时间',
  `updated_at` datetime DEFAULT NULL COMMENT '更新时间',
  `created_by` char(36) DEFAULT NULL COMMENT '创建人ID',
  `updated_by` char(36) DEFAULT NULL COMMENT '更新人ID',
  PRIMARY KEY (`id`),
  KEY `idx_order_id` (`order_id`),
  KEY `idx_spu_id` (`spu_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='采购订单明细表';
```

### 7.3 销售管理表

#### 销售订单表

```sql
CREATE TABLE `crm_sales_order` (
  `id` char(36) NOT NULL COMMENT '主键ID（GUID）',
  `order_no` varchar(50) NOT NULL COMMENT '订单编号',
  `customer_id` char(36) NOT NULL COMMENT '客户ID',
  `customer_name` varchar(200) DEFAULT NULL COMMENT '客户名称',
  `warehouse_id` char(36) NOT NULL COMMENT '出库仓库ID',
  `warehouse_name` varchar(100) DEFAULT NULL COMMENT '仓库名称',
  `currency_id` char(36) NOT NULL COMMENT '币种ID',
  `currency_code` varchar(10) DEFAULT NULL COMMENT '币种编码',
  `tax_rate_id` char(36) DEFAULT NULL COMMENT '税率ID',
  `tax_rate` decimal(5,2) DEFAULT '0.00' COMMENT '税率',
  `total_amount` decimal(12,2) DEFAULT '0.00' COMMENT '订单总额',
  `tax_amount` decimal(12,2) DEFAULT '0.00' COMMENT '税额',
  `discount_amount` decimal(12,2) DEFAULT '0.00' COMMENT '折扣金额',
  `pay_amount` decimal(12,2) DEFAULT '0.00' COMMENT '应收金额',
  `order_status` int DEFAULT '10' COMMENT '订单状态：10-草稿，20-已提交，30-已审批，40-已发货，50-已完成，60-已取消',
  `status` int DEFAULT '1' COMMENT '通用状态：1-正常，0-禁用',
  `remark` text COMMENT '备注',
  `is_deleted` int DEFAULT '0' COMMENT '是否删除：0-未删除，1-已删除',
  `created_at` datetime NOT NULL COMMENT '创建时间',
  `updated_at` datetime DEFAULT NULL COMMENT '更新时间',
  `created_by` char(36) DEFAULT NULL COMMENT '创建人ID',
  `updated_by` char(36) DEFAULT NULL COMMENT '更新人ID',
  PRIMARY KEY (`id`),
  UNIQUE KEY `uk_order_no` (`order_no`),
  KEY `idx_customer_id` (`customer_id`),
  KEY `idx_order_status` (`order_status`),
  KEY `idx_created_at` (`created_at`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='销售订单表';
```

#### 销售订单明细表

```sql
CREATE TABLE `crm_sales_order_item` (
  `id` char(36) NOT NULL COMMENT '主键ID（GUID）',
  `order_id` char(36) NOT NULL COMMENT '订单ID',
  `spu_id` char(36) NOT NULL COMMENT '商品ID',
  `spu_name` varchar(200) DEFAULT NULL COMMENT '商品名称',
  `spec_values` varchar(500) DEFAULT NULL COMMENT '规格值',
  `quantity` int NOT NULL COMMENT '数量',
  `unit_price` decimal(12,2) NOT NULL COMMENT '单价',
  `tax_rate` decimal(5,2) DEFAULT '0.00' COMMENT '税率',
  `tax_amount` decimal(12,2) DEFAULT '0.00' COMMENT '税额',
  `discount_amount` decimal(12,2) DEFAULT '0.00' COMMENT '折扣金额',
  `amount` decimal(12,2) DEFAULT '0.00' COMMENT '金额',
  `shipped_qty` int DEFAULT '0' COMMENT '已出库数量',
  `status` int DEFAULT '1' COMMENT '状态：1-正常，0-禁用',
  `is_deleted` int DEFAULT '0' COMMENT '是否删除：0-未删除，1-已删除',
  `created_at` datetime NOT NULL COMMENT '创建时间',
  `updated_at` datetime DEFAULT NULL COMMENT '更新时间',
  `created_by` char(36) DEFAULT NULL COMMENT '创建人ID',
  `updated_by` char(36) DEFAULT NULL COMMENT '更新人ID',
  PRIMARY KEY (`id`),
  KEY `idx_order_id` (`order_id`),
  KEY `idx_spu_id` (`spu_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='销售订单明细表';
```

### 7.4 库存管理表

#### 仓库表

```sql
CREATE TABLE `crm_warehouse` (
  `id` char(36) NOT NULL COMMENT '主键ID（GUID）',
  `code` varchar(50) NOT NULL COMMENT '仓库编码',
  `name` varchar(100) NOT NULL COMMENT '仓库名称',
  `type` int DEFAULT '1' COMMENT '类型：1-普通仓库，2-虚拟仓库，3-残次品仓库',
  `address` varchar(500) DEFAULT NULL COMMENT '地址',
  `manager_name` varchar(50) DEFAULT NULL COMMENT '负责人',
  `phone` varchar(50) DEFAULT NULL COMMENT '电话',
  `status` int DEFAULT '1' COMMENT '状态：1-启用，0-停用',
  `remark` text COMMENT '备注',
  `is_deleted` int DEFAULT '0' COMMENT '是否删除：0-未删除，1-已删除',
  `created_at` datetime NOT NULL COMMENT '创建时间',
  `updated_at` datetime DEFAULT NULL COMMENT '更新时间',
  `created_by` char(36) DEFAULT NULL COMMENT '创建人ID',
  `updated_by` char(36) DEFAULT NULL COMMENT '更新人ID',
  PRIMARY KEY (`id`),
  UNIQUE KEY `uk_code` (`code`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='仓库表';
```

#### 库存表

```sql
CREATE TABLE `crm_stock` (
  `id` char(36) NOT NULL COMMENT '主键ID（GUID）',
  `warehouse_id` char(36) NOT NULL COMMENT '仓库ID',
  `warehouse_name` varchar(100) DEFAULT NULL COMMENT '仓库名称',
  `spu_id` char(36) NOT NULL COMMENT '商品ID',
  `spu_name` varchar(200) DEFAULT NULL COMMENT '商品名称',
  `spec_values` varchar(500) DEFAULT NULL COMMENT '规格值',
  `quantity` int DEFAULT '0' COMMENT '库存数量',
  `locked_qty` int DEFAULT '0' COMMENT '锁定数量',
  `available_qty` int DEFAULT '0' COMMENT '可用数量',
  `cost_price` decimal(12,2) DEFAULT '0.00' COMMENT '成本价',
  `status` int DEFAULT '1' COMMENT '状态：1-正常，0-禁用',
  `is_deleted` int DEFAULT '0' COMMENT '是否删除：0-未删除，1-已删除',
  `created_at` datetime NOT NULL COMMENT '创建时间',
  `updated_at` datetime DEFAULT NULL COMMENT '更新时间',
  `created_by` char(36) DEFAULT NULL COMMENT '创建人ID',
  `updated_by` char(36) DEFAULT NULL COMMENT '更新人ID',
  PRIMARY KEY (`id`),
  UNIQUE KEY `uk_warehouse_spu_spec` (`warehouse_id`, `spu_id`, `spec_values`),
  KEY `idx_spu_id` (`spu_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='库存表';
```

#### 库存流水表

```sql
CREATE TABLE `crm_stock_record` (
  `id` char(36) NOT NULL COMMENT '主键ID（GUID）',
  `warehouse_id` char(36) NOT NULL COMMENT '仓库ID',
  `spu_id` char(36) NOT NULL COMMENT '商品ID',
  `spec_values` varchar(500) DEFAULT NULL COMMENT '规格值',
  `record_type` int NOT NULL COMMENT '类型：1-入库，2-出库，3-盘点，4-调整',
  `biz_type` int NOT NULL COMMENT '业务类型：1-采购，2-销售，3-退货，4-调拨，5-盘点，6-调整',
  `biz_id` char(36) DEFAULT NULL COMMENT '业务单据ID',
  `biz_no` varchar(50) DEFAULT NULL COMMENT '业务单据编号',
  `quantity` int NOT NULL COMMENT '数量',
  `before_qty` int DEFAULT NULL COMMENT '变动前数量',
  `after_qty` int DEFAULT NULL COMMENT '变动后数量',
  `cost_price` decimal(12,2) DEFAULT NULL COMMENT '成本价',
  `amount` decimal(12,2) DEFAULT NULL COMMENT '金额',
  `status` int DEFAULT '1' COMMENT '状态：1-正常，0-禁用',
  `is_deleted` int DEFAULT '0' COMMENT '是否删除：0-未删除，1-已删除',
  `created_at` datetime NOT NULL COMMENT '创建时间',
  `updated_at` datetime DEFAULT NULL COMMENT '更新时间',
  `created_by` char(36) DEFAULT NULL COMMENT '创建人ID',
  `updated_by` char(36) DEFAULT NULL COMMENT '更新人ID',
  PRIMARY KEY (`id`),
  KEY `idx_warehouse_id` (`warehouse_id`),
  KEY `idx_spu_id` (`spu_id`),
  KEY `idx_biz_id` (`biz_id`),
  KEY `idx_created_at` (`created_at`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='库存流水表';
```

### 7.5 财务管理表

#### 发票表

```sql
CREATE TABLE `crm_invoice` (
  `id` char(36) NOT NULL COMMENT '主键ID（GUID）',
  `invoice_no` varchar(50) NOT NULL COMMENT '发票号码',
  `invoice_type` int NOT NULL COMMENT '类型：1-销售发票，2-采购发票',
  `biz_id` char(36) DEFAULT NULL COMMENT '业务单据ID',
  `biz_no` varchar(50) DEFAULT NULL COMMENT '业务单据编号',
  `customer_id` char(36) DEFAULT NULL COMMENT '客户ID',
  `supplier_id` char(36) DEFAULT NULL COMMENT '供应商ID',
  `amount` decimal(12,2) DEFAULT '0.00' COMMENT '开票金额',
  `tax_amount` decimal(12,2) DEFAULT '0.00' COMMENT '税额',
  `total_amount` decimal(12,2) DEFAULT '0.00' COMMENT '价税合计',
  `invoice_status` int DEFAULT '10' COMMENT '发票状态：10-草稿，20-已开具，30-已作废',
  `issued_at` datetime DEFAULT NULL COMMENT '开票日期',
  `remark` text COMMENT '备注',
  `status` int DEFAULT '1' COMMENT '通用状态：1-正常，0-禁用',
  `is_deleted` int DEFAULT '0' COMMENT '是否删除：0-未删除，1-已删除',
  `created_at` datetime NOT NULL COMMENT '创建时间',
  `updated_at` datetime DEFAULT NULL COMMENT '更新时间',
  `created_by` char(36) DEFAULT NULL COMMENT '创建人ID',
  `updated_by` char(36) DEFAULT NULL COMMENT '更新人ID',
  PRIMARY KEY (`id`),
  UNIQUE KEY `uk_invoice_no` (`invoice_no`),
  KEY `idx_customer_id` (`customer_id`),
  KEY `idx_supplier_id` (`supplier_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='发票表';
```

#### 收付款表

```sql
CREATE TABLE `crm_payment` (
  `id` char(36) NOT NULL COMMENT '主键ID（GUID）',
  `payment_no` varchar(50) NOT NULL COMMENT '收付款单号',
  `payment_type` int NOT NULL COMMENT '类型：1-收款，2-付款',
  `biz_id` char(36) DEFAULT NULL COMMENT '业务单据ID',
  `biz_no` varchar(50) DEFAULT NULL COMMENT '业务单据编号',
  `customer_id` char(36) DEFAULT NULL COMMENT '客户ID',
  `supplier_id` char(36) DEFAULT NULL COMMENT '供应商ID',
  `amount` decimal(12,2) NOT NULL COMMENT '金额',
  `bank_account_id` char(36) DEFAULT NULL COMMENT '银行账户ID',
  `bank_account_name` varchar(100) DEFAULT NULL COMMENT '银行账户名称',
  `payment_method` int DEFAULT NULL COMMENT '支付方式',
  `payment_status` int DEFAULT '10' COMMENT '支付状态：10-草稿，20-已完成，30-已取消',
  `paid_at` datetime DEFAULT NULL COMMENT '支付日期',
  `remark` text COMMENT '备注',
  `status` int DEFAULT '1' COMMENT '通用状态：1-正常，0-禁用',
  `is_deleted` int DEFAULT '0' COMMENT '是否删除：0-未删除，1-已删除',
  `created_at` datetime NOT NULL COMMENT '创建时间',
  `updated_at` datetime DEFAULT NULL COMMENT '更新时间',
  `created_by` char(36) DEFAULT NULL COMMENT '创建人ID',
  `updated_by` char(36) DEFAULT NULL COMMENT '更新人ID',
  PRIMARY KEY (`id`),
  UNIQUE KEY `uk_payment_no` (`payment_no`),
  KEY `idx_customer_id` (`customer_id`),
  KEY `idx_supplier_id` (`supplier_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='收付款表';
```

#### 应收应付表

```sql
CREATE TABLE `crm_arap` (
  `id` char(36) NOT NULL COMMENT '主键ID（GUID）',
  `arap_type` int NOT NULL COMMENT '类型：1-应收，2-应付',
  `biz_id` char(36) DEFAULT NULL COMMENT '业务单据ID',
  `biz_no` varchar(50) DEFAULT NULL COMMENT '业务单据编号',
  `biz_type` int DEFAULT NULL COMMENT '业务类型：1-销售，2-采购，3-预收预付，4-其他',
  `customer_id` char(36) DEFAULT NULL COMMENT '客户ID',
  `supplier_id` char(36) DEFAULT NULL COMMENT '供应商ID',
  `total_amount` decimal(12,2) DEFAULT '0.00' COMMENT '总金额',
  `paid_amount` decimal(12,2) DEFAULT '0.00' COMMENT '已结算金额',
  `unpaid_amount` decimal(12,2) DEFAULT '0.00' COMMENT '未结算金额',
  `arap_status` int DEFAULT '10' COMMENT '状态：10-未结算，20-部分结算，30-已结算',
  `due_date` datetime DEFAULT NULL COMMENT '到期日',
  `status` int DEFAULT '1' COMMENT '通用状态：1-正常，0-禁用',
  `is_deleted` int DEFAULT '0' COMMENT '是否删除：0-未删除，1-已删除',
  `created_at` datetime NOT NULL COMMENT '创建时间',
  `updated_at` datetime DEFAULT NULL COMMENT '更新时间',
  `created_by` char(36) DEFAULT NULL COMMENT '创建人ID',
  `updated_by` char(36) DEFAULT NULL COMMENT '更新人ID',
  PRIMARY KEY (`id`),
  KEY `idx_customer_id` (`customer_id`),
  KEY `idx_supplier_id` (`supplier_id`),
  KEY `idx_arap_status` (`arap_status`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='应收应付表';
```

---

## 八、业务逻辑要点

### 8.1 多币种处理

#### 汇率转换规则

```csharp
/// <summary>
/// 货币转换服务
/// </summary>
public class CurrencyService : ICurrencyService
{
    /// <summary>
    /// 转换金额到本位币
    /// </summary>
    /// <param name="amount">原币金额</param>
    /// <param name="fromCurrencyId">原币种ID</param>
    /// <returns>本位币金额</returns>
    /// <remarks>
    /// 使用当日汇率将外币金额转换为本位币金额
    /// </remarks>
    public async Task<decimal> ConvertToBaseCurrencyAsync(decimal amount, string fromCurrencyId)
    {
        var currency = await _db.Queryable<Currency>()
            .Where(c => c.Id == fromCurrencyId)
            .FirstAsync();

        if (currency == null)
        {
            throw new BusinessException("币种不存在");
        }

        if (currency.IsDefault)
        {
            return amount;
        }

        return Math.Round(amount * currency.ExchangeRate, 2);
    }

    /// <summary>
    /// 转换金额到目标币种
    /// </summary>
    /// <param name="amount">原币金额</param>
    /// <param name="fromCurrencyId">原币种ID</param>
    /// <param name="toCurrencyId">目标币种ID</param>
    /// <returns>目标币种金额</returns>
    public async Task<decimal> ConvertAsync(decimal amount, string fromCurrencyId, string toCurrencyId)
    {
        // 先转换为本位币
        var baseAmount = await ConvertToBaseCurrencyAsync(amount, fromCurrencyId);

        // 再转换为目标币种
        var toCurrency = await _db.Queryable<Currency>()
            .Where(c => c.Id == toCurrencyId)
            .FirstAsync();

        if (toCurrency == null)
        {
            throw new BusinessException("目标币种不存在");
        }

        if (toCurrency.IsDefault)
        {
            return baseAmount;
        }

        return Math.Round(baseAmount / toCurrency.ExchangeRate, 2);
    }
}
```

### 8.2 税务处理

#### 税额计算规则

```csharp
/// <summary>
/// 税务服务
/// </summary>
public class TaxService : ITaxService
{
    /// <summary>
    /// 计算税额（含税价 → 不含税价 + 税额）
    /// </summary>
    /// <param name="amountWithTax">含税金额</param>
    /// <param name="taxRate">税率百分比</param>
    /// <returns>税额</returns>
    /// <remarks>
    /// 计算公式：税额 = 含税金额 ÷ (1 + 税率) × 税率
    /// </remarks>
    public decimal CalculateTaxFromAmountWithTax(decimal amountWithTax, decimal taxRate)
    {
        var taxRateDecimal = taxRate / 100;
        var taxAmount = amountWithTax / (1 + taxRateDecimal) * taxRateDecimal;
        return Math.Round(taxAmount, 2);
    }

    /// <summary>
    /// 计算税额（不含税价 → 税额）
    /// </summary>
    /// <param name="amount">不含税金额</param>
    /// <param name="taxRate">税率百分比</param>
    /// <returns>税额</returns>
    /// <remarks>
    /// 计算公式：税额 = 不含税金额 × 税率
    /// </remarks>
    public decimal CalculateTax(decimal amount, decimal taxRate)
    {
        var taxRateDecimal = taxRate / 100;
        var taxAmount = amount * taxRateDecimal;
        return Math.Round(taxAmount, 2);
    }

    /// <summary>
    /// 计算含税价
    /// </summary>
    /// <param name="amount">不含税金额</param>
    /// <param name="taxRate">税率百分比</param>
    /// <returns>含税金额</returns>
    public decimal CalculateAmountWithTax(decimal amount, decimal taxRate)
    {
        var taxRateDecimal = taxRate / 100;
        var amountWithTax = amount * (1 + taxRateDecimal);
        return Math.Round(amountWithTax, 2);
    }
}
```

### 8.3 库存管理

#### 加权平均成本计算

```csharp
/// <summary>
/// 计算加权平均成本
/// </summary>
/// <param name="currentQty">当前库存数量</param>
/// <param name="currentCost">当前成本价</param>
/// <param name="inQty">入库数量</param>
/// <param name="inCost">入库成本价</param>
/// <returns>新的加权平均成本价</returns>
/// <remarks>
/// 计算公式：新成本 = (当前库存 × 当前成本 + 入库数量 × 入库成本) / (当前库存 + 入库数量)
/// </remarks>
public decimal CalculateWeightedAverageCost(int currentQty, decimal currentCost, int inQty, decimal inCost)
{
    if (currentQty + inQty == 0)
    {
        return 0;
    }

    var totalAmount = currentQty * currentCost + inQty * inCost;
    var totalQty = currentQty + inQty;
    var newCost = totalAmount / totalQty;

    return Math.Round(newCost, 2);
}
```

#### 库存预警检查

```csharp
/// <summary>
/// 检查库存预警
/// </summary>
/// <param name="stock">库存记录</param>
/// <param name="minQty">最低库存</param>
/// <param name="maxQty">最高库存</param>
/// <returns>预警类型，null 表示无预警</returns>
public string CheckStockAlert(Stock stock, int minQty, int maxQty)
{
    if (stock.AvailableQty <= minQty)
    {
        return "low"; // 库存不足预警
    }

    if (stock.AvailableQty >= maxQty)
    {
        return "high"; // 库存积压预警
    }

    return null;
}
```

### 8.4 财务核销

#### 收付款核销逻辑

```csharp
/// <summary>
/// 核销收付款
/// </summary>
/// <param name="paymentId">收付款ID</param>
/// <param name="arapIds">应收应付ID列表</param>
/// <param name="amounts">核销金额列表</param>
/// <remarks>
    /// 1. 使用事务确保数据一致性
    /// 2. 更新应收应付的已结算金额和未结算金额
    /// 3. 更新应收应付状态（unpaid/partial/paid）
    /// 4. 记录核销流水
    /// </remarks>
[UnitOfWork]
public async Task WriteOffAsync(string paymentId, List<string> arapIds, List<decimal> amounts)
{
    // 查询收付款记录
    var payment = await _db.Queryable<Payment>()
        .Where(p => p.Id == paymentId)
        .FirstAsync();

    if (payment == null)
    {
        throw new BusinessException("收付款记录不存在");
    }

    // 计算核销总金额
    var totalWriteOffAmount = amounts.Sum();

    if (totalWriteOffAmount > payment.Amount)
    {
        throw new BusinessException("核销金额不能大于收付款金额");
    }

    // 核销应收应付
    for (int i = 0; i < arapIds.Count; i++)
    {
        var arapId = arapIds[i];
        var writeOffAmount = amounts[i];

        var arap = await _db.Queryable<Arap>()
            .Where(a => a.Id == arapId)
            .FirstAsync();

        if (arap == null)
        {
            throw new BusinessException($"应收应付记录不存在：{arapId}");
        }

        if (writeOffAmount > arap.UnpaidAmount)
        {
            throw new BusinessException($"核销金额不能大于未结算金额：{arap.BizNo}");
        }

        // 更新应收应付
        arap.PaidAmount += writeOffAmount;
        arap.UnpaidAmount -= writeOffAmount;

        // 更新状态
        if (arap.UnpaidAmount == 0)
        {
            arap.Status = "paid";
        }
        else if (arap.PaidAmount > 0)
        {
            arap.Status = "partial";
        }

        arap.UpdatedAt = DateTime.Now;

        await _db.Updateable(arap).ExecuteCommandAsync();

        // 记录核销流水（此处省略流水记录实体和逻辑）
    }
}
```

---

## 九、单元测试要求

### 9.1 测试原则

1. **涉钱逻辑必须测试**：库存流水、冲销、收付款核销等
2. **边界条件测试**：数量为 0、金额为 0、库存不足等
3. **异常流程测试**：参数验证、业务规则校验
4. **使用内存数据库**：SQLite in-memory 提高测试速度

### 9.2 库存服务测试示例

```csharp
using Xunit;
using Moq;
using SqlSugar;
using EasyProduct.Models.Constants;

namespace EasyProduct.Crm.Tests;

/// <summary>
/// 库存服务单元测试
/// </summary>
public class StockServiceTests
{
    private readonly ISqlSugarClient _db;
    private readonly IStockService _stockService;

    public StockServiceTests()
    {
        // 使用内存数据库
        _db = new SqlSugarClient(new ConnectionConfig
        {
            ConnectionString = "DataSource=:memory:",
            DbType = DbType.Sqlite,
            IsAutoCloseConnection = true,
            InitKeyType = InitKeyType.Attribute
        });

        // 创建表
        _db.CodeFirst.InitTables<Stock, StockRecord>();

        _stockService = new StockService(_db);
    }

    /// <summary>
    /// 测试入库：新库存
    /// </summary>
    [Fact]
    public async Task StockIn_NewStock_ShouldCreateStock()
    {
        // Arrange
        var warehouseId = Guid.NewGuid();
        var spuId = Guid.NewGuid();
        var specValues = "红色-L";
        var quantity = 100;
        var costPrice = 10.5m;
        var bizId = Guid.NewGuid();

        // Act
        await _stockService.StockInAsync(warehouseId, spuId, specValues, 
            quantity, costPrice, CrmStockBizType.Purchase, bizId, "PO20260101001");

        // Assert
        var stock = await _db.Queryable<Stock>()
            .Where(s => s.WarehouseId == warehouseId && s.SpuId == spuId)
            .FirstAsync();

        Assert.NotNull(stock);
        Assert.Equal(quantity, stock.Quantity);
        Assert.Equal(quantity, stock.AvailableQty);
        Assert.Equal(costPrice, stock.CostPrice);
    }

    /// <summary>
    /// 测试入库：更新库存和加权平均成本
    /// </summary>
    [Fact]
    public async Task StockIn_ExistingStock_ShouldUpdateWeightedAverageCost()
    {
        // Arrange
        var warehouseId = Guid.NewGuid();
        var spuId = Guid.NewGuid();
        var specValues = "红色-L";

        // 第一次入库
        await _stockService.StockInAsync(warehouseId, spuId, specValues, 
            100, 10m, CrmStockBizType.Purchase, Guid.NewGuid(), "PO001");

        // 第二次入库（不同成本价）
        var newQuantity = 50;
        var newCostPrice = 12m;

        // Act
        await _stockService.StockInAsync(warehouseId, spuId, specValues,
            newQuantity, newCostPrice, CrmStockBizType.Purchase, Guid.NewGuid(), "PO002");

        // Assert
        var stock = await _db.Queryable<Stock>()
            .Where(s => s.WarehouseId == warehouseId && s.SpuId == spuId)
            .FirstAsync();

        var expectedQuantity = 100 + 50;
        var expectedCost = (100 * 10m + 50 * 12m) / expectedQuantity; // 加权平均成本

        Assert.Equal(expectedQuantity, stock.Quantity);
        Assert.Equal(expectedCost, stock.CostPrice);
    }

    /// <summary>
    /// 测试出库：库存不足应抛出异常
    /// </summary>
    [Fact]
    public async Task StockOut_InsufficientStock_ShouldThrowException()
    {
        // Arrange
        var warehouseId = Guid.NewGuid();
        var spuId = Guid.NewGuid();
        var specValues = "红色-L";

        // 先入库 100
        await _stockService.StockInAsync(warehouseId, spuId, specValues,
            100, 10m, CrmStockBizType.Purchase, Guid.NewGuid(), "PO001");

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BusinessException>(() =>
            _stockService.StockOutAsync(warehouseId, spuId, specValues, 
                200, CrmStockBizType.Sales, Guid.NewGuid(), "SO001"));

        Assert.Contains("库存不足", exception.Message);
    }

    /// <summary>
    /// 测试出库：成功
    /// </summary>
    [Fact]
    public async Task StockOut_Success_ShouldUpdateStock()
    {
        // Arrange
        var warehouseId = Guid.NewGuid();
        var spuId = Guid.NewGuid();
        var specValues = "红色-L";

        // 先入库
        await _stockService.StockInAsync(warehouseId, spuId, specValues,
            100, 10m, CrmStockBizType.Purchase, Guid.NewGuid(), "PO001");

        // Act
        await _stockService.StockOutAsync(warehouseId, spuId, specValues,
            30, CrmStockBizType.Sales, Guid.NewGuid(), "SO001");

        // Assert
        var stock = await _db.Queryable<Stock>()
            .Where(s => s.WarehouseId == warehouseId && s.SpuId == spuId)
            .FirstAsync();

        Assert.Equal(70, stock.Quantity);
        Assert.Equal(70, stock.AvailableQty);
    }

    /// <summary>
    /// 测试库存流水记录
    /// </summary>
    [Fact]
    public async Task StockIn_ShouldCreateStockRecord()
    {
        // Arrange
        var warehouseId = Guid.NewGuid();
        var spuId = Guid.NewGuid();
        var specValues = "红色-L";
        var quantity = 100;
        var costPrice = 10.5m;
        var bizId = Guid.NewGuid();
        var bizNo = "PO001";

        // Act
        await _stockService.StockInAsync(warehouseId, spuId, specValues,
            quantity, costPrice, CrmStockBizType.Purchase, bizId, bizNo);

        // Assert
        var record = await _db.Queryable<StockRecord>()
            .Where(r => r.BizId == bizId)
            .FirstAsync();

        Assert.NotNull(record);
        Assert.Equal(CrmStockRecordType.In, record.RecordType);
        Assert.Equal(CrmStockBizType.Purchase, record.BizType);
        Assert.Equal(quantity, record.Quantity);
        Assert.Equal(bizNo, record.BizNo);
        Assert.Equal(0, record.BeforeQty);
        Assert.Equal(quantity, record.AfterQty);
    }
}
```

### 9.3 财务核销测试示例

```csharp
using EasyProduct.Models.Constants;

/// <summary>
/// 财务服务单元测试
/// </summary>
public class FinanceServiceTests
{
    private readonly ISqlSugarClient _db;
    private readonly IFinanceService _financeService;

    public FinanceServiceTests()
    {
        // 初始化内存数据库
        _db = new SqlSugarClient(new ConnectionConfig
        {
            ConnectionString = "DataSource=:memory:",
            DbType = DbType.Sqlite,
            IsAutoCloseConnection = true,
            InitKeyType = InitKeyType.Attribute
        });

        _db.CodeFirst.InitTables<Payment, Arap>();

        _financeService = new FinanceService(_db);
    }

    /// <summary>
    /// 测试核销：金额不能大于收付款金额
    /// </summary>
    [Fact]
    public async Task WriteOff_AmountExceedsPayment_ShouldThrowException()
    {
        // Arrange
        var payment = new Payment
        {
            PaymentNo = "PAY001",
            PaymentType = CrmPaymentType.Receive,
            Amount = 1000m,
            PaymentStatus = CrmPaymentStatus.Completed
        };

        var arap = new Arap
        {
            ArapType = CrmArapType.Receivable,
            BizNo = "SO001",
            TotalAmount = 2000m,
            PaidAmount = 0,
            UnpaidAmount = 2000m,
            ArapStatus = CrmArapStatus.Unpaid
        };

        await _db.Insertable(payment).ExecuteCommandAsync();
        await _db.Insertable(arap).ExecuteCommandAsync();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BusinessException>(() =>
            _financeService.WriteOffAsync(payment.Id, 
                new List<Guid> { arap.Id }, 
                new List<decimal> { 1500m }));

        Assert.Contains("核销金额不能大于收付款金额", exception.Message);
    }

    /// <summary>
    /// 测试核销：成功更新应收应付状态
    /// </summary>
    [Fact]
    public async Task WriteOff_Success_ShouldUpdateArapStatus()
    {
        // Arrange
        var payment = new Payment
        {
            PaymentNo = "PAY001",
            PaymentType = CrmPaymentType.Receive,
            Amount = 1000m,
            PaymentStatus = CrmPaymentStatus.Completed
        };

        var arap = new Arap
        {
            ArapType = CrmArapType.Receivable,
            BizNo = "SO001",
            TotalAmount = 1000m,
            PaidAmount = 0,
            UnpaidAmount = 1000m,
            ArapStatus = CrmArapStatus.Unpaid
        };

        await _db.Insertable(payment).ExecuteCommandAsync();
        await _db.Insertable(arap).ExecuteCommandAsync();

        // Act
        await _financeService.WriteOffAsync(payment.Id,
            new List<Guid> { arap.Id },
            new List<decimal> { 1000m });

        // Assert
        var updatedArap = await _db.Queryable<Arap>()
            .Where(a => a.Id == arap.Id)
            .FirstAsync();

        Assert.Equal(1000m, updatedArap.PaidAmount);
        Assert.Equal(0, updatedArap.UnpaidAmount);
        Assert.Equal(CrmArapStatus.Paid, updatedArap.ArapStatus);
    }

    /// <summary>
    /// 测试核销：部分核销
    /// </summary>
    [Fact]
    public async Task WriteOff_Partial_ShouldSetPartialStatus()
    {
        // Arrange
        var payment = new Payment
        {
            PaymentNo = "PAY001",
            PaymentType = CrmPaymentType.Receive,
            Amount = 500m,
            PaymentStatus = CrmPaymentStatus.Completed
        };

        var arap = new Arap
        {
            ArapType = CrmArapType.Receivable,
            BizNo = "SO001",
            TotalAmount = 1000m,
            PaidAmount = 0,
            UnpaidAmount = 1000m,
            ArapStatus = CrmArapStatus.Unpaid
        };

        await _db.Insertable(payment).ExecuteCommandAsync();
        await _db.Insertable(arap).ExecuteCommandAsync();

        // Act
        await _financeService.WriteOffAsync(payment.Id,
            new List<Guid> { arap.Id },
            new List<decimal> { 500m });

        // Assert
        var updatedArap = await _db.Queryable<Arap>()
            .Where(a => a.Id == arap.Id)
            .FirstAsync();

        Assert.Equal(500m, updatedArap.PaidAmount);
        Assert.Equal(500m, updatedArap.UnpaidAmount);
        Assert.Equal(CrmArapStatus.Partial, updatedArap.ArapStatus);
    }
}
```

---

## 十、开发检查清单

### 10.1 提交前检查

- [ ] `dotnet build` 0 错误 0 新警告
- [ ] 所有方法添加中文注释
- [ ] 涉钱逻辑编写单元测试并通过
- [ ] 参数验证使用 FluentValidation
- [ ] 异常不捕获，由全局异常中间件处理
- [ ] 事务正确使用（涉钱逻辑必须）
- [ ] 日志记录完整（关键业务操作）
- [ ] 数据库索引合理

### 10.2 实体类检查

- [ ] 继承 `BaseEntity` 基类
- [ ] 主键使用 `Guid` 类型（自动生成）
- [ ] 审计字段由基类提供（不重复定义）
- [ ] 状态字段使用 `int` 类型常量
- [ ] IsDeleted 使用 `int` 类型（0-未删除，1-已删除）
- [ ] SugarTable 特性正确设置表名
- [ ] SugarColumn 特性正确设置字段属性
- [ ] 金额字段 DecimalDigits = 2
- [ ] 导航属性使用 `[SugarColumn(IsIgnore = true)]`
- [ ] 外键字段使用 `Guid` 或 `Guid?` 类型

### 10.3 DTO 检查

- [ ] Request DTO 参数验证完整
- [ ] 命名规范正确
- [ ] 字段注释完整
- [ ] 使用 Mapster 映射

### 10.4 Service 层检查

- [ ] 接口和实现分离
- [ ] 构造器注入依赖
- [ ] 中文注释完整
- [ ] 事务使用正确
- [ ] 异常抛出 BusinessException
- [ ] 软删除逻辑正确

### 10.5 Controller 层检查

- [ ] 路由格式正确 `/api/admin/crm/xxx`
- [ ] 响应格式统一 `ApiResponse<T>`
- [ ] 权限控制正确
- [ ] 中文注释完整
- [ ] 不捕获异常

### 10.6 数据库检查

- [ ] 表名使用 `crm_` 前缀
- [ ] 字段类型正确
- [ ] 索引合理
- [ ] 外键关系正确
- [ ] 默认值合理

### 10.7 单元测试检查

- [ ] 涉钱逻辑测试覆盖
- [ ] 边界条件测试
- [ ] 异常流程测试
- [ ] 使用内存数据库
- [ ] 测试方法命名清晰

---

## 附录

### A. CRM 模块状态常量定义

```csharp
namespace EasyProduct.Models.Constants;

/// <summary>
/// 客户类型常量
/// </summary>
public static class CrmCustomerType
{
    /// <summary>
    /// 企业客户
    /// </summary>
    public const int B2B = 1;

    /// <summary>
    /// 零售客户
    /// </summary>
    public const int Retail = 2;
}

/// <summary>
/// 客户来源常量
/// </summary>
public static class CrmCustomerSource
{
    /// <summary>
    /// 询盘
    /// </summary>
    public const int Inquiry = 1;

    /// <summary>
    /// 注册
    /// </summary>
    public const int Register = 2;

    /// <summary>
    /// 手动录入
    /// </summary>
    public const int Manual = 3;
}

/// <summary>
/// 仓库类型常量
/// </summary>
public static class CrmWarehouseType
{
    /// <summary>
    /// 普通仓库
    /// </summary>
    public const int Normal = 1;

    /// <summary>
    /// 虚拟仓库
    /// </summary>
    public const int Virtual = 2;

    /// <summary>
    /// 残次品仓库
    /// </summary>
    public const int Damage = 3;
}

/// <summary>
/// 采购订单状态常量
/// </summary>
public static class CrmPurchaseOrderStatus
{
    /// <summary>
    /// 草稿
    /// </summary>
    public const int Draft = 10;

    /// <summary>
    /// 已提交
    /// </summary>
    public const int Submitted = 20;

    /// <summary>
    /// 已审批
    /// </summary>
    public const int Approved = 30;

    /// <summary>
    /// 已入库
    /// </summary>
    public const int Received = 40;

    /// <summary>
    /// 已取消
    /// </summary>
    public const int Cancelled = 50;
}

/// <summary>
/// 销售订单状态常量
/// </summary>
public static class CrmSalesOrderStatus
{
    /// <summary>
    /// 草稿
    /// </summary>
    public const int Draft = 10;

    /// <summary>
    /// 已提交
    /// </summary>
    public const int Submitted = 20;

    /// <summary>
    /// 已审批
    /// </summary>
    public const int Approved = 30;

    /// <summary>
    /// 已发货
    /// </summary>
    public const int Shipped = 40;

    /// <summary>
    /// 已完成
    /// </summary>
    public const int Completed = 50;

    /// <summary>
    /// 已取消
    /// </summary>
    public const int Cancelled = 60;
}

/// <summary>
/// 库存流水类型常量
/// </summary>
public static class CrmStockRecordType
{
    /// <summary>
    /// 入库
    /// </summary>
    public const int In = 1;

    /// <summary>
    /// 出库
    /// </summary>
    public const int Out = 2;

    /// <summary>
    /// 盘点
    /// </summary>
    public const int Check = 3;

    /// <summary>
    /// 调整
    /// </summary>
    public const int Adjust = 4;
}

/// <summary>
/// 库存流水业务类型常量
/// </summary>
public static class CrmStockBizType
{
    /// <summary>
    /// 采购
    /// </summary>
    public const int Purchase = 1;

    /// <summary>
    /// 销售
    /// </summary>
    public const int Sales = 2;

    /// <summary>
    /// 退货
    /// </summary>
    public const int Return = 3;

    /// <summary>
    /// 调拨
    /// </summary>
    public const int Transfer = 4;

    /// <summary>
    /// 盘点
    /// </summary>
    public const int Check = 5;

    /// <summary>
    /// 调整
    /// </summary>
    public const int Adjust = 6;
}

/// <summary>
/// 发票类型常量
/// </summary>
public static class CrmInvoiceType
{
    /// <summary>
    /// 销售发票
    /// </summary>
    public const int Sales = 1;

    /// <summary>
    /// 采购发票
    /// </summary>
    public const int Purchase = 2;
}

/// <summary>
/// 发票状态常量
/// </summary>
public static class CrmInvoiceStatus
{
    /// <summary>
    /// 草稿
    /// </summary>
    public const int Draft = 10;

    /// <summary>
    /// 已开具
    /// </summary>
    public const int Issued = 20;

    /// <summary>
    /// 已作废
    /// </summary>
    public const int Cancelled = 30;
}

/// <summary>
/// 收付款类型常量
/// </summary>
public static class CrmPaymentType
{
    /// <summary>
    /// 收款
    /// </summary>
    public const int Receive = 1;

    /// <summary>
    /// 付款
    /// </summary>
    public const int Pay = 2;
}

/// <summary>
/// 收付款状态常量
/// </summary>
public static class CrmPaymentStatus
{
    /// <summary>
    /// 草稿
    /// </summary>
    public const int Draft = 10;

    /// <summary>
    /// 已完成
    /// </summary>
    public const int Completed = 20;

    /// <summary>
    /// 已取消
    /// </summary>
    public const int Cancelled = 30;
}

/// <summary>
/// 应收应付类型常量
/// </summary>
public static class CrmArapType
{
    /// <summary>
    /// 应收
    /// </summary>
    public const int Receivable = 1;

    /// <summary>
    /// 应付
    /// </summary>
    public const int Payable = 2;
}

/// <summary>
/// 应收应付业务类型常量
/// </summary>
public static class CrmArapBizType
{
    /// <summary>
    /// 销售
    /// </summary>
    public const int Sales = 1;

    /// <summary>
    /// 采购
    /// </summary>
    public const int Purchase = 2;

    /// <summary>
    /// 预收预付
    /// </summary>
    public const int Advance = 3;

    /// <summary>
    /// 其他
    /// </summary>
    public const int Other = 4;
}

/// <summary>
/// 应收应付状态常量
/// </summary>
public static class CrmArapStatus
{
    /// <summary>
    /// 未结算
    /// </summary>
    public const int Unpaid = 10;

    /// <summary>
    /// 部分结算
    /// </summary>
    public const int Partial = 20;

    /// <summary>
    /// 已结算
    /// </summary>
    public const int Paid = 30;
}
```

### B. 业务异常定义

```csharp
namespace EasyProduct.Crm.Exceptions;

/// <summary>
/// 业务异常
/// </summary>
public class BusinessException : Exception
{
    public BusinessException(string message) : base(message)
    {
    }

    public BusinessException(string message, Exception innerException) 
        : base(message, innerException)
    {
    }
}
```

### C. 统一响应格式

```csharp
namespace EasyProduct.Core.Models;

/// <summary>
/// 统一 API 响应格式
/// </summary>
public class ApiResponse<T>
{
    /// <summary>
    /// 状态码（200 成功）
    /// </summary>
    public int Code { get; set; }

    /// <summary>
    /// 提示信息
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// 业务数据
    /// </summary>
    public T Data { get; set; }

    /// <summary>
    /// 时间戳
    /// </summary>
    public long Timestamp { get; set; } = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
}

/// <summary>
/// 分页结果
/// </summary>
public class PageResult<T>
{
    /// <summary>
    /// 数据列表
    /// </summary>
    public List<T> List { get; set; }

    /// <summary>
    /// 总条数
    /// </summary>
    public int Total { get; set; }
}

/// <summary>
/// ApiResponse 静态工厂方法
/// </summary>
public static class ApiResponse
{
    public static ApiResponse<T> Success<T>(T data, string message = "操作成功")
    {
        return new ApiResponse<T>
        {
            Code = 200,
            Message = message,
            Data = data
        };
    }

    public static ApiResponse Success(string message = "操作成功")
    {
        return new ApiResponse
        {
            Code = 200,
            Message = message,
            Data = null
        };
    }

    public static ApiResponse<T> Error<T>(string message, int code = 500)
    {
        return new ApiResponse<T>
        {
            Code = code,
            Message = message,
            Data = default
        };
    }
}
```

---

**文档版本**: v1.1  
**创建日期**: 2026-09-07  
**最后更新**: 2026-09-07  
**维护团队**: EasyProduct 后端团队

**v1.1 更新说明：**
- 应用新的设计规范：所有实体继承 BaseEntity
- 状态字段从 string 改为 int 类型
- IsDeleted 从 bool 改为 int 类型
- 新增 CRM 模块专用状态常量定义
- 更新所有示例代码和 SQL 表结构