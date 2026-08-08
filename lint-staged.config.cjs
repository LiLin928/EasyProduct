// 按路径把暂存文件路由到对应 app 的 ESLint（三 app 独立 package.json，不做 workspace）
module.exports = {
  'EasyProduct.Admin/**/*.{vue,ts}': () => 'cd EasyProduct.Admin && npx eslint --ext .vue,.ts --fix',
  'EasyProduct.Site/**/*.{vue,ts}': () => 'cd EasyProduct.Site && npx eslint --ext .vue,.ts --fix',
}