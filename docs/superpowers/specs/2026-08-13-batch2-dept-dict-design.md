# F2-1 批次2：部门管理与字典管理设计

> **日期：** 2026-08-13
> **状态：** 待评审
> **阶段：** F2-1 批次2（Admin Basic 模块）
> **上游依据：** `docs/superpowers/specs/2026-08-12-f2-1-basic-module-design.md`
> **参考项目：** `D:\4-MyProject\EasyProject\PCWeb\src\views\basic\dict`、`D:\4-MyProject\EasyProject\PCWeb\src\views\basic\department`

---

## 1. 背景与目标

### 1.1 背景

批次1已完成用户/角色/菜单三个核心RBAC页面。批次2需要开发部门管理和字典管理，为用户提供组织架构和字典数据的管理能力，同时为用户管理等页面提供部门选择功能。

### 1.2 目标

- 完成部门管理页面（树形结构 + 详情展示）
- 完成字典管理页面（主从结构：类型 + 数据）
- 完成部门选择组件（供其他页面复用）
- 提供完整的 API 层和 Mock 数据

### 1.3 范围

**包含：**
- 部门管理页面（左侧树 + 右侧详情）
- 字典管理页面（左侧类型列表 + 右侧数据表格）
- 部门选择组件（DeptSelect.vue）
- API 层（dept.ts、dict.ts 扩展）
- Mock 数据和路由
- i18n 翻译（中英文）

**不包含：**
- 部门拖拽排序（简化为基础树形表格）
- 字典数据导入导出
- 后端实现（属于 B 系列 P1 阶段）

---

## 2. 总体设计

### 2.1 开发顺序（方案A：最小可行路径）

1. **步骤1：部门管理页面**
   - 左侧：部门树（支持搜索、新增/编辑/删除）
   - 右侧：部门详情 + 成员列表
   - 部门选择组件

2. **步骤2：字典管理页面**
   - 左侧：字典类型列表（支持搜索、新增/编辑/删除）
   - 右侧：字典数据表格（支持分页、批量删除）

### 2.2 技术选型

| 功能 | 技术方案 | 说明 |
|------|---------|------|
| 部门树 | el-tree | 支持搜索、展开/收起、节点操作 |
| 部门详情 | el-descriptions | 描述列表展示部门信息 |
| 部门选择 | el-tree-select | 下拉树形选择器，支持搜索 |
| 字典类型列表 | el-table + 高亮当前行 | 左侧面板，320px 固定宽度 |
| 字典数据列表 | el-table + 分页 | 右侧面板，flex-1 |
| 表单弹窗 | useForm + useDialog | 复用 F2-0 封装层 |

---

## 3. 文件结构

```
EasyProduct.Admin/src/
├── views/basic/
│   ├── dept/                    # 部门管理（新建）
│   │   ├── index.vue           # 主从布局页面
│   │   └── components/
│   │       └── DeptFormDialog.vue # 部门表单弹窗
│   └── dict/                    # 字典管理（新建）
│       ├── index.vue           # 主从布局页面
│       └── components/
│           ├── DictTypeFormDialog.vue # 字典类型表单弹窗
│           └── DictDataFormDialog.vue  # 字典数据表单弹窗
├── api/basic/
│   ├── dept.ts                  # 部门API（新建）
│   └── dict.ts                  # 字典API（扩展）
├── components/common/
│   └── DeptSelect.vue           # 部门选择组件（新建）
└── types/
    └── basic.ts                 # 类型定义（扩展）

mock-server/src/
├── data/admin/
│   ├── dept.ts                  # 部门Mock数据（扩展）
│   └── dict.ts                  # 字典Mock数据（新建）
└── routes/admin/
    ├── dept.ts                  # 部门路由（扩展）
    └── dict.ts                  # 字典路由（新建）

EasyProduct.Admin/src/i18n/locales/
├── zh-CN/
│   └── basic.json               # 基础模块翻译（扩展 dept/dict 部分）
└── en-US/
    └── basic.json               # 基础模块翻译（扩展 dept/dict 部分）
```

