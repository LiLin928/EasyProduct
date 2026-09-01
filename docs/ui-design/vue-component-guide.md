# Vue 组件实现指南

## 概述

本文档提供 EasyProduct Admin 管理系统中多页签和侧边栏组件的 Vue 实现指南，包括组件结构、代码示例和最佳实践。

---

## 1. 页签组件实现

### 1.1 组件结构

```
src/components/layout/
├── TabBar/                    # 页签栏组件
│   ├── index.vue             # 主组件
│   ├── TabItem.vue           # 页签项组件
│   └── QuickMenu.vue         # 快捷菜单
├── Sidebar/                   # 侧边栏组件
│   ├── index.vue             # 主组件
│   ├── MenuItem.vue          # 菜单项组件
│   ├── SubMenu.vue           # 子菜单组件
│   └── FloatingPanel.vue     # 悬浮面板
└── composables/
    ├── useTabs.ts            # 页签状态管理
    └── useSidebar.ts         # 侧边栏状态管理
```

### 1.2 页签栏组件 (TabBar/index.vue)

```vue
<template>
  <div class="tab-bar">
    <div class="tab-list" ref="tabListRef">
      <TabItem
        v-for="tab in tabs"
        :key="tab.id"
        :tab="tab"
        :active="activeTab === tab.id"
        @click="handleTabClick(tab)"
        @close="handleTabClose(tab)"
        @contextmenu.prevent="handleContextMenu($event, tab)"
      />
    </div>
    
    <!-- 新建页签按钮 -->
    <button class="tab-add" @click="handleAddClick">
      <el-icon><Plus /></el-icon>
    </button>
    
    <!-- 快捷菜单 -->
    <QuickMenu
      v-model:visible="quickMenuVisible"
      :position="quickMenuPosition"
      @select="handleQuickMenuSelect"
    />
    
    <!-- 右键菜单 -->
    <ContextMenu
      v-model:visible="contextMenuVisible"
      :position="contextMenuPosition"
      :tab="contextMenuTab"
      @refresh="handleRefresh"
      @close="handleCloseCurrent"
      @close-others="handleCloseOthers"
      @close-all="handleCloseAll"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { Plus } from '@element-plus/icons-vue'
import { useTabsStore } from '@/stores/tabs'
import TabItem from './TabItem.vue'
import QuickMenu from './QuickMenu.vue'
import ContextMenu from './ContextMenu.vue'
import type { TabItem as TabItemType } from '@/types/tabs'

const route = useRoute()
const router = useRouter()
const tabsStore = useTabsStore()

const tabListRef = ref<HTMLElement>()
const quickMenuVisible = ref(false)
const quickMenuPosition = ref({ x: 0, y: 0 })
const contextMenuVisible = ref(false)
const contextMenuPosition = ref({ x: 0, y: 0 })
const contextMenuTab = ref<TabItemType | null>(null)

const tabs = computed(() => tabsStore.tabs)
const activeTab = computed(() => tabsStore.activeTab)

// 监听路由变化，自动添加页签
watch(() => route.path, (newPath) => {
  if (shouldAddToTabs(route)) {
    tabsStore.addTab({
      id: route.name as string,
      title: route.meta.title as string,
      path: newPath,
      icon: route.meta.icon as string,
      closable: route.name !== 'Home'
    })
  }
}, { immediate: true })

const shouldAddToTabs = (route: any) => {
  return route.meta?.title && !route.meta?.noTab
}

const handleTabClick = (tab: TabItemType) => {
  tabsStore.setActiveTab(tab.id)
  router.push(tab.path)
}

const handleTabClose = (tab: TabItemType) => {
  tabsStore.removeTab(tab.id)
}

const handleAddClick = (event: MouseEvent) => {
  const rect = (event.target as HTMLElement).getBoundingClientRect()
  quickMenuPosition.value = {
    x: rect.left,
    y: rect.bottom + 8
  }
  quickMenuVisible.value = true
}

const handleContextMenu = (event: MouseEvent, tab: TabItemType) => {
  contextMenuPosition.value = { x: event.clientX, y: event.clientY }
  contextMenuTab.value = tab
  contextMenuVisible.value = true
}

const handleQuickMenuSelect = (menuItem: any) => {
  router.push(menuItem.path)
}

const handleRefresh = () => {
  // 刷新当前页签
  const currentTab = tabsStore.tabs.find(t => t.id === activeTab.value)
  if (currentTab) {
    tabsStore.refreshTab(currentTab.id)
  }
}

const handleCloseCurrent = () => {
  if (contextMenuTab.value) {
    tabsStore.removeTab(contextMenuTab.value.id)
  }
}

const handleCloseOthers = () => {
  if (contextMenuTab.value) {
    tabsStore.closeOthers(contextMenuTab.value.id)
  }
}

const handleCloseAll = () => {
  tabsStore.closeAll()
  router.push('/')
}
</script>

<style scoped lang="scss">
.tab-bar {
  height: 40px;
  background: var(--bg-base);
  border-bottom: 1px solid var(--border-light);
  padding: 4px 8px;
  display: flex;
  align-items: center;
  gap: 4px;
}

.tab-list {
  display: flex;
  align-items: center;
  gap: 4px;
  flex: 1;
  overflow-x: auto;
  overflow-y: hidden;
  scrollbar-width: none;
  -ms-overflow-style: none;
  
  &::-webkit-scrollbar {
    display: none;
  }
}

.tab-add {
  width: 32px;
  height: 32px;
  border-radius: var(--radius-md);
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  color: var(--text-secondary);
  background: transparent;
  border: none;
  transition: var(--transition-fast);
  
  &:hover {
    background: var(--bg-page);
    color: var(--color-primary);
  }
}
</style>
```

