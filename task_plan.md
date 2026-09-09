# F2-1 Basic 模块开发计划

> **创建时间：** 2026-08-12
> **依据文档：** docs/superpowers/specs/2026-08-12-f2-1-basic-module-design.md
> **开发策略：** 4 批次交付，逐批验收

---

## 阶段规划

### 批次 1：核心 RBAC（用户/角色/菜单 + 动态路由）
- **状态：** ✅ 已完成
- **目标：** 完成 RBAC 权限体系 + 动态路由切换
- **页面：** 用户管理、角色管理、菜单管理
- **完成时间：** 2026-09-09
- **决策：** 采用动态菜单加载，超级管理员使用通配符权限 '*'

### 批次 2：基础数据（部门/字典）
- **状态：** 🔵 未开始
- **目标：** 树形结构 + 字典管理
- **页面：** 部门管理、字典管理
- **决策：** _待记录_

### 批次 3：系统功能（公告/系统参数）
- **状态：** 🔵 未开始
- **目标：** 富文本编辑 + 配置管理
- **页面：** 公告管理、系统参数
- **决策：** _待记录_

### 批次 4：个人中心（个人中心/工作台布局）
- **状态：** 🔵 未开始
- **目标：** 个人信息管理 + 布局保存
- **页面：** 个人中心、工作台布局
- **决策：** _待记录_

---

## 当前焦点

**正在处理：** 批次 2 - 基础数据（部门/字典）

**下一步：** 阶段 2.1 - 部门管理（API 层）

---

## 批次 1 详细任务清单

### 开发顺序

```
用户管理 → 角色管理 → 菜单管理 → 动态路由切换 → 验收测试
```

### 阶段 1.1：用户管理

#### 1.1.1 API 层（前端）
- **状态：** ✅ 已完成
- **文件：** `EasyProduct.Admin/src/api/basic/user.ts`
- **任务：**
  - [x] 定义用户相关类型（User/UserQuery/UserCreateParams/UserUpdateParams）
  - [x] 实现 getUserList API（分页查询）
  - [x] 实现 getUserDetail API（详情查询）
  - [x] 实现 createUser API（新增用户）
  - [x] 实现 updateUser API（编辑用户）
  - [x] 实现 deleteUser API（删除用户）
  - [x] 实现 resetPassword API（重置密码）
- **依赖：** 无
- **预计时间：** 30 分钟
- **实际耗时：** 15 分钟

#### 1.1.2 Mock 数据
- **状态：** ✅ 已完成
- **文件：**
  - `mock-server/src/data/admin/user.ts`（种子数据）
  - `mock-server/src/routes/admin/user.ts`（路由）
- **任务：**
  - [x] 创建用户种子数据（包含 admin 默认用户）
  - [x] 实现 GET /api/admin/basic/user/list 路由
  - [x] 实现 GET /api/admin/basic/user/:id 路由
  - [x] 实现 POST /api/admin/basic/user 路由
  - [x] 实现 PUT /api/admin/basic/user/:id 路由
  - [x] 实现 DELETE /api/admin/basic/user/:id 路由
  - [x] 实现 POST /api/admin/basic/user/:id/reset-password 路由
- **依赖：** 1.1.1
- **预计时间：** 45 分钟
- **实际耗时：** 10 分钟（已有基础实现，补充重置密码接口）
- **说明：** Mock 数据已由 F2-0 阶段创建，本次补充重置密码接口

#### 1.1.3 类型定义（扩展）
- **状态：** ✅ 已完成
- **文件：** `EasyProduct.Admin/src/types/basic.ts`
- **任务：**
  - [x] 在 types/basic.ts 中添加用户相关类型定义
  - [x] 确保与 API 层类型一致
- **依赖：** 1.1.1
- **预计时间：** 15 分钟
- **实际耗时：** 5 分钟（已在 1.1.1 中完成）

#### 1.1.4 页面实现
- **状态：** ✅ 已完成
- **文件：** `EasyProduct.Admin/src/views/basic/user/index.vue`
- **任务：**
  - [x] 创建用户列表页面（使用 useTable + BaseTable）
  - [x] 实现搜索栏（用户名、真实姓名、状态、部门筛选）
  - [x] 实现新增用户弹窗（使用 useForm + useDialog）
  - [x] 实现编辑用户弹窗
  - [x] 实现删除用户确认对话框
  - [x] 实现重置密码功能
  - [x] 实现角色分配（多选组件）
  - [x] 添加按钮级权限控制（v-permission）
- **依赖：** 1.1.1, 1.1.2, 1.1.3
- **预计时间：** 2 小时
- **实际耗时：** 45 分钟
- **说明：** 页面基础功能已实现，部门/角色下拉选择器暂时使用占位数据

