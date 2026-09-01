<template>
  <div class="report-preview-page">
    <el-card
      shadow="never"
      class="report-preview-page__card"
    >
      <template #header>
        <div class="report-header">
          <span class="report-title">{{ reportName }}</span>
          <el-button
            v-if="definition?.chartType != 'table'"
            type="primary"
            @click="toggleChartType"
          >
            {{ t('report.definition.switchChart') }}
          </el-button>
        </div>
      </template>

      <div
        v-if="loading"
        class="loading-container"
      >
        <el-icon
          class="is-loading"
          :size="32"
        >
          <Loading />
        </el-icon>
      </div>

      <template v-else-if="definition && previewData">
        <ReportChartRenderer
          :chart-type="displayChartType"
          :columns="previewData.columns"
          :rows="previewData.rows"
        />
      </template>

      <el-empty
        v-else
        :description="t('report.definition.notFound')"
      />
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, watch } from 'vue'
import { useRoute } from 'vue-router'
import { useI18n } from 'vue-i18n'
import { ElMessage } from 'element-plus'
import { Loading } from '@element-plus/icons-vue'
import { getDefinitionById, getDefinitionByCode, previewDefinition } from '@/api/report/definition'
import type { ReportDefinition, ChartType } from '@/types/report'
import ReportChartRenderer from './components/ReportChartRenderer.vue'

const { t } = useI18n()
const route = useRoute()

const routeId = computed(() => route.params.id as string | undefined)
const routeCode = computed(() => route.query.code as string | undefined)

const loading = ref(false)
const definition = ref<ReportDefinition | null>(null)
const previewData = ref<{ columns: Array<{ field: string; label: string }>; rows: Record<string, unknown>[] } | null>(null)
const displayChartType = ref<ChartType>('table')

const reportName = computed(() => {
  if (definition.value) {
    return definition.value.name
  }
  return t('report.definition.previewTitle')
})

const toggleChartType = () => {
  const types: ChartType[] = ['table', 'line', 'bar', 'pie']
  const currentIndex = types.indexOf(displayChartType.value)
  const nextIndex = (currentIndex + 1) % types.length
  displayChartType.value = types[nextIndex]
}

const loadDefinition = async () => {
  loading.value = true
  try {
    let def: ReportDefinition | null = null

    if (routeId.value) {
      def = await getDefinitionById(routeId.value)
    } else if (routeCode.value) {
      def = await getDefinitionByCode(routeCode.value)
    }

    if (!def) {
      ElMessage.error(t('report.definition.notFound'))
      return
    }

    definition.value = def
    displayChartType.value = def.chartType

    const data = await previewDefinition(def.id)
    previewData.value = data
  } catch {
    ElMessage.error(t('common.error'))
  } finally {
    loading.value = false
  }
}

watch([routeId, routeCode], () => {
  loadDefinition()
}, { immediate: true })

onMounted(() => {
  loadDefinition()
})
</script>
<style scoped lang="scss">
.report-preview-page {
  padding: $spacing-md;
  &__card {
    min-height: calc(100vh - 120px);
  }
  .report-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
  }
  .report-title {
    font-size: 18px;
    font-weight: 600;
    color: var(--ep-text-primary);
  }
  .loading-container {
    display: flex;
    justify-content: space-between;
    align-items: center;
    min-height: 400px;
  }
}
</style>
