<template>
  <el-dialog
    :model-value="visible"
    :title="isEdit ? t('crm.fixedAsset.edit') : t('crm.fixedAsset.add')"
    width="640px"
    append-to-body
    @update:model-value="handleClose"
  >
    <el-form
      ref="formRef"
      :model="model"
      :rules="rules"
      label-width="120px"
      :disabled="saving"
    >
      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item
            :label="t('crm.fixedAsset.assetNo')"
            prop="assetNo"
          >
            <el-input
              v-model="model.assetNo"
              :placeholder="t('crm.fixedAsset.form.assetNoPlaceholder')"
            />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item
            :label="t('crm.fixedAsset.name')"
            prop="name"
          >
            <el-input
              v-model="model.name"
              :placeholder="t('crm.fixedAsset.form.namePlaceholder')"
            />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item
            :label="t('crm.fixedAsset.category')"
            prop="category"
          >
            <el-select
              v-model="model.category"
              :placeholder="t('crm.fixedAsset.form.categoryPlaceholder')"
              style="width: 100%"
            >
              <el-option
                v-for="opt in CATEGORY_OPTIONS"
                :key="opt.value"
                :label="t(opt.labelKey)"
                :value="opt.value"
              />
            </el-select>
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item
            :label="t('crm.fixedAsset.originalValue')"
            prop="originalValue"
          >
            <el-input-number
              v-model="model.originalValue"
              :min="0"
              :precision="2"
              :controls="false"
              style="width: 100%"
            />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item
            :label="t('crm.fixedAsset.purchaseDate')"
            prop="purchaseDate"
          >
            <el-date-picker
              v-model="model.purchaseDate"
              type="date"
              value-format="YYYY-MM-DD"
              style="width: 100%"
            />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item
            :label="t('crm.fixedAsset.salvageValue')"
            prop="salvageValue"
          >
            <el-input-number
              v-model="model.salvageValue"
              :min="0"
              :precision="2"
              :controls="false"
              style="width: 100%"
            />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item
            :label="t('crm.fixedAsset.usefulYears')"
            prop="usefulYears"
          >
            <el-input-number
              v-model="model.usefulYears"
              :min="1"
              :max="50"
              :controls="false"
              style="width: 100%"
            />
          </el-form-item>
        </el-col>
        <el-col :span="24">
          <el-form-item
            :label="t('crm.fixedAsset.remark')"
            prop="remark"
          >
            <el-input
              v-model="model.remark"
              type="textarea"
              :rows="2"
              :placeholder="t('crm.fixedAsset.form.remarkPlaceholder')"
            />
          </el-form-item>
        </el-col>
      </el-row>
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
import { ref, reactive, computed, watch } from 'vue'
import type { FormInstance, FormRules } from 'element-plus'
import { ElMessage } from 'element-plus'
import { useI18n } from 'vue-i18n'
import { createFixedAsset, updateFixedAsset } from '@/api/crm/fixed-asset'
import { ASSET_CATEGORY_OPTIONS } from '@/types/crm'
import type { FixedAsset } from '@/types/crm'

const props = defineProps<{
  visible: boolean
  asset: FixedAsset | null
}>()

const emit = defineEmits<{
  (e: 'update:visible', value: boolean): void
  (e: 'success'): void
}>()

const { t } = useI18n()

const CATEGORY_OPTIONS = ASSET_CATEGORY_OPTIONS

const isEdit = computed(() => !!props.asset)

const formRef = ref<FormInstance>()
const saving = ref(false)

const model = reactive({
  assetNo: '',
  name: '',
  category: '',
  originalValue: 0,
  purchaseDate: new Date().toISOString().slice(0, 10),
  salvageValue: 0,
  usefulYears: 5,
  remark: '',
})

const rules: FormRules = {
  name: [{ required: true, message: () => t('common.required'), trigger: 'blur' }],
  category: [{ required: true, message: () => t('common.required'), trigger: 'change' }],
  originalValue: [{ required: true, message: () => t('common.required'), trigger: 'blur' }],
  purchaseDate: [{ required: true, message: () => t('common.required'), trigger: 'change' }],
}

watch(() => props.visible, (val) => {
  if (!val) return
  if (props.asset) {
    model.assetNo = props.asset.assetNo
    model.name = props.asset.name
    model.category = props.asset.category
    model.originalValue = props.asset.originalValue
    model.purchaseDate = props.asset.purchaseDate
    model.salvageValue = props.asset.salvageValue
    model.usefulYears = props.asset.usefulYears
    model.remark = props.asset.remark
  } else {
    model.assetNo = ''
    model.name = ''
    model.category = ''
    model.originalValue = 0
    model.purchaseDate = new Date().toISOString().slice(0, 10)
    model.salvageValue = 0
    model.usefulYears = 5
    model.remark = ''
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
      if (isEdit.value && props.asset) {
        await updateFixedAsset(props.asset.id, model)
        ElMessage.success(t('common.success'))
      } else {
        await createFixedAsset(model)
        ElMessage.success(t('common.success'))
      }
      emit('success')
      handleClose(false)
    } finally {
      saving.value = false
    }
  })
}
</script>