---

## 4. 部门管理设计

### 4.1 页面布局

```
┌─────────────────────────────────────────────────────────┐
│ 部门管理                                                  │
├──────────────┬──────────────────────────────────────────┤
│ 部门树       │ 部门详情                                   │
│              │                                           │
│ [新增根部门]  │ ┌─────────────────────────────────────┐  │
│              │ │ 部门名称：技术部                       │  │
│ [搜索框]     │ │ 部门编码：TECH                        │  │
│              │ │ 部门路径：总公司/技术部                 │  │
│ ○ 总公司     │ │ 层级：2                               │  │
│   ├─ 技术部  │ │ 负责人：李经理                         │  │
│   │  ├─ 前端组│ │ 联系电话：13800138001                  │  │
│   │  └─ 后端组│ │ 邮箱：li@company.com                   │  │
│   └─ 销售部  │ │ 成员数量：15                           │  │
│              │ │ 描述：技术研发部门                      │  │
│              │ └─────────────────────────────────────┘  │
│              │                                           │
│              │ 部门成员                                   │
│              │ ┌─────────────────────────────────────┐  │
│              │ │ 用户名 │ 真实姓名 │ 手机号 │ ...     │  │
│              │ │ user01 │ 张三     │ 138... │ ...     │  │
│              │ └─────────────────────────────────────┘  │
└──────────────┴──────────────────────────────────────────┘
```

### 4.2 核心功能

#### 左侧树区域

1. **部门树（el-tree）**
   - 默认展开所有节点
   - 支持搜索过滤（本地搜索，不调接口）
   - 高亮当前选中节点
   - hover 显示操作按钮：新增子部门/编辑/删除

2. **树节点操作**
   - 新增根部门：顶部按钮
   - 新增子部门：节点 hover 按钮
   - 编辑：节点 hover 按钮 / 右侧详情区编辑按钮
   - 删除：节点 hover 按钮，有子节点时禁用

#### 右侧详情区域

1. **部门详情（el-descriptions）**
   - 显示字段：名称、编码、路径、层级、负责人、电话、邮箱、成员数量、描述
   - 编辑按钮：编辑当前选中部门

2. **部门成员列表（el-table）**
   - 显示字段：用户名、真实姓名、手机号、邮箱、状态、角色
   - 只读展示，不提供操作

### 4.3 部门选择组件

```vue
<!-- src/components/common/DeptSelect.vue -->
<template>
  <el-tree-select
    v-model="selectedValue"
    :data="treeData"
    :props="{ children: 'children', label: 'name', value: 'id' }"
    :placeholder="placeholder"
    :disabled="disabled"
    clearable
    filterable
    check-strictly
    :render-after-expand="false"
  />
</template>
```

**特性：**
- 使用 el-tree-select（Element Plus 原生组件）
- 支持搜索过滤
- 支持清空
- 单选模式（check-strictly 禁止选择父节点自动选中子节点）

---

## 5. 字典管理设计

### 5.1 页面布局

```
┌─────────────────────────────────────────────────────────┐
│ 字典管理                                                  │
├──────────────────┬──────────────────────────────────────┤
│ 字典类型          │ 字典数据 - 通用状态                    │
│                  │                                       │
│ [新增类型]        │ [批量删除] [新增数据]                   │
│                  │                                       │
│ [搜索框]         │ ┌───────────────────────────────────┐│
│                  │ │ □ │ 标签   │ 值     │ 排序 │ 操作  ││
│ 通用状态 (启用)   │ │ □ │ 启用   │ enabled│ 1    │ 编辑  ││
│ 用户性别 (启用)   │ │ □ │ 禁用   │ disabled│ 2   │ 删除  ││
│ 订单状态 (启用)   │ └───────────────────────────────────┘│
│                  │                                       │
│                  │ 分页：共 2 条                          │
└──────────────────┴──────────────────────────────────────┘
```

