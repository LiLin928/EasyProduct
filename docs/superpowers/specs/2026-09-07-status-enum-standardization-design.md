# EasyProduct 状态字段统一规范设计

> 将所有枚举状态字段和布尔字段统一改为 int 类型

**设计日期**：2026-09-07
**设计者**：Claude Code
**优先级**：P0（高优先级）

---

## 一、背景与目标

### 1.1 当前问题

- **枚举字段类型不统一**：当前使用字符串类型（如 `'enabled'`, `'pending'`），性能较差
- **布尔字段使用 string/boolean**：数据库使用 `VARCHAR` 或 `BOOLEAN`，查询效率低
- **类型转换复杂**：前端需要频繁进行字符串比较，容易出错
- **数据库存储空间浪费**：字符串占用更多存储空间

### 1.2 目标

1. **统一类型**：所有状态字段使用 `int` 类型
2. **提升性能**：数值比较比字符串比较更高效
3. **减少存储**：`int` 类型占用更少存储空间
4. **提高可读性**：通过常量定义提升代码可读性
5. **便于维护**：统一的规范更易于维护和扩展

---

## 二、核心规范

### 2.1 枚举状态字段规范（int 类型）

#### 2.1.1 通用状态枚举

```typescript
/**
 * 通用状态：0=禁用，1=启用
 */
type Status = 0 | 1
```

**映射规则：**
| 字符串值 | int 值 | 说明 |
|---------|--------|------|
| `disabled` | `0` | 禁用 |
| `enabled` | `1` | 启用 |

**应用场景：**
- 用户状态：`status: 1`（启用）
- 角色状态：`status: 0`（禁用）
- 菜单状态：`status: 1`（启用）
- 字典状态：`status: 0`（禁用）

#### 2.1.2 订单状态枚举

```typescript
/**
 * 订单状态
 * 0=已取消，1=待支付，2=已支付，3=已发货，4=已完成，5=已退款
 */
type OrderStatus = 0 | 1 | 2 | 3 | 4 | 5
```

**映射规则：**
| 字符串值 | int 值 | 说明 |
|---------|--------|------|
| `cancelled` | `0` | 已取消 |
| `pending` | `1` | 待支付 |
| `paid` | `2` | 已支付 |
| `shipped` | `3` | 已发货 |
| `completed` | `4` | 已完成 |
| `refunded` | `5` | 已退款 |

#### 2.1.3 支付状态枚举

```typescript
/**
 * 支付状态
 * 0=待支付，1=成功，2=失败，3=已退款
 */
type PaymentStatus = 0 | 1 | 2 | 3
```

**映射规则：**
| 字符串值 | int 值 | 说明 |
|---------|--------|------|
| `pending` | `0` | 待支付 |
| `success` | `1` | 成功 |
| `failed` | `2` | 失败 |
| `refunded` | `3` | 已退款 |

#### 2.1.4 会员状态枚举

```typescript
/**
 * 会员状态
 * 0=不活跃，1=活跃
 */
type MemberStatus = 0 | 1
```

**映射规则：**
| 字符串值 | int 值 | 说明 |
|---------|--------|------|
| `inactive` | `0` | 不活跃 |
| `active` | `1` | 活跃 |

#### 2.1.5 流程状态枚举

```typescript
/**
 * 流程状态
 * 0=草稿，1=已提交，2=已审批，3=已完成，4=已取消，5=已拒绝
 */
type WorkflowStatus = 0 | 1 | 2 | 3 | 4 | 5
```

**映射规则：**
| 字符串值 | int 值 | 说明 |
|---------|--------|------|
| `draft` | `0` | 草稿 |
| `submitted` | `1` | 已提交 |
| `approved` | `2` | 已审批 |
| `completed` | `3` | 已完成 |
| `cancelled` | `4` | 已取消 |
| `rejected` | `5` | 已拒绝 |

### 2.2 布尔字段规范（改为 int 类型）

#### 2.2.1 布尔字段定义

```typescript
/**
 * 布尔值：0=否，1=是
 */
type BoolValue = 0 | 1
```