### 1.3 页签项组件 (TabItem.vue)

```vue
<template>
  <div
    class="tab-item"
    :class="{ active }"
    @click="emit('click')"
  >
    <el-icon v-if="tab.icon" class="tab-icon">
      <component :is="getIcon(tab.icon)" />
    </el-icon>
    <span class="tab-title">{{ $t(tab.title) }}</span>
    <el-icon
      v-if="tab.closable"
      class="tab-close"
      @click.stop="emit('close')"
    >
      <Close />
    </el-icon>
  </div>
</template>

<script setup lang="ts">
import { Close } from '@element-plus/icons-vue'
import type { TabItem } from '@/types/tabs'

interface Props {
  tab: TabItem
  active: boolean
}

defineProps<Props>()

const emit = defineEmits<{
  click: []
  close: []
}>()

const getIcon = (iconName: string) => {
  // 动态获取图标组件
  return iconName
}
</script>

<style scoped lang="scss">
.tab-item {
  height: 32px;
  padding: 0 16px;
  border-radius: var(--radius-md);
  display: flex;
  align-items: center;
  gap: 6px;
  cursor: pointer;
  user-select: none;
  transition: var(--transition-base);
  color: var(--text-regular);
  background: transparent;
  flex-shrink: 0;
  
  &:hover {
    background: var(--bg-page);
    color: var(--color-primary);
  }
  
  &.active {
    background: var(--color-primary);
    color: #FFFFFF;
    font-weight: var(--font-weight-medium);
    
    .tab-close {
      color: rgba(255, 255, 255, 0.8);
      
      &:hover {
        background: rgba(255, 255, 255, 0.2);
        color: #FFFFFF;
      }
    }
  }
}

.tab-icon {
  font-size: 14px;
}

.tab-title {
  font-size: 14px;
  white-space: nowrap;
}

.tab-close {
  width: 16px;
  height: 16px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  margin-left: 4px;
  font-size: 12px;
  opacity: 0;
  transition: var(--transition-fast);
  
  &:hover {
    background: var(--color-danger);
    color: #FFFFFF;
  }
}

.tab-item:hover .tab-close {
  opacity: 1;
}
</style>
```

### 1.4 Pinia Store (tabs.ts)

