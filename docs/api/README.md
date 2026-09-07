# EasyProduct Mock Server API 接口文档

> 本文档基于 mock-server 梳理，用于指导后端 EasyProduct.WebApi 开发

## 概览

### 技术栈
- **框架**: Express + TypeScript
- **Mock 数据**: MockJS
- **端口**: 7700
- **路由前缀**: `/api`

### 统一响应格式

```typescript
interface ApiResponse<T> {
  code: number       // 200 成功；400 参数错误；401 未授权；403 无权限；404 不存在；500 服务错误
  message: string    // 提示信息
  data: T            // 业务数据
  timestamp: number  // 时间戳
}

interface PageResult<T> {
  list: T[]          // 数据列表
  total: number      // 总条数
  pageIndex: number  // 当前页码（从 1 开始）
  pageSize: number   // 每页条数
  totalPages: number // 总页数
  hasNextPage: boolean
  hasPrevPage: boolean
}
```

### 模块划分

| 模块 | 路由前缀 | 说明 | 数据表前缀 |
|------|---------|------|-----------|
| Basic | `/api/admin` | 基础管理（认证、用户、角色、菜单、部门、字典、配置） | `basic_` |
| Site | `/api/site` | 官网公开内容（首页、新闻、产品、视频、下载、关于、联系） | `site_` |
| Product | `/api/admin/product-*` | 商品管理（分类、SPU、渠道） | `product_` |
| Mall | `/api/admin/mall-*` + `/api/app` | 商城（会员、等级、积分、优惠券、订单、支付、地址） | `mall_` |
| CRM | `/api/admin/crm-*` | 客户关系管理（客户、供应商、进销存、财务） | `crm_` |
| Workflow | `/api/admin/wf-*` + `/api/admin/workflow-*` | 工作流（我的申请、待办、已办、流程定义、设计器） | `wf_` |
| Report | `/api/admin/rpt-*` | 报表（数据源、报表定义、列模板） | `rpt_` |
| Ops | `/api/admin/ops-*` | 运维管理（操作日志、登录日志、定时任务、任务日志） | `ops_` |

### 认证方式

- **管理端 Admin**: JWT Token，通过 `/api/admin/auth/login` 获取
- **小程序会员 App**: JWT Token，通过 `/api/app/auth/login` 获取
- **官网 Site**: 公开接口，无需认证

---

## 详细接口文档

以下模块详细接口请查看对应文档：

- [Basic 模块](./api/basic-module.md) - 基础管理
- [Site 模块](./api/site-module.md) - 官网内容
- [Product 模块](./api/product-module.md) - 商品管理
- [Mall 模块](./api/mall-module.md) - 商城管理
- [CRM 模块](./api/crm-module.md) - 客户关系管理
- [App 模块](./api/app-module.md) - 小程序会员端
- [Workflow 模块](./api/workflow-module.md) - 工作流
- [Report 模块](./api/report-module.md) - 报表
- [Ops 模块](./api/ops-module.md) - 运维管理

---

## 实体对象总览

### Basic 模块实体
- `AdminUser` - 管理员用户
- `Role` - 角色
- `Menu` - 菜单
- `Department` - 部门
- `Dictionary` - 字典
- `Config` - 系统配置
- `Announcement` - 公告
- `File` - 文件

### Site 模块实体
- `SiteNews` - 新闻
- `SiteCategory` - 分类
- `SiteBanner` - Banner
- `SiteVideo` - 视频
- `SiteDownload` - 下载
- `SiteAbout` - 关于我们
- `SiteContact` - 联系方式
- `SiteInquiry` - 在线咨询

### Product 模块实体
- `ProductCategory` - 商品分类
- `ProductSpu` - 商品SPU
- `ProductChannel` - 商品渠道

### Mall 模块实体
- `MallMember` - 会员
- `MallLevel` - 会员等级
- `MallPoints` - 积分记录
- `MallCoupon` - 优惠券
- `MallOrder` - 订单
- `MallPayment` - 支付记录
- `MallAddress` - 收货地址
- `Cart` - 购物车

### CRM 模块实体
- `CrmCustomer` - 客户
- `CrmSupplier` - 供应商
- `CrmCurrency` - 币种
- `CrmTaxRate` - 税率
- `CrmSalesOrder` - 销售订单
- `CrmPurchaseOrder` - 采购订单
- `CrmWarehouse` - 仓库
- `CrmStock` - 库存
- `CrmStockRecord` - 库存流水
- `CrmStockCheck` - 库存盘点
- `CrmStockAlert` - 库存预警
- `CrmInvoice` - 发票
- `CrmPayment` - 收付款
- `CrmArap` - 应收应付
- `CrmFixedAsset` - 固定资产
- `CrmReversal` - 冲销记录

### App 模块实体
- `AppMember` - 小程序会员
- `AppCart` - 购物车
- `AppOrder` - 订单
- `AppPayment` - 支付
- `AppAddress` - 收货地址

### Workflow 模块实体
- `WfDefinition` - 流程定义
- `WfInstance` - 流程实例
- `WfNode` - 流程节点
- `WfTask` - 流程任务

### Report 模块实体
- `RptDatasource` - 数据源
- `RptDefinition` - 报表定义
- `RptColumnTemplate` - 列模板

### Ops 模块实体
- `OpsOperateLog` - 操作日志
- `OpsLoginLog` - 登录日志
- `OpsTask` - 定时任务
- `OpsTaskLog` - 任务日志

---

## Mock 状态跟踪

| 模块 | 状态 | 后端阶段 | 备注 |
|------|------|---------|------|
| Basic（认证/菜单/字典骨架） | pending | P1 | 待后端实现 |
| 官网内容（首页聚合骨架） | completed | P2 | 已完成 |
| 商城（会员登录骨架） | pending | P3 | 待后端实现 |
| 会员收货地址（小程序端） | completed | P3-ext | 已完成 |

---

## 开发注意事项

1. **契约优先**: 新增/修改 mock 接口必须与后端规范第 5 节路由和信封逐字一致
2. **禁止自创接口**: Mock 接口必须来源于后端需求，不得随意新增
3. **GUID 主键**: 所有实体主键使用 GUID 字符串
4. **JSON camelCase**: 接口返回数据一律使用 camelCase 命名
5. **分页参数**: pageIndex 从 1 开始，pageSize 默认 10
6. **状态字段**: 使用 int 类型（详见 `docs/api/status-enum-reference.md`）
7. **布尔字段**: 使用 int 类型（0=否，1=是）

---

生成时间: 2026-09-07