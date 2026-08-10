<!-- src/views/downloads/index.vue -->
<template>
  <div class="downloads-page">
    <h1 class="downloads-page__title">
      {{ t('site.downloads.title') }}
    </h1>
    <AppLoading :loading="loading" />
    <template v-if="!loading">
      <AppEmpty v-if="!downloads.length" />
      <ul
        v-else
        class="downloads-page__list"
      >
        <li
          v-for="item in downloads"
          :key="item.id"
          class="download-item"
        >
          <div class="download-item__icon">
            📄
          </div>
          <div class="download-item__content">
            <h3 class="download-item__title">
              {{ downloadTitle(item) }}
            </h3>
            <div class="download-item__meta">
              <span>{{ formatFileSize(item.fileSize) }}</span>
              <span>{{ item.downloadCount }} {{ t('site.downloads.downloads') }}</span>
            </div>
          </div>
          <a
            :href="item.fileUrl"
            class="download-item__btn"
            download
          >
            {{ t('site.downloads.download') }}
          </a>
        </li>
      </ul>
    </template>
    <AppPagination
      v-if="total > 0"
      :total="total"
      :page-size="query.pageSize"
      :current-page="query.pageIndex"
      @change="handlePageChange"
    />
  </div>
</template>

<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { useI18n } from 'vue-i18n'
import AppLoading from '@/components/common/AppLoading.vue'
import AppEmpty from '@/components/common/AppEmpty.vue'
import AppPagination from '@/components/common/AppPagination.vue'
import { getDownloadList } from '@/api/site/download'
import type { Download } from '@/types/site'

const { t, locale } = useI18n()

const downloads = ref<Download[]>([])
const loading = ref(false)
const total = ref(0)
const query = ref({ pageIndex: 1, pageSize: 10 })

const downloadTitle = (item: Download) =>
  locale.value === 'zh-CN' ? item.title : item.titleEn

const formatFileSize = (bytes: number) => {
  if (bytes < 1024) return `${bytes} B`
  if (bytes < 1024 * 1024) return `${(bytes / 1024).toFixed(1)} KB`
  return `${(bytes / (1024 * 1024)).toFixed(1)} MB`
}

onMounted(async () => {
  loading.value = true
  try {
    const result = await getDownloadList(query.value)
    downloads.value = result.list
    total.value = result.total
  } finally {
    loading.value = false
  }
})

function handlePageChange(page: number) {
  query.value.pageIndex = page
  loadDownloads()
}

async function loadDownloads() {
  loading.value = true
  try {
    const result = await getDownloadList(query.value)
    downloads.value = result.list
    total.value = result.total
  } finally {
    loading.value = false
  }
}
</script>

<style scoped lang="scss">
.downloads-page {
  @include site-container;
  padding-top: $spacing-lg;
  padding-bottom: $spacing-xl;

  &__title {
    margin-bottom: $spacing-lg;
  }

  &__list {
    list-style: none;
    padding: 0;
    margin: 0;
  }
}

.download-item {
  display: flex;
  align-items: center;
  gap: $spacing-md;
  padding: $spacing-md;
  border-bottom: 1px solid $color-border;

  &:last-child {
    border-bottom: none;
  }

  &__icon {
    font-size: 32px;
    flex-shrink: 0;
  }

  &__content {
    flex: 1;
    min-width: 0;
  }

  &__title {
    margin: 0 0 $spacing-xs;
    font-size: $font-size-base;
    @include ellipsis(1);
  }

  &__meta {
    display: flex;
    gap: $spacing-md;
    font-size: $font-size-sm;
    color: $color-text-secondary;
  }

  &__btn {
    flex-shrink: 0;
    padding: $spacing-xs $spacing-md;
    border: 1px solid $color-primary;
    border-radius: $radius-sm;
    color: $color-primary;
    text-decoration: none;
    cursor: pointer;
    transition: all 0.3s;

    &:hover {
      background: $color-primary;
      color: #fff;
    }
  }
}
</style>
