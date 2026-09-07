# Site、Product、App、Workflow、Report、Ops 模块接口文档

> 其他业务模块的接口说明

---

## 一、Site 模块 - 官网门户

官网公开接口，无需认证，支持匿名访问。

### 实体对象

#### SiteNews - 新闻资讯

```typescript
interface SiteNews {
  id: string
  categoryId: string
  title: string
  titleEn: string
  summary: string
  content: string
  coverImage: string
  isTop: 0 | 1  // 是否置顶：0=否，1=是
  viewCount: number
  publishTime: string
  status: 'draft' | 'published' | 'archived'
  createdAt: string
  updatedAt: string
}
```

#### SiteCategory - 分类

```typescript
interface SiteCategory {
  id: string
  name: string
  nameEn: string
  parentId: string
  type: 'news' | 'product' | 'video' | 'download'
  sort: number
  status: 0 | 1  // 状态：0=禁用，1=启用
}
```

#### SiteBanner - Banner

```typescript
interface SiteBanner {
  id: string
  title: string
  imageUrl: string
  linkUrl: string
  target: '_blank' | '_self'
  sort: number
  status: 0 | 1  // 状态：0=禁用，1=启用
  startTime: string
  endTime: string
}
```

#### SiteVideo - 视频

```typescript
interface SiteVideo {
  id: string
  title: string
  titleEn: string
  description: string
  coverImage: string
  videoUrl: string
  duration: number
  viewCount: number
  sort: number
  status: 0 | 1  // 状态：0=禁用，1=启用
}
```

#### SiteDownload - 下载资源

```typescript
interface SiteDownload {
  id: string
  title: string
  titleEn: string
  description: string
  fileUrl: string
  fileSize: number
  fileType: string
  downloadCount: number
  sort: number
  status: 0 | 1  // 状态：0=禁用，1=启用
}
```

#### SiteContact - 联系方式

```typescript
interface SiteContact {
  id: string
  companyName: string
  address: string
  phone: string
  email: string
  fax: string
  wechat: string
  weibo: string
  mapUrl: string
}
```

### 主要接口

**官网前端（/api/site）：**
- `GET /api/site/home/banners` - 首页Banner列表
- `GET /api/site/home/news` - 首页新闻列表
- `GET /api/site/news/list` - 新闻列表（分页）
- `GET /api/site/news/:id` - 新闻详情
- `GET /api/site/category/list` - 分类列表
- `GET /api/site/product/list` - 产品列表
- `GET /api/site/video/list` - 视频列表
- `GET /api/site/download/list` - 下载列表
- `GET /api/site/about` - 关于我们
- `GET /api/site/contact` - 联系方式
- `POST /api/site/inquiry` - 在线咨询提交

**管理后台（/api/admin）：**
- `GET /api/admin/site-news/list` - 新闻管理列表
- `POST /api/admin/site-news` - 创建新闻
- `PUT /api/admin/site-news/:id` - 更新新闻
- `DELETE /api/admin/site-news/:id` - 删除新闻
- 类似接口用于：category, banner, video, download, about, contact, inquiry

---

## 二、Product 模块 - 商品管理

商品核心数据模块，管理商品分类、SPU、SKU、渠道。

### 实体对象

#### ProductCategory - 商品分类

```typescript
interface ProductCategory {
  id: string
  name: string
  nameEn: string
  parentId: string
  code: string
  sort: number
  status: 0 | 1  // 状态：0=禁用，1=启用
  children?: ProductCategory[]
}
```

#### ProductSpu - 商品SPU

