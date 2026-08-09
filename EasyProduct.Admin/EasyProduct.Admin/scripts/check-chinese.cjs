// 扫描 src 下 .vue/.ts 中的硬编码中文（i18n 语言包目录除外）
const fs = require('fs')
const path = require('path')

const SRC = path.resolve(__dirname, '../src')
const IGNORE_DIRS = ['i18n']
const EXTS = ['.vue', '.ts']
const CJK = /[一-龥]/
let hits = 0

function walk(dir) {
  for (const name of fs.readdirSync(dir)) {
    const full = path.join(dir, name)
    const stat = fs.statSync(full)
    if (stat.isDirectory()) {
      if (!IGNORE_DIRS.includes(name)) walk(full)
      continue
    }
    if (!EXTS.includes(path.extname(name))) continue
    const lines = fs.readFileSync(full, 'utf8').split('\n')
    lines.forEach((line, i) => {
      const trimmed = line.trim()
      if (CJK.test(line) && !trimmed.startsWith('//') && !trimmed.startsWith('*')) {
        console.error(`${path.relative(SRC, full)}:${i + 1}: ${trimmed}`)
        hits += 1
      }
    })
  }
}

walk(SRC)
if (hits > 0) {
  console.error(`check:i18n FAILED: ${hits} hardcoded Chinese line(s)`)
  process.exit(1)
}
console.log('check:i18n passed')
