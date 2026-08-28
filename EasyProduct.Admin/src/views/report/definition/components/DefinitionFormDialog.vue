<template>
  <el-dialog
    :model-value="visible"
    :title="isEdit ? t('report.definition.edit') : t('report.definition.add')"
    width="800px"
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
      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item
            :label="t('report.definition.name')"
            prop="name"
          >
            <el-input
              v-model="model.name"
              :placeholder="t('report.definition.form.namePlaceholder')"
            />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item
            :label="t('report.definition.code')"
            prop="code"
          >
            <el-input
              v-model="model.code"
              :placeholder="t('report.definition.form.codePlaceholder')"
            />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item
            :label="t('report.definition.datasource')"
            prop="datasourceId"
          >
            <el-select
              v-model="model.datasourceId"
              :placeholder="t('report.definition.form.datasourcePlaceholder')"
              style="width: 100%"
              @change="handleDatasourceChange"
            >
              <el-option
                v-for="ds in datasourceOptions"
                :key="ds.id"
                :label="ds.name"
                :value="ds.id"
              />
            </el-select>
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item
            :label="t('report.definition.chartType')"
            prop="chartType"
          >
            <el-select
              v-model="model.chartType"
              :placeholder="t('report.definition.form.chartTypePlaceholder')"
              style="width: 100%"
            >
              <el-option
                v-for="opt in chartTypeOptions"
                :key="opt.value"
                :label="t(opt.labelKey)"
                :value="opt.value"
              />
            </el-select>
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item
            :label="t('report.definition.status')"
            prop="status"
          >
            <el-select
              v-model="model.status"
              :placeholder="t('report.definition.form.statusPlaceholder')"
              style="width: 100%"
            >
              <el-option
                v-for="opt in statusOptions"
                :key="opt.value"
                :label="t(opt.labelKey)"
                :value="opt.value"
              />
            </el-select>
          </el-form-item>
        </el-col>
        <el-col :span="24">
          <el-form-item
            :label="t('report.definition.sqlTemplate')"
            prop="sqlTemplate"
          >
            <el-input
              v-model="model.sqlTemplate"
              type="textarea"
              :rows="4"
              :placeholder="t('report.definition.form.sqlPlaceholder')"
              style="font-family: monospace"
            />
          </el-form-item>
        </el-col>
        <el-col :span="24">
          <el-form-item :label="t('report.definition.columns')">
            <div class="columns-config">
              <el-button
                size="small"
                type="primary"
                plain
                @click="addColumn"
              >
                {{ t('report.definition.addColumn') }}
              </el-button>
              <el-table
                :data="model.columns"
                border
                size="small"
                style="margin-top: 8px"
              >
                <el-table-column
                  prop="field"
                  :label="t('report.definition.colField')"
                  min-width="140"
                >
                  <template #default="{ row }">
                    <el-input
                      v-model="row.field"
                      size="small"
                    />
                  </template>
                </el-table-column>
                <el-table-column
                  prop="label"
                  :label="t('report.definition.colLabel')"
                  min-width="120"
                >
                  <template #default="{ row }">
                    <el-input
                      v-model="row.label"
                      size="small"
                    />
                  </template>
                </el-table-column>
                <el-table-column
                  prop="type"
                  :label="t('report.definition.colType')"
                  width="120"
                >
                  <template #default="{ row }">
                    <el-select
                      v-model="row.type"
                      size="small"
                      style="width: 100%"
                    >
                      <el-option
                        v-for="opt in columnTypeOptions"
                        :key="opt.value"
                        :label="t(opt.labelKey)"
                        :value="opt.value"
                      />
                    </el-select>
                  </template>
                </el-table-column>
                <el-table-column
                  prop="width"
                  :label="t('report.definition.colWidth')"
                  width="90"
                >
                  <template #default="{ row }">
                    <el-input-number
                      v-model="row.width"
                      size="small"
                      :min="50"
                      :max="500"
                      controls-position="right"
                      style="width: 80px"
                    />
                  </template>
                </el-table-column>
                <el-table-column
                  prop="sortable"
                  :label="t('report.definition.colSortable')"
                  width="80"
                  align="center"
                >
                  <template #default="{ row }">
                    <el-switch v-model="row.sortable" />
                  </template>
                </el-table-column>
                <el-table-column
                  :label="t('common.actions')"
                  width="70"
                  align="center"
                >
                  <template #default="{ $index }">
                    <el-button
                      link
                      type="danger"
                      size="small"
                      @click="removeColumn($index)"
                    >
                      {{ t('common.delete') }}
                    </el-button>
                  </template>
                </el-table-column>
              </el-table>
            </div>
          </el-form-item>
        </el-col>
        <el-col :span="24">
          <el-form-item
            :label="t('report.definition.remark')"
            prop="remark"
          >
            <el-input
              v-model="model.remark"
              type="textarea"
              :rows="2"
              :placeholder="t('report.definition.form.remarkPlaceholder')"
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
import { ref, reactive, watch, onMounted } from 'vue'
import type { FormInstance, FormRules } from 'element-plus'
import { ElMessage } from 'element-plus'
import { useI18n } from 'vue-i18n'
import { createDefinition, updateDefinition } from '@/api/report/definition'
import { getDatasourceOptions } from '@/api/report/datasource'
import { CHART_TYPE_OPTIONS, REPORT_STATUS_OPTIONS, COLUMN_TYPE_OPTIONS } from '@/types/report'
import type { ReportDefinition, ReportColumn } from '@/types/report'

