# 公告管理功能设计文档

> **日期：** 2026-08-19
> **状态：** 已确认
> **范围：** 管理端管理 + 官网展示 + 小程序展示

---

## 1. 功能概述

### 1.1 需求背景

系统公告管理功能，支持：
- 管理端：公告的创建、编辑、发布、撤回、置顶等全生命周期管理
- 官网：访客查看公开公告
- 小程序：会员查看公告，追踪阅读状态

### 1.2 核心功能

| 功能 | 描述 |
|------|------|
| 公告类型 | 全员公告、定向公告（指定角色） |
| 公告级别 | 普通、重要、紧急 |
| 状态流转 | 草稿 → 已发布 → 已撤回（可重新发布） |
| 置顶功能 | 支持置顶，置顶公告优先显示 |
| 阅读追踪 | 记录用户阅读状态，统计已读/未读人数 |
| 附件上传 | 支持上传 PDF、Word 等附件 |
| 富文本编辑 | 使用 WangEditor 编辑器 |

---

## 2. 数据模型

### 2.1 公告实体（`basic_announcement`）

| 字段 | 类型 | 说明 |
|------|------|------|
| id | GUID | 主键 |
| title | string(200) | 标题 |
| content | text | 内容（富文本 HTML） |
| type | enum | 类型：'all'(全员) / 'targeted'(定向) |
| level | enum | 级别：'normal'(普通) / 'important'(重要) / 'urgent'(紧急) |
| targetRoleIds | json? | 目标角色 ID 列表（定向公告时必填） |
| isTop | boolean | 是否置顶 |
| topTime | datetime? | 置顶时间 |
| publishTime | datetime? | 发布时间 |
| recallTime | datetime? | 撤回时间 |
| status | enum | 状态：'draft'(草稿) / 'published'(已发布) / 'recalled'(已撤回) |
| attachments | json? | 附件列表 [{name, url, size}] |
| creatorId | GUID | 创建人 ID |
| creatorName | string(50) | 创建人姓名（冗余字段） |
| createdAt | datetime | 创建时间 |
| updatedAt | datetime? | 更新时间 |

### 2.2 阅读记录实体（`basic_announcement_read`）

| 字段 | 类型 | 说明 |
|------|------|------|
| id | GUID | 主键 |
| announcementId | GUID | 公告 ID |
| userId | GUID | 用户 ID |
| readAt | datetime | 阅读时间 |

### 2.3 设计要点

1. **枚举使用字符串**：符合 EasyProduct 规范，便于阅读和维护
2. **附件使用 JSON 数组**：灵活存储多个附件信息
3. **冗余创建人姓名**：避免频繁关联查询，提升性能
4. **阅读记录独立表**：支持精确追踪用户阅读状态

---

## 3. API 设计

### 3.1 管理端 API（`/api/admin/basic/announcement`）

#### 列表查询
```
GET /api/admin/basic/announcement
Query: {
  pageIndex: number,
  pageSize: number,
  title?: string,
  type?: 'all' | 'targeted',
  level?: 'normal' | 'important' | 'urgent',
  status?: 'draft' | 'published' | 'recalled',
  isTop?: boolean
}
Response: ApiResponse<PageResult<AnnouncementDto>>
```

#### 详情查询
```
GET /api/admin/basic/announcement/:id
Response: ApiResponse<AnnouncementDto>
```

#### 创建公告
```
POST /api/admin/basic/announcement
Body: CreateAnnouncementDto
Response: ApiResponse<{ id: string }>
```

#### 更新公告
```
PUT /api/admin/basic/announcement/:id
Body: UpdateAnnouncementDto
Response: ApiResponse<null>
```

#### 删除公告
```
DELETE /api/admin/basic/announcement/:id
Response: ApiResponse<null>
```

#### 发布公告
```
PUT /api/admin/basic/announcement/:id/publish
Response: ApiResponse<null>
```

#### 撤回公告
```
PUT /api/admin/basic/announcement/:id/recall
Response: ApiResponse<null>
```

#### 置顶/取消置顶
```
PUT /api/admin/basic/announcement/:id/top
Body: { isTop: boolean }
Response: ApiResponse<null>
```

---

