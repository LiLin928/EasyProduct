<template>
  <el-container class="main-layout">
    <el-aside :width="appStore.sidebarCollapsed ? '64px' : '220px'">
      <AppSidebar />
    </el-aside>
    <el-container>
      <el-header class="main-layout__header">
        <AppTopbar />
      </el-header>
      <TagsView />
      <el-main class="main-layout__body">
        <router-view />
      </el-main>
    </el-container>
  </el-container>
</template>

<script setup lang="ts">
import AppSidebar from './components/AppSidebar.vue'
import AppTopbar from './components/AppTopbar.vue'
import TagsView from './components/TagsView.vue'
import { useAppStore } from '@/stores/app'

const appStore = useAppStore()
</script>

<style scoped lang="scss">
.main-layout {
  height: 100%;

  &__header {
    display: flex;
    align-items: center;
    border-bottom: 1px solid var(--ep-border);
    background: var(--ep-bg-card);
    height: 56px;
  }

  &__body {
    padding: 12px;
  }
}
</style>
