# Basic 模块 API 文档

> 基础管理模块，包含用户、角色、部门、菜单、字典等基础功能

---

## 实体定义

### AdminUser（用户）

| 字段名 | 类型 | 说明 | 备注 |
|--------|------|------|------|
| id | string | 用户ID | GUID |
| userName | string | 用户名 | 唯一 |
| password | string | 密码 | BCrypt加密 |
| realName | string | 真实姓名 | - |
| email | string | 邮箱 | - |
| phone | string | 手机号 | - |
| status | string | 状态 | enabled/disabled |
| deptId | string | 部门ID | - |
| roleIds | string[] | 角色ID列表 | - |
| createdAt | string | 创建时间 | ISO 8601 |
| updatedAt | string | 更新时间 | ISO 8601 |

### Role（角色）

| 字段名 | 类型 | 说明 | 备注 |
|--------|------|------|------|
| id | string | 角色ID | GUID |
| name | string | 角色名称 | - |
| code | string | 角色编码 | 唯一 |
| menuIds | string[] | 菜单ID列表 | - |
| status | string | 状态 | enabled/disabled |
| sort | number | 排序 | - |
| remark | string | 备注 | - |
| createdAt | string | 创建时间 | ISO 8601 |
| updatedAt | string | 更新时间 | ISO 8601 |

### Dept（部门）

| 字段名 | 类型 | 说明 | 备注 |
|--------|------|------|------|
| id | string | 部门ID | GUID |
| parentId | string | 父部门ID | '0'为根 |
| name | string | 部门名称 | - |
| code | string | 部门编码 | 唯一 |
| sort | number | 排序 | - |
| status | string | 状态 | enabled/disabled |
| leaderName | string | 负责人姓名 | 可选 |
| phone | string | 联系电话 | 可选 |
| email | string | 邮箱 | 可选 |
| fullPath | string | 完整路径 | 如：总公司/技术部 |
| level | number | 层级 | 从1开始 |
| memberCount | number | 成员数量 | - |
| description | string | 描述 | 可选 |
| children | Dept[] | 子部门 | 树形结构 |

### Menu（菜单）

| 字段名 | 类型 | 说明 | 备注 |
|--------|------|------|------|
| id | string | 菜单ID | GUID |
| parentId | string | 父菜单ID | '0'为根 |
| name | string | 菜单标识 | 路由名 |
| path | string | 菜单路径 | - |
| titleKey | string | 标题国际化key | - |
| icon | string | 图标 | Element Plus 图标 |
| sort | number | 排序 | - |
| status | string | 状态 | enabled/disabled |
| visible | boolean | 是否可见 | - |
| children | Menu[] | 子菜单 | 树形结构 |

### DictType（字典类型）

| 字段名 | 类型 | 说明 | 备注 |
|--------|------|------|------|
| id | string | 类型ID | GUID |
| name | string | 类型名称 | - |
| code | string | 类型编码 | 唯一 |
| status | string | 状态 | enabled/disabled |
| remark | string | 备注 | - |

### DictData（字典数据）

| 字段名 | 类型 | 说明 | 备注 |
|--------|------|------|------|
| id | string | 数据ID | GUID |
| typeCode | string | 字典类型编码 | - |
| value | string | 字典值 | - |
| labelKey | string | 标签国际化key | - |
| sort | number | 排序 | - |
| status | string | 状态 | enabled/disabled |

---

## API 接口

### 认证管理

#### 用户登录
- **URL**: /api/admin/auth/login
- **Method**: POST
- **描述**: 用户登录获取 Token
- **请求参数**:
  | 参数名 | 类型 | 必填 | 说明 |
  |--------|------|------|------|
  | userName | string | 是 | 用户名 |
  | password | string | 是 | 密码 |
- **响应结构**: ApiResponse<{ accessToken, refreshToken, user, permissions }>

#### 刷新 Token
- **URL**: /api/admin/auth/refresh
- **Method**: POST
- **描述**: 使用 refreshToken 获取新的 accessToken
- **请求参数**:
  | 参数名 | 类型 | 必填 | 说明 |
  |--------|------|------|------|
  | refreshToken | string | 是 | 刷新令牌 |
- **响应结构**: ApiResponse<{ accessToken }>

#### 获取当前用户信息
- **URL**: /api/admin/auth/user-info
- **Method**: GET
- **描述**: 获取当前登录用户信息
- **请求参数**: 无
- **响应结构**: ApiResponse<{ user, permissions }>