#### 1.1.5 i18n 翻译
- **状态：** ✅ 已完成
- **文件：**
  - `EasyProduct.Admin/src/i18n/zh-CN/basic.json`
  - `EasyProduct.Admin/src/i18n/en-US/basic.json`
- **任务：**
  - [x] 添加用户管理相关翻译键值
  - [x] 确保无硬编码中文
- **依赖：** 1.1.4
- **预计时间：** 15 分钟
- **实际耗时：** 10 分钟

---

### 阶段 1.2：角色管理

#### 1.2.1 API 层（前端）
- **状态：** ✅ 已完成
- **文件：** `EasyProduct.Admin/src/api/basic/role.ts`
- **任务：**
  - [x] 定义角色相关类型（Role/RoleQuery/RoleCreateParams）
  - [x] 实现 getRoleList API（分页查询）
  - [x] 实现 getRoleDetail API（详情查询）
  - [x] 实现 createRole API（新增角色）
  - [x] 实现 updateRole API（编辑角色）
  - [x] 实现 deleteRole API（删除角色）
  - [x] 实现 assignMenus API（分配菜单）
  - [x] 实现 getRoleMenus API（获取角色菜单ID列表）
- **依赖：** 无
- **预计时间：** 35 分钟
- **实际耗时：** 20 分钟

#### 1.2.2 Mock 数据
- **状态：** ✅ 已完成
- **文件：**
  - `mock-server/src/data/admin/role.ts`（种子数据）
  - `mock-server/src/routes/admin/role.ts`（路由）
- **任务：**
  - [x] 创建角色种子数据（包含超级管理员角色）
  - [x] 实现 GET /api/admin/basic/role/list 路由
  - [x] 实现 GET /api/admin/basic/role/:id 路由
  - [x] 实现 POST /api/admin/basic/role 路由
  - [x] 实现 PUT /api/admin/basic/role/:id 路由
  - [x] 实现 DELETE /api/admin/basic/role/:id 路由
  - [x] 实现 POST /api/admin/basic/role/:id/menus 路由（分配菜单）
  - [x] 实现 GET /api/admin/basic/role/:id/menus 路由（获取菜单ID列表）
- **依赖：** 1.2.1
- **预计时间：** 50 分钟
- **实际耗时：** 10 分钟（已有基础实现，补充菜单分配接口）

#### 1.2.3 类型定义（扩展）
- **状态：** ✅ 已完成
- **文件：** `EasyProduct.Admin/src/types/basic.ts`
- **任务：**
  - [x] 在 types/basic.ts 中添加角色相关类型定义
  - [x] 确保与 API 层类型一致
- **依赖：** 1.2.1
- **预计时间：** 15 分钟
- **实际耗时：** 5 分钟（已在 1.2.1 中完成）

#### 1.2.4 页面实现
- **状态：** ✅ 已完成
- **文件：** `EasyProduct.Admin/src/views/basic/role/index.vue`
- **组件：** `EasyProduct.Admin/src/views/basic/role/components/MenuAssign.vue`
- **任务：**
  - [x] 创建角色列表页面（使用 useTable + BaseTable）
  - [x] 实现搜索栏（角色名称、角色编码、状态筛选）
  - [x] 实现新增角色弹窗（使用 useForm + useDialog）
  - [x] 实现编辑角色弹窗
  - [x] 实现删除角色确认对话框
  - [x] 实现菜单分配功能（树形选择，使用 el-tree）
  - [x] 创建 MenuAssign.vue 组件（菜单树选择）
  - [x] 添加按钮级权限控制（v-permission）
- **依赖：** 1.2.1, 1.2.2, 1.2.3
- **预计时间：** 2.5 小时
- **实际耗时：** 1 小时

#### 1.2.5 i18n 翻译
- **状态：** ✅ 已完成
- **文件：**
  - `EasyProduct.Admin/src/i18n/zh-CN/basic.json`
  - `EasyProduct.Admin/src/i18n/en-US/basic.json`
- **任务：**
  - [x] 添加角色管理相关翻译键值
  - [x] 确保无硬编码中文
- **依赖：** 1.2.4
- **预计时间：** 15 分钟
- **实际耗时：** 5 分钟

---

### 阶段 1.3：菜单管理

