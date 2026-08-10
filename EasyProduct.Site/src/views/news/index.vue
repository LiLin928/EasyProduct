<!-- src/views/news/index.vue -->
<template>
  <div class="news-page">
    <h1 class="news-page__title">
      {{ t('site.news.title') }}
    </h1>
    <AppLoading :loading="loading" />
    <template v-if="!loading">
      <AppEmpty v-if="!newsList.length" />
      <ul
        v-else
        class="news-page__list"
      >
        <li
          v-for="item in newsList"
          :key="item.id"
          class="news-item"
        >
          <router-link
            :to="`/news/${item.id}`"
            class="news-item__link"
          >
            <img
              :src="item.coverImage"
              :alt="item.title"
              class="news-item__image"
            >
            <div class="news-item__content">
              <div class="news-item__header">
                <h2 class="news-item__title">
                  {{ newsTitle(item) }}
                </h2>
                <span
                  v-if="item.isTop"
                  class="news-item__top"
                >{{ t('site.news.top') }}</span>
              </div>
              <p class="news-item__summary">
                {{ item.summary }}
              </p>
              <div class="news-item__meta">
                <span>{{ dayjs(item.publishTime).format('YYYY-MM-DD') }}</span>
                <span>{{ item.viewCount }} {{ t('site.news.views') }}</span>
              </div>
            </div>
          </router-link>
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
import dayjs from 'dayjs'
import AppLoading from '@/components/common/AppLoading.vue'
import AppEmpty from '@/components/common/AppEmpty.vue'
import AppPagination from '@/components/common/AppPagination.vue'
import { getNewsList } from '@/api/site/news'
import type { NewsItem } from '@/types/site'

const { t, locale } = useI18n()

const newsList = ref<NewsItem[]>([])
const loading = ref(false)
const total = ref(0)
const query = ref({ pageIndex: 1, pageSize: 10 })

const newsTitle = (item: NewsItem) =>
  locale.value === 'zh-CN' ? item.title : item.titleEn

onMounted(async () => {
  await loadNews()
})

async function loadNews() {
  loading.value = true
  try {
    const result = await getNewsList(query.value)
    newsList.value = result.list
    total.value = result.total
  } finally {
    loading.value = false
  }
}

function handlePageChange(page: number) {
  query.value.pageIndex = page
  loadNews()
}
</script>

<style scoped lang="scss">
.news-page {
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

.news-item {
  margin-bottom: $spacing-lg;
  border-bottom: 1px solid $color-border;

  &__link {
    display: grid;
    gap: $spacing-md;
    padding-bottom: $spacing-lg;

    @include desktop {
      grid-template-columns: 200px 1fr;
    }
  }

  &__image {
    width: 100%;
    height: 120px;
    object-fit: cover;
    border-radius: $radius-sm;
  }

  &__content {
    display: flex;
    flex-direction: column;
    gap: $spacing-sm;
  }

  &__header {
    display: flex;
    align-items: center;
    gap: $spacing-sm;
  }

  &__title {
    margin: 0;
    font-size: $font-size-lg;
  }

  &__top {
    padding: 2px 8px;
    background: $color-primary;
    color: #fff;
    font-size: 12px;
    border-radius: $radius-sm;
  }

  &__summary {
    margin: 0;
    color: $color-text-secondary;
    @include ellipsis(2);
  }

  &__meta {
    display: flex;
    gap: $spacing-md;
    font-size: $font-size-sm;
    color: $color-text-secondary;
  }
}
</style>