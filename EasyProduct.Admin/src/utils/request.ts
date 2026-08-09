import axios from 'axios'
import { ElMessage, ElMessageBox } from 'element-plus'
import type { ApiResponse } from '@/types/api'
import { clearTokens, getAccessToken } from '@/utils/auth'

const service = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL || '',
  timeout: 15000,
})

service.interceptors.request.use((config) => {
  const token = getAccessToken()
  if (token) config.headers.Authorization = `Bearer ${token}`
  config.headers['Accept-Language'] = localStorage.getItem('locale') || 'zh-CN'
  return config
})

let tokenExpired = false // 防止 401 多次弹窗

service.interceptors.response.use(
  (response) => {
    const res = response.data as ApiResponse<unknown>
    if (res.code === 200) return res.data as never
    if (res.code === 401 && !tokenExpired) {
      tokenExpired = true
      clearTokens()
      const redirect = encodeURIComponent(window.location.pathname + window.location.search)
      ElMessageBox.confirm('common.error.tokenExpired', 'common.tip', { type: 'warning' })
        .finally(() => {
          tokenExpired = false
          window.location.href = `/login?redirect=${redirect}`
        })
      return Promise.reject(new Error(res.message))
    }
    if (res.code === 403) ElMessage.error('common.error.forbidden')
    else ElMessage.error(res.message || 'common.error.request')
    return Promise.reject(new Error(res.message))
  },
  (error: unknown) => {
    ElMessage.error('common.error.network')
    return Promise.reject(error instanceof Error ? error : new Error(String(error)))
  },
)

export function get<T>(url: string, params?: unknown): Promise<T> {
  return service.get(url, { params }) as Promise<T>
}
export function post<T>(url: string, data?: unknown): Promise<T> {
  return service.post(url, data) as Promise<T>
}
export function put<T>(url: string, data?: unknown): Promise<T> {
  return service.put(url, data) as Promise<T>
}
export function del<T>(url: string, params?: unknown): Promise<T> {
  return service.delete(url, { params }) as Promise<T>
}