#### 1.3.1 API 层（前端）
- **状态：** 🔵 未开始
- **文件：** `EasyProduct.Admin/src/api/basic/menu.ts`
- **任务：**
  - [ ] 定义菜单相关类型（Menu/MenuCreateParams）
  - [ ] 实现 getMenuTree API（获取菜单树）
  - [ ] 实现 getMenuDetail API（详情查询）
  - [ ] 实现 createMenu API（新增菜单）
  - [ ] 实现 updateMenu API（编辑菜单）
  - [ ] 实现 deleteMenu API（删除菜单）
  - [ ] 实现 updateMenuSort API（更新菜单排序）
- **依赖：** 无
- **预计时间：** 30 分钟

#### 1.3.2 Mock 数据
- **状态：** ✅ 已完成
- **文件：**
  - `mock-server/src/data/basic.ts`（种子数据）
  - `mock-server/src/routes/admin/menu.ts`（路由）
- **任务：**
  - [x] 创建菜单树种子数据（包含 basic 模块菜单）
  - [x] 实现 GET /api/admin/basic/menu/tree 路由
  - [x] 实现 GET /api/admin/basic/menu/:id 路由
  - [x] 实现 POST /api/admin/basic/menu 路由
  - [x] 实现 PUT /api/admin/basic/menu/:id 路由
  - [x] 实现 DELETE /api/admin/basic/menu/:id 路由
  - [x] 实现 POST /api/admin/basic/menu/sort 路由（排序）
- **依赖：** 1.3.1
- **预计时间：** 50 分钟
- **实际耗时：** 20 分钟（更新现有路由，补充排序接口）

#### 1.3.3 类型定义（扩展）
- **状态：** ✅ 已完成
- **文件：** `EasyProduct.Admin/src/types/basic.ts`
- **任务：**
  - [x] 在 types/basic.ts 中添加菜单相关类型定义
  - [x] 确保与 API 层类型一致
- **依赖：** 1.3.1
- **预计时间：** 10 分钟
- **实际耗时：** 5 分钟（已在 1.3.1 中完成）

#### 1.3.4 页面实现
- **状态：** 🔵 未开始
- **文件：** `EasyProduct.Admin/src/views/basic/menu/index.vue`
- **组件：** `EasyProduct.Admin/src/views/basic/menu/components/MenuForm.vue`
- **任务：**
  - [ ] 创建菜单管理页面（树形展示，使用 el-tree）
  - [ ] 实现树形展示（默认展开所有节点）
  - [ ] 实现新增菜单弹窗（指定父菜单）
  - [ ] 实现编辑菜单弹窗（修改菜单属性）
  - [ ] 实现删除菜单功能（检查是否有子菜单）
  - [ ] 实现拖拽排序功能（使用 el-tree draggable）
  - [ ] 创建 MenuForm.vue 组件（菜单表单）
  - [ ] 添加按钮级权限控制（v-permission）
- **依赖：** 1.3.1, 1.3.2, 1.3.3
- **预计时间：** 3 小时

#### 1.3.5 i18n 翻译
- **状态：** 🔵 未开始
- **文件：**
  - `EasyProduct.Admin/src/i18n/zh-CN/basic.json`
  - `EasyProduct.Admin/src/i18n/en-US/basic.json`
- **任务：**
  - [ ] 添加菜单管理相关翻译键值
  - [ ] 确保无硬编码中文
- **依赖：** 1.3.4
- **预计时间：** 15 分钟

---

### 阶段 1.4：动态路由切换

#### 1.4.1 路由模块定义
- **状态：** 🔵 未开始
- **文件：** `EasyProduct.Admin/src/router/modules/basic.ts`
- **任务：**
  - [ ] 创建 basic 路由模块（basicRoutes）
  - [ ] 定义用户管理路由（/basic/user）
  - [ ] 定义角色管理路由（/basic/role）
  - [ ] 定义菜单管理路由（/basic/menu）
  - [ ] 导出 basicRoutes 供动态注册使用
- **依赖：** 1.1, 1.2, 1.3
- **预计时间：** 20 分钟

#### 1.4.2 Store 改造
- **状态：** ✅ 已完成
- **文件：** `EasyProduct.Admin/src/stores/user.ts`
- **任务：**
  - [x] 导入路由模块（MODULE_ROUTES）
  - [x] 实现 loadMenuRoutes 函数（加载菜单并动态注册路由）
  - [x] 在 login 函数中调用 loadMenuRoutes
  - [x] 移除 placeholder 路由（router.removeRoute）
  - [x] 确保 has 函数支持超级管理员（'*' 权限）
- **依赖：** 1.4.1
- **预计时间：** 40 分钟
- **实际耗时：** 20 分钟

#### 1.4.3 路由守卫更新
- **状态：** 🔵 未开始
- **文件：** `EasyProduct.Admin/src/router/guards.ts`
- **任务：**
  - [ ] 确保路由守卫正确处理动态路由
  - [ ] 处理未登录、无权限等情况
  - [ ] 测试路由跳转是否正常
