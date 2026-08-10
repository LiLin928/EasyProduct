<!-- src/views/videos/index.vue -->
<template>
  <div class="videos-page">
    <h1 class="videos-page__title">
      {{ t('site.videos.title') }}
    </h1>
    <AppLoading :loading="loading" />
    <template v-if="!loading">
      <AppEmpty v-if="!videos.length" />
      <div
        v-else
        class="videos-page__grid"
      >
        <div
          v-for="video in videos"
          :key="video.id"
          class="video-card"
          @click="playVideo(video)"
        >
          <div class="video-card__cover">
            <img
              :src="video.coverImage"
              :alt="video.title"
            >
            <div class="video-card__play">
              ▶
            </div>
            <span class="video-card__duration">{{ formatDuration(video.duration) }}</span>
          </div>
          <h3 class="video-card__title">
            {{ videoTitle(video) }}
          </h3>
          <div class="video-card__meta">
            <span>{{ video.viewCount }} {{ t('site.videos.views') }}</span>
          </div>
        </div>
      </div>
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
import { getVideoList } from '@/api/site/video'
import type { Video } from '@/types/site'

const { t, locale } = useI18n()

const videos = ref<Video[]>([])
const loading = ref(false)
const total = ref(0)
const query = ref({ pageIndex: 1, pageSize: 12 })

const videoTitle = (video: Video) =>
  locale.value === 'zh-CN' ? video.title : video.titleEn

const formatDuration = (seconds: number) => {
  const mins = Math.floor(seconds / 60)
  const secs = seconds % 60
  return `${mins}:${secs.toString().padStart(2, '0')}`
}

const playVideo = (video: Video) => {
  alert(`${videoTitle(video)}`)
}

onMounted(async () => {
  loading.value = true
  try {
    const result = await getVideoList(query.value)
    videos.value = result.list
    total.value = result.total
  } finally {
    loading.value = false
  }
})

function handlePageChange(page: number) {
  query.value.pageIndex = page
  loadVideos()
}

async function loadVideos() {
  loading.value = true
  try {
    const result = await getVideoList(query.value)
    videos.value = result.list
    total.value = result.total
  } finally {
    loading.value = false
  }
}
</script>

<style scoped lang="scss">
.videos-page {
  @include site-container;
  padding-top: $spacing-lg;
  padding-bottom: $spacing-xl;

  &__title {
    margin-bottom: $spacing-lg;
  }

  &__grid {
    display: grid;
    gap: $spacing-lg;

    @include mobile {
      grid-template-columns: 1fr;
    }

    @include tablet {
      grid-template-columns: repeat(2, 1fr);
    }

    @include desktop {
      grid-template-columns: repeat(3, 1fr);
    }
  }
}

.video-card {
  cursor: pointer;
  transition: transform 0.3s;

  &:hover {
    transform: translateY(-4px);
  }

  &__cover {
    position: relative;
    border-radius: $radius-md;
    overflow: hidden;

    img {
      width: 100%;
      height: 180px;
      object-fit: cover;
    }
  }

  &__play {
    position: absolute;
    top: 50%;
    left: 50%;
    transform: translate(-50%, -50%);
    width: 48px;
    height: 48px;
    background: rgba(0, 0, 0, 0.7);
    border-radius: 50%;
    display: flex;
    align-items: center;
    justify-content: center;
    color: #fff;
    font-size: 20px;
  }

  &__duration {
    position: absolute;
    bottom: $spacing-xs;
    right: $spacing-xs;
    padding: 2px 8px;
    background: rgba(0, 0, 0, 0.7);
    color: #fff;
    font-size: 12px;
    border-radius: $radius-sm;
  }

  &__title {
    margin: $spacing-sm 0;
    font-size: $font-size-base;
    @include ellipsis(2);
  }

  &__meta {
    font-size: $font-size-sm;
    color: $color-text-secondary;
  }
}
</style>
