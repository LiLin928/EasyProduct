# P3.1 会员管理子系统实施计划

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** 实现完整的会员管理体系,包括会员主档、等级、积分、收藏功能,为商城订单和购物车提供会员身份基础。

**Architecture:** 采用分层架构,Entity → Service → Controller,会员 JWT 独立认证体系,微信登录集成。

**Tech Stack:** .NET 8 + SqlSugar + BCrypt + JWT + 微信小程序登录 API

---

## 子系统概述

**核心模块:**
1. 会员主档 (mall_member)
2. 会员等级 (mall_member_level)
3. 积分记录 (mall_points_record)
4. 收藏 (mall_favorite)
5. 微信登录集成

**数据库表:**
- `mall_member` - 会员主表
- `mall_member_level` - 会员等级
- `mall_points_record` - 积分流水
- `mall_favorite` - 收藏记录

**API 分区:**
- `/api/app/member/*` - 小程序会员端(需 Member JWT)
- `/api/admin/mall/member/*` - 管理端(需 Admin JWT)

---

## 文件结构

**后端文件:**
```
EasyProduct.WebApi/
├── EasyProduct.Models/
│   ├── Entitys/Mall/
│   │   ├── Member.cs
│   │   ├── MemberLevel.cs
│   │   ├── PointsRecord.cs
│   │   └── Favorite.cs
│   ├── Dto/Mall/
│   │   ├── Member/
│   │   │   ├── MemberDto.cs
│   │   │   ├── MemberQuery.cs
│   │   │   ├── MemberCreateDto.cs
│   │   │   ├── MemberUpdateDto.cs
│   │   │   ├── WechatLoginDto.cs
│   │   │   ├── WechatRegisterDto.cs
│   │   │   └── MemberInfoDto.cs
│   │   ├── MemberLevel/
│   │   │   ├── MemberLevelDto.cs
│   │   │   ├── MemberLevelQuery.cs
│   │   │   ├── MemberLevelCreateDto.cs
│   │   │   └── MemberLevelUpdateDto.cs
│   │   ├── PointsRecord/
│   │   │   ├── PointsRecordDto.cs
│   │   │   ├── PointsRecordQuery.cs
│   │   │   └── PointsChangeDto.cs
│   │   └── Favorite/
│   │       ├── FavoriteDto.cs
│   │       ├── FavoriteQuery.cs
│   │       └── FavoriteCreateDto.cs
│   └── Enums/Mall/
│       ├── MemberStatus.cs
│       ├── Gender.cs
│       └── PointsType.cs
├── EasyProduct.Business/Mall/
│   ├── IMemberService.cs
│   ├── MemberService.cs
│   ├── IMemberLevelService.cs
│   ├── MemberLevelService.cs
│   ├── IPointsRecordService.cs
│   ├── PointsRecordService.cs
│   ├── IFavoriteService.cs
│   └── FavoriteService.cs
└── EasyProduct.Web/Controllers/
    ├── App/Mall/
    │   ├── MemberController.cs
    │   ├── PointsController.cs
    │   └── FavoriteController.cs
    └── Admin/Mall/
        ├── MemberController.cs
        ├── MemberLevelController.cs
        └── PointsRecordController.cs
```

**前端文件(Admin):**
```
EasyProduct.Admin/src/
├── api/mall/
│   ├── member.ts
│   ├── member-level.ts
│   └── points-record.ts
├── types/mall.ts
├── i18n/zh-CN/mall.json
├── i18n/en-US/mall.json
└── views/mall/member/
    ├── index.vue
    ├── MemberForm.vue
    ├── LevelForm.vue
    └── PointsDialog.vue
```

---

## 任务分解

### Task 1: 数据库表创建

**Files:**
- Modify: `sql/init-database.sql`

- [ ] **Step 1: 编写会员表 SQL**

在 `sql/init-database.sql` 文件末尾添加:

