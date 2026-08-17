# 批次3设计：公告 / 系统参数 / 个人中心 / 工作台轻量首页

> 承接：`2026-08-13-batch2-dept-dict-design.md`（批次2 已完成部门+字典+部门选择组件）
> 阶段：F2-1 Basic 模块收口批次（9 页中的剩余 4 页）
> 日期：2026-08-17

## 1. 目标与范围

完成 F2-1 Basic 模块剩余 4 页，收口 Basic 模块：

| 页面 | 路由 | 性质 |
|------|------|------|
| 公告管理 | `/basic/announcement` | CRUD + 富文本 |
| 系统参数 | `/basic/config` | 扁平键值列表（内联编辑） |
| 个人中心 | `/profile`（独立路由） | 基本信息 + 头像 + 改密 |
| 工作台首页 | `/desktop`（重写占位页） | 轻量聚合首页 |

不在本批次范围：工作台完整 Widget 化（经营概览/KPI/待办/预警四 Widget 系统属 F2-9）。

## 2. 关键决策

| 维度 | 决策 | 理由 |
|------|------|------|
| 范围 | 4 页一次性完成 | F2-1 收口，富文本/ImageUpload 作为通用组件首次引入 |
| 公告正文 | 富文本 WangEditor | F2-0 蓝图约定首个富文本需求引入 WangEditor，公告需图文排版 |
| 系统参数形态 | 扁平键值列表 | 运维可随意扩展参数，无需改代码增减字段 |
| 工作台程度 | 轻量首页（统计卡片+快捷入口+待办+最近订单） | F2-9 才完整 Widget 化，本批次仅做实用轻量首页 |
| 个人中心功能 | 基本信息 + 头像上传 + 修改密码 | 覆盖 F2-1「个人中心改密」要点 |
| 系统参数命名 | `config`（非 `setting`） | 贴合「键值配置」语义，与 `api/basic/config.ts` 对齐 |
| 个人中心路由 | 独立 `/profile`，不放 basic children | 对当前用户而非管理对象，顶栏头像下拉进入 |
| 图片上传 | 统一 `/api/admin/basic/file/upload` 接口 | ImageUpload 与 RichTextEditor 图片插入共用 |

## 3. 架构与文件结构

沿用既有分层，不改变骨架。新增/修改文件：

```
mock-server/src/
├── data/admin/announcement.ts          # 公告种子数据
├── data/admin/config.ts                 # 系统参数种子数据
├── routes/admin/announcement.ts         # 公告路由
├── routes/admin/config.ts               # 系统参数路由
├── routes/admin/file.ts                 # 文件上传路由（通用）
└── routes/index.ts                      # 注册 file/announcement/config 路由

EasyProduct.Admin/src/
├── types/basic.ts                       # 追加 Announcement/SystemConfig/ProfileInfo/DesktopOverview 等
├── api/basic/announcement.ts            # 公告 API
├── api/basic/config.ts                  # 系统参数 API
├── api/basic/profile.ts                 # 个人中心 API
├── api/basic/desktop.ts                # 工作台概览 API
├── api/common/file.ts                   # 文件上传 API（通用）
├── components/common/ImageUpload.vue    # 通用图片上传组件
├── components/common/RichTextEditor.vue # 通用富文本组件（WangEditor 封装）
├── views/basic/announcement/index.vue   # 公告管理列表
├── views/basic/announcement/components/AnnouncementFormDialog.vue
├── views/basic/config/index.vue         # 系统参数（内联编辑）
├── views/basic/config/components/ConfigFormDialog.vue  # 仅新增用
├── views/basic/profile/index.vue        # 个人中心
├── views/desktop/index.vue              # 重写：轻量首页
├── router/modules/basic.ts              # 追加 announcement/config 子路由
├── router/index.ts                      # 追加 /profile 独立路由
└── i18n/locales/{zh-CN,en-US}/basic.json # 追加 4 页文案 key（basic 模块语言包）
```

## 4. 通用组件设计

### 4.1 ImageUpload.vue

- 基于 Element Plus `el-upload`，单图模式
- Props：`modelValue: string`（url）、`maxSize?: number`（默认 2MB）、`accept?: string`（默认 `image/*`）、`disabled?: boolean`、`circle?: boolean`（头像圆形）
- 上传 `POST /api/admin/basic/file/upload`（multipart `file` 字段），成功取 `data.url` 赋 `modelValue`
- 预览 + 删除按钮；超 size 用 `ElMessage.warning`（走 `useLocale` 文案）
- 用途：个人中心头像、公告封面