```typescript
import { defineStore } from 'pinia'
import { ref, computed } from 'vue'

export interface TabItem {
  id: string
  title: string
  path: string
  icon?: string
  closable: boolean
  cached?: boolean
}

export const useTabsStore = defineStore('tabs', () => {
  const tabs = ref<TabItem[]>([
    {
      id: 'home',
      title: 'menu.home',
      path: '/',
      icon: 'HomeFilled',
      closable: false
    }
  ])
  
  const activeTab = ref<string>('home')
  
  const cachedViews = computed(() => {
    return tabs.value.filter(tab => tab.cached).map(tab => tab.id)
  })

  const addTab = (tab: TabItem) => {
    const exists = tabs.value.find(t => t.id === tab.id)
    if (!exists) {
      tabs.value.push(tab)
    }
    activeTab.value = tab.id
  }

  const removeTab = (tabId: string) => {
    const index = tabs.value.findIndex(t => t.id === tabId)
    if (index === -1) return
    
    tabs.value.splice(index, 1)
    
    // 如果关闭的是当前页签，切换到左侧页签
    if (activeTab.value === tabId) {
      const newIndex = Math.max(0, index - 1)
      activeTab.value = tabs.value[newIndex]?.id || ''
    }
  }

  const setActiveTab = (tabId: string) => {
    activeTab.value = tabId
  }

  const refreshTab = (tabId: string) => {
    // 实现页签刷新逻辑
    const tab = tabs.value.find(t => t.id === tabId)
    if (tab) {
      tab.cached = false
      setTimeout(() => {
        tab.cached = true
      }, 0)
    }
  }

  const closeOthers = (keepTabId: string) => {
    const keepTab = tabs.value.find(t => t.id === keepTabId)
    const homeTab = tabs.value.find(t => t.id === 'home')
    
    tabs.value = homeTab ? [homeTab] : []
    if (keepTab && keepTab.id !== 'home') {
      tabs.value.push(keepTab)
    }
    
    activeTab.value = keepTabId
  }

  const closeAll = () => {
    const homeTab = tabs.value.find(t => t.id === 'home')
    tabs.value = homeTab ? [homeTab] : []
    activeTab.value = homeTab?.id || ''
  }

  // 从 localStorage 恢复
  const restoreTabs = () => {
    const saved = localStorage.getItem('tabs')
    if (saved) {
      try {
        const parsed = JSON.parse(saved)
        tabs.value = parsed.tabs || tabs.value
        activeTab.value = parsed.activeTab || activeTab.value
      } catch (e) {
        console.error('Failed to restore tabs:', e)
      }
    }
  }

  // 保存到 localStorage
  const saveTabs = () => {
    localStorage.setItem('tabs', JSON.stringify({
      tabs: tabs.value,
      activeTab: activeTab.value
    }))
  }

  return {
    tabs,
    activeTab,
    cachedViews,
    addTab,
    removeTab,
    setActiveTab,
    refreshTab,
    closeOthers,
    closeAll,
    restoreTabs,
    saveTabs
  }
})
```

---

## 2. 侧边栏组件实现

### 2.1 侧边栏主组件 (Sidebar/index.vue)