const props = defineProps<{
  visible: boolean
  isEdit: boolean
  rowData?: ReportDefinition
}>()

const emit = defineEmits<{
  (e: 'update:visible', value: boolean): void
  (e: 'success'): void
}>()

const { t } = useI18n()

const formRef = ref<FormInstance>()
const saving = ref(false)

const chartTypeOptions = CHART_TYPE_OPTIONS
const statusOptions = REPORT_STATUS_OPTIONS
const columnTypeOptions = COLUMN_TYPE_OPTIONS

const datasourceOptions = ref<Array<{ id: string; name: string; type: string }>>([])

const loadDatasourceOptions = async () => {
  try {
    datasourceOptions.value = await getDatasourceOptions()
  } catch {
    datasourceOptions.value = []
  }
}

onMounted(() => {
  loadDatasourceOptions()
})

const model = reactive({
  name: '',
  code: '',
  datasourceId: '',
  datasourceName: '',
  sqlTemplate: '',
  chartType: 'table' as 'table' | 'line' | 'bar' | 'pie',
  columns: [] as ReportColumn[],
  status: 'draft' as 'draft' | 'published' | 'archived',
  remark: '',
})

const rules: FormRules = {
  name: [{ required: true, message: () => t('common.required'), trigger: 'blur' }],
  code: [{ required: true, message: () => t('common.required'), trigger: 'blur' }],
  datasourceId: [{ required: true, message: () => t('common.required'), trigger: 'change' }],
}

const handleDatasourceChange = (val: string) => {
  const ds = datasourceOptions.value.find(d => d.id === val)
  model.datasourceName = ds?.name || ''
}

const addColumn = () => {
  model.columns.push({
    field: '',
    label: '',
    type: 'string',
    width: 120,
    format: '',
    sortable: false,
  })
}

const removeColumn = (index: number) => {
  model.columns.splice(index, 1)
}

watch(() => props.visible, (val) => {
  if (!val) return
  if (props.isEdit && props.rowData) {
    model.name = props.rowData.name
    model.code = props.rowData.code
    model.datasourceId = props.rowData.datasourceId
    model.datasourceName = props.rowData.datasourceName
    model.sqlTemplate = props.rowData.sqlTemplate
    model.chartType = props.rowData.chartType
    model.columns = props.rowData.columns.map(c => ({ ...c }))
    model.status = props.rowData.status
    model.remark = props.rowData.remark
  } else {
    model.name = ''
    model.code = ''
    model.datasourceId = ''
    model.datasourceName = ''
    model.sqlTemplate = ''
    model.chartType = 'table'
    model.columns = []
    model.status = 'draft'
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
      if (props.isEdit && props.rowData) {
        await updateDefinition(props.rowData.id, model)
        ElMessage.success(t('common.success'))
      } else {
        await createDefinition(model)
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

<style scoped lang="scss">
.columns-config {
  width: 100%;
}
</style>