```sql
-- ========================================
-- Mall 模块表 - 会员相关
-- ========================================

-- ----------------------------
-- 会员等级表 (mall_member_level)
-- ----------------------------
DROP TABLE IF EXISTS `mall_member_level`;
CREATE TABLE `mall_member_level` (
  `id` CHAR(36) NOT NULL COMMENT '主键ID',
  `level_name` VARCHAR(50) NOT NULL COMMENT '等级名称',
  `level_code` VARCHAR(50) NOT NULL COMMENT '等级编码',
  `level` INT NOT NULL COMMENT '等级数值',
  `min_points` INT DEFAULT 0 COMMENT '最低积分要求',
  `max_points` INT DEFAULT 0 COMMENT '最高积分上限',
  `discount_rate` DECIMAL(3,2) DEFAULT 1.00 COMMENT '折扣率(0.00-1.00)',
  `icon` VARCHAR(255) DEFAULT NULL COMMENT '等级图标',
  `status` INT DEFAULT 1 COMMENT '状态：0=禁用，1=启用',
  `create_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `update_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
  `create_by` VARCHAR(50) DEFAULT NULL COMMENT '创建人',
  `is_deleted` TINYINT(1) DEFAULT 0 COMMENT '软删除标记：0=未删除，1=已删除',
  PRIMARY KEY (`id`),
  UNIQUE KEY `uk_level_code` (`level_code`),
  KEY `idx_level` (`level`),
  KEY `idx_status` (`status`),
  KEY `idx_is_deleted` (`is_deleted`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='会员等级表';

-- ----------------------------
-- 会员主表 (mall_member)
-- ----------------------------
DROP TABLE IF EXISTS `mall_member`;
CREATE TABLE `mall_member` (
  `id` CHAR(36) NOT NULL COMMENT '主键ID',
  `openid` VARCHAR(100) DEFAULT NULL COMMENT '微信OpenID',
  `unionid` VARCHAR(100) DEFAULT NULL COMMENT '微信UnionID',
  `nickname` VARCHAR(100) DEFAULT NULL COMMENT '昵称',
  `avatar` VARCHAR(500) DEFAULT NULL COMMENT '头像URL',
  `gender` INT DEFAULT 0 COMMENT '性别：0=未知，1=男，2=女',
  `phone` VARCHAR(20) DEFAULT NULL COMMENT '手机号',
  `real_name` VARCHAR(50) DEFAULT NULL COMMENT '真实姓名',
  `birthday` DATE DEFAULT NULL COMMENT '生日',
  `email` VARCHAR(100) DEFAULT NULL COMMENT '邮箱',
  `level_id` CHAR(36) DEFAULT NULL COMMENT '会员等级ID',
  `points` INT DEFAULT 0 COMMENT '当前积分',
  `total_points` INT DEFAULT 0 COMMENT '累计积分',
  `balance` DECIMAL(18,2) DEFAULT 0.00 COMMENT '账户余额',
  `status` INT DEFAULT 1 COMMENT '状态：0=禁用，1=启用',
  `last_login_time` DATETIME DEFAULT NULL COMMENT '最后登录时间',
  `last_login_ip` VARCHAR(50) DEFAULT NULL COMMENT '最后登录IP',
  `create_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `update_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
  `create_by` VARCHAR(50) DEFAULT NULL COMMENT '创建人',
  `is_deleted` TINYINT(1) DEFAULT 0 COMMENT '软删除标记：0=未删除，1=已删除',
  PRIMARY KEY (`id`),
  UNIQUE KEY `uk_openid` (`openid`),
  KEY `idx_unionid` (`unionid`),
  KEY `idx_phone` (`phone`),
  KEY `idx_level_id` (`level_id`),
  KEY `idx_status` (`status`),
  KEY `idx_is_deleted` (`is_deleted`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='会员主表';

-- ----------------------------
-- 积分记录表 (mall_points_record)
-- ----------------------------
DROP TABLE IF EXISTS `mall_points_record`;
CREATE TABLE `mall_points_record` (
  `id` CHAR(36) NOT NULL COMMENT '主键ID',
  `member_id` CHAR(36) NOT NULL COMMENT '会员ID',
  `points_type` INT NOT NULL COMMENT '积分类型：1=消费获得，2=订单使用，3=后台调整，4=签到，5=注册赠送',
  `points` INT NOT NULL COMMENT '积分变动(正数为获得，负数为使用)',
  `balance` INT NOT NULL COMMENT '变动后余额',
  `order_no` VARCHAR(50) DEFAULT NULL COMMENT '关联订单号',
  `remark` VARCHAR(255) DEFAULT NULL COMMENT '备注',
  `create_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `update_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
  `create_by` VARCHAR(50) DEFAULT NULL COMMENT '创建人',
  `is_deleted` TINYINT(1) DEFAULT 0 COMMENT '软删除标记：0=未删除，1=已删除',
  PRIMARY KEY (`id`),
  KEY `idx_member_id` (`member_id`),
  KEY `idx_points_type` (`points_type`),
  KEY `idx_order_no` (`order_no`),
  KEY `idx_create_time` (`create_time`),
  KEY `idx_is_deleted` (`is_deleted`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='积分记录表';

-- ----------------------------
-- 收藏表 (mall_favorite)
-- ----------------------------
DROP TABLE IF EXISTS `mall_favorite`;
CREATE TABLE `mall_favorite` (
  `id` CHAR(36) NOT NULL COMMENT '主键ID',
  `member_id` CHAR(36) NOT NULL COMMENT '会员ID',
  `spu_id` CHAR(36) NOT NULL COMMENT '商品SPU ID',
  `create_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `update_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
  `create_by` VARCHAR(50) DEFAULT NULL COMMENT '创建人',
  `is_deleted` TINYINT(1) DEFAULT 0 COMMENT '软删除标记：0=未删除，1=已删除',
  PRIMARY KEY (`id`),
  UNIQUE KEY `uk_member_spu` (`member_id`, `spu_id`),
  KEY `idx_member_id` (`member_id`),
  KEY `idx_spu_id` (`spu_id`),
  KEY `idx_is_deleted` (`is_deleted`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='收藏表';
```

- [ ] **Step 2: 执行 SQL 创建表**

```bash
mysql -u root -p < D:/4-MyProject/EasyProduct/sql/init-database.sql
```

- [ ] **Step 3: 验证表创建成功**

```sql
USE easyproduct;
SHOW TABLES LIKE 'mall_%';
DESCRIBE mall_member;
DESCRIBE mall_member_level;
DESCRIBE mall_points_record;
DESCRIBE mall_favorite;
```

---

### Task 2: 枚举定义

**Files:**
- Create: `EasyProduct.Models/Enums/Mall/MemberStatus.cs`
- Create: `EasyProduct.Models/Enums/Mall/Gender.cs`
- Create: `EasyProduct.Models/Enums/Mall/PointsType.cs`

- [ ] **Step 1: 创建会员状态枚举**

```csharp
// EasyProduct.Models/Enums/Mall/MemberStatus.cs
namespace EasyProduct.Models.Enums.Mall;

/// <summary>
/// 会员状态枚举
/// </summary>
public enum MemberStatus
{
    /// <summary>禁用</summary>
    Disabled = 0,
    /// <summary>启用</summary>
    Enabled = 1
}
```

- [ ] **Step 2: 创建性别枚举**

```csharp
// EasyProduct.Models/Enums/Mall/Gender.cs
namespace EasyProduct.Models.Enums.Mall;

/// <summary>
/// 性别枚举
/// </summary>
public enum Gender
{
    /// <summary>未知</summary>
    Unknown = 0,
    /// <summary>男</summary>
    Male = 1,
    /// <summary>女</summary>
    Female = 2
}
```

- [ ] **Step 3: 创建积分类型枚举**

```csharp
// EasyProduct.Models/Enums/Mall/PointsType.cs
namespace EasyProduct.Models.Enums.Mall;

/// <summary>
/// 积分类型枚举
/// </summary>
public enum PointsType
{
    /// <summary>消费获得</summary>
    ConsumeEarn = 1,
    /// <summary>订单使用</summary>
    OrderUse = 2,
    /// <summary>后台调整</summary>
    AdminAdjust = 3,
    /// <summary>签到</summary>
    SignIn = 4,
    /// <summary>注册赠送</summary>
    RegisterGift = 5
}
```

