<template>
  <div class="tags-view">
    <div class="tags-view__list">
      <div
        v-for="tag in visitedViews"
        :key="tag.path"
        :class="['tags-view__item', { 'is-active': isActive(tag) }]"
        @click.middle="closeSelectedTag(tag)"
        @click="handleTagClick(tag)"
      >
        <span
          v-if="isActive(tag)"
          class="tags-view__dot"
        />
        <span class="tags-view__title">{{ t(tag.title) }}</span>
        <el-icon
          v-if="!tag.affix"
          class="tags-view__close"
          @click.prevent.stop="closeSelectedTag(tag)"
        >
          <Close />
        </el-icon>
      </div>
    </div>
    <el-dropdown
      class="tags-view__dropdown"
      @command="handleCommand"
    >
      <el-button
        text
        class="tags-view__menu-btn"
      >
        <el-icon><ArrowDown /></el-icon>
      </el-button>
      <template #dropdown>
        <el-dropdown-menu>
          <el-dropdown-item command="closeOthers">
            {{ t('common.tagsView.closeOthers') }}
          </el-dropdown-item>
          <el-dropdown-item command="closeAll">
            {{ t('common.tagsView.closeAll') }}
          </el-dropdown-item>
        </el-dropdown-menu>
      </template>
    </el-dropdown>
  </div>
</template>

<script setup lang="ts">
import { ref, watch, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { Close, ArrowDown } from '@element-plus/icons-vue'
import { useI18n } from 'vue-i18n'

interface TagView {
  name: string
  path: string
  title: string
  affix?: boolean
}

const { t } = useI18n()
const route = useRoute()
const router = useRouter()
const visitedViews = ref<TagView[]>([
  { name: 'desktop', path: '/desktop', title: 'menu.desktop', affix: true }
])

const isActive = (tag: TagView) => {
  return tag.path === route.path
}

const addView = () => {
  const { name, path, meta } = route
  if (name && path) {
    const exists = visitedViews.value.some(v => v.path === path)
    if (!exists) {
      visitedViews.value.push({
        name: name as string,
        path,
        title: (meta?.title as string) || name as string
      })
    }
  }
}

const handleTagClick = (tag: TagView) => {
  router.push(tag.path)
}

const closeSelectedTag = (tag: TagView) => {
  const index = visitedViews.value.findIndex(v => v.path === tag.path)
  if (index > -1) {
    visitedViews.value.splice(index, 1)
    if (isActive(tag)) {
      const latestView = visitedViews.value.slice(-1)[0]
      if (latestView) {
        router.push(latestView.path)
      } else {
        router.push('/')
      }
    }
  }
}

const handleCommand = (command: string) => {
  switch (command) {
    case 'closeOthers':
      visitedViews.value = visitedViews.value.filter(
        v => v.affix || v.path === route.path
      )
      break
    case 'closeAll':
      visitedViews.value = visitedViews.value.filter(v => v.affix)
      if (!visitedViews.value.some(v => v.path === route.path)) {
        router.push('/')
      }
      break
  }
}

onMounted(() => {
  addView()
})

watch(
  () => route.path,
  () => {
    addView()
  }
)
</script>

<style scoped lang="scss">
.tags-view {
  display: flex;
  align-items: center;
  height: 48px;
  padding: 0 12px;
  background:white;
  border-bottom: 1px solid var(--ep-border-light);

  &__list {
    flex: 1;
    display: flex;
    gap: 8px;
    overflow: hidden;
    padding: 4px 0;
  }

  &__item {
    display: flex;
    align-items: center;
    gap: 6px;
    height: 32px;
    padding: 0 12px;
    border-radius: 16px;
    cursor: pointer;
    transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
    background: var(--ep-fill-color-light);
    border: 1px solid var(--ep-border-light);
    user-select: none;
    position: relative;

    &:not(.is-active) {
      background: var(--ep-fill-color-light);
    }

    &:hover {
      transform: translateY(-1px);
      box-shadow: 0 2px 8px rgba(0, 0, 0, 0.08);
      border-color: var(--ep-border);
      background: var(--ep-bg-card);
    }

    &.is-active {
      background: var(--ep-primary);
      border-color: var(--ep-primary);
      color: var(--ep-color-white);
      box-shadow: 0 2px 8px rgba(var(--ep-primary-rgb), 0.3);

      .tags-view__close {
        color: var(--ep-color-white);
      }
    }
  }

  &__dot {
    width: 6px;
    height: 6px;
    border-radius: 50%;
    background: var(--ep-color-white);
    flex-shrink: 0;
  }

  &__title {
    font-size: 13px;
    font-weight: 500;
    white-space: nowrap;
  }

  &__close {
    width: 18px;
    height: 18px;
    display: flex;
    align-items: center;
    justify-content: center;
    border-radius: 50%;
    font-size: 12px;
    color: var(--ep-text-secondary);
    transition: all 0.2s;
    flex-shrink: 0;

    &:hover {
      background: rgba(var(--ep-danger-rgb), 0.1);
      color: var(--ep-danger);
    }
  }

  &__dropdown {
    margin-left: 8px;
  }

  &__menu-btn {
    width: 32px;
    height: 32px;
    border-radius: 8px;
    color: var(--ep-text-secondary);
    transition: all 0.2s;

    &:hover {
      background: var(--ep-bg-hover);
      color: var(--ep-primary);
    }
  }
}
</style>
