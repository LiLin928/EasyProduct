<!-- src/views/announcement/detail.vue -->
<template>
  <div class="announcement-detail">
    <AppLoading :loading="loading" />
    <article
      v-if="announcement"
      class="announcement-detail__content"
    >
      <h1 class="announcement-detail__title">
        {{ announcementTitle }}
      </h1>
      <div class="announcement-detail__meta">
        <span>{{ dayjs(announcement.publishTime).format('YYYY-MM-DD') }}</span>
        <span
          v-if="announcement.isTop"
          class="announcement-detail__top"
        >{{ t('site.announcement.top') }}</span>
        <span
          :class="['announcement-detail__level', `announcement-detail__level--${announcement.level}`]"
        >
          {{ t(`site.announcement.level.${announcement.level}`) }}
        </span>
      </div>
      <div class="announcement-detail__body">
        {{ locale === 'zh-CN' ? announcement.content : announcement.contentEn }}
      </div>
      <div
        v-if="announcement.attachments && announcement.attachments.length > 0"
        class="announcement-detail__attachments"
      >
        <h3 class="announcement-detail__attachments-title">
          {{ t('site.announcement.downloadAttachment') }}
        </h3>
        <ul class="announcement-detail__attachments-list">
          <li
            v-for="(file, index) in announcement.attachments"
            :key="index"
            class="attachment-item"
          >
            <a
              :href="file.url"
              :download="file.name"
              class="attachment-item__link"
            >
              <span class="attachment-item__name">{{ file.name }}</span>
              <span class="attachment-item__size">({{ formatFileSize(file.size) }})</span>
            </a>
          </li>
        </ul>
      </div>
    </article>
  </div>
</template>

<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useI18n } from 'vue-i18n'
import dayjs from 'dayjs'
import { getPublicAnnouncementById } from '@/api/site/announcement'
import AppLoading from '@/components/common/AppLoading.vue'
import type { PublicAnnouncement } from '@/types/announcement'

const { t, locale } = useI18n()
const route = useRoute()
const router = useRouter()

const announcement = ref<PublicAnnouncement | null>(null)
const loading = ref(false)

const announcementTitle = computed(() =>
  announcement.value ? (locale.value === 'zh-CN' ? announcement.value.title : announcement.value.titleEn) : ''
)

function formatFileSize(bytes: number): string {
  if (bytes < 1024) {
    return `${bytes} B`
  }
  if (bytes < 1024 * 1024) {
    return `${(bytes / 1024).toFixed(2)} KB`
  }
  return `${(bytes / (1024 * 1024)).toFixed(2)} MB`
}

onMounted(async () => {
  loading.value = true
  try {
    announcement.value = await getPublicAnnouncementById(route.params.id as string)
  } catch {
    router.push('/announcement')
  } finally {
    loading.value = false
  }
})
</script>

<style scoped lang="scss">
.announcement-detail {
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
    align-items: center;
    gap: $spacing-md;
    margin-bottom: $spacing-lg;
    font-size: $font-size-sm;
    color: $color-text-secondary;
  }

  &__top {
    padding: 2px 8px;
    background: $color-primary;
    color: #fff;
    font-size: 12px;
    border-radius: $radius-sm;
  }

  &__level {
    padding: 2px 8px;
    font-size: 12px;
    border-radius: $radius-sm;

    &--normal {
      background: $color-info;
      color: #fff;
    }

    &--important {
      background: $color-warning;
      color: #fff;
    }

    &--urgent {
      background: $color-danger;
      color: #fff;
    }
  }

  &__body {
    line-height: 1.8;
    color: $color-text;
  }

  &__attachments {
    margin-top: $spacing-xl;
    padding-top: $spacing-lg;
    border-top: 1px solid $color-border;
  }

  &__attachments-title {
    margin: 0 0 $spacing-md;
    font-size: $font-size-lg;
  }

  &__attachments-list {
    list-style: none;
    padding: 0;
    margin: 0;
  }
}

.attachment-item {
  margin-bottom: $spacing-sm;

  &__link {
    display: inline-flex;
    align-items: center;
    gap: $spacing-sm;
    padding: $spacing-sm $spacing-md;
    background: $color-bg-light;
    border-radius: $radius-sm;
    transition: background 0.3s;

    &:hover {
      background: $color-bg-hover;
    }
  }

  &__name {
    color: $color-primary;
    font-weight: 500;
  }

  &__size {
    font-size: $font-size-sm;
    color: $color-text-secondary;
  }
}
</style>