### 4.2 RichTextEditor.vue（首次引入富文本依赖）

- 依赖：`@wangeditor/editor` + `@wangeditor/editor-for-vue@next`（Vue3 官方适配，约 400KB）
- 依赖理由：公告正文需图文排版；WangEditor 是 F2-0 蓝图指定的首个富文本实现，社区活跃、Vue3 兼容
- Props：`modelValue: string`（html）、`height?: string`（默认 `300px`）、`placeholder?: string`、`disabled?: boolean`
- 工具栏精简：标题/加粗/斜体/引用/有序无序列表/链接/图片/对齐/撤销重做
- 图片插入复用 `/api/admin/basic/file/upload`，自定义 `customUpload` 钩子
- 生命周期：`onCreated` 存 editor 实例，`onChange` emit html，`onBeforeUnmount` 调 `editor.destroy()` 防泄漏
- 用途：公告正文

### 4.3 文件上传 mock

- 路由：`POST /api/admin/basic/file/upload`，接收 multipart `file`
- 返回 `ok({ url, fileName })`
- url 形态：mock 阶段返回 `data:image/<ext>;base64,...` 占位（避免真实 OSS），后端交付换真实 OSS

## 5. 数据模型与 API 契约

### 5.1 公告 Announcement

```ts
interface Announcement {
  id: string
  title: string
  content: string              // html
  coverImage?: string
  type: 'notice' | 'system'    // 通知/系统
  status: 'enabled' | 'disabled'
  isTop: boolean
  publishTime: string
  viewCount: number
  createByName: string
  createTime: string
  updateTime: string
}
interface AnnouncementQuery extends PageQuery { title?: string; type?: string; status?: string }
interface AnnouncementCreateParams { title: string; content: string; coverImage?: string; type: 'notice' | 'system'; status: 'enabled' | 'disabled'; isTop: boolean }
```

Mock 路由（前缀 `/api/admin`）：
- `GET /basic/announcement/list` — 分页 + title/type/status 筛选
- `GET /basic/announcement/:id` — 详情
- `POST /basic/announcement` — 新增
- `PUT /basic/announcement/:id` — 编辑
- `DELETE /basic/announcement/:id` — 删除
- `POST /basic/announcement/batch-delete` — 批量删除 `{ ids: string[] }`
- `PUT /basic/announcement/:id/publish` — 发布（置 publishTime）

### 5.2 系统参数 SystemConfig

```ts
interface SystemConfig {
  id: string
  key: string                  // 唯一
  label: string
  value: string
  type: 'string' | 'number' | 'boolean'   // 值类型，驱动编辑控件
  remark: string
  createTime: string
  updateTime: string
}
interface SystemConfigQuery extends PageQuery { key?: string; label?: string }
interface SystemConfigCreateParams { key: string; label: string; value: string; type: 'string' | 'number' | 'boolean'; remark: string }
```

Mock 路由：
- `GET /basic/config/list` — key/label 筛选分页
- `POST /basic/config` — 新增（key 重复返 400「参数键已存在」）
- `PUT /basic/config/:id` — 编辑
- `DELETE /basic/config/:id` — 删除
- `POST /basic/config/batch-delete` — 批量删除

### 5.3 个人中心 Profile

```ts
interface ProfileInfo {
  id: string
  userName: string
  realName: string
  phone?: string
  email?: string
  avatar?: string
  roles: string[]
  deptName?: string
  lastLoginTime?: string
}
interface ProfileUpdateParams { realName: string; phone?: string; email?: string; avatar?: string }
interface ChangePwdParams { oldPassword: string; newPassword: string }
```

Mock 路由：
- `GET /basic/profile` — 从 Authorization 取当前用户返其信息
- `PUT /basic/profile` — 更新基本信息
- `PUT /basic/profile/password` — 改密（校验 oldPassword = 当前账号密码，错误返 400「原密码不正确」）

### 5.4 工作台概览 Desktop

```ts
interface DesktopOverview {
  userCount: number
  orderCount: number
  salesAmount: number
  todayVisits: number
  recentOrders: { id: string; amount: number; status: string; customerName: string; createTime: string }[]
  todos: { id: string; title: string; type: string; createTime: string }[]
}
```

Mock 路由：
- `GET /basic/desktop/overview` — 聚合返上述数据，数值用 mockjs 随机

全部走既有信封 `ok/fail/paginate`，HTTP 一律 200，`code===200` 成功，与后端规范 §5 逐字一致。

