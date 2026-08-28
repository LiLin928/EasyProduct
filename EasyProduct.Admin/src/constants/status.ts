// src/constants/status.ts
import type { StatusOption } from '@/components/common/BaseStatusTag.vue'

/** 通用启用/停用状态选项，所有 basic 模块列表页共用 */
export const ENABLED_DISABLED_STATUS: Record<string, StatusOption> = {
  enabled: { label: 'common.status.enabled', type: 'success' },
  disabled: { label: 'common.status.disabled', type: 'danger' },
}
