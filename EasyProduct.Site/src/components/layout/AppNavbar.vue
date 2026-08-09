<template>
  <header class="app-navbar">
    <div class="app-navbar__inner">
      <router-link
        to="/"
        class="app-navbar__brand"
      >
        {{ t('common.app.name') }}
      </router-link>
      <nav class="app-navbar__links">
        <router-link
          v-for="item in NAV_ITEMS"
          :key="item.path"
          :to="item.path"
        >
          {{ t(item.titleKey) }}
        </router-link>
      </nav>
      <button
        class="app-navbar__locale"
        @click="handleToggleLocale"
      >
        {{ locale === 'zh-CN' ? t('common.locale.enUS') : t('common.locale.zhCN') }}
      </button>
    </div>
  </header>
</template>

<script setup lang="ts">
import { useI18n } from 'vue-i18n'
import { setLocale } from '@/i18n'

const { t, locale } = useI18n()

const NAV_ITEMS = [{ path: '/', titleKey: 'common.nav.home' }] as const

const handleToggleLocale = () => {
  setLocale(locale.value === 'zh-CN' ? 'en-US' : 'zh-CN')
}
</script>

<style scoped lang="scss">
.app-navbar {
  border-bottom: 1px solid $color-border;
  background: $color-bg;

  &__inner {
    @include site-container;
    display: flex;
    align-items: center;
    gap: $spacing-lg;
    height: 64px;
  }

  &__brand {
    font-weight: 700;
    font-size: 18px;
  }

  &__links {
    display: flex;
    gap: $spacing-md;

    a.router-link-active {
      color: $color-primary;
    }
  }

  &__locale {
    margin-left: auto;
    border: 1px solid $color-border;
    border-radius: $radius-sm;
    background: transparent;
    padding: $spacing-xs $spacing-sm;
    cursor: pointer;
  }
}
</style>
