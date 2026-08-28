<template>
  <el-dialog
    :model-value="visible"
    :title="isEdit ? t('mall.level.form.editTitle') : t('mall.level.form.addTitle')"
    width="500px"
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
        :label="t('mall.level.name')"
        prop="name"
      >
        <el-input
          v-model="model.name"
          :placeholder="t('mall.level.form.namePlaceholder')"
        />
      </el-form-item>
      <el-form-item
        :label="t('mall.level.minPoints')"
        prop="minPoints"
      >
        <el-input-number
          v-model="model.minPoints"
          :min="0"
          :placeholder="t('mall.level.form.minPointsPlaceholder')"
          style="width: 100%"
        />
      </el-form-item>
      <el-form-item
        :label="t('mall.level.discount')"
        prop="discount"
      >
        <el-input-number
          v-model="model.discount"
          :min="0"
          :max="1"
          :step="0.01"
          :placeholder="t('mall.level.form.discountPlaceholder')"
          style="width: 100%"
        />
      </el-form-item>
      <el-form-item
        :label="t('mall.level.sort')"
        prop="sort"
      >
        <el-input-number
          v-model="model.sort"
          :min="0"
          :placeholder="t('mall.level.form.sortPlaceholder')"
          style="width: 100%"
        />
      </el-form-item>
      <el-form-item
        :label="t('mall.level.status')"
        prop="status"
      >
        <el-radio-group v-model="model.status">
          <el-radio value="enabled">
            {{ t('mall.level.statusEnabled') }}
          </el-radio>
          <el-radio value="disabled">
            {{ t('mall.level.statusDisabled') }}
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
import { createLevel, updateLevel } from '@/api/mall/level'
import type { LevelParams, MemberLevel } from '@/types/mall'

const props = defineProps<{
  visible: boolean
  isEdit: boolean
  rowData?: MemberLevel
}>()

const emit = defineEmits<{
  (e: 'update:visible', value: boolean): void
  (e: 'success'): void
}>()

const { t } = useI18n()

const formRef = ref<FormInstance>()
const saving = ref(false)

const model = reactive<LevelParams>({
  name: '',
  minPoints: 0,
  discount: 1.0,
  sort: 0,
  status: 'enabled',
})

const rules: FormRules = {
  name: [{ required: true, message: () => t('mall.level.form.nameRequired'), trigger: 'blur' }],
}

watch(() => props.visible, (val) => {
  if (!val) return
  if (props.isEdit && props.rowData) {
    model.name = props.rowData.name
    model.minPoints = props.rowData.minPoints
    model.discount = props.rowData.discount
    model.sort = props.rowData.sort
    model.status = props.rowData.status
  } else {
    model.name = ''
    model.minPoints = 0
    model.discount = 1.0
    model.sort = 0
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
        await updateLevel(props.rowData.id, model)
        ElMessage.success(t('mall.level.message.updateSuccess'))
      } else {
        await createLevel(model)
        ElMessage.success(t('mall.level.message.createSuccess'))
      }
      emit('success')
      handleClose(false)
    } finally {
      saving.value = false
    }
  })
}
</script>
