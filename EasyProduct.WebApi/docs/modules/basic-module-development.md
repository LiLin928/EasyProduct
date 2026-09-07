# Basic 模块后端开发说明文档

> **适用范围**：EasyProduct.WebApi 的 Basic 模块
> **创建时间**：2026-09-07
> **文档依据**：`docs/api/basic-module.md` 接口定义 + `docs/backend-guidelines.md` 后端规范

---

## 1. 模块概述

### 1.1 模块定位

Basic 模块是 EasyProduct 的基础管理模块，提供认证、用户、角色、菜单、部门、字典、配置等核心功能。

### 1.2 功能范围

| 功能 | 说明 | 路由前缀 |
|------|------|---------|
| 认证管理 | 登录、刷新Token、获取用户信息、菜单树 | `/api/admin/auth` |
| 用户管理 | 用户CRUD、重置密码 | `/api/admin/basic/user` |
| 角色管理 | 角色CRUD、权限分配 | `/api/admin/basic/role` |
| 菜单管理 | 菜单CRUD、排序、状态管理 | `/api/admin/basic/menu` |
| 部门管理 | 部门树CRUD、成员管理 | `/api/admin/basic/dept` |
| 字典管理 | 字典类型和数据CRUD | `/api/admin/basic/dict-type`、`/api/admin/basic/dict-data` |
| 系统配置 | 系统参数CRUD | `/api/admin/basic/config` |
| 个人中心 | 个人信息、修改密码 | `/api/admin/basic/profile` |

### 1.3 技术特点

- **认证方式**：Admin JWT（管理端）+ Member JWT（小程序会员）
- **权限控制**：RBAC（基于角色的访问控制）+ 菜单权限
- **数据隔离**：软删除 + 数据权限过滤
- **依赖注入**：Autofac 属性注入（参考现有实现）

---

## 2. 技术栈和依赖

### 2.1 核心技术栈

| 技术 | 版本 | 用途 |
|------|------|------|
| .NET | 8.0 LTS | 运行时 |
| SqlSugarCore | 5.1.4.x | ORM框架 |
| Autofac | 8.x/9.x | 依赖注入容器 |
| Mapster | 10.x | 对象映射 |
| BCrypt.Net-Next | 4.x | 密码加密 |
| Serilog | 8.x | 日志框架 |
| xUnit | 2.x | 单元测试框架 |

### 2.2 数据库依赖

- **数据库**：MySQL 8.0
- **字符集**：utf8mb4
- **表前缀**：`basic_`

### 2.3 NuGet 包列表

```xml
<!-- EasyProduct.Models.csproj -->
<PackageReference Include="SqlSugarCore" Version="5.1.4.*" />
<PackageReference Include="Mapster" Version="10.*" />

<!-- EasyProduct.Business.csproj -->
<PackageReference Include="Autofac" Version="8.*" />
<PackageReference Include="Autofac.Extensions.DependencyInjection" Version="8.*" />
<PackageReference Include="BCrypt.Net-Next" Version="4.*" />

<!-- EasyProduct.Web.csproj -->
<PackageReference Include="Serilog.AspNetCore" Version="8.*" />
<PackageReference Include="Swashbuckle.AspNetCore" Version="6.9.*" />
```

---

## 3. 实体类设计（Entity）

### 3.1 实体基类

**重要：所有实体类必须继承 `BaseEntity` 基类，统一管理通用字段。**

**文件路径**：`EasyProduct.Models/Entitys/Base/BaseEntity.cs`

**BaseEntity 包含的通用字段**：
- `Id` (Guid): GUID 主键
- `IsDeleted` (int): 软删除标记（0-未删除，1-已删除）
- `Status` (int): 状态字段（1-启用，0-禁用）
- `CreatedAt` (DateTime): 创建时间
- `UpdatedAt` (DateTime?): 更新时间
- `CreatedBy` (string?): 创建人ID
- `UpdatedBy` (string?): 更新人ID

**使用常量类**：
- `CommonStatus.Enabled` (1): 启用/正常
- `CommonStatus.Disabled` (0): 禁用/停用
- `DeleteStatus.NotDeleted` (0): 未删除
- `DeleteStatus.Deleted` (1): 已删除

**参考文件**：
- `EasyProduct.Models/Constants/CommonConstants.cs` - 状态常量定义
- `EasyProduct.Models/Entitys/Base/BaseEntity.cs` - 基类定义

**重要说明：**
- 所有实体类必须继承 `BaseEntity`，不允许独立定义 Id、IsDeleted、Status 等通用字段
- 状态字段统一使用 `int` 类型，使用 `CommonStatus.Enabled`/`CommonStatus.Disabled` 常量
- 删除标记统一使用 `int` 类型，使用 `DeleteStatus.NotDeleted`/`DeleteStatus.Deleted` 常量
- 时间字段命名统一为 `CreatedAt`/`UpdatedAt`（而非 CreateTime/UpdateTime）

### 3.2 用户实体（basic_user）

**文件路径**：`EasyProduct.Models/Entitys/Basic/basic_user.cs`

```csharp
using SqlSugar;
using EasyProduct.Models.Entitys.Base;

namespace EasyProduct.Models.Entitys.Basic;

/// <summary>
/// 用户实体
/// </summary>
/// <remarks>
/// 对应数据库表 basic_user
/// 继承 BaseEntity 基类，包含通用字段（Id、IsDeleted、Status、CreatedAt、UpdatedAt、CreatedBy、UpdatedBy）
/// </remarks>
[SugarTable("basic_user")]
public class basic_user : BaseEntity
{
    /// <summary>
    /// 用户名
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = false)]
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// 密码（BCrypt加密）
    /// </summary>
    [SugarColumn(Length = 255, IsNullable = false)]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// 真实姓名
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = true)]
    public string? RealName { get; set; }

    /// <summary>
    /// 手机号
    /// </summary>
    [SugarColumn(Length = 20, IsNullable = true)]
    public string? Phone { get; set; }

    /// <summary>
    /// 邮箱
    /// </summary>
    [SugarColumn(Length = 100, IsNullable = true)]
    public string? Email { get; set; }

    /// <summary>
    /// 头像URL
    /// </summary>
    [SugarColumn(Length = 500, IsNullable = true)]
    public string? Avatar { get; set; }
}
```

### 3.3 角色实体（basic_role）

**文件路径**：`EasyProduct.Models/Entitys/Basic/basic_role.cs`

```csharp
using SqlSugar;
using EasyProduct.Models.Entitys.Base;

namespace EasyProduct.Models.Entitys.Basic;

/// <summary>
/// 角色实体
/// </summary>
/// <remarks>
/// 对应数据库表 basic_role
/// 继承 BaseEntity 基类，包含通用字段（Id、IsDeleted、Status、CreatedAt、UpdatedAt、CreatedBy、UpdatedBy）
/// </remarks>
[SugarTable("basic_role")]
public class basic_role : BaseEntity
{
    /// <summary>
    /// 角色名称
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = false)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 角色编码（唯一）
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = false)]
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// 排序
    /// </summary>
    [SugarColumn(IsNullable = false)]
    public int Sort { get; set; } = 0;

    /// <summary>
    /// 备注
    /// </summary>
    [SugarColumn(ColumnDataType = "text", IsNullable = true)]
    public string? Remark { get; set; }
}
```

### 3.4 菜单实体（basic_menu）

**文件路径**：`EasyProduct.Models/Entitys/Basic/basic_menu.cs`

