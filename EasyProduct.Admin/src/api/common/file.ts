// src/api/common/file.ts
import { post } from '@/utils/request'

/** 上传响应 */
export interface UploadResult {
  url: string
  fileName: string
}

/**
 * 上传文件（multipart form-data）
 * @param file 文件对象
 */
export const uploadFile = (file: File): Promise<UploadResult> => {
  const formData = new FormData()
  formData.append('file', file)
  return post<UploadResult>('/api/admin/basic/file/upload', formData)
}
