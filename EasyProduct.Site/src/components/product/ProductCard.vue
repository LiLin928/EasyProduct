<!-- src/components/product/ProductCard.vue -->
<template>
  <router-link
    :to="`/products/${product.id}`"
    class="product-card"
  >
    <img
      :src="product.coverImage"
      :alt="productName"
      class="product-card__image"
    >
    <div class="product-card__content">
      <h3 class="product-card__title">
        {{ productName }}
      </h3>
      <p class="product-card__summary">
        {{ product.summary }}
      </p>
      <div class="product-card__footer">
        <span class="product-card__price">¥{{ product.price.toFixed(2) }}</span>
        <span class="product-card__unit">/{{ product.unit }}</span>
      </div>
    </div>
  </router-link>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { useI18n } from 'vue-i18n'
import type { Product } from '@/types/site'

const { locale } = useI18n()

const props = defineProps<{ product: Product }>()

const productName = computed(() =>
  locale.value === 'zh-CN' ? props.product.name : props.product.nameEn
)
</script>

<style scoped lang="scss">
.product-card {
  display: block;
  border-radius: $radius-md;
  background: $color-bg-card;
  overflow: hidden;
  transition: box-shadow 0.3s;

  &:hover {
    box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
  }

  &__image {
    width: 100%;
    height: 200px;
    object-fit: cover;
  }

  &__content {
    padding: $spacing-md;
  }

  &__title {
    margin: 0 0 $spacing-xs;
    font-size: $font-size-lg;
    color: $color-text;
    @include ellipsis(2);
  }

  &__summary {
    margin: 0 0 $spacing-sm;
    color: $color-text-secondary;
    font-size: $font-size-sm;
    @include ellipsis(2);
  }

  &__footer {
    display: flex;
    align-items: baseline;
    gap: $spacing-xs;
  }

  &__price {
    font-size: 18px;
    font-weight: 600;
    color: $color-primary;
  }

  &__unit {
    font-size: $font-size-sm;
    color: $color-text-secondary;
  }
}
</style>