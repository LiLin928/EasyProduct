# Basic 模块接口文档

> 基础管理模块，包含认证、用户、角色、菜单、部门、字典、配置等核心功能

## 实体对象

### AdminUser - 管理员用户

```typescript
interface AdminUser {
  id: string                    // GUID 主键
  userName: string              // 用户名（登录账号）
  password: string              // 密码
  realName: string              // 真实姓名
  email: string                 // 邮箱
  phone: string                 // 手机号
  status: 0 | 1                 // 状态：0=禁用，1=启用
  deptId: string                // 部门ID
  roleIds: string[]             // 角色ID列表
  createdAt: string             // 创建时间（ISO 8601）
  updatedAt: string             // 更新时间（ISO 8601）
}
```

### Role - 角色

```typescript
interface Role {
  id: string                    // GUID 主键
  name: string                  // 角色名称
  code: string                  // 角色编码
  status: 0 | 1                 // 状态：0=禁用，1=启用
  sort: number                  // 排序
  remark?: string               // 备注
  menuIds?: string[]            // 菜单ID列表
  createdAt: string
  updatedAt: string
}
```

### Menu - 菜单

```typescript
interface Menu {
  id: string                    // GUID 主键
  parentId: string              // 父菜单ID（'0' 表示根节点）
  name: string                  // 菜单名称（路由name）
  path: string                  // 路由路径
  titleKey: string              // 标题i18n key
  icon: string                  // 图标名称
  sort: number                  // 排序
  status: 0 | 1                 // 状态：0=禁用，1=启用
  visible: 0 | 1                // 是否可见：0=否，1=是
  children?: Menu[]             // 子菜单
}
```

### Dept - 部门

```typescript
interface Dept {
  id: string                    // GUID 主键
  parentId: string              // 父部门ID
  name: string                  // 部门名称
  code: string                  // 部门编码
  sort: number                  // 排序
  status: 0 | 1                 // 状态：0=禁用，1=启用
  leaderName?: string           // 部门负责人
  phone?: string                // 联系电话
  email?: string                // 邮箱
  fullPath?: string             // 完整路径（如：总公司/技术部）
  level?: number                // 层级
  memberCount?: number          // 成员数量
  description?: string          // 描述
  children?: Dept[]             // 子部门
}
```

### DictType - 字典类型

```typescript
interface DictType {
  id: string                    // GUID 主键
  name: string                  // 字典类型名称
  code: string                  // 字典类型编码（唯一）
  status: 0 | 1                 // 状态：0=禁用，1=启用
  remark: string                // 备注
}
```

### DictData - 字典数据

```typescript
interface DictData {
  id: string                    // GUID 主键
  typeCode: string              // 字典类型编码
  value: string                 // 字典值
  labelKey: string              // 标签i18n key
  sort: number                  // 排序
  status: 0 | 1                 // 状态：0=禁用，1=启用
}
```

### SystemConfig - 系统配置

```typescript
interface SystemConfig {
  id: string                    // GUID 主键
  key: string                   // 配置键（唯一）
  label: string                 // 配置名称
  value: string                 // 配置值
  type: 'string' | 'number' | 'boolean' // 值类型
  remark: string                // 备注
  createdAt: string
  updatedAt: string
}
```

---

## API 接口

### 1. 认证相关（Auth）

#### POST /api/admin/auth/login
用户登录

**请求参数：**
```json
{
  "userName": "string",
  "password": "string"
}
```

**返回值：**
```json
{
  "code": 200,
  "message": "操作成功",
  "data": {
    "accessToken": "string",
    "refreshToken": "string",
    "user": {
      "id": "string",
      "userName": "string",
      "realName": "string"
    },
    "permissions": ["string"]
  },
  "timestamp": 1234567890
}
```

**错误情况：**
- 账号或密码错误：`{ code: 400, message: "账号或密码错误" }`

---

#### POST /api/admin/auth/refresh
刷新访问令牌

**请求参数：**
```json
{
  "refreshToken": "string"
}
```

**返回值：**
```json
{
  "code": 200,
  "data": {
    "accessToken": "string"
  }
}
```

---

#### GET /api/admin/auth/user-info
获取当前用户信息（用于刷新页面后恢复登录状态）

**返回值：**
```json
{
  "code": 200,
  "data": {
    "user": {
      "id": "string",
      "userName": "string",
      "realName": "string"
    },
    "permissions": ["string"]
  }
}
```