```csharp
using SqlSugar;
using EasyProduct.Models.Entitys.Base;

namespace EasyProduct.Models.Entitys.Basic;

/// <summary>
/// 菜单实体
/// </summary>
/// <remarks>
/// 对应数据库表 basic_menu
/// 继承 BaseEntity 基类，包含通用字段（Id、IsDeleted、Status、CreatedAt、UpdatedAt、CreatedBy、UpdatedBy）
/// </remarks>
[SugarTable("basic_menu")]
public class basic_menu : BaseEntity
{
    /// <summary>
    /// 父菜单ID（'0' 表示根节点）
    /// </summary>
    [SugarColumn(Length = 36, IsNullable = false)]
    public string ParentId { get; set; } = "0";

    /// <summary>
    /// 菜单名称（路由name）
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = false)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 路由路径
    /// </summary>
    [SugarColumn(Length = 200, IsNullable = true)]
    public string? Path { get; set; }

    /// <summary>
    /// 标题i18n key
    /// </summary>
    [SugarColumn(Length = 100, IsNullable = false)]
    public string TitleKey { get; set; } = string.Empty;

    /// <summary>
    /// 图标名称
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = true)]
    public string? Icon { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    [SugarColumn(IsNullable = false)]
    public int Sort { get; set; } = 0;

    /// <summary>
    /// 是否可见
    /// </summary>
    [SugarColumn(IsNullable = false)]
    public bool Visible { get; set; } = true;
}
```

### 3.5 部门实体（basic_dept）

**文件路径**：`EasyProduct.Models/Entitys/Basic/basic_dept.cs`

```csharp
using SqlSugar;
using EasyProduct.Models.Entitys.Base;

namespace EasyProduct.Models.Entitys.Basic;

/// <summary>
/// 部门实体
/// </summary>
/// <remarks>
/// 对应数据库表 basic_dept
/// 继承 BaseEntity 基类，包含通用字段（Id、IsDeleted、Status、CreatedAt、UpdatedAt、CreatedBy、UpdatedBy）
/// </remarks>
[SugarTable("basic_dept")]
public class basic_dept : BaseEntity
{
    /// <summary>
    /// 父部门ID
    /// </summary>
    [SugarColumn(Length = 36, IsNullable = false)]
    public string ParentId { get; set; } = "0";

    /// <summary>
    /// 部门名称
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = false)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 部门编码（唯一）
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = false)]
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// 排序
    /// </summary>
    [SugarColumn(IsNullable = false)]
    public int Sort { get; set; } = 0;

    /// <summary>
    /// 部门负责人
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = true)]
    public string? LeaderName { get; set; }

    /// <summary>
    /// 联系电话
    /// </summary>
    [SugarColumn(Length = 20, IsNullable = true)]
    public string? Phone { get; set; }

    /// <summary>
    /// 邮箱
    /// </summary>
    [SugarColumn(Length = 100, IsNullable = true)]
    public string? Email { get; set; }

    /// <summary>
    /// 完整路径（如：总公司/技术部）
    /// </summary>
    [SugarColumn(Length = 500, IsNullable = true)]
    public string? FullPath { get; set; }

    /// <summary>
    /// 层级
    /// </summary>
    [SugarColumn(IsNullable = true)]
    public int? Level { get; set; }

    /// <summary>
    /// 成员数量
    /// </summary>
    [SugarColumn(IsNullable = false)]
    public int MemberCount { get; set; } = 0;

    /// <summary>
    /// 描述
    /// </summary>
    [SugarColumn(ColumnDataType = "text", IsNullable = true)]
    public string? Description { get; set; }
}
```

### 3.6 字典类型实体（basic_dict_type）

**文件路径**：`EasyProduct.Models/Entitys/Basic/basic_dict_type.cs`

```csharp
using SqlSugar;
using EasyProduct.Models.Entitys.Base;

namespace EasyProduct.Models.Entitys.Basic;

/// <summary>
/// 字典类型实体
/// </summary>
/// <remarks>
/// 对应数据库表 basic_dict_type
/// 继承 BaseEntity 基类，包含通用字段（Id、IsDeleted、Status、CreatedAt、UpdatedAt、CreatedBy、UpdatedBy）
/// </remarks>
[SugarTable("basic_dict_type")]
public class basic_dict_type : BaseEntity
{
    /// <summary>
    /// 字典类型名称
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = false)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 字典类型编码（唯一）
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = false)]
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// 备注
    /// </summary>
    [SugarColumn(ColumnDataType = "text", IsNullable = true)]
    public string? Remark { get; set; }
}
```

### 3.7 字典数据实体（basic_dict_data）

**文件路径**：`EasyProduct.Models/Entitys/Basic/basic_dict_data.cs`

```csharp
using SqlSugar;
using EasyProduct.Models.Entitys.Base;

namespace EasyProduct.Models.Entitys.Basic;

/// <summary>
/// 字典数据实体
/// </summary>
/// <remarks>
/// 对应数据库表 basic_dict_data
/// 继承 BaseEntity 基类，包含通用字段（Id、IsDeleted、Status、CreatedAt、UpdatedAt、CreatedBy、UpdatedBy）
/// </remarks>
[SugarTable("basic_dict_data")]
public class basic_dict_data : BaseEntity
{
    /// <summary>
    /// 字典类型编码
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = false)]
    public string TypeCode { get; set; } = string.Empty;

    /// <summary>
    /// 字典值
    /// </summary>
    [SugarColumn(Length = 100, IsNullable = false)]
    public string Value { get; set; } = string.Empty;

    /// <summary>
    /// 标签i18n key
    /// </summary>
    [SugarColumn(Length = 100, IsNullable = false)]
    public string LabelKey { get; set; } = string.Empty;

    /// <summary>
    /// 排序
    /// </summary>
    [SugarColumn(IsNullable = false)]
    public int Sort { get; set; } = 0;
}
```

### 3.8 系统配置实体（basic_config）

**文件路径**：`EasyProduct.Models/Entitys/Basic/basic_config.cs`

```csharp
using SqlSugar;
using EasyProduct.Models.Entitys.Base;

namespace EasyProduct.Models.Entitys.Basic;

/// <summary>
/// 系统配置实体
/// </summary>
/// <remarks>
/// 对应数据库表 basic_config
/// 继承 BaseEntity 基类，包含通用字段（Id、IsDeleted、Status、CreatedAt、UpdatedAt、CreatedBy、UpdatedBy）
/// </remarks>
[SugarTable("basic_config")]
public class basic_config : BaseEntity
{
    /// <summary>
    /// 配置键（唯一）
    /// </summary>
    [SugarColumn(Length = 100, IsNullable = false)]
    public string Key { get; set; } = string.Empty;

    /// <summary>
    /// 配置名称
    /// </summary>
    [SugarColumn(Length = 100, IsNullable = false)]
    public string Label { get; set; } = string.Empty;

    /// <summary>
    /// 配置值
    /// </summary>
    [SugarColumn(ColumnDataType = "text", IsNullable = true)]
    public string? Value { get; set; }

    /// <summary>
    /// 值类型：string/number/boolean
    /// </summary>
    [SugarColumn(Length = 20, IsNullable = false)]
    public string Type { get; set; } = "string";

    /// <summary>
    /// 备注
    /// </summary>
    [SugarColumn(ColumnDataType = "text", IsNullable = true)]
    public string? Remark { get; set; }
}
```

### 3.9 关联表实体

**用户角色关联表（basic_user_role）**

**文件路径**：`EasyProduct.Models/Entitys/Basic/basic_user_role.cs`

```csharp
using SqlSugar;

namespace EasyProduct.Models.Entitys.Basic;

/// <summary>
/// 用户角色关联实体
/// </summary>
/// <remarks>
/// 多对多关系表，用户与角色的关联
/// </remarks>
[SugarTable("basic_user_role")]
public class basic_user_role
{
    /// <summary>
    /// 用户ID
    /// </summary>
    [SugarColumn(IsPrimaryKey = true)]
    public Guid UserId { get; set; }

    /// <summary>
    /// 角色ID
    /// </summary>
    [SugarColumn(IsPrimaryKey = true)]
    public Guid RoleId { get; set; }
}
```

