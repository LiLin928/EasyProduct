<template>
  <div class="report-chart-renderer">
    <!-- 表格视图 -->
    <div
      v-if="chartType === 'table'"
      class="table-view"
    >
      <el-table
        :data="rows"
        border
        stripe
      >
        <el-table-column
          v-for="col in columns"
          :key="col.field"
          :prop="col.field"
          :label="col.label"
          min-width="120"
        />
      </el-table>
    </div>

    <!-- 图表视图 -->
    <div
      v-else
      class="chart-view"
    >
      <v-chart
        v-if="chartOption"
        :option="chartOption"
        autoresize
        class="chart-container"
      />
      <el-empty
        v-else
        :description="t('report.definition.noData')"
      />
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { use } from 'echarts/core'
import { CanvasRenderer } from 'echarts/renderers'
import { GridComponent, TooltipComponent, LegendComponent } from 'echarts/components'
import { LineChart, BarChart, PieChart } from 'echarts/charts'
import { LabelLayout } from 'echarts/features'
import VChart from 'vue-echarts'
import { useI18n } from 'vue-i18n'
import type { EChartsOption } from 'echarts'

// 注册 ECharts 组件
use([
  CanvasRenderer,
  GridComponent,
  TooltipComponent,
  LegendComponent,
  LineChart,
  BarChart,
  PieChart,
  LabelLayout,
])

const { t } = useI18n()

interface Column {
  field: string
  label: string
}

interface Props {
  chartType: 'table' | 'line' | 'bar' | 'pie'
  columns: Column[]
  rows: Record<string, unknown>[]
}

const props = defineProps<Props>()

/**
 * 判断是否为数字类型
 */
const isNumeric = (value: unknown): boolean => {
  if (value === null || value === undefined || value === '') return false
  const num = Number(value)
  return !isNaN(num) && isFinite(num)
}

/**
 * 获取数字列索引（用于折线图/柱状图的系列数据）
 */
const getNumericColumns = (): Column[] => {
  if (props.columns.length <= 1) return []
  return props.columns.slice(1).filter((col) => {
    // 检查该列是否有数字数据
    return props.rows.some((row) => isNumeric(row[col.field]))
  })
}

/**
 * 获取 X 轴类目数据（第一列）
 */
const getXAxisData = (): string[] => {
  if (props.columns.length === 0) return []
  const firstCol = props.columns[0].field
  return props.rows.map((row) => String(row[firstCol] ?? ''))
}

/**
 * 图表配置
 */
const chartOption = computed((): EChartsOption | null => {
  if (!props.rows || props.rows.length === 0) return null
  if (props.columns.length === 0) return null

  const commonOptions: Partial<EChartsOption> = {
    tooltip: {
      trigger: props.chartType === 'pie' ? 'item' : 'axis',
    },
    legend: {
      bottom: 0,
    },
    grid: {
      left: '3%',
      right: '4%',
      bottom: '15%',
      top: '10%',
      containLabel: true,
    },
  }

  switch (props.chartType) {
    case 'line':
      return buildLineChartOption(commonOptions)
    case 'bar':
      return buildBarChartOption(commonOptions)
    case 'pie':
      return buildPieChartOption(commonOptions)
    default:
      return null
  }
})

/**
 * 构建折线图配置
 */
const buildLineChartOption = (commonOptions: Partial<EChartsOption>): EChartsOption | null => {
  const xAxisData = getXAxisData()
  const numericColumns = getNumericColumns()

  if (numericColumns.length === 0) return null

  const series = numericColumns.map((col) => ({
    name: col.label,
    type: 'line',
    smooth: true,
    data: props.rows.map((row) => {
      const val = row[col.field]
      return isNumeric(val) ? Number(val) : 0
    }),
  }))

  return {
    ...commonOptions,
    xAxis: {
      type: 'category',
      data: xAxisData,
      boundaryGap: false,
    },
    yAxis: {
      type: 'value',
    },
    series,
  } as EChartsOption
}

/**
 * 构建柱状图配置
 */
const buildBarChartOption = (commonOptions: Partial<EChartsOption>): EChartsOption | null => {
  const xAxisData = getXAxisData()
  const numericColumns = getNumericColumns()

  if (numericColumns.length === 0) return null

  const series = numericColumns.map((col) => ({
    name: col.label,
    type: 'bar',
    data: props.rows.map((row) => {
      const val = row[col.field]
      return isNumeric(val) ? Number(val) : 0
    }),
  }))

  return {
    ...commonOptions,
    xAxis: {
      type: 'category',
      data: xAxisData,
    },
    yAxis: {
      type: 'value',
    },
    series,
  } as EChartsOption
}

/**
 * 构建饼图配置
 */
const buildPieChartOption = (commonOptions: Partial<EChartsOption>): EChartsOption | null => {
  if (props.columns.length < 2) return null

  const nameCol = props.columns[0].field
  const valueCol = props.columns[1].field

  const data = props.rows
    .map((row) => {
      const name = String(row[nameCol] ?? '')
      const value = isNumeric(row[valueCol]) ? Number(row[valueCol]) : 0
      return { name, value }
    })
    .filter((item) => item.name !== '' && item.value > 0)

  if (data.length === 0) return null

  return {
    ...commonOptions,
    series: [
      {
        name: props.columns[1]?.label || 'Value',
        type: 'pie',
        radius: ['40%', '70%'],
        avoidLabelOverlap: false,
        itemStyle: {
          borderRadius: 10,
          borderColor: '#fff',
          borderWidth: 2,
        },
        label: {
          show: false,
          position: 'center',
        },
        emphasis: {
          label: {
            show: true,
            fontSize: 20,
            fontWeight: 'bold',
          },
        },
        labelLine: {
          show: false,
        },
        data,
      },
    ],
  } as EChartsOption
}
</script>

<style scoped>
.report-chart-renderer {
  width: 100%;
}

.table-view {
  width: 100%;
}

.chart-view {
  width: 100%;
  height: 400px;
}

.chart-container {
  width: 100%;
  height: 100%;
}
</style>