### 3.2 官网 API（`/api/site/announcement`）

#### 公开公告列表
```
GET /api/site/announcement
Query: { pageIndex: number, pageSize: number }
Response: ApiResponse<PageResult<PublicAnnouncementDto>>
```

#### 公告详情
```
GET /api/site/announcement/:id
Response: ApiResponse<PublicAnnouncementDto>
```

**注意：** 仅返回已发布、未撤回的公告。

---

### 3.3 小程序 API（`/api/app/announcement`）

#### 用户公告列表（含阅读状态）
```
GET /api/app/announcement
Query: { pageIndex: number, pageSize: number }
Response: ApiResponse<PageResult<UserAnnouncementDto>>
```

#### 用户公告详情
```
GET /api/app/announcement/:id
Response: ApiResponse<UserAnnouncementDto>
```

#### 标记已读
```
POST /api/app/announcement/:id/read
Response: ApiResponse<null>
```

#### 未读数量
```
GET /api/app/announcement/unread-count
Response: ApiResponse<{ count: number }>
```

**注意：** 根据用户角色过滤定向公告。

---

## 4. 前端设计

### 4.1 管理端页面

#### 4.1.1 公告列表页（`src/views/basic/announcement/index.vue`）

**页面结构：**
- 顶部：查询表单（标题、类型、级别、状态筛选）
- 工具栏：新增、批量删除按钮
- 表格：公告列表，支持排序、分页
- 操作列：
  - 草稿：编辑、删除、发布
  - 已发布：查看、撤回、置顶/取消置顶
  - 已撤回：查看、删除、重新发布

**列表字段：**
- 标题（可点击预览）
- 类型（全员/定向）
- 级别（普通/重要/紧急，Tag 颜色区分）
- 状态（草稿/已发布/已撤回，Tag 颜色区分）
- 置顶（是/否）
- 发布时间
- 创建人

#### 4.1.2 公告编辑页（`src/views/basic/announcement/edit.vue`）

**表单字段：**
- 标题：必填，长度 2-200
- 类型：单选（全员/定向）
  - 定向时显示角色选择器（多选）
- 级别：单选（普通/重要/紧急）
- 内容：富文本编辑器（WangEditor）
- 附件：文件上传组件（支持 PDF、Word 等）

**操作按钮：**
- 保存草稿
- 发布
- 取消

#### 4.1.3 公告详情弹窗（`src/views/basic/announcement/components/DetailDialog.vue`）

**展示内容：**
- 标题、类型、级别、状态
- 内容（富文本渲染）
- 附件列表（可下载）
- 发布时间、创建人
- 阅读统计（已读/未读人数）

---

### 4.2 官网页面

#### 4.2.1 公告列表页（`src/views/announcement/index.vue`）

**展示内容：**
- 标题（可点击查看详情）
- 级别标签（重要/紧急用红色）
- 发布时间
- 置顶公告优先显示

#### 4.2.2 公告详情页（`src/views/announcement/detail.vue`）

**展示内容：**
- 标题、发布时间、级别标签
- 内容（富文本渲染）
- 附件列表（可下载）

---

### 4.3 小程序页面

#### 4.3.1 公告列表页（`pages/announcement/index`）

**展示内容：**
- 未读数量显示（导航栏右侧）
- 标题（可点击查看详情）
- 级别标签
- 发布时间
- 未读标记（红点 ●）
- 置顶公告优先显示

#### 4.3.2 公告详情页（`pages/announcement/detail`）

**展示内容：**
- 标题、发布时间、级别标签
- 内容（rich-text 组件渲染）
- 附件列表（可下载）
- 自动标记已读

---

## 5. 后端设计

### 5.1 实体定义

**公告实体：** `EasyProduct.Models/Entities/Basic/Announcement.cs`

**阅读记录实体：** `EasyProduct.Models/Entities/Basic/AnnouncementRead.cs`

详见第 2 节数据模型。

---

### 5.2 Service 层

**核心方法：**