### 5.2 核心功能

#### 左侧类型区域

1. **字典类型列表（el-table）**
   - 固定宽度：320px
   - 支持搜索过滤（本地搜索）
   - 高亮当前选中行（highlight-current-row）
   - 状态开关：直接调用更新接口
   - 操作按钮：编辑/删除

2. **类型管理**
   - 新增类型：顶部按钮
   - 编辑类型：行内按钮
   - 删除类型：行内按钮，有确认对话框

#### 右侧数据区域

1. **字典数据表格（el-table）**
   - 未选择类型时显示 el-empty 提示
   - 支持复选框选择（批量删除）
   - 状态开关：直接调用更新接口
   - 操作按钮：编辑/删除
   - 分页组件

2. **数据管理**
   - 新增数据：顶部按钮（需先选择类型）
   - 编辑数据：行内按钮
   - 删除数据：行内按钮，有确认对话框
   - 批量删除：顶部按钮（需选中数据）

---

## 6. API 契约

### 6.1 部门管理 API

```typescript
// api/basic/dept.ts

/** 部门树 */
export const getDeptTree = () =>
  get<Dept[]>('/api/admin/basic/dept/tree')

/** 部门详情 */
export const getDeptDetail = (id: string) =>
  get<Dept>(`/api/admin/basic/dept/${id}`)

/** 部门成员列表 */
export const getDeptUsers = (deptId: string) =>
  get<UserInfo[]>(`/api/admin/basic/dept/${deptId}/users`)

/** 新增部门 */
export const createDept = (data: DeptCreateParams) =>
  post<{ id: string }>('/api/admin/basic/dept', data)

/** 编辑部门 */
export const updateDept = (id: string, data: DeptUpdateParams) =>
  put<{ id: string }>(`/api/admin/basic/dept/${id}`, data)

/** 删除部门 */
export const deleteDept = (id: string) =>
  del<null>(`/api/admin/basic/dept/${id}`)
```

### 6.2 字典管理 API

```typescript
// api/basic/dict.ts（扩展）

// 已有：字典数据查询（用于渲染）
export const getDictData = (typeCode: string) =>
  get<DictItem[]>('/api/admin/basic/dict-data', { typeCode })

// 新增：字典类型管理
/** 字典类型列表（分页） */
export const getDictTypeList = (params: DictTypeQuery) =>
  get<PageResult<DictType>>('/api/admin/basic/dict-type/list', params)

/** 新增字典类型 */
export const createDictType = (data: DictTypeCreateParams) =>
  post<{ id: string }>('/api/admin/basic/dict-type', data)

/** 编辑字典类型 */
export const updateDictType = (id: string, data: Partial<DictTypeCreateParams>) =>
  put<{ id: string }>(`/api/admin/basic/dict-type/${id}`, data)

/** 删除字典类型 */
export const deleteDictType = (id: string) =>
  del<null>(`/api/admin/basic/dict-type/${id}`)

// 新增：字典数据管理
/** 字典数据列表（分页） */
export const getDictDataList = (params: DictDataQuery) =>
  get<PageResult<DictData>>('/api/admin/basic/dict-data/list', params)

/** 新增字典数据 */
export const createDictData = (data: DictDataCreateParams) =>
  post<{ id: string }>('/api/admin/basic/dict-data', data)

/** 编辑字典数据 */
export const updateDictData = (id: string, data: Partial<DictDataCreateParams>) =>
  put<{ id: string }>(`/api/admin/basic/dict-data/${id}`, data)

/** 删除字典数据 */
export const deleteDictData = (id: string) =>
  del<null>(`/api/admin/basic/dict-data/${id}`)

/** 批量删除字典数据 */
export const deleteDictDataBatch = (ids: string[]) =>
  post<null>('/api/admin/basic/dict-data/batch-delete', { ids })
```

---

## 7. 数据模型

### 7.1 部门模型

