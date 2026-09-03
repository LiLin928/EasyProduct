<template>
  <div
    class="product-card"
    @click="router.push(`/products/${product.id}`)"
  >
    <div class="product-image">
      <img
        v-if="product.coverImage"
        :src="product.coverImage"
        :alt="productName"
        loading="lazy"
      >
      <div
        v-else
        class="image-placeholder"
      >
        <span>📦</span>
      </div>
    </div>

    <div class="product-content">
      <h3 class="product-title">
        {{ productName }}
      </h3>
      <p class="product-summary">
        {{ productSummary }}
      </p>

      <div class="product-footer">
        <div class="product-price">
          <span class="currency">¥</span>
          <span class="amount">{{ product.price.toFixed(2) }}</span>
          <span class="unit">/{{ product.unit }}</span>
        </div>
        <button
          class="add-btn"
          @click.stop="addToInquiry"
        >
          +
        </button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { useRouter } from 'vue-router'
import { useI18n } from 'vue-i18n'
import { useInquiryStore } from '@/stores/inquiry'
import type { Product } from '@/types/site'

const props = defineProps<{ product: Product }>()
const router = useRouter()
const { locale } = useI18n()
const inquiryStore = useInquiryStore()

const productName = computed(() =>
  locale.value === 'zh-CN' ? props.product.name : props.product.nameEn
)

const productSummary = computed(() =>
  locale.value === 'zh-CN' ? props.product.summary : props.product.summaryEn
)

const addToInquiry = () => {
  inquiryStore.addItem({
    productId: props.product.id,
    productName: productName.value,
    quantity: 1,
    unit: props.product.unit,
  })
}
</script>

<style scoped lang="scss">
@use '@/assets/styles/variables' as *;
@use '@/assets/styles/mixins' as *;

.product-card {
  background: white;
  border-radius: $radius-lg;
  overflow: hidden;
  box-shadow: $shadow-sm;
  transition: all $transition-base;
  cursor: pointer;
  border: 1px solid transparent;

  &:hover {
    transform: translateY(-8px);
    box-shadow: $shadow-xl;
    border-color: $color-primary;
  }

  @include compact {
    &:hover {
      transform: translateY(-4px);
    }
  }
}

.product-image {
  width: 100%;
  height: 200px;
  background: linear-gradient(135deg, $color-bg-tertiary, $color-bg-secondary);
  overflow: hidden;
  position: relative;

  img {
    width: 100%;
    height: 100%;
    object-fit: cover;
    transition: transform $transition-base;
  }

  .product-card:hover & img {
    transform: scale(1.05);
  }

  @include compact {
    height: 180px;
  }
}

.image-placeholder {
  @include flex-center;
  height: 100%;
  font-size: 48px;
  opacity: 0.3;
}

.product-content {
  padding: $spacing-md;
}

.product-title {
  font-size: 18px;
  font-weight: 600;
  color: $color-text-primary;
  margin-bottom: $spacing-xs;
  @include text-ellipsis(1);

  @include compact {
    font-size: 16px;
  }
}

.product-summary {
  font-size: 14px;
  color: $color-text-secondary;
  margin-bottom: $spacing-md;
  @include text-ellipsis(2);
}

.product-footer {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.product-price {
  .currency {
    font-size: 14px;
    color: $color-accent;
  }

  .amount {
    font-size: 20px;
    font-weight: 700;
    color: $color-accent;
  }

  .unit {
    font-size: 12px;
    color: $color-text-tertiary;
  }
}

.add-btn {
  width: 36px;
  height: 36px;
  border-radius: 50%;
  background: $color-primary;
  color: white;
  border: none;
  font-size: 20px;
  cursor: pointer;
  transition: all $transition-fast;

  &:hover {
    background: $color-primary-light;
    transform: scale(1.1);
  }
}
</style>
