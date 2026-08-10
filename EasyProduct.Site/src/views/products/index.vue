<!-- src/views/products/index.vue -->
<template>
  <div class="products-page">
    <h1 class="products-page__title">
      {{ t('site.products.title') }}
    </h1>
    <ProductFilter
      :categories="categories"
      :selected-category-id="query.categoryId"
      @select="handleCategorySelect"
    />
    <template v-if="loading">
      <AppLoading :loading="loading" />
    </template>
    <template v-else>
      <AppEmpty v-if="!products.length" />
      <ProductGrid
        v-else
        :products="products"
      />
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
import ProductFilter from '@/components/product/ProductFilter.vue'
import ProductGrid from '@/components/product/ProductGrid.vue'
import AppLoading from '@/components/common/AppLoading.vue'
import AppEmpty from '@/components/common/AppEmpty.vue'
import AppPagination from '@/components/common/AppPagination.vue'
import { getCategoryList } from '@/api/site/category'
import { getProductList } from '@/api/site/product'
import type { ProductCategory, Product } from '@/types/site'

const { t } = useI18n()

const categories = ref<ProductCategory[]>([])
const products = ref<Product[]>([])
const loading = ref(false)
const total = ref(0)
const query = ref({ pageIndex: 1, pageSize: 10, categoryId: undefined as string | undefined })

onMounted(async () => {
  categories.value = await getCategoryList()
  await loadProducts()
})

async function loadProducts() {
  loading.value = true
  try {
    const result = await getProductList(query.value)
    products.value = result.list
    total.value = result.total
  } finally {
    loading.value = false
  }
}

function handleCategorySelect(categoryId?: string) {
  query.value.categoryId = categoryId
  query.value.pageIndex = 1
  loadProducts()
}

function handlePageChange(page: number) {
  query.value.pageIndex = page
  loadProducts()
}
</script>

<style scoped lang="scss">
.products-page {
  @include site-container;
  padding-top: $spacing-lg;
  padding-bottom: $spacing-xl;

  &__title {
    margin-bottom: $spacing-lg;
  }
}
</style>