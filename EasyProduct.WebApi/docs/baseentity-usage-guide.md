# BaseEntity 基类使用说明

> 本文档说明如何使用 BaseEntity 基类创建实体类

---

## 📋 概述

`BaseEntity` 是所有业务实体类的基类，包含了通用的基础字段，避免在每个实体类中重复定义。

### 包含的字段

| 字段名 | 类型 | 说明 | 默认值 |
|--------|------|------|--------|
| **Id** | `Guid` | 主键ID（GUID） | 自动生成 GUID |
| **IsDeleted** | `bool` | 软删除标记 | `false` |
| **CreatedAt** | `DateTime` | 创建时间 | `DateTime.UtcNow` |
| **UpdatedAt** | `DateTime?` | 更新时间 | `null` |
| **CreatedBy** | `string?` | 创建人ID | `null` |
| **UpdatedBy** | `string?` | 更新人ID | `null` |

---

## 🚀 使用方法

### 1. 继承 BaseEntity

```csharp
using SqlSugar;
using EasyProduct.Models.Entitys.Base;

namespace EasyProduct.Models.Entitys.Basic;

/// <summary>
/// 用户实体
/// </summary>
[SugarTable("basic_user", "用户表")]
public class basic_user : BaseEntity
{
    /// <summary>
    /// 用户名
    /// </summary>
    [SugarColumn(Length = 50, ColumnDescription = "用户名")]
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// 密码（BCrypt加密）
    /// </summary>
    [SugarColumn(Length = 255, ColumnDescription = "密码")]
    public string Password { get; set; } = string.Empty;

    // 其他业务字段...
}
```

### 2. 完整示例 - 创建订单实体

```csharp
using SqlSugar;
using EasyProduct.Models.Entitys.Base;

namespace EasyProduct.Models.Entitys.Mall;

/// <summary>
/// 订单实体
/// </summary>
[SugarTable("mall_order", "订单表")]
public class Order : BaseEntity
{
    /// <summary>
    /// 订单编号
    /// </summary>
    [SugarColumn(Length = 50, ColumnDescription = "订单编号")]
    public string OrderNo { get; set; } = string.Empty;

    /// <summary>
    /// 会员ID
    /// </summary>
    [SugarColumn(ColumnDescription = "会员ID")]
    public Guid MemberId { get; set; }

    /// <summary>
    /// 订单总金额
    /// </summary>
    [SugarColumn(DecimalDigits = 2, ColumnDescription = "订单总金额")]
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// 订单状态
    /// </summary>
    [SugarColumn(Length = 20, ColumnDescription = "订单状态")]
    public string Status { get; set; } = "pending";

    // 其他业务字段...
}
```

---

## ✅ 优势

### 1. 减少重复代码

**修改前（每个实体都需要重复定义）：**

```csharp
public class basic_user
{
    public Guid Id { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }

    // 业务字段...
}

public class Order
{
    public Guid Id { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }

    // 业务字段...
}
```

**修改后（继承 BaseEntity）：**

```csharp
public class basic_user : BaseEntity
{
    // 业务字段...
}

public class Order : BaseEntity
{
    // 业务字段...
}
```

### 2. 统一规范

- ✅ 所有实体使用相同的主键类型（GUID）
- ✅ 统一的字段命名（CreatedAt、UpdatedAt）
- ✅ 统一的时间格式（UTC）
- ✅ 统一的软删除机制

### 3. 自动赋值

BaseEntity 的构造函数会自动赋值：
- `Id` 自动生成 GUID
- `IsDeleted` 默认 `false`
- `CreatedAt` 默认当前 UTC 时间

---

## ⚠️ 注意事项

### 1. 字段名称变更

**原字段名** → **新字段名**
- `CreateTime` → `CreatedAt`
- `UpdateTime` → `UpdatedAt`

### 2. 时间格式

- ✅ 使用 `DateTime.UtcNow`（UTC 时间）
- ❌ 不要使用 `DateTime.Now`（本地时间）

### 3. 软删除

- 查询时默认过滤 `IsDeleted = true` 的记录
- 删除时设置 `IsDeleted = true`，而不是物理删除

### 4. 主键类型

- 所有实体使用 `Guid` 类型主键
- 自动生成，无需手动赋值

---

## 📊 BaseService 兼容性

`BaseService<T>` 已支持 BaseEntity：

```csharp
public class BaseService<T> where T : class, new()
{
    /// <summary>
    /// 数据库上下文（通过 Autofac 属性注入）
    /// </summary>
    public ISqlSugarClient _db { get; set; } = null!;

    // CRUD 方法自动支持 BaseEntity 的字段
    public virtual async Task<T?> GetByIdAsync(Guid id)
    {
        return await _db.Queryable<T>().InSingleAsync(id);
    }
}
```

---

## 🔧 迁移指南

### 已修改的文件

1. ✅ **BaseEntity.cs** - 创建基类
2. ✅ **basic_user.cs** - 继承 BaseEntity
3. ✅ **UserService.cs** - 更新字段名

### 需要修改的文件

所有现有的实体类都需要：
1. 继承 `BaseEntity`
2. 删除重复的字段（Id、IsDeleted、CreateTime、UpdateTime）
3. 更新字段名（CreateTime → CreatedAt，UpdateTime → UpdatedAt）

### 迁移步骤

1. **创建新的实体类文件**

```csharp
public class NewEntity : BaseEntity
{
    // 只定义业务字段
}
```

2. **更新 Service 层**

```csharp
// 使用 CreatedAt 和 UpdatedAt
entity.CreatedAt = DateTime.UtcNow;
entity.UpdatedAt = DateTime.UtcNow;
```

3. **更新 DTO**

确保 DTO 包含需要的基类字段：

```csharp
public class UserDto
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    // 其他字段...
}
```

---

## 📝 示例对比

### 修改前

```csharp
[SugarTable("basic_user")]
public class basic_user
{
    [SugarColumn(IsPrimaryKey = true)]
    public Guid Id { get; set; }

    public string UserName { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreateTime { get; set; }
    public DateTime UpdateTime { get; set; }
}
```

### 修改后

```csharp
[SugarTable("basic_user", "用户表")]
public class basic_user : BaseEntity
{
    [SugarColumn(Length = 50, ColumnDescription = "用户名")]
    public string UserName { get; set; } = string.Empty;
}
```

**代码行数减少：** 10 行 → 4 行

---

## 🎯 最佳实践

1. **所有业务实体类都应继承 BaseEntity**
2. **不要重复定义基类字段**
3. **使用 UTC 时间**（CreatedAt、UpdatedAt）
4. **使用软删除**（IsDeleted）
5. **在 DTO 中按需包含基类字段**

---

## 📚 相关文档

- [后端开发规范](../../backend-guidelines.md)
- [Autofac 属性注入](../autofac-property-injection-migration.md)
- [各模块开发文档](./modules/)

---

**创建时间：** 2026-09-07
**维护者：** 后端开发团队