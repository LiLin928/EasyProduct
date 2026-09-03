<template>
  <div class="home-page">
    <!-- Hero carousel -->
    <section class="hero-section">
      <div
        class="carousel-container"
        @mouseenter="stopAutoplay"
        @mouseleave="startAutoplay"
      >
        <div
          class="carousel-wrapper"
          :style="{ transform: `translateX(-${currentIndex * 100}%)` }"
        >
          <div
            v-for="banner in banners"
            :key="banner.id"
            class="carousel-slide"
          >
            <div class="slide-image">
              <img
                :src="banner.imageUrl"
                :alt="localize(banner, 'title')"
              >
            </div>
            <div class="slide-content">
              <h1 class="slide-title">
                {{ localize(banner, 'title') }}
              </h1>
              <router-link
                v-if="banner.link"
                :to="banner.link"
                class="slide-btn"
              >
                {{ t('site.home.viewMore') }}
              </router-link>
            </div>
          </div>
        </div>

        <!-- Carousel controls -->
        <button
          v-if="banners.length > 1"
          class="carousel-btn prev"
          @click="prevSlide"
        >
          ‹
        </button>
        <button
          v-if="banners.length > 1"
          class="carousel-btn next"
          @click="nextSlide"
        >
          ›
        </button>

        <!-- Indicators -->
        <div
          v-if="banners.length > 1"
          class="carousel-indicators"
        >
          <button
            v-for="(_, index) in banners"
            :key="index"
            class="indicator"
            :class="{ active: currentIndex === index }"
            @click="goToSlide(index)"
          />
        </div>
      </div>
    </section>

    <!-- Feature section -->
    <section class="features-section">
      <div class="container">
        <div class="features-grid">
          <div class="feature-card">
            <div class="feature-icon">
              ⚡
            </div>
            <h3>{{ t('site.home.feature1Title') }}</h3>
            <p>{{ t('site.home.feature1Desc') }}</p>
          </div>
          <div class="feature-card">
            <div class="feature-icon">
              🌍
            </div>
            <h3>{{ t('site.home.feature2Title') }}</h3>
            <p>{{ t('site.home.feature2Desc') }}</p>
          </div>
          <div class="feature-card">
            <div class="feature-icon">
              🏆
            </div>
            <h3>{{ t('site.home.feature3Title') }}</h3>
            <p>{{ t('site.home.feature3Desc') }}</p>
          </div>
        </div>
      </div>
    </section>

    <!-- Product categories preview -->
    <section
      v-if="categories.length > 0"
      class="categories-section"
    >
      <div class="container">
        <h2 class="section-title">
          {{ t('site.home.categoriesTitle') }}
        </h2>
        <p class="section-subtitle">
          {{ t('site.home.categoriesSubtitle') }}
        </p>
        <div class="categories-grid">
          <div
            v-for="cat in categories"
            :key="cat.id"
            class="category-card"
            @click="router.push(`/products?categoryId=${cat.id}`)"
          >
            <div class="category-image">
              <span class="category-icon">📦</span>
            </div>
            <div class="category-content">
              <h3>{{ localize(cat, 'name') }}</h3>
              <span class="category-link">{{ t('common.viewAll') }} →</span>
            </div>
          </div>
        </div>
      </div>
    </section>

    <!-- Latest news -->
    <section
      v-if="news.length > 0"
      class="news-section"
    >
      <div class="container">
        <div class="section-header">
          <h2 class="section-title">
            {{ t('site.home.newsTitle') }}
          </h2>
          <router-link
            to="/news"
            class="view-all-link"
          >
            {{ t('site.home.moreNews') }} →
          </router-link>
        </div>
        <div class="news-grid">
          <article
            v-for="item in news"
            :key="item.id"
            class="news-card"
          >
            <router-link :to="`/news/${item.id}`">
              <h3>{{ localize(item, 'title') }}</h3>
              <time>{{ formatDate(item.publishTime) }}</time>
            </router-link>
          </article>
        </div>
      </div>
    </section>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted } from 'vue'
import { useRouter } from 'vue-router'
import { useI18n } from 'vue-i18n'
import { getBannerList, getNewsList } from '@/api/site/home'
import { getCategoryList } from '@/api/site/category'
import type { Banner, NewsItem, ProductCategory } from '@/types/site'
import dayjs from 'dayjs'