**角色菜单关联表（basic_role_menu）**

**文件路径**：`EasyProduct.Models/Entitys/Basic/basic_role_menu.cs`

```csharp
using SqlSugar;

namespace EasyProduct.Models.Entitys.Basic;

/// <summary>
/// 角色菜单关联实体
/// </summary>
/// <remarks>
/// 多对多关系表，角色与菜单的关联
/// </remarks>
[SugarTable("basic_role_menu")]
public class basic_role_menu
{
    /// <summary>
    /// 角色ID
    /// </summary>
    [SugarColumn(IsPrimaryKey = true)]
    public Guid RoleId { get; set; }

    /// <summary>
    /// 菜单ID
    /// </summary>
    [SugarColumn(IsPrimaryKey = true)]
    public Guid MenuId { get; set; }
}
```

---

## 4. DTO 设计（Request/Response）

### 4.1 用户 DTO

**文件路径**：`EasyProduct.Models/Dto/Basic/UserDto.cs`

```csharp
namespace EasyProduct.Models.Dto.Basic;

/// <summary>
/// 用户 DTO
/// </summary>
public class UserDto
{
    /// <summary>
    /// 用户ID
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// 用户名
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// 真实姓名
    /// </summary>
    public string? RealName { get; set; }

    /// <summary>
    /// 邮箱
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// 手机号
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// 状态（int 类型：1-启用，0-禁用）
    /// </summary>
    public int Status { get; set; } = 1;

    /// <summary>
    /// 部门ID
    /// </summary>
    public string? DeptId { get; set; }

    /// <summary>
    /// 角色ID列表
    /// </summary>
    public List<string> RoleIds { get; set; } = new();

    /// <summary>
    /// 创建时间（ISO 8601）
    /// </summary>
    public string CreatedAt { get; set; } = string.Empty;

    /// <summary>
    /// 更新时间（ISO 8601）
    /// </summary>
    public string UpdatedAt { get; set; } = string.Empty;
}

/// <summary>
/// 创建用户请求 DTO
/// </summary>
public class CreateUserDto
{
    /// <summary>
    /// 用户名
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// 密码
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// 真实姓名
    /// </summary>
    public string? RealName { get; set; }

    /// <summary>
    /// 邮箱
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// 手机号
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// 状态（int 类型：1-启用，0-禁用）
    /// </summary>
    public int Status { get; set; } = 1;

    /// <summary>
    /// 部门ID
    /// </summary>
    public string? DeptId { get; set; }

    /// <summary>
    /// 角色ID列表
    /// </summary>
    public List<string> RoleIds { get; set; } = new();
}

/// <summary>
/// 更新用户请求 DTO
/// </summary>
public class UpdateUserDto
{
    /// <summary>
    /// 用户ID
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// 真实姓名
    /// </summary>
    public string? RealName { get; set; }

    /// <summary>
    /// 邮箱
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// 手机号
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    public string? Status { get; set; }

    /// <summary>
    /// 部门ID
    /// </summary>
    public string? DeptId { get; set; }

    /// <summary>
    /// 角色ID列表
    /// </summary>
    public List<string>? RoleIds { get; set; }
}

/// <summary>
/// 用户查询 DTO
/// </summary>
public class UserQueryDto
{
    /// <summary>
    /// 页码，从 1 开始
    /// </summary>
    public int PageIndex { get; set; } = 1;

    /// <summary>
    /// 每页数量
    /// </summary>
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// 用户名（模糊搜索）
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// 真实姓名（模糊搜索）
    /// </summary>
    public string? RealName { get; set; }

    /// <summary>
    /// 状态筛选（int 类型：1-启用，0-禁用）
    /// </summary>
    public int? Status { get; set; }
}
```

### 4.2 角色 DTO

**文件路径**：`EasyProduct.Models/Dto/Basic/RoleDto.cs`

```csharp
namespace EasyProduct.Models.Dto.Basic;

/// <summary>
/// 角色 DTO
/// </summary>
public class RoleDto
{
    /// <summary>
    /// 角色ID
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// 角色名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 角色编码
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// 状态（int 类型：1-启用，0-禁用）
    /// </summary>
    public int Status { get; set; } = 1;

    /// <summary>
    /// 排序
    /// </summary>
    public int Sort { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    public string? Remark { get; set; }

    /// <summary>
    /// 菜单ID列表
    /// </summary>
    public List<string>? MenuIds { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public string CreatedAt { get; set; } = string.Empty;

    /// <summary>
    /// 更新时间
    /// </summary>
    public string UpdatedAt { get; set; } = string.Empty;
}

/// <summary>
/// 创建角色请求 DTO
/// </summary>
public class CreateRoleDto
{
    /// <summary>
    /// 角色名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 角色编码
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// 状态（int 类型：1-启用，0-禁用）
    /// </summary>
    public int Status { get; set; } = 1;

    /// <summary>
    /// 排序
    /// </summary>
    public int Sort { get; set; } = 0;

    /// <summary>
    /// 备注
    /// </summary>
    public string? Remark { get; set; }
}

/// <summary>
/// 更新角色请求 DTO
/// </summary>
public class UpdateRoleDto
{
    /// <summary>
    /// 角色ID
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// 角色名称
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// 状态（int 类型：1-启用，0-禁用）
    /// </summary>
    public int? Status { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    public int? Sort { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    public string? Remark { get; set; }
}

/// <summary>
/// 角色查询 DTO
/// </summary>
public class RoleQueryDto
{
    /// <summary>
    /// 页码，从 1 开始
    /// </summary>
    public int PageIndex { get; set; } = 1;

    /// <summary>
    /// 每页数量
    /// </summary>
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// 角色名称（模糊搜索）
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// 角色编码（模糊搜索）
    /// </summary>
    public string? Code { get; set; }

    /// <summary>
    /// 状态筛选（int 类型：1-启用，0-禁用）
    /// </summary>
    public int? Status { get; set; }
}

/// <summary>
/// 分配菜单给角色请求 DTO
/// </summary>
public class AssignMenuDto
{
    /// <summary>
    /// 菜单ID列表
    /// </summary>
    public List<string> MenuIds { get; set; } = new();
}
```

### 4.3 认证 DTO

**文件路径**：`EasyProduct.Models/Dto/Basic/AuthDto.cs`

```csharp
namespace EasyProduct.Models.Dto.Basic;

/// <summary>
/// 登录请求 DTO
/// </summary>
public class LoginDto
{
    /// <summary>
    /// 用户名
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// 密码
    /// </summary>
    public string Password { get; set; } = string.Empty;
}

/// <summary>
/// 登录响应 DTO
/// </summary>
public class LoginResultDto
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
    public UserInfoDto User { get; set; } = new();

    /// <summary>
    /// 权限列表
    /// </summary>
    public List<string> Permissions { get; set; } = new();
}

/// <summary>
/// 用户信息 DTO
/// </summary>
public class UserInfoDto
{
    /// <summary>
    /// 用户ID
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// 用户名
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// 真实姓名
    /// </summary>
    public string? RealName { get; set; }
}

/// <summary>
/// 刷新Token请求 DTO
/// </summary>
public class RefreshTokenDto
{
    /// <summary>
    /// 刷新令牌
    /// </summary>
    public string RefreshToken { get; set; } = string.Empty;
}

/// <summary>
/// 刷新Token响应 DTO
/// </summary>
public class RefreshTokenResultDto
{
    /// <summary>
    /// 访问令牌
    /// </summary>
    public string AccessToken { get; set; } = string.Empty;
}
```

---

