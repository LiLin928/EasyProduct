// src/api/basic/profile.ts
import { get, put } from '@/utils/request'
import type { ProfileInfo, ProfileUpdateParams, ChangePwdParams } from '@/types/basic'

/** 获取当前用户信息 */
export const getProfile = () =>
  get<ProfileInfo>('/api/admin/basic/profile')

/** 更新当前用户基本信息 */
export const updateProfile = (data: ProfileUpdateParams) =>
  put<null>('/api/admin/basic/profile', data)

/** 修改密码 */
export const changePassword = (data: ChangePwdParams) =>
  put<null>('/api/admin/basic/profile/password', data)