```vue
<template>
  <aside
    class="sidebar"
    :class="{ collapsed: sidebarStore.collapsed }"
    :style="{ width: sidebarWidth + 'px' }"
  >
    <!-- Logo 区域 -->
    <div class="sidebar-logo">
      <img src="/logo.svg" alt="Logo" class="logo-icon" />
      <span v-show="!sidebarStore.collapsed" class="logo-text">
        {{ $t('app.name') }}
      </span>
    </div>
    
    <!-- 菜单区域 -->
    <div class="sidebar-menu">
      <MenuItem
        v-for="menu in menus"
        :key="menu.id"
        :menu="menu"
        :collapsed="sidebarStore.collapsed"
      />
    </div>
    
    <!-- 展开/收起按钮 -->
    <div class="collapse-btn" @click="toggleCollapse">
      <el-icon :class="{ rotated: sidebarStore.collapsed }">
        <ArrowLeft />
      </el-icon>
      <span v-show="!sidebarStore.collapsed">{{ $t('sidebar.collapse') }}</span>
    </div>
    
    <!-- 悬浮面板 (收缩状态下) -->
    <FloatingPanel
      v-if="sidebarStore.collapsed && hoveredMenu"
      :menu="hoveredMenu"
      :position="floatingPosition"
    />
  </aside>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import { ArrowLeft } from '@element-plus/icons-vue'
import { useSidebarStore } from '@/stores/sidebar'
import { usePermissionStore } from '@/stores/permission'
import MenuItem from './MenuItem.vue'
import FloatingPanel from './FloatingPanel.vue'

const sidebarStore = useSidebarStore()
const permissionStore = usePermissionStore()

const sidebarWidth = computed(() => sidebarStore.collapsed ? 64 : 220)
const menus = computed(() => permissionStore.menus)

const hoveredMenu = ref(null)
const floatingPosition = ref({ x: 64, y: 0 })

const toggleCollapse = () => {
  sidebarStore.toggleCollapse()
}

// 监听菜单悬停
watch(() => sidebarStore.hoveredMenu, (menu) => {
  hoveredMenu.value = menu
  if (menu) {
    const menuElement = document.querySelector(`[data-menu-id="${menu.id}"]`)
    if (menuElement) {
      const rect = menuElement.getBoundingClientRect()
      floatingPosition.value = { x: 64, y: rect.top }
    }
  }
})
</script>

<style scoped lang="scss">
.sidebar {
  height: 100vh;
  background: var(--sidebar-bg);
  display: flex;
  flex-direction: column;
  transition: width var(--transition-slow);
  position: relative;
  flex-shrink: 0;
}

.sidebar-logo {
  height: 60px;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 12px;
  border-bottom: 1px solid rgba(255, 255, 255, 0.1);
  
  .logo-icon {
    width: 32px;
    height: 32px;
  }
  
  .logo-text {
    color: #FFFFFF;
    font-size: 18px;
    font-weight: var(--font-weight-bold);
    white-space: nowrap;
  }
}

.sidebar-menu {
  flex: 1;
  overflow-y: auto;
  overflow-x: hidden;
  padding: 8px 0;
}

.collapse-btn {
  height: 48px;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 8px;
  color: var(--sidebar-text);
  cursor: pointer;
  border-top: 1px solid rgba(255, 255, 255, 0.1);
  transition: var(--transition-fast);
  white-space: nowrap;
  
  &:hover {
    background: var(--sidebar-hover);
    color: #FFFFFF;
  }
  
  .el-icon {
    transition: transform var(--transition-base);
    
    &.rotated {
      transform: rotate(180deg);
    }
  }
}
</style>
```

### 2.2 菜单项组件 (MenuItem.vue)

