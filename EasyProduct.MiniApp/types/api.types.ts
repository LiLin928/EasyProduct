// types/api.types.ts —— 新增类型一律无 I 前缀（规范 1.3 + 4.3 注）
/** 统一响应信封 */
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
