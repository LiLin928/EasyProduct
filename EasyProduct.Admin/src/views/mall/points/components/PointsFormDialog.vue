<template>
  <el-dialog
    :model-value="visible"
    :title="t('mall.points.form.addTitle')"
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
        :label="t('mall.points.memberName')"
        prop="memberId"
      >
        <el-select
          v-model="model.memberId"
          :placeholder="t('mall.points.form.memberPlaceholder')"
          filterable
          style="width: 100%"
        >
          <el-option
            v-for="m in members"
            :key="m.id"
            :label="m.name"
            :value="m.id"
          />
        </el-select>
      </el-form-item>
      <el-form-item
        :label="t('mall.points.type')"
        prop="type"
      >
        <el-radio-group v-model="model.type">
          <el-radio value="earn">
            {{ t('mall.points.typeEarn') }}
          </el-radio>
          <el-radio value="spend">
            {{ t('mall.points.typeSpend') }}
          </el-radio>
        </el-radio-group>
      </el-form-item>
      <el-form-item
        :label="t('mall.points.amount')"
        prop="amount"
      >
        <el-input-number
          v-model="model.amount"
          :min="1"
          :placeholder="t('mall.points.form.amountPlaceholder')"
          style="width: 100%"
        />
      </el-form-item>
      <el-form-item
        :label="t('mall.points.source')"
        prop="source"
      >
        <el-select
          v-model="model.source"
          :placeholder="t('mall.points.form.sourcePlaceholder')"
          style="width: 100%"
        >
          <el-option
            :label="t('mall.points.sourceOrder')"
            value="order"
          />
          <el-option
            :label="t('mall.points.sourceSignin')"
            value="signin"
          />
          <el-option
            :label="t('mall.points.sourceActivity')"
            value="activity"
          />
          <el-option
            :label="t('mall.points.sourceRefund')"
            value="refund"
          />
          <el-option
            :label="t('mall.points.sourceAdjust')"
            value="adjust"
          />
        </el-select>
      </el-form-item>
      <el-form-item
        :label="t('mall.points.description')"
        prop="description"
      >
        <el-input
          v-model="model.description"
          type="textarea"
          :rows="2"
          :placeholder="t('mall.points.form.descriptionPlaceholder')"
        />
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
import { createPointsRecord } from '@/api/mall/points'

const props = defineProps<{
  visible: boolean
  members: Array<{ id: string; name: string }>
}>()

const emit = defineEmits<{
  (e: 'update:visible', value: boolean): void
  (e: 'success'): void
}>()

const { t } = useI18n()

const formRef = ref<FormInstance>()
const saving = ref(false)

const model = reactive({
  memberId: '',
  type: 'earn' as 'earn' | 'spend',
  amount: 1,
  source: 'adjust' as string,
  description: '',
})

const rules: FormRules = {
  memberId: [{ required: true, message: () => t('mall.points.form.memberRequired'), trigger: 'change' }],
  type: [{ required: true, message: () => t('mall.points.form.typeRequired'), trigger: 'change' }],
  amount: [{ required: true, message: () => t('mall.points.form.amountRequired'), trigger: 'blur' }],
  source: [{ required: true, message: () => t('mall.points.form.sourceRequired'), trigger: 'change' }],
}

watch(() => props.visible, (val) => {
  if (!val) return
  model.memberId = ''
  model.type = 'earn'
  model.amount = 1
  model.source = 'adjust'
  model.description = ''
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
      const submitAmount = model.type === 'spend' ? -model.amount : model.amount
      await createPointsRecord({
        memberId: model.memberId,
        type: model.type,
        amount: submitAmount,
        source: model.source,
        description: model.description,
      })
      ElMessage.success(t('mall.points.message.createSuccess'))
      emit('success')
      handleClose(false)
    } finally {
      saving.value = false
    }
  })
}
</script>
