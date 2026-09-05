// stores/member.store.ts —— 会员状态管理
import { BaseStore } from './base.store'
import type { Member } from '../types/member.types'
import { getMemberToken, setMemberToken, removeMemberToken, setMemberInfo, clearMemberInfo, getMemberInfo } from '../utils/storage'

interface MemberState {
  isLoggedIn: boolean
  member: Member | null
  token: string
}

export class MemberStore extends BaseStore<MemberState> {
  private static instance: MemberStore

  static getInstance(): MemberStore {
    if (!MemberStore.instance) {
      MemberStore.instance = new MemberStore()
    }
    return MemberStore.instance
  }

  constructor() {
    const token = getMemberToken()
    const memberInfo = getMemberInfo()
    super({
      isLoggedIn: !!token,
      member: memberInfo ? {
        id: memberInfo.id,
        nickName: memberInfo.nickName,
        avatar: memberInfo.avatar,
        level: memberInfo.level,
        points: memberInfo.points,
      } : null,
      token: token || '',
    })
  }

  /** 登录成功 */
  login(token: string, member: Member): void {
    setMemberToken(token)
    setMemberInfo({
      id: member.id,
      nickName: member.nickName,
      avatar: member.avatar,
      level: member.level,
      points: member.points,
    })
    this.setState({
      isLoggedIn: true,
      token,
      member,
    })
  }

  /** 退出登录 */
  logout(): void {
    removeMemberToken()
    clearMemberInfo()
    this.setState({
      isLoggedIn: false,
      token: '',
      member: null,
    })
  }

  /** 更新会员信息 */
  updateMember(member: Partial<Member>): void {
    if (this.state.member) {
      const updatedMember = { ...this.state.member, ...member }
      this.setState({
        member: updatedMember,
      })
      // 同时更新 storage
      setMemberInfo({
        id: updatedMember.id,
        nickName: updatedMember.nickName,
        avatar: updatedMember.avatar,
        level: updatedMember.level,
        points: updatedMember.points,
      })
    }
  }
}

export const memberStore = MemberStore.getInstance()