**映射规则：**
| boolean 值 | int 值 | 说明 |
|-----------|--------|------|
| `false` | `0` | 否 |
| `true` | `1` | 是 |

#### 2.2.2 应用场景

| 字段名 | 类型 | 说明 |
|--------|------|------|
| `isDefault` | `int` | 是否默认：0=否，1=是 |
| `visible` | `int` | 是否可见：0=否，1=是 |
| `isTop` | `int` | 是否置顶：0=否，1=是 |
| `isRead` | `int` | 是否已读：0=否，1=是 |
| `selected` | `int` | 是否选中：0=否，1=是 |

---

## 三、数据库设计规范

### 3.1 状态字段（INT 类型）

```sql
-- 通用状态字段
status INT DEFAULT 1 COMMENT '状态：0=禁用，1=启用'

-- 订单状态字段
order_status INT DEFAULT 1 COMMENT '订单状态：0=已取消，1=待支付，2=已支付，3=已发货，4=已完成，5=已退款'

-- 支付状态字段
payment_status INT DEFAULT 0 COMMENT '支付状态：0=待支付，1=成功，2=失败，3=已退款'

-- 会员状态字段
member_status INT DEFAULT 1 COMMENT '会员状态：0=不活跃，1=活跃'
```

### 3.2 布尔字段（改为 INT）

```sql
-- 布尔字段改为 INT
is_default INT DEFAULT 0 COMMENT '是否默认：0=否，1=是'
visible INT DEFAULT 1 COMMENT '是否可见：0=否，1=是'
is_top INT DEFAULT 0 COMMENT '是否置顶：0=否，1=是'
is_read INT DEFAULT 0 COMMENT '是否已读：0=否，1=是'
selected INT DEFAULT 1 COMMENT '是否选中：0=否，1=是'
```

### 3.3 索引优化

```sql
-- 状态字段添加索引（提升查询性能）
CREATE INDEX idx_status ON basic_user(status);
CREATE INDEX idx_order_status ON mall_order(order_status);
CREATE INDEX idx_payment_status ON mall_payment(payment_status);
```

---

## 四、后端设计规范（C#）

### 4.1 枚举定义

#### 4.1.1 通用枚举

```csharp
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
/// 订单状态枚举
/// </summary>
public enum OrderStatus
{
    /// <summary>
    /// 已取消
    /// </summary>
    Cancelled = 0,
    
    /// <summary>
    /// 待支付
    /// </summary>
    Pending = 1,
    
    /// <summary>
    /// 已支付
    /// </summary>
    Paid = 2,
    
    /// <summary>
    /// 已发货
    /// </summary>
    Shipped = 3,
    
    /// <summary>
    /// 已完成
    /// </summary>
    Completed = 4,
    
    /// <summary>
    /// 已退款
    /// </summary>
    Refunded = 5
}

/// <summary>
/// 支付状态枚举
/// </summary>
public enum PaymentStatus
{
    /// <summary>
    /// 待支付
    /// </summary>
    Pending = 0,
    
    /// <summary>
    /// 成功
    /// </summary>
    Success = 1,
    
    /// <summary>
    /// 失败
    /// </summary>
    Failed = 2,
    
    /// <summary>
    /// 已退款
    /// </summary>
    Refunded = 3
}

/// <summary>
/// 会员状态枚举
/// </summary>
public enum MemberStatus
{
    /// <summary>
    /// 不活跃
    /// </summary>
    Inactive = 0,
    
    /// <summary>
    /// 活跃
    /// </summary>
    Active = 1
}

/// <summary>
/// 流程状态枚举
/// </summary>
public enum WorkflowStatus
{
    /// <summary>
    /// 草稿
    /// </summary>
    Draft = 0,
    
    /// <summary>
    /// 已提交
    /// </summary>
    Submitted = 1,
    
    /// <summary>
    /// 已审批
    /// </summary>
    Approved = 2,
    
    /// <summary>
    /// 已完成
    /// </summary>
    Completed = 3,
    
    /// <summary>
    /// 已取消
    /// </summary>
    Cancelled = 4,
    
    /// <summary>
    /// 已拒绝
    /// </summary>
    Rejected = 5
}
```

