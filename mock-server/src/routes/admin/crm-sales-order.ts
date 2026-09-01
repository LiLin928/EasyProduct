// src/routes/admin/crm-sales-order.ts
import { Router } from 'express'
import { ok, fail, paginate } from '../../helpers/envelope.js'
import { guid, isoTime } from '../../helpers/id.js'
import { CUSTOMERS } from '../../data/crm.js'
import { SALES_ORDERS, type SalesOrder, type SalesOrderItem } from '../../data/crm-sales.js'
import { STOCKS, STOCK_RECORDS, type StockRecord, WAREHOUSES } from '../../data/crm-inventory.js'

export const adminCrmSalesOrderRouter = Router()

/**
 * 检查库存是否充足
 * @returns {boolean} 是否充足
 */
function checkStockAvailability(order: SalesOrder): { available: boolean; insufficientItems: string[] } {
  const insufficientItems: string[] = []

  for (const item of order.items) {
    const stock = STOCKS.find(s => s.warehouseId === item.warehouseId && s.skuCode === item.skuCode)
    if (!stock || stock.available < item.quantity) {
      const availableQty = stock?.available ?? 0
      insufficientItems.push(`${item.skuName}(需${item.quantity}, 可用${availableQty})`)
    }
  }

  return {
    available: insufficientItems.length === 0,
    insufficientItems,
  }
}

/**
 * 处理销售出库
 * 当销售订单状态变更为 shipped 时，自动创建出库记录并扣减库存
 */
function processSalesOutbound(order: SalesOrder): void {
  const operator = 'System'
  const remark = `销售出库 - 订单: ${order.orderNo}`

  for (const item of order.items) {
    // 1. 创建出库记录
    const warehouse = WAREHOUSES.find(w => w.id === item.warehouseId) ?? WAREHOUSES[0]
    const record: StockRecord = {
      id: guid(),
      warehouseId: item.warehouseId,
      warehouseName: warehouse?.name ?? '深圳主仓',
      skuCode: item.skuCode,
      skuName: item.skuName,
      spec: item.spec,
      unit: '个',
      type: 'out',
      sourceType: 'sales_out',
      sourceOrderNo: order.orderNo,
      quantity: item.quantity,
      operator,
      remark,
      createdAt: isoTime(),
    }
    STOCK_RECORDS.unshift(record)

    // 2. 扣减库存
    const stock = STOCKS.find(s => s.warehouseId === item.warehouseId && s.skuCode === item.skuCode)
    if (stock) {
      stock.available = Math.max(0, stock.available - item.quantity)
      stock.total = Math.max(0, stock.total - item.quantity)
      stock.updatedAt = isoTime()
    }
  }
}

// ── 客户下拉 ──
adminCrmSalesOrderRouter.get('/crm/sales-order/customer/options', (_req, res) => {
  const options = CUSTOMERS
    .filter(c => c.status === 'active')
    .map(c => ({ id: c.id, name: c.name }))
  res.json(ok(options))
})

// ── 列表 ──
adminCrmSalesOrderRouter.get('/crm/sales-order/list', (req, res) => {
  const pageIndex = Number(req.query.pageIndex ?? 1)
  const pageSize = Number(req.query.pageSize ?? 10)
  const orderNo = req.query.orderNo as string | undefined
  const customerId = req.query.customerId as string | undefined
  const status = req.query.status as string | undefined

  let filtered = [...SALES_ORDERS]
  if (orderNo) filtered = filtered.filter(o => o.orderNo.toLowerCase().includes(orderNo.toLowerCase()))
  if (customerId) filtered = filtered.filter(o => o.customerId === customerId)
  if (status) filtered = filtered.filter(o => o.status === status)
  filtered.sort((a, b) => b.createdAt.localeCompare(a.createdAt))

  res.json(ok(paginate(filtered, pageIndex, pageSize)))
})

// ── 详情 ──
adminCrmSalesOrderRouter.get('/crm/sales-order/:id', (req, res) => {
  const order = SALES_ORDERS.find(o => o.id === req.params.id)
  if (!order) { res.json(fail('sales order not found', 404)); return }
  res.json(ok(order))
})

