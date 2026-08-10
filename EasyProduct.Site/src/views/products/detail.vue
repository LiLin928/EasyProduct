<!-- src/views/products/detail.vue -->
<template>
  <div class="product-detail">
    <AppLoading :loading="loading" />
    <div
      v-if="product"
      class="product-detail__content"
    >
      <div class="product-detail__gallery">
        <img
          :src="currentImage"
          :alt="productName"
          class="product-detail__main-image"
        >
        <div
          v-if="product.images.length > 1"
          class="product-detail__thumbnails"
        >
          <button
            v-for="(img, i) in product.images"
            :key="i"
            class="product-detail__thumb"
            :class="{ 'product-detail__thumb--active': currentImage === img }"
            @click="currentImage = img"
          >
            <img
              :src="img"
              alt=""
            >
          </button>
        </div>
      </div>
      <div class="product-detail__info">
        <h1 class="product-detail__title">
          {{ productName }}
        </h1>
        <p class="product-detail__summary">
          {{ product.summary }}
        </p>
        <div class="product-detail__price-box">
          <span class="product-detail__price">¥{{ product.price.toFixed(2) }}</span>
          <span class="product-detail__unit">/{{ product.unit }}</span>
        </div>
        <div class="product-detail__quantity">
          <label>{{ t('site.products.quantity') }}:</label>
          <input
            v-model.number="quantity"
            type="number"
            min="1"
          >
        </div>
        <button
          class="product-detail__inquiry-btn"
          @click="handleAddToInquiry"
        >
          {{ t('site.products.addToInquiry') }}
        </button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useI18n } from 'vue-i18n'
import { useInquiryStore } from '@/stores/inquiry'
import { getProductDetail } from '@/api/site/product'
import AppLoading from '@/components/common/AppLoading.vue'
import type { Product } from '@/types/site'

const { t, locale } = useI18n()
const route = useRoute()
const router = useRouter()
const inquiryStore = useInquiryStore()

const product = ref<Product | null>(null)
const loading = ref(false)
const currentImage = ref('')
const quantity = ref(1)

const productName = computed(() =>
  product.value ? (locale.value === 'zh-CN' ? product.value.name : product.value.nameEn) : ''
)

onMounted(async () => {
  loading.value = true
  try {
    product.value = await getProductDetail(route.params.id as string)
    currentImage.value = product.value.coverImage
  } catch {
    router.push('/products')
  } finally {
    loading.value = false
  }
})

function handleAddToInquiry() {
  if (!product.value) return
  inquiryStore.addItem({
    productId: product.value.id,
    productName: productName.value,
    quantity: quantity.value,
    unit: product.value.unit,
  })
  router.push('/inquiry')
}
</script>

<style scoped lang="scss">
.product-detail {
  @include site-container;
  padding-top: $spacing-lg;
  padding-bottom: $spacing-xl;

  &__content {
    display: grid;
    gap: $spacing-lg;

    @include desktop {
      grid-template-columns: 1fr 1fr;
    }
  }

  &__gallery {
    display: flex;
    flex-direction: column;
    gap: $spacing-md;
  }

  &__main-image {
    width: 100%;
    border-radius: $radius-md;
  }

  &__thumbnails {
    display: flex;
    gap: $spacing-sm;
    overflow-x: auto;
  }

  &__thumb {
    flex-shrink: 0;
    width: 80px;
    height: 80px;
    border: 2px solid transparent;
    border-radius: $radius-sm;
    overflow: hidden;
    cursor: pointer;
    background: $color-bg;
    padding: 0;

    &--active {
      border-color: $color-primary;
    }

    img {
      width: 100%;
      height: 100%;
      object-fit: cover;
    }
  }

  &__info {
    display: flex;
    flex-direction: column;
    gap: $spacing-md;
  }

  &__title {
    margin: 0;
    font-size: 24px;
  }

  &__summary {
    color: $color-text-secondary;
  }

  &__price-box {
    padding: $spacing-md;
    background: $color-bg-soft;
    border-radius: $radius-sm;
  }

  &__price {
    font-size: 24px;
    font-weight: 600;
    color: $color-primary;
  }

  &__unit {
    color: $color-text-secondary;
  }

  &__quantity {
    display: flex;
    align-items: center;
    gap: $spacing-sm;

    input {
      width: 80px;
      padding: $spacing-xs $spacing-sm;
      border: 1px solid $color-border;
      border-radius: $radius-sm;
    }
  }

  &__inquiry-btn {
    padding: $spacing-md $spacing-lg;
    border: none;
    border-radius: $radius-sm;
    background: $color-primary;
    color: #fff;
    font-size: $font-size-lg;
    cursor: pointer;

    &:hover {
      opacity: 0.9;
    }
  }
}
</style>