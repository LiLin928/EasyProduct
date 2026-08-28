<template>
  <div class="channel-page">
    <BaseSearchForm
      :fields="searchFields"
      :model="searchModel"
      @search="handleSearch"
      @reset="handleReset"
    />

    <el-card class="channel-page__table">
      <BaseTable
        :loading="loading"
        :data="list"
        :total="total"
        :current-page="query.pageIndex"
        :page-size="query.pageSize"
        @page-change="handlePageChange"
      >
        <el-table-column
          :label="t('product.channel.mainImage')"
          width="80"
          align="center"
        >
          <template #default="{ row }">
            <el-image
              v-if="row.mainImage"
              :src="row.mainImage"
              :preview-src-list="[row.mainImage]"
              fit="cover"
              style="width: 50px; height: 50px; border-radius: 4px"
            />
            <span v-else>-</span>
          </template>
        </el-table-column>
        <el-table-column
          prop="spuName"
          :label="t('product.channel.spuName')"
          min-width="160"
          show-overflow-tooltip
        />
        <el-table-column
          prop="spuCode"
          :label="t('product.channel.spuCode')"
          width="140"
        />
        <el-table-column
          v-for="ch in CHANNEL_TYPES"
          :key="ch"
          :label="t(channelLabel(ch))"
          width="170"
          align="center"
        >
          <template #default="{ row }">
            <div class="channel-cell">
              <el-switch
                :model-value="getChannelInfo(row, ch).published"
                @change="(val: boolean | string | number) => handleToggle(row, ch, !!val)"
              />
              <el-input-number
                :model-value="getChannelInfo(row, ch).sort"
                :controls="false"
                :min="0"
                :precision="0"
                size="small"
                style="width: 80px"
                @change="(val: number | undefined) => handleSortChange(row, ch, val ?? 0)"
              />
            </div>
          </template>
        </el-table-column>
      </BaseTable>
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { useI18n } from 'vue-i18n'
import { ElMessage } from 'element-plus'
import { useTable } from '@/composables/useTable'
import { useSearch } from '@/composables/useSearch'
import { getChannelList, toggleChannel, updateChannelSort } from '@/api/product/channel'
import type { ProductChannelRow, ChannelInfo, ChannelType } from '@/types/product'
import type { SearchField } from '@/types/search'
import BaseTable from '@/components/common/BaseTable.vue'
import BaseSearchForm from '@/components/common/BaseSearchForm.vue'

const { t } = useI18n()

const CHANNEL_TYPES: ChannelType[] = ['site', 'miniapp', 'b2b']

const channelLabel = (ch: ChannelType): string => {
  const map: Record<ChannelType, string> = {
    site: 'product.channel.typeSite',
    miniapp: 'product.channel.typeMiniapp',
    b2b: 'product.channel.typeB2b',
  }
  return map[ch]
}

const getChannelInfo = (row: ProductChannelRow, ch: ChannelType): ChannelInfo => {
  return row.channels.find(c => c.channel === ch) ?? {
    id: '',
    channel: ch,
    published: false,
    sort: 0,
  }
}

const searchFields: SearchField[] = [
  { prop: 'spuName', label: 'product.channel.searchSpuName', type: 'input' },
  {
    prop: 'channel',
    label: 'product.channel.searchChannel',
    type: 'select',
    options: [
      { label: 'product.channel.typeSite', value: 'site' },
      { label: 'product.channel.typeMiniapp', value: 'miniapp' },
      { label: 'product.channel.typeB2b', value: 'b2b' },
    ],
  },
  {
    prop: 'published',
    label: 'product.channel.searchPublished',
    type: 'select',
    options: [
      { label: 'product.channel.published', value: 'true' },
      { label: 'product.channel.unpublished', value: 'false' },
    ],
  },
]

const { searchModel, resetModel, getSearchParams } = useSearch({
  defaultModel: { spuName: '', channel: '', published: '' },
})

const {
  loading,
  list,
  total,
  query,
  handleSearch: tableSearch,
  handleReset: tableReset,
  handlePageChange,
  reload,
} = useTable(getChannelList, { immediate: true })

const handleSearch = (): void => {
  Object.assign(query, getSearchParams())
  tableSearch()
}

const handleReset = (): void => {
  resetModel()
  delete query.spuName
  delete query.channel
  delete query.published
  tableReset()
}

const handleToggle = async (
  row: ProductChannelRow,
  ch: ChannelType,
  published: boolean,
): Promise<void> => {
  try {
    await toggleChannel({ spuId: row.spuId, channel: ch, published })
    ElMessage.success(t('product.channel.message.toggleSuccess'))
  } catch {
    // handled by interceptor
  } finally {
    reload()
  }
}

const handleSortChange = async (
  row: ProductChannelRow,
  ch: ChannelType,
  sort: number,
): Promise<void> => {
  try {
    await updateChannelSort({ spuId: row.spuId, channel: ch, sort })
    ElMessage.success(t('product.channel.message.sortSuccess'))
  } catch {
    // handled by interceptor
  } finally {
    reload()
  }
}
</script>

<style scoped lang="scss">
.channel-page {
}

.channel-cell {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 4px;
}
</style>