## 5. Service 层实现指南

### 5.1 Service 接口定义规范

**命名规范**：
- 接口名：`I{Entity}Service`
- 实现类名：`{Entity}Service`
- 文件位置：`EasyProduct.Business/Basic/`

**依赖注入方式**：使用 Autofac 属性注入（继承 BaseService）

### 5.2 Service 接口示例

**文件路径**：`EasyProduct.Business/Basic/IUserService.cs`

```csharp
using EasyProduct.Common.Base;
using EasyProduct.Models.Dto.Basic;

namespace EasyProduct.Business.Basic;

/// <summary>
/// 用户服务接口
/// </summary>
/// <remarks>
/// 提供用户的 CRUD 操作和业务逻辑方法
/// </remarks>
public interface IUserService
{
    /// <summary>
    /// 获取用户分页列表（支持多条件查询）
    /// </summary>
    /// <param name="query">查询参数，包含分页、用户名、真实姓名、状态等</param>
    /// <returns>用户分页列表</returns>
    Task<PageResponse<UserDto>> GetPageListAsync(UserQueryDto query);

    /// <summary>
    /// 根据ID获取用户详情
    /// </summary>
    /// <param name="id">用户ID（GUID字符串）</param>
    /// <returns>用户信息，不存在则返回 null</returns>
    Task<UserDto?> GetByIdAsync(string id);

    /// <summary>
    /// 创建用户
    /// </summary>
    /// <param name="dto">创建用户参数</param>
    /// <returns>创建成功返回新用户ID</returns>
    /// <exception cref="BusinessException">用户名已存在时抛出异常</exception>
    Task<string> CreateAsync(CreateUserDto dto);

    /// <summary>
    /// 更新用户
    /// </summary>
    /// <param name="dto">更新用户参数</param>
    /// <returns>更新成功返回 true</returns>
    /// <exception cref="BusinessException">用户不存在时抛出异常</exception>
    Task<bool> UpdateAsync(UpdateUserDto dto);

    /// <summary>
    /// 删除用户（软删除）
    /// </summary>
    /// <param name="id">用户ID（GUID字符串）</param>
    /// <returns>删除成功返回 true</returns>
    Task<bool> DeleteAsync(string id);

    /// <summary>
    /// 重置用户密码
    /// </summary>
    /// <param name="id">用户ID（GUID字符串）</param>
    /// <returns>重置成功返回默认密码</returns>
    /// <exception cref="BusinessException">用户不存在时抛出异常</exception>
    Task<string> ResetPasswordAsync(string id);
}
```

### 5.3 Service 实现示例

**文件路径**：`EasyProduct.Business/Basic/UserService.cs`

```csharp
using EasyProduct.Common.Base;
using EasyProduct.Common.Error;
using EasyProduct.Models.Dto.Basic;
using EasyProduct.Models.Entitys.Basic;
using EasyProduct.Models.Constants;
using Mapster;
using SqlSugar;

namespace EasyProduct.Business.Basic;

/// <summary>
/// 用户服务实现
/// </summary>
/// <remarks>
/// 继承 BaseService&lt;basic_user&gt;，提供用户管理的业务逻辑实现。
/// 数据库上下文 _db 通过 Autofac 属性注入自动赋值。
/// </remarks>
public class UserService : BaseService<basic_user>, IUserService
{
    /// <summary>
    /// 分页查询用户列表（支持多条件查询和排序）
    /// </summary>
    /// <param name="query">查询参数，包含分页、用户名、真实姓名、状态等</param>
    /// <returns>用户分页列表</returns>
    /// <remarks>
    /// 1. 支持按用户名、真实姓名模糊搜索
    /// 2. 支持按状态筛选（使用 CommonStatus 常量）
    /// 3. 默认按创建时间倒序排列
    /// 4. 自动过滤已删除数据（使用 DeleteStatus 常量）
    /// </remarks>
    public async Task<PageResponse<UserDto>> GetPageListAsync(UserQueryDto query)
    {
        var whereExpr = Expressionable.Create<basic_user>()
            .AndIF(!string.IsNullOrEmpty(query.UserName),
                u => u.UserName.Contains(query.UserName!))
            .AndIF(!string.IsNullOrEmpty(query.RealName),
                u => u.RealName!.Contains(query.RealName!))
            .AndIF(query.Status.HasValue,
                u => u.Status == query.Status!.Value)
            .And(u => u.IsDeleted == DeleteStatus.NotDeleted)
            .ToExpression();

        var result = await GetPageListAsync(
            query.PageIndex,
            query.PageSize,
            whereExpr,
            u => u.CreatedAt,
            isAsc: false);

        // 使用 Mapster 进行对象映射
        var dtoList = result.List.Adapt<List<UserDto>>();

        // 转换时间格式为 ISO 8601
        dtoList.ForEach(dto =>
        {
            dto.CreatedAt = result.List.First(u => u.Id == Guid.Parse(dto.Id)).CreatedAt.ToString("o");
            dto.UpdatedAt = result.List.First(u => u.Id == Guid.Parse(dto.Id)).UpdatedAt?.ToString("o") ?? string.Empty;
        });

        return PageResponse<UserDto>.Create(dtoList, result.Total, result.PageIndex, result.PageSize);
    }

    /// <summary>
    /// 根据ID获取用户详情
    /// </summary>
    /// <param name="id">用户ID（GUID字符串）</param>
    /// <returns>用户信息，不存在则返回 null</returns>
    /// <remarks>
    /// 1. 自动过滤已删除数据（使用 DeleteStatus 常量）
    /// 2. 包含用户的角色ID列表
    /// </remarks>
    public async Task<UserDto?> GetByIdAsync(string id)
    {
        var guid = Guid.Parse(id);
        var entity = await GetByIdAsync(guid);

        if (entity == null || entity.IsDeleted == DeleteStatus.Deleted)
            return null;

        var dto = entity.Adapt<UserDto>();
        dto.CreatedAt = entity.CreatedAt.ToString("o");
        dto.UpdatedAt = entity.UpdatedAt?.ToString("o") ?? string.Empty;

        // 查询用户的角色ID列表
        var roleIds = await _db.Queryable<basic_user_role>()
            .Where(ur => ur.UserId == guid)
            .Select(ur => ur.RoleId.ToString())
            .ToListAsync();

        dto.RoleIds = roleIds;

        return dto;
    }

    /// <summary>
    /// 创建用户
    /// </summary>
    /// <param name="dto">创建用户参数</param>
    /// <returns>创建成功返回新用户ID</returns>
    /// <exception cref="BusinessException">用户名已存在时抛出异常</exception>
    /// <remarks>
    /// 1. 检查用户名唯一性
    /// 2. 使用 BCrypt 加密密码
    /// 3. 自动设置创建时间和更新时间（由 BaseEntity 管理）
    /// 4. 保存用户角色关联关系
    /// 5. 使用 CommonStatus 常量设置状态
    /// </remarks>
    public async Task<string> CreateAsync(CreateUserDto dto)
    {
        // 检查用户名是否已存在
        if (await ExistsAsync(u => u.UserName == dto.UserName && u.IsDeleted == DeleteStatus.NotDeleted))
        {
            throw new BusinessException("用户名已存在", 400);
        }

        var entity = dto.Adapt<basic_user>();
        entity.Id = Guid.NewGuid();
        entity.Password = BCrypt.Net.BCrypt.HashPassword(dto.Password);
        entity.Status = dto.Status;
        entity.IsDeleted = DeleteStatus.NotDeleted;

        // 保存用户
        await InsertAsync(entity);

        // 保存用户角色关联
        if (dto.RoleIds != null && dto.RoleIds.Any())
        {
            var userRoles = dto.RoleIds
                .Select(roleId => new basic_user_role
                {
                    UserId = entity.Id,
                    RoleId = Guid.Parse(roleId)
                })
                .ToList();

            await _db.Insertable(userRoles).ExecuteCommandAsync();
        }

        return entity.Id.ToString();
    }

    /// <summary>
    /// 更新用户
    /// </summary>
    /// <param name="dto">更新用户参数</param>
    /// <returns>更新成功返回 true</returns>
    /// <exception cref="BusinessException">用户不存在时抛出异常</exception>
    /// <remarks>
    /// 1. 不允许修改用户名和密码
    /// 2. 自动更新更新时间（由 BaseEntity 管理）
    /// 3. 更新用户角色关联关系（先删后增）
    /// 4. 使用 CommonStatus 常量更新状态
    /// </remarks>
    public async Task<bool> UpdateAsync(UpdateUserDto dto)
    {
        var guid = Guid.Parse(dto.Id);
        var entity = await GetByIdAsync(guid);

        if (entity == null || entity.IsDeleted == DeleteStatus.Deleted)
        {
            throw new BusinessException("用户不存在", 404);
        }

        // 更新字段
        if (!string.IsNullOrEmpty(dto.RealName))
            entity.RealName = dto.RealName;
        if (!string.IsNullOrEmpty(dto.Phone))
            entity.Phone = dto.Phone;
        if (!string.IsNullOrEmpty(dto.Email))
            entity.Email = dto.Email;
        if (dto.Status.HasValue)
            entity.Status = dto.Status.Value;

        entity.UpdatedAt = DateTime.UtcNow;

        // 更新用户
        await UpdateAsync(entity);

        // 更新用户角色关联（先删后增）
        if (dto.RoleIds != null)
        {
            await _db.Deleteable<basic_user_role>()
                .Where(ur => ur.UserId == guid)
                .ExecuteCommandAsync();

            if (dto.RoleIds.Any())
            {
                var userRoles = dto.RoleIds
                    .Select(roleId => new basic_user_role
                    {
                        UserId = guid,
                        RoleId = Guid.Parse(roleId)
                    })
                    .ToList();

                await _db.Insertable(userRoles).ExecuteCommandAsync();
            }
        }

        return true;
    }

    /// <summary>
    /// 删除用户（软删除）
    /// </summary>
    /// <param name="id">用户ID（GUID字符串）</param>
    /// <returns>删除成功返回 true</returns>
    /// <remarks>
    /// 1. 软删除，设置 IsDeleted = DeleteStatus.Deleted
    /// 2. 自动更新状态为 CommonStatus.Disabled
    /// 3. 自动更新更新时间（由 BaseEntity 管理）
    /// </remarks>
    public async Task<bool> DeleteAsync(string id)
    {
        var guid = Guid.Parse(id);
        var entity = await GetByIdAsync(guid);

        if (entity == null || entity.IsDeleted == DeleteStatus.Deleted)
            return false;

        entity.IsDeleted = DeleteStatus.Deleted;
        entity.UpdatedAt = DateTime.UtcNow;
        entity.Status = CommonStatus.Disabled;

        return await UpdateAsync(entity) > 0;
    }

    /// <summary>
    /// 重置用户密码
    /// </summary>
    /// <param name="id">用户ID（GUID字符串）</param>
    /// <returns>重置成功返回默认密码</returns>
    /// <exception cref="BusinessException">用户不存在时抛出异常</exception>
    /// <remarks>
    /// 1. 将密码重置为默认值 "123456"
    /// 2. 使用 BCrypt 加密密码
    /// 3. 自动更新更新时间（由 BaseEntity 管理）
    /// </remarks>
    public async Task<string> ResetPasswordAsync(string id)
    {
        var guid = Guid.Parse(id);
        var entity = await GetByIdAsync(guid);

        if (entity == null || entity.IsDeleted == DeleteStatus.Deleted)
        {
            throw new BusinessException("用户不存在", 404);
        }

        var defaultPassword = "123456";
        entity.Password = BCrypt.Net.BCrypt.HashPassword(defaultPassword);
        entity.UpdatedAt = DateTime.UtcNow;

        await UpdateAsync(entity);

        return defaultPassword;
    }
}
```