- [ ] **Step 4: 构建项目验证枚举**

```bash
cd D:/4-MyProject/EasyProduct/EasyProduct.WebApi && dotnet build
```

---

### Task 3: 实体类创建

**Files:**
- Create: `EasyProduct.Models/Entitys/Mall/Member.cs`
- Create: `EasyProduct.Models/Entitys/Mall/MemberLevel.cs`
- Create: `EasyProduct.Models/Entitys/Mall/PointsRecord.cs`
- Create: `EasyProduct.Models/Entitys/Mall/Favorite.cs`

- [ ] **Step 1: 创建会员等级实体**

```csharp
// EasyProduct.Models/Entitys/Mall/MemberLevel.cs
using SqlSugar;
using EasyProduct.Models.Enums.Mall;

namespace EasyProduct.Models.Entitys.Mall;

/// <summary>
/// 会员等级实体
/// </summary>
[SugarTable("mall_member_level", "会员等级表")]
public class MemberLevel : BaseEntity
{
    /// <summary>
    /// 等级名称
    /// </summary>
    [SugarColumn(Length = 50)]
    public string LevelName { get; set; } = null!;

    /// <summary>
    /// 等级编码
    /// </summary>
    [SugarColumn(Length = 50)]
    public string LevelCode { get; set; } = null!;

    /// <summary>
    /// 等级数值
    /// </summary>
    public int Level { get; set; }

    /// <summary>
    /// 最低积分要求
    /// </summary>
    public int MinPoints { get; set; }

    /// <summary>
    /// 最高积分上限
    /// </summary>
    public int MaxPoints { get; set; }

    /// <summary>
    /// 折扣率(0.00-1.00)
    /// </summary>
    [SugarColumn(Length = 3, DecimalDigits = 2)]
    public decimal DiscountRate { get; set; } = 1.00m;

    /// <summary>
    /// 等级图标
    /// </summary>
    [SugarColumn(Length = 255, IsNullable = true)]
    public string? Icon { get; set; }

    /// <summary>
    /// 状态：0=禁用，1=启用
    /// </summary>
    public MemberStatus Status { get; set; } = MemberStatus.Enabled;
}
```

- [ ] **Step 2: 创建会员实体**

```csharp
// EasyProduct.Models/Entitys/Mall/Member.cs
using SqlSugar;
using EasyProduct.Models.Enums.Mall;

namespace EasyProduct.Models.Entitys.Mall;

/// <summary>
/// 会员主表实体
/// </summary>
[SugarTable("mall_member", "会员主表")]
public class Member : BaseEntity
{
    /// <summary>
    /// 微信OpenID
    /// </summary>
    [SugarColumn(Length = 100, IsNullable = true)]
    public string? OpenId { get; set; }

    /// <summary>
    /// 微信UnionID
    /// </summary>
    [SugarColumn(Length = 100, IsNullable = true)]
    public string? UnionId { get; set; }

    /// <summary>
    /// 昵称
    /// </summary>
    [SugarColumn(Length = 100, IsNullable = true)]
    public string? Nickname { get; set; }

    /// <summary>
    /// 头像URL
    /// </summary>
    [SugarColumn(Length = 500, IsNullable = true)]
    public string? Avatar { get; set; }

    /// <summary>
    /// 性别：0=未知，1=男，2=女
    /// </summary>
    public Gender Gender { get; set; } = Gender.Unknown;

    /// <summary>
    /// 手机号
    /// </summary>
    [SugarColumn(Length = 20, IsNullable = true)]
    public string? Phone { get; set; }

    /// <summary>
    /// 真实姓名
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = true)]
    public string? RealName { get; set; }

    /// <summary>
    /// 生日
    /// </summary>
    public DateTime? Birthday { get; set; }

    /// <summary>
    /// 邮箱
    /// </summary>
    [SugarColumn(Length = 100, IsNullable = true)]
    public string? Email { get; set; }

    /// <summary>
    /// 会员等级ID
    /// </summary>
    public Guid? LevelId { get; set; }

    /// <summary>
    /// 当前积分
    /// </summary>
    public int Points { get; set; }

    /// <summary>
    /// 累计积分
    /// </summary>
    public int TotalPoints { get; set; }

    /// <summary>
    /// 账户余额
    /// </summary>
    [SugarColumn(Length = 18, DecimalDigits = 2)]
    public decimal Balance { get; set; }

    /// <summary>
    /// 状态：0=禁用，1=启用
    /// </summary>
    public MemberStatus Status { get; set; } = MemberStatus.Enabled;

    /// <summary>
    /// 最后登录时间
    /// </summary>
    public DateTime? LastLoginTime { get; set; }

    /// <summary>
    /// 最后登录IP
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = true)]
    public string? LastLoginIp { get; set; }
}
```

- [ ] **Step 3: 创建积分记录实体**

```csharp
// EasyProduct.Models/Entitys/Mall/PointsRecord.cs
using SqlSugar;
using EasyProduct.Models.Enums.Mall;

namespace EasyProduct.Models.Entitys.Mall;

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
    /// 积分类型
    /// </summary>
    public PointsType PointsType { get; set; }

    /// <summary>
    /// 积分变动(正数为获得，负数为使用)
    /// </summary>
    public int Points { get; set; }

    /// <summary>
    /// 变动后余额
    /// </summary>
    public int Balance { get; set; }

    /// <summary>
    /// 关联订单号
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = true)]
    public string? OrderNo { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    [SugarColumn(Length = 255, IsNullable = true)]
    public string? Remark { get; set; }
}
```

- [ ] **Step 4: 创建收藏实体**

```csharp
// EasyProduct.Models/Entitys/Mall/Favorite.cs
using SqlSugar;

namespace EasyProduct.Models.Entitys.Mall;

/// <summary>
/// 收藏实体
/// </summary>
[SugarTable("mall_favorite", "收藏表")]
public class Favorite : BaseEntity
{
    /// <summary>
    /// 会员ID
    /// </summary>
    public Guid MemberId { get; set; }

    /// <summary>
    /// 商品SPU ID
    /// </summary>
    public Guid SpuId { get; set; }
}
```