## 6. 各页面要点

### 6.1 公告管理 `views/basic/announcement/index.vue`

- `BaseSearchForm` + `useSearch`（title/type/status 筛选；type 走静态 options 非 dict，因仅 notice/system 两值）
- `BaseTable` + `useTable`，toolbar 新增/批量删除按钮
- 弹窗 `AnnouncementFormDialog`：title 输入、type 单选、isTop 开关、status 单选、`ImageUpload` 封面、`RichTextEditor` 正文
- 列表列：title / cover / type / status(tag) / isTop / publishTime / viewCount / 操作(编辑/发布/删除)
- 权限标识：`basic:announcement:list/add/edit/delete/publish`

### 6.2 系统参数 `views/basic/config/index.vue`

- `BaseSearchForm`（key/label 筛选）+ `BaseTable` + `useTable`，toolbar 新增/批量删除
- **内联编辑**：行内「编辑」按钮切换为可编辑，`type` 字段驱动控件（string→input、number→number、boolean→switch），行内保存/取消
- 新增走 `ConfigFormDialog`（key/label/value/type/remark）
- 权限标识：`basic:config:list/add/edit/delete`

### 6.3 个人中心 `views/basic/profile/index.vue`

- 独立路由 `/profile`（不放 basic children），顶栏头像下拉进入
- 左侧头像（`ImageUpload` 圆形）+ 右侧 `el-tabs`：
  - 基本信息：realName/phone/email 表单，保存调 `PUT /basic/profile`
  - 修改密码：oldPassword/newPassword/confirm 三字段 + 一致性校验，调 `PUT /basic/profile/password`，成功后清 token 跳登录页
- 无按钮级权限（所有登录用户可访问自身）

### 6.4 工作台 `views/desktop/index.vue`（重写占位页）

- 欢迎区：realName + 角色 + 当前时间（dayjs 格式化）
- 4 统计卡片：userCount / orderCount / salesAmount / todayVisits（mock 随机）
- 快捷入口：跳转各模块路由
- 待办列表（todos[]）+ 最近订单（recentOrders[]）
- 数据来自 `GET /basic/desktop/overview`

## 7. 错误处理

- HTTP/信封错误沿用 `utils/request` 拦截器：401 跳登录、403 forbidden、网络错误 ElMessage
- 改密旧密码错误：后端返 `{code:400, message:'原密码不正确'}`，拦截器统一 `ElMessage.error(message)`，页面层 catch 空处理
- 系统参数 key 重复：同上 400 + message
- 上传失败/超 size：`ImageUpload` 内 `ElMessage` 提示，不发 emit
- `RichTextEditor`：`onBeforeUnmount` 调 `editor.destroy()` 防泄漏；富文本空内容提交时前端补「正文不能为空」校验

## 8. 测试与 DoD

- 前端工程无单测框架，遵循 `frontend-guidelines` §5 DoD：
  - `vue-tsc --noEmit` 零错误
  - ESLint 通过
  - `check:i18n` 无硬编码中文
  - `pnpm build` 通过
- mock-server 独立工程，验证走 `pnpm dev` 启动 + 接口手测
- 本批次无后端、无涉钱逻辑，不触发 xUnit 强制
- 人工走查：4 页中英切换、权限按钮显隐、CRUD 闭环

## 9. 实现编排（方案 A：分层推进 + 由简到繁）

1. **通用组件层**：`ImageUpload` + `RichTextEditor` + mock `/file/upload` 路由 + `api/common/file.ts`
2. **系统参数页**（纯 CRUD，无新组件依赖）
3. **工作台轻量首页**（聚合展示）
4. **个人中心页**（用 ImageUpload）
5. **公告页**（用 RichText + ImageUpload，最重放最后）

每页完整链路：mock data + mock route + types + api + 页面 + 路由 + i18n，独立可演示、可提交。

## 10. 上游依据

- `docs/superpowers/specs/2026-08-08-easyproduct-integration-design.md` §7.1（Admin 骨架）、§10（阶段规划）
- `docs/frontend-guidelines.md` §2.4（composables 签名）、§2.5（查询组件封装）、§5（DoD）
- `docs/mock-guidelines.md`（契约与退役）
- `docs/superpowers/plans/2026-08-08-frontend-first-development.md` F2 蓝图（F2-1 Basic 9 页、F2-0 通用组件）
- `router/modules/basic.ts:40` 注释预告「批次 3~4 路由（announcement/setting/profile）」