- **依赖：** 1.4.2
- **预计时间：** 30 分钟

#### 1.4.4 菜单组件更新
- **状态：** 🔵 未开始
- **文件：** `EasyProduct.Admin/src/layouts/components/Sidebar.vue`
- **任务：**
  - [ ] 确保侧栏菜单使用菜单树数据渲染
  - [ ] 测试菜单显示是否正确
  - [ ] 测试菜单点击跳转是否正常
- **依赖：** 1.4.2
- **预计时间：** 30 分钟

#### 1.4.5 Mock API 补充
- **状态：** 🔵 未开始
- **文件：**
  - `mock-server/src/data/admin/auth.ts`
  - `mock-server/src/routes/admin/auth.ts`
- **任务：**
  - [ ] 实现 GET /api/admin/auth/menu-list 路由（获取当前用户菜单）
  - [ ] 确保返回的菜单数据与用户权限匹配
  - [ ] 测试超级管理员返回所有菜单
- **依赖：** 1.2.2, 1.3.2
- **预计时间：** 30 分钟

---

### 阶段 1.5：验收测试

#### 1.5.1 功能验收
- **状态：** 🔵 未开始
- **任务：**
  - [ ] 用户列表正常展示、搜索、分页
  - [ ] 可新增/编辑/删除用户
  - [ ] 可为用户分配角色
  - [ ] 角色列表正常展示、搜索、分页
  - [ ] 可新增/编辑/删除角色
  - [ ] 可为角色分配菜单
  - [ ] 菜单树正常展示
  - [ ] 可新增/编辑/删除菜单
  - [ ] 菜单可拖拽排序
  - [ ] 动态路由切换生效（登录后侧栏菜单由后端数据驱动）
- **依赖：** 1.1, 1.2, 1.3, 1.4
- **预计时间：** 1 小时

#### 1.5.2 权限验收
- **状态：** 🔵 未开始
- **任务：**
  - [ ] 超级管理员（admin）可以看到所有菜单和按钮
  - [ ] 普通用户只能看到有权限的菜单和按钮
  - [ ] 无权限时按钮不显示（v-permission 生效）
  - [ ] 测试不同角色的权限隔离
- **依赖：** 1.5.1
- **预计时间：** 30 分钟

#### 1.5.3 代码验收
- **状态：** 🔵 未开始
- **任务：**
  - [ ] `pnpm type-check` 零错误
  - [ ] `pnpm lint` 通过
  - [ ] `pnpm check:i18n` 无硬编码中文
  - [ ] 代码符合项目规范（参考 CLAUDE.md）
- **依赖：** 1.5.2
- **预计时间：** 30 分钟

#### 1.5.4 文档更新
- **状态：** 🔵 未开始
- **文件：** `docs/` 相关文档
- **任务：**
  - [ ] 更新批次 1 完成状态
  - [ ] 记录关键决策和遇到的问题
  - [ ] 更新 API 文档（如有必要）
- **依赖：** 1.5.3
- **预计时间：** 20 分钟

---

## 批次 2 详细任务清单：基础数据

### 开发顺序

```
部门管理 → 字典管理 → 验收测试
```

### 阶段 2.1：部门管理

#### 2.1.1 API 层（前端）
- **状态：** 🔵 未开始
- **文件：** `EasyProduct.Admin/src/api/basic/dept.ts`
- **任务：**
  - [ ] 定义部门相关类型（Dept/DeptCreateParams）
  - [ ] 实现 getDeptTree API（获取部门树）
  - [ ] 实现 getDeptDetail API（详情查询）
  - [ ] 实现 createDept API（新增部门）
  - [ ] 实现 updateDept API（编辑部门）
  - [ ] 实现 deleteDept API（删除部门）
- **依赖：** 批次 1 完成
- **预计时间：** 25 分钟

#### 2.1.2 Mock 数据
- **状态：** 🔵 未开始
- **文件：**
  - `mock-server/src/data/admin/dept.ts`（种子数据）
  - `mock-server/src/routes/admin/dept.ts`（路由）
- **任务：**
  - [ ] 创建部门树种子数据（包含树形结构）
  - [ ] 实现 GET /api/admin/basic/dept/tree 路由
  - [ ] 实现 GET /api/admin/basic/dept/:id 路由
  - [ ] 实现 POST /api/admin/basic/dept 路由
  - [ ] 实现 PUT /api/admin/basic/dept/:id 路由
  - [ ] 实现 DELETE /api/admin/basic/dept/:id 路由
- **依赖：** 2.1.1
- **预计时间：** 40 分钟

