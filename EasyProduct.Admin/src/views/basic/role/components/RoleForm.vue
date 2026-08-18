<template>
  <el-dialog
    :model-value="visible"
    :title="isEdit ? t('basic.role.edit') : t('basic.role.add')"
    width="600px"
    @update:model-value="handleClose"
  >
    <el-form
      ref="formRef"
      :model="model"
      :rules="rules"
      label-width="100px"
    >
      <el-form-item
        :label="t('basic.role.name')"
        prop="name"
      >
        <el-input
          v-model="model.name"
          :placeholder="t('common.inputPlaceholder')"
        />
      </el-form-item>
      <el-form-item
        :label="t('basic.role.code')"
        prop="code"
      >
        <el-input
          v-model="model.code"
          :placeholder="t('common.inputPlaceholder')"
          :disabled="isEdit"
        />
      </el-form-item>
      <el-form-item
        :label="t('basic.role.sort')"
        prop="sort"
      >
        <el-input-number
          v-model="model.sort"
          :min="1"
          :max="999"
        />
      </el-form-item>
      <el-form-item
        :label="t('basic.role.status')"
        prop="status"
      >
        <el-radio-group v-model="model.status">
          <el-radio value="enabled">
            {{ t('common.status.enabled') }}
          </el-radio>
          <el-radio value="disabled">
            {{ t('common.status.disabled') }}
          </el-radio>
        </el-radio-group>
      </el-form-item>
      <el-form-item
        :label="t('basic.role.remark')"
        prop="remark"
      >
        <el-input
          v-model="model.remark"
          type="textarea"
          :rows="3"
          :placeholder="t('common.inputPlaceholder')"
        />
      </el-form-item>
    </el-form>
    <template #footer>
      <el-button @click="handleClose">
        {{ t('common.cancel') }}
      </el-button>
      <el-button
        type="primary"
        :loading="loading"
        @click="handleSubmit"
      >
        {{ t('common.confirm') }}
      </el-button>
    </template>
  </el-dialog>
</template>

<script setup lang="ts">
import { ref, reactive, watch } from 'vue'
import { useI18n } from 'vue-i18n'
import type { FormInstance, FormRules } from 'element-plus'
import { createRole, updateRole, getRoleDetail } from '@/api/basic/role'
import type { Role, RoleCreateParams } from '@/types/basic'

interface Props {
  visible: boolean
  payload?: Role
  isEdit: boolean
}

const props = defineProps<Props>()
const emit = defineEmits<{
  'update:visible': [value: boolean]
  'success': []
}>()

const { t } = useI18n()

const formRef = ref<FormInstance>()
const loading = ref(false)

// 表单模型
const model = reactive<RoleCreateParams>({
  name: '',
  code: '',
  status: 'enabled',
  sort: 1,
  remark: '',
  menuIds: []
})

// 表单验证规则
const rules: FormRules = {
  name: [{ required: true, message: () => t('common.required'), trigger: 'blur' }],
  code: [{ required: true, message: () => t('common.required'), trigger: 'blur' }],
  status: [{ required: true, message: () => t('common.required'), trigger: 'change' }],
  sort: [{ required: true, message: () => t('common.required'), trigger: 'blur' }]
}

// 重置表单
const resetForm = (): void => {
  Object.assign(model, {
    name: '',
    code: '',
    status: 'enabled',
    sort: 1,
    remark: '',
    menuIds: []
  })
  formRef.value?.clearValidate()
}

// 监听 payload 变化，填充表单
watch(
  () => props.payload,
  async (payload) => {
    if (payload) {
      // 编辑模式，加载详情
      try {
        const detail = await getRoleDetail(payload.id)
        Object.assign(model, {
          name: detail.name,
          code: detail.code,
          status: detail.status,
          sort: detail.sort,
          remark: detail.remark,
          menuIds: detail.menuIds || []
        })
      } catch (error) {
        // 加载失败
      }
    } else {
      // 新增模式，重置表单
      resetForm()
    }
  },
  { immediate: true }
)

// 关闭弹窗
const handleClose = (): void => {
  emit('update:visible', false)
  resetForm()
}

// 提交表单
const handleSubmit = async (): Promise<void> => {
  const valid = await formRef.value?.validate().catch(() => false)
  if (!valid) return

  loading.value = true
  try {
    if (props.isEdit && props.payload) {
      // 编辑角色
      await updateRole(props.payload.id, model)
    } else {
      // 新增角色
      await createRole(model)
    }
    emit('success')
    handleClose()
  } finally {
    loading.value = false
  }
}
</script>