---

#### GET /api/admin/auth/menu-list
获取当前用户菜单树

**返回值：**
```json
{
  "code": 200,
  "data": [Menu] // 菜单树数组
}
```

---

### 2. 用户管理（User）

#### GET /api/admin/basic/user/list
获取用户列表（分页）

**查询参数：**
- `pageIndex` - 页码（默认 1）
- `pageSize` - 每页条数（默认 10）
- `userName` - 用户名（模糊搜索）
- `realName` - 真实姓名（模糊搜索）
- `status` - 状态筛选

**返回值：**
```json
{
  "code": 200,
  "data": {
    "list": [AdminUser],
    "total": 100,
    "pageIndex": 1,
    "pageSize": 10,
    "totalPages": 10,
    "hasNextPage": true,
    "hasPrevPage": false
  }
}
```

---

#### GET /api/admin/basic/user/:id
获取用户详情

**路径参数：**
- `id` - 用户ID

**返回值：**
```json
{
  "code": 200,
  "data": AdminUser
}
```

---

#### POST /api/admin/basic/user
创建用户

**请求参数：**
```json
{
  "userName": "string",
  "password": "string",
  "realName": "string",
  "email": "string",
  "phone": "string",
  "status": "enabled",
  "deptId": "string",
  "roleIds": ["string"]
}
```

**返回值：**
```json
{
  "code": 200,
  "message": "创建成功",
  "data": { "id": "string" }
}
```

---

#### PUT /api/admin/basic/user/:id
更新用户

**路径参数：**
- `id` - 用户ID

**请求参数：**
```json
{
  "realName": "string",
  "email": "string",
  "phone": "string",
  "status": "enabled",
  "deptId": "string",
  "roleIds": ["string"]
}
```

**返回值：**
```json
{
  "code": 200,
  "message": "更新成功"
}
```

---

#### DELETE /api/admin/basic/user/:id
删除用户

**路径参数：**
- `id` - 用户ID

**返回值：**
```json
{
  "code": 200,
  "message": "删除成功"
}
```

---

#### POST /api/admin/basic/user/:id/reset-password
重置用户密码

**路径参数：**
- `id` - 用户ID

**返回值：**
```json
{
  "code": 200,
  "message": "密码已重置为：123456"
}
```

---

### 3. 角色管理（Role）

#### GET /api/admin/basic/role/list
获取角色列表（分页）

**查询参数：**
- `pageIndex` - 页码
- `pageSize` - 每页条数
- `name` - 角色名称（模糊搜索）
- `code` - 角色编码（模糊搜索）
- `status` - 状态筛选

**返回值：**
```json
{
  "code": 200,
  "data": {
    "list": [Role],
    "total": 10
  }
}
```

---

#### GET /api/admin/basic/role/:id
获取角色详情

---

#### POST /api/admin/basic/role
创建角色

---

#### PUT /api/admin/basic/role/:id
更新角色

---

#### DELETE /api/admin/basic/role/:id
删除角色

---

#### GET /api/admin/basic/role/:id/menus
获取角色的菜单ID列表

**返回值：**
```json
{
  "code": 200,
  "data": ["menuId1", "menuId2"]
}
```

---

#### POST /api/admin/basic/role/:id/menus
分配菜单给角色

**请求参数：**
```json
{
  "menuIds": ["menuId1", "menuId2"]
}
```

---

### 4. 菜单管理（Menu）

#### GET /api/admin/basic/menu/tree
获取菜单树

**返回值：**
```json
{
  "code": 200,
  "data": [Menu] // 菜单树
}
```

---

#### GET /api/admin/basic/menu/:id
获取菜单详情

---

#### POST /api/admin/basic/menu
创建菜单

---

#### PUT /api/admin/basic/menu/:id
更新菜单

---

#### DELETE /api/admin/basic/menu/:id
删除菜单

---

#### POST /api/admin/basic/menu/sort
更新菜单排序

---

#### PUT /api/admin/basic/menu/:id/status
更新菜单状态

---

#### PUT /api/admin/basic/menu/:id/visible
更新菜单可见性

---

### 5. 部门管理（Dept）

#### GET /api/admin/basic/dept/tree
获取部门树

**返回值：**
```json
{
  "code": 200,
  "data": [Dept] // 部门树
}
```

