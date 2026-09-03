<template>
  <div class="videos-page">
    <div class="videos-container">
      <!-- Header -->
      <div class="videos-header">
        <h1>{{ t('site.videos.title') }}</h1>
        <p>{{ t('site.videos.subtitle') }}</p>
      </div>

      <div class="videos-content">
        <!-- Category filter sidebar (desktop) -->
        <aside class="videos-sidebar">
          <div class="filter-section">
            <h3 class="filter-title">
              {{ t('site.videos.filter') }}
            </h3>
            <ul class="category-list">
              <li>
                <button
                  class="category-btn"
                  :class="{ active: !selectedCategory }"
                  @click="selectCategory(undefined)"
                >
                  {{ t('site.videos.all') }}
                </button>
              </li>
              <li
                v-for="cat in categories"
                :key="cat.id"
              >
                <button
                  class="category-btn"
                  :class="{ active: selectedCategory === cat.id }"
                  @click="selectCategory(cat.id)"
                >
                  {{ cat.name }}
                </button>
              </li>
            </ul>
          </div>
        </aside>

        <!-- Main content area -->
        <main class="videos-main">
          <!-- Category selection (mobile) -->
          <div class="category-tabs-mobile">
            <button
              class="tab-btn"
              :class="{ active: !selectedCategory }"
              @click="selectCategory(undefined)"
            >
              {{ t('site.videos.all') }}
            </button>
            <button
              v-for="cat in categories"
              :key="cat.id"
              class="tab-btn"
              :class="{ active: selectedCategory === cat.id }"
              @click="selectCategory(cat.id)"
            >
              {{ cat.name }}
            </button>
          </div>

          <!-- Search -->
          <div class="videos-toolbar">
            <input
              v-model="keyword"
              type="search"
              class="search-input"
              :placeholder="t('site.videos.searchPlaceholder')"
            >
          </div>

          <!-- Loading -->
          <div
            v-if="loading"
            class="loading"
          >
            {{ t('common.loading') }}
          </div>
          
          <!-- Empty -->
          <div
            v-else-if="videos.length === 0"
            class="empty"
          >
            {{ t('common.empty') }}
          </div>

          <!-- Video Grid -->
          <div
            v-else
            class="videos-grid"
          >
            <article
              v-for="video in videos"
              :key="video.id"
              class="video-card"
            >
              <button
                type="button"
                class="video-cover"
                @click="openVideo(video)"
              >
                <img
                  :src="video.coverImage"
                  :alt="videoTitle(video)"
                  loading="lazy"
                >
                <span
                  class="play-icon"
                  aria-hidden="true"
                >▶</span>
                <span
                  v-if="video.duration"
                  class="duration"
                >{{ formatDuration(video.duration) }}</span>
              </button>
              <div class="video-body">
                <h3>{{ videoTitle(video) }}</h3>
                <div class="video-meta">
                  <span>{{ video.viewCount }} {{ t('site.videos.views') }}</span>
                </div>
              </div>
            </article>
          </div>

          <!-- Pagination -->
          <div
            v-if="totalPages > 1"
            class="pagination"
          >
            <button
              class="page-btn"
              :disabled="query.pageIndex === 1"
              @click="goToPage(query.pageIndex - 1)"
            >
              {{ t('common.previous') }}
            </button>
            <span class="page-info">{{ query.pageIndex }} / {{ totalPages }}</span>
            <button
              class="page-btn"
              :disabled="query.pageIndex === totalPages"
              @click="goToPage(query.pageIndex + 1)"
            >
              {{ t('common.next') }}
            </button>
          </div>
        </main>
      </div>
    </div>

    <!-- Video Modal -->
    <div
      v-if="activeVideo"
      class="video-modal"
      @click.self="closeVideo"
    >
      <div
        class="video-modal__panel"
        role="dialog"
        aria-modal="true"
      >
        <div class="video-modal__header">
          <h3>{{ videoTitle(activeVideo) }}</h3>
          <button
            type="button"
            class="close-btn"
            @click="closeVideo"
          >
            ×
          </button>
        </div>
        <div class="video-modal__frame">
          <iframe
            :src="activeVideo.videoUrl"
            title="video player"
            allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture"
            allowfullscreen
          />
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, onMounted, ref, watch } from 'vue'
import { useI18n } from 'vue-i18n'
import { getVideoList, getVideoCategoryList } from '@/api/site/video'
import type { Video, VideoCategory } from '@/types/site'

const { t, locale } = useI18n()

