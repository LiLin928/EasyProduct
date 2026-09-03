<template>
  <nav
    class="navbar"
    :class="{ scrolled: isScrolled, 'menu-open': isMobileMenuOpen }"
  >
    <div class="navbar-content">
      <router-link
        to="/"
        class="logo"
        @click="handleLogoClick"
      >
        <span class="logo-text">StarryBird</span>
      </router-link>

      <ul
        v-if="!isCompact"
        class="nav-menu"
      >
        <li>
          <router-link
            to="/"
            class="nav-link"
          >
            {{ t('common.nav.home') }}
          </router-link>
        </li>
        <li>
          <router-link
            to="/products"
            class="nav-link"
          >
            {{ t('common.nav.products') }}
          </router-link>
        </li>
        <li>
          <router-link
            to="/about"
            class="nav-link"
          >
            {{ t('common.nav.about') }}
          </router-link>
        </li>
        <li>
          <router-link
            to="/news"
            class="nav-link"
          >
            {{ t('common.nav.news') }}
          </router-link>
        </li>
        <li>
          <router-link
            to="/videos"
            class="nav-link"
          >
            {{ t('common.nav.videos') }}
          </router-link>
        </li>
        <li>
          <router-link
            to="/downloads"
            class="nav-link"
          >
            {{ t('common.nav.downloads') }}
          </router-link>
        </li>
        <li>
          <router-link
            to="/contact"
            class="nav-link"
          >
            {{ t('common.nav.contact') }}
          </router-link>
        </li>
      </ul>

      <div class="nav-actions">
        <button
          type="button"
          class="action-btn lang-btn"
          @click="toggleLanguage"
        >
          {{ currentLanguage === 'zh-CN' ? t('common.locale.enUS') : t('common.locale.zhCN') }}
        </button>

        <router-link
          to="/inquiry"
          class="action-btn inquiry-btn"
          :aria-label="t('common.nav.inquiry')"
          :title="t('common.nav.inquiry')"
          @click="closeMobileMenu"
        >
          <span
            class="inquiry-icon"
            aria-hidden="true"
          >🛒</span>
          <span
            v-if="badgeText"
            class="badge-count"
          >{{ badgeText }}</span>
        </router-link>

        <!-- <router-link
          v-if="!isCompact"
          to="/admin"
          class="action-btn admin-btn"
          :aria-label="t('nav.admin')"
          :title="t('nav.admin')"
        >
          <span
            class="admin-icon"
            aria-hidden="true"
          >⚙</span>
          <span class="admin-text">{{ t('nav.admin') }}</span>
        </router-link> -->

        <button
          v-if="isCompact"
          type="button"
          class="hamburger"
          :class="{ open: isMobileMenuOpen }"
          :aria-expanded="isMobileMenuOpen"
          aria-controls="mobile-nav-drawer"
          :aria-label="isMobileMenuOpen ? t('common.nav.closeMenu') : t('common.nav.openMenu')"
          @click="toggleMobileMenu"
        >
          <span />
          <span />
          <span />
        </button>
      </div>
    </div>

    <MobileNavDrawer
      :open="isMobileMenuOpen"
      @close="closeMobileMenu"
    />
  </nav>
</template>

<script setup lang="ts">
import { computed, onMounted, onUnmounted, ref } from 'vue'
import { useI18n } from 'vue-i18n'
import { storeToRefs } from 'pinia'
import { useInquiryStore } from '@/stores/inquiry'
import MobileNavDrawer from '@/components/layout/MobileNavDrawer.vue'

/** ≤1024 使用汉堡菜单（含平板） */
const COMPACT_BREAKPOINT = 1024

const { t, locale } = useI18n()
const inquiryStore = useInquiryStore()

const { totalItems } = storeToRefs(inquiryStore)

// 本地状态：语言
const currentLanguage = ref(localStorage.getItem('locale') || 'zh-CN')

// 本地状态：移动端菜单
const isMobileMenuOpen = ref(false)

const isScrolled = ref(false)
const isCompact = ref(
  typeof window !== 'undefined' ? window.innerWidth <= COMPACT_BREAKPOINT : true
)

const badgeText = computed(() => {
  const count = totalItems.value
  if (count <= 0) return ''
  if (count > 99) return '99+'
  return String(count)
})

const handleScroll = () => {
  isScrolled.value = window.scrollY > 50
}

const handleResize = () => {
  const compact = window.innerWidth <= COMPACT_BREAKPOINT
  isCompact.value = compact
  if (!compact && isMobileMenuOpen.value) {
    closeMobileMenu()
  }
}

const toggleLanguage = () => {
  const newLang = currentLanguage.value === 'zh-CN' ? 'en-US' : 'zh-CN'
  currentLanguage.value = newLang
  localStorage.setItem('locale', newLang)
  locale.value = newLang
  // 切换语言后刷新页面
  window.location.reload()
}

const toggleMobileMenu = () => {
  isMobileMenuOpen.value = !isMobileMenuOpen.value
}

const closeMobileMenu = () => {
  isMobileMenuOpen.value = false
}

const handleLogoClick = () => {
  closeMobileMenu()
}

