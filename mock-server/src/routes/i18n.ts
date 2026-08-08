// src/routes/i18n.ts
import { Router } from 'express'
import { readFileSync } from 'node:fs'
import { fileURLToPath } from 'node:url'
import path from 'node:path'

export const i18nRouter = Router()
const I18N_DIR = path.join(path.dirname(fileURLToPath(import.meta.url)), '../i18n')

/** 返回原始 JSON（不套信封）——模拟一期 nginx 静态托管 deploy/i18n/ */
i18nRouter.get('/:lang/:file', (req, res) => {
  const { lang, file } = req.params
  if (!/^(zh-CN|en-US)$/.test(lang) || !/^[a-z]+\.json$/.test(file)) {
    res.status(404).end()
    return
  }
  try {
    const json = readFileSync(path.join(I18N_DIR, lang, file), 'utf8')
    res.type('application/json').send(json)
  } catch {
    res.status(404).end()
  }
})
