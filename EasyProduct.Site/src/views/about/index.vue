<!-- src/views/about/index.vue -->
<template>
  <div class="about-page">
    <h1 class="about-page__title">
      {{ aboutTitle }}
    </h1>
    <AppLoading :loading="loading" />
    <article
      v-if="about"
      class="about-page__content"
    >
      <div class="about-page__body">
        {{ locale === 'zh-CN' ? about.content : about.contentEn }}
      </div>
    </article>
  </div>
</template>

<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { useI18n } from 'vue-i18n'
import { getAboutDetail } from '@/api/site/about'
import AppLoading from '@/components/common/AppLoading.vue'
import type { About } from '@/types/site'

const { locale } = useI18n()

const about = ref<About | null>(null)
const loading = ref(false)

const aboutTitle = computed(() =>
  about.value ? (locale.value === 'zh-CN' ? about.value.title : about.value.titleEn) : ''
)

onMounted(async () => {
  loading.value = true
  try {
    about.value = await getAboutDetail()
  } finally {
    loading.value = false
  }
})
</script>

<style scoped lang="scss">
.about-page {
  @include site-container;
  padding-top: $spacing-lg;
  padding-bottom: $spacing-xl;

  &__title {
    margin-bottom: $spacing-lg;
    text-align: center;
  }

  &__content {
    max-width: 800px;
    margin: 0 auto;
  }

  &__body {
    line-height: 1.8;
    color: $color-text;
    white-space: pre-wrap;
  }
}
</style>
