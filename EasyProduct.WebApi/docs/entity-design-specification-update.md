# 实体设计规范更新说明

> 本文档说明 EasyProduct 项目实体设计的重大变更

---

## 📋 变更概述

### 变更时间
2026-09-07

### 变更内容

| 项目 | 变更前 | 变更后 |
|------|--------|--------|
| **枚举类型** | `string`（如 "active", "pending"） | `int`（如 1, 10, 20） |
| **布尔类型** | `bool` | `int`（0 或 1） |
| **基础字段** | 每个实体单独定义 | 继承 `BaseEntity` 基类 |
| **时间字段** | `CreateTime`, `UpdateTime` | `CreatedAt`, `UpdatedAt` |

---

## ✅ 新的设计规范

### 1. BaseEntity 基类

所有业务实体类继承 `BaseEntity` 基类：

```csharp
public abstract class BaseEntity
{
    public Guid Id { get; set; }                    // GUID 主键
    public int IsDeleted { get; set; }              // 软删除标记（0-未删除，1-已删除）
    public int Status { get; set; }                 // 状态（1-启用，0-禁用）
    public DateTime CreatedAt { get; set; }         // 创建时间
    public DateTime? UpdatedAt { get; set; }        // 更新时间
    public string? CreatedBy { get; set; }          // 创建人ID
    public string? UpdatedBy { get; set; }          // 更新人ID
}
```

### 2. 状态常量类

使用 `int` 常量代替字符串：

```csharp
// 通用状态
public static class CommonStatus
{
    public const int Enabled = 1;    // 启用
    public const int Disabled = 0;   // 禁用
}

// 删除状态
public static class DeleteStatus
{
    public const int NotDeleted = 0; // 未删除
    public const int Deleted = 1;    // 已删除
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
```

### 3. 实体类示例

```csharp
[SugarTable("basic_user", "用户表")]
public class basic_user : BaseEntity
{
    [SugarColumn(Length = 50)]
    public string UserName { get; set; } = string.Empty;

    // Status 和 IsDeleted 由 BaseEntity 继承
}
```

---

## 🎯 变更原因

### 1. 性能优化
- `int` 类型比 `string` 更快
- 数据库索引更小、查询更快
- 减少存储空间

### 2. 代码一致性
- 所有实体统一继承 `BaseEntity`
- 统一的命名规范（CreatedAt 而非 CreateTime）
- 统一的字段类型

### 3. 可维护性
- 减少重复代码
- 集中管理通用字段
- 易于扩展

---

## 📝 迁移指南

### 已修改的文件

#### 核心文件

| 文件 | 修改内容 |
|------|---------|
| **BaseEntity.cs** | 新建基类，定义通用字段 |
| **CommonConstants.cs** | 新建常量类 |
| **basic_user.cs** | 继承 BaseEntity，删除重复字段 |
| **UserService.cs** | 使用 int 类型常量 |

### 字段映射

| 旧字段名 | 新字段名 | 类型变更 |
|---------|---------|---------|
| `bool IsDeleted` | `int IsDeleted` | bool → int |
| `string Status` | `int Status` | string → int |
| `DateTime CreateTime` | `DateTime CreatedAt` | 命名变更 |
| `DateTime UpdateTime` | `DateTime? UpdatedAt` | 命名变更 + 可空 |

### 代码迁移示例

#### 查询条件

**修改前：**
```csharp
.And(u => !u.IsDeleted)
.And(u => u.Status == "active")
```

**修改后：**
```csharp
.And(u => u.IsDeleted == DeleteStatus.NotDeleted)
.And(u => u.Status == CommonStatus.Enabled)
```

#### 赋值语句

**修改前：**
```csharp
entity.Status = "active";
entity.IsDeleted = false;
```

**修改后：**
```csharp
entity.Status = UserStatus.Active;
entity.IsDeleted = DeleteStatus.NotDeleted;
```

