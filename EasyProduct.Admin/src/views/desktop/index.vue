<!-- src/views/desktop/index.vue -->
<template>
  <div class="desktop-page">
    <!-- 欢迎区 -->
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

    <!-- 统计卡片 -->
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
      <!-- 最近订单 -->
      <el-col :span="14">
        <el-card
          shadow="never"
          class="desktop-page__panel"
        >
          <template #header>
            {{ t('basic.desktop.recentOrders') }}
          </template>
          <el-table
            :data="overview.recentOrders"
            border
            size="small"
          >
            <el-table-column
              prop="customerName"
              :label="t('basic.desktop.order.customer')"
              min-width="120"
            />
            <el-table-column
              prop="amount"
              :label="t('basic.desktop.order.amount')"
              width="120"
              align="right"
            >
              <template #default="{ row }">
                ¥{{ row.amount }}
              </template>
            </el-table-column>
            <el-table-column
              prop="status"
              :label="t('basic.desktop.order.status')"
              width="100"
              align="center"
            >
              <template #default="{ row }">
                <el-tag size="small">
                  {{ row.status }}
                </el-tag>
              </template>
            </el-table-column>
            <el-table-column
              prop="createTime"
              :label="t('basic.desktop.order.createTime')"
              width="160"
            />
          </el-table>
        </el-card>
      </el-col>

      <!-- 待办 -->
      <el-col :span="10">
        <el-card
          shadow="never"
          class="desktop-page__panel"
        >
          <template #header>
            {{ t('basic.desktop.todos') }}
          </template>
          <el-timeline>
            <el-timeline-item
              v-for="todo in overview.todos"
              :key="todo.id"
              :timestamp="todo.createTime"
              placement="top"
            >
              <el-tag
                size="small"
                type="info"
                class="todo-type"
              >
                {{ todo.type }}
              </el-tag>
              <span class="todo-title">{{ todo.title }}</span>
            </el-timeline-item>
          </el-timeline>
        </el-card>
      </el-col>
    </el-row>

    <!-- 快捷入口 -->
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
import dayjs from 'dayjs'
import { useLocale } from '@/composables/useLocale'
import { useUserStore } from '@/stores/user'
import { getDesktopOverview } from '@/api/basic/desktop'
import type { DesktopOverview } from '@/types/basic'

const { t } = useLocale()
const router = useRouter()
const userStore = useUserStore()

const today = dayjs().format('YYYY-MM-DD HH:mm')

const overview = ref<DesktopOverview>({
  userCount: 0,
  orderCount: 0,
  salesAmount: 0,
  todayVisits: 0,
  recentOrders: [],
  todos: [],
})

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

const loadOverview = async (): Promise<void> => {
  try {
    overview.value = await getDesktopOverview()
  } catch {
    // 错误已由拦截器处理
  }
}

onMounted(() => {
  loadOverview()
})
</script>

<style scoped lang="scss">
.desktop-page {
  padding: $spacing-md;

  &__welcome {
    margin-bottom: $spacing-md;

    .welcome-info {
      h2 {
        margin: 0 0 $spacing-xs;
      }

      .welcome-sub {
        color: var(--ep-text-secondary);
        margin: 0;
      }
    }
  }

  &__stats {
    margin-bottom: $spacing-md;

    .stat-card {
      text-align: center;

      &__value {
        font-size: 28px;
        font-weight: 600;
        color: var(--ep-primary);
      }

      &__label {
        color: var(--ep-text-secondary);
        margin-top: $spacing-xs;
      }
    }
  }

  &__panel {
    margin-bottom: $spacing-md;
  }

  &__quick {
    .quick-btn {
      width: 100%;
    }
  }

  .todo-type {
    margin-right: $spacing-xs;
  }
}
</style>