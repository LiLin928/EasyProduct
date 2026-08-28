import { get, post, put, del } from '@/utils/request'
import type { PageResult } from '@/types/api'
import type { FixedAsset, FixedAssetStatus, AssetDepreciation } from '@/types/crm'

export interface FixedAssetQuery {
  pageIndex: number
  pageSize: number
  category?: string
  status?: string
  keyword?: string
}

export const getFixedAssetList = (params: FixedAssetQuery) =>
  get<PageResult<FixedAsset>>('/api/admin/crm/fixed-asset/list', params)

export const getFixedAssetById = (id: string) =>
  get<FixedAsset>(`/api/admin/crm/fixed-asset/${id}`)

export const getFixedAssetDepreciations = (id: string) =>
  get<AssetDepreciation[]>(`/api/admin/crm/fixed-asset/${id}/depreciations`)

export const createFixedAsset = (data: Partial<FixedAsset>) =>
  post<{ id: string }>('/api/admin/crm/fixed-asset', data)

export const updateFixedAsset = (id: string, data: Partial<FixedAsset>) =>
  put<null>(`/api/admin/crm/fixed-asset/${id}`, data)

export const updateFixedAssetStatus = (id: string, data: { status: FixedAssetStatus }) =>
  post<null>(`/api/admin/crm/fixed-asset/${id}/status`, data)

export const deleteFixedAsset = (id: string) =>
  del<null>(`/api/admin/crm/fixed-asset/${id}`)