```typescript
/** 部门（完整模型） */
export interface Dept {
  id: string
  parentId: string
  name: string
  code: string
  sort: number
  status: 'enabled' | 'disabled'
  leaderName?: string      // 部门负责人
  phone?: string           // 联系电话
  email?: string           // 邮箱
  fullPath?: string        // 部门路径（如：总公司/技术部/前端组）
  level?: number           // 层级
  memberCount?: number     // 成员数量
  description?: string     // 描述
  children?: Dept[]
}

/** 部门创建参数 */
export interface DeptCreateParams {
  parentId: string
  name: string
  code: string
  sort: number
  status: 'enabled' | 'disabled'
  leaderName?: string
  phone?: string
  email?: string
  description?: string
}
```

### 7.2 字典模型

```typescript
/** 字典类型 */
export interface DictType {
  id: string
  name: string
  code: string
  status: 'enabled' | 'disabled'
  remark: string
}

/** 字典类型查询参数 */
export interface DictTypeQuery extends PageQuery {
  name?: string
  code?: string
}

/** 字典数据（管理页面用，区别于 DictItem） */
export interface DictData {
  id: string
  typeCode: string
  value: string
  labelKey: string
  sort: number
  status: 'enabled' | 'disabled'
}

/** 字典数据查询参数 */
export interface DictDataQuery extends PageQuery {
  typeCode: string
}

/** 字典数据创建参数 */
export interface DictDataCreateParams {
  typeCode: string
  value: string
  labelKey: string
  sort: number
  status: 'enabled' | 'disabled'
}
```

---

## 8. Mock 数据设计

### 8.1 部门 Mock 数据

```typescript
// mock-server/src/data/admin/dept.ts（扩展）

export const DEPTS: Dept[] = [
  {
    id: guid(),
    parentId: '0',
    name: '总公司',
    code: 'ROOT',
    sort: 1,
    status: 'enabled',
    leaderName: '张总',
    phone: '13800138000',
    email: 'zhang@company.com',
    fullPath: '总公司',
    level: 1,
    memberCount: 5,
    description: '公司总部',
    children: [
      {
        id: guid(),
        parentId: '1',
        name: '技术部',
        code: 'TECH',
        sort: 1,
        status: 'enabled',
        leaderName: '李经理',
        phone: '13800138001',
        email: 'li@company.com',
        fullPath: '总公司/技术部',
        level: 2,
        memberCount: 15,
        description: '技术研发部门',
        children: [
          {
            id: guid(),
            parentId: '2',
            name: '前端组',
            code: 'TECH_FE',
            sort: 1,
            status: 'enabled',
            fullPath: '总公司/技术部/前端组',
            level: 3,
            memberCount: 5,
          },
          {
            id: guid(),
            parentId: '2',
            name: '后端组',
            code: 'TECH_BE',
            sort: 2,
            status: 'enabled',
            fullPath: '总公司/技术部/后端组',
            level: 3,
            memberCount: 8,
          },
        ],
      },
      {
        id: guid(),
        parentId: '1',
        name: '销售部',
        code: 'SALES',
        sort: 2,
        status: 'enabled',
        leaderName: '王经理',
        fullPath: '总公司/销售部',
        level: 2,
        memberCount: 10,
      },
    ],
  },
]
```

### 8.2 字典 Mock 数据

