<!-- src/components/common/BaseTable.vue -->
<template>
  <div class="base-table">
    <el-table
      v-loading="loading"
      :data="data"
      :border="border"
      :stripe="stripe"
      class="base-table__body"
      @selection-change="handleSelectionChange"
    >
      <slot />
    </el-table>
    <div
      v-if="showPagination"
      class="base-table__footer"
    >
      <el-pagination
        :current-page="currentPage"
        :page-size="pageSize"
        :total="total"
        :page-sizes="pageSizes"
        layout="total, sizes, prev, pager, next, jumper"
        @current-change="handlePageChange"
        @size-change="handleSizeChange"
      />
    </div>
  </div>
</template>

<script setup lang="ts">
interface Props {
  loading?: boolean
  data: unknown[]
  total: number
  pageSize?: number
  currentPage?: number
  pageSizes?: number[]
  showPagination?: boolean
  border?: boolean
  stripe?: boolean
}

withDefaults(defineProps<Props>(), {
  loading: false,
  pageSize: 10,
  currentPage: 1,
  pageSizes: () => [10, 20, 50, 100],
  showPagination: true,
  border: true,
  stripe: false,
})

const emit = defineEmits<{
  'page-change': [page: number]
  'size-change': [size: number]
  'selection-change': [selection: unknown[]]
}>()

const handlePageChange = (page: number): void => {
  emit('page-change', page)
}

const handleSizeChange = (size: number): void => {
  emit('size-change', size)
}

const handleSelectionChange = (selection: unknown[]): void => {
  emit('selection-change', selection)
}
</script>

<style scoped lang="scss">
.base-table {
  &__body {
    width: 100%;
  }

  &__footer {
    display: flex;
    justify-content: flex-end;
    padding: $spacing-md 0;
  }
}
</style>
