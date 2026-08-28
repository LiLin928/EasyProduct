 import { get, post, put, del } from '@/utils/request'
 import type { PageResult } from '@/types/api'
 import type { Warehouse } from '@/types/crm'

 export interface WarehouseQuery {
   pageIndex: number
   pageSize: number
   code?: string
   name?: string
   status?: string
 }

 export const getWarehouseList = (params: WarehouseQuery) =>
   get<PageResult<Warehouse>>('/api/admin/crm/warehouse/list', params)

 export const getWarehouseById = (id: string) =>
   get<Warehouse>(`/api/admin/crm/warehouse/${id}`)

 export const getWarehouseOptions = () =>
   get<Array<{ id: string; name: string; code: string }>>('/api/admin/crm/warehouse/options')

 export const createWarehouse = (data: Partial<Warehouse>) =>
   post<{ id: string }>('/api/admin/crm/warehouse', data)

 export const updateWarehouse = (id: string, data: Partial<Warehouse>) =>
   put<null>(`/api/admin/crm/warehouse/${id}`, data)

 export const deleteWarehouse = (id: string) =>
   del<null>(`/api/admin/crm/warehouse/${id}`)
