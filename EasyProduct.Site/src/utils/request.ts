import axios from 'axios'
import type { ApiResponse } from '@/types/api'
import { showToast } from '@/utils/toast'

const service = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL || '',
  timeout: 15000,
})

service.interceptors.request.use((config) => {
  config.headers['Accept-Language'] = localStorage.getItem('locale') || 'zh-CN'
  return config
})

service.interceptors.response.use(
  (response) => {
    const res = response.data as ApiResponse<unknown>
    if (res.code === 200) return res.data as never
    showToast(res.message || 'Error')
    return Promise.reject(new Error(res.message))
  },
  (error: unknown) => {
    showToast('Network Error')
    return Promise.reject(error instanceof Error ? error : new Error(String(error)))
  },
)

export function get<T>(url: string, params?: unknown): Promise<T> {
  return service.get(url, { params }) as Promise<T>
}
export function post<T>(url: string, data?: unknown): Promise<T> {
  return service.post(url, data) as Promise<T>
}
