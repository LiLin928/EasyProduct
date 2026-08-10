<!-- src/components/product/ProductFilter.vue -->
<template>
  <div class="product-filter">
    <div class="product-filter__category">
      <button
        class="product-filter__btn"
        :class="{ 'product-filter__btn--active': !selectedCategoryId }"
        @click="$emit('select', undefined)"
      >
        {{ t('site.products.allCategories') }}
      </button>
      <button
        v-for="cat in categories"
        :key="cat.id"
        class="product-filter__btn"
        :class="{ 'product-filter__btn--active': selectedCategoryId === cat.id }"
        @click="$emit('select', cat.id)"
      >
        {{ locale === 'zh-CN' ? cat.name : cat.nameEn }}
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useI18n } from 'vue-i18n'
import type { ProductCategory } from '@/types/site'

const { t, locale } = useI18n()

defineProps<{
  categories: ProductCategory[]
  selectedCategoryId?: string
}>()

defineEmits<{ select: [categoryId: string | undefined] }>()
</script>

<style scoped lang="scss">
.product-filter {
  margin-bottom: $spacing-lg;

  &__category {
    display: flex;
    flex-wrap: wrap;
    gap: $spacing-sm;
  }

  &__btn {
    padding: $spacing-xs $spacing-md;
    border: 1px solid $color-border;
    border-radius: $radius-sm;
    background: $color-bg;
    cursor: pointer;
    transition: all 0.3s;

    &--active {
      border-color: $color-primary;
      color: $color-primary;
    }

    &:not(&--active):hover {
      border-color: $color-primary;
      opacity: 0.8;
    }
  }
}
</style>