#### 4.1.2 枚举文件位置

```
EasyProduct.WebApi/
└── src/
    └── Core/
        └── Enums/
            ├── Status.cs
            ├── OrderStatus.cs
            ├── PaymentStatus.cs
            ├── MemberStatus.cs
            └── WorkflowStatus.cs
```

### 4.2 实体类示例

```csharp
/// <summary>
/// 菜单实体
/// </summary>
[SugarTable("basic_menu")]
public class Menu
{
    /// <summary>
    /// 主键ID
    /// </summary>
    [SugarColumn(IsPrimaryKey = true)]
    public string Id { get; set; }
    
    /// <summary>
    /// 父菜单ID
    /// </summary>
    public string ParentId { get; set; }
    
    /// <summary>
    /// 菜单名称
    /// </summary>
    public string Name { get; set; }
    
    /// <summary>
    /// 路由路径
    /// </summary>
    public string Path { get; set; }
    
    /// <summary>
    /// 标题i18n key
    /// </summary>
    public string TitleKey { get; set; }
    
    /// <summary>
    /// 图标名称
    /// </summary>
    public string Icon { get; set; }
    
    /// <summary>
    /// 排序
    /// </summary>
    public int Sort { get; set; }
    
    /// <summary>
    /// 状态：0=禁用，1=启用
    /// </summary>
    public Status Status { get; set; }
    
    /// <summary>
    /// 是否可见：0=否，1=是
    /// </summary>
    public int Visible { get; set; }
    
    /// <summary>
    /// 子菜单
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public List<Menu> Children { get; set; }
}
```

### 4.3 DTO 示例

```csharp
/// <summary>
/// 菜单DTO
/// </summary>
public class MenuDto
{
    /// <summary>
    /// 主键ID
    /// </summary>
    public string Id { get; set; }
    
    /// <summary>
    /// 父菜单ID
    /// </summary>
    public string ParentId { get; set; }
    
    /// <summary>
    /// 菜单名称
    /// </summary>
    public string Name { get; set; }
    
    /// <summary>
    /// 路由路径
    /// </summary>
    public string Path { get; set; }
    
    /// <summary>
    /// 标题i18n key
    /// </summary>
    public string TitleKey { get; set; }
    
    /// <summary>
    /// 图标名称
    /// </summary>
    public string Icon { get; set; }
    
    /// <summary>
    /// 排序
    /// </summary>
    public int Sort { get; set; }
    
    /// <summary>
    /// 状态：0=禁用，1=启用
    /// </summary>
    public int Status { get; set; }
    
    /// <summary>
    /// 是否可见：0=否，1=是
    /// </summary>
    public int Visible { get; set; }
    
    /// <summary>
    /// 子菜单
    /// </summary>
    public List<MenuDto> Children { get; set; }
}
```

### 4.4 Service 业务逻辑示例

```csharp
/// <summary>
/// 获取启用的菜单树
/// </summary>
public async Task<List<MenuDto>> GetEnabledMenuTree()
{
    var menus = await _menuRepository
        .Where(m => m.Status == Status.Enabled)  // 使用枚举比较
        .OrderBy(m => m.Sort)
        .ToListAsync();
    
    return BuildMenuTree(menus);
}

/// <summary>
/// 创建菜单
/// </summary>
public async Task<string> CreateMenu(CreateMenuDto dto)
{
    var menu = new Menu
    {
        Id = Guid.NewGuid().ToString(),
        ParentId = dto.ParentId,
        Name = dto.Name,
        Path = dto.Path,
        TitleKey = dto.TitleKey,
        Icon = dto.Icon,
        Sort = dto.Sort,
        Status = Status.Enabled,    // 默认启用
        Visible = 1                 // 默认可见
    };
    
    await _menuRepository.InsertAsync(menu);
    return menu.Id;
}
```

---

## 五、前端设计规范（TypeScript）

### 5.1 类型定义

