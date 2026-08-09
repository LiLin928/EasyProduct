const ACCESS_TOKEN = 'access_token'
const REFRESH_TOKEN = 'refresh_token'

export const getAccessToken = (): string => localStorage.getItem(ACCESS_TOKEN) ?? ''
export const getRefreshToken = (): string => localStorage.getItem(REFRESH_TOKEN) ?? ''

export function setTokens(accessToken: string, refreshToken: string): void {
  localStorage.setItem(ACCESS_TOKEN, accessToken)
  localStorage.setItem(REFRESH_TOKEN, refreshToken)
}

export function clearTokens(): void {
  localStorage.removeItem(ACCESS_TOKEN)
  localStorage.removeItem(REFRESH_TOKEN)
}
