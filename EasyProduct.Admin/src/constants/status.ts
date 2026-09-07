// src/constants/status.ts
import type { StatusOption } from '@/components/common/BaseStatusTag.vue'

/**
 * 状态常量定义（int 类型）
 */

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

/** 通用启用/停用状态选项，所有 basic 模块列表页共用 */
export const ENABLED_DISABLED_STATUS: Record<string, StatusOption> = {
  enabled: { label: 'common.status.enabled', type: 'success' },
  disabled: { label: 'common.status.disabled', type: 'danger' },
}