```typescript
// src/types/common.ts

/**
 * 通用状态：0=禁用，1=启用
 */
export type Status = 0 | 1

/**
 * 订单状态：0=已取消，1=待支付，2=已支付，3=已发货，4=已完成，5=已退款
 */
export type OrderStatus = 0 | 1 | 2 | 3 | 4 | 5

/**
 * 支付状态：0=待支付，1=成功，2=失败，3=已退款
 */
export type PaymentStatus = 0 | 1 | 2 | 3

/**
 * 会员状态：0=不活跃，1=活跃
 */
export type MemberStatus = 0 | 1

/**
 * 流程状态：0=草稿，1=已提交，2=已审批，3=已完成，4=已取消，5=已拒绝
 */
export type WorkflowStatus = 0 | 1 | 2 | 3 | 4 | 5

/**
 * 布尔值：0=否，1=是
 */
export type BoolValue = 0 | 1
```

### 5.2 状态常量

```typescript
// src/constants/status.ts

/**
 * 通用状态常量
 */
export const STATUS = {
  DISABLED: 0,  // 禁用
  ENABLED: 1,   // 启用
} as const

/**
 * 订单状态常量
 */
export const ORDER_STATUS = {
  CANCELLED: 0,  // 已取消
  PENDING: 1,    // 待支付
  PAID: 2,       // 已支付
  SHIPPED: 3,    // 已发货
  COMPLETED: 4,  // 已完成
  REFUNDED: 5,   // 已退款
} as const

/**
 * 支付状态常量
 */
export const PAYMENT_STATUS = {
  PENDING: 0,    // 待支付
  SUCCESS: 1,    // 成功
  FAILED: 2,     // 失败
  REFUNDED: 3,   // 已退款
} as const

/**
 * 会员状态常量
 */
export const MEMBER_STATUS = {
  INACTIVE: 0,  // 不活跃
  ACTIVE: 1,    // 活跃
} as const

/**
 * 流程状态常量
 */
export const WORKFLOW_STATUS = {
  DRAFT: 0,       // 草稿
  SUBMITTED: 1,   // 已提交
  APPROVED: 2,    // 已审批
  COMPLETED: 3,   // 已完成
  CANCELLED: 4,   // 已取消
  REJECTED: 5,    // 已拒绝
} as const

/**
 * 布尔值常量
 */
export const BOOL = {
  FALSE: 0,  // 否
  TRUE: 1,   // 是
} as const
```

### 5.3 实体接口示例

```typescript
// src/types/menu.ts

/**
 * 菜单实体
 */
export interface Menu {
  id: string
  parentId: string
  name: string
  path: string
  titleKey: string
  icon: string
  sort: number
  status: Status        // 0=禁用，1=启用
  visible: BoolValue    // 0=不可见，1=可见
  children?: Menu[]
}

/**
 * 订单实体
 */
export interface Order {
  id: string
  orderNo: string
  memberId: string
  memberName: string
  totalAmount: number
  status: OrderStatus   // 0=已取消，1=待支付，2=已支付，3=已发货，4=已完成，5=已退款
  createdAt: string
  updatedAt: string
}
```

### 5.4 i18n 翻译

```typescript
// src/locales/zh-CN/status.ts

export default {
  status: {
    '0': '禁用',
    '1': '启用',
  },
  
  orderStatus: {
    '0': '已取消',
    '1': '待支付',
    '2': '已支付',
    '3': '已发货',
    '4': '已完成',
    '5': '已退款',
  },
  
  paymentStatus: {
    '0': '待支付',
    '1': '支付成功',
    '2': '支付失败',
    '3': '已退款',
  },
  
  memberStatus: {
    '0': '不活跃',
    '1': '活跃',
  },
  
  workflowStatus: {
    '0': '草稿',
    '1': '已提交',
    '2': '已审批',
    '3': '已完成',
    '4': '已取消',
    '5': '已拒绝',
  },
  
  bool: {
    '0': '否',
    '1': '是',
  },
}
```

