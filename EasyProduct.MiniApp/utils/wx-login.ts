// utils/wx-login.ts —— code 换会员 Token（mock 阶段不校验真实 code）
import { ENV } from '../config/env'
import { setMemberToken } from './storage'

export async function silentLogin(): Promise<string> {
  const { code } = await wx.login()
  const res = await new Promise<WechatMiniprogram.RequestSuccessCallbackResult>((resolve, reject) => {
    wx.request({
      url: `${ENV.apiBase}/auth/wx-login`,
      method: 'POST',
      data: { code },
      success: resolve,
      fail: reject,
    })
  })
  const envelope = res.data as { code: number; data: { memberToken: string } }
  if (envelope.code !== 200) throw new Error('wx-login failed')
  setMemberToken(envelope.data.memberToken)
  return envelope.data.memberToken
}
