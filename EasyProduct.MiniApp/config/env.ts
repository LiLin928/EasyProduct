// config/env.ts —— 替代原 USE_MOCK 硬编码（规范 4.5）
export const ENV = {
  /** dev 可临时开；build 必须 false。F0~F3 阶段走 mock-server */
  useMock: true,
  /** 只调 /api/app/**；mock 阶段指向 7700，联调改真后端域名 */
  apiBase: 'http://localhost:7700/api/app',
}
