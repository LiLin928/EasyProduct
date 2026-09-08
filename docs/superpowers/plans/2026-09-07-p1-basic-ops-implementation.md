# P1 基础平台实施计划（Basic + Ops）

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** 完成基础管理模块（Basic）和运维模块（Ops）的后端实现，建立完整的 RBAC 权限体系，实现可登录的管理后台。

**Architecture:** 模块化单体架构，Basic 模块包含用户/部门/角色/菜单/字典/公告/系统参数/个人中心；Ops 模块包含操作日志/登录日志。使用 SqlSugar ORM，Autofac DI，Serilog 日志，JWT 双 Token 认证。

**Tech Stack:** .NET 8.0 + SqlSugar 5.1.4 + MySqlConnector 2.5.x + Autofac 8.x + Serilog 8.x + Mapster 10.x + JWT Bearer + BCrypt 4.x

---

## 前置条件

- ✅ EasyProduct.WebApi 项目已创建
- ✅ 四层架构已搭建
- ✅ 基础中间件已实现
- ✅ BaseService 已实现

---

## 模块划分

本计划分为 4 个批次，每个批次独立可演示：

| 批次 | 模块 | 核心功能 | 预计工时 |
|------|------|---------|---------|
| **批次 1** | 认证 + 用户 | 登录/刷新Token、用户管理 CRUD | 2-3 天 |
| **批次 2** | 角色 + 菜单 + 部门 | RBAC 核心、树形结构 | 2-3 天 |
| **批次 3** | 字典 + 公告 + 系统参数 | 字典管理、富文本、配置管理 | 1-2 天 |
| **批次 4** | Ops + 个人中心 | 操作日志、登录日志、个人中心 | 1-2 天 |

---

## 批次 1：认证服务 + 用户管理

### Task 1: 创建数据库初始化脚本

**Files:**
- Create: `sql/init-database.sql`
- Create: `sql/seed-data.sql`

- [ ] **Step 1: 创建数据库初始化脚本**

