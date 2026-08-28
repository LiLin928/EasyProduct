<template>
  <el-dialog
    :model-value="visible"
    :title="isEdit ? t('mall.member.form.editTitle') : t('mall.member.form.addTitle')"
    width="600px"
    append-to-body
    @update:model-value="handleClose"
  >
    <el-form
      ref="formRef"
      :model="model"
      :rules="rules"
      label-width="100px"
      :disabled="saving"
    >
      <el-form-item
        :label="t('mall.member.nickname')"
        prop="nickname"
      >
        <el-input
          v-model="model.nickname"
          :placeholder="t('mall.member.form.nicknamePlaceholder')"
        />
      </el-form-item>
      <el-form-item
        :label="t('mall.member.phone')"
        prop="phone"
      >
        <el-input
          v-model="model.phone"
          :placeholder="t('mall.member.form.phonePlaceholder')"
        />
      </el-form-item>
      <el-form-item
        :label="t('mall.member.level')"
        prop="levelId"
      >
        <el-select
          v-model="model.levelId"
          :placeholder="t('mall.member.form.levelPlaceholder')"
          style="width: 100%"
        >
          <el-option
            v-for="lv in levels"
            :key="lv.id"
            :label="lv.name"
            :value="lv.id"
          />
        </el-select>
      </el-form-item>
      <el-form-item
        :label="t('mall.member.status')"
        prop="status"
      >
        <el-radio-group v-model="model.status">
          <el-radio value="active">
            {{ t('mall.member.statusActive') }}
          </el-radio>
          <el-radio value="inactive">
            {{ t('mall.member.statusInactive') }}
          </el-radio>
        </el-radio-group>
      </el-form-item>
    </el-form>

    <template #footer>
      <el-button @click="handleClose(false)">
        {{ t('common.cancel') }}
      </el-button>
      <el-button
        type="primary"
        :loading="saving"
        @click="handleSubmit"
      >
        {{ t('common.confirm') }}
      </el-button>
    </template>
  </el-dialog>
</template>

<script setup lang="ts">
import { ref, reactive, watch } from 'vue'
import type { FormInstance, FormRules } from 'element-plus'
import { ElMessage } from 'element-plus'
import { useI18n } from 'vue-i18n'
import { createMember, updateMember } from '@/api/mall/member'
import type { MemberParams, Member } from '@/types/mall'

const props = defineProps<{
  visible: boolean
  isEdit: boolean
  rowData?: Member
  levels: Array<{ id: string; name: string }>
}>()

const emit = defineEmits<{
  (e: 'update:visible', value: boolean): void
  (e: 'success'): void
}>()

const { t } = useI18n()

const formRef = ref<FormInstance>()
const saving = ref(false)

const model = reactive<MemberParams>({
  nickname: '',
  phone: '',
  levelId: '',
  status: 'active',
})

const rules: FormRules = {
  nickname: [{ required: true, message: () => t('mall.member.form.nicknameRequired'), trigger: 'blur' }],
}

watch(() => props.visible, (val) => {
  if (!val) return
  if (props.isEdit && props.rowData) {
    model.nickname = props.rowData.nickname
    model.phone = props.rowData.phone
    model.levelId = props.rowData.levelId
    model.status = props.rowData.status
  } else {
    model.nickname = ''
    model.phone = ''
    model.levelId = ''
    model.status = 'active'
  }
  formRef.value?.clearValidate()
})

const handleClose = (val: boolean = false) => {
  emit('update:visible', val)
}

const handleSubmit = async () => {
  if (!formRef.value) return
  await formRef.value.validate(async (valid) => {
    if (!valid) return
    saving.value = true
    try {
      if (props.isEdit && props.rowData) {
        await updateMember(props.rowData.id, model)
        ElMessage.success(t('mall.member.message.updateSuccess'))
      } else {
        await createMember(model)
        ElMessage.success(t('mall.member.message.createSuccess'))
      }
      emit('success')
      handleClose(false)
    } finally {
      saving.value = false
    }
  })
}
</script>
