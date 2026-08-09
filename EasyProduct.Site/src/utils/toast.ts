/** Site 轻量提示（不引入 Element Plus） */
export function showToast(message: string): void {
  const el = document.createElement('div')
  el.className = 'app-toast'
  el.textContent = message
  document.body.appendChild(el)
  window.setTimeout(() => el.remove(), 2500)
}