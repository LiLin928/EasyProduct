// src/server.ts
import express from 'express'
import cors from 'cors'
import { ok } from './helpers/envelope.js'
import { resetAll } from './helpers/registry.js'
import { adminGuard, appGuard } from './helpers/auth.js'

const PORT = 7700
const app = express()
app.use(cors())
app.use(express.json())

// 模块 mock 状态表（与 README 一致；新增模块在此登记）
const MOCK_STATUS = [
  { zone: 'admin', module: 'Basic（认证/菜单/字典骨架）', status: 'pending', backendPhase: 'P1' },
  { zone: 'site', module: '官网内容（首页聚合骨架）', status: 'pending', backendPhase: 'P2' },
  { zone: 'app', module: '商城（会员登录骨架）', status: 'pending', backendPhase: 'P3' },
]

app.get('/__mock/status', (_req, res) => res.json(ok(MOCK_STATUS)))
app.post('/__mock/reset', (_req, res) => {
  resetAll()
  res.json(ok(null, '已重置'))
})

// Task 2 挂载路由（先占位注释）：
// app.use('/api/admin', adminGuard)  + adminAuthRouter / menuRouter / dictRouter
// app.use('/api/site', siteHomeRouter)
// app.use('/api/app', appGuard) + appAuthRouter
// app.use('/api/i18n', i18nRouter)

app.listen(PORT, () => {
  // eslint-disable-next-line no-console
  console.log(`[mock-server] running at http://localhost:${PORT}`)
})

export { adminGuard, appGuard }