<!-- src/views/basic/dict/components/DictDataFormDialog.vue -->
<template>
  <BaseFormDialog
    :visible="modelValue"
    :title="isEdit ? t('basic.dict.data.form.editTitle') : t('basic.dict.data.form.addTitle')"
    :model="formData"
    :rules="formRules"
    :loading="saving"
    width="500px"
    @update:visible="emit('update:modelValue', $event)"
    @close="handleClose"
    @submit="handleSave"
  >
    <!-- 字典标签 -->
    <el-form-item
      :label="t('basic.dict.data.form.label')"
      prop="labelKey"
    >
      <el-input
        v-model="formData.labelKey"
        maxlength="100"
        show-word-limit
        :placeholder="t('basic.dict.data.form.labelPlaceholder')"
      />
    </el-form-item>

    <!-- 字典值 -->
    <el-form-item
      :label="t('basic.dict.data.form.value')"
      prop="value"
    >
      <el-input
        v-model="formData.value"
        maxlength="50"
        show-word-limit
        :placeholder="t('basic.dict.data.form.valuePlaceholder')"
        :disabled="isEdit"
      />
    </el-form-item>

    <!-- 排序 -->
    <el-form-item
      :label="t('basic.dict.data.form.sort')"
      prop="sort"
    >
      <el-input-number
        v-model="formData.sort"
        :min="0"
        :max="999"
      />
    </el-form-item>

    <!-- 状态 -->
    <el-form-item
      :label="t('basic.dict.data.form.status')"
      prop="status"
    >
      <el-radio-group v-model="formData.status">
        <el-radio :value="1">
          {{ t('common.status.enabled') }}
        </el-radio>
        <el-radio :value="0">
          {{ t('common.status.disabled') }}
        </el-radio>
      </el-radio-group>
    </el-form-item>
  </BaseFormDialog>
</template>

<script setup lang="ts">
import { ref, reactive, computed, watch } from 'vue'
import { ElMessage } from 'element-plus'
import type { FormRules } from 'element-plus'
import { useLocale } from '@/composables/useLocale'
import { getDictDataList, createDictData, updateDictData } from '@/api/basic/dict'
import type { DictDataCreateParams } from '@/types/basic'
import BaseFormDialog from '@/components/common/BaseFormDialog.vue'

const props = defineProps<{
  modelValue: boolean
  dataId?: string
  typeCode: string // 字典类型编码
}>()

const emit = defineEmits<{
  (e: 'update:modelValue', value: boolean): void
  (e: 'success'): void
}>()

const { t } = useLocale()

const saving = ref(false)

// 是否是编辑模式
const isEdit = computed(() => !!props.dataId)

// 表单数据
const formData = reactive<DictDataCreateParams>({
  typeCode: '',
  value: '',
  labelKey: '',
  sort: 0,
  status: 1
})

// 表单验证规则
const formRules: FormRules = {
  labelKey: [
    { required: true, message: () => t('basic.dict.data.form.labelRequired'), trigger: 'blur' },
    { min: 2, max: 100, message: () => t('basic.dict.data.form.labelLength'), trigger: 'blur' }
  ],
  value: [
    { required: true, message: () => t('basic.dict.data.form.valueRequired'), trigger: 'blur' },
    { pattern: /^[a-zA-Z0-9_-]+$/, message: () => t('basic.dict.data.form.valueFormat'), trigger: 'blur' }
  ]
}

// 监听弹窗打开
watch(
  () => props.modelValue,
  async (val) => {
    if (val) {
      if (props.dataId) {
        // 编辑模式：加载字典数据详情
        await loadDetail(props.dataId)
      } else {
        // 新增模式：重置表单
        resetForm()
        formData.typeCode = props.typeCode
      }
    }
  }
)

// 加载字典数据详情
const loadDetail = async (id: string) => {
  try {
    // 由于没有单个字典数据详情接口，从列表中查找
    const result = await getDictDataList({ typeCode: props.typeCode, pageIndex: 1, pageSize: 1000 })
    const data = result.list.find(item => item.id === id)
    if (data) {
      formData.typeCode = data.typeCode
      formData.value = data.value
      formData.labelKey = data.labelKey
      formData.sort = data.sort
      formData.status = data.status
    }
  } catch {
    ElMessage.error(t('common.error.request'))
  }
}

// 重置表单
const resetForm = () => {
  formData.typeCode = ''
  formData.value = ''
  formData.labelKey = ''
  formData.sort = 0
  formData.status = 1
}

// 关闭弹窗
const handleClose = () => {
  resetForm()
}

// 保存（验证由 BaseFormDialog 处理）
const handleSave = async () => {
  saving.value = true
  try {
    if (isEdit.value) {
      await updateDictData(props.dataId!, {
        labelKey: formData.labelKey,
        sort: formData.sort,
        status: formData.status
      })
      ElMessage.success(t('basic.dict.data.message.updateSuccess'))
    } else {
      await createDictData(formData)
      ElMessage.success(t('basic.dict.data.message.addSuccess'))
    }
    emit('update:modelValue', false)
    emit('success')
  } catch {
    ElMessage.error(t('common.error.request'))
  } finally {
    saving.value = false
  }
}
</script>