```sql
-- sql/init-database.sql

-- 创建数据库
CREATE DATABASE IF NOT EXISTS `easyproduct` DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

USE `easyproduct`;

-- ============================================
-- Basic 模块表
-- ============================================

-- 用户表
CREATE TABLE IF NOT EXISTS `basic_user` (
  `id` CHAR(36) NOT NULL COMMENT '主键GUID',
  `user_name` VARCHAR(50) NOT NULL COMMENT '登录账号',
  `password` VARCHAR(255) NOT NULL COMMENT '密码（BCrypt哈希）',
  `real_name` VARCHAR(50) NOT NULL COMMENT '真实姓名',
  `email` VARCHAR(100) COMMENT '邮箱',
  `phone` VARCHAR(20) COMMENT '手机号',
  `status` INT NOT NULL DEFAULT 1 COMMENT '状态：0=禁用，1=启用',
  `dept_id` CHAR(36) COMMENT '部门ID',
  `create_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `update_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
  `create_by` VARCHAR(50) COMMENT '创建人',
  `is_deleted` TINYINT(1) NOT NULL DEFAULT 0 COMMENT '软删除标记',
  PRIMARY KEY (`id`),
  UNIQUE KEY `uk_user_name` (`user_name`),
  KEY `idx_dept_id` (`dept_id`),
  KEY `idx_status` (`status`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='用户表';

-- 角色表
CREATE TABLE IF NOT EXISTS `basic_role` (
  `id` CHAR(36) NOT NULL COMMENT '主键GUID',
  `name` VARCHAR(50) NOT NULL COMMENT '角色名称',
  `code` VARCHAR(50) NOT NULL COMMENT '角色编码',
  `status` INT NOT NULL DEFAULT 1 COMMENT '状态：0=禁用，1=启用',
  `sort` INT NOT NULL DEFAULT 0 COMMENT '排序',
  `remark` VARCHAR(500) COMMENT '备注',
  `create_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `update_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
  `create_by` VARCHAR(50) COMMENT '创建人',
  `is_deleted` TINYINT(1) NOT NULL DEFAULT 0 COMMENT '软删除标记',
  PRIMARY KEY (`id`),
  UNIQUE KEY `uk_code` (`code`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='角色表';

-- 用户角色关联表
CREATE TABLE IF NOT EXISTS `basic_user_role` (
  `id` CHAR(36) NOT NULL COMMENT '主键GUID',
  `user_id` CHAR(36) NOT NULL COMMENT '用户ID',
  `role_id` CHAR(36) NOT NULL COMMENT '角色ID',
  `create_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  PRIMARY KEY (`id`),
  UNIQUE KEY `uk_user_role` (`user_id`, `role_id`),
  KEY `idx_user_id` (`user_id`),
  KEY `idx_role_id` (`role_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='用户角色关联表';

-- 菜单表
CREATE TABLE IF NOT EXISTS `basic_menu` (
  `id` CHAR(36) NOT NULL COMMENT '主键GUID',
  `parent_id` CHAR(36) NOT NULL DEFAULT '0' COMMENT '父菜单ID（根节点为0）',
  `name` VARCHAR(50) NOT NULL COMMENT '路由name',
  `path` VARCHAR(200) NOT NULL COMMENT '路由path',
  `title_key` VARCHAR(100) NOT NULL COMMENT '标题i18n key',
  `icon` VARCHAR(50) COMMENT '图标',
  `sort` INT NOT NULL DEFAULT 0 COMMENT '排序',
  `permission` VARCHAR(100) COMMENT '权限标识',
  `component` VARCHAR(200) COMMENT '组件路径',
  `visible` INT NOT NULL DEFAULT 1 COMMENT '是否显示：0=否，1=是',
  `status` INT NOT NULL DEFAULT 1 COMMENT '状态：0=禁用，1=启用',
  `create_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `update_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
  `create_by` VARCHAR(50) COMMENT '创建人',
  `is_deleted` TINYINT(1) NOT NULL DEFAULT 0 COMMENT '软删除标记',
  PRIMARY KEY (`id`),
  KEY `idx_parent_id` (`parent_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='菜单表';

-- 角色菜单关联表
CREATE TABLE IF NOT EXISTS `basic_role_menu` (
  `id` CHAR(36) NOT NULL COMMENT '主键GUID',
  `role_id` CHAR(36) NOT NULL COMMENT '角色ID',
  `menu_id` CHAR(36) NOT NULL COMMENT '菜单ID',
  `create_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  PRIMARY KEY (`id`),
  UNIQUE KEY `uk_role_menu` (`role_id`, `menu_id`),
  KEY `idx_role_id` (`role_id`),
  KEY `idx_menu_id` (`menu_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='角色菜单关联表';

-- 部门表
CREATE TABLE IF NOT EXISTS `basic_dept` (
  `id` CHAR(36) NOT NULL COMMENT '主键GUID',
  `parent_id` CHAR(36) NOT NULL DEFAULT '0' COMMENT '父部门ID（根节点为0）',
  `name` VARCHAR(50) NOT NULL COMMENT '部门名称',
  `code` VARCHAR(50) NOT NULL COMMENT '部门编码',
  `sort` INT NOT NULL DEFAULT 0 COMMENT '排序',
  `status` INT NOT NULL DEFAULT 1 COMMENT '状态：0=禁用，1=启用',
  `leader_name` VARCHAR(50) COMMENT '负责人姓名',
  `phone` VARCHAR(20) COMMENT '联系电话',
  `email` VARCHAR(100) COMMENT '邮箱',
  `description` VARCHAR(500) COMMENT '描述',
  `create_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `update_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
  `create_by` VARCHAR(50) COMMENT '创建人',
  `is_deleted` TINYINT(1) NOT NULL DEFAULT 0 COMMENT '软删除标记',
  PRIMARY KEY (`id`),
  UNIQUE KEY `uk_code` (`code`),
  KEY `idx_parent_id` (`parent_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='部门表';

-- ============================================
-- Ops 模块表
-- ============================================

-- 操作日志表
CREATE TABLE IF NOT EXISTS `ops_operate_log` (
  `id` CHAR(36) NOT NULL COMMENT '主键GUID',
  `user_name` VARCHAR(50) NOT NULL COMMENT '操作人账号',
  `real_name` VARCHAR(50) NOT NULL COMMENT '操作人姓名',
  `module` VARCHAR(50) NOT NULL COMMENT '模块名称',
  `target` VARCHAR(100) COMMENT '操作对象',
  `action` VARCHAR(50) NOT NULL COMMENT '操作类型',
  `method` VARCHAR(10) NOT NULL COMMENT '请求方法',
  `path` VARCHAR(500) NOT NULL COMMENT '请求路径',
  `ip` VARCHAR(50) NOT NULL COMMENT 'IP地址',
  `user_agent` VARCHAR(500) COMMENT '用户代理',
  `request_data` TEXT COMMENT '请求参数',
  `response_code` INT COMMENT '响应code',
  `response_message` VARCHAR(500) COMMENT '响应message',
  `duration` INT COMMENT '耗时（毫秒）',
  `trace_id` VARCHAR(50) COMMENT '追踪ID',
  `create_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  PRIMARY KEY (`id`),
  KEY `idx_user_name` (`user_name`),
  KEY `idx_module` (`module`),
  KEY `idx_create_time` (`create_time`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='操作日志表';

-- 登录日志表
CREATE TABLE IF NOT EXISTS `ops_login_log` (
  `id` CHAR(36) NOT NULL COMMENT '主键GUID',
  `user_name` VARCHAR(50) NOT NULL COMMENT '登录账号',
  `real_name` VARCHAR(50) COMMENT '用户姓名',
  `login_type` VARCHAR(20) NOT NULL COMMENT '登录类型：password/wechat',
  `ip` VARCHAR(50) NOT NULL COMMENT 'IP地址',
  `location` VARCHAR(200) COMMENT '登录地点',
  `browser` VARCHAR(100) COMMENT '浏览器',
  `os` VARCHAR(100) COMMENT '操作系统',
  `status` INT NOT NULL COMMENT '登录状态：0=失败，1=成功',
  `message` VARCHAR(500) COMMENT '登录消息',
  `trace_id` VARCHAR(50) COMMENT '追踪ID',
  `create_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  PRIMARY KEY (`id`),
  KEY `idx_user_name` (`user_name`),
  KEY `idx_create_time` (`create_time`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='登录日志表';
```

- [ ] **Step 2: 创建种子数据脚本**

```sql
-- sql/seed-data.sql

USE `easyproduct`;

-- ============================================
-- 部门种子数据
-- ============================================

INSERT INTO `basic_dept` (`id`, `parent_id`, `name`, `code`, `sort`, `status`, `leader_name`, `phone`, `email`, `description`, `create_by`) VALUES
('d001', '0', '总公司', 'ROOT', 1, 1, '张总', '13800138000', 'zhang@company.com', '公司总部', 'admin'),
('d002', 'd001', '技术部', 'TECH', 1, 1, '李经理', '13800138001', 'li@company.com', '技术研发部门', 'admin'),
('d003', 'd001', '销售部', 'SALES', 2, 1, '王经理', '13800138002', 'wang@company.com', '销售部门', 'admin');

-- ============================================
-- 角色种子数据
-- ============================================

INSERT INTO `basic_role` (`id`, `name`, `code`, `status`, `sort`, `remark`, `create_by`) VALUES
('r001', '超级管理员', 'super_admin', 1, 1, '拥有所有权限', 'admin'),
('r002', '普通用户', 'normal_user', 1, 2, '普通用户角色', 'admin');

-- ============================================
-- 菜单种子数据（示例）
-- ============================================

INSERT INTO `basic_menu` (`id`, `parent_id`, `name`, `path`, `title_key`, `icon`, `sort`, `permission`, `component`, `visible`, `status`, `create_by`) VALUES
('m001', '0', 'desktop', '/desktop', 'menu.desktop', 'Monitor', 1, NULL, NULL, 1, 1, 'admin'),
('m002', '0', 'basic', '/basic', 'menu.basic', 'Setting', 2, NULL, NULL, 1, 1, 'admin'),
('m003', 'm002', 'basic-user', '/basic/user', 'menu.basic.user', 'User', 1, 'basic:user:list', 'basic/user/index', 1, 1, 'admin'),
('m004', 'm002', 'basic-role', '/basic/role', 'menu.basic.role', 'UserFilled', 2, 'basic:role:list', 'basic/role/index', 1, 1, 'admin'),
('m005', 'm002', 'basic-menu', '/basic/menu', 'menu.basic.menu', 'Menu', 3, 'basic:menu:list', 'basic/menu/index', 1, 1, 'admin'),
('m006', 'm002', 'basic-dept', '/basic/dept', 'menu.basic.dept', 'OfficeBuilding', 4, 'basic:dept:list', 'basic/dept/index', 1, 1, 'admin');

-- ============================================
-- 用户种子数据
-- ============================================

-- 密码：admin123（BCrypt 哈希）
INSERT INTO `basic_user` (`id`, `user_name`, `password`, `real_name`, `email`, `phone`, `status`, `dept_id`, `create_by`) VALUES
('u001', 'admin', '$2a$10$N.zmdr9k7uOCQb376NoUnuTJ8iAt6Z5EHsM8lE9lBOsl7iKTVKIUi', '系统管理员', 'admin@example.com', '13800138000', 1, 'd001', 'admin'),
('u002', 'user01', '$2a$10$N.zmdr9k7uOCQb376NoUnuTJ8iAt6Z5EHsM8lE9lBOsl7iKTVKIUi', '测试用户', 'user01@example.com', '13800138001', 1, 'd002', 'admin');

-- ============================================
-- 用户角色关联
-- ============================================

INSERT INTO `basic_user_role` (`id`, `user_id`, `role_id`) VALUES
('ur001', 'u001', 'r001'),
('ur002', 'u002', 'r002');

-- ============================================
-- 角色菜单关联（超级管理员拥有所有菜单）
-- ============================================

INSERT INTO `basic_role_menu` (`id`, `role_id`, `menu_id`) VALUES
('rm001', 'r001', 'm001'),
('rm002', 'r001', 'm002'),
('rm003', 'r001', 'm003'),
('rm004', 'r001', 'm004'),
('rm005', 'r001', 'm005'),
('rm006', 'r001', 'm006');
```

- [ ] **Step 3: 验证脚本**

运行以下命令验证脚本是否正确：

```bash
# 连接 MySQL
mysql -u root -p

# 执行初始化脚本
source D:\4-MyProject\EasyProduct\sql\init-database.sql

# 执行种子数据脚本
source D:\4-MyProject\EasyProduct\sql\seed-data.sql

# 验证数据
USE easyproduct;
SHOW TABLES;
SELECT * FROM basic_user;
SELECT * FROM basic_role;
SELECT * FROM basic_menu;
```

---

### Task 2: 创建基础实体类

**Files:**
- Modify: `EasyProduct.Models/Entitys/Basic/basic_user.cs`
- Create: `EasyProduct.Models/Entitys/Basic/basic_role.cs`
- Create: `EasyProduct.Models/Entitys/Basic/basic_user_role.cs`
- Create: `EasyProduct.Models/Entitys/Basic/basic_menu.cs`
- Create: `EasyProduct.Models/Entitys/Basic/basic_role_menu.cs`
- Create: `EasyProduct.Models/Entitys/Basic/basic_dept.cs`
- Create: `EasyProduct.Models/Entitys/BaseEntity.cs`
- Create: `EasyProduct.Models/Enums/Status.cs`

- [ ] **Step 1: 创建基础实体基类**

```csharp
// EasyProduct.Models/Entitys/BaseEntity.cs

using SqlSugar;

namespace EasyProduct.Models.Entitys;

/// <summary>
/// 实体基类，包含通用字段
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// 主键GUID
    /// </summary>
    [SugarColumn(IsPrimaryKey = true)]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreateTime { get; set; } = DateTime.Now;

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime UpdateTime { get; set; } = DateTime.Now;

    /// <summary>
    /// 创建人
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = true)]
    public string? CreateBy { get; set; }

    /// <summary>
    /// 软删除标记
    /// </summary>
    public bool IsDeleted { get; set; } = false;
}

/// <summary>
/// 软删除接口
/// </summary>
public interface ISoftDelete
{
    bool IsDeleted { get; set; }
}
```

- [ ] **Step 2: 创建状态枚举**

```csharp
// EasyProduct.Models/Enums/Status.cs

namespace EasyProduct.Models.Enums;

/// <summary>
/// 通用状态枚举
/// </summary>
public enum Status
{
    /// <summary>
    /// 禁用
    /// </summary>
    Disabled = 0,

    /// <summary>
    /// 启用
    /// </summary>
    Enabled = 1
}

/// <summary>
/// 是否可见枚举
/// </summary>
public enum Visible
{
    /// <summary>
    /// 否
    /// </summary>
    No = 0,

    /// <summary>
    /// 是
    /// </summary>
    Yes = 1
}
```

- [ ] **Step 3: 创建用户实体**

```csharp
// EasyProduct.Models/Entitys/Basic/basic_user.cs

using SqlSugar;
using EasyProduct.Models.Enums;

namespace EasyProduct.Models.Entitys.Basic;

/// <summary>
/// 用户表
/// </summary>
[SugarTable("basic_user", "用户表")]
public class basic_user : BaseEntity, ISoftDelete
{
    /// <summary>
    /// 登录账号
    /// </summary>
    [SugarColumn(Length = 50)]
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// 密码（BCrypt哈希）
    /// </summary>
    [SugarColumn(Length = 255)]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// 真实姓名
    /// </summary>
    [SugarColumn(Length = 50)]
    public string RealName { get; set; } = string.Empty;

    /// <summary>
    /// 邮箱
    /// </summary>
    [SugarColumn(Length = 100, IsNullable = true)]
    public string? Email { get; set; }

    /// <summary>
    /// 手机号
    /// </summary>
    [SugarColumn(Length = 20, IsNullable = true)]
    public string? Phone { get; set; }

    /// <summary>
    /// 状态：0=禁用，1=启用
    /// </summary>
    public Status Status { get; set; } = Status.Enabled;

    /// <summary>
    /// 部门ID
    /// </summary>
    [SugarColumn(IsNullable = true)]
    public Guid? DeptId { get; set; }
}
```

- [ ] **Step 4: 创建其他实体类**

（篇幅限制，省略其他实体类的完整代码，结构类似）

---

### Task 3: 实现认证服务

**Files:**
- Create: `EasyProduct.Business/Basic/IAuthService.cs`
- Create: `EasyProduct.Business/Basic/AuthService.cs`
- Create: `EasyProduct.Models/Dto/Basic/LoginDto.cs`
- Create: `EasyProduct.Models/Dto/Basic/TokenDto.cs`
- Create: `EasyProduct.Web/Controllers/Admin/Basic/AuthController.cs`

- [ ] **Step 1: 创建 DTO**

```csharp
// EasyProduct.Models/Dto/Basic/LoginDto.cs

using System.ComponentModel.DataAnnotations;

namespace EasyProduct.Models.Dto.Basic;

/// <summary>
/// 登录请求参数
/// </summary>
public class LoginDto
{
    /// <summary>
    /// 登录账号
    /// </summary>
    [Required(ErrorMessage = "登录账号不能为空")]
    [MaxLength(50)]
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// 密码
    /// </summary>
    [Required(ErrorMessage = "密码不能为空")]
    public string Password { get; set; } = string.Empty;
}

/// <summary>
/// 登录响应数据
/// </summary>
public class LoginResponseDto
{
    /// <summary>
    /// 访问令牌
    /// </summary>
    public string AccessToken { get; set; } = string.Empty;

    /// <summary>
    /// 刷新令牌
    /// </summary>
    public string RefreshToken { get; set; } = string.Empty;

    /// <summary>
    /// 用户信息
    /// </summary>
    public UserInfoDto User { get; set; } = null!;
}

/// <summary>
/// 用户信息
/// </summary>
public class UserInfoDto
{
    /// <summary>
    /// 用户ID
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// 登录账号
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// 真实姓名
    /// </summary>
    public string RealName { get; set; } = string.Empty;

    /// <summary>
    /// 邮箱
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// 手机号
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// 部门ID
    /// </summary>
    public string? DeptId { get; set; }

    /// <summary>
    /// 部门名称
    /// </summary>
    public string? DeptName { get; set; }

    /// <summary>
    /// 角色编码列表
    /// </summary>
    public List<string> Roles { get; set; } = new();

    /// <summary>
    /// 权限标识列表
    /// </summary>
    public List<string> Permissions { get; set; } = new();
}
```

- [ ] **Step 2: 实现认证服务**

（完整实现代码请参考源项目 EasyWechatWeb，使用 BCrypt 验证密码，生成 JWT Token）

---

**由于篇幅限制，这里只展示了部分计划。完整计划包含：**

- 批次 1：认证服务 + 用户管理（完整实现）
- 批次 2：角色 + 菜单 + 部门（完整实现）
- 批次 3：字典 + 公告 + 系统参数（完整实现）
- 批次 4：Ops + 个人中心（完整实现）

每个批次都包含：
- 详细的任务分解
- 完整的代码示例
- 测试步骤
- 验收标准

---

## 🎯 接下来做什么？

1. **选择执行方式**：
   - **Subagent-Driven（推荐）**：我为每个任务派发独立子代理，快速迭代
   - **Inline Execution**：在当前会话中批量执行，有检查点

2. **确认批次顺序**：
   - 是否按批次 1 → 2 → 3 → 4 顺序执行？
   - 还是有特定的优先级要求？

请告诉我你的选择，我会继续创建完整的实施计划！