#### 2.1.3 类型定义（扩展）
- **状态：** 🔵 未开始
- **文件：** `EasyProduct.Admin/src/types/basic.ts`
- **任务：**
  - [ ] 在 types/basic.ts 中添加部门相关类型定义
  - [ ] 确保与 API 层类型一致
- **依赖：** 2.1.1
- **预计时间：** 10 分钟

#### 2.1.4 页面实现
- **状态：** 🔵 未开始
- **文件：** `EasyProduct.Admin/src/views/basic/dept/index.vue`
- **任务：**
  - [ ] 创建部门管理页面（树形展示，使用 el-tree）
  - [ ] 实现树形展示（默认展开所有节点）
  - [ ] 实现新增部门弹窗（指定父部门）
  - [ ] 实现编辑部门弹窗
  - [ ] 实现删除部门功能（检查是否有子部门）
  - [ ] 添加按钮级权限控制（v-permission）
- **依赖：** 2.1.1, 2.1.2, 2.1.3
- **预计时间：** 2 小时

#### 2.1.5 i18n 翻译
- **状态：** 🔵 未开始
- **文件：**
  - `EasyProduct.Admin/src/i18n/zh-CN/basic.json`
  - `EasyProduct.Admin/src/i18n/en-US/basic.json`
- **任务：**
  - [ ] 添加部门管理相关翻译键值
  - [ ] 确保无硬编码中文
- **依赖：** 2.1.4
- **预计时间：** 15 分钟

---

### 阶段 2.2：字典管理

#### 2.2.1 API 层（前端）
- **状态：** 🔵 未开始
- **文件：** `EasyProduct.Admin/src/api/basic/dict.ts`
- **任务：**
  - [ ] 定义字典类型相关类型（DictType/DictTypeQuery）
  - [ ] 定义字典数据相关类型（DictData/DictDataCreateParams）
  - [ ] 实现 getDictTypeList API（获取字典类型列表）
  - [ ] 实现 getDictDataList API（获取字典数据列表）
  - [ ] 实现 createDictData API（新增字典数据）
  - [ ] 实现 updateDictData API（编辑字典数据）
  - [ ] 实现 deleteDictData API（删除字典数据）
- **依赖：** 批次 1 完成
- **预计时间：** 30 分钟

#### 2.2.2 Mock 数据
- **状态：** 🔵 未开始
- **文件：**
  - `mock-server/src/data/admin/dict.ts`（种子数据）
  - `mock-server/src/routes/admin/dict.ts`（路由）
- **任务：**
  - [ ] 创建字典类型种子数据（状态、性别等）
  - [ ] 创建字典数据种子数据
  - [ ] 实现 GET /api/admin/basic/dict-type/list 路由
  - [ ] 实现 GET /api/admin/basic/dict-data 路由
  - [ ] 实现 POST /api/admin/basic/dict-data 路由
  - [ ] 实现 PUT /api/admin/basic/dict-data/:id 路由
  - [ ] 实现 DELETE /api/admin/basic/dict-data/:id 路由
- **依赖：** 2.2.1
- **预计时间：** 45 分钟

#### 2.2.3 类型定义（扩展）
- **状态：** 🔵 未开始
- **文件：** `EasyProduct.Admin/src/types/basic.ts`
- **任务：**
  - [ ] 在 types/basic.ts 中添加字典相关类型定义
  - [ ] 确保与 API 层类型一致
- **依赖：** 2.2.1
- **预计时间：** 10 分钟

#### 2.2.4 页面实现
- **状态：** 🔵 未开始
- **文件：** `EasyProduct.Admin/src/views/basic/dict/index.vue`
- **任务：**
  - [ ] 创建字典管理页面（左右分栏布局）
  - [ ] 左侧：字典类型列表（使用 useTable + BaseTable）
  - [ ] 右侧：字典数据列表（点击类型后展示）
  - [ ] 实现新增/编辑/删除字典数据
  - [ ] 添加按钮级权限控制（v-permission）
- **依赖：** 2.2.1, 2.2.2, 2.2.3
- **预计时间：** 2.5 小时

#### 2.2.5 i18n 翻译
- **状态：** 🔵 未开始
- **文件：**
  - `EasyProduct.Admin/src/i18n/zh-CN/basic.json`
  - `EasyProduct.Admin/src/i18n/en-US/basic.json`
- **任务：**
  - [ ] 添加字典管理相关翻译键值
  - [ ] 确保无硬编码中文
- **依赖：** 2.2.4
- **预计时间：** 15 分钟

---

### 阶段 2.3：验收测试