- [ ] **Step 5: 构建项目验证实体**

```bash
cd D:/4-MyProject/EasyProduct/EasyProduct.WebApi && dotnet build
```

---

### Task 4: DTO 定义

**Files:**
- Create: `EasyProduct.Models/Dto/Mall/Member/MemberDto.cs`
- Create: `EasyProduct.Models/Dto/Mall/Member/MemberQuery.cs`
- Create: `EasyProduct.Models/Dto/Mall/Member/MemberCreateDto.cs`
- Create: `EasyProduct.Models/Dto/Mall/Member/MemberUpdateDto.cs`
- Create: `EasyProduct.Models/Dto/Mall/Member/WechatLoginDto.cs`
- Create: `EasyProduct.Models/Dto/Mall/Member/MemberInfoDto.cs`

- [ ] **Step 1: 创建会员查询 DTO**

```csharp
// EasyProduct.Models/Dto/Mall/Member/MemberQuery.cs
using System.ComponentModel.DataAnnotations;
using EasyProduct.Models.Dto.Common;

namespace EasyProduct.Models.Dto.Mall.Member;

/// <summary>
/// 会员查询参数
/// </summary>
public class MemberQuery : PageQuery
{
    /// <summary>
    /// 关键词(昵称/手机号)
    /// </summary>
    [MaxLength(100)]
    public string? Keyword { get; set; }

    /// <summary>
    /// 会员等级ID
    /// </summary>
    public Guid? LevelId { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    public int? Status { get; set; }
}
```

- [ ] **Step 2: 创建会员 DTO**

```csharp
// EasyProduct.Models/Dto/Mall/Member/MemberDto.cs
namespace EasyProduct.Models.Dto.Mall.Member;

/// <summary>
/// 会员DTO
/// </summary>
public class MemberDto
{
    /// <summary>
    /// 主键ID
    /// </summary>
    public string Id { get; set; } = null!;

    /// <summary>
    /// 昵称
    /// </summary>
    public string? Nickname { get; set; }

    /// <summary>
    /// 头像URL
    /// </summary>
    public string? Avatar { get; set; }

    /// <summary>
    /// 性别：0=未知，1=男，2=女
    /// </summary>
    public int Gender { get; set; }

    /// <summary>
    /// 手机号
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// 真实姓名
    /// </summary>
    public string? RealName { get; set; }

    /// <summary>
    /// 生日
    /// </summary>
    public DateTime? Birthday { get; set; }

    /// <summary>
    /// 邮箱
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// 会员等级ID
    /// </summary>
    public string? LevelId { get; set; }

    /// <summary>
    /// 会员等级名称
    /// </summary>
    public string? LevelName { get; set; }

    /// <summary>
    /// 当前积分
    /// </summary>
    public int Points { get; set; }

    /// <summary>
    /// 累计积分
    /// </summary>
    public int TotalPoints { get; set; }

    /// <summary>
    /// 账户余额
    /// </summary>
    public decimal Balance { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// 最后登录时间
    /// </summary>
    public DateTime? LastLoginTime { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreateTime { get; set; }
}
```

- [ ] **Step 3: 创建会员创建 DTO**

```csharp
// EasyProduct.Models/Dto/Mall/Member/MemberCreateDto.cs
using System.ComponentModel.DataAnnotations;

namespace EasyProduct.Models.Dto.Mall.Member;

/// <summary>
/// 会员创建DTO
/// </summary>
public class MemberCreateDto
{
    /// <summary>
    /// 昵称
    /// </summary>
    [MaxLength(100)]
    public string? Nickname { get; set; }

    /// <summary>
    /// 头像URL
    /// </summary>
    [MaxLength(500)]
    public string? Avatar { get; set; }

    /// <summary>
    /// 性别
    /// </summary>
    public int Gender { get; set; }

    /// <summary>
    /// 手机号
    /// </summary>
    [MaxLength(20)]
    [RegularExpression(@"^1[3-9]\d{9}$", ErrorMessage = "手机号格式不正确")]
    public string? Phone { get; set; }

    /// <summary>
    /// 真实姓名
    /// </summary>
    [MaxLength(50)]
    public string? RealName { get; set; }

    /// <summary>
    /// 生日
    /// </summary>
    public DateTime? Birthday { get; set; }

    /// <summary>
    /// 邮箱
    /// </summary>
    [MaxLength(100)]
    [EmailAddress(ErrorMessage = "邮箱格式不正确")]
    public string? Email { get; set; }

    /// <summary>
    /// 会员等级ID
    /// </summary>
    public Guid? LevelId { get; set; }
}
```

- [ ] **Step 4: 创建会员更新 DTO**

```csharp
// EasyProduct.Models/Dto/Mall/Member/MemberUpdateDto.cs
using System.ComponentModel.DataAnnotations;

namespace EasyProduct.Models.Dto.Mall.Member;

/// <summary>
/// 会员更新DTO
/// </summary>
public class MemberUpdateDto
{
    /// <summary>
    /// 昵称
    /// </summary>
    [MaxLength(100)]
    public string? Nickname { get; set; }

    /// <summary>
    /// 头像URL
    /// </summary>
    [MaxLength(500)]
    public string? Avatar { get; set; }

    /// <summary>
    /// 性别
    /// </summary>
    public int? Gender { get; set; }

    /// <summary>
    /// 手机号
    /// </summary>
    [MaxLength(20)]
    [RegularExpression(@"^1[3-9]\d{9}$", ErrorMessage = "手机号格式不正确")]
    public string? Phone { get; set; }

    /// <summary>
    /// 真实姓名
    /// </summary>
    [MaxLength(50)]
    public string? RealName { get; set; }

    /// <summary>
    /// 生日
    /// </summary>
    public DateTime? Birthday { get; set; }

    /// <summary>
    /// 邮箱
    /// </summary>
    [MaxLength(100)]
    [EmailAddress(ErrorMessage = "邮箱格式不正确")]
    public string? Email { get; set; }

    /// <summary>
    /// 会员等级ID
    /// </summary>
    public Guid? LevelId { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    public int? Status { get; set; }
}
```

