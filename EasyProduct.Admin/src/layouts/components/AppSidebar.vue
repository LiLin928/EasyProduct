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
      <template
        v-for="item in menus"
        :key="item.id"
      >
        <!-- 有子菜单 -->
        <el-sub-menu
          v-if="item.children && item.children.length > 0"
          :index="item.path"
        >
          <template #title>
            <el-icon><component :is="item.icon" /></el-icon>
            <span>{{ t(item.titleKey) }}</span>
          </template>
          <el-menu-item
            v-for="child in item.children"
            :key="child.id"
            :index="child.path"
          >
            <el-icon><component :is="child.icon" /></el-icon>
            <span>{{ t(child.titleKey) }}</span>
          </el-menu-item>
        </el-sub-menu>
        <!-- 无子菜单 -->
        <el-menu-item
          v-else
          :index="item.path"
        >
          <el-icon><component :is="item.icon" /></el-icon>
          <span>{{ t(item.titleKey) }}</span>
        </el-menu-item>
      </template>
    </el-menu>
  </div>
</template>

<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { useRoute } from 'vue-router'
import { getUserMenuTree } from '@/api/basic/menu'
import { useAppStore } from '@/stores/app'
import { useI18n } from 'vue-i18n'
import type { MenuItem } from '@/types/basic'

const { t } = useI18n()
const route = useRoute()
const appStore = useAppStore()
const menus = ref<MenuItem[]>([])

onMounted(async () => {
  menus.value = await getUserMenuTree()
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