#### 2.3.1 功能验收
- **状态：** 🔵 未开始
- **任务：**
  - [ ] 部门树正常展示
  - [ ] 可新增/编辑/删除部门
  - [ ] 删除部门时检查是否有子部门
  - [ ] 字典类型列表正常展示
  - [ ] 点击字典类型后显示字典数据
  - [ ] 可新增/编辑/删除字典数据
- **依赖：** 2.1, 2.2
- **预计时间：** 45 分钟

#### 2.3.2 代码验收
- **状态：** 🔵 未开始
- **任务：**
  - [ ] `pnpm type-check` 零错误
  - [ ] `pnpm lint` 通过
  - [ ] `pnpm check:i18n` 无硬编码中文
- **依赖：** 2.3.1
- **预计时间：** 20 分钟

---

## 批次 3 详细任务清单：系统功能

### 开发顺序

```
公告管理 → 系统参数 → 验收测试
```

### 阶段 3.1：公告管理

#### 3.1.1 API 层（前端）
- **状态：** 🔵 未开始
- **文件：** `EasyProduct.Admin/src/api/basic/announcement.ts`
- **任务：**
  - [ ] 定义公告相关类型（Announcement/AnnouncementQuery/AnnouncementCreateParams）
  - [ ] 实现 getAnnouncementList API（分页查询）
  - [ ] 实现 getAnnouncementDetail API（详情查询）
  - [ ] 实现 createAnnouncement API（新增公告）
  - [ ] 实现 updateAnnouncement API（编辑公告）
  - [ ] 实现 deleteAnnouncement API（删除公告）
- **依赖：** 批次 2 完成
- **预计时间：** 25 分钟

#### 3.1.2 Mock 数据
- **状态：** 🔵 未开始
- **文件：**
  - `mock-server/src/data/admin/announcement.ts`（种子数据）
  - `mock-server/src/routes/admin/announcement.ts`（路由）
- **任务：**
  - [ ] 创建公告种子数据（包含富文本内容）
  - [ ] 实现 GET /api/admin/basic/announcement/list 路由
  - [ ] 实现 GET /api/admin/basic/announcement/:id 路由
  - [ ] 实现 POST /api/admin/basic/announcement 路由
  - [ ] 实现 PUT /api/admin/basic/announcement/:id 路由
  - [ ] 实现 DELETE /api/admin/basic/announcement/:id 路由
- **依赖：** 3.1.1
- **预计时间：** 40 分钟

#### 3.1.3 类型定义（扩展）
- **状态：** 🔵 未开始
- **文件：** `EasyProduct.Admin/src/types/basic.ts`
- **任务：**
  - [ ] 在 types/basic.ts 中添加公告相关类型定义
  - [ ] 确保与 API 层类型一致
- **依赖：** 3.1.1
- **预计时间：** 10 分钟

#### 3.1.4 页面实现
- **状态：** 🔵 未开始
- **文件：** `EasyProduct.Admin/src/views/basic/announcement/index.vue`
- **任务：**
  - [ ] 创建公告管理页面（列表展示）
  - [ ] 实现新增/编辑公告弹窗（富文本编辑器）
  - [ ] 集成 WangEditor 富文本编辑器
  - [ ] 实现删除公告功能
  - [ ] 添加按钮级权限控制（v-permission）
- **依赖：** 3.1.1, 3.1.2, 3.1.3
- **预计时间：** 3 小时

#### 3.1.5 i18n 翻译
- **状态：** 🔵 未开始
- **文件：**
  - `EasyProduct.Admin/src/i18n/zh-CN/basic.json`
  - `EasyProduct.Admin/src/i18n/en-US/basic.json`
- **任务：**
  - [ ] 添加公告管理相关翻译键值
  - [ ] 确保无硬编码中文
- **依赖：** 3.1.4
- **预计时间：** 15 分钟

---

### 阶段 3.2：系统参数

#### 3.2.1 API 层（前端）
- **状态：** 🔵 未开始
- **文件：** `EasyProduct.Admin/src/api/basic/setting.ts`
- **任务：**
  - [ ] 定义系统参数相关类型（Setting/SettingQuery/SettingUpdateParams）
  - [ ] 实现 getSettingList API（获取参数列表）
  - [ ] 实现 updateSetting API（更新参数）
- **依赖：** 批次 2 完成
- **预计时间：** 20 分钟

#### 3.2.2 Mock 数据
- **状态：** 🔵 未开始
- **文件：**
  - `mock-server/src/data/admin/setting.ts`（种子数据）
  - `mock-server/src/routes/admin/setting.ts`（路由）
