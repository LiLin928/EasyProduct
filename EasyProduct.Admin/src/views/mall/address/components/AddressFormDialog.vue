<template>
  <BaseFormDialog
    :visible="visible"
    :title="dialogTitle"
    :model="formModel"
    :rules="formRules"
    :loading="submitting"
    width="600px"
    @update:visible="emit('update:visible', $event)"
    @submit="handleSubmit"
  >
    <el-form-item
      :label="t('mall.address.memberId')"
      prop="memberId"
    >
      <MemberSelect
        v-model="formModel.memberId"
        :disabled="isEdit"
      />
    </el-form-item>
    <el-form-item
      :label="t('mall.address.name')"
      prop="name"
    >
      <el-input
        v-model="formModel.name"
        :maxlength="50"
      />
    </el-form-item>
    <el-form-item
      :label="t('mall.address.phone')"
      prop="phone"
    >
      <el-input
        v-model="formModel.phone"
        :maxlength="11"
      />
    </el-form-item>
    <el-row :gutter="16">
      <el-col :span="8">
        <el-form-item
          :label="t('mall.address.province')"
          prop="province"
        >
          <el-input v-model="formModel.province" />
        </el-form-item>
      </el-col>
      <el-col :span="8">
        <el-form-item
          :label="t('mall.address.city')"
          prop="city"
        >
          <el-input v-model="formModel.city" />
        </el-form-item>
      </el-col>
      <el-col :span="8">
        <el-form-item
          :label="t('mall.address.district')"
          prop="district"
        >
          <el-input v-model="formModel.district" />
        </el-form-item>
      </el-col>
    </el-row>
    <el-form-item
      :label="t('mall.address.detail')"
      prop="detail"
    >
      <el-input
        v-model="formModel.detail"
        type="textarea"
        :rows="3"
        :maxlength="200"
        show-word-limit
      />
    </el-form-item>
    <el-form-item>
      <el-checkbox v-model="formModel.isDefault">
        {{ t('mall.address.isDefault') }}
      </el-checkbox>
    </el-form-item>
  </BaseFormDialog>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import { useI18n } from 'vue-i18n'
import { ElMessage } from 'element-plus'
import type { FormRules } from 'element-plus'
import { createAddress, updateAddress } from '@/api/mall/address'
import type { AddressListItem } from '@/types/mall'
import BaseFormDialog from '@/components/common/BaseFormDialog.vue'
import MemberSelect from '@/components/common/MemberSelect.vue'

const props = defineProps<{
  visible: boolean
  isEdit: boolean
  rowData?: AddressListItem | null
}>()

const emit = defineEmits<{
  (e: 'update:visible', value: boolean): void
  (e: 'success'): void
}>()

const { t } = useI18n()

const submitting = ref(false)

const dialogTitle = computed(() =>
  props.isEdit ? t('mall.address.edit') : t('mall.address.add')
)

const defaultFormModel = {
  memberId: '',
  name: '',
  phone: '',
  province: '',
  city: '',
  district: '',
  detail: '',
  isDefault: false,
}

const formModel = ref({ ...defaultFormModel })

const formRules = computed<FormRules>(() => ({
  memberId: [{ required: true, message: t('mall.address.memberIdRequired'), trigger: 'change' }],
  name: [
    { required: true, message: t('mall.address.nameRequired'), trigger: 'blur' },
    { max: 50, message: t('common.validation.maxLength', { max: 50 }), trigger: 'blur' },
  ],
  phone: [
    { required: true, message: t('mall.address.phoneRequired'), trigger: 'blur' },
    { pattern: /^1[3-9]\d{9}$/, message: t('mall.address.phoneInvalid'), trigger: 'blur' },
  ],
  province: [{ required: true, message: t('mall.address.provinceRequired'), trigger: 'blur' }],
  city: [{ required: true, message: t('mall.address.cityRequired'), trigger: 'blur' }],
  district: [{ required: true, message: t('mall.address.districtRequired'), trigger: 'blur' }],
  detail: [
    { required: true, message: t('mall.address.detailRequired'), trigger: 'blur' },
    { max: 200, message: t('common.validation.maxLength', { max: 200 }), trigger: 'blur' },
  ],
}))

watch(
  () => props.visible,
  (visible) => {
    if (visible) {
      if (props.isEdit && props.rowData) {
        formModel.value = {
          memberId: props.rowData.memberId,
          name: props.rowData.name,
          phone: props.rowData.phone,
          province: props.rowData.province,
          city: props.rowData.city,
          district: props.rowData.district,
          detail: props.rowData.detail,
          isDefault: props.rowData.isDefault,
        }
      } else {
        formModel.value = { ...defaultFormModel }
      }
    }
  },
  { immediate: true }
)

async function handleSubmit() {
  submitting.value = true
  try {
    if (props.isEdit && props.rowData) {
      await updateAddress(props.rowData.id, formModel.value)
      ElMessage.success(t('mall.address.message.updateSuccess'))
    } else {
      await createAddress(formModel.value)
      ElMessage.success(t('mall.address.message.createSuccess'))
    }
    emit('update:visible', false)
    emit('success')
  } catch (error: any) {
    ElMessage.error(error?.message || t('common.error'))
  } finally {
    submitting.value = false
  }
}
</script>