```typescript
interface ProductSpu {
  id: string
  code: string
  name: string
  nameEn: string
  categoryId: string
  type: 'ticket' | 'normal' | 'service' // 类型：票务/普通商品/服务
  mainImage: string
  images: string[]
  description: string
  unit: string
  brand: string
  specs: SpecGroup[]         // 规格组
  status: 'active' | 'inactive'
  skus: ProductSku[]        // SKU列表
  createdAt: string
  updatedAt: string
}

interface SpecGroup {
  name: string              // 规格名称（如：颜色）
  values: string[]          // 规格值（如：['红色', '蓝色']）
}

interface ProductSku {
  id: string
  spuId: string
  specValues: Record<string, string> // 规格值对象
  barcode: string
  retailPrice: number       // 零售价
  memberPrice: number       // 会员价
  b2bPrice: number         // B2B价
  costPrice: number        // 成本价
  stock: number            // 库存
  status: 'active' | 'inactive'
}
```

#### ProductChannel - 商品渠道

```typescript
interface ProductChannel {
  id: string
  name: string
  code: string
  type: 'online' | 'offline' | 'third-party'
  description: string
  status: 0 | 1  // 状态：0=禁用，1=启用
}
```

### 主要接口

**分类管理：**
- `GET /api/admin/product/category/tree` - 分类树
- `POST /api/admin/product/category` - 创建分类
- `PUT /api/admin/product/category/:id` - 更新分类
- `DELETE /api/admin/product/category/:id` - 删除分类

**SPU管理：**
- `GET /api/admin/product/spu/list` - SPU列表（分页）
- `GET /api/admin/product/spu/:id` - SPU详情（含SKU）
- `POST /api/admin/product/spu` - 创建SPU（自动生成SKU）
- `PUT /api/admin/product/spu/:id` - 更新SPU
- `DELETE /api/admin/product/spu/:id` - 删除SPU
- `PUT /api/admin/product/sku/:id` - 更新单个SKU
- `PUT /api/admin/product/sku/batch` - 批量更新SKU

**渠道管理：**
- `GET /api/admin/product/channel/list` - 渠道列表
- `POST /api/admin/product/channel` - 创建渠道

---

## 三、App 模块 - 小程序会员端

小程序会员端接口，需要 Member JWT 认证。

### 实体对象

已在 Mall 模块中定义（Member, Order, Cart, Address 等）

### 主要接口

**认证相关：**
- `POST /api/app/auth/login` - 微信登录（code → openid → member）
- `POST /api/app/auth/phone` - 绑定手机号

**会员中心：**
- `GET /api/app/member/info` - 当前会员信息
- `GET /api/app/member/profile` - 会员资料
- `GET /api/app/member/points` - 积分记录
- `PUT /api/app/member/profile` - 更新资料

**商品浏览：**
- `GET /api/app/product/list` - 商品列表
- `GET /api/app/product/:id` - 商品详情

**购物车：**
- `GET /api/app/cart/list` - 购物车列表
- `POST /api/app/cart` - 加入购物车
- `PUT /api/app/cart/:id` - 更新数量
- `DELETE /api/app/cart/:id` - 删除商品

**订单：**
- `GET /api/app/orders/list` - 我的订单
- `GET /api/app/orders/:id` - 订单详情
- `POST /api/app/orders` - 创建订单
- `POST /api/app/orders/:id/cancel` - 取消订单

**支付：**
- `POST /api/app/payment/create` - 创建支付（返回支付参数）
- `POST /api/app/payment/callback` - 支付回调

**收货地址：**
- `GET /api/app/addresses/list` - 地址列表
- `POST /api/app/addresses` - 创建地址
- `PUT /api/app/addresses/:id` - 更新地址
- `DELETE /api/app/addresses/:id` - 删除地址

---

## 四、Workflow 模块 - 工作流

工作流引擎，支持流程定义、审批、流程实例管理。

### 实体对象

#### WfDefinition - 流程定义

```typescript
interface WfDefinition {
  id: string
  code: string
  name: string
  version: number
  category: string
  description: string
  nodes: WfNode[]          // 流程节点
  status: 'draft' | 'published' | 'archived'
  createdAt: string
  updatedAt: string
}

interface WfNode {
  id: string
  name: string
  type: 'start' | 'end' | 'task' | 'gateway' | 'subprocess'
  assigneeType: 'user' | 'role' | 'dept' | 'expression'
  assigneeId: string
  approvalType: 'or' | 'and' | 'sequential' // 或签/会签/顺序签
  timeout: number          // 超时时间（小时）
}
```