#### 获取用户菜单树
- **URL**: /api/admin/auth/menu-list
- **Method**: GET
- **描述**: 获取当前用户菜单树（根据权限过滤）
- **请求参数**: 无
- **响应结构**: ApiResponse<Menu[]>

### 用户管理

#### 获取用户列表
- **URL**: /api/admin/basic/user/list
- **Method**: GET
- **描述**: 分页查询用户列表
- **请求参数**:
  | 参数名 | 类型 | 必填 | 说明 |
  |--------|------|------|------|
  | pageIndex | number | 否 | 页码，默认1 |
  | pageSize | number | 否 | 每页数量，默认10 |
  | userName | string | 否 | 用户名搜索 |
  | realName | string | 否 | 真实姓名搜索 |
  | status | string | 否 | 状态筛选 |
- **响应结构**: ApiResponse<PageResponse<AdminUser>>

#### 获取用户详情
- **URL**: /api/admin/basic/user/{id}
- **Method**: GET
- **描述**: 根据ID获取用户详情
- **请求参数**:
  | 参数名 | 类型 | 必填 | 说明 |
  |--------|------|------|------|
  | id | string | 是 | 用户ID |
- **响应结构**: ApiResponse<AdminUser>

#### 创建用户
- **URL**: /api/admin/basic/user
- **Method**: POST
- **描述**: 创建新用户
- **请求参数**: AdminUser (JSON Body)
- **响应结构**: ApiResponse<{ id }>

#### 更新用户
- **URL**: /api/admin/basic/user/{id}
- **Method**: PUT
- **描述**: 更新用户信息
- **请求参数**:
  | 参数名 | 类型 | 必填 | 说明 |
  |--------|------|------|------|
  | id | string | 是 | 用户ID |
  | body | AdminUser | 是 | 用户数据 |
- **响应结构**: ApiResponse<null>

#### 删除用户
- **URL**: /api/admin/basic/user/{id}
- **Method**: DELETE
- **描述**: 删除用户
- **请求参数**:
  | 参数名 | 类型 | 必填 | 说明 |
  |--------|------|------|------|
  | id | string | 是 | 用户ID |
- **响应结构**: ApiResponse<null>

#### 重置密码
- **URL**: /api/admin/basic/user/{id}/reset-password
- **Method**: POST
- **描述**: 重置用户密码为默认密码
- **请求参数**:
  | 参数名 | 类型 | 必填 | 说明 |
  |--------|------|------|------|
  | id | string | 是 | 用户ID |
- **响应结构**: ApiResponse<null>

### 角色管理

#### 获取角色列表
- **URL**: /api/admin/basic/role/list
- **Method**: GET
- **描述**: 分页查询角色列表
- **请求参数**:
  | 参数名 | 类型 | 必填 | 说明 |
  |--------|------|------|------|
  | pageIndex | number | 否 | 页码，默认1 |
  | pageSize | number | 否 | 每页数量，默认10 |
- **响应结构**: ApiResponse<PageResponse<Role>>

#### 获取角色详情
- **URL**: /api/admin/basic/role/{id}
- **Method**: GET
- **描述**: 根据ID获取角色详情
- **请求参数**:
  | 参数名 | 类型 | 必填 | 说明 |
  |--------|------|------|------|
  | id | string | 是 | 角色ID |
- **响应结构**: ApiResponse<Role>

#### 创建角色
- **URL**: /api/admin/basic/role
- **Method**: POST
- **描述**: 创建新角色
- **请求参数**: Role (JSON Body)
- **响应结构**: ApiResponse<{ id }>

#### 更新角色
- **URL**: /api/admin/basic/role/{id}
- **Method**: PUT
- **描述**: 更新角色信息
- **请求参数**:
  | 参数名 | 类型 | 必填 | 说明 |
  |--------|------|------|------|
  | id | string | 是 | 角色ID |
  | body | Role | 是 | 角色数据 |
- **响应结构**: ApiResponse<null>

#### 删除角色
- **URL**: /api/admin/basic/role/{id}
- **Method**: DELETE
- **描述**: 删除角色
- **请求参数**:
  | 参数名 | 类型 | 必填 | 说明 |
  |--------|------|------|------|
  | id | string | 是 | 角色ID |
- **响应结构**: ApiResponse<null>

### 部门管理

#### 获取部门树
- **URL**: /api/admin/basic/dept/tree
- **Method**: GET
- **描述**: 获取部门树形结构
- **请求参数**: 无
- **响应结构**: ApiResponse<Dept[]>