---

## 🔄 数据库迁移

### ALTER TABLE 语句

```sql
-- 修改 IsDeleted 字段
ALTER TABLE `basic_user` MODIFY COLUMN `IsDeleted` INT NOT NULL DEFAULT 0 COMMENT '是否删除';

-- 修改 Status 字段
ALTER TABLE `basic_user` MODIFY COLUMN `Status` INT NOT NULL DEFAULT 1 COMMENT '状态';

-- 重命名字段
ALTER TABLE `basic_user` CHANGE `CreateTime` `CreatedAt` DATETIME NOT NULL COMMENT '创建时间';
ALTER TABLE `basic_user` CHANGE `UpdateTime` `UpdatedAt` DATETIME NULL COMMENT '更新时间';

-- 添加缺失字段
ALTER TABLE `basic_user` ADD COLUMN `CreatedBy` VARCHAR(36) NULL COMMENT '创建人ID';
ALTER TABLE `basic_user` ADD COLUMN `UpdatedBy` VARCHAR(36) NULL COMMENT '更新人ID';
```

### 数据迁移脚本

```sql
-- 迁移 Status 数据
UPDATE `basic_user` SET `Status` = 1 WHERE `Status` = 'active';
UPDATE `basic_user` SET `Status` = 0 WHERE `Status` = 'inactive';

-- 迁移 IsDeleted 数据
UPDATE `basic_user` SET `IsDeleted` = 0 WHERE `IsDeleted` = false;
UPDATE `basic_user` SET `IsDeleted` = 1 WHERE `IsDeleted` = true;
```

---

## 📚 常量类参考

### CommonConstants.cs

| 常量类 | 常量名 | 值 | 说明 |
|--------|--------|----|----|
| **CommonStatus** | Enabled | 1 | 启用/正常 |
| | Disabled | 0 | 禁用/停用 |
| **DeleteStatus** | NotDeleted | 0 | 未删除 |
| | Deleted | 1 | 已删除 |
| **UserStatus** | Active | 1 | 活跃 |
| | Inactive | 0 | 禁用 |
| **OrderStatus** | Pending | 10 | 待支付 |
| | Paid | 20 | 已支付 |
| | Shipped | 30 | 已发货 |
| | Completed | 40 | 已完成 |
| | Cancelled | 50 | 已取消 |
| | Refunded | 60 | 已退款 |
| **PaymentStatus** | Pending | 10 | 待支付 |
| | Success | 20 | 成功 |
| | Failed | 30 | 失败 |
| | Refunded | 40 | 已退款 |
| **PaymentMethod** | WeChat | 1 | 微信支付 |
| | Alipay | 2 | 支付宝 |
| | Bank | 3 | 银行转账 |
| | Cash | 4 | 现金 |
| **PointsType** | Earn | 1 | 获得 |
| | Spend | 2 | 消费 |
| **CouponType** | Fixed | 1 | 固定金额 |
| | Percent | 2 | 百分比折扣 |

---

## ⚠️ 注意事项

### 1. 查询条件变更

所有查询需要使用常量：
```csharp
// ❌ 错误
.And(u => !u.IsDeleted)

// ✅ 正确
.And(u => u.IsDeleted == DeleteStatus.NotDeleted)
```

### 2. 状态判断变更

```csharp
// ❌ 错误
if (user.Status == "active")

// ✅ 正确
if (user.Status == UserStatus.Active)
```

### 3. DTO 设计

DTO 中的状态字段也需要使用 `int`：
```csharp
public class UserDto
{
    public int Status { get; set; }
    public int IsDeleted { get; set; }
}
```

---

## 📖 相关文档

- [BaseEntity 使用说明](./baseentity-usage-guide.md)
- [各模块开发文档](./modules/README.md)
- [后端开发规范](./backend-guidelines.md)

---

**更新时间：** 2026-09-07
**维护者：** 后端开发团队