const router = useRouter()
const { t, locale } = useI18n()

const banners = ref<Banner[]>([])
const categories = ref<ProductCategory[]>([])
const news = ref<NewsItem[]>([])
const currentIndex = ref(0)
let autoplayTimer: ReturnType<typeof setInterval> | null = null

const localize = (item: Banner | ProductCategory | NewsItem, field: 'title' | 'name') => {
  const key = field as keyof typeof item
  const enKey = `${field}En` as keyof typeof item
  const value = locale.value === 'zh-CN' ? item[key] : item[enKey] || item[key]
  return String(value ?? '')
}

const formatDate = (date: string) => dayjs(date).format('YYYY-MM-DD')

const nextSlide = () => {
  currentIndex.value = (currentIndex.value + 1) % banners.value.length
}

const prevSlide = () => {
  currentIndex.value = (currentIndex.value - 1 + banners.value.length) % banners.value.length
}

const goToSlide = (index: number) => {
  currentIndex.value = index
}

const startAutoplay = () => {
  if (banners.value.length > 1) {
    autoplayTimer = setInterval(nextSlide, 5000)
  }
}

const stopAutoplay = () => {
  if (autoplayTimer) {
    clearInterval(autoplayTimer)
    autoplayTimer = null
  }
}

onMounted(async () => {
  try {
    const [bannerRes, categoryRes, newsRes] = await Promise.all([
      getBannerList(),
      getCategoryList(),
      getNewsList({ pageIndex: 1, pageSize: 4 }),
    ])
    banners.value = bannerRes
    categories.value = categoryRes.slice(0, 6)
    news.value = newsRes.list || []
    startAutoplay()
  } catch {
    // Failed to load home data
  }
})

onUnmounted(() => {
  stopAutoplay()
})
</script>

<style scoped lang="scss">
@use '@/assets/styles/variables' as *;
@use '@/assets/styles/mixins' as *;

// Hero 轮播区域
.hero-section {
  position: relative;
  height: 600px;
  overflow: hidden;

  @include compact {
    height: 400px;
  }
}

.carousel-container {
  position: relative;
  width: 100%;
  height: 100%;
}

.carousel-wrapper {
  display: flex;
  height: 100%;
  transition: transform 0.5s ease;
}

.carousel-slide {
  flex: 0 0 100%;
  position: relative;
}

.slide-image {
  position: absolute;
  inset: 0;

  img {
    width: 100%;
    height: 100%;
    object-fit: cover;
  }

  &::after {
    content: '';
    position: absolute;
    inset: 0;
    background: linear-gradient(to right, rgba(0, 0, 0, 0.6), rgba(0, 0, 0, 0.2));
  }
}

.slide-content {
  position: absolute;
  bottom: 120px;
  left: 0;
  right: 0;
  padding: 0 $spacing-xl;
  color: white;
  max-width: 1400px;
  margin: 0 auto;

  @include compact {
    bottom: 80px;
    padding: 0 $spacing-md;
  }
}

.slide-title {
  font-family: $font-display;
  font-size: 48px;
  font-weight: 700;
  margin-bottom: $spacing-lg;
  text-shadow: 0 2px 4px rgba(0, 0, 0, 0.3);

  @include compact {
    font-size: 28px;
    margin-bottom: $spacing-md;
  }
}

.slide-btn {
  display: inline-flex;
  padding: $spacing-sm $spacing-lg;
  background: $color-accent;
  color: white;
  font-weight: 600;
  border-radius: $radius-md;
  text-decoration: none;
  transition: all $transition-base;

  &:hover {
    background: $color-accent-dark;
    transform: translateY(-2px);
  }
}

.carousel-btn {
  position: absolute;
  top: 50%;
  transform: translateY(-50%);
  width: 48px;
  height: 48px;
  border-radius: 50%;
  background: rgba(255, 255, 255, 0.9);
  border: none;
  font-size: 24px;
  cursor: pointer;
  transition: all $transition-fast;
  z-index: 10;

  &:hover {
    background: white;
    transform: translateY(-50%) scale(1.1);
  }

  &.prev {
    left: $spacing-xl;
  }
  &.next {
    right: $spacing-xl;
  }

  @include compact {
    display: none;
  }
}

.carousel-indicators {
  position: absolute;
  bottom: $spacing-xl;
  left: 50%;
  transform: translateX(-50%);
  display: flex;
  gap: $spacing-sm;
  z-index: 10;

  @include compact {
    bottom: $spacing-md;
  }
}