| 方法 | 说明 |
|------|------|
| GetListAsync | 获取公告列表（管理端，支持筛选、分页） |
| GetByIdAsync | 获取公告详情 |
| CreateAsync | 创建公告（草稿状态） |
| UpdateAsync | 更新公告（仅草稿可编辑） |
| DeleteAsync | 删除公告（仅草稿、已撤回可删除） |
| PublishAsync | 发布公告（草稿 → 已发布） |
| RecallAsync | 撤回公告（已发布 → 已撤回） |
| SetTopAsync | 置顶/取消置顶 |
| GetPublicListAsync | 获取公开公告列表（官网） |
| GetUserListAsync | 获取用户公告列表（小程序，含阅读状态） |
| MarkAsReadAsync | 标记已读 |
| GetUnreadCountAsync | 获取未读数量 |
| GetReadStatsAsync | 获取阅读统计 |

---

### 5.3 Controller 层

**管理端：** `Controllers/Admin/Basic/AnnouncementController.cs`
- 路由：`/api/admin/basic/announcement`
- 权限：Admin JWT

**官网：** `Controllers/Site/AnnouncementController.cs`
- 路由：`/api/site/announcement`
- 权限：匿名访问，限流

**小程序：** `Controllers/App/AnnouncementController.cs`
- 路由：`/api/app/announcement`
- 权限：Member JWT

---

### 5.4 业务逻辑要点

**状态流转规则：**
- 草稿：可编辑、删除、发布
- 已发布：可撤回、置顶，不可编辑删除
- 已撤回：可删除、重新发布，不可编辑

**定向公告过滤：**
- 存储角色 ID JSON 数组：`["role-id-1", "role-id-2"]`
- 查询时解析 JSON，与用户角色比对

**置顶排序：**
- 列表查询：置顶优先（`ORDER BY is_top DESC, top_time DESC, publish_time DESC`）
- 置顶时间记录最后一次置顶操作

---

## 6. Mock 数据设计

### 6.1 Mock 数据结构

**公告数据：** `mock-server/data/announcement.js`

**阅读记录数据：** `mock-server/data/announcement-read.js`

包含各种状态、类型、级别组合的测试数据。

---

### 6.2 Mock 路由

**管理端：** `mock-server/routes/admin/basic/announcement.js`
- 实现完整 CRUD + 状态流转逻辑

**官网：** `mock-server/routes/site/announcement.js`
- 仅返回已发布、未撤回的公告

**小程序：** `mock-server/routes/app/announcement.js`
- 返回已发布、未撤回的公告
- 根据用户角色过滤定向公告
- 包含阅读状态、未读数量统计

---

## 7. 技术要点

### 7.1 组件复用

- **富文本编辑器：** 使用现有 `RichTextEditor` 组件（WangEditor 封装）
- **文件上传：** 使用现有文件上传接口
- **列表组件：** 使用 `BaseSearchForm` + `BaseTable` 组合

### 7.2 国际化

所有 UI 文本通过 i18n key 引用，支持 zh-CN/en-US。

### 7.3 权限控制

- 管理端：Admin JWT 认证
- 官网：匿名访问，需限流
- 小程序：Member JWT 认证

---

## 8. 开发计划

### 8.1 阶段划分

**阶段 1：管理端开发**
- 前端：列表页 + 编辑页 + 详情弹窗
- Mock：管理端 API

**阶段 2：官网开发**
- 前端：列表页 + 详情页
- Mock：官网 API

**阶段 3：小程序开发**
- 前端：列表页 + 详情页
- Mock：小程序 API

**阶段 4：后端开发**
- 实体 + Service + Controller
- 单元测试

---

## 9. 验收标准

- ✅ TypeScript 类型检查通过（0 错误）
- ✅ ESLint 检查通过
- ✅ i18n 硬编码检查通过
- ✅ 生产构建成功
- ✅ 三端功能正常（管理端、官网、小程序）
- ✅ 状态流转符合设计
- ✅ 定向公告按角色过滤
- ✅ 置顶排序正确
- ✅ 阅读状态追踪准确

---

## 10. 参考资料

- EasyProject 源项目：`D:\4-MyProject\EasyProject\EasyWechatWeb\`
- EasyProduct 集成设计：`docs/superpowers/specs/2026-08-08-easyproduct-integration-design.md`
- 前端开发规范：`docs/frontend-guidelines.md`
- 后端开发规范：`docs/backend-guidelines.md`