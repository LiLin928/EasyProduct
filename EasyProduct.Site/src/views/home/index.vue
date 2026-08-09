<template>
  <div class="home-page">
    <section class="home-page__hero">
      <img
        v-if="banner"
        :src="banner.imageUrl"
        :alt="bannerTitle"
        class="home-page__banner"
      >
    </section>
    <section class="home-page__news">
      <div class="home-page__news-head">
        <h2>{{ t('site.home.newsTitle') }}</h2>
        <router-link to="/news">
          {{ t('site.home.moreNews') }}
        </router-link>
      </div>
      <ul
        v-if="news.length"
        class="home-page__news-list"
      >
        <li
          v-for="item in news"
          :key="item.id"
        >
          <router-link :to="`/news/${item.id}`">
            {{ locale === 'zh-CN' ? item.title : item.titleEn }}
          </router-link>
        </li>
      </ul>
    </section>
  </div>
</template>

<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { useI18n } from 'vue-i18n'
import { getBannerList, getNewsList } from '@/api/site/home'
import type { Banner, NewsItem } from '@/types/site'

const { t, locale } = useI18n()
const banners = ref<Banner[]>([])
const news = ref<NewsItem[]>([])

const banner = computed(() => banners.value[0])
const bannerTitle = computed(() =>
  banner.value ? (locale.value === 'zh-CN' ? banner.value.title : banner.value.titleEn) : '',
)

onMounted(async () => {
  const [bannerList, newsPage] = await Promise.all([
    getBannerList(),
    getNewsList({ pageIndex: 1, pageSize: 5 }),
  ])
  banners.value = bannerList
  news.value = newsPage.list
})
</script>

<style scoped lang="scss">
.home-page {
  @include site-container;
  padding-top: $spacing-lg;
  padding-bottom: $spacing-xl;

  &__banner {
    width: 100%;
    border-radius: $radius-md;
  }

  &__news {
    margin-top: $spacing-xl;
  }

  &__news-head {
    display: flex;
    align-items: baseline;
    justify-content: space-between;
  }

  &__news-list {
    list-style: none;
    padding: 0;

    li {
      padding: $spacing-sm 0;
      border-bottom: 1px solid $color-border;
    }
  }
}
</style>
