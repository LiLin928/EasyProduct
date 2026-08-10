// src/composables/useDict.ts
import { ref } from 'vue'
import type { Ref } from 'vue'
import { useI18n } from 'vue-i18n'
import { getDictData } from '@/api/basic/dict'
import type { DictItem } from '@/types/basic'

export interface DictOption {
  value: string
  label: string
}

export interface UseDictReturn {
  options: Ref<DictOption[]>
  getLabel: (value: string) => string
  loading: Ref<boolean>
}

/**
 * 字典数据 composable（签名对齐前端规范 2.4）
 * @param typeCode 字典类型编码
 */
export function useDict(typeCode: string): UseDictReturn {
  const { t } = useI18n()
  const options = ref<DictOption[]>([])
  const loading = ref(false)
  const dictMap = ref<Map<string, DictItem>>(new Map())

  const fetchDict = async (): Promise<void> => {
    loading.value = true
    try {
      const items = await getDictData(typeCode)
      dictMap.value = new Map(items.map((item) => [item.value, item]))
      options.value = items.map((item) => ({
        value: item.value,
        label: t(item.labelKey),
      }))
    } finally {
      loading.value = false
    }
  }

  const getLabel = (value: string): string => {
    const item = dictMap.value.get(value)
    return item ? t(item.labelKey) : value
  }

  // 立即加载字典数据
  fetchDict()

  return {
    options,
    getLabel,
    loading,
  }
}