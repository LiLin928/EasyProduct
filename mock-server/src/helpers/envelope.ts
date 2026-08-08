// src/helpers/envelope.ts
/** 统一响应信封（与后端规范 5.3 逐字一致） */
export const ok = <T>(data: T, message = '操作成功') =>
  ({ code: 200, message, data, timestamp: Date.now() })

export const fail = (message: string, code = 400) =>
  ({ code, message, data: null, timestamp: Date.now() })

export interface PageData<T> {
  list: T[]
  total: number
  pageIndex: number
  pageSize: number
  totalPages: number
  hasNextPage: boolean
  hasPrevPage: boolean
}

export function paginate<T>(source: T[], pageIndex = 1, pageSize = 10, keyword?: string): PageData<T> {
  const filtered = keyword
    ? source.filter((i) => JSON.stringify(i).toLowerCase().includes(keyword.toLowerCase()))
    : source
  const totalPages = Math.ceil(filtered.length / pageSize)
  return {
    list: filtered.slice((pageIndex - 1) * pageSize, pageIndex * pageSize),
    total: filtered.length,
    pageIndex,
    pageSize,
    totalPages,
    hasNextPage: pageIndex < totalPages,
    hasPrevPage: pageIndex > 1,
  }
}