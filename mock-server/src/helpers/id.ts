// src/helpers/id.ts
import Mock from 'mockjs'

/** GUID 主键（全库主键形态） */
export const guid = (): string => Mock.mock('@guid')

/** ISO 8601 时间（全库时间形态） */
export const isoTime = (): string => new Date().toISOString()

/** 业务编码：前缀 + 时间戳 + 随机数，如 CU20260808123045123 */
export const code = (prefix: string): string =>
  `${prefix}${Mock.mock('@datetime("yyyyMMddHHmmss")')}${Mock.mock('@integer(100, 999)')}`