const videos = ref<Video[]>([])
const categories = ref<VideoCategory[]>([])
const loading = ref(true)
const keyword = ref('')
const activeVideo = ref<Video | null>(null)
const selectedCategory = ref<string | undefined>(undefined)
const total = ref(0)
const query = ref({
  pageIndex: 1,
  pageSize: 9
})
let searchTimer: ReturnType<typeof setTimeout> | null = null

const totalPages = computed(() => Math.max(1, Math.ceil(total.value / query.value.pageSize)))

const videoTitle = (video: Video) =>
  locale.value === 'zh-CN' ? video.title : video.titleEn

const formatDuration = (seconds: number) => {
  const mins = Math.floor(seconds / 60)
  const secs = seconds % 60
  return mins + ':' + secs.toString().padStart(2, '0')
}

const openVideo = (video: Video) => {
  activeVideo.value = video
}

const closeVideo = () => {
  activeVideo.value = null
}



const loadCategories = async () => {
  try {
    const res = await getVideoCategoryList()
    categories.value = res || []
  } catch {
    categories.value = []
  }
}

const loadVideos = async () => {
  try {
    loading.value = true
    const res = await getVideoList({
      pageIndex: query.value.pageIndex,
      pageSize: query.value.pageSize,
      categoryId: selectedCategory.value,
      keyword: keyword.value?.trim() || undefined
    })
    videos.value = res.list || []
    total.value = res.total || 0
  } catch {
    videos.value = []
    total.value = 0
  } finally {
    loading.value = false
  }
}
const selectCategory = (id: string | undefined) => {

  selectedCategory.value = id
  query.value.pageIndex = 1
  loadVideos()
}

const goToPage = (page: number) => {
  query.value.pageIndex = page
  loadVideos()
}

watch(keyword, () => {
  if (searchTimer) clearTimeout(searchTimer)
  searchTimer = setTimeout(() => {
    query.value.pageIndex = 1
    loadVideos()
  }, 300)
})

onMounted(() => {
  loadCategories()
  loadVideos()
})
</script>

<style scoped lang="scss">
.videos-page {
  min-height: 100vh;
  background: $color-bg-secondary;
  padding-top: 80px;
  padding-bottom: $spacing-xl;

  @include compact {
    padding-top: 56px;
  }
}

.videos-container {
  max-width: 1400px;
  margin: 0 auto;
  padding: $spacing-xl;

  @include compact {
    padding: $spacing-md;
  }
}

.videos-header {
  text-align: center;
  margin-bottom: $spacing-2xl;

  h1 {
    font-size: 36px;
    font-weight: 700;
    color: $color-text-primary;
    margin-bottom: $spacing-sm;

    @include compact {
      font-size: 24px;
    }
  }

  p {
    font-size: 16px;
    color: $color-text-secondary;

    @include compact {
      font-size: 14px;
    }
  }
}

.videos-content {
  display: grid;
  grid-template-columns: 220px 1fr;
  gap: $spacing-xl;

  @include compact {
    grid-template-columns: 1fr;
  }
}

.videos-sidebar {
  @include compact {
    display: none;
  }
}

.filter-section {
  background: white;
  padding: $spacing-md;
  border-radius: $radius-lg;
  box-shadow: $shadow-sm;
}

.filter-title {
  font-size: 16px;
  font-weight: 600;
  color: $color-text-primary;
  margin-bottom: $spacing-md;
}

.category-list {
  list-style: none;
  padding: 0;
  margin: 0;

  li {
    margin-bottom: $spacing-xs;
  }
}

.category-btn {
  width: 100%;
  padding: $spacing-sm $spacing-md;
  background: transparent;
  border: none;
  border-radius: $radius-sm;
  text-align: left;
  cursor: pointer;
  transition: all $transition-fast;
  color: $color-text-secondary;
  font-size: 14px;

  &:hover {
    background: $color-bg-secondary;
    color: $color-primary;
  }

  &.active {
    background: $color-primary;
    color: white;
    font-weight: 600;
  }
}

.category-tabs-mobile {
  display: none;
  
  @include compact {
    display: flex;
    gap: $spacing-sm;
    overflow-x: auto;
    margin-bottom: $spacing-md;
    padding-bottom: 2px;
    -webkit-overflow-scrolling: touch;
    scrollbar-width: none;

    &::-webkit-scrollbar {
      display: none;
    }
    
    .tab-btn {
      flex-shrink: 0;
      white-space: nowrap;
      padding: 8px 16px;
      border-radius: 999px;
      border: 1px solid $color-border;
      background: white;
      cursor: pointer;
      font-size: 14px;
      color: $color-text-secondary;
      transition: all $transition-fast;
      
      &.active {
        background: $color-primary;
        color: white;
        border-color: $color-primary;
      }
    }
  }
}

