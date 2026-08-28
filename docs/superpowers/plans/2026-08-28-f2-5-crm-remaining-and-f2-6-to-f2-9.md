 # F2-5 CRM 剩余子批 + F2-6~F2-9 后续开发计划
 
 > **创建日期：** 2026-08-28
 > **基于：** `2026-08-08-frontend-first-development.md` F2 阶段蓝图
 > **当前 Git 提交数：** 111 commits
 > **当前进度：** F2-1~F2-4 完成；F2-5 CRM 子批 1~3 完成（主数据/销售订单/采购订单）
 
 ---
 
 ## 当前已完成状态
 
 | 任务 | 模块 | 状态 | 提交 |
 |------|------|------|------|
 | F2-0 | 封装层 | ✅ 完成 | useTable/useForm/useDialog/useDict/useSearch + BaseTable/BaseSearchForm/BaseFormDialog/BaseStatusTag |
 | F2-1 | Basic（9 页） | ✅ 完成 | 用户/部门/角色/菜单/字典/公告/工作台/系统参数/个人中心 |
 | F2-2 | Site 管理（8 页） | ✅ 完成 | 新闻/分类/Banner/视频/下载/关于/询价/留言 |
 | F2-3 | Product（3 页） | ✅ 完成 | 商品分类/SPU+SKU/渠道发布 |
 | F2-4 | Mall（6 页） | ✅ 完成 | 会员/等级/积分/优惠券/商城订单/支付记录 |
 | F2-5 子批1 | CRM 主数据 | ✅ 完成 | 客户/供应商/币种/税率（4 页） |
 | F2-5 子批2 | CRM 销售订单 | ✅ 完成 | 列表+详情弹窗+状态机 |
 | F2-5 子批3 | CRM 采购订单 | ✅ 完成 | 列表+详情弹窗+状态机 |
 
 ---
 
 ## 待开发任务总览
 
 | 任务 | 内容 | 页面数 | 预计子批 |
 |------|------|--------|---------|
 | F2-5 子批4 | CRM 库存模块 | 5 页 | 1 批 |
 | F2-5 子批5 | CRM 财务模块 | 4 页 | 1 批 |
 | F2-5 子批6 | CRM 冲销模块 | 2 页 | 1 批 |
 | F2-6 | 工作流模块 | 6 页 + 设计器 | 2~3 批 |
 | F2-7 | 报表模块 | 3 页 | 1 批 |
 | F2-8 | 运维管理模块 | 5 页 | 1 批 |
 | F2-9 | 工作台整合 | 4 个 Widget | 1 批 |
 
 ---
 
 ## F2-5 子批4：库存模块（5 页）
 
 **数据表（整合设计 §6.4）：**
 - `crm_warehouse`：仓库主档（编码/名称/地址/负责人/状态）
 - `crm_stock`：库存账面（仓库+SKU+可用量+锁定量）
 - `crm_stock_record`：出入库流水（来源类型：采购入库/销售出库/商城出库/盘点调整/冲销退回）
 - `crm_stock_check`：盘点单
 - 库存上下限规则 → 预警记录
 
 **页面清单：**
 
 ### 4-1 仓库管理（warehouse）
 - 路径：`src/views/crm/warehouse/index.vue` + `components/WarehouseFormDialog.vue`
 - API：`src/api/crm/warehouse.ts`
 - 类型：`Warehouse` interface + `WAREHOUSE_STATUS_OPTIONS` 常量
 - 模式：标准 CRUD（useTable + BaseSearchForm + BaseTable + FormDialog）
 - Mock：`mock-server/src/data/crm-warehouse.ts` + `routes/admin/crm-warehouse.ts`
 
 ### 4-2 库存查询（stock）
 - 路径：`src/views/crm/stock/index.vue`
 - API：`src/api/crm/stock.ts`
 - 类型：`Stock` interface（warehouseId/warehouseName/skuId/skuName/available/locked/total）
 - 模式：只读列表 + 搜索（仓库下拉+SKU 关键词），不新增不编辑
 - Mock：`mock-server/src/data/crm-stock.ts` + `routes/admin/crm-stock.ts`
 
 ### 4-3 出入库流水（stock-record）
 - 路径：`src/views/crm/stock-record/index.vue`
 - API：`src/api/crm/stock-record.ts`
 - 类型：`StockRecord` interface（warehouseId/skuId/type:in|out/sourceType/sourceOrderId/quantity/operator/createdAt）
 - 模式：只读列表 + 搜索（仓库/来源类型/日期范围），DetailDialog 展示流水详情
 - 来源类型常量：`STOCK_RECORD_SOURCE_OPTIONS`（purchase_in/sales_out/mall_out/check_adjust/reversal_return）
 - Mock：`mock-server/src/data/crm-stock-record.ts` + `routes/admin/crm-stock-record.ts`
 
 ### 4-4 盘点管理（stock-check）
 - 路径：`src/views/crm/stock-check/index.vue` + `components/StockCheckDetailDialog.vue`
 - API：`src/api/crm/stock-check.ts`
 - 类型：`StockCheck` interface（warehouseId/检查人/盘点日期/状态/明细行[skuId/systemQty/countedQty/diff]）
 - 模式：列表 + DetailDialog（盘点明细行表格 + 差异列）；状态机：draft→counting→completed
 - Mock：`mock-server/src/data/crm-stock-check.ts` + `routes/admin/crm-stock-check.ts`
 
 ### 4-5 库存预警（stock-alert）
 - 路径：`src/views/crm/stock-alert/index.vue`
 - API：`src/api/crm/stock-alert.ts`
 - 类型：`StockAlert` interface（warehouseId/skuId/available/minLimit/maxLimit/alertType:low|high/status:pending|resolved）
 - 模式：只读列表 + 搜索（仓库/预警类型/状态）；批量"标记已处理"操作
 - Mock：`mock-server/src/data/crm-stock-alert.ts` + `routes/admin/crm-stock-alert.ts`
 
 **路由追加（`src/router/modules/crm.ts`）：**
 ```typescript
 { path: 'warehouse', name: 'crm-warehouse', ... },
 { path: 'stock', name: 'crm-stock', ... },
 { path: 'stock-record', name: 'crm-stock-record', ... },
 { path: 'stock-check', name: 'crm-stock-check', ... },
 { path: 'stock-alert', name: 'crm-stock-alert', ... },
 ```
 
 **i18n key 前缀：** `crm.warehouse.*` / `crm.stock.*` / `crm.stockRecord.*` / `crm.stockCheck.*` / `crm.stockAlert.*`
 **菜单 key：** `menu.crmWarehouse` / `menu.crmStock` / `menu.crmStockRecord` / `menu.crmStockCheck` / `menu.crmStockAlert`
 
 ---
 
 ## F2-5 子批5：财务模块（4 页）
 
 **数据表（整合设计 §6.5）：**
 - `crm_invoice`：开票登记（销项/进项、金额、状态），不做税务申报
 - `crm_payment`：收付款单，关联销售/采购订单，核销
 - `crm_arap`：应收应付台账（订单应收 − 已收款 = 余额，账龄分段 30/60/90/90+）
 - `crm_fixed_asset`：资产卡片（原值/购入日期/折旧方法(直线法)/状态）；折旧记录表 `crm_asset_depreciation`
 
 **页面清单：**
 
 ### 5-1 发票管理（invoice）
 - 路径：`src/views/crm/invoice/index.vue` + `components/InvoiceFormDialog.vue`
 - API：`src/api/crm/invoice.ts`
 - 类型：`Invoice` interface（invoiceNo/type:output|input/orderType:sales|purchase/orderId/customerId|supplierId/amount/taxAmount/total/issueDate/status）
 - 模式：标准 CRUD + 状态机（draft→issued→voided）
 - Mock：`mock-server/src/data/crm-invoice.ts` + `routes/admin/crm-invoice.ts`
 
 ### 5-2 收付款管理（payment）
 - 路径：`src/views/crm/payment/index.vue` + `components/PaymentDetailDialog.vue`
 - API：`src/api/crm/payment.ts`
 - 类型：`Payment` interface（paymentNo/type:receipt|payment/orderType:sales|purchase/orderId/partyName/amount/method:cash|bank|wechat/remark/status）
 - 模式：列表 + DetailDialog；状态机：draft→confirmed→voided
 - Mock：`mock-server/src/data/crm-payment.ts` + `routes/admin/crm-payment.ts`
 
 ### 5-3 应收应付台账（arap）
 - 路径：`src/views/crm/arap/index.vue`
 - API：`src/api/crm/arap.ts`
 - 类型：`Arap` interface（orderType:sales|purchase/orderNo/partyName/receivable/received/balance/aging:0-30|31-60|61-90|90+/status:settled|unsettled）
 - 模式：只读列表 + 搜索（订单类型/账龄/状态）；汇总卡片显示总应收/总应付/总余额
 - Mock：`mock-server/src/data/crm-arap.ts` + `routes/admin/crm-arap.ts`
 
 ### 5-4 固定资产管理（fixed-asset）
 - 路径：`src/views/crm/fixed-asset/index.vue` + `components/FixedAssetFormDialog.vue` + `components/FixedAssetDetailDialog.vue`
 - API：`src/api/crm/fixed-asset.ts`
 - 类型：`FixedAsset` interface（assetNo/name/category/originalValue/purchaseDate/depreciationMethod:straight-line/salvageValue/usefulYears/currentValue/status:active|scrapped）+ `AssetDepreciation` interface
 - 模式：列表 + FormDialog（新建/编辑）+ DetailDialog（折旧记录表格）
 - Mock：`mock-server/src/data/crm-fixed-asset.ts` + `routes/admin/crm-fixed-asset.ts`
 
 **路由追加：**
 ```typescript
 { path: 'invoice', name: 'crm-invoice', ... },
 { path: 'payment', name: 'crm-payment', ... },
 { path: 'arap', name: 'crm-arap', ... },
 { path: 'fixed-asset', name: 'crm-fixed-asset', ... },
 ```
 
 ---
 
 ## F2-5 子批6：冲销模块（2 页）
 
 **数据表（整合设计 §6.4）：**
 - `crm_reversal` / `_item`：统一红冲（商城退款/销售退货/采购退货/单据作废四类）；`wf_biz_link` 挂审批
 
 **页面清单：**
 
 ### 6-1 冲销列表（reversal）
 - 路径：`src/views/crm/reversal/index.vue`
 - API：`src/api/crm/reversal.ts`
 - 类型：`Reversal` interface（reversalNo/type:mall_refund|sales_return|purchase_return|document_void/sourceOrderType/sourceOrderNo/partyName/amount/reason/operator/status/createdAt）
 - 模式：列表 + 搜索（类型/来源单号/状态/日期范围）；状态机：draft→submitted→approved→rejected→executed
 - Mock：`mock-server/src/data/crm-reversal.ts` + `routes/admin/crm-reversal.ts`
 
 ### 6-2 冲销详情（reversal-detail）
 - 路径：`src/views/crm/reversal/components/ReversalDetailDialog.vue`
 - 模式：DetailDialog（el-descriptions 头部 + el-table 冲销明细行 + el-descriptions 金额汇总）
 - 复用 SalesOrderDetailDialog / PurchaseOrderDetailDialog 的详情弹窗模式
 
 **路由追加：**
 ```typescript
 { path: 'reversal', name: 'crm-reversal', ... },
 ```
 
 ---
 
 ## F2-6：工作流模块（6 页 + 设计器）
 
 **数据表（整合设计 §6.6）：** `wf_*`：definition / node / edge / instance / task / history / biz_link（继承 AntWorkflow）
 
 **页面清单：**
 
 | 页面 | 路径 | 模式 |
 |------|------|------|
 | 流程设计器 | `src/views/workflow/designer/index.vue` | 画布（最重子任务，单独成批）；节点拖拽 + 连线 + 属性面板 |
 | 流程发布 | `src/views/workflow/publish/index.vue` | 列表 + 发布/停用操作 |
 | 我的申请 | `src/views/workflow/my-apply/index.vue` | 列表 + 发起申请入口（请假示范） |
 | 待我审批 | `src/views/workflow/todo/index.vue` | 列表 + 审批操作（同意/拒绝/转交） |
 | 已办 | `src/views/workflow/done/index.vue` | 只读列表 |
 | 实例查询 | `src/views/workflow/instance/index.vue` | 只读列表 + 详情查看（审批历史时间线） |
 
 **建议拆分：**
 - 子批1：审批中心四页（我的申请/待我审批/已办/实例查询）— 标准 CRUD/只读模式
 - 子批2：流程发布（列表 + 发布/停用）
 - 子批3：流程设计器（画布，单独成批，可能引入 jsplumb/dagre 或自研）
 
 **请假示范业务：** 在"我的申请"页面内嵌请假申请表单（类型选择=请假、天数、事由、附件）
 
 ---
 
 ## F2-7：报表模块（3 页）
 
 **数据表：** `rpt_*`：datasource / report / column_template
 
 **页面清单：**
 
 | 页面 | 路径 | 模式 |
 |------|------|------|
 | 数据源管理 | `src/views/report/datasource/index.vue` | CRUD（连接配置/测试连接） |
 | 报表定义 | `src/views/report/definition/index.vue` | CRUD（SQL 模板 + 列配置 + 预览） |
 | 列模板 | `src/views/report/column-template/index.vue` | CRUD（列名/类型/格式化规则） |
 
 **ECharts 依赖：** `vue-echarts` + `echarts/core` 按需引入（折线图/柱状图/饼图）
 
 ---
 
 ## F2-8：运维管理模块（5 页）
 
 **数据表：** `ops_*`：operate_log / login_log / task / task_log
 
 **页面清单：**
 
 | 页面 | 路径 | 模式 |
 |------|------|------|
 | 操作日志 | `src/views/ops/operate-log/index.vue` | 只读列表（用户/模块/操作/IP/时间） |
 | 登录日志 | `src/views/ops/login-log/index.vue` | 只读列表（用户/IP/浏览器/状态/时间） |
 | 定时任务 | `src/views/ops/task/index.vue` | CRUD（cron 表达式 + 执行类 + 状态） |
 | 任务日志 | `src/views/ops/task-log/index.vue` | 只读列表（任务名/执行时间/耗时/状态/错误信息） |
 | 日志查询 | `src/views/ops/log-query/index.vue` | 只读列表（统一日志查询入口，按模块/级别/关键词/时间） |
 
 ---
 
 ## F2-9：工作台整合（4 个 Widget）
 
 **目标：** 将 EasyCRM dashboard 4 页并入 desktop 工作台
 
 | Widget | 说明 |
 |--------|------|
 | 经营概览 | 销售额/采购额/利润概览卡片 + 趋势折线图 |
 | KPI | 关键指标环形图/柱状图（客户增长/订单转化率/库存周转） |
 | 待办 | 待审批/待处理/低库存预警数量卡片 + 跳转链接 |
 | 预警 | 库存预警/应收逾期/异常订单滚动列表 |
 
 **实现：** 复用 PCWeb Widget 容器概念，4 个 Widget 组件注册到工作台布局
 
 ---
 
 ## 开发模式总结（每个子批的标准流程）
 
 每个子批遵循以下步骤：
 
 1. **类型定义** — 在 `src/types/crm.ts` 追加 interface + 状态常量
 2. **API 层** — 在 `src/api/crm/` 新建 `xxx.ts`（get/post/put/del）
 3. **Mock 数据** — `mock-server/src/data/crm-xxx.ts`（引用已有 CUSTOMERS/SUPPLIERS 等种子）
 4. **Mock 路由** — `mock-server/src/routes/admin/crm-xxx.ts`（ok/fail/paginate helpers）+ `server.ts` 挂载
 5. **页面组件** — `src/views/crm/xxx/index.vue`（useTable + BaseSearchForm + BaseTable）+ `components/XxxFormDialog.vue` 或 `XxxDetailDialog.vue`
 6. **路由** — `src/router/modules/crm.ts` 追加 children
 7. **i18n** — `src/i18n/zh-CN/crm.json` + `en-US/crm.json` 追加 key；`menu.json` 追加菜单 key
 8. **验证** — `vue-tsc --noEmit` + `eslint` + `check-chinese`
 9. **提交** — `feat(mock): xxx` + `feat(admin): xxx`（mock 先行，admin 跟进）
 
 ---
 
 ## 开发优先级建议
 
 ```
 F2-5 子批4（库存）→ 子批5（财务）→ 子批6（冲销）
   → F2-8（运维，简单只读为主，可提前穿插）
   → F2-7（报表）
   → F2-6（工作流，设计器最重放最后）
   → F2-9（工作台整合，收尾）
 ```
 
 **理由：** 库存/财务/冲销是 CRM 闭环的最后三块，逻辑紧密相连（冲销依赖订单和库存），优先完成；运维模块以只读列表为主，开发速度快，可在 CRM 各子批间穿插；工作流设计器复杂度最高，放最后集中精力；工作台整合依赖各模块完成，作为收尾。
 
 ---
 
 ## 参考文件索引
 
 | 用途 | 文件路径 |
 |------|---------|
 | 整体设计 | `docs/superpowers/specs/2026-08-08-easyproduct-integration-design.md`（§6.3-6.6） |
 | F2 主计划 | `docs/superpowers/plans/2026-08-08-frontend-first-development.md`（F2 阶段蓝图，行 3362+） |
 | 前端规范 | `docs/frontend-guidelines.md`（2.5 查询组件封装规范） |
 | 后端规范 | `docs/backend-guidelines.md` |
 | Mock 规范 | `docs/mock-guidelines.md` |
 | 进度报告 | `docs/progress-report-2026-08-27.md`（需随开发更新） |
 | CRM 类型定义 | `src/types/crm.ts`（所有 CRM interface + 常量） |
 | CRM 路由 | `src/router/modules/crm.ts` |
 | CRUD 参考 | `src/views/crm/customer/index.vue` + `CustomerFormDialog.vue` |
 | 订单详情参考 | `src/views/crm/sales-order/components/SalesOrderDetailDialog.vue` |
 | 状态机参考 | `src/views/crm/purchase-order/index.vue`（状态按钮 + BaseStatusTag） |
 | 通用组件 | `src/components/common/BaseTable.vue` / `BaseSearchForm.vue` / `BaseFormDialog.vue` / `BaseStatusTag.vue` |
 | Composables | `src/composables/useTable.ts` / `useSearch.ts` / `useCrud.ts` |
 | Mock helpers | `mock-server/src/helpers/envelope.ts`（ok/fail/paginate）/ `id.ts`（guid/isoTime） |
 | API 模式参考 | `src/api/mall/level.ts`（get/post/put/del 模式） |
