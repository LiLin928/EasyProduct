<template>
  <el-dialog
    :model-value="visible"
    :title="isEdit ? t('basic.user.edit') : t('basic.user.add')"
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
        :label="t('basic.user.userName')"
        prop="userName"
      >
        <el-input
          v-model="model.userName"
          :placeholder="t('common.inputPlaceholder')"
          :disabled="isEdit"
        />
      </el-form-item>
      <el-form-item
        v-if="!isEdit"
        :label="t('basic.user.password')"
        prop="password"
      >
        <el-input
          v-model="model.password"
          type="password"
          show-password
          :placeholder="t('common.inputPlaceholder')"
        />
      </el-form-item>
      <el-form-item
        :label="t('basic.user.realName')"
        prop="realName"
      >
        <el-input
          v-model="model.realName"
          :placeholder="t('common.inputPlaceholder')"
        />
      </el-form-item>
      <el-form-item
        :label="t('basic.user.email')"
        prop="email"
      >
        <el-input
          v-model="model.email"
          :placeholder="t('common.inputPlaceholder')"
        />
      </el-form-item>
      <el-form-item
        :label="t('basic.user.phone')"
        prop="phone"
      >
        <el-input
          v-model="model.phone"
          :placeholder="t('common.inputPlaceholder')"
        />
      </el-form-item>
      <el-form-item
        :label="t('basic.user.status')"
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
        :label="t('basic.user.dept')"
        prop="deptId"
      >
        <el-input
          v-model="model.deptId"
          :placeholder="t('common.inputPlaceholder')"
        />
      </el-form-item>
      <el-form-item
        :label="t('basic.user.roles')"
        prop="roleIds"
      >
        <el-select
          v-model="model.roleIds"
          multiple
          :placeholder="t('common.selectPlaceholder')"
        >
          <el-option
            :label="t('basic.user.roleSuper')"
            value="1"
          />
          <el-option
            :label="t('basic.user.roleNormal')"
            value="2"
          />
        </el-select>
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
import { createUser, updateUser, getUserDetail } from '@/api/basic/user'
import type { User, UserCreateParams, UserUpdateParams } from '@/types/basic'

interface Props {
  visible: boolean
  payload?: User
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
const model = reactive<UserCreateParams>({
  userName: '',
  password: '',
  realName: '',
  email: '',
  phone: '',
  status: 'enabled',
  deptId: '',
  roleIds: []
})

// 表单验证规则
const rules: FormRules = {
  userName: [{ required: true, message: () => t('common.required'), trigger: 'blur' }],
  password: [{ required: true, message: () => t('common.required'), trigger: 'blur' }],
  realName: [{ required: true, message: () => t('common.required'), trigger: 'blur' }],
  email: [
    { required: true, message: () => t('common.required'), trigger: 'blur' },
    { type: 'email', message: () => t('common.emailFormat'), trigger: 'blur' }
  ],
  phone: [{ required: true, message: () => t('common.required'), trigger: 'blur' }],
  status: [{ required: true, message: () => t('common.required'), trigger: 'change' }],
  deptId: [{ required: true, message: () => t('common.required'), trigger: 'change' }],
  roleIds: [{ required: true, message: () => t('common.required'), trigger: 'change' }]
}

// 重置表单
const resetForm = (): void => {
  Object.assign(model, {
    userName: '',
    password: '',
    realName: '',
    email: '',
    phone: '',
    status: 'enabled',
    deptId: '',
    roleIds: []
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
        const detail = await getUserDetail(payload.id)
        Object.assign(model, {
          userName: detail.userName,
          realName: detail.realName,
          email: detail.email,
          phone: detail.phone,
          status: detail.status,
          deptId: detail.deptId,
          roleIds: detail.roleIds
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
      // 编辑用户
      const params: UserUpdateParams = {
        realName: model.realName,
        email: model.email,
        phone: model.phone,
        status: model.status,
        deptId: model.deptId,
        roleIds: model.roleIds
      }
      await updateUser(props.payload.id, params)
    } else {
      // 新增用户
      await createUser(model)
    }
    emit('success')
    handleClose()
  } finally {
    loading.value = false
  }
}
</script>