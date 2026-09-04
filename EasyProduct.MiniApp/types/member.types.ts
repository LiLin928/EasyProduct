// types/member.types.ts —— 会员相关类型定义

/** 会员信息 */
export interface Member {
  id: string
  openId: string
  nickname: string
  avatarUrl: string
  phone: string
  gender: 0 | 1 | 2
  level?: string
  points?: number
  balance?: number
  createdAt?: string
}

/** 登录响应 */
export interface LoginResult {
  token: string
  member: Member
}
