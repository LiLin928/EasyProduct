<template>
  <el-dialog
    :model-value="visible"
    title="选择图标"
    width="800px"
    @update:model-value="handleClose"
  >
    <div class="icon-select">
      <el-input
        v-model="searchKeyword"
        placeholder="搜索图标"
        clearable
        class="icon-select__search"
      >
        <template #prefix>
          <el-icon><Search /></el-icon>
        </template>
      </el-input>
      <el-scrollbar
        height="400px"
        class="icon-select__list"
      >
        <div class="icon-grid">
          <div
            v-for="icon in filteredIcons"
            :key="icon"
            class="icon-item"
            :class="{ 'is-selected': selectedIcon === icon }"
            @click="handleSelect(icon)"
          >
            <el-icon :size="24">
              <component :is="icon" />
            </el-icon>
            <div class="icon-name">
              {{ icon }}
            </div>
          </div>
        </div>
      </el-scrollbar>
    </div>
    <template #footer>
      <el-button @click="handleClose">
        {{ t('common.cancel') }}
      </el-button>
      <el-button
        type="primary"
        @click="handleConfirm"
      >
        {{ t('common.confirm') }}
      </el-button>
    </template>
  </el-dialog>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import { useI18n } from 'vue-i18n'
import { Search } from '@element-plus/icons-vue'

// 导入所有 Element Plus 图标
import * as Icons from '@element-plus/icons-vue'

interface Props {
  visible: boolean
  modelValue?: string
}

const props = defineProps<Props>()
const emit = defineEmits<{
  'update:visible': [value: boolean]
  'update:modelValue': [value: string]
}>()

const { t } = useI18n()

const searchKeyword = ref('')
const selectedIcon = ref(props.modelValue || '')

// 所有图标列表
const allIcons = Object.keys(Icons)

// 过滤后的图标列表
const filteredIcons = computed(() => {
  if (!searchKeyword.value) {
    return allIcons
  }
  return allIcons.filter((icon) =>
    icon.toLowerCase().includes(searchKeyword.value.toLowerCase())
  )
})

// 选择图标
const handleSelect = (icon: string): void => {
  selectedIcon.value = icon
}

// 确认选择
const handleConfirm = (): void => {
  emit('update:modelValue', selectedIcon.value)
  handleClose()
}

// 关闭弹窗
const handleClose = (): void => {
  emit('update:visible', false)
}
</script>

<style scoped lang="scss">
.icon-select {
  &__search {
    margin-bottom: 16px;
  }

  &__list {
    border: 1px solid var(--el-border-color);
    border-radius: 4px;
  }
}

.icon-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(100px, 1fr));
  gap: 12px;
  padding: 12px;
}

.icon-item {
  display: flex;
  flex-direction: column;
  align-items: center;
  padding: 12px;
  border: 1px solid var(--el-border-color);
  border-radius: 4px;
  cursor: pointer;
  transition: all 0.3s;

  &:hover {
    border-color: var(--el-color-primary);
    background-color: var(--el-color-primary-light-9);
  }

  &.is-selected {
    border-color: var(--el-color-primary);
    background-color: var(--el-color-primary-light-8);
  }
}

.icon-name {
  margin-top: 8px;
  font-size: 12px;
  color: var(--el-text-color-secondary);
  word-break: break-all;
  text-align: center;
}
</style>