### 5.4 Service 方法注释规范（强制）

**所有 Service 方法必须添加中文注释**，包括：

1. **summary**：功能简述（一句话说明方法用途）
2. **param**：参数说明
3. **returns**：返回值说明
4. **remarks**（可选）：详细说明、业务逻辑、注意事项
5. **exception**（可选）：可能抛出的异常

**示例：**

```csharp
/// <summary>
/// 创建用户
/// </summary>
/// <param name="dto">创建用户参数</param>
/// <returns>创建成功返回新用户ID</returns>
/// <exception cref="BusinessException">用户名已存在时抛出异常</exception>
/// <remarks>
/// 1. 检查用户名唯一性
/// 2. 使用 BCrypt 加密密码
/// 3. 自动设置创建时间和更新时间
/// 4. 保存用户角色关联关系
/// </remarks>
public async Task<string> CreateAsync(CreateUserDto dto)
{
    // 实现代码...
}
```

---

## 6. Controller 层实现指南

### 6.1 Controller 基类

**文件路径**：`EasyProduct.Web/Controllers/Admin/Base/AdminControllerBase.cs`

```csharp
using EasyProduct.Common.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyProduct.Web.Controllers.Admin.Base;

/// <summary>
/// 管理端控制器基类
/// </summary>
/// <remarks>
/// 所有管理端 Controller 继承此基类，统一配置：
/// 1. 路由前缀：/api/admin/{module}/{resource}
/// 2. 认证方案：AdminJwt
/// 3. 统一响应格式
/// </remarks>
[ApiController]
[Route("api/admin/basic")]
[Authorize(AuthenticationSchemes = "AdminJwt")]
public abstract class AdminControllerBase : BaseController
{
    // 提供管理端特定的辅助方法
}
```

### 6.2 Controller 实现示例

**文件路径**：`EasyProduct.Web/Controllers/Admin/Basic/UserController.cs`

