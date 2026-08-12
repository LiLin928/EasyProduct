# F2-1 Basic 模块开发发现

> **创建时间：** 2026-08-12

---

## 设计文档分析

**来源：** docs/superpowers/specs/2026-08-12-f2-1-basic-module-design.md

### 核心发现

1. **交付策略**
   - 4 批次交付，每批独立验收
   - 批次 1 最关键（核心 RBAC + 动态路由切换）
   - 开发顺序：用户 → 角色 → 菜单 → 动态路由

2. **技术架构**
   - 基于 F2-0 封装层（useTable/useForm/useDialog/useDict/usePermission/BaseTable）
   - 菜单树、部门树使用 Element Plus el-tree 组件
   - 富文本编辑（批次 3）使用 WangEditor

3. **权限标识规范**
   - 三层结构：`模块:页面:操作`
   - 操作类型：list/edit/delete
   - 示例：`basic:user:list` / `basic:user:edit` / `basic:user:delete`

4. **数据模型**
   - GUID 主键
   - ISO 8601 时间格式
   - 状态字段使用小写字符串（enabled/disabled）

---

## API 契约摘要

### 批次 1 API

**用户管理：**
- `GET /api/admin/basic/user/list` - 用户列表（分页）
- `GET /api/admin/basic/user/:id` - 用户详情
- `POST /api/admin/basic/user` - 新增用户
- `PUT /api/admin/basic/user/:id` - 编辑用户
- `DELETE /api/admin/basic/user/:id` - 删除用户
- `POST /api/admin/basic/user/:id/reset-password` - 重置密码

**角色管理：**
- `GET /api/admin/basic/role/list` - 角色列表（分页）
- `GET /api/admin/basic/role/:id` - 角色详情
- `POST /api/admin/basic/role` - 新增角色
- `PUT /api/admin/basic/role/:id` - 编辑角色
- `DELETE /api/admin/basic/role/:id` - 删除角色
- `POST /api/admin/basic/role/:id/menus` - 分配菜单
- `GET /api/admin/basic/role/:id/menus` - 获取角色菜单ID列表

**菜单管理：**
- `GET /api/admin/basic/menu/tree` - 菜单树
- `GET /api/admin/basic/menu/:id` - 菜单详情
- `POST /api/admin/basic/menu` - 新增菜单
- `PUT /api/admin/basic/menu/:id` - 编辑菜单
- `DELETE /api/admin/basic/menu/:id` - 删除菜单
- `POST /api/admin/basic/menu/sort` - 更新菜单排序

---

## 技术债务

_暂无_

---

## 后续批次发现

### 批次 2 关键发现

1. **部门管理**
   - 树形结构，类似菜单管理
   - 删除前需检查是否有子部门
   - 用户管理需要引用部门数据（下拉选择）

2. **字典管理**
   - 双层结构：字典类型 + 字典数据
   - 左右分栏布局（类型列表 + 数据列表）
   - useDict composable 需要使用字典数据

### 批次 3 关键发现

1. **公告管理**
   - 首次引入富文本编辑器（WangEditor）
   - 需要处理富文本内容的存储和展示
   - 可能需要图片上传功能

2. **系统参数**
   - 键值对结构，按分组展示
   - 不同类型参数（文本、数字、布尔、JSON）
   - 系统级配置，需谨慎修改

### 批次 4 关键发现

1. **个人中心**
   - 基于当前登录用户
   - 修改密码需要验证旧密码
   - 头像上传可选（第一阶段可不实现）

2. **工作台布局**
   - 用户级布局保存
   - 可选：支持拖拽调整布局（使用 vue-grid-layout）
   - 恢复默认布局功能

---

## 技术依赖关系

```
批次 1（核心 RBAC）
    ↓
批次 2（部门/字典）← 用户管理需要部门数据
    ↓
批次 3（公告/参数）← 字典数据可复用
    ↓
批次 4（个人中心）← 独立模块
```

---

## 风险提示

1. **批次 1 最关键**
   - 动态路由切换是整个系统的核心
   - 必须充分测试权限隔离
   - 建议批次 1 完成后暂停，验证后再继续

2. **批次 3 富文本编辑器**
   - WangEditor 首次引入，需研究集成方式
   - 图片上传需要后端支持（B 系列 P1 阶段）
   - 建议先实现基础功能，图片上传后续迭代

3. **批次 4 工作台布局**
   - 布局保存功能相对独立
   - 可简化为只保存基础配置（如折叠状态）
   - 完整的拖拽布局可延后到后续迭代

---