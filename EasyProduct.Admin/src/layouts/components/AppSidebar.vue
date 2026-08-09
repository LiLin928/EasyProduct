<template>
  <div class="app-sidebar">
    <div class="app-sidebar__logo">
      {{ appStore.sidebarCollapsed ? 'EP' : t('common.app.name') }}
    </div>
    <el-menu
      :collapse="appStore.sidebarCollapsed"
      :default-active="route.path"
      router
      class="app-sidebar__menu"
    >
      <el-menu-item
        v-for="item in menus"
        :key="item.id"
        :index="item.path"
      >
        <span>{{ t(item.titleKey) }}</span>
      </el-menu-item>
    </el-menu>
  </div>
</template>

<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { useRoute } from 'vue-router'
import { getMenuList } from '@/api/basic/menu'
import { useAppStore } from '@/stores/app'
import { useI18n } from 'vue-i18n'
import type { MenuItem } from '@/types/basic'

const { t } = useI18n()
const route = useRoute()
const appStore = useAppStore()
const menus = ref<MenuItem[]>([])

onMounted(async () => {
  menus.value = await getMenuList()
})
</script>

<style scoped lang="scss">
.app-sidebar {
  height: 100%;
  background: var(--ep-bg-card);
  border-right: 1px solid var(--ep-border);

  &__logo {
    height: 56px;
    display: flex;
    align-items: center;
    justify-content: center;
    font-weight: 600;
  }

  &__menu {
    border-right: none;
  }
}
</style>