```csharp
using EasyProduct.Common.Base;
using EasyProduct.Models.Dto.Basic;
using EasyProduct.Business.Basic;
using EasyProduct.Web.Controllers.Admin.Base;
using Microsoft.AspNetCore.Mvc;

namespace EasyProduct.Web.Controllers.Admin.Basic;

/// <summary>
/// 用户管理控制器
/// </summary>
/// <remarks>
/// 提供用户管理的 CRUD 接口，路由前缀：/api/admin/basic/user
/// </remarks>
[Route("user")]
public class UserController : AdminControllerBase
{
    /// <summary>
    /// 用户服务接口（属性注入）
    /// </summary>
    /// <remarks>
    /// 通过 Autofac 属性注入自动赋值，无需构造函数
    /// </remarks>
    public IUserService _userService { get; set; } = null!;

    /// <summary>
    /// 获取用户分页列表
    /// </summary>
    /// <param name="query">查询参数，包含分页、用户名、真实姓名、状态等</param>
    /// <returns>用户分页列表</returns>
    /// <remarks>
    /// GET /api/admin/basic/user/list
    /// </remarks>
    [HttpGet("list")]
    public async Task<ApiResponse<PageResponse<UserDto>>> GetList([FromQuery] UserQueryDto query)
    {
        var result = await _userService.GetPageListAsync(query);
        return Success(result);
    }

    /// <summary>
    /// 根据ID获取用户详情
    /// </summary>
    /// <param name="id">用户ID（GUID字符串）</param>
    /// <returns>用户信息</returns>
    /// <remarks>
    /// GET /api/admin/basic/user/{id}
    /// </remarks>
    [HttpGet("{id}")]
    public async Task<ApiResponse<UserDto?>> GetById(string id)
    {
        var result = await _userService.GetByIdAsync(id);
        return Success(result);
    }

    /// <summary>
    /// 创建用户
    /// </summary>
    /// <param name="dto">创建用户参数</param>
    /// <returns>创建结果，成功返回新用户ID</returns>
    /// <remarks>
    /// POST /api/admin/basic/user
    /// </remarks>
    [HttpPost]
    public async Task<ApiResponse<object>> Create([FromBody] CreateUserDto dto)
    {
        var id = await _userService.CreateAsync(dto);
        return Success(new { id }, "创建成功");
    }

    /// <summary>
    /// 更新用户
    /// </summary>
    /// <param name="id">用户ID（GUID字符串）</param>
    /// <param name="dto">更新用户参数</param>
    /// <returns>更新结果</returns>
    /// <remarks>
    /// PUT /api/admin/basic/user/{id}
    /// </remarks>
    [HttpPut("{id}")]
    public async Task<ApiResponse<object>> Update(string id, [FromBody] UpdateUserDto dto)
    {
        dto.Id = id; // 确保ID一致
        var result = await _userService.UpdateAsync(dto);
        return result ? Success("更新成功") : Error<object>("更新失败");
    }

    /// <summary>
    /// 删除用户
    /// </summary>
    /// <param name="id">用户ID（GUID字符串）</param>
    /// <returns>删除结果</returns>
    /// <remarks>
    /// DELETE /api/admin/basic/user/{id}
    /// </remarks>
    [HttpDelete("{id}")]
    public async Task<ApiResponse<object>> Delete(string id)
    {
        var result = await _userService.DeleteAsync(id);
        return result ? Success("删除成功") : Error<object>("删除失败");
    }

    /// <summary>
    /// 重置用户密码
    /// </summary>
    /// <param name="id">用户ID（GUID字符串）</param>
    /// <returns>重置成功返回默认密码</returns>
    /// <remarks>
    /// POST /api/admin/basic/user/{id}/reset-password
    /// </remarks>
    [HttpPost("{id}/reset-password")]
    public async Task<ApiResponse<object>> ResetPassword(string id)
    {
        var defaultPassword = await _userService.ResetPasswordAsync(id);
        return Success(new { password = defaultPassword }, $"密码已重置为：{defaultPassword}");
    }
}
```

### 6.3 Controller 方法注释规范（强制）

**所有 Controller 方法必须添加中文注释**，包括：

1. **summary**：功能简述（一句话说明方法用途）
2. **param**：参数说明
3. **returns**：返回值说明
4. **remarks**：HTTP 方法和路由路径

**示例：**

```csharp
/// <summary>
/// 创建用户
/// </summary>
/// <param name="dto">创建用户参数</param>
/// <returns>创建结果，成功返回新用户ID</returns>
/// <remarks>
/// POST /api/admin/basic/user
/// </remarks>
[HttpPost]
public async Task<ApiResponse<object>> Create([FromBody] CreateUserDto dto)
{
    // 实现代码...
}
```

### 6.4 Controller 规范要点

1. **禁止业务逻辑**：Controller 只负责收参、调用 Service、返回结果
2. **禁止 try/catch**：异常由全局异常中间件统一处理
3. **使用属性注入**：通过 `public IService _service { get; set; } = null!;` 注入
4. **统一响应格式**：使用 `Success()`、`Error()` 方法返回统一信封
5. **路由规范**：遵循 RESTful 风格，资源名用小写连字符单数名词

---

## 7. 数据库表设计（SQL）

### 7.1 用户表（basic_user）

