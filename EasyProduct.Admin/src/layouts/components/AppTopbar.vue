<template>
  <div class="app-topbar">
    <el-button
      text
      @click="appStore.toggleSidebar()"
    >
      {{ appStore.sidebarCollapsed ? '>' : '<' }}
    </el-button>
    <div class="app-topbar__right">
      <el-switch
        :model-value="appStore.locale === 'zh-CN'"
        :active-text="t('common.locale.zhCN')"
        :inactive-text="t('common.locale.enUS')"
        @change="handleLocaleChange"
      />
      <el-button
        text
        @click="appStore.toggleTheme()"
      >
        {{ appStore.theme }}
      </el-button>
      <span class="app-topbar__user">{{ userStore.realName }}</span>
      <el-button
        text
        @click="handleLogout"
      >
        {{ t('common.login.logout') }}
      </el-button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useRouter } from 'vue-router'
import { useI18n } from 'vue-i18n'
import { useAppStore } from '@/stores/app'
import { useUserStore } from '@/stores/user'

const { t } = useI18n()
const router = useRouter()
const appStore = useAppStore()
const userStore = useUserStore()

const handleLocaleChange = (zh: boolean | string | number) => {
  appStore.switchLocale(zh ? 'zh-CN' : 'en-US')
}

const handleLogout = () => {
  userStore.logout()
  router.push('/login')
}
</script>

<style scoped lang="scss">
.app-topbar {
  display: flex;
  align-items: center;
  justify-content: space-between;
  width: 100%;

  &__right {
    display: flex;
    align-items: center;
    gap: $spacing-md;
  }

  &__user {
    color: var(--ep-text-secondary);
  }
}
</style>
