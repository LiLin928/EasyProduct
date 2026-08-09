/** 统一响应信封（HTTP 一律 200，code===200 成功） */
export interface ApiResponse<T> {
  code: number
  message: string
  data: T
  timestamp: number
}

/** 分页查询参数 */
export interface PageQuery {
  pageIndex: number
  pageSize: number
}

/** 分页响应 */
export interface PageResult<T> {
  list: T[]
  total: number
}
