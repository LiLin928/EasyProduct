import { ref, computed, type Ref, type ComputedRef } from 'vue'

export type CronPattern = 'hourly' | 'daily' | 'weekly' | 'monthly' | 'custom'

export interface CronBuilderReturn {
  // 状态
  pattern: Ref<CronPattern>
  hour: Ref<number>
  minute: Ref<number>
  dayOfWeek: Ref<number>
  dayOfMonth: Ref<number>
  
  // 计算属性
  cronExpression: ComputedRef<string>
  isCustom: ComputedRef<boolean>
  
  // 方法
  parseCron: (cron: string) => boolean
  reset: () => void
}

/**
 * Cron 表达式构建器
 * 支持可视化配置常用定时模式，自动转换为 Cron 表达式
 * 
 * @param initialCron - 初始 Cron 表达式（用于编辑时解析）
 * @returns Cron 构建器状态和操作方法
 */
export function useCronBuilder(initialCron: string = ''): CronBuilderReturn {
  const pattern = ref<CronPattern>('daily')
  const hour = ref<number>(1)
  const minute = ref<number>(0)
  const dayOfWeek = ref<number>(1)
  const dayOfMonth = ref<number>(1)

  /**
   * 将 Cron 表达式解析为配置状态
   */
  const parseCron = (cron: string): boolean => {
    if (!cron || typeof cron !== 'string') return false

    const parts = cron.trim().split(/\s+/)
    if (parts.length !== 6 && parts.length !== 7) return false

    const [seconds, minutes, hours, dayOfMonthStr, month, dayOfWeekStr] = parts

    // 每小时: 0 0 * * * ?
    if (seconds === '0' && minutes === '0' && hours === '*' && 
        dayOfMonthStr === '*' && month === '*' && dayOfWeekStr === '?') {
      pattern.value = 'hourly'
      return true
    }

    // 每天: 0 mm HH * * ?
    if (seconds === '0' && dayOfMonthStr === '*' && month === '*' && dayOfWeekStr === '?') {
      const h = parseInt(hours, 10)
      const m = parseInt(minutes, 10)
      if (!isNaN(h) && !isNaN(m) && h >= 0 && h <= 23 && m >= 0 && m <= 59) {
        pattern.value = 'daily'
        hour.value = h
        minute.value = m
        return true
      }
    }

    // 每周: 0 mm HH ? * D
    if (seconds === '0' && dayOfMonthStr === '?' && month === '*') {
      const h = parseInt(hours, 10)
      const m = parseInt(minutes, 10)
      const dw = parseInt(dayOfWeekStr, 10)
      if (!isNaN(h) && !isNaN(m) && !isNaN(dw) && 
          h >= 0 && h <= 23 && m >= 0 && m <= 59 && dw >= 1 && dw <= 7) {
        pattern.value = 'weekly'
        hour.value = h
        minute.value = m
        dayOfWeek.value = dw
        return true
      }
    }

    // 每月: 0 mm HH DD * ?
    if (seconds === '0' && month === '*' && dayOfWeekStr === '?') {
      const h = parseInt(hours, 10)
      const m = parseInt(minutes, 10)
      const dm = parseInt(dayOfMonthStr, 10)
      if (!isNaN(h) && !isNaN(m) && !isNaN(dm) && 
          h >= 0 && h <= 23 && m >= 0 && m <= 59 && dm >= 1 && dm <= 31) {
        pattern.value = 'monthly'
        hour.value = h
        minute.value = m
        dayOfMonth.value = dm
        return true
      }
    }

    // 无法解析，使用自定义
    pattern.value = 'custom'
    return false
  }

  /**
   * 根据当前配置生成 Cron 表达式
   */
  const cronExpression = computed(() => {
    switch (pattern.value) {
      case 'hourly':
        return '0 0 * * * ?'
      case 'daily':
        return `0 ${minute.value} ${hour.value} * * ?`
      case 'weekly':
        return `0 ${minute.value} ${hour.value} ? * ${dayOfWeek.value}`
      case 'monthly':
        return `0 ${minute.value} ${hour.value} ${dayOfMonth.value} * ?`
      default:
        return ''
    }
  })

  /**
   * 是否为自定义模式
   */
  const isCustom = computed(() => pattern.value === 'custom')

  /**
   * 重置为默认值
   */
  const reset = () => {
    pattern.value = 'daily'
    hour.value = 1
    minute.value = 0
    dayOfWeek.value = 1
    dayOfMonth.value = 1
  }

  return {
    // 状态
    pattern,
    hour,
    minute,
    dayOfWeek,
    dayOfMonth,
    
    // 计算属性
    cronExpression,
    isCustom,
    
    // 方法
    parseCron,
    reset,
  }
}

/**
 * 星期选项（中文）
 */
export const DAYS_OF_WEEK_CN = [
  { value: 1, label: '周一' },
  { value: 2, label: '周二' },
  { value: 3, label: '周三' },
  { value: 4, label: '周四' },
  { value: 5, label: '周五' },
  { value: 6, label: '周六' },
  { value: 7, label: '周日' },
]

/**
 * 频率选项（中文）
 */
export const PATTERN_OPTIONS_CN = [
  { value: 'hourly' as CronPattern, label: '每小时', desc: '每小时的第0分钟执行' },
  { value: 'daily' as CronPattern, label: '每天', desc: '每天指定时间执行' },
  { value: 'weekly' as CronPattern, label: '每周', desc: '每周指定星期和时间执行' },
  { value: 'monthly' as CronPattern, label: '每月', desc: '每月指定日期和时间执行' },
  { value: 'custom' as CronPattern, label: '自定义', desc: '使用 Cron 表达式' },
]