```vue
<template>
  <div
    class="menu-item-wrapper"
    :data-menu-id="menu.id"
  >
    <div
      class="menu-item"
      :class="{
        active: isActive,
        collapsed: collapsed && !hasChildren
      }"
      @click="handleClick"
      @mouseenter="handleMouseEnter"
      @mouseleave="handleMouseLeave"
    >
      <!-- 激活指示器 -->
      <div v-if="isActive" class="active-indicator" />
      
      <!-- 图标 -->
      <div class="menu-icon">
        <el-icon>
          <component :is="getIcon(menu.icon)" />
        </el-icon>
      </div>
      
      <!-- 文字 -->
      <span v-show="!collapsed" class="menu-text">
        {{ $t(menu.title) }}
      </span>
      
      <!-- 展开箭头 -->
      <el-icon
        v-if="hasChildren && !collapsed"
        class="expand-arrow"
        :class="{ expanded: isOpen }"
      >
        <ArrowRight />
      </el-icon>
    </div>
    
    <!-- 子菜单 -->
    <Transition name="submenu">
      <div
        v-if="hasChildren && isOpen && !collapsed"
        class="submenu"
      >
        <div
          v-for="child in menu.children"
          :key="child.id"
          class="submenu-item"
          :class="{ active: isChildActive(child) }"
          @click="handleChildClick(child)"
        >
          {{ $t(child.title) }}
        </div>
      </div>
    </Transition>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { ArrowRight } from '@element-plus/icons-vue'
import { useSidebarStore } from '@/stores/sidebar'

interface MenuItemType {
  id: string
  title: string
  icon: string
  path?: string
  children?: MenuItemType[]
  hidden?: boolean
}

interface Props {
  menu: MenuItemType
  collapsed: boolean
}

const props = defineProps<Props>()
const route = useRoute()
const router = useRouter()
const sidebarStore = useSidebarStore()

const isOpen = ref(false)

const hasChildren = computed(() => {
  return props.menu.children && props.menu.children.length > 0
})

const isActive = computed(() => {
  return route.path === props.menu.path || 
         route.path.startsWith(props.menu.path + '/')
})

const isChildActive = (child: MenuItemType) => {
  return route.path === child.path
}

const handleClick = () => {
  if (hasChildren.value) {
    isOpen.value = !isOpen.value
  } else if (props.menu.path) {
    router.push(props.menu.path)
  }
}

const handleChildClick = (child: MenuItemType) => {
  if (child.path) {
    router.push(child.path)
  }
}

const handleMouseEnter = () => {
  if (props.collapsed) {
    sidebarStore.setHoveredMenu(props.menu)
  }
}

const handleMouseLeave = () => {
  if (props.collapsed) {
    sidebarStore.setHoveredMenu(null)
  }
}

const getIcon = (iconName: string) => iconName
</script>

<style scoped lang="scss">
.menu-item-wrapper {
  position: relative;
}

.menu-item {
  height: 48px;
  display: flex;
  align-items: center;
  padding: 0 16px;
  color: var(--sidebar-text);
  cursor: pointer;
  position: relative;
  transition: var(--transition-fast);
  
  &:hover {
    background: var(--sidebar-hover);
    color: #FFFFFF;
  }
  
  &.active {
    color: var(--sidebar-active);
  }
  
  &.collapsed {
    justify-content: center;
    padding: 0;
  }
}

.active-indicator {
  position: absolute;
  left: 0;
  top: 50%;
  transform: translateY(-50%);
  width: 3px;
  height: 24px;
  background: var(--sidebar-active);
  border-radius: 0 3px 3px 0;
}

.menu-icon {
  width: 24px;
  height: 24px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 18px;
  flex-shrink: 0;
}

.menu-text {
  margin-left: 12px;
  flex: 1;
  white-space: nowrap;
  overflow: hidden;
  font-size: 14px;
}

.expand-arrow {
  font-size: 12px;
  transition: transform var(--transition-base);
  
  &.expanded {
    transform: rotate(90deg);
  }
}

.submenu {
  background: var(--submenu-bg);
  overflow: hidden;
}

.submenu-item {
  height: 40px;
  padding-left: 52px;
  display: flex;
  align-items: center;
  color: var(--sidebar-text);
  cursor: pointer;
  font-size: 13px;
  transition: var(--transition-fast);
  
  &:hover {
    color: #FFFFFF;
  }
  
  &.active {
    color: var(--sidebar-active);
  }
}

// 子菜单动画
.submenu-enter-active,
.submenu-leave-active {
  transition: height var(--transition-base), opacity var(--transition-base);
}

.submenu-enter-from,
.submenu-leave-to {
  height: 0;
  opacity: 0;
}
</style>
```

### 2.3 悬浮面板组件 (FloatingPanel.vue)