---

#### GET /api/admin/basic/dept/:id
获取部门详情

---

#### GET /api/admin/basic/dept/:id/users
获取部门成员列表

**返回值：**
```json
{
  "code": 200,
  "data": [{
    "id": "string",
    "userName": "string",
    "realName": "string",
    "phone": "string",
    "email": "string",
    "status": "enabled",
    "roleNames": ["string"],
    "deptId": "string",
    "deptName": "string",
    "roleIds": ["string"],
    "createdAt": "string",
    "updatedAt": "string"
  }]
}
```

---

#### POST /api/admin/basic/dept
创建部门

---

#### PUT /api/admin/basic/dept/:id
更新部门

---

#### DELETE /api/admin/basic/dept/:id
删除部门

---

### 6. 字典管理（Dict）

#### GET /api/admin/basic/dict-type/list
获取字典类型列表（分页）

**查询参数：**
- `pageIndex` - 页码
- `pageSize` - 每页条数
- `name` - 类型名称
- `code` - 类型编码

---

#### POST /api/admin/basic/dict-type
创建字典类型

---

#### PUT /api/admin/basic/dict-type/:id
更新字典类型

---

#### DELETE /api/admin/basic/dict-type/:id
删除字典类型

---

#### GET /api/admin/basic/dict-data
按字典类型取字典项

**查询参数：**
- `typeCode` - 字典类型编码

**返回值：**
```json
{
  "code": 200,
  "data": [
    { "value": "enabled", "labelKey": "common.status.enabled" },
    { "value": "disabled", "labelKey": "common.status.disabled" }
  ]
}
```

---

#### GET /api/admin/basic/dict-data/list
获取字典数据列表（分页）

**查询参数：**
- `typeCode` - 字典类型编码
- `pageIndex` - 页码
- `pageSize` - 每页条数

---

#### POST /api/admin/basic/dict-data
创建字典数据

---

#### PUT /api/admin/basic/dict-data/:id
更新字典数据

---

#### DELETE /api/admin/basic/dict-data/:id
删除字典数据

---

#### POST /api/admin/basic/dict-data/batch-delete
批量删除字典数据

**请求参数：**
```json
{
  "ids": ["id1", "id2"]
}
```

---

### 7. 系统配置（Config）

#### GET /api/admin/basic/config/list
获取系统参数列表（分页）

**查询参数：**
- `pageIndex` - 页码
- `pageSize` - 每页条数
- `key` - 参数键
- `label` - 参数名称

---

#### POST /api/admin/basic/config
创建系统参数

**请求参数：**
```json
{
  "key": "string",
  "label": "string",
  "value": "string",
  "type": "string",
  "remark": "string"
}
```

---

#### PUT /api/admin/basic/config/:id
更新系统参数

---

#### DELETE /api/admin/basic/config/:id
删除系统参数

---

#### POST /api/admin/basic/config/batch-delete
批量删除系统参数

---

### 8. 个人中心（Profile）

#### GET /api/admin/basic/profile
获取当前用户信息

**返回值：**
```json
{
  "code": 200,
  "data": {
    "id": "string",
    "userName": "string",
    "realName": "string",
    "phone": "string",
    "email": "string",
    "avatar": "string",
    "roles": ["string"],
    "deptName": "string",
    "lastLoginTime": "string"
  }
}
```

---

#### PUT /api/admin/basic/profile
更新当前用户基本信息

**请求参数：**
```json
{
  "realName": "string",
  "phone": "string",
  "email": "string",
  "avatar": "string"
}
```

---

#### PUT /api/admin/basic/profile/password
修改密码

**请求参数：**
```json
{
  "oldPassword": "string",
  "newPassword": "string"
}
```

**错误情况：**
- 原密码不正确：`{ code: 400, message: "原密码不正确" }`
- 新密码长度不足：`{ code: 400, message: "新密码长度不能少于 6 位" }`

---

## 数据库表设计

### basic_user
```sql
CREATE TABLE basic_user (
  id VARCHAR(36) PRIMARY KEY,
  user_name VARCHAR(50) NOT NULL UNIQUE,
  password VARCHAR(255) NOT NULL,
  real_name VARCHAR(50) NOT NULL,
  email VARCHAR(100),
  phone VARCHAR(20),
  status INT DEFAULT 1 COMMENT '状态：0=禁用，1=启用',
  dept_id VARCHAR(36),
  created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
  updated_at DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  INDEX idx_dept_id (dept_id)
);
```

