<template>
  <div class="desktop-page">
    <el-card
      shadow="never"
      class="desktop-page__welcome"
    >
      <div class="welcome-info">
        <h2>{{ t('basic.desktop.welcome') }}，{{ userStore.realName }}</h2>
        <p class="welcome-sub">
          {{ t('basic.desktop.today') }}：{{ today }}
        </p>
      </div>
    </el-card>
    <el-row
      :gutter="16"
      class="desktop-page__stats"
    >
      <el-col
        v-for="card in statCards"
        :key="card.key"
        :span="6"
      >
        <el-card
          shadow="hover"
          class="stat-card"
        >
          <div class="stat-card__value">
            {{ card.value }}
          </div>
          <div class="stat-card__label">
            {{ t(card.label) }}
          </div>
        </el-card>
      </el-col>
    </el-row>
    <el-row :gutter="16">
      <el-col :span="16">
        <el-card
          shadow="never"
          class="desktop-page__panel"
        >
          <template #header>
            {{ t('basic.desktop.widget.overview') }}
          </template>
          <el-row
            :gutter="16"
            class="overview-cards"
          >
            <el-col :span="8">
              <div class="overview-card overview-card--sales">
                <div class="overview-card__label">
                  {{ t('basic.desktop.widget.salesAmount') }}
                </div>
                <div class="overview-card__value">
                  ¥{{ overviewData.salesAmount }}
                </div>
              </div>
            </el-col>
            <el-col :span="8">
              <div class="overview-card overview-card--purchase">
                <div class="overview-card__label">
                  {{ t('basic.desktop.widget.purchaseAmount') }}
                </div>
                <div class="overview-card__value">
                  ¥{{ overviewData.purchaseAmount }}
                </div>
              </div>
            </el-col>
            <el-col :span="8">
              <div class="overview-card overview-card--profit">
                <div class="overview-card__label">
                  {{ t('basic.desktop.widget.profitAmount') }}
                </div>
                <div class="overview-card__value">
                  ¥{{ overviewData.profitAmount }}
                </div>
              </div>
            </el-col>
          </el-row>
          <v-chart
            :option="trendChartOption"
            autoresize
            class="trend-chart"
          />
        </el-card>
      </el-col>
      <el-col :span="8">
        <el-card
          shadow="never"
          class="desktop-page__panel"
        >
          <template #header>
            {{ t('basic.desktop.widget.todoCount') }}
          </template>
          <div class="todo-count-list">
            <div
              v-for="item in todoCountItems"
              :key="item.key"
              class="todo-count-item"
              @click="router.push(item.path)"
            >
              <div
                class="todo-count-item__value"
                :class="`todo-count-item__value--${item.color}`"
              >
                {{ item.value }}
              </div>
              <div class="todo-count-item__label">
                {{ t(item.labelKey) }}
              </div>
            </div>
          </div>
        </el-card>
      </el-col>
    </el-row>
    <el-row :gutter="16">
      <el-col :span="8">
        <el-card
          shadow="never"
          class="desktop-page__panel"
        >
          <template #header>
            {{ t('basic.desktop.widget.kpiCustomerGrowth') }}
          </template>
          <v-chart
            :option="customerGrowthChartOption"
            autoresize
            class="kpi-chart"
          />
        </el-card>
      </el-col>
      <el-col :span="8">
        <el-card
          shadow="never"
          class="desktop-page__panel"
        >
          <template #header>
            {{ t('basic.desktop.widget.kpiOrderConversion') }}
          </template>
          <v-chart
            :option="orderConversionChartOption"
            autoresize
            class="kpi-chart"
          />
        </el-card>
      </el-col>
      <el-col :span="8">
        <el-card
          shadow="never"
          class="desktop-page__panel"
        >
          <template #header>
            {{ t('basic.desktop.widget.kpiInventoryTurnover') }}
          </template>
          <v-chart
            :option="inventoryTurnoverChartOption"
            autoresize
            class="kpi-chart"
          />
        </el-card>
      </el-col>
    </el-row>
    <el-card
      shadow="never"
      class="desktop-page__panel"
    >
      <template #header>
        {{ t('basic.desktop.widget.alerts') }}
      </template>
      <el-table
        :data="alerts"
        border
        size="small"
        max-height="300"
      >
        <el-table-column
          prop="type"
          :label="t('basic.desktop.widget.alertType')"
          width="100"
          align="center"
        >
          <template #default="{ row }">
            <el-tag
              :type="alertTagType(row.type)"
              size="small"
            >
              {{ t(`basic.desktop.widget.alertType_${row.type}`) }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column
          prop="title"
          :label="t('basic.desktop.widget.alertTitle')"
          min-width="300"
          show-overflow-tooltip
        />
        <el-table-column
          prop="level"
          :label="t('basic.desktop.widget.alertLevel')"
          width="100"
          align="center"
        >
          <template #default="{ row }">
            <el-tag
              :type="row.level === 'danger' ? 'danger' : 'warning'"
              size="small"
            >
              {{ t(`basic.desktop.widget.alertLevel_${row.level}`) }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column
          prop="createdAt"
          :label="t('basic.desktop.widget.alertTime')"
          width="170"
        />
      </el-table>
    </el-card>
    <el-card
      shadow="never"
      class="desktop-page__quick"
    >
      <template #header>
        {{ t('basic.desktop.quickEntry') }}
      </template>
      <el-row :gutter="16">
        <el-col
          v-for="m in QUICK_ENTRIES"
          :key="m.path"
          :span="3"
        >
          <el-button
            class="quick-btn"
            @click="router.push(m.path)"
          >
            {{ t(m.titleKey) }}
          </el-button>
        </el-col>
      </el-row>
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { useRouter } from 'vue-router'
import { useI18n } from 'vue-i18n'
import dayjs from 'dayjs'
import VChart from 'vue-echarts'
import { use } from 'echarts/core'
import { CanvasRenderer } from 'echarts/renderers'
import { LineChart, BarChart, PieChart } from 'echarts/charts'
import { GridComponent, TooltipComponent, LegendComponent } from 'echarts/components'
import { useUserStore } from '@/stores/user'
import { getDesktopOverview, getDashboardOverview, getDashboardKpi, getDashboardTodoCount, getDashboardAlerts } from '@/api/basic/desktop'
import type { DesktopOverview, DashboardOverview, DashboardKpi, DashboardTodoCount, DashboardAlert } from '@/types/basic'

use([CanvasRenderer, LineChart, BarChart, PieChart, GridComponent, TooltipComponent, LegendComponent])

const { t } = useI18n()
const router = useRouter()
const userStore = useUserStore()
const today = dayjs().format('YYYY-MM-DD HH:mm')

const overview = ref<DesktopOverview>({ userCount: 0, orderCount: 0, salesAmount: 0, todayVisits: 0, recentOrders: [], todos: [] })
const overviewData = ref<DashboardOverview>({ salesAmount: 0, purchaseAmount: 0, profitAmount: 0, trend: [] })
const kpiData = ref<DashboardKpi>({ customerGrowth: [], orderConversion: [], inventoryTurnover: [] })
const todoCount = ref<DashboardTodoCount>({ pendingApproval: 0, processingOrder: 0, lowStockAlert: 0 })
const alerts = ref<DashboardAlert[]>([])

const QUICK_ENTRIES = [
  { path: '/basic/user', titleKey: 'menu.basicUser' },
  { path: '/basic/role', titleKey: 'menu.basicRole' },
  { path: '/basic/menu', titleKey: 'menu.basicMenu' },
  { path: '/basic/dept', titleKey: 'menu.basicDept' },
  { path: '/basic/dict', titleKey: 'menu.basicDict' },
  { path: '/basic/config', titleKey: 'menu.basicConfig' },
  { path: '/profile', titleKey: 'menu.profile' },
] as const

const statCards = computed(() => [
  { key: 'user', value: overview.value.userCount, label: 'basic.desktop.stat.userCount' },
  { key: 'order', value: overview.value.orderCount, label: 'basic.desktop.stat.orderCount' },
  { key: 'sales', value: `¥${overview.value.salesAmount}`, label: 'basic.desktop.stat.salesAmount' },
  { key: 'visits', value: overview.value.todayVisits, label: 'basic.desktop.stat.todayVisits' },
])

const todoCountItems = computed(() => [
  { key: 'approval', value: todoCount.value.pendingApproval, labelKey: 'basic.desktop.widget.todoApproval', path: '/workflow/todo', color: 'primary' },
  { key: 'order', value: todoCount.value.processingOrder, labelKey: 'basic.desktop.widget.todoOrder', path: '/mall/order', color: 'warning' },
  { key: 'stock', value: todoCount.value.lowStockAlert, labelKey: 'basic.desktop.widget.todoStock', path: '/crm/stock-alert', color: 'danger' },
])

const trendChartOption = computed(() => ({
  tooltip: { trigger: 'axis' },
  legend: { data: [t('basic.desktop.widget.salesAmount'), t('basic.desktop.widget.purchaseAmount'), t('basic.desktop.widget.profitAmount')] },
  grid: { left: '3%', right: '4%', bottom: '3%', containLabel: true },
  xAxis: { type: 'category', data: overviewData.value.trend.map((i) => i.month) },
  yAxis: { type: 'value' },
  series: [
    { name: t('basic.desktop.widget.salesAmount'), type: 'line', smooth: true, data: overviewData.value.trend.map((i) => i.sales) },
    { name: t('basic.desktop.widget.purchaseAmount'), type: 'line', smooth: true, data: overviewData.value.trend.map((i) => i.purchase) },
    { name: t('basic.desktop.widget.profitAmount'), type: 'line', smooth: true, data: overviewData.value.trend.map((i) => i.profit) },
  ],
}))

const customerGrowthChartOption = computed(() => ({
  tooltip: { trigger: 'axis' },
  grid: { left: '3%', right: '4%', bottom: '3%', containLabel: true },
  xAxis: { type: 'category', data: kpiData.value.customerGrowth.map((i) => i.month) },
  yAxis: { type: 'value' },
  series: [{ type: 'bar', data: kpiData.value.customerGrowth.map((i) => i.value), itemStyle: { color: '#409eff' } }],
}))

const orderConversionChartOption = computed(() => ({
  tooltip: { trigger: 'item' },
  legend: { bottom: 0 },
  series: [{ type: 'pie', radius: ['40%', '70%'], data: kpiData.value.orderConversion.map((i) => ({ name: i.name, value: i.value })) }],
}))

const inventoryTurnoverChartOption = computed(() => ({
  tooltip: { trigger: 'axis' },
  grid: { left: '3%', right: '4%', bottom: '3%', containLabel: true },
  xAxis: { type: 'category', data: kpiData.value.inventoryTurnover.map((i) => i.month) },
  yAxis: { type: 'value' },
  series: [{ type: 'line', smooth: true, data: kpiData.value.inventoryTurnover.map((i) => i.value), itemStyle: { color: '#67c23a' } }],
}))

const alertTagType = (type: string): '' | 'warning' | 'danger' | 'info' => {
  const map: Record<string, '' | 'warning' | 'danger' | 'info'> = { stock: 'warning', ar: 'danger', order: 'info' }
  return map[type] ?? 'info'
}

const loadAll = async () => {
  try {
    const [ov, widgetOv, kpi, tc, al] = await Promise.all([
      getDesktopOverview(), getDashboardOverview(), getDashboardKpi(), getDashboardTodoCount(), getDashboardAlerts(),
    ])
    overview.value = ov
    overviewData.value = widgetOv
    kpiData.value = kpi
    todoCount.value = tc
    alerts.value = al
  } catch { /* handled by interceptor */ }
}

onMounted(() => { loadAll() })
</script>

<style scoped lang="scss">
.desktop-page {
  padding: $spacing-md;
  &__welcome {
    margin-bottom: $spacing-md;
    .welcome-info { h2 { margin: 0 0 $spacing-xs; } .welcome-sub { color: var(--ep-text-secondary); margin: 0; } }
  }
  &__stats {
    margin-bottom: $spacing-md;
    .stat-card {
      text-align: center;
      &__value { font-size: 28px; font-weight: 600; color: var(--ep-primary); }
      &__label { color: var(--ep-text-secondary); margin-top: $spacing-xs; }
    }
  }
  &__panel { margin-bottom: $spacing-md; }
  &__quick { .quick-btn { width: 100%; } }
  .overview-cards { margin-bottom: $spacing-md; }
  .overview-card {
    text-align: center; padding: $spacing-sm; border-radius: 6px;
    &--sales { background: #ecf5ff; }
    &--purchase { background: #fdf6ec; }
    &--profit { background: #f0f9eb; }
    &__label { font-size: 13px; color: var(--ep-text-secondary); margin-bottom: $spacing-xs; }
    &__value { font-size: 22px; font-weight: 600; }
  }
  .trend-chart { height: 280px; }
  .kpi-chart { height: 240px; }
  .todo-count-list {
    display: flex; flex-direction: column; gap: $spacing-md; padding: $spacing-sm 0;
  }
  .todo-count-item {
    text-align: center; padding: $spacing-sm; border: 1px solid var(--ep-border-color); border-radius: 6px; cursor: pointer; transition: border-color 0.2s;
    &:hover { border-color: var(--ep-primary); }
    &__value { font-size: 32px; font-weight: 700; line-height: 1.2;
      &--primary { color: var(--ep-primary); }
      &--warning { color: var(--ep-color-warning); }
      &--danger { color: var(--ep-color-danger); }
    }
    &__label { color: var(--ep-text-secondary); margin-top: $spacing-xs; font-size: 13px; }
  }
}
</style>