```typescript
// src/locales/en-US/status.ts

export default {
  status: {
    '0': 'Disabled',
    '1': 'Enabled',
  },
  
  orderStatus: {
    '0': 'Cancelled',
    '1': 'Pending',
    '2': 'Paid',
    '3': 'Shipped',
    '4': 'Completed',
    '5': 'Refunded',
  },
  
  paymentStatus: {
    '0': 'Pending',
    '1': 'Success',
    '2': 'Failed',
    '3': 'Refunded',
  },
  
  memberStatus: {
    '0': 'Inactive',
    '1': 'Active',
  },
  
  workflowStatus: {
    '0': 'Draft',
    '1': 'Submitted',
    '2': 'Approved',
    '3': 'Completed',
    '4': 'Cancelled',
    '5': 'Rejected',
  },
  
  bool: {
    '0': 'No',
    '1': 'Yes',
  },
}
```

### 5.5 使用示例

```vue
<!-- MenuList.vue -->
<template>
  <el-table :data="menuList">
    <el-table-column prop="name" label="菜单名称" />
    <el-table-column label="状态">
      <template #default="{ row }">
        <el-tag :type="row.status === STATUS.ENABLED ? 'success' : 'danger'">
          {{ $t(`status.${row.status}`) }}
        </el-tag>
      </template>
    </el-table-column>
    <el-table-column label="可见">
      <template #default="{ row }">
        {{ $t(`bool.${row.visible}`) }}
      </template>
    </el-table-column>
  </el-table>
</template>

<script setup lang="ts">
import { STATUS, BOOL } from '@/constants/status'
import type { Menu } from '@/types/menu'

const menuList = ref<Menu[]>([])
</script>
```

---

## 六、Mock 数据规范

### 6.1 Mock 数据示例

```javascript
// mock-server/src/data/menu.js

module.exports = [
  {
    id: '1',
    parentId: '0',
    name: 'System',
    path: '/system',
    titleKey: 'menu.system',
    icon: 'setting',
    sort: 1,
    status: 1,      // 1=启用
    visible: 1,     // 1=可见
  },
  {
    id: '2',
    parentId: '1',
    name: 'User',
    path: '/system/user',
    titleKey: 'menu.user',
    icon: 'user',
    sort: 1,
    status: 0,      // 0=禁用
    visible: 0,     // 0=不可见
  },
]
```

```javascript
// mock-server/src/data/order.js

module.exports = [
  {
    id: '1',
    orderNo: 'ORD-20260001',
    memberId: 'member-001',
    memberName: '张三',
    totalAmount: 299.00,
    status: 2,      // 2=已支付
    createdAt: '2026-09-07T10:00:00Z',
    updatedAt: '2026-09-07T10:00:00Z',
  },
  {
    id: '2',
    orderNo: 'ORD-20260002',
    memberId: 'member-002',
    memberName: '李四',
    totalAmount: 599.00,
    status: 4,      // 4=已完成
    createdAt: '2026-09-06T14:30:00Z',
    updatedAt: '2026-09-07T09:00:00Z',
  },
]
```

---

## 七、实施计划

### 7.1 Phase 1: 文档更新（预计 2 小时）

**任务清单：**
- [ ] 创建状态枚举参考文档（`docs/api/status-enum-reference.md`）
- [ ] 更新 `docs/backend-guidelines.md`（实体设计规范）
- [ ] 更新 `docs/api/basic-module.md`（Basic 模块）
- [ ] 更新 `docs/api/mall-module.md`（Mall 模块）
- [ ] 更新 `docs/api/crm-module.md`（CRM 模块）
- [ ] 更新 `docs/api/other-modules.md`（其他模块）
- [ ] 更新 `docs/api/api-app-module.md`（App 模块）
- [ ] 更新 `docs/api/api-site-module.md`（Site 模块）
- [ ] 更新 `docs/api/README.md`（总览文档）
- [ ] 更新 `docs/frontend-guidelines.md`（前端状态常量规范）

### 7.2 Phase 2: 后端代码修改（预计 1 天）

