<!-- src/views/inquiry/index.vue -->
<template>
  <div class="inquiry-page">
    <h1 class="inquiry-page__title">
      {{ t('site.inquiry.title') }}
    </h1>

    <div
      v-if="inquiryStore.items.length === 0"
      class="inquiry-page__empty"
    >
      <AppEmpty :text="t('site.inquiry.empty')" />
      <router-link
        to="/products"
        class="inquiry-page__link"
      >
        {{ t('site.inquiry.browseProducts') }}
      </router-link>
    </div>

    <div
      v-else
      class="inquiry-page__content"
    >
      <div class="inquiry-page__items">
        <div
          v-for="item in inquiryStore.items"
          :key="item.productId"
          class="inquiry-item"
        >
          <div class="inquiry-item__name">
            {{ item.productName }}
          </div>
          <div class="inquiry-item__quantity">
            <button
              :disabled="item.quantity <= 1"
              @click="updateQuantity(item.productId, item.quantity - 1)"
            >
              -
            </button>
            <input
              v-model.number="item.quantity"
              type="number"
              min="1"
            >
            <button @click="updateQuantity(item.productId, item.quantity + 1)">
              +
            </button>
          </div>
          <div class="inquiry-item__unit">
            {{ item.unit }}
          </div>
          <button
            class="inquiry-item__remove"
            @click="inquiryStore.removeItem(item.productId)"
          >
            {{ t('site.inquiry.remove') }}
          </button>
        </div>
      </div>

      <form
        class="inquiry-page__form"
        @submit.prevent="handleSubmit"
      >
        <div class="inquiry-page__form-title">
          {{ t('site.inquiry.contactInfo') }}
        </div>
        <div class="inquiry-page__field">
          <label>{{ t('site.inquiry.companyName') }} *</label>
          <input
            v-model="form.companyName"
            type="text"
            required
          >
        </div>
        <div class="inquiry-page__field">
          <label>{{ t('site.inquiry.contactName') }} *</label>
          <input
            v-model="form.contactName"
            type="text"
            required
          >
        </div>
        <div class="inquiry-page__field">
          <label>{{ t('site.inquiry.phone') }} *</label>
          <input
            v-model="form.phone"
            type="tel"
            required
          >
        </div>
        <div class="inquiry-page__field">
          <label>{{ t('site.inquiry.email') }} *</label>
          <input
            v-model="form.email"
            type="email"
            required
          >
        </div>
        <button
          type="submit"
          class="inquiry-page__submit"
          :disabled="submitting"
        >
          {{ submitting ? t('site.inquiry.submitting') : t('site.inquiry.submit') }}
        </button>
      </form>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useI18n } from 'vue-i18n'
import { useInquiryStore } from '@/stores/inquiry'
import { submitInquiry } from '@/api/site/inquiry'
import AppEmpty from '@/components/common/AppEmpty.vue'

const { t } = useI18n()
const router = useRouter()
const inquiryStore = useInquiryStore()

const form = ref({
  companyName: '',
  contactName: '',
  phone: '',
  email: '',
})

const submitting = ref(false)

function updateQuantity(productId: string, quantity: number) {
  if (quantity >= 1) {
    inquiryStore.updateQuantity(productId, quantity)
  }
}

async function handleSubmit() {
  if (submitting.value) return

  submitting.value = true
  try {
    const result = await submitInquiry({
      ...form.value,
      items: inquiryStore.items,
    })

    inquiryStore.clearItems()
    router.push(`/inquiry/${result.id}`)
  } catch {
    // 提交失败，用户会看到按钮重新启用
  } finally {
    submitting.value = false
  }
}
</script>

<style scoped lang="scss">
.inquiry-page {
  @include site-container;
  padding-top: $spacing-lg;
  padding-bottom: $spacing-xl;

  &__title {
    margin-bottom: $spacing-lg;
  }

  &__empty {
    text-align: center;
    padding: $spacing-xl;
  }

  &__link {
    display: inline-block;
    margin-top: $spacing-md;
    padding: $spacing-sm $spacing-lg;
    border: 1px solid $color-primary;
    border-radius: $radius-sm;
    color: $color-primary;
    text-decoration: none;

    &:hover {
      background: $color-primary;
      color: #fff;
    }
  }

  &__content {
    display: grid;
    gap: $spacing-lg;

    @include desktop {
      grid-template-columns: 1fr 400px;
    }
  }

  &__items {
    background: $color-bg-card;
    padding: $spacing-md;
    border-radius: $radius-md;
  }

  &__form {
    background: $color-bg-card;
    padding: $spacing-md;
    border-radius: $radius-md;

    &-title {
      font-size: $font-size-lg;
      font-weight: 600;
      margin-bottom: $spacing-md;
    }
  }

  &__field {
    margin-bottom: $spacing-md;

    label {
      display: block;
      margin-bottom: $spacing-xs;
      font-weight: 500;
    }

    input {
      width: 100%;
      padding: $spacing-sm;
      border: 1px solid $color-border;
      border-radius: $radius-sm;
    }
  }

  &__submit {
    width: 100%;
    padding: $spacing-md;
    border: none;
    border-radius: $radius-sm;
    background: $color-primary;
    color: #fff;
    font-size: $font-size-lg;
    cursor: pointer;

    &:disabled {
      opacity: 0.5;
      cursor: not-allowed;
    }

    &:not(:disabled):hover {
      opacity: 0.9;
    }
  }
}

.inquiry-item {
  display: grid;
  grid-template-columns: 1fr auto auto auto;
  gap: $spacing-md;
  align-items: center;
  padding: $spacing-md;
  border-bottom: 1px solid $color-border;

  &:last-child {
    border-bottom: none;
  }

  &__name {
    font-weight: 500;
  }

  &__quantity {
    display: flex;
    align-items: center;
    gap: $spacing-xs;

    button {
      padding: $spacing-xs $spacing-sm;
      border: 1px solid $color-border;
      background: $color-bg;
      cursor: pointer;

      &:disabled {
        opacity: 0.5;
        cursor: not-allowed;
      }
    }

    input {
      width: 60px;
      text-align: center;
      padding: $spacing-xs;
      border: 1px solid $color-border;
    }
  }

  &__unit {
    color: $color-text-secondary;
  }

  &__remove {
    padding: $spacing-xs $spacing-sm;
    border: 1px solid $color-border;
    border-radius: $radius-sm;
    background: $color-bg;
    cursor: pointer;

    &:hover {
      color: #f56c6c;
      border-color: #f56c6c;
    }
  }
}
</style>