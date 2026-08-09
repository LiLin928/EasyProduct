<template>
  <div class="desktop-page">
    <el-card>
      <template #header>
        {{ t('menu.desktop') }}
      </template>
      <p class="desktop-page__hello">
        {{ userStore.realName }}，EasyProduct
      </p>
      <el-row :gutter="16">
        <el-col
          v-for="m in MODULES"
          :key="m"
          :span="6"
        >
          <el-card
            shadow="hover"
            class="desktop-page__widget"
          >
            {{ t(`menu.${m}`) }}
          </el-card>
        </el-col>
      </el-row>
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { useI18n } from 'vue-i18n'
import { useUserStore } from '@/stores/user'

const { t } = useI18n()
const userStore = useUserStore()

/** F2 工作台 Widget 化的八大域占位（经营概览/KPI/待办/预警在 F2-9 落位） */
const MODULES = ['basic', 'product', 'site', 'mall', 'crm', 'workflow', 'report', 'ops'] as const
</script>

<style scoped lang="scss">
.desktop-page {
  &__hello {
    color: var(--ep-text-secondary);
  }

  &__widget {
    margin-bottom: $spacing-md;
    text-align: center;
  }
}
</style>