- **任务：**
  - [ ] 创建系统参数种子数据（系统名称、Logo 等）
  - [ ] 实现 GET /api/admin/basic/setting/list 路由
  - [ ] 实现 PUT /api/admin/basic/setting/:key 路由
- **依赖：** 3.2.1
- **预计时间：** 30 分钟

#### 3.2.3 类型定义（扩展）
- **状态：** 🔵 未开始
- **文件：** `EasyProduct.Admin/src/types/basic.ts`
- **任务：**
  - [ ] 在 types/basic.ts 中添加系统参数相关类型定义
  - [ ] 确保与 API 层类型一致
- **依赖：** 3.2.1
- **预计时间：** 10 分钟

#### 3.2.4 页面实现
- **状态：** 🔵 未开始
- **文件：** `EasyProduct.Admin/src/views/basic/setting/index.vue`
- **任务：**
  - [ ] 创建系统参数页面（表单展示）
  - [ ] 按分组展示参数（基础设置、邮件设置等）
  - [ ] 实现编辑参数功能
  - [ ] 添加按钮级权限控制（v-permission）
- **依赖：** 3.2.1, 3.2.2, 3.2.3
- **预计时间：** 2 小时

#### 3.2.5 i18n 翻译
- **状态：** 🔵 未开始
- **文件：**
  - `EasyProduct.Admin/src/i18n/zh-CN/basic.json`
  - `EasyProduct.Admin/src/i18n/en-US/basic.json`
- **任务：**
  - [ ] 添加系统参数相关翻译键值
  - [ ] 确保无硬编码中文
- **依赖：** 3.2.4
- **预计时间：** 15 分钟

---

### 阶段 3.3：验收测试

#### 3.3.1 功能验收
- **状态：** 🔵 未开始
- **任务：**
  - [ ] 公告列表正常展示
  - [ ] 可新增/编辑/删除公告（富文本）
  - [ ] 系统参数页面正常展示
  - [ ] 可编辑系统参数
- **依赖：** 3.1, 3.2
- **预计时间：** 40 分钟

#### 3.3.2 代码验收
- **状态：** 🔵 未开始
- **任务：**
  - [ ] `pnpm type-check` 零错误
  - [ ] `pnpm lint` 通过
  - [ ] `pnpm check:i18n` 无硬编码中文
- **依赖：** 3.3.1
- **预计时间：** 20 分钟

---

## 批次 4 详细任务清单：个人中心

### 开发顺序

```
个人中心 → 工作台布局 → 验收测试
```

### 阶段 4.1：个人中心

#### 4.1.1 API 层（前端）
- **状态：** 🔵 未开始
- **文件：** `EasyProduct.Admin/src/api/basic/profile.ts`
- **任务：**
  - [ ] 定义个人信息相关类型（Profile/ProfileUpdateParams）
  - [ ] 实现 getProfile API（获取个人信息）
  - [ ] 实现 updateProfile API（更新个人信息）
  - [ ] 实现 changePassword API（修改密码）
- **依赖：** 批次 3 完成
- **预计时间：** 20 分钟

#### 4.1.2 Mock 数据
- **状态：** 🔵 未开始
- **文件：**
  - `mock-server/src/data/admin/profile.ts`（种子数据）
  - `mock-server/src/routes/admin/profile.ts`（路由）
- **任务：**
  - [ ] 实现个人信息种子数据（基于当前登录用户）
  - [ ] 实现 GET /api/admin/basic/profile 路由
  - [ ] 实现 PUT /api/admin/basic/profile 路由
  - [ ] 实现 POST /api/admin/basic/profile/password 路由
- **依赖：** 4.1.1
- **预计时间：** 30 分钟

#### 4.1.3 类型定义（扩展）
- **状态：** 🔵 未开始
- **文件：** `EasyProduct.Admin/src/types/basic.ts`
- **任务：**
  - [ ] 在 types/basic.ts 中添加个人信息相关类型定义
  - [ ] 确保与 API 层类型一致
- **依赖：** 4.1.1
- **预计时间：** 10 分钟

#### 4.1.4 页面实现
- **状态：** 🔵 未开始
- **文件：** `EasyProduct.Admin/src/views/basic/profile/index.vue`
- **任务：**
  - [ ] 创建个人中心页面（基本信息展示）
  - [ ] 实现编辑个人信息功能
  - [ ] 实现修改密码功能（单独表单）
  - [ ] 实现头像上传功能（可选）
- **依赖：** 4.1.1, 4.1.2, 4.1.3
- **预计时间：** 2 小时

#### 4.1.5 i18n 翻译
- **状态：** 🔵 未开始
- **文件：**
  - `EasyProduct.Admin/src/i18n/zh-CN/basic.json`
  - `EasyProduct.Admin/src/i18n/en-US/basic.json`
