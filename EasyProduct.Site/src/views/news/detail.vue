<!-- src/views/news/detail.vue -->
<template>
  <div class="news-detail">
    <AppLoading :loading="loading" />
    <article
      v-if="news"
      class="news-detail__content"
    >
      <h1 class="news-detail__title">
        {{ newsTitle }}
      </h1>
      <div class="news-detail__meta">
        <span>{{ dayjs(news.publishTime).format('YYYY-MM-DD') }}</span>
        <span>{{ news.viewCount }} {{ t('site.news.views') }}</span>
      </div>
      <img
        :src="news.coverImage"
        :alt="newsTitle"
        class="news-detail__cover"
      >
      <div class="news-detail__body">
        {{ locale === 'zh-CN' ? news.content : news.contentEn }}
      </div>
    </article>
  </div>
</template>

<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useI18n } from 'vue-i18n'
import dayjs from 'dayjs'
import { getNewsDetail } from '@/api/site/news'
import AppLoading from '@/components/common/AppLoading.vue'
import type { NewsDetail } from '@/types/site'

const { t, locale } = useI18n()
const route = useRoute()
const router = useRouter()

const news = ref<NewsDetail | null>(null)
const loading = ref(false)

const newsTitle = computed(() =>
  news.value ? (locale.value === 'zh-CN' ? news.value.title : news.value.titleEn) : ''
)

onMounted(async () => {
  loading.value = true
  try {
    news.value = await getNewsDetail(route.params.id as string)
  } catch {
    router.push('/news')
  } finally {
    loading.value = false
  }
})
</script>

<style scoped lang="scss">
.news-detail {
  @include site-container;
  padding-top: $spacing-lg;
  padding-bottom: $spacing-xl;

  &__content {
    max-width: 800px;
    margin: 0 auto;
  }

  &__title {
    margin: 0 0 $spacing-md;
    font-size: 28px;
  }

  &__meta {
    display: flex;
    gap: $spacing-md;
    margin-bottom: $spacing-lg;
    font-size: $font-size-sm;
    color: $color-text-secondary;
  }

  &__cover {
    width: 100%;
    border-radius: $radius-md;
    margin-bottom: $spacing-lg;
  }

  &__body {
    line-height: 1.8;
    color: $color-text;
  }
}
</style>