#### 获取部门详情
- **URL**: /api/admin/basic/dept/{id}
- **Method**: GET
- **描述**: 根据ID获取部门详情
- **请求参数**:
  | 参数名 | 类型 | 必填 | 说明 |
  |--------|------|------|------|
  | id | string | 是 | 部门ID |
- **响应结构**: ApiResponse<Dept>

#### 创建部门
- **URL**: /api/admin/basic/dept
- **Method**: POST
- **描述**: 创建新部门
- **请求参数**: Dept (JSON Body)
- **响应结构**: ApiResponse<{ id }>

#### 更新部门
- **URL**: /api/admin/basic/dept/{id}
- **Method**: PUT
- **描述**: 更新部门信息
- **请求参数**:
  | 参数名 | 类型 | 必填 | 说明 |
  |--------|------|------|------|
  | id | string | 是 | 部门ID |
  | body | Dept | 是 | 部门数据 |
- **响应结构**: ApiResponse<null>

#### 删除部门
- **URL**: /api/admin/basic/dept/{id}
- **Method**: DELETE
- **描述**: 删除部门
- **请求参数**:
  | 参数名 | 类型 | 必填 | 说明 |
  |--------|------|------|------|
  | id | string | 是 | 部门ID |
- **响应结构**: ApiResponse<null>

### 菜单管理

#### 获取菜单树
- **URL**: /api/admin/basic/menu/tree
- **Method**: GET
- **描述**: 获取菜单树形结构
- **请求参数**: 无
- **响应结构**: ApiResponse<Menu[]>

#### 获取菜单详情
- **URL**: /api/admin/basic/menu/{id}
- **Method**: GET
- **描述**: 根据ID获取菜单详情
- **请求参数**:
  | 参数名 | 类型 | 必填 | 说明 |
  |--------|------|------|------|
  | id | string | 是 | 菜单ID |
- **响应结构**: ApiResponse<Menu>

#### 创建菜单
- **URL**: /api/admin/basic/menu
- **Method**: POST
- **描述**: 创建新菜单
- **请求参数**: Menu (JSON Body)
- **响应结构**: ApiResponse<{ id }>

#### 更新菜单
- **URL**: /api/admin/basic/menu/{id}
- **Method**: PUT
- **描述**: 更新菜单信息
- **请求参数**:
  | 参数名 | 类型 | 必填 | 说明 |
  |--------|------|------|------|
  | id | string | 是 | 菜单ID |
  | body | Menu | 是 | 菜单数据 |
- **响应结构**: ApiResponse<null>

#### 删除菜单
- **URL**: /api/admin/basic/menu/{id}
- **Method**: DELETE
- **描述**: 删除菜单
- **请求参数**:
  | 参数名 | 类型 | 必填 | 说明 |
  |--------|------|------|------|
  | id | string | 是 | 菜单ID |
- **响应结构**: ApiResponse<null>

#### 更新菜单排序
- **URL**: /api/admin/basic/menu/sort
- **Method**: POST
- **描述**: 更新菜单排序
- **请求参数**: { ids: string[] }
- **响应结构**: ApiResponse<null>

#### 更新菜单状态
- **URL**: /api/admin/basic/menu/{id}/status
- **Method**: PUT
- **描述**: 更新菜单状态
- **请求参数**:
  | 参数名 | 类型 | 必填 | 说明 |
  |--------|------|------|------|
  | id | string | 是 | 菜单ID |
  | status | string | 是 | 状态 |
- **响应结构**: ApiResponse<null>

#### 更新菜单可见性
- **URL**: /api/admin/basic/menu/{id}/visible
- **Method**: PUT
- **描述**: 更新菜单可见性
- **请求参数**:
  | 参数名 | 类型 | 必填 | 说明 |
  |--------|------|------|------|
  | id | string | 是 | 菜单ID |
  | visible | boolean | 是 | 是否可见 |
- **响应结构**: ApiResponse<null>

### 字典管理

#### 获取字典类型列表
- **URL**: /api/admin/basic/dict-type/list
- **Method**: GET
- **描述**: 分页查询字典类型列表
- **请求参数**:
  | 参数名 | 类型 | 必填 | 说明 |
  |--------|------|------|------|
  | pageIndex | number | 否 | 页码，默认1 |
  | pageSize | number | 否 | 每页数量，默认10 |
  | name | string | 否 | 名称搜索 |
  | code | string | 否 | 编码搜索 |
- **响应结构**: ApiResponse<PageResponse<DictType>>

#### 创建字典类型
- **URL**: /api/admin/basic/dict-type
- **Method**: POST
- **描述**: 创建新字典类型
- **请求参数**: DictType (JSON Body)
- **响应结构**: ApiResponse<{ id }>

