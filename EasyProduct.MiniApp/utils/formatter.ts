// utils/formatter.ts —— 格式化工具函数

/** CDN 基础地址（从环境配置读取） */
const CDN_BASE = 'https://cdn.example.com'

/** 占位图片 */
const PLACEHOLDER_IMAGE = '/static/images/placeholder.jpg'

/** 格式化图片 URL
 * @param url 原始 URL
 * @returns 完整的图片 URL
 */
export function formatImageUrl(url: string | undefined): string {
  if (!url) return PLACEHOLDER_IMAGE
  if (url.startsWith('http')) return url
  if (url.startsWith('/')) return url
  return `${CDN_BASE}/${url}`
}

/** 格式化价格（分转元）
 * @param price 价格（分）
 * @returns 格式化后的价格字符串
 */
export function formatPrice(price: number): string {
  return (price / 100).toFixed(2)
}

/** 格式化价格显示（带¥符号）
 * @param price 价格（分）
 * @returns 格式化后的价格字符串
 */
export function formatPriceWithSymbol(price: number): string {
  return `¥${formatPrice(price)}`
}

/** 格式化时间
 * @param timestamp 时间戳或日期字符串
 * @returns 格式化后的时间字符串（YYYY-MM-DD）
 */
export function formatTime(timestamp: string | number | Date): string {
  const date = new Date(timestamp)
  const year = date.getFullYear()
  const month = String(date.getMonth() + 1).padStart(2, '0')
  const day = String(date.getDate()).padStart(2, '0')
  return `${year}-${month}-${day}`
}

/** 格式化日期时间
 * @param timestamp 时间戳或日期字符串
 * @returns 格式化后的时间字符串（YYYY-MM-DD HH:mm:ss）
 */
export function formatDateTime(timestamp: string | number | Date): string {
  const date = new Date(timestamp)
  const year = date.getFullYear()
  const month = String(date.getMonth() + 1).padStart(2, '0')
  const day = String(date.getDate()).padStart(2, '0')
  const hour = String(date.getHours()).padStart(2, '0')
  const minute = String(date.getMinutes()).padStart(2, '0')
  const second = String(date.getSeconds()).padStart(2, '0')
  return `${year}-${month}-${day} ${hour}:${minute}:${second}`
}

/** 格式化手机号（隐藏中间4位）
 * @param phone 手机号
 * @returns 格式化后的手机号
 */
export function maskPhone(phone: string): string {
  if (!phone || phone.length !== 11) return phone
  return `${phone.slice(0, 3)}****${phone.slice(7)}`
}

/** 格式化数字（超过999显示为999+）
 * @param num 数字
 * @returns 格式化后的字符串
 */
export function formatCount(num: number): string {
  if (num > 999) return '999+'
  return String(num)
}

/** 格式化文件大小
 * @param bytes 字节数
 * @returns 格式化后的文件大小
 */
export function formatFileSize(bytes: number): string {
  if (bytes === 0) return '0 B'
  const k = 1024
  const sizes = ['B', 'KB', 'MB', 'GB']
  const i = Math.floor(Math.log(bytes) / Math.log(k))
  return `${(bytes / Math.pow(k, i)).toFixed(2)} ${sizes[i]}`
}
