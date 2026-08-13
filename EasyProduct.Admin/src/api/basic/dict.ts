import { get, post, put, del } from '@/utils/request'
import type { PageResult } from '@/types/api'
import type {
  DictItem,
  DictType,
  DictTypeQuery,
  DictTypeCreateParams,
  DictData,
  DictDataQuery,
  DictDataCreateParams
} from '@/types/basic'

/** 按字典类型取字典项 */
export const getDictData = (typeCode: string) =>
  get<DictItem[]>('/api/admin/basic/dict-data', { typeCode })

// 字典类型管理
/** 字典类型列表（分页） */
export const getDictTypeList = (params: DictTypeQuery) =>
  get<PageResult<DictType>>('/api/admin/basic/dict-type/list', params)

/** 新增字典类型 */
export const createDictType = (data: DictTypeCreateParams) =>
  post<{ id: string }>('/api/admin/basic/dict-type', data)

/** 编辑字典类型 */
export const updateDictType = (id: string, data: Partial<DictTypeCreateParams>) =>
  put<{ id: string }>(`/api/admin/basic/dict-type/${id}`, data)

/** 删除字典类型 */
export const deleteDictType = (id: string) =>
  del<null>(`/api/admin/basic/dict-type/${id}`)

// 字典数据管理
/** 字典数据列表（分页） */
export const getDictDataList = (params: DictDataQuery) =>
  get<PageResult<DictData>>('/api/admin/basic/dict-data/list', params)

/** 新增字典数据 */
export const createDictData = (data: DictDataCreateParams) =>
  post<{ id: string }>('/api/admin/basic/dict-data', data)

/** 编辑字典数据 */
export const updateDictData = (id: string, data: Partial<DictDataCreateParams>) =>
  put<{ id: string }>(`/api/admin/basic/dict-data/${id}`, data)

/** 删除字典数据 */
export const deleteDictData = (id: string) =>
  del<null>(`/api/admin/basic/dict-data/${id}`)

/** 批量删除字典数据 */
export const deleteDictDataBatch = (ids: string[]) =>
  post<null>('/api/admin/basic/dict-data/batch-delete', { ids })
