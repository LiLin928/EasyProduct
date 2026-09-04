// stores/member.store.ts —— 会员状态管理
import { BaseStore } from './base.store'
import type { Member } from '../types/member.types'
import { getMemberToken, setMemberToken, removeMemberToken } from '../utils/storage'

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
    super({
      isLoggedIn: !!token,
      member: null,
      token: token || '',
    })
  }

  /** 登录成功 */
  login(token: string, member: Member): void {
    setMemberToken(token)
    this.setState({
      isLoggedIn: true,
      token,
      member,
    })
  }

  /** 退出登录 */
  logout(): void {
    removeMemberToken()
    this.setState({
      isLoggedIn: false,
      token: '',
      member: null,
    })
  }

  /** 更新会员信息 */
  updateMember(member: Partial<Member>): void {
    if (this.state.member) {
      this.setState({
        member: { ...this.state.member, ...member },
      })
    }
  }
}

export const memberStore = MemberStore.getInstance()