#### WfInstance - 流程实例

```typescript
interface WfInstance {
  id: string
  definitionId: string
  definitionName: string
  businessKey: string      // 业务单据ID
  businessType: string     // 业务类型
  title: string
  applicantId: string      // 申请人ID
  applicantName: string
  currentNodeId: string
  currentNodeName: string
  status: 'running' | 'completed' | 'cancelled' | 'rejected'
  startTime: string
  endTime: string
}
```

#### WfTask - 流程任务

```typescript
interface WfTask {
  id: string
  instanceId: string
  nodeId: string
  nodeName: string
  assigneeId: string
  assigneeName: string
  status: 'pending' | 'approved' | 'rejected' | 'delegated'
  comment: string          // 审批意见
  createdAt: string
  completedAt: string
}
```

### 主要接口

**我的申请：**
- `GET /api/admin/workflow/my-apply/list` - 我的申请列表
- `POST /api/admin/workflow/my-apply/start` - 发起流程

**待办任务：**
- `GET /api/admin/workflow/todo/list` - 待办列表
- `GET /api/admin/workflow/todo/:id` - 待办详情
- `POST /api/admin/workflow/todo/:id/approve` - 审批通过
- `POST /api/admin/workflow/todo/:id/reject` - 审批拒绝
- `POST /api/admin/workflow/todo/:id/delegate` - 委托

**已办任务：**
- `GET /api/admin/workflow/done/list` - 已办列表

**流程实例：**
- `GET /api/admin/workflow/instance/list` - 流程实例列表
- `GET /api/admin/workflow/instance/:id` - 实例详情
- `POST /api/admin/workflow/instance/:id/cancel` - 撤销流程

**流程定义：**
- `GET /api/admin/workflow/definition/list` - 流程定义列表
- `POST /api/admin/workflow/definition` - 创建定义
- `PUT /api/admin/workflow/definition/:id` - 更新定义
- `POST /api/admin/workflow/definition/:id/publish` - 发布流程

**流程设计器：**
- `GET /api/admin/workflow/designer/:id` - 获取流程设计数据
- `POST /api/admin/workflow/designer/save` - 保存流程设计

---

## 五、Report 模块 - 报表

报表引擎，支持自定义报表、数据源配置。

### 实体对象

#### RptDatasource - 数据源

```typescript
interface RptDatasource {
  id: string
  name: string
  type: 'mysql' | 'postgresql' | 'sqlserver' | 'oracle'
  host: string
  port: number
  database: string
  username: string
  password: string
  status: 'connected' | 'error'
  createdAt: string
}
```

#### RptDefinition - 报表定义

```typescript
interface RptDefinition {
  id: string
  name: string
  category: string
  datasourceId: string
  sql: string              // 查询SQL
  columns: RptColumn[]     // 列定义
  filters: RptFilter[]     // 筛选条件
  status: 'draft' | 'published'
  createdAt: string
}

interface RptColumn {
  name: string
  field: string
  type: 'text' | 'number' | 'date' | 'datetime'
  width: number
  align: 'left' | 'center' | 'right'
  sortable: boolean
}

interface RptFilter {
  field: string
  label: string
  type: 'text' | 'select' | 'date' | 'daterange'
  defaultValue: string
  required: boolean
}
```

#### RptColumnTemplate - 列模板

```typescript
interface RptColumnTemplate {
  id: string
  name: string
  category: string
  columns: RptColumn[]
}
```

### 主要接口

**数据源管理：**
- `GET /api/admin/report/datasource/list` - 数据源列表
- `POST /api/admin/report/datasource` - 创建数据源
- `POST /api/admin/report/datasource/:id/test` - 测试连接

