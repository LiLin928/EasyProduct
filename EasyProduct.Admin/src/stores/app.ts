import { defineStore } from 'pinia'
import { ref, watch } from 'vue'
import type { Locale } from '@/i18n'
import { setLocale } from '@/i18n'

export const useAppStore = defineStore('app', () => {
  const sidebarCollapsed = ref(false)
  const theme = ref<'light' | 'dark'>((localStorage.getItem('theme') as 'light' | 'dark') || 'light')
  const locale = ref<Locale>((localStorage.getItem('locale') as Locale) || 'zh-CN')

  function toggleSidebar(): void {
    sidebarCollapsed.value = !sidebarCollapsed.value
  }

  function toggleTheme(): void {
    theme.value = theme.value === 'light' ? 'dark' : 'light'
  }

  function switchLocale(next: Locale): void {
    locale.value = next
    setLocale(next)
  }

  watch(
    theme,
    (v) => {
      document.documentElement.dataset.theme = v
      localStorage.setItem('theme', v)
    },
    { immediate: true },
  )

  return { sidebarCollapsed, theme, locale, toggleSidebar, toggleTheme, switchLocale }
})
