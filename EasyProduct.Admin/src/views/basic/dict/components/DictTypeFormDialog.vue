<!-- src/views/basic/dict/components/DictTypeFormDialog.vue -->
<template>
  <BaseFormDialog
    :visible="modelValue"
    :title="isEdit ? t('basic.dict.type.form.editTitle') : t('basic.dict.type.form.addTitle')"
    :model="formData"
    :rules="formRules"
    :loading="saving"
    width="500px"
    @update:visible="emit('update:modelValue', $event)"
    @close="handleClose"
    @submit="handleSave"
  >
    <!-- 类型名称 -->
    <el-form-item
      :label="t('basic.dict.type.form.name')"
      prop="name"
    >
      <el-input
        v-model="formData.name"
        maxlength="100"
        show-word-limit
        :placeholder="t('basic.dict.type.form.namePlaceholder')"
      />
    </el-form-item>

    <!-- 类型编码 -->
    <el-form-item
      :label="t('basic.dict.type.form.code')"
      prop="code"
    >
      <el-input
        v-model="formData.code"
        maxlength="50"
        show-word-limit
        :placeholder="t('basic.dict.type.form.codePlaceholder')"
        :disabled="isEdit"
      />
    </el-form-item>

    <!-- 状态 -->
    <el-form-item
      :label="t('basic.dict.type.form.status')"
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

    <!-- 备注 -->
    <el-form-item
      :label="t('basic.dict.type.form.remark')"
      prop="remark"
    >
      <el-input
        v-model="formData.remark"
        type="textarea"
        :rows="3"
        maxlength="500"
        show-word-limit
        :placeholder="t('basic.dict.type.form.remarkPlaceholder')"
      />
    </el-form-item>
  </BaseFormDialog>
</template>

<script setup lang="ts">
import { ref, reactive, computed, watch } from 'vue'
import { ElMessage } from 'element-plus'
import type { FormRules } from 'element-plus'
import { useLocale } from '@/composables/useLocale'
import { getDictTypeList, createDictType, updateDictType } from '@/api/basic/dict'
import type { DictTypeCreateParams } from '@/types/basic'
import BaseFormDialog from '@/components/common/BaseFormDialog.vue'

const props = defineProps<{
  modelValue: boolean
  typeId?: string
}>()

const emit = defineEmits<{
  (e: 'update:modelValue', value: boolean): void
  (e: 'success'): void
}>()

const { t } = useLocale()

const saving = ref(false)

// 是否是编辑模式
const isEdit = computed(() => !!props.typeId)

// 表单数据
const formData = reactive<DictTypeCreateParams>({
  name: '',
  code: '',
  status: 1,
  remark: ''
})

// 表单验证规则
const formRules: FormRules = {
  name: [
    { required: true, message: () => t('basic.dict.type.form.nameRequired'), trigger: 'blur' },
    { min: 2, max: 100, message: () => t('basic.dict.type.form.nameLength'), trigger: 'blur' }
  ],
  code: [
    { required: true, message: () => t('basic.dict.type.form.codeRequired'), trigger: 'blur' },
    { pattern: /^[a-zA-Z][a-zA-Z0-9_-]*$/, message: () => t('basic.dict.type.form.codeFormat'), trigger: 'blur' }
  ]
}

// 监听弹窗打开
watch(
  () => props.modelValue,
  async (val) => {
    if (val) {
      if (props.typeId) {
        // 编辑模式：加载字典类型详情
        await loadDetail(props.typeId)
      } else {
        // 新增模式：重置表单
        resetForm()
      }
    }
  }
)

// 加载字典类型详情
const loadDetail = async (id: string) => {
  try {
    // 由于没有单个字典类型详情接口，从列表中查找
    const result = await getDictTypeList({ pageIndex: 1, pageSize: 1000 })
    const type = result.list.find(item => item.id === id)
    if (type) {
      formData.name = type.name
      formData.code = type.code
      formData.status = type.status
      formData.remark = type.remark || ''
    }
  } catch {
    ElMessage.error(t('common.error.request'))
  }
}

// 重置表单
const resetForm = () => {
  formData.name = ''
  formData.code = ''
  formData.status = 1
  formData.remark = ''
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
      await updateDictType(props.typeId!, {
        name: formData.name,
        status: formData.status,
        remark: formData.remark
      })
      ElMessage.success(t('basic.dict.type.message.updateSuccess'))
    } else {
      await createDictType(formData)
      ElMessage.success(t('basic.dict.type.message.addSuccess'))
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
