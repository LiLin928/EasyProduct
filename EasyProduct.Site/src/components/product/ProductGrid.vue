<!-- src/components/product/ProductGrid.vue -->
<template>
  <div class="product-grid">
    <ProductCard
      v-for="product in products"
      :key="product.id"
      :product="product"
    />
  </div>
</template>

<script setup lang="ts">
import ProductCard from './ProductCard.vue'
import type { Product } from '@/types/site'

defineProps<{ products: Product[] }>()
</script>

<style scoped lang="scss">
.product-grid {
  display: grid;
  gap: $spacing-lg;

  @include mobile {
    grid-template-columns: 1fr;
  }

  @include tablet {
    grid-template-columns: repeat(2, 1fr);
  }

  @include desktop {
    grid-template-columns: repeat(4, 1fr);
  }
}
</style>