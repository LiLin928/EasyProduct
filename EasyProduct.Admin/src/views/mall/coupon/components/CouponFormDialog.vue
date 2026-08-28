<template>
  <el-dialog
    :model-value="visible"
    :title="isEdit ? t('mall.coupon.form.editTitle') : t('mall.coupon.form.addTitle')"
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
        :label="t('mall.coupon.name')"
        prop="name"
      >
        <el-input
          v-model="model.name"
          :placeholder="t('mall.coupon.form.namePlaceholder')"
        />
      </el-form-item>
      <el-form-item
        :label="t('mall.coupon.type')"
        prop="type"
      >
        <el-radio-group v-model="model.type">
          <el-radio value="fixed">
            {{ t('mall.coupon.typeFixed') }}
          </el-radio>
          <el-radio value="percent">
            {{ t('mall.coupon.typePercent') }}
          </el-radio>
        </el-radio-group>
      </el-form-item>
      <el-form-item
        :label="t('mall.coupon.value')"
        prop="value"
      >
        <el-input-number
          v-model="model.value"
          :min="0"
          :step="model.type === 'fixed' ? 1 : 0.01"
          :max="model.type === 'percent' ? 1 : undefined"
          :placeholder="t('mall.coupon.form.valuePlaceholder')"
          style="width: 100%"
        />
      </el-form-item>
      <el-form-item
        :label="t('mall.coupon.minSpend')"
        prop="minSpend"
      >
        <el-input-number
          v-model="model.minSpend"
          :min="0"
          :placeholder="t('mall.coupon.form.minSpendPlaceholder')"
          style="width: 100%"
        />
      </el-form-item>
      <el-form-item
        :label="t('mall.coupon.totalCount')"
        prop="totalCount"
      >
        <el-input-number
          v-model="model.totalCount"
          :min="0"
          :placeholder="t('mall.coupon.form.totalCountPlaceholder')"
          style="width: 100%"
        />
      </el-form-item>
      <el-form-item
        :label="t('mall.coupon.startDate')"
        prop="startDate"
      >
        <el-date-picker
          v-model="model.startDate"
          type="date"
          value-format="YYYY-MM-DD"
          :placeholder="t('mall.coupon.startDate')"
          style="width: 100%"
        />
      </el-form-item>
      <el-form-item
        :label="t('mall.coupon.endDate')"
        prop="endDate"
      >
        <el-date-picker
          v-model="model.endDate"
          type="date"
          value-format="YYYY-MM-DD"
          :placeholder="t('mall.coupon.endDate')"
          style="width: 100%"
        />
      </el-form-item>
      <el-form-item
        :label="t('mall.coupon.status')"
        prop="status"
      >
        <el-radio-group v-model="model.status">
          <el-radio value="enabled">
            {{ t('mall.coupon.statusEnabled') }}
          </el-radio>
          <el-radio value="disabled">
            {{ t('mall.coupon.statusDisabled') }}
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
import { createCoupon, updateCoupon } from '@/api/mall/coupon'
import type { CouponParams, Coupon } from '@/types/mall'

const props = defineProps<{
  visible: boolean
  isEdit: boolean
  rowData?: Coupon
}>()

const emit = defineEmits<{
  (e: 'update:visible', value: boolean): void
  (e: 'success'): void
}>()

const { t } = useI18n()

const formRef = ref<FormInstance>()
const saving = ref(false)

const model = reactive<CouponParams>({
  name: '',
  type: 'fixed',
  value: 0,
  minSpend: 0,
  totalCount: 0,
  startDate: '',
  endDate: '',
  status: 'enabled',
})

const rules: FormRules = {
  name: [{ required: true, message: () => t('mall.coupon.form.nameRequired'), trigger: 'blur' }],
  type: [{ required: true, message: () => t('mall.coupon.form.typeRequired'), trigger: 'change' }],
  value: [{ required: true, message: () => t('mall.coupon.form.valueRequired'), trigger: 'blur' }],
  startDate: [{ required: true, message: () => t('mall.coupon.form.startDateRequired'), trigger: 'change' }],
  endDate: [{ required: true, message: () => t('mall.coupon.form.endDateRequired'), trigger: 'change' }],
}

watch(() => props.visible, (val) => {
  if (!val) return
  if (props.isEdit && props.rowData) {
    model.name = props.rowData.name
    model.type = props.rowData.type
    model.value = props.rowData.value
    model.minSpend = props.rowData.minSpend
    model.totalCount = props.rowData.totalCount
    model.startDate = props.rowData.startDate
    model.endDate = props.rowData.endDate
    model.status = props.rowData.status
  } else {
    model.name = ''
    model.type = 'fixed'
    model.value = 0
    model.minSpend = 0
    model.totalCount = 0
    model.startDate = ''
    model.endDate = ''
    model.status = 'enabled'
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
        await updateCoupon(props.rowData.id, model)
        ElMessage.success(t('mall.coupon.message.updateSuccess'))
      } else {
        await createCoupon(model)
        ElMessage.success(t('mall.coupon.message.createSuccess'))
      }
      emit('success')
      handleClose(false)
    } finally {
      saving.value = false
    }
  })
}
</script>