**任务清单：**
- [ ] 创建枚举类文件
  - [ ] `Status.cs`
  - [ ] `OrderStatus.cs`
  - [ ] `PaymentStatus.cs`
  - [ ] `MemberStatus.cs`
  - [ ] `WorkflowStatus.cs`
- [ ] 修改已实现的实体类
  - [ ] `Menu` 实体
  - [ ] `AdminUser` 实体
  - [ ] `Role` 实体
  - [ ] `Dept` 实体
  - [ ] 其他已实现实体
- [ ] 修改 DTO 类
- [ ] 调整业务逻辑代码（Service 层）
- [ ] 编写单元测试（涉钱逻辑必须）

### 7.3 Phase 3: Mock 数据修改（预计 2 小时）

**任务清单：**
- [ ] 修改 `mock-server/src/data/menu.js`
- [ ] 修改 `mock-server/src/data/user.js`
- [ ] 修改 `mock-server/src/data/role.js`
- [ ] 修改 `mock-server/src/data/order.js`
- [ ] 修改其他 Mock 数据文件
- [ ] 测试 Mock 接口

### 7.4 Phase 4: 前端代码修改（预计半天）

**任务清单：**
- [ ] 创建状态常量文件 `src/constants/status.ts`
- [ ] 修改 TypeScript 类型定义
- [ ] 更新 i18n 翻译文件
  - [ ] `zh-CN/status.ts`
  - [ ] `en-US/status.ts`
- [ ] 调整组件代码（状态判断逻辑）
- [ ] 测试前端功能

---

## 八、风险控制

### 8.1 兼容性保障

1. **后端 DTO 保持 int 类型**：确保 API 响应数据格式一致
2. **前端同步修改**：前端类型定义和业务逻辑同步更新
3. **Mock 数据一次性更新**：避免 Mock 和实际 API 不一致
4. **前后端同步部署**：确保部署时数据格式一致

### 8.2 测试验证

1. **后端单元测试**：覆盖状态判断逻辑
2. **前端组件测试**：覆盖状态显示逻辑
3. **端到端测试**：验证完整流程
4. **回归测试**：确保现有功能不受影响

### 8.3 回滚方案

1. **保留旧代码备份**：Git 分支管理
2. **数据库备份**：修改前备份数据库
3. **分阶段发布**：先发布后端，再发布前端

---

## 九、验收标准

### 9.1 文档验收

- [ ] 所有模块 API 文档已更新
- [ ] 状态枚举映射表完整准确
- [ ] 开发规范文档已更新

### 9.2 代码验收

- [ ] 后端枚举类定义完整
- [ ] 实体类字段类型正确
- [ ] DTO 字段类型正确
- [ ] 业务逻辑正确
- [ ] 单元测试通过

### 9.3 功能验收

- [ ] Mock 接口返回正确
- [ ] 前端显示正确
- [ ] 状态筛选功能正常
- [ ] 国际化翻译完整

---

## 十、附录：完整状态枚举映射表

### 10.1 Basic 模块

| 实体 | 字段 | 类型 | 值域 | 说明 |
|-----|------|------|------|------|
| AdminUser | status | Status | 0=禁用，1=启用 | 用户状态 |
| Role | status | Status | 0=禁用，1=启用 | 角色状态 |
| Menu | status | Status | 0=禁用，1=启用 | 菜单状态 |
| Menu | visible | BoolValue | 0=否，1=是 | 是否可见 |
| Dept | status | Status | 0=禁用，1=启用 | 部门状态 |
| DictType | status | Status | 0=禁用，1=启用 | 字典类型状态 |
| DictData | status | Status | 0=禁用，1=启用 | 字典数据状态 |

### 10.2 Mall 模块

| 实体 | 字段 | 类型 | 值域 | 说明 |
|-----|------|------|------|------|
| Member | status | MemberStatus | 0=不活跃，1=活跃 | 会员状态 |
| MemberLevel | status | Status | 0=禁用，1=启用 | 等级状态 |
| Coupon | status | Status | 0=禁用，1=启用 | 优惠券状态 |
| Order | status | OrderStatus | 0=已取消，1=待支付，2=已支付，3=已发货，4=已完成，5=已退款 | 订单状态 |
| PaymentRecord | status | PaymentStatus | 0=待支付，1=成功，2=失败，3=已退款 | 支付状态 |
| Address | isDefault | BoolValue | 0=否，1=是 | 是否默认地址 |

