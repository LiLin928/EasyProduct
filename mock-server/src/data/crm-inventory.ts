 // src/data/crm-inventory.ts
 // CRM 库存 seed：仓库/库存/出入库流水/盘点/预警

 import { guid, isoTime } from '../helpers/id.js'

 // ────────────── 仓库 ──────────────
 export interface Warehouse {
   id: string
   code: string
   name: string
   address: string
   manager: string
   phone: string
   status: 'active' | 'inactive'
   remark: string
   createdAt: string
   updatedAt: string
 }

 export const WAREHOUSES: Warehouse[] = [
   {
     id: guid(),
     code: 'WH001',
     name: '深圳主仓',
     address: '深圳市南山区科技园北区',
     manager: '李建国',
     phone: '13800138001',
     status: 'active',
     remark: '主营仓，存放全部品类',
     createdAt: isoTime(-30),
     updatedAt: isoTime(-5),
   },
   {
     id: guid(),
     code: 'WH002',
     name: '上海分仓',
     address: '上海市浦东新区张江高科技园区',
     manager: '王芳',
     phone: '13900139002',
     status: 'active',
     remark: '华东区域分仓',
     createdAt: isoTime(-25),
     updatedAt: isoTime(-10),
   },
   {
     id: guid(),
     code: 'WH003',
     name: '广州临时仓',
     address: '广州市黄埔区开发大道',
     manager: '陈志',
     phone: '13700137003',
     status: 'inactive',
     remark: '已停用，仅保留历史数据',
     createdAt: isoTime(-20),
     updatedAt: isoTime(-3),
   },
 ]

 // ────────────── 库存账面 ──────────────
 export interface Stock {
   id: string
   warehouseId: string
   warehouseName: string
   skuCode: string
   skuName: string
   spec: string
   unit: string
   available: number
   locked: number
   total: number
   minLimit: number
   maxLimit: number
   updatedAt: string
 }

 // 预生成仓库 ID 引用
 const wh1 = WAREHOUSES[0].id
 const wh2 = WAREHOUSES[1].id

 export const STOCKS: Stock[] = [
   {
     id: guid(),
     warehouseId: wh1,
     warehouseName: '深圳主仓',
     skuCode: 'SKU-001',
     skuName: '蓝牙耳机 Pro',
     spec: '黑色/主动降噪',
     unit: '个',
     available: 320,
     locked: 50,
     total: 370,
     minLimit: 100,
     maxLimit: 1000,
     updatedAt: isoTime(-2),
   },
   {
     id: guid(),
     warehouseId: wh1,
     warehouseName: '深圳主仓',
     skuCode: 'SKU-002',
     skuName: 'USB-C 充电器 65W',
     spec: '氮化镓/双口',
     unit: '个',
     available: 80,
     locked: 20,
     total: 100,
     minLimit: 100,
     maxLimit: 500,
     updatedAt: isoTime(-1),
   },
   {
     id: guid(),
     warehouseId: wh1,
     warehouseName: '深圳主仓',
     skuCode: 'SKU-003',
     skuName: '无线键盘',
     spec: '87键/蓝牙',
     unit: '把',
     available: 540,
     locked: 30,
     total: 570,
     minLimit: 50,
     maxLimit: 800,
     updatedAt: isoTime(-3),
   },
   {
     id: guid(),
     warehouseId: wh2,
     warehouseName: '上海分仓',
     skuCode: 'SKU-001',
     skuName: '蓝牙耳机 Pro',
     spec: '黑色/主动降噪',
     unit: '个',
     available: 150,
     locked: 0,
     total: 150,
     minLimit: 100,
     maxLimit: 500,
     updatedAt: isoTime(-4),
   },
   {
     id: guid(),
     warehouseId: wh2,
     warehouseName: '上海分仓',
     skuCode: 'SKU-004',
     skuName: '便携移动电源 20000mAh',
     spec: 'PD 30W/黑色',
     unit: '个',
     available: 40,
     locked: 10,
     total: 50,
     minLimit: 50,
     maxLimit: 300,
     updatedAt: isoTime(-1),
   },
 ]

 // ────────────── 出入库流水 ──────────────
 export interface StockRecord {
   id: string
   warehouseId: string
   warehouseName: string
   skuCode: string
   skuName: string
   spec: string
   unit: string
   type: 'in' | 'out'
   sourceType: 'purchase_in' | 'sales_out' | 'mall_out' | 'check_adjust' | 'reversal_return'
   sourceOrderNo: string
   quantity: number
   operator: string
   remark: string
   createdAt: string
 }

 export const STOCK_RECORDS: StockRecord[] = [
   {
     id: guid(),
     warehouseId: wh1,
     warehouseName: '深圳主仓',
     skuCode: 'SKU-001',
     skuName: '蓝牙耳机 Pro',
     spec: '黑色/主动降噪',
     unit: '个',
     type: 'in',
     sourceType: 'purchase_in',
     sourceOrderNo: 'PO-20260825-001',
     quantity: 200,
     operator: '李建国',
     remark: '采购入库',
     createdAt: isoTime(-10),
   },
   {
     id: guid(),
     warehouseId: wh1,
     warehouseName: '深圳主仓',
     skuCode: 'SKU-001',
     skuName: '蓝牙耳机 Pro',
     spec: '黑色/主动降噪',
     unit: '个',
     type: 'out',
     sourceType: 'sales_out',
     sourceOrderNo: 'SO-20260826-003',
     quantity: 50,
     operator: '张伟',
     remark: '销售出库',
     createdAt: isoTime(-7),
   },
   {
     id: guid(),
     warehouseId: wh1,
     warehouseName: '深圳主仓',
     skuCode: 'SKU-002',
     skuName: 'USB-C 充电器 65W',
     spec: '氮化镓/双口',
     unit: '个',
     type: 'in',
     sourceType: 'purchase_in',
     sourceOrderNo: 'PO-20260824-002',
     quantity: 100,
     operator: '李建国',
     remark: '采购入库',
     createdAt: isoTime(-9),
   },
   {
     id: guid(),
     warehouseId: wh1,
     warehouseName: '深圳主仓',
     skuCode: 'SKU-002',
     skuName: 'USB-C 充电器 65W',
     spec: '氮化镓/双口',
     unit: '个',
     type: 'out',
     sourceType: 'mall_out',
     sourceOrderNo: 'MO-20260827-018',
     quantity: 20,
     operator: '系统自动',
     remark: '商城订单出库',
     createdAt: isoTime(-2),
   },
   {
     id: guid(),
     warehouseId: wh2,
     warehouseName: '上海分仓',
     skuCode: 'SKU-001',
     skuName: '蓝牙耳机 Pro',
     spec: '黑色/主动降噪',
     unit: '个',
     type: 'in',
     sourceType: 'check_adjust',
     sourceOrderNo: 'SC-20260820-001',
     quantity: 10,
     operator: '王芳',
     remark: '盘盈调整',
     createdAt: isoTime(-5),
   },
   {
     id: guid(),
     warehouseId: wh2,
     warehouseName: '上海分仓',
     skuCode: 'SKU-004',
     skuName: '便携移动电源 20000mAh',
     spec: 'PD 30W/黑色',
     unit: '个',
     type: 'out',
     sourceType: 'reversal_return',
     sourceOrderNo: 'RV-20260826-002',
     quantity: 5,
     operator: '陈志',
     remark: '退货冲销退回',
     createdAt: isoTime(-1),
   },
 ]

 // ────────────── 盘点 ──────────────
 export interface StockCheckItem {
   id: string
   checkId: string
   skuCode: string
   skuName: string
   spec: string
   unit: string
   systemQty: number
   countedQty: number
   diff: number
 }

 export interface StockCheck {
   id: string
   checkNo: string
   warehouseId: string
   warehouseName: string
   checker: string
   checkDate: string
   status: 'draft' | 'counting' | 'completed'
   remark: string
   items: StockCheckItem[]
   createdAt: string
   updatedAt: string
 }

 export const STOCK_CHECKS: StockCheck[] = [
   {
     id: guid(),
     checkNo: 'SC-20260820-001',
     warehouseId: wh2,
     warehouseName: '上海分仓',
     checker: '王芳',
     checkDate: isoTime(-8).slice(0, 10),
     status: 'completed',
     remark: '月度盘点',
     items: [
       {
         id: guid(),
         checkId: '',
         skuCode: 'SKU-001',
         skuName: '蓝牙耳机 Pro',
         spec: '黑色/主动降噪',
         unit: '个',
         systemQty: 140,
         countedQty: 150,
         diff: 10,
       },
       {
         id: guid(),
         checkId: '',
         skuCode: 'SKU-004',
         skuName: '便携移动电源 20000mAh',
         spec: 'PD 30W/黑色',
         unit: '个',
         systemQty: 45,
         countedQty: 45,
         diff: 0,
       },
     ],
     createdAt: isoTime(-8),
     updatedAt: isoTime(-5),
   },
   {
     id: guid(),
     checkNo: 'SC-20260828-002',
     warehouseId: wh1,
     warehouseName: '深圳主仓',
     checker: '李建国',
     checkDate: isoTime(0).slice(0, 10),
     status: 'counting',
     remark: '季度大盘点',
     items: [
       {
         id: guid(),
         checkId: '',
         skuCode: 'SKU-001',
         skuName: '蓝牙耳机 Pro',
         spec: '黑色/主动降噪',
         unit: '个',
         systemQty: 370,
         countedQty: 0,
         diff: 0,
       },
       {
         id: guid(),
         checkId: '',
         skuCode: 'SKU-002',
         skuName: 'USB-C 充电器 65W',
         spec: '氮化镓/双口',
         unit: '个',
         systemQty: 100,
         countedQty: 0,
         diff: 0,
       },
       {
         id: guid(),
         checkId: '',
         skuCode: 'SKU-003',
         skuName: '无线键盘',
         spec: '87键/蓝牙',
         unit: '把',
         systemQty: 570,
         countedQty: 0,
         diff: 0,
       },
     ],
     createdAt: isoTime(0),
     updatedAt: isoTime(0),
   },
 ]

 // 回填 checkId
 STOCK_CHECKS.forEach(sc => {
   sc.items.forEach(item => { item.checkId = sc.id })
 })

 // ────────────── 库存预警 ──────────────
 export interface StockAlert {
   id: string
   warehouseId: string
   warehouseName: string
   skuCode: string
   skuName: string
   spec: string
   available: number
   minLimit: number
   maxLimit: number
   alertType: 'low' | 'high'
   status: 'pending' | 'resolved'
   createdAt: string
   resolvedAt: string | null
   remark: string
 }

 export const STOCK_ALERTS: StockAlert[] = [
   {
     id: guid(),
     warehouseId: wh1,
     warehouseName: '深圳主仓',
     skuCode: 'SKU-002',
     skuName: 'USB-C 充电器 65W',
     spec: '氮化镓/双口',
     available: 80,
     minLimit: 100,
     maxLimit: 500,
     alertType: 'low',
     status: 'pending',
     createdAt: isoTime(-1),
     resolvedAt: null,
     remark: '库存低于下限，请及时补货',
   },
   {
     id: guid(),
     warehouseId: wh2,
     warehouseName: '上海分仓',
     skuCode: 'SKU-004',
     skuName: '便携移动电源 20000mAh',
     spec: 'PD 30W/黑色',
     available: 40,
     minLimit: 50,
     maxLimit: 300,
     alertType: 'low',
     status: 'pending',
     createdAt: isoTime(-1),
     resolvedAt: null,
     remark: '库存低于下限，请及时补货',
   },
   {
     id: guid(),
     warehouseId: wh1,
     warehouseName: '深圳主仓',
     skuCode: 'SKU-003',
     skuName: '无线键盘',
     spec: '87键/蓝牙',
     available: 570,
     minLimit: 50,
     maxLimit: 800,
     alertType: 'high',
     status: 'resolved',
     createdAt: isoTime(-5),
     resolvedAt: isoTime(-3),
     remark: '库存接近上限，建议促销消化',
   },
 ]

 // ────────────── 仓库选项（下拉） ──────────────
 export function getWarehouseOptions() {
   return WAREHOUSES
     .filter(w => w.status === 'active')
     .map(w => ({ id: w.id, name: w.name, code: w.code }))
 }