```typescript
// mock-server/src/data/admin/dict.ts（新建）

// 字典类型种子数据
export const DICT_TYPES: DictType[] = [
  {
    id: guid(),
    name: '通用状态',
    code: 'common_status',
    status: 'enabled',
    remark: '通用状态字典',
  },
  {
    id: guid(),
    name: '用户性别',
    code: 'user_gender',
    status: 'enabled',
    remark: '用户性别字典',
  },
  {
    id: guid(),
    name: '订单状态',
    code: 'order_status',
    status: 'enabled',
    remark: '订单状态字典',
  },
]

// 字典数据种子数据
export const DICT_DATA: DictData[] = [
  // 通用状态
  {
    id: guid(),
    typeCode: 'common_status',
    value: 'enabled',
    labelKey: 'common.status.enabled',
    sort: 1,
    status: 'enabled',
  },
  {
    id: guid(),
    typeCode: 'common_status',
    value: 'disabled',
    labelKey: 'common.status.disabled',
    sort: 2,
    status: 'enabled',
  },
  // 用户性别
  {
    id: guid(),
    typeCode: 'user_gender',
    value: 'male',
    labelKey: 'common.gender.male',
    sort: 1,
    status: 'enabled',
  },
  {
    id: guid(),
    typeCode: 'user_gender',
    value: 'female',
    labelKey: 'common.gender.female',
    sort: 2,
    status: 'enabled',
  },
]
```

---

## 9. i18n 翻译键值

### 9.1 部门管理翻译

**zh-CN：**
```json
{
  "dept": {
    "title": "部门管理",
    "tree": {
      "title": "部门树",
      "addRoot": "新增根部门",
      "addChild": "新增子部门",
      "edit": "编辑",
      "delete": "删除",
      "searchPlaceholder": "搜索部门名称"
    },
    "detail": {
      "selectDept": "请选择部门",
      "selectDeptTip": "请在左侧选择部门查看详情",
      "name": "部门名称",
      "code": "部门编码",
      "path": "部门路径",
      "level": "层级",
      "leader": "部门负责人",
      "phone": "联系电话",
      "email": "邮箱",
      "memberCount": "成员数量",
      "description": "描述"
    },
    "member": {
      "title": "部门成员",
      "userName": "用户名",
      "realName": "真实姓名",
      "phone": "手机号",
      "email": "邮箱",
      "status": "状态",
      "roles": "角色"
    },
    "form": {
      "parentId": "上级部门",
      "name": "部门名称",
      "code": "部门编码",
      "sort": "排序",
      "status": "状态",
      "leaderName": "部门负责人",
      "phone": "联系电话",
      "email": "邮箱",
      "description": "描述"
    },
    "message": {
      "hasChildren": "存在子部门，无法删除",
      "deleteConfirm": "确定要删除部门【{name}】吗？",
      "deleteSuccess": "删除成功"
    }
  }
}
```

### 9.2 字典管理翻译

**zh-CN：**
```json
{
  "dict": {
    "title": "字典管理",
    "dictType": "字典类型",
    "dictData": "字典数据",
    "addType": "新增类型",
    "editType": "编辑类型",
    "deleteType": "删除类型",
    "addData": "新增数据",
    "editData": "编辑数据",
    "deleteData": "删除数据",
    "batchDelete": "批量删除",
    "searchTypePlaceholder": "搜索类型名称",
    "selectDictType": "请在左侧选择字典类型",
    "typeName": "类型名称",
    "typeCode": "类型编码",
    "dataLabel": "字典标签",
    "dataValue": "字典值",
    "dataSort": "排序",
    "status": "状态",
    "operation": "操作",
    "enabled": "已启用",
    "disabled": "已禁用",
    "deleteTypeConfirm": "确定要删除该字典类型吗？",
    "deleteDataConfirm": "确定要删除该字典数据吗？",
    "deleteDatasConfirm": "确定要删除选中的 {count} 条字典数据吗？",
    "deleteTypeSuccess": "删除字典类型成功",
    "deleteDataSuccess": "删除字典数据成功",
    "form": {
      "typeName": "类型名称",
      "typeCode": "类型编码",
      "remark": "备注",
      "dataLabel": "字典标签",
      "dataValue": "字典值",
      "dataSort": "排序"
    }
  }
}
```

---

## 10. 实现要点

### 10.1 部门管理实现要点

1. **树节点搜索**
   - 使用 el-tree 的 `filter-node-method` 属性实现本地搜索
   - 监听搜索框变化，调用 `treeRef.value?.filter(val)`

