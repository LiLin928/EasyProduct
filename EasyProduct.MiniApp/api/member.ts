// api/member.ts —— 会员相关 API
import { request } from '../utils/request'
import type { Member, LoginResult } from '../types/member.types'

const BASE_URL = '/member'

/** 微信登录
 * @param code 微信登录code
 */
export function wxLogin(code: string): Promise<LoginResult> {
  return request<LoginResult>({
    url: `${BASE_URL}/wx-login`,
    method: 'POST',
    data: { code },
    needAuth: false,
  })
}

/** 获取会员信息 */
export function getMemberInfo(): Promise<Member> {
  return request<Member>({
    url: `${BASE_URL}/info`,
    method: 'GET',
  })
}

/** 更新会员信息
 * @param data 会员数据
 */
export function updateMemberInfo(data: Partial<Member>): Promise<Member> {
  return request<Member>({
    url: `${BASE_URL}/info`,
    method: 'PUT',
    data,
  })
}