**报表定义：**
- `GET /api/admin/report/definition/list` - 报表列表
- `GET /api/admin/report/definition/:id` - 报表详情
- `POST /api/admin/report/definition` - 创建报表
- `PUT /api/admin/report/definition/:id` - 更新报表
- `POST /api/admin/report/definition/:id/execute` - 执行报表查询
- `POST /api/admin/report/definition/:id/export` - 导出报表

**列模板：**
- `GET /api/admin/report/column-template/list` - 模板列表
- `POST /api/admin/report/column-template` - 创建模板

---

## 六、Ops 模块 - 运维管理

运维管理模块，包含日志、定时任务等。

### 实体对象

#### OpsOperateLog - 操作日志

```typescript
interface OpsOperateLog {
  id: string
  module: string           // 模块名称
  action: string           // 操作类型
  businessId: string       // 业务ID
  businessName: string     // 业务名称
  operatorId: string       // 操作人ID
  operatorName: string     // 操作人姓名
  requestMethod: string    // 请求方法
  requestUrl: string       // 请求URL
  requestParams: string    // 请求参数
  responseResult: string   // 响应结果
  ip: string               // IP地址
  userAgent: string        // 浏览器标识
  status: 'success' | 'fail'
  errorMsg: string         // 错误信息
  duration: number         // 执行时长（ms）
  createdAt: string
}
```

#### OpsLoginLog - 登录日志

```typescript
interface OpsLoginLog {
  id: string
  userName: string         // 用户名
  loginType: 'admin' | 'app' // 登录类型
  loginMethod: 'password' | 'wechat' | 'sms'
  ip: string
  location: string         // 地理位置
  browser: string
  os: string
  status: 'success' | 'fail'
  message: string
  createdAt: string
}
```

#### OpsTask - 定时任务

```typescript
interface OpsTask {
  id: string
  name: string
  group: string            // 任务分组
  cron: string             // Cron表达式
  className: string        // 执行类
  methodName: string       // 执行方法
  params: string           // 参数（JSON）
  status: 'running' | 'paused' | 'error'
  lastExecuteTime: string
  nextExecuteTime: string
  remark: string
  createdAt: string
}
```

#### OpsTaskLog - 任务日志

```typescript
interface OpsTaskLog {
  id: string
  taskId: string
  taskName: string
  executeTime: string
  duration: number
  status: 'success' | 'fail'
  result: string
  errorMsg: string
  createdAt: string
}
```

### 主要接口

**操作日志：**
- `GET /api/admin/ops/operate-log/list` - 操作日志列表
- `GET /api/admin/ops/operate-log/:id` - 日志详情
- `POST /api/admin/ops/operate-log/export` - 导出日志

**登录日志：**
- `GET /api/admin/ops/login-log/list` - 登录日志列表
- `POST /api/admin/ops/login-log/export` - 导出日志

**定时任务：**
- `GET /api/admin/ops/task/list` - 任务列表
- `POST /api/admin/ops/task` - 创建任务
- `PUT /api/admin/ops/task/:id` - 更新任务
- `DELETE /api/admin/ops/task/:id` - 删除任务
- `POST /api/admin/ops/task/:id/start` - 启动任务
- `POST /api/admin/ops/task/:id/pause` - 暂停任务
- `POST /api/admin/ops/task/:id/execute` - 立即执行

**任务日志：**
- `GET /api/admin/ops/task-log/list` - 任务日志列表

**日志查询：**
- `POST /api/admin/ops/log-query` - 综合日志查询

---

## 开发注意事项

1. **Site 模块**: 官网接口需考虑 SEO、CDN 缓存、限流
2. **Product 模块**: SKU 生成规则、库存扣减逻辑、价格策略
3. **App 模块**: 微信登录流程、支付对接、订单状态机
4. **Workflow 模块**: 流程定义标准、审批策略、任务分配算法
5. **Report 模块**: SQL 注入防护、数据权限控制、大数据查询优化
6. **Ops 模块**: 日志存储策略、任务调度器（Quartz）、日志分析

---

生成时间: 2026-09-07