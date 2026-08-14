<!-- src/components/common/DeptSelect.vue -->
<template>
  <el-tree-select
    v-model="selectedValue"
    :data="treeData"
    :props="treeSelectProps"
    :placeholder="placeholder ? t(placeholder) : t('common.selectPlaceholder')"
    :clearable="clearable"
    :disabled="disabled"
    check-strictly
    style="width: 100%"
    @change="handleChange"
  />
</template>

<script setup lang="ts">
import { ref, computed, watch, onMounted } from 'vue'
import { useI18n } from 'vue-i18n'
import { getDeptTree } from '@/api/basic/dept'
import type { Dept } from '@/types/basic'

interface Props {
  modelValue?: string // 部门 ID
  placeholder?: string // 占位符（i18n key）
  clearable?: boolean
  disabled?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  modelValue: undefined,
  placeholder: undefined,
  clearable: true,
  disabled: false
})

const emit = defineEmits<{
  (e: 'update:modelValue', value: string | undefined): void
  (e: 'change', value: string | undefined): void
}>()

const { t } = useI18n()

// 树数据
const deptTree = ref<Dept[]>([])

// 选中的值
const selectedValue = ref<string | undefined>(props.modelValue)

// 树选择器配置
const treeSelectProps = {
  children: 'children',
  label: 'name',
  value: 'id'
}

// 为 TreeSelect 准备的数据
const treeData = computed(() => {
  return deptTree.value
})

// 加载部门树
const loadDeptTree = async (): Promise<void> => {
  try {
    const data = await getDeptTree()
    deptTree.value = data
  } catch (error) {
    // 错误已在拦截器处理
  }
}

// 处理选择变化
const handleChange = (value: string | undefined): void => {
  emit('update:modelValue', value)
  emit('change', value)
}

// 监听外部值变化
watch(
  () => props.modelValue,
  (val) => {
    selectedValue.value = val
  }
)

// 页面加载时获取部门树
onMounted(() => {
  loadDeptTree()
})
</script>