### 10.3 CRM 模块

| 实体 | 字段 | 类型 | 值域 | 说明 |
|-----|------|------|------|------|
| Customer | status | Status | 0=禁用，1=启用 | 客户状态 |
| Supplier | status | Status | 0=禁用，1=启用 | 供应商状态 |
| Currency | isDefault | BoolValue | 0=否，1=是 | 是否本位币 |
| Warehouse | status | Status | 0=禁用，1=启用 | 仓库状态 |
| StockAlert | status | Status | 0=待处理，1=已处理 | 预警状态 |
| PurchaseOrder | status | WorkflowStatus | 0=草稿，1=已提交，2=已审批，3=已完成，4=已取消 | 采购订单状态 |
| SalesOrder | status | WorkflowStatus | 0=草稿，1=已提交，2=已审批，3=已完成，4=已取消 | 销售订单状态 |
| Invoice | status | Status | 0=草稿，1=已开票，2=已作废 | 发票状态 |
| Payment | status | Status | 0=草稿，1=已完成，2=已取消 | 收付款状态 |

### 10.4 Site 模块

| 实体 | 字段 | 类型 | 值域 | 说明 |
|-----|------|------|------|------|
| SiteNews | status | Status | 0=草稿，1=已发布 | 新闻状态 |
| SiteNews | isTop | BoolValue | 0=否，1=是 | 是否置顶 |
| SiteCategory | status | Status | 0=禁用，1=启用 | 分类状态 |
| SiteBanner | status | Status | 0=禁用，1=启用 | Banner 状态 |
| SiteVideo | status | Status | 0=草稿，1=已发布 | 视频状态 |
| SiteDownload | status | Status | 0=草稿，1=已发布 | 下载状态 |

### 10.5 Product 模块

| 实体 | 字段 | 类型 | 值域 | 说明 |
|-----|------|------|------|------|
| ProductCategory | status | Status | 0=禁用，1=启用 | 分类状态 |
| ProductSpu | status | Status | 0=禁用，1=启用 | 商品状态 |
| ProductSku | status | Status | 0=禁用，1=启用 | SKU 状态 |
| ProductChannel | status | Status | 0=禁用，1=启用 | 渠道状态 |

### 10.6 App 模块

| 实体 | 字段 | 类型 | 值域 | 说明 |
|-----|------|------|------|------|
| CartItem | selected | BoolValue | 0=否，1=是 | 是否选中 |
| Order | status | OrderStatus | 0=已取消，1=待支付，2=已支付，3=已发货，4=已完成，5=已退款 | 订单状态 |
| PaymentRecord | status | PaymentStatus | 0=待支付，1=成功，2=失败，3=已退款 | 支付状态 |
| Address | isDefault | BoolValue | 0=否，1=是 | 是否默认地址 |

### 10.7 Workflow 模块

| 实体 | 字段 | 类型 | 值域 | 说明 |
|-----|------|------|------|------|
| WfDefinition | status | Status | 0=草稿，1=已发布，2=已归档 | 流程定义状态 |
| WfInstance | status | Status | 0=运行中，1=已完成，2=已取消，3=已拒绝 | 流程实例状态 |
| WfTask | status | Status | 0=待处理，1=已通过，2=已拒绝，3=已委托 | 任务状态 |

### 10.8 Ops 模块

| 实体 | 字段 | 类型 | 值域 | 说明 |
|-----|------|------|------|------|
| OpsOperateLog | status | Status | 0=失败，1=成功 | 操作状态 |
| OpsLoginLog | status | Status | 0=失败，1=成功 | 登录状态 |
| OpsTask | status | Status | 0=暂停，1=运行中，2=错误 | 任务状态 |

---

**文档版本**：v1.0
**最后更新**：2026-09-07