onMounted(() => {
  // 初始化语言
  const savedLocale = localStorage.getItem('locale')
  if (savedLocale) {
    currentLanguage.value = savedLocale
    locale.value = savedLocale
  }
  
  handleResize()
  window.addEventListener('scroll', handleScroll, { passive: true })
  window.addEventListener('resize', handleResize)
})

onUnmounted(() => {
  window.removeEventListener('scroll', handleScroll)
  window.removeEventListener('resize', handleResize)
  closeMobileMenu()
})
</script>

<style scoped lang="scss">
.navbar {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  height: 80px;
  background: rgb(255 255 255 / 95%);
  backdrop-filter: blur(20px);
  border-bottom: 1px solid rgb(26 91 167 / 10%);
  z-index: $z-index-navbar;
  transition: height $transition-base, box-shadow $transition-base;

  &.scrolled {
    height: 64px;
    box-shadow: $shadow-md;
  }

  @media (max-width: #{$breakpoint-tablet}) {
    height: 56px;

    &.scrolled {
      height: 56px;
    }
  }
}

.navbar-content {
  max-width: 1400px;
  margin: 0 auto;
  padding: 0 $spacing-xl;
  height: 100%;
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: $spacing-sm;

  @media (max-width: #{$breakpoint-tablet}) {
    padding: 0 12px 0 16px;
  }
}

.logo {
  display: flex;
  align-items: center;
  gap: $spacing-sm;
  text-decoration: none;
  flex-shrink: 0;
}

.logo-text {
  font-family: $font-heading;
  font-size: 24px;
  font-weight: 700;
  color: $color-primary;
  letter-spacing: -0.5px;

  @media (max-width: #{$breakpoint-tablet}) {
    font-size: 20px;
  }
}

.nav-menu {
  display: flex;
  gap: $spacing-lg;
  list-style: none;
  margin: 0;
  padding: 0;
  flex: 1;
  justify-content: center;
}

.nav-link {
  font-family: $font-heading;
  font-size: 15px;
  font-weight: 500;
  color: $color-text-secondary;
  text-decoration: none;
  transition: color $transition-fast;

  &:hover,
  &.router-link-active {
    color: $color-primary;
  }
}

.nav-actions {
  display: flex;
  align-items: center;
  gap: 8px;
  flex-shrink: 0;
}

.action-btn {
  font-family: $font-heading;
  font-size: 14px;
  font-weight: 600;
  border: none;
  cursor: pointer;
  text-decoration: none;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  transition: all $transition-fast;
  position: relative;
  background: transparent;
  color: $color-text-secondary;
}

.lang-btn {
  min-width: 40px;
  height: 40px;
  padding: 0 10px;
  border-radius: $radius-sm;
  border: 1px solid $color-border;
  color: $color-primary;
  background: $color-bg-primary;

  &:hover {
    border-color: $color-primary;
    background: rgba($color-primary, 0.06);
  }

  @media (min-width: #{$breakpoint-tablet + 1px}) {
    min-width: auto;
    height: auto;
    padding: 10px 18px;
    border-radius: $radius-md;
  }
}

.inquiry-btn {
  width: 40px;
  height: 40px;
  border-radius: $radius-sm;
  background: $color-accent;
  color: #fff;

  &:hover {
    background: $color-accent-dark;
  }

  @media (min-width: #{$breakpoint-tablet + 1px}) {
    width: auto;
    min-width: 48px;
    height: auto;
    padding: 10px 18px;
    border-radius: $radius-md;
  }
}

.inquiry-icon {
  font-size: 16px;
  line-height: 1;
}

.admin-btn {
  gap: 6px;
  height: auto;
  padding: 10px 16px;
  border-radius: $radius-md;
  border: 1px solid $color-border;
  color: $color-primary;
  background: $color-bg-primary;

  &:hover {
    border-color: $color-primary;
    background: rgba($color-primary, 0.06);
  }
}

.admin-icon {
  font-size: 15px;
  line-height: 1;
}

.admin-text {
  white-space: nowrap;
}

.badge-count {
  position: absolute;
  top: -4px;
  right: -4px;
  min-width: 18px;
  height: 18px;
  padding: 0 5px;
  background: $color-primary;
  color: #fff;
  font-size: 11px;
  font-weight: 700;
  border-radius: 999px;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  line-height: 1;
  border: 2px solid #fff;
}

.hamburger {
  width: 40px;
  height: 40px;
  display: inline-flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  gap: 5px;
  padding: 0;
  background: none;
  border: none;
  cursor: pointer;
  border-radius: $radius-sm;

  span {
    display: block;
    width: 20px;
    height: 2px;
    background: $color-primary;
    border-radius: 2px;
    transition: transform 280ms ease, opacity 280ms ease;
  }

  &.open {
    span:nth-child(1) {
      transform: translateY(7px) rotate(45deg);
    }

    span:nth-child(2) {
      opacity: 0;
    }

    span:nth-child(3) {
      transform: translateY(-7px) rotate(-45deg);
    }
  }

  &:hover {
    background: $color-bg-secondary;
  }
}
</style>