```vue
<template>
  <Transition name="floating">
    <div
      class="floating-panel"
      :style="panelStyle"
      v-if="menu"
    >
      <div class="panel-header">
        <el-icon class="panel-icon">
          <component :is="getIcon(menu.icon)" />
        </el-icon>
        <span>{{ $t(menu.title) }}</span>
      </div>
      
      <div v-if="hasChildren" class="panel-content">
        <div
          v-for="child in menu.children"
          :key="child.id"
          class="panel-item"
          :class="{ active: isActive(child) }"
          @click="handleClick(child)"
        >
          {{ $t(child.title) }}
        </div>
      </div>
      
      <div v-else class="panel-content">
        <div class="panel-item" @click="handleClick(menu)">
          {{ $t(menu.title) }}
        </div>
      </div>
    </div>
  </Transition>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { useRoute, useRouter } from 'vue-router'

interface MenuItemType {
  id: string
  title: string
  icon: string
  path?: string
  children?: MenuItemType[]
}

interface Props {
  menu: MenuItemType | null
  position: { x: number; y: number }
}

const props = defineProps<Props>()
const route = useRoute()
const router = useRouter()

const panelStyle = computed(() => ({
  left: props.position.x + 'px',
  top: props.position.y + 'px'
}))

const hasChildren = computed(() => {
  return props.menu?.children && props.menu.children.length > 0
})

const isActive = (item: MenuItemType) => {
  return route.path === item.path
}

const handleClick = (item: MenuItemType) => {
  if (item.path) {
    router.push(item.path)
  }
}

const getIcon = (iconName: string) => iconName
</script>

<style scoped lang="scss">
.floating-panel {
  position: fixed;
  left: 64px;
  min-width: 180px;
  background: var(--sidebar-bg);
  border-radius: var(--radius-md);
  box-shadow: var(--shadow-sidebar);
  z-index: var(--z-dropdown);
  overflow: hidden;
}

.panel-header {
  height: 48px;
  display: flex;
  align-items: center;
  gap: 12px;
  padding: 0 16px;
  color: #FFFFFF;
  font-weight: var(--font-weight-medium);
  border-bottom: 1px solid rgba(255, 255, 255, 0.1);
}

.panel-icon {
  font-size: 18px;
}

.panel-content {
  padding: 8px 0;
}

.panel-item {
  height: 36px;
  padding: 0 16px;
  display: flex;
  align-items: center;
  color: var(--sidebar-text);
  font-size: 13px;
  cursor: pointer;
  transition: var(--transition-fast);
  
  &:hover {
    background: var(--sidebar-hover);
    color: #FFFFFF;
  }
  
  &.active {
    color: var(--sidebar-active);
  }
}

// 悬浮面板动画
.floating-enter-active,
.floating-leave-active {
  transition: opacity var(--transition-fast), transform var(--transition-fast);
}

.floating-enter-from,
.floating-leave-to {
  opacity: 0;
  transform: translateX(-10px);
}
</style>
```

### 2.4 侧边栏 Store (sidebar.ts)

```typescript
import { defineStore } from 'pinia'
import { ref, computed } from 'vue'

export const useSidebarStore = defineStore('sidebar', () => {
  const collapsed = ref(false)
  const hoveredMenu = ref<any>(null)
  const activeMenu = ref('')
  const openMenus = ref<string[]>([])

  const sidebarWidth = computed(() => collapsed.value ? 64 : 220)

  const toggleCollapse = () => {
    collapsed.value = !collapsed.value
    // 保存状态
    localStorage.setItem('sidebarCollapsed', String(collapsed.value))
  }

  const setCollapsed = (value: boolean) => {
    collapsed.value = value
    localStorage.setItem('sidebarCollapsed', String(value))
  }

  const setHoveredMenu = (menu: any) => {
    hoveredMenu.value = menu
  }

  const setActiveMenu = (menuId: string) => {
    activeMenu.value = menuId
  }

  const toggleMenu = (menuId: string) => {
    const index = openMenus.value.indexOf(menuId)
    if (index > -1) {
      openMenus.value.splice(index, 1)
    } else {
      openMenus.value.push(menuId)
    }
  }

  // 恢复状态
  const restoreState = () => {
    const saved = localStorage.getItem('sidebarCollapsed')
    if (saved !== null) {
      collapsed.value = saved === 'true'
    }
  }

  return {
    collapsed,
    hoveredMenu,
    activeMenu,
    openMenus,
    sidebarWidth,
    toggleCollapse,
    setCollapsed,
    setHoveredMenu,
    setActiveMenu,
    toggleMenu,
    restoreState
  }
})
```

---

## 3. 布局整合

### 3.1 主布局组件 (Layout/index.vue)