2. **树节点操作按钮**
   - 使用 el-tree 的默认插槽自定义节点内容
   - hover 显示操作按钮（CSS `:hover` 选择器）

3. **删除限制**
   - 有子节点时禁用删除按钮（`:disabled="data.children && data.children.length > 0"`）
   - 删除前显示确认对话框

4. **部门详情同步**
   - 点击树节点时更新右侧详情
   - 编辑保存后需要刷新树并重新选中当前节点

### 10.2 字典管理实现要点

1. **主从联动**
   - 左侧类型列表使用 `highlight-current-row` 高亮当前行
   - 点击类型行时加载右侧数据列表
   - 默认选中第一个类型

2. **类型搜索**
   - 本地搜索，不调接口
   - 使用 computed 过滤类型列表

3. **数据表格分页**
   - 使用 el-pagination 组件
   - 监听 `size-change` 和 `current-change` 事件重新加载数据

4. **批量删除**
   - 使用 el-table 的 `selection` 列
   - 监听 `selection-change` 事件收集选中行
   - 批量删除前显示确认对话框

### 10.3 部门选择组件实现要点

1. **组件设计**
   - 使用 el-tree-select 组件
   - 支持双向绑定（v-model）
   - 支持搜索、清空、禁用

2. **数据加载**
   - 组件内部调用 `getDeptTree()` 获取部门树
   - 将树形数据转换为 el-tree-select 需要的格式

---

## 11. 验收标准

### 11.1 功能验收

**部门管理：**
- [ ] 部门树可正常展示、搜索
- [ ] 可新增根部门
- [ ] 可新增子部门（指定父部门）
- [ ] 可编辑部门信息
- [ ] 可删除部门（有子部门时禁用删除）
- [ ] 右侧详情可正常展示部门信息
- [ ] 右侧成员列表可正常展示
- [ ] 部门选择组件可在其他页面正常使用

**字典管理：**
- [ ] 字典类型列表可正常展示、搜索
- [ ] 可新增/编辑/删除字典类型
- [ ] 字典类型状态开关可正常切换
- [ ] 点击类型可加载对应数据
- [ ] 字典数据表格可正常展示、分页
- [ ] 可新增/编辑/删除字典数据
- [ ] 字典数据状态开关可正常切换
- [ ] 可批量删除字典数据

### 11.2 代码验收

- [ ] `pnpm type-check` 零错误
- [ ] `pnpm lint` 通过
- [ ] `pnpm check:i18n` 无硬编码中文
- [ ] API 调用使用统一的 request 工具
- [ ] 状态字段使用 `"enabled"/"disabled"` 字符串

### 11.3 集成验收

- [ ] 用户管理页面可使用部门选择组件
- [ ] 用户管理页面部门字段可正常显示部门名称
- [ ] 字典数据可通过 useDict 正常渲染

---

## 12. 风险与依赖

### 12.1 已知风险

| 风险 | 影响 | 缓解措施 |
|------|------|---------|
| 部门树数据量大时性能问题 | 页面加载慢 | 后续支持懒加载，Mock 数据控制在 20 个节点内 |
| 字典类型删除时数据未清理 | 数据不一致 | 删除前检查是否有数据，有数据时提示用户 |
| 部门选择组件数据未同步 | 选择后数据未更新 | 组件内部缓存树数据，提供刷新方法 |

### 12.2 依赖项

| 依赖 | 状态 | 说明 |
|------|------|------|
| F2-0 封装层 | 已完成 | useDialog/useForm/useLocale |
| Element Plus | 已安装 | el-tree/el-tree-select/el-descriptions |
| 批次1 用户管理 | 已完成 | 需要集成部门选择组件 |

---

## 13. 后续计划

批次2完成后，继续开发批次3：

- **批次3**：公告管理（富文本编辑）、系统参数（配置管理）
- **批次4**：个人中心、工作台布局

---

**设计版本：** v1.0
**最后更新：** 2026-08-13