- [ ] **Step 5: 创建微信登录 DTO**

```csharp
// EasyProduct.Models/Dto/Mall/Member/WechatLoginDto.cs
using System.ComponentModel.DataAnnotations;

namespace EasyProduct.Models.Dto.Mall.Member;

/// <summary>
/// 微信登录DTO
/// </summary>
public class WechatLoginDto
{
    /// <summary>
    /// 微信登录code
    /// </summary>
    [Required(ErrorMessage = "code不能为空")]
    [MaxLength(100)]
    public string Code { get; set; } = null!;

    /// <summary>
    /// 用户信息(可选,首次登录加密数据)
    /// </summary>
    public string? EncryptedData { get; set; }

    /// <summary>
    /// 加密算法初始向量
    /// </summary>
    public string? Iv { get; set; }
}
```

- [ ] **Step 6: 创建会员信息 DTO**

```csharp
// EasyProduct.Models/Dto/Mall/Member/MemberInfoDto.cs
namespace EasyProduct.Models.Dto.Mall.Member;

/// <summary>
/// 会员信息DTO(小程序端使用)
/// </summary>
public class MemberInfoDto
{
    /// <summary>
    /// 主键ID
    /// </summary>
    public string Id { get; set; } = null!;

    /// <summary>
    /// 昵称
    /// </summary>
    public string? Nickname { get; set; }

    /// <summary>
    /// 头像URL
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
    /// 会员等级图标
    /// </summary>
    public string? LevelIcon { get; set; }

    /// <summary>
    /// 当前积分
    /// </summary>
    public int Points { get; set; }

    /// <summary>
    /// 账户余额
    /// </summary>
    public decimal Balance { get; set; }

    /// <summary>
    /// 折扣率
    /// </summary>
    public decimal DiscountRate { get; set; } = 1.00m;
}
```

- [ ] **Step 7: 构建项目验证 DTO**

```bash
cd D:/4-MyProject/EasyProduct/EasyProduct.WebApi && dotnet build
```

---

### Task 5: 会员等级 DTO

**Files:**
- Create: `EasyProduct.Models/Dto/Mall/MemberLevel/MemberLevelDto.cs`
- Create: `EasyProduct.Models/Dto/Mall/MemberLevel/MemberLevelQuery.cs`
- Create: `EasyProduct.Models/Dto/Mall/MemberLevel/MemberLevelCreateDto.cs`
- Create: `EasyProduct.Models/Dto/Mall/MemberLevel/MemberLevelUpdateDto.cs`

- [ ] **Step 1: 创建会员等级查询 DTO**

```csharp
// EasyProduct.Models/Dto/Mall/MemberLevel/MemberLevelQuery.cs
using EasyProduct.Models.Dto.Common;

namespace EasyProduct.Models.Dto.Mall.MemberLevel;

/// <summary>
/// 会员等级查询参数
/// </summary>
public class MemberLevelQuery : PageQuery
{
    /// <summary>
    /// 关键词(等级名称/编码)
    /// </summary>
    public string? Keyword { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    public int? Status { get; set; }
}
```

- [ ] **Step 2: 创建会员等级 DTO**

```csharp
// EasyProduct.Models/Dto/Mall/MemberLevel/MemberLevelDto.cs
namespace EasyProduct.Models.Dto.Mall.MemberLevel;

/// <summary>
/// 会员等级DTO
/// </summary>
public class MemberLevelDto
{
    /// <summary>
    /// 主键ID
    /// </summary>
    public string Id { get; set; } = null!;

    /// <summary>
    /// 等级名称
    /// </summary>
    public string LevelName { get; set; } = null!;

    /// <summary>
    /// 等级编码
    /// </summary>
    public string LevelCode { get; set; } = null!;

    /// <summary>
    /// 等级数值
    /// </summary>
    public int Level { get; set; }

    /// <summary>
    /// 最低积分要求
    /// </summary>
    public int MinPoints { get; set; }

    /// <summary>
    /// 最高积分上限
    /// </summary>
    public int MaxPoints { get; set; }

    /// <summary>
    /// 折扣率
    /// </summary>
    public decimal DiscountRate { get; set; }

    /// <summary>
    /// 等级图标
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreateTime { get; set; }
}
```

- [ ] **Step 3: 创建会员等级创建 DTO**

```csharp
// EasyProduct.Models/Dto/Mall/MemberLevel/MemberLevelCreateDto.cs
using System.ComponentModel.DataAnnotations;

namespace EasyProduct.Models.Dto.Mall.MemberLevel;

/// <summary>
/// 会员等级创建DTO
/// </summary>
public class MemberLevelCreateDto
{
    /// <summary>
    /// 等级名称
    /// </summary>
    [Required(ErrorMessage = "等级名称不能为空")]
    [MaxLength(50)]
    public string LevelName { get; set; } = null!;

    /// <summary>
    /// 等级编码
    /// </summary>
    [Required(ErrorMessage = "等级编码不能为空")]
    [MaxLength(50)]
    public string LevelCode { get; set; } = null!;

    /// <summary>
    /// 等级数值
    /// </summary>
    [Required(ErrorMessage = "等级数值不能为空")]
    public int Level { get; set; }

    /// <summary>
    /// 最低积分要求
    /// </summary>
    public int MinPoints { get; set; }

    /// <summary>
    /// 最高积分上限
    /// </summary>
    public int MaxPoints { get; set; }

    /// <summary>
    /// 折扣率(0.00-1.00)
    /// </summary>
    public decimal DiscountRate { get; set; } = 1.00m;

    /// <summary>
    /// 等级图标
    /// </summary>
    [MaxLength(255)]
    public string? Icon { get; set; }
}
```

- [ ] **Step 4: 创建会员等级更新 DTO**