#### 更新字典类型
- **URL**: /api/admin/basic/dict-type/{id}
- **Method**: PUT
- **描述**: 更新字典类型
- **请求参数**:
  | 参数名 | 类型 | 必填 | 说明 |
  |--------|------|------|------|
  | id | string | 是 | 类型ID |
  | body | DictType | 是 | 类型数据 |
- **响应结构**: ApiResponse<{ id }>

#### 删除字典类型
- **URL**: /api/admin/basic/dict-type/{id}
- **Method**: DELETE
- **描述**: 删除字典类型
- **请求参数**:
  | 参数名 | 类型 | 必填 | 说明 |
  |--------|------|------|------|
  | id | string | 是 | 类型ID |
- **响应结构**: ApiResponse<null>

#### 获取字典数据列表
- **URL**: /api/admin/basic/dict-data/list
- **Method**: GET
- **描述**: 分页查询字典数据列表
- **请求参数**:
  | 参数名 | 类型 | 必填 | 说明 |
  |--------|------|------|------|
  | pageIndex | number | 否 | 页码，默认1 |
  | pageSize | number | 否 | 每页数量，默认10 |
  | typeCode | string | 否 | 字典类型编码 |
- **响应结构**: ApiResponse<PageResponse<DictData>>

#### 获取字典项（前端用）
- **URL**: /api/admin/basic/dict-data
- **Method**: GET
- **描述**: 根据字典类型获取字典项列表
- **请求参数**:
  | 参数名 | 类型 | 必填 | 说明 |
  |--------|------|------|------|
  | typeCode | string | 是 | 字典类型编码 |
- **响应结构**: ApiResponse<Array<{ value, labelKey }>>

#### 创建字典数据
- **URL**: /api/admin/basic/dict-data
- **Method**: POST
- **描述**: 创建新字典数据
- **请求参数**: DictData (JSON Body)
- **响应结构**: ApiResponse<{ id }>

#### 更新字典数据
- **URL**: /api/admin/basic/dict-data/{id}
- **Method**: PUT
- **描述**: 更新字典数据
- **请求参数**:
  | 参数名 | 类型 | 必填 | 说明 |
  |--------|------|------|------|
  | id | string | 是 | 数据ID |
  | body | DictData | 是 | 数据内容 |
- **响应结构**: ApiResponse<{ id }>

#### 删除字典数据
- **URL**: /api/admin/basic/dict-data/{id}
- **Method**: DELETE
- **描述**: 删除字典数据
- **请求参数**:
  | 参数名 | 类型 | 必填 | 说明 |
  |--------|------|------|------|
  | id | string | 是 | 数据ID |
- **响应结构**: ApiResponse<null>

#### 批量删除字典数据
- **URL**: /api/admin/basic/dict-data/batch-delete
- **Method**: POST
- **描述**: 批量删除字典数据
- **请求参数**: { ids: string[] }
- **响应结构**: ApiResponse<null>

---

## 状态常量

### 通用状态

| 常量名 | 值 | 说明 |
|--------|-----|------|
| status | enabled | 启用 |
| status | disabled | 禁用 |

### 用户性别

| 常量名 | 值 | 说明 |
|--------|-----|------|
| user_gender | male | 男 |
| user_gender | female | 女 |
| user_gender | unknown | 未知 |

### 订单状态

| 常量名 | 值 | 说明 |
|--------|-----|------|
| order_status | pending | 待付款 |
| order_status | paid | 已付款 |
| order_status | shipped | 已发货 |
| order_status | completed | 已完成 |
| order_status | cancelled | 已取消 |

### 支付方式

| 常量名 | 值 | 说明 |
|--------|-----|------|
| payment_method | alipay | 支付宝 |
| payment_method | wechat | 微信支付 |
| payment_method | bank | 银行转账 |
| payment_method | cash | 现金 |

### 物流状态

| 常量名 | 值 | 说明 |
|--------|-----|------|
| logistics_status | pending_pickup | 待揽收 |
| logistics_status | in_transit | 运输中 |
| logistics_status | delivered | 已送达 |
| logistics_status | exception | 异常 |

---

## 数据表名

| 实体 | 表名 | 说明 |
|------|------|------|
| AdminUser | basic_user | 用户表 |
| Role | basic_role | 角色表 |
| Dept | basic_dept | 部门表 |
| Menu | basic_menu | 菜单表 |
| DictType | basic_dict_type | 字典类型表 |
| DictData | basic_dict_data | 字典数据表 |