.indicator {
  width: 12px;
  height: 12px;
  border-radius: 50%;
  background: rgba(255, 255, 255, 0.4);
  border: 2px solid transparent;
  cursor: pointer;
  transition: all $transition-base;

  &.active {
    background: white;
    transform: scale(1.3);
  }

  &:hover:not(.active) {
    background: rgba(255, 255, 255, 0.7);
  }
}

// 特色功能区域
.features-section {
  padding: $spacing-3xl 0;
  background: white;

  @include compact {
    padding: $spacing-xl 0;
  }
}

.container {
  max-width: 1400px;
  margin: 0 auto;
  padding: 0 $spacing-xl;

  @include compact {
    padding: 0 $spacing-md;
  }
}

.features-grid {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: $spacing-xl;

  @include compact {
    grid-template-columns: 1fr;
    gap: $spacing-md;
  }
}

.feature-card {
  text-align: center;
  padding: $spacing-xl;

  .feature-icon {
    font-size: 48px;
    margin-bottom: $spacing-md;
  }

  h3 {
    font-family: $font-heading;
    font-size: 24px;
    font-weight: 700;
    color: $color-text-primary;
    margin-bottom: $spacing-sm;
  }

  p {
    font-size: 16px;
    color: $color-text-secondary;
    line-height: 1.6;
  }
}

// 分类区域
.categories-section {
  padding: $spacing-3xl 0;
  background: $color-bg-secondary;

  @include compact {
    padding: $spacing-xl 0;
  }
}

.section-title {
  font-family: $font-display;
  font-size: 36px;
  font-weight: 700;
  color: $color-text-primary;
  text-align: center;
  margin-bottom: $spacing-sm;

  @include compact {
    font-size: 24px;
  }
}

.section-subtitle {
  text-align: center;
  font-size: 16px;
  color: $color-text-secondary;
  margin-bottom: $spacing-2xl;

  @include compact {
    font-size: 14px;
    margin-bottom: $spacing-lg;
  }
}

.categories-grid {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: $spacing-lg;

  @include tablet {
    grid-template-columns: repeat(2, 1fr);
  }

  @include mobile {
    grid-template-columns: 1fr;
  }
}

.category-card {
  background: white;
  border-radius: $radius-lg;
  overflow: hidden;
  box-shadow: $shadow-sm;
  cursor: pointer;
  transition: all $transition-base;

  &:hover {
    transform: translateY(-8px);
    box-shadow: $shadow-xl;
  }
}

.category-image {
  width: 100%;
  height: 180px;
  background: linear-gradient(135deg, $color-bg-tertiary, $color-bg-secondary);
  @include flex-center;
}

.category-icon {
  font-size: 64px;
  opacity: 0.6;
}

.category-content {
  padding: $spacing-md;

  h3 {
    font-size: 18px;
    font-weight: 600;
    color: $color-text-primary;
    margin-bottom: $spacing-xs;
  }
}

.category-link {
  color: $color-primary;
  font-size: 14px;
  font-weight: 600;
}

// 新闻区域
.news-section {
  padding: $spacing-3xl 0;
  background: white;

  @include compact {
    padding: $spacing-xl 0;
  }
}

.section-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: $spacing-xl;

  @include compact {
    flex-direction: column;
    gap: $spacing-sm;
    text-align: center;
  }

  .section-title {
    text-align: left;

    @include compact {
      text-align: center;
    }
  }
}

.view-all-link {
  color: $color-primary;
  font-weight: 600;
  text-decoration: none;

  &:hover {
    color: $color-primary-light;
  }
}

.news-grid {
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  gap: $spacing-lg;

  @include compact {
    grid-template-columns: 1fr;
  }
}

.news-card {
  a {
    display: block;
    padding: $spacing-md;
    background: $color-bg-secondary;
    border-radius: $radius-md;
    text-decoration: none;
    transition: all $transition-fast;

    &:hover {
      background: $color-bg-tertiary;
    }
  }

  h3 {
    font-size: 16px;
    font-weight: 600;
    color: $color-text-primary;
    margin-bottom: $spacing-xs;
    @include text-ellipsis(1);
  }

  time {
    font-size: 13px;
    color: $color-text-tertiary;
  }
}
</style>
