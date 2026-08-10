<!-- src/views/contact/index.vue -->
<template>
  <div class="contact-page">
    <h1 class="contact-page__title">
      {{ t('site.contact.title') }}
    </h1>
    <div class="contact-page__content">
      <div class="contact-page__info">
        <h2 class="contact-page__subtitle">
          {{ t('site.contact.info') }}
        </h2>
        <div
          v-if="contactInfo"
          class="contact-page__details"
        >
          <div class="contact-page__item">
            <label>{{ t('site.contact.address') }}:</label>
            <span>{{ locale === 'zh-CN' ? contactInfo.address : contactInfo.addressEn }}</span>
          </div>
          <div class="contact-page__item">
            <label>{{ t('site.contact.phone') }}:</label>
            <span>{{ contactInfo.phone }}</span>
          </div>
          <div class="contact-page__item">
            <label>{{ t('site.contact.email') }}:</label>
            <span>{{ contactInfo.email }}</span>
          </div>
          <div class="contact-page__item">
            <label>{{ t('site.contact.workingHours') }}:</label>
            <span>{{ locale === 'zh-CN' ? contactInfo.workingHours : contactInfo.workingHoursEn }}</span>
          </div>
        </div>
      </div>
      <form
        class="contact-page__form"
        @submit.prevent="handleSubmit"
      >
        <h2 class="contact-page__subtitle">
          {{ t('site.contact.form') }}
        </h2>
        <div class="contact-page__field">
          <label>{{ t('site.contact.companyName') }} *</label>
          <input
            v-model="form.companyName"
            type="text"
            required
          >
        </div>
        <div class="contact-page__field">
          <label>{{ t('site.contact.contactName') }} *</label>
          <input
            v-model="form.contactName"
            type="text"
            required
          >
        </div>
        <div class="contact-page__field">
          <label>{{ t('site.contact.phone') }} *</label>
          <input
            v-model="form.phone"
            type="tel"
            required
          >
        </div>
        <div class="contact-page__field">
          <label>{{ t('site.contact.email') }} *</label>
          <input
            v-model="form.email"
            type="email"
            required
          >
        </div>
        <div class="contact-page__field">
          <label>{{ t('site.contact.content') }} *</label>
          <textarea
            v-model="form.content"
            rows="5"
            required
          />
        </div>
        <button
          type="submit"
          class="contact-page__submit"
          :disabled="submitting"
        >
          {{ submitting ? t('site.contact.submitting') : t('site.contact.submit') }}
        </button>
      </form>
    </div>
  </div>
</template>

<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { useI18n } from 'vue-i18n'
import { getContactInfo } from '@/api/site/about'
import { submitContact } from '@/api/site/contact'
import type { ContactInfo } from '@/types/site'

const { t, locale } = useI18n()

const contactInfo = ref<ContactInfo | null>(null)
const submitting = ref(false)
const form = ref({
  companyName: '',
  contactName: '',
  phone: '',
  email: '',
  content: '',
})

onMounted(async () => {
  contactInfo.value = await getContactInfo()
})

async function handleSubmit() {
  if (submitting.value) return
  submitting.value = true
  try {
    await submitContact(form.value)
    alert(t('site.contact.success'))
    form.value = { companyName: '', contactName: '', phone: '', email: '', content: '' }
  } catch {
    // 提交失败
  } finally {
    submitting.value = false
  }
}
</script>

<style scoped lang="scss">
.contact-page {
  @include site-container;
  padding-top: $spacing-lg;
  padding-bottom: $spacing-xl;

  &__title { margin-bottom: $spacing-lg; text-align: center; }
  &__content { display: grid; gap: $spacing-xl; @include desktop { grid-template-columns: 1fr 1fr; } }
  &__subtitle { margin-bottom: $spacing-md; font-size: $font-size-lg; }
  &__info { background: $color-bg-card; padding: $spacing-lg; border-radius: $radius-md; }
  &__details { display: flex; flex-direction: column; gap: $spacing-md; }
  &__item { display: flex; gap: $spacing-sm; label { font-weight: 600; min-width: 80px; } }
  &__form { background: $color-bg-card; padding: $spacing-lg; border-radius: $radius-md; }
  &__field { margin-bottom: $spacing-md; label { display: block; margin-bottom: $spacing-xs; font-weight: 500; }
    input, textarea { width: 100%; padding: $spacing-sm; border: 1px solid $color-border; border-radius: $radius-sm; }
    textarea { resize: vertical; } }
  &__submit { width: 100%; padding: $spacing-md; border: none; border-radius: $radius-sm; background: $color-primary;
    color: #fff; font-size: $font-size-lg; cursor: pointer;
    &:disabled { opacity: 0.5; cursor: not-allowed; }
    &:not(:disabled):hover { opacity: 0.9; } }
}
</style>
