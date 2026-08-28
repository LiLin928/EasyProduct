// src/helpers/id.ts
import Mock from 'mockjs'

/** GUID 主键（全库主键形态，后端序列化小写） */
export const guid = (): string => Mock.mock('@guid').toLowerCase()

/** ISO 8601 时间（全库时间形态）；可选 dayOffset 正为未来、负为过去 */
export const isoTime = (dayOffset: number = 0): string => {
  const d = new Date()
  if (dayOffset) d.setDate(d.getDate() + dayOffset)
  return d.toISOString()
}

/** 业务编码：前缀 + 时间戳 + 随机数，如 CU20260808123045123 */
export const code = (prefix: string): string =>
  `${prefix}${Mock.mock('@datetime("yyyyMMddHHmmss")')}${Mock.mock('@integer(100, 999)')}`