### basic_role
```sql
CREATE TABLE basic_role (
  id VARCHAR(36) PRIMARY KEY,
  name VARCHAR(50) NOT NULL,
  code VARCHAR(50) NOT NULL UNIQUE,
  status INT DEFAULT 1 COMMENT '状态：0=禁用，1=启用',
  sort INT DEFAULT 0,
  remark TEXT,
  created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
  updated_at DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
);
```

### basic_menu
```sql
CREATE TABLE basic_menu (
  id VARCHAR(36) PRIMARY KEY,
  parent_id VARCHAR(36) DEFAULT '0',
  name VARCHAR(50) NOT NULL,
  path VARCHAR(200),
  title_key VARCHAR(100) NOT NULL,
  icon VARCHAR(50),
  sort INT DEFAULT 0,
  status INT DEFAULT 1 COMMENT '状态：0=禁用，1=启用',
  visible INT DEFAULT 1 COMMENT '是否可见：0=否，1=是',
  INDEX idx_parent_id (parent_id)
);
```

### basic_dept
```sql
CREATE TABLE basic_dept (
  id VARCHAR(36) PRIMARY KEY,
  parent_id VARCHAR(36) DEFAULT '0',
  name VARCHAR(50) NOT NULL,
  code VARCHAR(50) NOT NULL UNIQUE,
  sort INT DEFAULT 0,
  status INT DEFAULT 1 COMMENT '状态：0=禁用，1=启用',
  leader_name VARCHAR(50),
  phone VARCHAR(20),
  email VARCHAR(100),
  full_path VARCHAR(500),
  level INT,
  member_count INT DEFAULT 0,
  description TEXT,
  INDEX idx_parent_id (parent_id)
);
```

### basic_dict_type
```sql
CREATE TABLE basic_dict_type (
  id VARCHAR(36) PRIMARY KEY,
  name VARCHAR(50) NOT NULL,
  code VARCHAR(50) NOT NULL UNIQUE,
  status INT DEFAULT 1 COMMENT '状态：0=禁用，1=启用',
  remark TEXT,
  created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
  updated_at DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
);
```

### basic_dict_data
```sql
CREATE TABLE basic_dict_data (
  id VARCHAR(36) PRIMARY KEY,
  type_code VARCHAR(50) NOT NULL,
  value VARCHAR(100) NOT NULL,
  label_key VARCHAR(100) NOT NULL,
  sort INT DEFAULT 0,
  status INT DEFAULT 1 COMMENT '状态：0=禁用，1=启用',
  INDEX idx_type_code (type_code),
  UNIQUE KEY uk_type_value (type_code, value)
);
```

### basic_config
```sql
CREATE TABLE basic_config (
  id VARCHAR(36) PRIMARY KEY,
  `key` VARCHAR(100) NOT NULL UNIQUE,
  label VARCHAR(100) NOT NULL,
  value TEXT,
  type VARCHAR(20) DEFAULT 'string',
  remark TEXT,
  created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
  updated_at DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
);
```

### basic_user_role
```sql
CREATE TABLE basic_user_role (
  user_id VARCHAR(36) NOT NULL,
  role_id VARCHAR(36) NOT NULL,
  PRIMARY KEY (user_id, role_id)
);
```

### basic_role_menu
```sql
CREATE TABLE basic_role_menu (
  role_id VARCHAR(36) NOT NULL,
  menu_id VARCHAR(36) NOT NULL,
  PRIMARY KEY (role_id, menu_id)
);
```

---

## 开发注意事项

1. **认证方式**: Admin JWT，需在请求头携带 `Authorization: Bearer {token}`
2. **权限控制**: 通过 `permissions` 数组控制接口访问权限
3. **菜单过滤**: 登录时根据用户角色过滤菜单
4. **字典使用**: 前端使用 `useDict` composable 获取字典数据
5. **状态字段**: 使用 int 类型（0=禁用，1=启用，详见 `docs/api/status-enum-reference.md`）
6. **布尔字段**: 使用 int 类型（0=否，1=是）
7. **密码安全**: 密码需加密存储（BCrypt）
8. **审计日志**: 用户操作需记录到操作日志表

---

生成时间: 2026-09-07