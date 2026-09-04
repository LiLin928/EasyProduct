<template>
  <aside
    class="app-sidebar"
    :class="{ 'is-collapsed': appStore.sidebarCollapsed }"
  >
    <div class="app-sidebar__header">
      <div class="app-sidebar__logo">
        <div class="logo-icon">
          <span
            v-if="!appStore.sidebarCollapsed"
            class="logo-text"
          >Easy Product</span>
        </div>
      </div>
      <el-button
        class="app-sidebar__collapse-btn"
        text
        circle
        @click="appStore.toggleSidebar()"
      >
        <el-icon
          v-if="!appStore.sidebarCollapsed"
          class="collapse-icon"
        >
          <component is="ArrowRight" />
        </el-icon>
        <span
          v-else
          class="logo-text"
        >EP</span>
      </el-button>
    </div>
    <el-menu
      :collapse="appStore.sidebarCollapsed"
      :collapse-transition="false"
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
            <el-icon class="menu-icon">
              <component :is="item.icon" />
            </el-icon>
            <span class="menu-title">{{ t(item.titleKey) }}</span>
          </template>
          <el-menu-item
            v-for="child in item.children"
            :key="child.id"
            :index="child.path"
          >
            <el-icon class="menu-icon">
              <component :is="child.icon" />
            </el-icon>
            <span class="menu-title">{{ t(child.titleKey) }}</span>
          </el-menu-item>
        </el-sub-menu>
        <!-- 无子菜单 -->
        <el-menu-item
          v-else
          :index="item.path"
        >
          <el-icon class="menu-icon">
            <component :is="item.icon" />
          </el-icon>
          <span class="menu-title">{{ t(item.titleKey) }}</span>
        </el-menu-item>
      </template>
    </el-menu>
  </aside>
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
  width: 220px;
  height: 100vh;
  background: var(--ep-bg-card);
  // border-right: 1px solid var(--ep-border-light);
  display: flex;
  flex-direction: column;
  transition: width 0.3s cubic-bezier(0.4, 0, 0.2, 1);
  overflow: hidden;

  &.is-collapsed {
    width: 64px;

    :deep(.menu-title) {
      opacity: 0;
      transform: translateX(-10px);
    }

    .logo-text-accent {
      display: none;
    }
  }

  &__header {
    height: 60px;
    display: flex;
    align-items: center;
    justify-content: space-between;
    padding: 0 16px;
    border-bottom: 1px solid var(--ep-border-light);
    background: linear-gradient(135deg, var(--ep-bg-card) 0%, var(--ep-bg-page) 100%);
  }

  &__logo {
    display: flex;
    align-items: center;
    overflow: hidden;
    white-space: nowrap;

    .logo-icon {
      display: flex;
      align-items: center;
      gap: 4px;
    }

    .logo-text {
      font-weight: 700;
      font-size: 18px;
      color: var(--ep-primary);
      letter-spacing: -0.5px;
    }

    .logo-text-accent {
      font-weight: 600;
      font-size: 16px;
      color: var(--ep-text-primary);
      transition: opacity 0.3s;
    }
  }

  &__collapse-btn {
    width: 32px;
    height: 32px;
    border-radius: 8px;
    color: var(--ep-text-secondary);
    transition: all 0.3s;

    &:hover {
      background: var(--ep-bg-hover);
      color: var(--ep-primary);
      transform: scale(1.05);
    }

    .collapse-icon {
      transition: transform 0.3s cubic-bezier(0.4, 0, 0.2, 1);
    }
  }

  &__menu {
    flex: 1;
    border-right: none;
    overflow-y: auto;
    overflow-x: hidden;

    :deep(.el-menu) {
      background: transparent;
    }

    :deep(.el-menu-item),
    :deep(.el-sub-menu__title) {
      height: 44px;
      line-height: 44px;
      margin: 4px 8px;
      border-radius: 8px;
      transition: all 0.2s;

      &:hover {
        background: var(--ep-bg-hover);
        color: var(--ep-primary);
      }

      &.is-active {
        background: var(--ep-primary-light-9);
        color: var(--ep-primary);
        font-weight: 500;
      }
    }

    :deep(.el-sub-menu__title) {
      border-radius: 8px;
    }

    // :deep(.el-menu-item.is-active) {
    //   border-right: 3px solid var(--ep-primary);
    // }

    :deep(.menu-icon) {
      font-size: 18px;
      margin-right: 12px;
      transition: all 0.3s;
    }

    :deep(.menu-title) {
      transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
      white-space: nowrap;
      overflow: hidden;
      text-overflow: ellipsis;
    }

    // 侧边栏折叠状态
    :deep(.el-menu--collapse) {
      .el-menu-item,
      .el-sub-menu__title {
        justify-content: center;
        margin: 4px 8px;

        .menu-icon {
          margin-right: 0;
          font-size: 20px;
        }
      }
    }
  }
}

// 暗黑模式适配
:root[data-theme='dark'] .app-sidebar {
  &__header {
    background: linear-gradient(135deg, var(--ep-bg-card) 0%, rgba(0, 0, 0, 0.2) 100%);
  }
}
</style>