// ── 新建 ──
adminCrmSalesOrderRouter.post('/crm/sales-order', (req, res) => {
  const body = req.body
  const cust = CUSTOMERS.find(c => c.id === body.customerId)
  if (!cust) { res.json(fail('customer not found', 404)); return }

  const defaultWarehouseId = WAREHOUSES[0]?.id ?? ''
  const id = guid()
  const items: SalesOrderItem[] = (body.items ?? []).map((it: SalesOrderItem) => {
    const amount = +(it.price * it.quantity).toFixed(2)
    const taxAmount = +(amount * it.taxRate / 100).toFixed(2)
    return {
      id: guid(),
      orderId: id,
      skuCode: it.skuCode ?? '',
      skuName: it.skuName ?? it.productName ?? '',
      productName: it.productName,
      spec: it.spec || '',
      price: it.price,
      quantity: it.quantity,
      taxRateCode: it.taxRateCode,
      taxRate: it.taxRate,
      amount,
      taxAmount,
      totalAmount: +(amount + taxAmount).toFixed(2),
      warehouseId: it.warehouseId ?? defaultWarehouseId,
    }
  })
  const subtotal = +items.reduce((s, i) => s + i.amount, 0).toFixed(2)
  const tax = +items.reduce((s, i) => s + i.taxAmount, 0).toFixed(2)
  const total = +(subtotal + tax).toFixed(2)

  const order: SalesOrder = {
    id,
    orderNo: `SO-2026-${String(SALES_ORDERS.length + 1).padStart(4, '0')}`,
    customerId: cust.id,
    customerName: cust.name,
    salesPersonName: body.salesPersonName || cust.salesPersonName || '',
    currencyCode: body.currencyCode || 'CNY',
    currencySymbol: body.currencySymbol || '¥',
    paymentTerms: body.paymentTerms || 'Net 30',
    deliveryDate: body.deliveryDate || isoTime(7).slice(0, 10),
    status: 'draft',
    items,
    subtotalAmount: subtotal,
    taxAmount: tax,
    totalAmount: total,
    remark: body.remark || '',
    createdAt: isoTime(),
    updatedAt: isoTime(),
  }
  SALES_ORDERS.unshift(order)
  res.json(ok({ id: order.id }, 'created'))
})

// ── 更新（仅草稿可改） ──
adminCrmSalesOrderRouter.put('/crm/sales-order/:id', (req, res) => {
  const order = SALES_ORDERS.find(o => o.id === req.params.id)
  if (!order) { res.json(fail('sales order not found', 404)); return }
  if (order.status !== 'draft') { res.json(fail('only draft orders can be edited')); return }

  const body = req.body
  const defaultWarehouseId = WAREHOUSES[0]?.id ?? ''
  if (body.salesPersonName !== undefined) order.salesPersonName = body.salesPersonName
  if (body.paymentTerms !== undefined) order.paymentTerms = body.paymentTerms
  if (body.deliveryDate !== undefined) order.deliveryDate = body.deliveryDate
  if (body.remark !== undefined) order.remark = body.remark
  if (body.items !== undefined) {
    order.items = body.items.map((it: SalesOrderItem) => {
      const amount = +(it.price * it.quantity).toFixed(2)
      const taxAmount = +(amount * it.taxRate / 100).toFixed(2)
      return {
        ...it,
        id: it.id || guid(),
        orderId: order.id,
        skuCode: it.skuCode ?? '',
        skuName: it.skuName ?? it.productName ?? '',
        warehouseId: it.warehouseId ?? defaultWarehouseId,
        amount,
        taxAmount,
        totalAmount: +(amount + taxAmount).toFixed(2),
      }
    })
    order.subtotalAmount = +order.items.reduce((s, i) => s + i.amount, 0).toFixed(2)
    order.taxAmount = +order.items.reduce((s, i) => s + i.taxAmount, 0).toFixed(2)
    order.totalAmount = +(order.subtotalAmount + order.taxAmount).toFixed(2)
  }
  order.updatedAt = isoTime()
  res.json(ok(null, 'updated'))
})

// ── 状态变更 ──
adminCrmSalesOrderRouter.put('/crm/sales-order/:id/status', (req, res) => {
  const order = SALES_ORDERS.find(o => o.id === req.params.id)
  if (!order) { res.json(fail('sales order not found', 404)); return }

  const newStatus = req.body.status as SalesOrder['status']
  const valid: Record<SalesOrder['status'], SalesOrder['status'][]> = {
    draft: ['confirmed', 'cancelled'],
    confirmed: ['shipped', 'cancelled'],
    shipped: ['completed'],
    completed: [],
    cancelled: [],
  }
  if (!valid[order.status].includes(newStatus)) {
    res.json(fail(`cannot transition from ${order.status} to ${newStatus}`))
    return
  }

  // 当状态变为 shipped 时，检查库存并执行出库
  if (newStatus === 'shipped') {
    const stockCheck = checkStockAvailability(order)
    if (!stockCheck.available) {
      res.json(fail(`库存不足: ${stockCheck.insufficientItems.join(', ')}`))
      return
    }
    processSalesOutbound(order)
  }

  order.status = newStatus
  order.updatedAt = isoTime()
  res.json(ok(null, 'status updated'))
})

// ── 删除（仅草稿可删） ──
adminCrmSalesOrderRouter.delete('/crm/sales-order/:id', (req, res) => {
  const idx = SALES_ORDERS.findIndex(o => o.id === req.params.id)
  if (idx === -1) { res.json(fail('sales order not found', 404)); return }
  if (SALES_ORDERS[idx].status !== 'draft') { res.json(fail('only draft orders can be deleted')); return }
  SALES_ORDERS.splice(idx, 1)
  res.json(ok(null, 'deleted'))
})
