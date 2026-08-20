<!-- src/views/announcement/index.vue -->
<template>
  <div class="announcement-page">
    <h1 class="announcement-page__title">
      {{ t('site.announcement.title') }}
    </h1>
    <AppLoading :loading="loading" />
    <template v-if="!loading">
      <AppEmpty v-if="!announcementList.length" />
      <ul
        v-else
        class="announcement-page__list"
      >
        <li
          v-for="item in announcementList"
          :key="item.id"
          class="announcement-item"
        >
          <router-link
            :to="`/announcement/${item.id}`"
            class="announcement-item__link"
          >
            <div class="announcement-item__content">
              <div class="announcement-item__header">
                <h2 class="announcement-item__title">
                  {{ announcementTitle(item) }}
                </h2>
                <span
                  v-if="item.isTop"
                  class="announcement-item__top"
                >{{ t('site.announcement.top') }}</span>
                <span
                  :class="['announcement-item__level', `announcement-item__level--${item.level}`]"
                >
                  {{ t(`site.announcement.level.${item.level}`) }}
                </span>
              </div>
              <p class="announcement-item__summary">
                {{ announcementSummary(item) }}
              </p>
              <div class="announcement-item__meta">
                <span>{{ dayjs(item.publishTime).format('YYYY-MM-DD') }}</span>
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
import { getPublicAnnouncementList } from '@/api/site/announcement'
import type { PublicAnnouncement } from '@/types/announcement'

const { t, locale } = useI18n()

const announcementList = ref<PublicAnnouncement[]>([])
const loading = ref(false)
const total = ref(0)
const query = ref({ pageIndex: 1, pageSize: 10 })

const announcementTitle = (item: PublicAnnouncement) =>
  locale.value === 'zh-CN' ? item.title : item.titleEn

const announcementSummary = (item: PublicAnnouncement) =>
  locale.value === 'zh-CN' ? item.summary : item.summaryEn

onMounted(async () => {
  await loadAnnouncements()
})

async function loadAnnouncements() {
  loading.value = true
  try {
    const result = await getPublicAnnouncementList(query.value)
    announcementList.value = result.list
    total.value = result.total
  } finally {
    loading.value = false
  }
}

function handlePageChange(page: number) {
  query.value.pageIndex = page
  loadAnnouncements()
}
</script>

<style scoped lang="scss">
.announcement-page {
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

.announcement-item {
  margin-bottom: $spacing-lg;
  border-bottom: 1px solid $color-border;

  &__link {
    display: block;
    padding-bottom: $spacing-lg;
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
    flex: 1;
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