<!-- src/components/common/AppPagination.vue -->
<template>
  <div
    v-if="total > 0"
    class="app-pagination"
  >
    <button
      class="app-pagination__btn"
      :disabled="currentPage === 1"
      @click="$emit('change', currentPage - 1)"
    >
      {{ t('common.pagination.prev') }}
    </button>
    <span class="app-pagination__info">
      {{ t('common.pagination.page', { current: currentPage, total: totalPages }) }}
    </span>
    <button
      class="app-pagination__btn"
      :disabled="currentPage === totalPages"
      @click="$emit('change', currentPage + 1)"
    >
      {{ t('common.pagination.next') }}
    </button>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { useI18n } from 'vue-i18n'

const { t } = useI18n()

const props = defineProps<{
  total: number
  pageSize: number
  currentPage: number
}>()

defineEmits<{ change: [page: number] }>()

const totalPages = computed(() => Math.ceil(props.total / props.pageSize))
</script>

<style scoped lang="scss">
.app-pagination {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: $spacing-md;
  margin-top: $spacing-lg;

  &__btn {
    padding: $spacing-xs $spacing-md;
    border: 1px solid $color-border;
    border-radius: $radius-sm;
    background: $color-bg;
    cursor: pointer;
    transition: all 0.3s;

    &:disabled {
      opacity: 0.5;
      cursor: not-allowed;
    }

    &:not(:disabled):hover {
      border-color: $color-primary;
      color: $color-primary;
    }
  }

  &__info {
    color: $color-text-secondary;
  }
}
</style>