```csharp
// EasyProduct.Models/Dto/Mall/MemberLevel/MemberLevelUpdateDto.cs
using System.ComponentModel.DataAnnotations;

namespace EasyProduct.Models.Dto.Mall.MemberLevel;

/// <summary>
/// 会员等级更新DTO
/// </summary>
public class MemberLevelUpdateDto
{
    /// <summary>
    /// 等级名称
    /// </summary>
    [MaxLength(50)]
    public string? LevelName { get; set; }

    /// <summary>
    /// 等级编码
    /// </summary>
    [MaxLength(50)]
    public string? LevelCode { get; set; }

    /// <summary>
    /// 等级数值
    /// </summary>
    public int? Level { get; set; }

    /// <summary>
    /// 最低积分要求
    /// </summary>
    public int? MinPoints { get; set; }

    /// <summary>
    /// 最高积分上限
    /// </summary>
    public int? MaxPoints { get; set; }

    /// <summary>
    /// 折扣率
    /// </summary>
    public decimal? DiscountRate { get; set; }

    /// <summary>
    /// 等级图标
    /// </summary>
    [MaxLength(255)]
    public string? Icon { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    public int? Status { get; set; }
}
```

- [ ] **Step 5: 构建项目**

```bash
cd D:/4-MyProject/EasyProduct/EasyProduct.WebApi && dotnet build
```

---

### Task 6: 积分和收藏 DTO

**Files:**
- Create: `EasyProduct.Models/Dto/Mall/PointsRecord/PointsRecordDto.cs`
- Create: `EasyProduct.Models/Dto/Mall/PointsRecord/PointsRecordQuery.cs`
- Create: `EasyProduct.Models/Dto/Mall/PointsRecord/PointsChangeDto.cs`
- Create: `EasyProduct.Models/Dto/Mall/Favorite/FavoriteDto.cs`
- Create: `EasyProduct.Models/Dto/Mall/Favorite/FavoriteQuery.cs`
- Create: `EasyProduct.Models/Dto/Mall/Favorite/FavoriteCreateDto.cs`

- [ ] **Step 1: 创建积分记录查询 DTO**

```csharp
// EasyProduct.Models/Dto/Mall/PointsRecord/PointsRecordQuery.cs
using EasyProduct.Models.Dto.Common;

namespace EasyProduct.Models.Dto.Mall.PointsRecord;

/// <summary>
/// 积分记录查询参数
/// </summary>
public class PointsRecordQuery : PageQuery
{
    /// <summary>
    /// 会员ID
    /// </summary>
    public Guid? MemberId { get; set; }

    /// <summary>
    /// 积分类型
    /// </summary>
    public int? PointsType { get; set; }

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

- [ ] **Step 2: 创建积分记录 DTO**

```csharp
// EasyProduct.Models/Dto/Mall/PointsRecord/PointsRecordDto.cs
namespace EasyProduct.Models.Dto.Mall.PointsRecord;

/// <summary>
/// 积分记录DTO
/// </summary>
public class PointsRecordDto
{
    /// <summary>
    /// 主键ID
    /// </summary>
    public string Id { get; set; } = null!;

    /// <summary>
    /// 会员ID
    /// </summary>
    public string MemberId { get; set; } = null!;

    /// <summary>
    /// 会员昵称
    /// </summary>
    public string? MemberNickname { get; set; }

    /// <summary>
    /// 积分类型
    /// </summary>
    public int PointsType { get; set; }

    /// <summary>
    /// 积分类型名称
    /// </summary>
    public string PointsTypeName { get; set; } = null!;

    /// <summary>
    /// 积分变动
    /// </summary>
    public int Points { get; set; }

    /// <summary>
    /// 变动后余额
    /// </summary>
    public int Balance { get; set; }

