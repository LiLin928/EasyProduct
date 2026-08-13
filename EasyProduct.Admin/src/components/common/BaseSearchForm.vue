<template>
  <el-card class="base-search-form">
    <!-- 工具栏插槽 -->
    <div v-if="$slots.toolbar" class="base-search-form__toolbar">
      <slot name="toolbar" />
    </div>

    <el-form
      :model="localModel"
      :inline="true"
      :label-width="labelWidth"
    >
      <!-- 动态渲染字段 -->
      <el-form-item
        v-for="field in fields"
        :key="field.prop"
        :label="t(field.label)"
      >
        <!-- Input 类型 -->
        <el-input
          v-if="field.type === 'input'"
          v-model="localModel[field.prop]"
          :placeholder="t(field.placeholder || 'common.inputPlaceholder')"
          :clearable="field.clearable !== false"
          @keyup.enter="handleSearch"
          @clear="handleSearch"
        />

        <!-- Select 类型 -->
        <el-select
          v-else-if="field.type === 'select'"
          v-model="localModel[field.prop]"
          :placeholder="t(field.placeholder || 'common.selectPlaceholder')"
          :clearable="field.clearable !== false"
          @change="handleSearch"
        >
          <el-option
            v-for="option in field.options"
            :key="option.value"
            :label="t(option.label)"
            :value="option.value"
          />
        </el-select>

        <!-- DateRange 类型 -->
        <el-date-picker
          v-else-if="field.type === 'dateRange'"
          v-model="localModel[field.prop]"
          type="daterange"
          :start-placeholder="t(field.startPlaceholder || 'common.startDate')"
          :end-placeholder="t(field.endPlaceholder || 'common.endDate')"
          value-format="YYYY-MM-DD"
          @change="handleSearch"
        />
      </el-form-item>

      <!-- 操作按钮（右对齐） -->
      <el-form-item v-if="showButtons" class="base-search-form__actions">
        <el-button
          type="primary"
          :loading="loading"
          @click="handleSearch"
        >
          {{ t(searchButtonText) }}
        </el-button>
        <el-button @click="handleReset">
          {{ t(resetButtonText) }}
        </el-button>
      </el-form-item>
    </el-form>
  </el-card>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { useI18n } from 'vue-i18n'
import type { SearchField } from '@/types/search'

interface Props {
  fields: SearchField[]
  model: Record<string, unknown>
  loading?: boolean
  labelWidth?: string | number
  showButtons?: boolean
  searchButtonText?: string
  resetButtonText?: string
}

const props = withDefaults(defineProps<Props>(), {
  loading: false,
  labelWidth: 'auto',
  showButtons: true,
  searchButtonText: 'common.search',
  resetButtonText: 'common.reset'
})

const emit = defineEmits<{
  'update:model': [value: Record<string, unknown>]
  search: []
  reset: []
}>()

const { t } = useI18n()

// 本地模型（用于双向绑定）
const localModel = computed({
  get: () => props.model,
  set: (value) => emit('update:model', value)
})

// 搜索方法
const handleSearch = (): void => {
  emit('search')
}

// 重置方法
const handleReset = (): void => {
  emit('reset')
}
</script>

<style scoped lang="scss">
.base-search-form {
  margin-bottom: $spacing-md;

  &__toolbar {
    margin-bottom: $spacing-md;
    padding-bottom: $spacing-md;
    border-bottom: 1px solid #ebeef5;
  }

  &__actions {
    margin-left: auto; // 推到右侧
  }

  // 下拉框最小宽度
  .el-select {
    min-width: 200px;
  }

  // 日期选择器最小宽度
  .el-date-editor--daterange {
    min-width: 240px;
  }
}
</style>