```vue
<template>
  <div class="layout">
    <Sidebar />
    <div class="main-container">
      <AppHeader />
      <TabBar />
      <div class="content-wrapper">
        <router-view v-slot="{ Component }">
          <KeepAlive :include="cachedViews">
            <component :is="Component" />
          </KeepAlive>
        </router-view>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { useTabsStore } from '@/stores/tabs'
import Sidebar from '@/components/layout/Sidebar/index.vue'
import AppHeader from '@/components/layout/AppHeader/index.vue'
import TabBar from '@/components/layout/TabBar/index.vue'

const tabsStore = useTabsStore()
const cachedViews = computed(() => tabsStore.cachedViews)
</script>

<style scoped lang="scss">
.layout {
  display: flex;
  height: 100vh;
}

.main-container {
  flex: 1;
  display: flex;
  flex-direction: column;
  overflow: hidden;
}

.content-wrapper {
  flex: 1;
  overflow: auto;
  background: var(--bg-page);
  padding: 16px;
}
</style>
```

---

## 4. 类型定义

### 4.1 页签类型 (types/tabs.ts)

```typescript
export interface TabItem {
  id: string
  title: string
  path: string
  icon?: string
  closable: boolean
  cached?: boolean
  query?: Record<string, any>
  params?: Record<string, any>
}

export interface TabState {
  tabs: TabItem[]
  activeTab: string
}
```

### 4.2 菜单类型 (types/menu.ts)

```typescript
export interface MenuItem {
  id: string
  title: string
  icon: string
  path?: string
  component?: string
  children?: MenuItem[]
  hidden?: boolean
  permissions?: string[]
}
```

---

## 5. 最佳实践

### 5.1 性能优化

```typescript
// 1. 使用虚拟滚动处理大量页签
import { useVirtualList } from '@vueuse/core'

const { list: virtualTabs } = useVirtualList(tabs, {
  itemHeight: 40,
  overscan: 5
})

// 2. 组件懒加载
const QuickMenu = defineAsyncComponent(() => 
  import('./QuickMenu.vue')
)

// 3. 防抖处理
import { debounce } from 'lodash-es'

const handleTabScroll = debounce((e: Event) => {
  // 处理滚动
}, 16)
```

### 5.2 可访问性

```vue
<!-- 键盘导航 -->
<template>
  <div
    class="tab-item"
    role="tab"
    :aria-selected="active"
    :tabindex="active ? 0 : -1"
    @keydown="handleKeydown"
  >
    <span class="tab-title">{{ title }}</span>
  </div>
</template>

<script setup>
const handleKeydown = (e: KeyboardEvent) => {
  switch (e.key) {
    case 'ArrowLeft':
      // 切换到上一个页签
      break
    case 'ArrowRight':
      // 切换到下一个页签
      break
    case 'Delete':
      // 关闭当前页签
      break
  }
}
</script>
```

### 5.3 错误处理

```typescript
// 路由导航守卫
router.beforeEach((to, from, next) => {
  try {
    // 检查权限
    if (!hasPermission(to)) {
      next('/403')
      return
    }
    next()
  } catch (error) {
    console.error('Navigation error:', error)
    next('/error')
  }
})
```

---

## 6. i18n 配置示例

```typescript
// locales/zh-CN/menu.ts
export default {
  menu: {
    home: '首页',
    product: '商品管理',
    productList: '商品列表',
    category: '分类管理',
    brand: '品牌管理',
    order: '订单管理',
    user: '用户管理',
    settings: '系统设置'
  },
  sidebar: {
    collapse: '收起菜单',
    expand: '展开菜单'
  }
}

// locales/en-US/menu.ts
export default {
  menu: {
    home: 'Home',
    product: 'Product',
    productList: 'Product List',
    category: 'Category',
    brand: 'Brand',
    order: 'Order',
    user: 'User',
    settings: 'Settings'
  },
  sidebar: {
    collapse: 'Collapse',
    expand: 'Expand'
  }
}
```

---

*文档版本: 1.0*
*更新日期: 2026-09-01*