```sql
CREATE TABLE `basic_user` (
  `id` VARCHAR(36) NOT NULL COMMENT '用户ID，GUID主键',
  `user_name` VARCHAR(50) NOT NULL COMMENT '用户名',
  `password` VARCHAR(255) NOT NULL COMMENT '密码（BCrypt加密）',
  `real_name` VARCHAR(50) DEFAULT NULL COMMENT '真实姓名',
  `email` VARCHAR(100) DEFAULT NULL COMMENT '邮箱',
  `phone` VARCHAR(20) DEFAULT NULL COMMENT '手机号',
  `avatar` VARCHAR(500) DEFAULT NULL COMMENT '头像URL',
  `status` INT NOT NULL DEFAULT 1 COMMENT '状态：1-启用，0-禁用（使用 CommonStatus 常量）',
  `is_deleted` INT NOT NULL DEFAULT 0 COMMENT '是否删除：0-未删除，1-已删除（使用 DeleteStatus 常量）',
  `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `updated_at` DATETIME DEFAULT NULL COMMENT '更新时间',
  `created_by` VARCHAR(36) DEFAULT NULL COMMENT '创建人ID',
  `updated_by` VARCHAR(36) DEFAULT NULL COMMENT '更新人ID',
  PRIMARY KEY (`id`),
  UNIQUE KEY `uk_user_name` (`user_name`),
  INDEX `idx_status` (`status`),
  INDEX `idx_is_deleted` (`is_deleted`),
  INDEX `idx_created_at` (`created_at`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='用户表';
```

### 7.2 角色表（basic_role）

```sql
CREATE TABLE `basic_role` (
  `id` VARCHAR(36) NOT NULL COMMENT '角色ID，GUID主键',
  `name` VARCHAR(50) NOT NULL COMMENT '角色名称',
  `code` VARCHAR(50) NOT NULL COMMENT '角色编码',
  `status` INT NOT NULL DEFAULT 1 COMMENT '状态：1-启用，0-禁用（使用 CommonStatus 常量）',
  `sort` INT NOT NULL DEFAULT 0 COMMENT '排序',
  `remark` TEXT DEFAULT NULL COMMENT '备注',
  `is_deleted` INT NOT NULL DEFAULT 0 COMMENT '是否删除：0-未删除，1-已删除（使用 DeleteStatus 常量）',
  `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `updated_at` DATETIME DEFAULT NULL COMMENT '更新时间',
  `created_by` VARCHAR(36) DEFAULT NULL COMMENT '创建人ID',
  `updated_by` VARCHAR(36) DEFAULT NULL COMMENT '更新人ID',
  PRIMARY KEY (`id`),
  UNIQUE KEY `uk_code` (`code`),
  INDEX `idx_status` (`status`),
  INDEX `idx_is_deleted` (`is_deleted`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='角色表';
```

### 7.3 菜单表（basic_menu）

```sql
CREATE TABLE `basic_menu` (
  `id` VARCHAR(36) NOT NULL COMMENT '菜单ID，GUID主键',
  `parent_id` VARCHAR(36) NOT NULL DEFAULT '0' COMMENT '父菜单ID（0表示根节点）',
  `name` VARCHAR(50) NOT NULL COMMENT '菜单名称（路由name）',
  `path` VARCHAR(200) DEFAULT NULL COMMENT '路由路径',
  `title_key` VARCHAR(100) NOT NULL COMMENT '标题i18n key',
  `icon` VARCHAR(50) DEFAULT NULL COMMENT '图标名称',
  `sort` INT NOT NULL DEFAULT 0 COMMENT '排序',
  `status` INT NOT NULL DEFAULT 1 COMMENT '状态：1-启用，0-禁用（使用 CommonStatus 常量）',
  `visible` TINYINT(1) NOT NULL DEFAULT 1 COMMENT '是否可见',
  `is_deleted` INT NOT NULL DEFAULT 0 COMMENT '是否删除：0-未删除，1-已删除（使用 DeleteStatus 常量）',
  `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `updated_at` DATETIME DEFAULT NULL COMMENT '更新时间',
  `created_by` VARCHAR(36) DEFAULT NULL COMMENT '创建人ID',
  `updated_by` VARCHAR(36) DEFAULT NULL COMMENT '更新人ID',
  PRIMARY KEY (`id`),
  INDEX `idx_parent_id` (`parent_id`),
  INDEX `idx_status` (`status`),
  INDEX `idx_is_deleted` (`is_deleted`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='菜单表';
```

### 7.4 部门表（basic_dept）

```sql
CREATE TABLE `basic_dept` (
  `id` VARCHAR(36) NOT NULL COMMENT '部门ID，GUID主键',
  `parent_id` VARCHAR(36) NOT NULL DEFAULT '0' COMMENT '父部门ID',
  `name` VARCHAR(50) NOT NULL COMMENT '部门名称',
  `code` VARCHAR(50) NOT NULL COMMENT '部门编码',
  `sort` INT NOT NULL DEFAULT 0 COMMENT '排序',
  `status` INT NOT NULL DEFAULT 1 COMMENT '状态：1-启用，0-禁用（使用 CommonStatus 常量）',
  `leader_name` VARCHAR(50) DEFAULT NULL COMMENT '部门负责人',
  `phone` VARCHAR(20) DEFAULT NULL COMMENT '联系电话',
  `email` VARCHAR(100) DEFAULT NULL COMMENT '邮箱',
  `full_path` VARCHAR(500) DEFAULT NULL COMMENT '完整路径（如：总公司/技术部）',
  `level` INT DEFAULT NULL COMMENT '层级',
  `member_count` INT NOT NULL DEFAULT 0 COMMENT '成员数量',
  `description` TEXT DEFAULT NULL COMMENT '描述',
  `is_deleted` INT NOT NULL DEFAULT 0 COMMENT '是否删除：0-未删除，1-已删除（使用 DeleteStatus 常量）',
  `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `updated_at` DATETIME DEFAULT NULL COMMENT '更新时间',
  `created_by` VARCHAR(36) DEFAULT NULL COMMENT '创建人ID',
  `updated_by` VARCHAR(36) DEFAULT NULL COMMENT '更新人ID',
  PRIMARY KEY (`id`),
  UNIQUE KEY `uk_code` (`code`),
  INDEX `idx_parent_id` (`parent_id`),
  INDEX `idx_status` (`status`),
  INDEX `idx_is_deleted` (`is_deleted`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='部门表';
```

### 7.5 字典类型表（basic_dict_type）

```sql
CREATE TABLE `basic_dict_type` (
  `id` VARCHAR(36) NOT NULL COMMENT '字典类型ID，GUID主键',
  `name` VARCHAR(50) NOT NULL COMMENT '字典类型名称',
  `code` VARCHAR(50) NOT NULL COMMENT '字典类型编码',
  `status` INT NOT NULL DEFAULT 1 COMMENT '状态：1-启用，0-禁用（使用 CommonStatus 常量）',
  `remark` TEXT DEFAULT NULL COMMENT '备注',
  `is_deleted` INT NOT NULL DEFAULT 0 COMMENT '是否删除：0-未删除，1-已删除（使用 DeleteStatus 常量）',
  `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `updated_at` DATETIME DEFAULT NULL COMMENT '更新时间',
  `created_by` VARCHAR(36) DEFAULT NULL COMMENT '创建人ID',
  `updated_by` VARCHAR(36) DEFAULT NULL COMMENT '更新人ID',
  PRIMARY KEY (`id`),
  UNIQUE KEY `uk_code` (`code`),
  INDEX `idx_status` (`status`),
  INDEX `idx_is_deleted` (`is_deleted`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='字典类型表';
```

### 7.6 字典数据表（basic_dict_data）

```sql
CREATE TABLE `basic_dict_data` (
  `id` VARCHAR(36) NOT NULL COMMENT '字典数据ID，GUID主键',
  `type_code` VARCHAR(50) NOT NULL COMMENT '字典类型编码',
  `value` VARCHAR(100) NOT NULL COMMENT '字典值',
  `label_key` VARCHAR(100) NOT NULL COMMENT '标签i18n key',
  `sort` INT NOT NULL DEFAULT 0 COMMENT '排序',
  `status` INT NOT NULL DEFAULT 1 COMMENT '状态：1-启用，0-禁用（使用 CommonStatus 常量）',
  `is_deleted` INT NOT NULL DEFAULT 0 COMMENT '是否删除：0-未删除，1-已删除（使用 DeleteStatus 常量）',
  `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `updated_at` DATETIME DEFAULT NULL COMMENT '更新时间',
  `created_by` VARCHAR(36) DEFAULT NULL COMMENT '创建人ID',
  `updated_by` VARCHAR(36) DEFAULT NULL COMMENT '更新人ID',
  PRIMARY KEY (`id`),
  UNIQUE KEY `uk_type_value` (`type_code`, `value`),
  INDEX `idx_type_code` (`type_code`),
  INDEX `idx_status` (`status`),
  INDEX `idx_is_deleted` (`is_deleted`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='字典数据表';
```

### 7.7 系统配置表（basic_config）

```sql
CREATE TABLE `basic_config` (
  `id` VARCHAR(36) NOT NULL COMMENT '配置ID，GUID主键',
  `key` VARCHAR(100) NOT NULL COMMENT '配置键',
  `label` VARCHAR(100) NOT NULL COMMENT '配置名称',
  `value` TEXT DEFAULT NULL COMMENT '配置值',
  `type` VARCHAR(20) NOT NULL DEFAULT 'string' COMMENT '值类型：string/number/boolean',
  `remark` TEXT DEFAULT NULL COMMENT '备注',
  `status` INT NOT NULL DEFAULT 1 COMMENT '状态：1-启用，0-禁用（使用 CommonStatus 常量）',
  `is_deleted` INT NOT NULL DEFAULT 0 COMMENT '是否删除：0-未删除，1-已删除（使用 DeleteStatus 常量）',
  `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `updated_at` DATETIME DEFAULT NULL COMMENT '更新时间',
  `created_by` VARCHAR(36) DEFAULT NULL COMMENT '创建人ID',
  `updated_by` VARCHAR(36) DEFAULT NULL COMMENT '更新人ID',
  PRIMARY KEY (`id`),
  UNIQUE KEY `uk_key` (`key`),
  INDEX `idx_status` (`status`),
  INDEX `idx_is_deleted` (`is_deleted`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='系统配置表';
```

### 7.8 用户角色关联表（basic_user_role）

```sql
CREATE TABLE `basic_user_role` (
  `user_id` VARCHAR(36) NOT NULL COMMENT '用户ID',
  `role_id` VARCHAR(36) NOT NULL COMMENT '角色ID',
  PRIMARY KEY (`user_id`, `role_id`),
  INDEX `idx_role_id` (`role_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='用户角色关联表';
```

### 7.9 角色菜单关联表（basic_role_menu）

```sql
CREATE TABLE `basic_role_menu` (
  `role_id` VARCHAR(36) NOT NULL COMMENT '角色ID',
  `menu_id` VARCHAR(36) NOT NULL COMMENT '菜单ID',
  PRIMARY KEY (`role_id`, `menu_id`),
  INDEX `idx_menu_id` (`menu_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='角色菜单关联表';
```

---

## 8. 业务逻辑要点

### 8.1 认证与授权

#### 8.1.1 登录流程

1. 接收用户名和密码
2. 查询用户，验证密码（BCrypt 验证）
3. 查询用户角色和菜单权限
4. 生成 JWT Token（accessToken + refreshToken）
5. 返回用户信息、Token、权限列表

#### 8.1.2 权限验证

- 使用 `[Permission("module:resource:action")]` 特性标注接口
- 权限标识格式：`basic:user:create`、`basic:user:update`
- 超级管理员短路放行

### 8.2 用户管理

#### 8.2.1 用户名唯一性

- 创建时检查用户名是否已存在
- 用户名不允许修改

#### 8.2.2 密码管理

- 使用 BCrypt 加密存储
- 重置密码为默认值 "123456"
- 密码长度不少于 6 位

#### 8.2.3 软删除

- 删除时设置 `is_deleted = DeleteStatus.Deleted` (1)
- 自动更新状态为 `CommonStatus.Disabled` (0)
- 查询时过滤 `is_deleted = DeleteStatus.NotDeleted` (0)
- 使用常量类，避免硬编码

### 8.3 角色管理

#### 8.3.1 角色编码唯一性

- 创建时检查角色编码是否已存在
- 角色编码不允许修改

#### 8.3.2 角色权限分配

- 分配菜单给角色（`basic_role_menu` 表）
- 更新角色菜单时先删后增

### 8.4 菜单管理

#### 8.4.1 菜单树构建

- 递归构建菜单树
- 过滤已禁用和已删除菜单
- 按排序字段排序

#### 8.4.2 菜单权限

- 每个菜单可配置权限标识
- 用户登录时根据角色过滤菜单

### 8.5 部门管理

#### 8.5.1 部门树构建

- 递归构建部门树
- 自动计算层级和完整路径
- 统计成员数量

#### 8.5.2 部门成员管理

- 查询部门下的所有用户
- 包含用户的角色信息

### 8.6 字典管理

#### 8.6.1 字典缓存

- 字典数据优先缓存
- 字典变更时主动失效缓存

#### 8.6.2 字典使用

- 前端使用 `useDict` composable 获取字典
- 字典值使用小写字符串常量

### 8.7 系统配置

#### 8.7.1 配置缓存

- 配置数据优先缓存
- 配置变更时主动失效缓存

#### 8.7.2 配置类型

- 支持 string、number、boolean 三种类型
- 前端根据类型解析配置值

---

## 9. 单元测试要求

### 9.1 测试范围

**强制测试**：
1. 认证逻辑（登录、刷新Token）
2. 密码加密和验证
3. 权限验证逻辑

**建议测试**：
1. 用户 CRUD 操作
2. 角色权限分配
3. 菜单树构建
4. 部门树构建
5. 字典数据管理

### 9.2 测试项目结构

```
EasyProduct.Tests/
├─ Basic/
│  ├─ UserServiceTests.cs
│  ├─ RoleServiceTests.cs
│  ├─ AuthServiceTests.cs
│  └─ MenuServiceTests.cs
```

### 9.3 测试示例

```csharp
using Xunit;
using EasyProduct.Business.Basic;
using EasyProduct.Models.Dto.Basic;
using EasyProduct.Common.Error;

namespace EasyProduct.Tests.Basic;

/// <summary>
/// 用户服务测试
/// </summary>
public class UserServiceTests
{
    /// <summary>
    /// 创建用户_用户名已存在_抛出异常
    /// </summary>
    [Fact]
    public async Task CreateAsync_UserNameExists_ThrowBusinessException()
    {
        // Arrange
        var service = new UserService();
        var dto = new CreateUserDto { UserName = "admin", Password = "123456" };

        // Act & Assert
        await Assert.ThrowsAsync<BusinessException>(() => service.CreateAsync(dto));
    }

    /// <summary>
    /// 创建用户_成功_返回用户ID
    /// </summary>
    [Fact]
    public async Task CreateAsync_Success_ReturnUserId()
    {
        // Arrange
        var service = new UserService();
        var dto = new CreateUserDto
        {
            UserName = "testuser",
            Password = "123456",
            RealName = "测试用户"
        };

        // Act
        var id = await service.CreateAsync(dto);

        // Assert
        Assert.NotNull(id);
        Assert.True(Guid.TryParse(id, out _));
    }

    /// <summary>
    /// 重置密码_用户不存在_抛出异常
    /// </summary>
    [Fact]
    public async Task ResetPasswordAsync_UserNotFound_ThrowBusinessException()
    {
        // Arrange
        var service = new UserService();
        var userId = Guid.NewGuid().ToString();

        // Act & Assert
        await Assert.ThrowsAsync<BusinessException>(() => service.ResetPasswordAsync(userId));
    }
}
```

### 9.4 测试命名规范

- 方法名：`{方法名}_{场景}_{期望结果}`
- 使用 given-when-then 分段注释

---

## 10. 开发检查清单

### 10.1 代码规范检查

- [ ] 所有方法添加中文注释（summary、param、returns、remarks）
- [ ] 使用属性注入（`public IService _service { get; set; } = null!;`）
- [ ] Service 继承 `BaseService` 或 `BaseService<T>`
- [ ] Controller 继承 `AdminControllerBase`
- [ ] Controller 禁止 try/catch
- [ ] DTO 字段添加中文注释
- [ ] 实体类添加 `[SugarTable]` 特性和中文注释

### 10.2 接口规范检查

- [ ] 路由遵循 RESTful 风格
- [ ] 使用统一响应格式（`ApiResponse<T>`）
- [ ] HTTP 状态码一律返回 200
- [ ] 分页参数使用 `pageIndex` 和 `pageSize`
- [ ] 软删除过滤（`IsDeleted = false`）

### 10.3 数据库检查

- [ ] 表名使用 `basic_` 前缀
- [ ] 主键使用 GUID（VARCHAR(36)）
- [ ] **状态字段使用 INT 类型**（1-启用，0-禁用，使用 CommonStatus 常量）
- [ ] **删除标记使用 INT 类型**（0-未删除，1-已删除，使用 DeleteStatus 常量）
- [ ] 时间字段使用 `datetime` 类型
- [ ] 包含 BaseEntity 基类所有字段（Id、IsDeleted、Status、CreatedAt、UpdatedAt、CreatedBy、UpdatedBy）
- [ ] 添加必要的索引

### 10.4 安全检查

- [ ] 密码使用 BCrypt 加密
- [ ] 敏感信息（密码、Token）不入库
- [ ] 接口添加认证特性（`[Authorize]`）
- [ ] 权限标识正确配置

### 10.5 性能检查

- [ ] 分页查询使用 `ToPageListAsync`
- [ ] 字典数据使用缓存
- [ ] 避免 N+1 查询
- [ ] 使用索引优化查询

### 10.6 提交前检查

- [ ] `dotnet build` 零错误零警告
- [ ] `dotnet test` 全部通过
- [ ] 代码格式化（Ctrl+K+D）
- [ ] 移除调试代码
- [ ] 更新 Swagger 文档

---

## 附录：参考资源

### A. 相关文档

- [后端开发规范](../../backend-guidelines.md)
- [前端开发规范](../../frontend-guidelines.md)
- [Basic 模块接口文档](../../api/basic-module.md)

### B. 示例代码位置

- UserService：`EasyProduct.Business/Basic/UserService.cs`
- UserController：`EasyProduct.Web/Controllers/Admin/Basic/UserController.cs`
- BaseService：`EasyProduct.Common/Base/BaseService.cs`

### C. 常用工具类

- `BusinessException`：业务异常类
- `ApiResponse`：统一响应封装
- `PageResponse`：分页响应封装
- `Expressionable`：SqlSugar 表达式构建器

---

**文档版本**：v1.1
**最后更新**：2026-09-07
**维护者**：EasyProduct 后端团队

## 版本更新记录

### v1.1 (2026-09-07)
- 应用新的设计规范：所有实体继承 BaseEntity 基类
- 状态字段使用 int 类型（CommonStatus 常量）
- 删除标记使用 int 类型（DeleteStatus 常量）
- 更新数据库表设计，添加 BaseEntity 相关字段
- 更新 Service 层示例代码，使用常量类
- 更新开发检查清单

### v1.0 (2026-09-07)
- 初始版本