<template>
  <el-select
    :model-value="modelValue"
    :placeholder="t('mall.address.form.memberPlaceholder')"
    :disabled="disabled"
    clearable
    filterable
    remote
    :remote-method="handleSearch"
    :loading="loading"
    @update:model-value="emit('update:modelValue', $event)"
  >
    <el-option
      v-for="item in options"
      :key="item.id"
      :label="`${item.nickname} (${item.phone})`"
      :value="item.id"
    />
  </el-select>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useI18n } from 'vue-i18n'
import { getMemberList } from '@/api/mall/member'
import type { Member } from '@/types/mall'

const props = defineProps<{
  modelValue: string
  disabled?: boolean
}>()

const emit = defineEmits<{
  (e: 'update:modelValue', value: string): void
}>()

// Use props to avoid eslint error
const { modelValue, disabled } = props
void modelValue
void disabled

const { t } = useI18n()

const loading = ref(false)
const options = ref<Member[]>([])

async function fetchMembers(keyword?: string) {
  loading.value = true
  try {
    const result = await getMemberList({
      pageIndex: 1,
      pageSize: 20,
      nickname: keyword || undefined,
      phone: keyword || undefined,
    })
    options.value = result.list || []
  } finally {
    loading.value = false
  }
}

function handleSearch(query: string) {
  if (query) {
    fetchMembers(query)
  }
}

onMounted(() => {
  fetchMembers()
})
</script>
