<template>
  <el-tag
    :type="tagType"
    :size="size"
    :effect="effect"
  >
    {{ label }}
  </el-tag>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { useI18n } from 'vue-i18n'

type TagType = '' | 'success' | 'warning' | 'info' | 'danger'

export interface StatusOption {
  label: string
  type: TagType
}

interface Props {
  value: string
  options: Record<string, StatusOption>
  size?: 'large' | 'default' | 'small'
  effect?: 'dark' | 'light' | 'plain'
}

const props = withDefaults(defineProps<Props>(), {
  size: 'small',
  effect: 'light',
})

const { t } = useI18n()

const matched = computed<StatusOption | undefined>(() => props.options[props.value])

const tagType = computed<TagType>(() => matched.value?.type ?? 'info')

const label = computed<string>(() => {
  const opt = matched.value
  if (!opt) return props.value
  return t(opt.label)
})
</script>