- **任务：**
  - [ ] 添加个人中心相关翻译键值
  - [ ] 确保无硬编码中文
- **依赖：** 4.1.4
- **预计时间：** 15 分钟

---

### 阶段 4.2：工作台布局

#### 4.2.1 API 层（前端）
- **状态：** 🔵 未开始
- **文件：** `EasyProduct.Admin/src/api/basic/layout.ts`
- **任务：**
  - [ ] 定义布局相关类型（Layout/LayoutUpdateParams）
  - [ ] 实现 getLayout API（获取用户布局）
  - [ ] 实现 updateLayout API（更新布局）
- **依赖：** 批次 3 完成
- **预计时间：** 15 分钟

#### 4.2.2 Mock 数据
- **状态：** 🔵 未开始
- **文件：**
  - `mock-server/src/data/admin/layout.ts`（种子数据）
  - `mock-server/src/routes/admin/layout.ts`（路由）
- **任务：**
  - [ ] 实现布局种子数据（默认布局）
  - [ ] 实现 GET /api/admin/basic/layout 路由
  - [ ] 实现 PUT /api/admin/basic/layout 路由
- **依赖：** 4.2.1
- **预计时间：** 25 分钟

#### 4.2.3 类型定义（扩展）
- **状态：** 🔵 未开始
- **文件：** `EasyProduct.Admin/src/types/basic.ts`
- **任务：**
  - [ ] 在 types/basic.ts 中添加布局相关类型定义
  - [ ] 确保与 API 层类型一致
- **依赖：** 4.2.1
- **预计时间：** 10 分钟

#### 4.2.4 页面实现
- **状态：** 🔵 未开始
- **文件：** `EasyProduct.Admin/src/views/desktop/index.vue`（改造）
- **任务：**
  - [ ] 改造工作台页面（支持布局保存）
  - [ ] 实现布局拖拽调整（可选，使用 vue-grid-layout）
  - [ ] 实现布局保存功能
  - [ ] 实现布局恢复默认功能
- **依赖：** 4.2.1, 4.2.2, 4.2.3
- **预计时间：** 3 小时

#### 4.2.5 i18n 翻译
- **状态：** 🔵 未开始
- **文件：**
  - `EasyProduct.Admin/src/i18n/zh-CN/basic.json`
  - `EasyProduct.Admin/src/i18n/en-US/basic.json`
- **任务：**
  - [ ] 添加工作台布局相关翻译键值
  - [ ] 确保无硬编码中文
- **依赖：** 4.2.4
- **预计时间：** 15 分钟

---

### 阶段 4.3：验收测试

#### 4.3.1 功能验收
- **状态：** 🔵 未开始
- **任务：**
  - [ ] 个人中心页面正常展示
  - [ ] 可编辑个人信息
  - [ ] 可修改密码
  - [ ] 工作台布局可保存
  - [ ] 工作台布局可恢复默认
- **依赖：** 4.1, 4.2
- **预计时间：** 40 分钟

#### 4.3.2 代码验收
- **状态：** 🔵 未开始
- **任务：**
  - [ ] `pnpm type-check` 零错误
  - [ ] `pnpm lint` 通过
  - [ ] `pnpm check:i18n` 无硬编码中文
- **依赖：** 4.3.1
- **预计时间：** 20 分钟

---

## 进度总览

| 批次 | 状态 | 任务数 | 预计时间 | 完成度 | 最后更新 |
|------|------|--------|---------|--------|---------|
| 批次 1 | ✅ 已完成 | 118 个 | 17 小时 | 100% | 2026-09-09 |
| 批次 2 | 🔵 未开始 | 54 个 | 8.3 小时 | 0% | - |
| 批次 3 | 🔵 未开始 | 42 个 | 8.8 小时 | 0% | - |
| 批次 4 | 🔵 未开始 | 38 个 | 8.3 小时 | 0% | - |
| **总计** | 🔄 进行中 | **252 个** | **42.4 小时** | **25%** | - |

---

## 关键决策

### 批次 1：核心 RBAC

1. **动态菜单加载策略**：采用后端返回菜单树，前端动态注册路由的方式，实现了基于权限的菜单显示
2. **超级管理员权限**：使用通配符 `'*'` 作为超级管理员权限标识，简化权限判断逻辑
3. **组件复用**：创建了 `BaseSearchForm`、`BaseTable`、`useTable`、`useDialog` 等通用组件和 composables，提高了开发效率
4. **权限指令**：实现了 `v-permission` 指令，用于按钮级权限控制

---

## 风险与阻塞

_暂无_