    /// <summary>
    /// 关联订单号
    /// </summary>
    public string? OrderNo { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    public string? Remark { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreateTime { get; set; }
}
```

- [ ] **Step 3: 创建积分变动 DTO**

```csharp
// EasyProduct.Models/Dto/Mall/PointsRecord/PointsChangeDto.cs
using System.ComponentModel.DataAnnotations;

namespace EasyProduct.Models.Dto.Mall.PointsRecord;

/// <summary>
/// 积分变动DTO
/// </summary>
public class PointsChangeDto
{
    /// <summary>
    /// 会员ID
    /// </summary>
    [Required(ErrorMessage = "会员ID不能为空")]
    public Guid MemberId { get; set; }

    /// <summary>
    /// 积分类型
    /// </summary>
    [Required(ErrorMessage = "积分类型不能为空")]
    public int PointsType { get; set; }

    /// <summary>
    /// 积分变动数量
    /// </summary>
    [Required(ErrorMessage = "积分变动数量不能为空")]
    public int Points { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    [MaxLength(255)]
    public string? Remark { get; set; }
}
```

- [ ] **Step 4: 创建收藏查询 DTO**

```csharp
// EasyProduct.Models/Dto/Mall/Favorite/FavoriteQuery.cs
using EasyProduct.Models.Dto.Common;

namespace EasyProduct.Models.Dto.Mall.Favorite;

/// <summary>
/// 收藏查询参数
/// </summary>
public class FavoriteQuery : PageQuery
{
    /// <summary>
    /// 会员ID
    /// </summary>
    public Guid? MemberId { get; set; }

    /// <summary>
    /// 商品SPU ID
    /// </summary>
    public Guid? SpuId { get; set; }
}
```

- [ ] **Step 5: 创建收藏 DTO**

```csharp
// EasyProduct.Models/Dto/Mall/Favorite/FavoriteDto.cs
namespace EasyProduct.Models.Dto.Mall.Favorite;

/// <summary>
/// 收藏DTO
/// </summary>
public class FavoriteDto
{
    /// <summary>
    /// 主键ID
    /// </summary>
    public string Id { get; set; } = null!;

    /// <summary>
    /// 会员ID
    /// </summary>
    public string MemberId { get; set; } = null!;

    /// <summary>
    /// 商品SPU ID
    /// </summary>
    public string SpuId { get; set; } = null!;

    /// <summary>
    /// 商品名称
    /// </summary>
    public string? SpuName { get; set; }

    /// <summary>
    /// 商品主图
    /// </summary>
    public string? SpuImage { get; set; }

    /// <summary>
    /// 商品价格
    /// </summary>
    public decimal? Price { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreateTime { get; set; }
}
```

- [ ] **Step 6: 创建收藏创建 DTO**

```csharp
// EasyProduct.Models/Dto/Mall/Favorite/FavoriteCreateDto.cs
using System.ComponentModel.DataAnnotations;

namespace EasyProduct.Models.Dto.Mall.Favorite;

/// <summary>
/// 收藏创建DTO
/// </summary>
public class FavoriteCreateDto
{
    /// <summary>
    /// 商品SPU ID
    /// </summary>
    [Required(ErrorMessage = "商品ID不能为空")]
    public Guid SpuId { get; set; }
}
```

- [ ] **Step 7: 构建项目**

```bash
cd D:/4-MyProject/EasyProduct/EasyProduct.WebApi && dotnet build
```

---

### Task 7: 会员等级 Service

**Files:**
- Create: `EasyProduct.Business/Mall/IMemberLevelService.cs`
- Create: `EasyProduct.Business/Mall/MemberLevelService.cs`

- [ ] **Step 1: 创建会员等级服务接口**

```csharp
// EasyProduct.Business/Mall/IMemberLevelService.cs
using EasyProduct.Models.Dto.Common;
using EasyProduct.Models.Dto.Mall.MemberLevel;
using EasyProduct.Models.Entitys.Mall;

namespace EasyProduct.Business.Mall;

/// <summary>
/// 会员等级服务接口
/// </summary>
public interface IMemberLevelService
{
    /// <summary>
    /// 获取会员等级分页列表
    /// </summary>
    Task<PageResult<MemberLevelDto>> GetListAsync(MemberLevelQuery query);

    /// <summary>
    /// 获取所有启用的会员等级
    /// </summary>
    Task<List<MemberLevelDto>> GetAllAsync();

    /// <summary>
    /// 获取会员等级详情
    /// </summary>
    Task<MemberLevelDto> GetByIdAsync(Guid id);

    /// <summary>
    /// 创建会员等级
    /// </summary>
    Task<Guid> CreateAsync(MemberLevelCreateDto dto);

    /// <summary>
    /// 更新会员等级
    /// </summary>
    Task<bool> UpdateAsync(Guid id, MemberLevelUpdateDto dto);

    /// <summary>
    /// 删除会员等级
    /// </summary>
    Task<bool> DeleteAsync(Guid id);

    /// <summary>
    /// 根据积分计算会员等级
    /// </summary>
    Task<MemberLevel?> CalculateLevelByPointsAsync(int points);
}
```

- [ ] **Step 2: 创建会员等级服务实现**

```csharp
// EasyProduct.Business/Mall/MemberLevelService.cs
using Mapster;
using SqlSugar;
using EasyProduct.Common.Base;
using EasyProduct.Common.Error;
using EasyProduct.Models.Dto.Common;
using EasyProduct.Models.Dto.Mall.MemberLevel;
using EasyProduct.Models.Entitys.Mall;

namespace EasyProduct.Business.Mall;

/// <summary>
/// 会员等级服务实现
/// </summary>
public class MemberLevelService : BaseService, IMemberLevelService
{
    private readonly ILogger<MemberLevelService> _logger;

    public MemberLevelService(ISqlSugarClient db, ILogger<MemberLevelService> logger) : base(db)
    {
        _logger = logger;
    }

    /// <summary>
    /// 获取会员等级分页列表
    /// </summary>
    public async Task<PageResult<MemberLevelDto>> GetListAsync(MemberLevelQuery query)
    {
        var queryable = _db.Queryable<MemberLevel>();

        // 关键词搜索
        if (!string.IsNullOrEmpty(query.Keyword))
        {
            queryable = queryable.Where(x =>
                x.LevelName.Contains(query.Keyword) || x.LevelCode.Contains(query.Keyword));
        }

        // 状态筛选
        if (query.Status.HasValue)
        {
            queryable = queryable.Where(x => x.Status == (Models.Enums.Mall.MemberStatus)query.Status.Value);
        }

        // 排序
        queryable = queryable.OrderBy(x => x.Level);

        // 分页
        var total = 0;
        var list = await queryable
            .ToPageListAsync(query.PageIndex, query.PageSize, total);

        return new PageResult<MemberLevelDto>
        {
            List = list.Adapt<List<MemberLevelDto>>(),
            Total = total
        };
    }

    /// <summary>
    /// 获取所有启用的会员等级
    /// </summary>
    public async Task<List<MemberLevelDto>> GetAllAsync()
    {
        var list = await _db.Queryable<MemberLevel>()
            .Where(x => x.Status == Models.Enums.Mall.MemberStatus.Enabled)
            .OrderBy(x => x.Level)
            .ToListAsync();

        return list.Adapt<List<MemberLevelDto>>();
    }

    /// <summary>
    /// 获取会员等级详情
    /// </summary>
    public async Task<MemberLevelDto> GetByIdAsync(Guid id)
    {
        var entity = await _db.Queryable<MemberLevel>()
            .FirstAsync(x => x.Id == id);

        if (entity == null)
        {
            throw BusinessException.NotFound("会员等级不存在");
        }

        return entity.Adapt<MemberLevelDto>();
    }

    /// <summary>
    /// 创建会员等级
    /// </summary>
    public async Task<Guid> CreateAsync(MemberLevelCreateDto dto)
    {
        // 检查编码唯一性
        var exists = await _db.Queryable<MemberLevel>()
            .Where(x => x.LevelCode == dto.LevelCode)
            .FirstAsync();

        if (exists != null)
        {
            throw BusinessException.BadRequest($"等级编码 {dto.LevelCode} 已存在");
        }

        // 检查等级数值唯一性
        var levelExists = await _db.Queryable<MemberLevel>()
            .Where(x => x.Level == dto.Level)
            .FirstAsync();

        if (levelExists != null)
        {
            throw BusinessException.BadRequest($"等级数值 {dto.Level} 已存在");
        }

        var entity = dto.Adapt<MemberLevel>();
        entity.Id = Guid.NewGuid();

        await _db.Insertable(entity).ExecuteCommandAsync();

        _logger.LogInformation("创建会员等级成功: {LevelCode}", entity.LevelCode);

        return entity.Id;
    }

    /// <summary>
    /// 更新会员等级
    /// </summary>
    public async Task<bool> UpdateAsync(Guid id, MemberLevelUpdateDto dto)
    {
        var entity = await _db.Queryable<MemberLevel>()
            .FirstAsync(x => x.Id == id);

        if (entity == null)
        {
            throw BusinessException.NotFound("会员等级不存在");
        }

        // 检查编码唯一性
        if (!string.IsNullOrEmpty(dto.LevelCode) && dto.LevelCode != entity.LevelCode)
        {
            var exists = await _db.Queryable<MemberLevel>()
                .Where(x => x.LevelCode == dto.LevelCode && x.Id != id)
                .FirstAsync();

            if (exists != null)
            {
                throw BusinessException.BadRequest($"等级编码 {dto.LevelCode} 已存在");
            }
        }

        // 检查等级数值唯一性
        if (dto.Level.HasValue && dto.Level.Value != entity.Level)
        {
            var levelExists = await _db.Queryable<MemberLevel>()
                .Where(x => x.Level == dto.Level.Value && x.Id != id)
                .FirstAsync();

            if (levelExists != null)
            {
                throw BusinessException.BadRequest($"等级数值 {dto.Level} 已存在");
            }
        }

        // 更新字段
        if (!string.IsNullOrEmpty(dto.LevelName)) entity.LevelName = dto.LevelName;
        if (!string.IsNullOrEmpty(dto.LevelCode)) entity.LevelCode = dto.LevelCode;
        if (dto.Level.HasValue) entity.Level = dto.Level.Value;
        if (dto.MinPoints.HasValue) entity.MinPoints = dto.MinPoints.Value;
        if (dto.MaxPoints.HasValue) entity.MaxPoints = dto.MaxPoints.Value;
        if (dto.DiscountRate.HasValue) entity.DiscountRate = dto.DiscountRate.Value;
        if (dto.Icon != null) entity.Icon = dto.Icon;
        if (dto.Status.HasValue) entity.Status = (Models.Enums.Mall.MemberStatus)dto.Status.Value;

        entity.UpdateTime = DateTime.Now;

        await _db.Updateable(entity).ExecuteCommandAsync();

        _logger.LogInformation("更新会员等级成功: {LevelCode}", entity.LevelCode);

        return true;
    }

    /// <summary>
    /// 删除会员等级
    /// </summary>
    public async Task<bool> DeleteAsync(Guid id)
    {
        var entity = await _db.Queryable<MemberLevel>()
            .FirstAsync(x => x.Id == id);

        if (entity == null)
        {
            throw BusinessException.NotFound("会员等级不存在");
        }

        // 检查是否有会员使用此等级
        var memberCount = await _db.Queryable<Member>()
            .Where(x => x.LevelId == id)
            .CountAsync();

        if (memberCount > 0)
        {
            throw BusinessException.BadRequest($"该等级下有 {memberCount} 个会员,无法删除");
        }

        // 软删除
        entity.IsDeleted = true;
        entity.UpdateTime = DateTime.Now;

        await _db.Updateable(entity).ExecuteCommandAsync();

        _logger.LogInformation("删除会员等级成功: {LevelCode}", entity.LevelCode);

        return true;
    }

    /// <summary>
    /// 根据积分计算会员等级
    /// </summary>
    public async Task<MemberLevel?> CalculateLevelByPointsAsync(int points)
    {
        var level = await _db.Queryable<MemberLevel>()
            .Where(x =>
                x.Status == Models.Enums.Mall.MemberStatus.Enabled &&
                x.MinPoints <= points &&
                (x.MaxPoints == 0 || x.MaxPoints >= points))
            .OrderBy(x => x.Level, OrderByType.Desc)
            .FirstAsync();

        return level;
    }
}
```

- [ ] **Step 3: 构建项目验证**

```bash
cd D:/4-MyProject/EasyProduct/EasyProduct.WebApi && dotnet build
```

---

### Task 8: 会员 Service(核心)

**Files:**
- Create: `EasyProduct.Business/Mall/IMemberService.cs`
- Create: `EasyProduct.Business/Mall/MemberService.cs`

- [ ] **Step 1: 创建会员服务接口**

```csharp
// EasyProduct.Business/Mall/IMemberService.cs
using EasyProduct.Models.Dto.Common;
using EasyProduct.Models.Dto.Mall.Member;
using EasyProduct.Models.Entitys.Mall;

namespace EasyProduct.Business.Mall;

/// <summary>
/// 会员服务接口
/// </summary>
public interface IMemberService
{
    /// <summary>
    /// 获取会员分页列表
    /// </summary>
    Task<PageResult<MemberDto>> GetListAsync(MemberQuery query);

    /// <summary>
    /// 获取会员详情
    /// </summary>
    Task<MemberDto> GetByIdAsync(Guid id);

    /// <summary>
    /// 创建会员
    /// </summary>
    Task<Guid> CreateAsync(MemberCreateDto dto);

    /// <summary>
    /// 更新会员
    /// </summary>
    Task<bool> UpdateAsync(Guid id, MemberUpdateDto dto);

    /// <summary>
    /// 删除会员
    /// </summary>
    Task<bool> DeleteAsync(Guid id);

    /// <summary>
    /// 微信登录
    /// </summary>
    Task<(string token, string refreshToken, MemberInfoDto member)> WechatLoginAsync(WechatLoginDto dto);

    /// <summary>
    /// 获取当前会员信息
    /// </summary>
    Task<MemberInfoDto> GetCurrentMemberInfoAsync(Guid memberId);

    /// <summary>
    /// 更新会员等级
    /// </summary>
    Task<bool> UpdateMemberLevelAsync(Guid memberId);
}
```

由于响应长度限制,我将在下一个消息中继续编写剩余的任务步骤。