.videos-main {
  min-width: 0;
}

.videos-toolbar {
  margin-bottom: $spacing-lg;

  @include compact {
    margin-bottom: $spacing-md;
  }
}

.search-input {
  width: 100%;
  padding: $spacing-sm $spacing-md;
  border: 1px solid $color-border;
  border-radius: $radius-md;
  font-size: 16px;
  transition: all $transition-fast;
  box-sizing: border-box;

  &:focus {
    outline: none;
    border-color: $color-primary;
    box-shadow: 0 0 0 3px rgba($color-primary, 0.1);
  }
}

.loading,
.empty {
  text-align: center;
  padding: $spacing-3xl;
  color: $color-text-tertiary;
  font-size: 16px;
}

.videos-grid {
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

.video-card {
  background: white;
  border-radius: $radius-lg;
  overflow: hidden;
  box-shadow: $shadow-sm;
  transition: transform $transition-base;

  &:hover {
    transform: translateY(-4px);
    box-shadow: $shadow-md;
  }
}

.video-cover {
  position: relative;
  display: block;
  width: 100%;
  aspect-ratio: 16 / 9;
  border: 0;
  padding: 0;
  cursor: pointer;
  background: #000;

  img {
    width: 100%;
    height: 100%;
    object-fit: cover;
    opacity: 0.9;
  }

  .play-icon {
    position: absolute;
    inset: 0;
    margin: auto;
    width: 56px;
    height: 56px;
    border-radius: 50%;
    background: rgba($color-primary, 0.9);
    color: white;
    display: grid;
    place-items: center;
    font-size: 18px;
    transition: transform $transition-base;
  }

  &:hover .play-icon {
    transform: scale(1.1);
  }

  .duration {
    position: absolute;
    right: 10px;
    bottom: 10px;
    background: rgba(0, 0, 0, 0.7);
    color: white;
    font-size: 12px;
    padding: 2px 6px;
    border-radius: 4px;
  }
}

.video-body {
  padding: $spacing-md;

  h3 {
    margin: 0 0 8px;
    font-size: 16px;
    color: $color-text-primary;
    line-height: 1.4;
  }

  .video-meta {
    font-size: 13px;
    color: $color-text-tertiary;
  }
}

.pagination {
  display: flex;
  justify-content: center;
  align-items: center;
  gap: $spacing-md;
  margin-top: $spacing-2xl;
  flex-wrap: wrap;

  @include compact {
    margin-top: $spacing-lg;
    gap: $spacing-sm;
  }
}

.page-btn {
  padding: $spacing-sm $spacing-lg;
  background: white;
  border: 1px solid $color-border;
  border-radius: $radius-md;
  cursor: pointer;
  transition: all $transition-fast;
  font-size: 14px;
  color: $color-text-secondary;
  min-height: 40px;
  min-width: 88px;

  @include compact {
    min-width: 72px;
    padding: 10px 14px;
  }

  &:hover:not(:disabled) {
    background: $color-primary;
    color: white;
    border-color: $color-primary;
  }

  &:disabled {
    opacity: 0.5;
    cursor: not-allowed;
  }
}

.page-info {
  font-size: 14px;
  color: $color-text-secondary;
  min-width: 56px;
  text-align: center;
}

.video-modal {
  position: fixed;
  inset: 0;
  z-index: 2000;
  background: rgba(0, 0, 0, 0.75);
  display: grid;
  place-items: center;
  padding: 16px;
}

.video-modal__panel {
  width: min(960px, 100%);
  background: white;
  border-radius: $radius-lg;
  overflow: hidden;
  box-shadow: $shadow-xl;
}

.video-modal__header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 12px 16px;
  border-bottom: 1px solid $color-border;

  h3 {
    margin: 0;
    font-size: 16px;
    color: $color-text-primary;
  }

  .close-btn {
    border: 0;
    background: transparent;
    font-size: 24px;
    cursor: pointer;
    line-height: 1;
    color: $color-text-secondary;
    padding: 0;
    width: 32px;
    height: 32px;
    display: flex;
    align-items: center;
    justify-content: center;
    border-radius: $radius-sm;

    &:hover {
      background: $color-bg-secondary;
      color: $color-text-primary;
    }
  }
}

.video-modal__frame {
  position: relative;
  width: 100%;
  aspect-ratio: 16 / 9;
  background: #000;

  iframe {
    width: 100%;
    height: 100%;
    border: 0;
  }
}
</style>
